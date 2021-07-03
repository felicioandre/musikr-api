using Microsoft.AspNet.Identity;
using MusikrApi.Core.Database;
using MusikrApi.Core.Dto;
using MusikrApi.Core.Repositorio;
using MusikrApi.Core.Repositorio.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MusikrApi.Controllers
{
    [RoutePrefix("vagas")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class VagasController : ApiController
    {
        private IRepositorioUsuario RepositorioUsuario
        {
            get; set;
        }

        private IRepositorioBanda RepositorioBanda
        {
            get; set;
        }

        private IRepositorioVaga RepositorioVaga
        {
            get; set;
        }

        public VagasController()
        {
            RepositorioUsuario = new RepositorioUsuario(new db_musikr_andreEntities());
            RepositorioBanda = new RepositorioBanda(new db_musikr_andreEntities());
            RepositorioVaga = new RepositorioVaga(new db_musikr_andreEntities());
        }

        [Route("criar/{id:int}")]
        [HttpPost]
        public IHttpActionResult Criar(CriarVagaDto dados, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var instrumento = (dados.Instrumento != 0 ? dados.Instrumento : null);

            var userid = User.Identity.GetUserId<int>();
            var vagaExiste = RepositorioVaga.BuscarVagaPorInstrumentoBanda(dados.Banda.Value, instrumento);
            Vaga vaga;

            if (id == 0)
            {
                if (vagaExiste != null)
                {
                    ModelState.AddModelError("", "Já existe uma vaga dessa banda para este instrumento!");
                    return BadRequest(ModelState);
                }

                vaga = new Vaga
                {
                    Ativo = true,
                    CriadoEm = DateTime.Now,
                    UsuarioFK = userid
                };
            }
            else
            {
                vaga = RepositorioVaga.BuscarVagaPorIdParaAdmin(id, userid);

                if (vaga.InstrumentoFK != dados.Instrumento && vagaExiste != null)
                {
                    ModelState.AddModelError("", "Já existe uma vaga dessa banda para este instrumento!");
                    return BadRequest(ModelState);
                }
                
            }
            
            vaga.Titulo = dados.Titulo;
            vaga.Descricao = dados.Descricao;
            vaga.BandaFK = dados.Banda.Value;
            vaga.InstrumentoFK = instrumento;

            RepositorioVaga.SalvarVaga(vaga);

            return Ok();
        }

        [Route("minhas-vagas")]
        [HttpGet]
        public IHttpActionResult MinhasVagas()
        {
            var lista = RepositorioVaga.BuscarVagaPorBandaDoUsuario(User.Identity.GetUserId<int>());

            return Ok(lista.Select(p => new { nomeBanda = p.NomeBanda, instrumento = (string.IsNullOrEmpty(p.Instrumento) ? "Vocalista" : p.Instrumento), criadoEm = p.CriadoEm.ToString("dd/MM/yyyy"), vagaId = p.IdVaga }));
        }

        [Route("detalhe/{id:int}")]
        [HttpGet]
        public IHttpActionResult DetalheVaga(int id)
        {
            var detalhevaga = RepositorioVaga.BuscarDetalheVaga(id, User.Identity.GetUserId<int>());
            return Ok(detalhevaga);
        }

        [Route("deletar/{id:int}")]
        [HttpPost]
        public IHttpActionResult DeletarVaga(int id)
        {
            RepositorioVaga.DeletarVaga(id, User.Identity.GetUserId<int>());
            return Ok();
        }

        [Route("editar/{id:int}")]
        [HttpGet]
        public IHttpActionResult EditarVaga(int id)
        {
            var vaga = RepositorioVaga.BuscarVagaPorIdParaAdmin(id, User.Identity.GetUserId<int>());

            return Ok(new { titulo = vaga.Titulo, descricao = vaga.Descricao, banda = vaga.BandaFK, instrumento = vaga.InstrumentoFK });
        }

        [Route("lista-todas/{todas:int}/{id:int}")]
        [HttpGet]
        public IHttpActionResult ListaTodas(int todas, int? id)
        {
            List<ListaVagasDto> lista = null;

            if (todas == 1)
            {
                lista = RepositorioVaga.BuscarVagasAtivas();
            }
            else
            {
                if(id == 0)
                {
                    id = null;
                }
                lista = RepositorioVaga.BuscarVagasAtivasPorInstrumento(id);
            }
            

            return Ok(lista.Select(p => new { nomeBanda = p.NomeBanda, instrumento = (string.IsNullOrEmpty(p.Instrumento) ? "Vocalista" : p.Instrumento), criadoEm = p.CriadoEm.ToString("dd/MM/yyyy"), vagaId = p.IdVaga }));
        }

        [Route("lista-candidatos/{vagaid:int}")]
        [HttpGet]
        public IHttpActionResult ListaCandidatos(int vagaid)
        {
            var lista = RepositorioVaga.BuscarCandidatosPorVaga(vagaid);


            return Ok(lista.Select(p => new {
                nome = p.Nome,
                id = p.Id,
                foto = p.FotoPerfil,
                criadoEm = p.CriadoEm.ToString("dd/MM/yyyy"),
                isCantor = p.EhVocalista,
                generos = p.UsuarioGeneroMusical.Select(i => i.GeneroMusical.Nome)
            }));
        }

        [Route("adicionar-candidato/{id:int}")]
        [HttpPost]
        public IHttpActionResult AdicionarCandidato(int id)
        {
            
            var userid = User.Identity.GetUserId<int>();
           
            var vaga = RepositorioVaga.BuscarVagaPorId(id);

            if (vaga.VagaCandidato.Any(p => p.UsuarioFK == userid))
            {
                ModelState.AddModelError("", "O Usuário já é um candidato a essa vaga.");
                return BadRequest(ModelState);
            }

            vaga.VagaCandidato.Add(new VagaCandidato
            {
                UsuarioFK = userid,
                VagaFK = id
            });

            RepositorioVaga.SalvarVaga(vaga);

            return Ok();
        }

        [Route("remover-candidato/{id:int}")]
        [HttpPost]
        public IHttpActionResult RemoverIntegrante(int id)
        {

            var userid = User.Identity.GetUserId<int>();
            
            var vagaCandidato = RepositorioVaga.BuscarCandidatoVaga(userid, id);

            if (vagaCandidato == null)
            {
                ModelState.AddModelError("", "O Usuário não é um candidato a essa vaga.");
                return BadRequest(ModelState);
            }

            RepositorioVaga.RemoverCandidatoVaga(userid, id);

            return Ok();
        }
    }
}
