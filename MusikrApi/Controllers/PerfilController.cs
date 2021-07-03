using Microsoft.AspNet.Identity;
using MusikrApi.Core;
using MusikrApi.Core.Database;
using MusikrApi.Core.Dto;
using MusikrApi.Core.Repositorio;
using MusikrApi.Core.Repositorio.Impl;
using MusikrApi.Core.Services;
using MusikrApi.Core.Services.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MusikrApi.Controllers
{
    [RoutePrefix("perfil")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class PerfilController : ApiController
    {
        private IRepositorioUsuario RepositorioUsuario
        {
            get; set;
        }
        private IRepositorioGeneroMusical RepositorioGeneroMusical
        {
            get; set;
        }
        private IRepositorioInstrumento RepositorioInstrumento
        {
            get; set;
        }

        private IServiceEncrypt ServiceEncrypt
        {
            get; set;
        }

        public PerfilController()
        {
            RepositorioUsuario = new RepositorioUsuario(new db_musikr_andreEntities());
            RepositorioGeneroMusical = new RepositorioGeneroMusical(new db_musikr_andreEntities());
            RepositorioInstrumento = new RepositorioInstrumento(new db_musikr_andreEntities());
            ServiceEncrypt = new ServiceEncrypt();
        }

        [Route("detalhe/{id:int}")]
        [HttpGet]
        public IHttpActionResult Detalhe(int id)
        {
            var userid = User.Identity.GetUserId<int>();
            var detalhePerfil = RepositorioUsuario.BuscarDetalheUsuario(id, userid);

            return Ok(detalhePerfil);
        }

        [Route("editar-generos")]
        [HttpGet]
        public IHttpActionResult EditarGeneros()
        {
            var userid = User.Identity.GetUserId<int>();
            var usuario = RepositorioUsuario.BuscarUsuarioPorId(userid);
            var generosMusicais = RepositorioGeneroMusical.ListarGenerosMusicais();

            //return Ok(new { generosMusicais = generosMusicais.Select(p => new { text = p.Nome, value = p.GeneroMusicalId, @checked = usuario.UsuarioGeneroMusical.Any(s => s.GeneroMusicalFk == p.GeneroMusicalId) }) });
            return Ok( generosMusicais.Select(p => new { text = p.Nome, value = p.GeneroMusicalId, @checked = usuario.UsuarioGeneroMusical.Any(s => s.GeneroMusicalFk == p.GeneroMusicalId) }));
        }

        [Route("editar-generos")]
        [HttpPost]
        public IHttpActionResult EditarGeneros(EditarGeneroMusicalDto dados)
        {
            var userid = User.Identity.GetUserId<int>();
            var usuario = RepositorioUsuario.BuscarUsuarioPorId(userid);

            RepositorioUsuario.RemoverGenerosMusicais(usuario);

            foreach (var genero in dados.GeneroMusical)
            {
                usuario.UsuarioGeneroMusical.Add(new UsuarioGeneroMusical { GeneroMusicalFk = genero });
            }

            RepositorioUsuario.SalvarUsuario(usuario);

            return Ok();
        }

        [Route("editar-instrumentos")]
        [HttpGet]
        public IHttpActionResult EditarInstrumentos()
        {
            var userid = User.Identity.GetUserId<int>();
            var usuario = RepositorioUsuario.BuscarUsuarioPorId(userid);
            var instrumentos = RepositorioInstrumento.ListaInstrumentos();

            //return Ok(new { generosMusicais = generosMusicais.Select(p => new { text = p.Nome, value = p.GeneroMusicalId, @checked = usuario.UsuarioGeneroMusical.Any(s => s.GeneroMusicalFk == p.GeneroMusicalId) }) });
            return Ok(new { ehCantor = usuario.EhVocalista, instrumentos = instrumentos.Select(p => new { text = p.Nome, value = p.InstrumentoId, @checked = usuario.UsuarioInstrumento.Any(s => s.InstrumentoFk == p.InstrumentoId) }) });
        }
        
        [Route("editar-instrumentos")]
        [HttpPost]
        public IHttpActionResult EditarInstrumentos(EditarInstrumentoDto dados)
        {
            var userid = User.Identity.GetUserId<int>();
            var usuario = RepositorioUsuario.BuscarUsuarioPorId(userid);

            RepositorioUsuario.RemoverInstrumentos(usuario);

            foreach (var instrumento in dados.Instrumento)
            {
                usuario.UsuarioInstrumento.Add(new UsuarioInstrumento { InstrumentoFk = instrumento });
            }

            usuario.EhVocalista = dados.Cantor;

            RepositorioUsuario.SalvarUsuario(usuario);

            return Ok();
        }

        [Route("editar")]
        [HttpGet]
        public IHttpActionResult EditarPerfil()
        {
            var userid = User.Identity.GetUserId<int>();
            var usuario = RepositorioUsuario.BuscarUsuarioPorId(userid);

            return Ok(new { sexo = usuario.Sexo, email = usuario.Email, nome = usuario.Nome, foto = usuario.FotoPerfil, aniversario = usuario.DataNascimento });
        }

        [Route("editar")]
        [HttpPost]
        public IHttpActionResult EditarPerfil(EditarPerfilDto dados)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userid = User.Identity.GetUserId<int>();
            var usuario = RepositorioUsuario.BuscarUsuarioPorId(userid);
            var usuarioExiste = RepositorioUsuario.BuscarUsuario(dados.Email);

            if (usuarioExiste != null && usuario.Email != usuarioExiste.Email)
            {
                ModelState.AddModelError("", "Este e-mail já está sendo usado!");
                return BadRequest(ModelState);
            }

            usuario.DataNascimento = dados.DataNascimento;
            usuario.Sexo = dados.Sexo;
            usuario.Nome = dados.Nome;
            usuario.Email = dados.Email;

            RepositorioUsuario.SalvarUsuario(usuario);

            if (!string.IsNullOrWhiteSpace(dados.FotoBase64))
            {
                var folder = HttpContext.Current.Server.MapPath("~/") + Constantes.Servidor.UPLOADFOTO;
                var fileName = usuario.Id + ".jpeg";
                var arquivoApi = folder + fileName;
                var caminhoFoto = Constantes.Servidor.URL + Constantes.Servidor.UPLOADFOTO + fileName;

                var bytes = Convert.FromBase64String(dados.FotoBase64);
                using (var imageFile = new FileStream(arquivoApi, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

                usuario.FotoPerfil = caminhoFoto + "?t=" + DateTime.Now.Ticks;

                RepositorioUsuario.SalvarUsuario(usuario);
            }

            return Ok();
        }


        [Route("editar-senha")]
        [HttpPost]
        public IHttpActionResult EditarSenha(EditarSenhaDto dados)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userid = User.Identity.GetUserId<int>();
            var usuario = RepositorioUsuario.BuscarUsuarioPorId(userid);

            var senhaEncrypt = ServiceEncrypt.Encrypt(dados.SenhaAtual);

            if (usuario.Senha != senhaEncrypt)
            {
                ModelState.AddModelError("", "A senha atual digitada não corresponde com a senha cadastrada. Digite novamente.");
                return BadRequest(ModelState);
            }

            usuario.Senha = ServiceEncrypt.Encrypt(dados.SenhaNova);

            RepositorioUsuario.SalvarUsuario(usuario);

            return Ok();
        }

        [Route("lista-seguidores/{id:int}")]
        [HttpGet]
        public IHttpActionResult ListaSeguidores(int id)
        {
            var lista = RepositorioUsuario.BuscarSeguidores(id);

            return Ok(lista.Select(p => new { nome = p.Nome, id = p.Id, foto = p.FotoPerfil }));
        }

        [Route("lista-seguindo/{id:int}")]
        [HttpGet]
        public IHttpActionResult ListaSeguindo(int id)
        {
            var lista = RepositorioUsuario.BuscarSeguindo(id);

            return Ok(lista.Select(p => new { nome = p.Nome, id = p.Id, foto = p.FotoPerfil }));
        }
    }
}
