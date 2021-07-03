using Microsoft.AspNet.Identity;
using MusikrApi.Core;
using MusikrApi.Core.Database;
using MusikrApi.Core.Dto;
using MusikrApi.Core.Repositorio;
using MusikrApi.Core.Repositorio.Impl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MusikrApi.Controllers
{
    [RoutePrefix("banda")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class BandaController : ApiController
    {

        private IRepositorioBanda RepositorioBanda
        {
            get; set;
        }

        private IRepositorioUsuario RepositorioUsuario
        {
            get; set;
        }

        private IRepositorioGeneroMusical RepositorioGeneroMusical
        {
            get; set;
        }

        public BandaController()
        {
            RepositorioBanda = new RepositorioBanda(new db_musikr_andreEntities());
            RepositorioUsuario = new RepositorioUsuario(new db_musikr_andreEntities());
            RepositorioGeneroMusical = new RepositorioGeneroMusical(new db_musikr_andreEntities());
        }

        [Route("criar/{id:int}")]
        [HttpPost]
        public IHttpActionResult Criar(CriarBandaDto dados, int id)
        {

            if (dados.GeneroMusical == null || !dados.GeneroMusical.Any())
            {
                ModelState.AddModelError("", "Você precisa escolher ao menos um gênero musical.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Banda banda;

            var userid = User.Identity.GetUserId<int>();

            if (id == 0)
            {
                banda = new Banda
                {
                    Ativo = true,
                    CriadoEm = DateTime.Now,
                    Logotipo = Constantes.Servidor.URL + "Content/banda-default.jpg"
                };

                banda.BandaUsuario.Add(new BandaUsuario { UsuarioFK = userid, isAdmin = true });
                banda.BandaFollow.Add(new BandaFollow { UsuarioFK = userid });
            }
            else
            {
                banda = RepositorioBanda.BuscarBandaPorIdParaAdmin(id, userid);
                RepositorioBanda.RemoverGenerosMusicais(banda);
            }

            banda.Nome = dados.NomeBanda;

            foreach (var genero in dados.GeneroMusical)
            {
                banda.BandaGeneroMusical.Add(new BandaGeneroMusical { GeneroMusicalFK = genero });
            }

            RepositorioBanda.SalvarBanda(banda);



            if (!string.IsNullOrWhiteSpace(dados.LogoBase64))
            {
                var folder = HttpContext.Current.Server.MapPath("~/") + Constantes.Servidor.UPLOADFOTO;
                var fileName = "logoBanda" + banda.BandaId + ".jpeg";
                var arquivoApi = folder + fileName;
                var caminhoFoto = Constantes.Servidor.URL + Constantes.Servidor.UPLOADFOTO + fileName;

                var bytes = Convert.FromBase64String(dados.LogoBase64);
                using (var imageFile = new FileStream(arquivoApi, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

                banda.Logotipo = caminhoFoto + "?t=" + DateTime.Now.Ticks;

                RepositorioBanda.SalvarBanda(banda);
            }

            if (!string.IsNullOrWhiteSpace(dados.FotoBase64))
            {
                var folder = HttpContext.Current.Server.MapPath("~/") + Constantes.Servidor.UPLOADFOTO;
                var fileName = "fotoBanda" + banda.BandaId + ".jpeg";
                var arquivoApi = folder + fileName;
                var caminhoFoto = Constantes.Servidor.URL + Constantes.Servidor.UPLOADFOTO + fileName;

                var bytes = Convert.FromBase64String(dados.FotoBase64);
                using (var imageFile = new FileStream(arquivoApi, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

                banda.Foto = caminhoFoto + "?t=" + DateTime.Now.Ticks;

                RepositorioBanda.SalvarBanda(banda);
            }

            return Ok();
        }

        [Route("minhas-bandas")]
        [HttpGet]
        public IHttpActionResult MinhasBandas()
        {
            var lista = RepositorioUsuario.BuscarBandasPorUsuario(User.Identity.GetUserId<int>());

            return Ok(lista.Select(p => new { nome = p.Nome, logotipo = p.Logotipo, criadoEm = p.CriadoEm.ToString("dd/MM/yyyy"), id = p.BandaId }));
        }

        [Route("minhas-bandas-admin")]
        [HttpGet]
        public IHttpActionResult MinhasBandasAdmin()
        {
            var lista = RepositorioUsuario.BuscarBandasPorUsuarioAdmin(User.Identity.GetUserId<int>());

            return Ok(lista.Select(p => new { nome = p.Nome, logotipo = p.Logotipo, id = p.BandaId }));
        }

        [Route("lista-genero/{id:int}")]
        [HttpGet]
        public IHttpActionResult ListaGenero(int id)
        {
            var lista = RepositorioBanda.BuscarBandaPorGenero(id);
            return Ok(lista.Select(p => new { nome = p.Nome, logotipo = p.Logotipo, criadoEm = p.CriadoEm.ToString("dd/MM/yyyy"), id = p.BandaId }));
        }

        [Route("detalhe/{id:int}")]
        [HttpGet]
        public IHttpActionResult DetalheBanda(int id)
        {
            var detalheBanda = RepositorioBanda.BuscarDetalheBanda(id, User.Identity.GetUserId<int>());

            return Ok(detalheBanda);
        }

        [Route("deletar/{id:int}")]
        [HttpPost]
        public IHttpActionResult DeletarBanda(int id)
        {
            RepositorioBanda.DeletarBanda(id, User.Identity.GetUserId<int>());
            return Ok();
        }

        [Route("editar/{id:int}")]
        [HttpGet]
        public IHttpActionResult EditarBanda(int id)
        {
            var banda = RepositorioBanda.BuscarBandaPorIdParaAdmin(id, User.Identity.GetUserId<int>());
            var generosMusicais = RepositorioGeneroMusical.ListarGenerosMusicais();

            return Ok(new { foto = banda.Foto, logotipo = banda.Logotipo, nome = banda.Nome, generosMusicais = generosMusicais.Select(p => new { text = p.Nome, value = p.GeneroMusicalId, @checked = banda.BandaGeneroMusical.Any(s => s.GeneroMusicalFK == p.GeneroMusicalId) }) });
        }

        [Route("seguir/{id:int}/{tipo:int}")]
        [HttpPost]
        public IHttpActionResult SeguirBanda(int id, int tipo)
        {
            var banda = RepositorioBanda.BuscarBandaPorId(id);
            var userid = User.Identity.GetUserId<int>();

            if (tipo == 1)
            {

                if (!banda.BandaFollow.Any(p => p.UsuarioFK == userid))
                {
                    banda.BandaFollow.Add(new BandaFollow
                    {
                        UsuarioFK = User.Identity.GetUserId<int>()
                    });

                    RepositorioBanda.SalvarBanda(banda);
                }

            }
            else
            {
                RepositorioBanda.RemoverFollowBanda(id, userid);
            }

            return Ok(new { seguidores = banda.BandaFollow.Count });
        }

        [Route("lista-seguidores/{id:int}")]
        [HttpGet]
        public IHttpActionResult ListaSeguidores(int id)
        {
            var lista = RepositorioBanda.BuscarSeguidores(id);

            return Ok(lista.Select(p => new { nome = p.Nome, id = p.Id, foto = p.FotoPerfil }));
        }

        [Route("lista-integrantes/{id:int}")]
        [HttpGet]
        public IHttpActionResult ListaIntegrantesBanda(int id)
        {
            var lista = RepositorioBanda.BuscarIntegrantesBanda(id);

            return Ok(lista.Select(p => new { NomeIntegrante = p.NomeIntegrante, IdIntegrante = p.IdIntegrante, FotoIntegrante = p.FotoIntegrante, isAdmin = p.isAdmin }));
        }

        [Route("adicionar-integrante/{id:int}")]
        [HttpGet]
        public IHttpActionResult AdicionarIntegrante(int id)
        {
            var lista = RepositorioBanda.BuscarSeguidoresNaoIntegrante(id);

            return Ok(lista.Select(p => new { nome = p.Nome, id = p.Id, foto = p.FotoPerfil }));
        }

        [Route("adicionar-integrante/{id:int}")]
        [HttpPost]
        public IHttpActionResult AdicionarIntegrante(BandaAdicionarIntegranteDto dados, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userid = User.Identity.GetUserId<int>();

            if (!RepositorioBanda.VerificaUsuarioAdminBanda(userid, id))
            {
                ModelState.AddModelError("", "Você não é o administrador dessa banda!");
                return BadRequest(ModelState);
            }

            var banda = RepositorioBanda.BuscarBandaPorId(id);

            if (banda.BandaUsuario.Any(p => p.UsuarioFK == dados.UsuarioId))
            {
                ModelState.AddModelError("", "O Usuário já é um integrante da banda.");
                return BadRequest(ModelState);
            }

            banda.BandaUsuario.Add(new BandaUsuario
            {
                UsuarioFK = dados.UsuarioId,
                isAdmin = dados.IsAdmin
            });

            RepositorioBanda.SalvarBanda(banda);

            return Ok();
        }

        [Route("editar-integrante/{idBanda:int}/{id:int}")]
        [HttpGet]
        public IHttpActionResult EditarIntegrante(int idBanda, int id)
        {
            var bandaintegrante = RepositorioBanda.BuscarBandaIntegrante(id, idBanda);

            if (bandaintegrante == null)
            {
                ModelState.AddModelError("", "O Usuário não é um integrante da banda.");
                return BadRequest(ModelState);
            }

            return Ok(new { isAdmin = bandaintegrante.isAdmin, nome = bandaintegrante.Usuario.Nome, foto = bandaintegrante.Usuario.FotoPerfil });
        }

        [Route("editar-integrante/{idBanda:int}/{id:int}")]
        [HttpPost]
        public IHttpActionResult EditarIntegrante(BandaEditarIntegranteDto dados, int idBanda, int id)
        {

            var userid = User.Identity.GetUserId<int>();

            if (!RepositorioBanda.VerificaUsuarioAdminBanda(userid, idBanda))
            {
                ModelState.AddModelError("", "Você não é o administrador dessa banda!");
                return BadRequest(ModelState);
            }

            var bandaintegrante = RepositorioBanda.BuscarBandaIntegrante(id, idBanda);

            if(bandaintegrante == null)
            {
                ModelState.AddModelError("", "O Usuário não é um integrante da banda.");
                return BadRequest(ModelState);
            }

            bandaintegrante.isAdmin = dados.IsAdmin;
            RepositorioBanda.SalvarBandaIntegrante(bandaintegrante);

            return Ok();
        }

        [Route("remover-integrante/{idBanda:int}/{id:int}")]
        [HttpPost]
        public IHttpActionResult RemoverIntegrante(int idBanda, int id)
        {

            var userid = User.Identity.GetUserId<int>();

            if (!RepositorioBanda.VerificaUsuarioAdminBanda(userid, idBanda))
            {
                ModelState.AddModelError("", "Você não é o administrador dessa banda!");
                return BadRequest(ModelState);
            }

            if (userid == id)
            {
                ModelState.AddModelError("", "Você não pode remover você mesmo da banda!");
                return BadRequest(ModelState);
            }

            var bandaintegrante = RepositorioBanda.BuscarBandaIntegrante(id, idBanda);

            if (bandaintegrante == null)
            {
                ModelState.AddModelError("", "O Usuário não é um integrante da banda.");
                return BadRequest(ModelState);
            }

            RepositorioBanda.RemoverIntegranteBanda(id, idBanda);

            return Ok();
        }

    }
}
