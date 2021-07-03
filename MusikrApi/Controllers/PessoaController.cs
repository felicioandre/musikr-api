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
    [RoutePrefix("pessoa")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class PessoaController : ApiController
    {

        private IRepositorioUsuario RepositorioUsuario
        {
            get; set;
        }

        private IRepositorioGeneroMusical RepositorioGeneroMusical
        {
            get; set;
        }

        public PessoaController()
        {
            RepositorioUsuario = new RepositorioUsuario(new db_musikr_andreEntities());
            RepositorioGeneroMusical = new RepositorioGeneroMusical(new db_musikr_andreEntities());
        }

        [Route("lista-genero/{id:int}")]
        [HttpGet]
        public IHttpActionResult ListaGenero(int id)
        {
            var userid = User.Identity.GetUserId<int>();
            var lista = RepositorioUsuario.BuscarUsuarioPorGenero(id, userid);
            return Ok(lista.Select(p => new {
                nome = p.Nome,
                id = p.Id,
                foto = p.FotoPerfil,
                criadoEm = p.CriadoEm.ToString("dd/MM/yyyy"),
                isCantor = p.EhVocalista,
                instrumentos = p.UsuarioInstrumento.Select(i => i.Instrumento.Nome)
            }));
        }

        [Route("lista-instrumento/{id:int}")]
        [HttpGet]
        public IHttpActionResult ListaInstrumento(int id)
        {
            var userid = User.Identity.GetUserId<int>();

            List<Usuario>lista;

            if (id == 0)
            {
                lista = RepositorioUsuario.BuscarUsuarioVocalista(userid);
            }
            else
            {
                lista = RepositorioUsuario.BuscarUsuarioPorInstrumento(id, userid);
            }
            
            return Ok(lista.Select(p => new {
                nome = p.Nome,
                id = p.Id,
                foto = p.FotoPerfil,
                criadoEm = p.CriadoEm.ToString("dd/MM/yyyy"),
                generos = p.UsuarioGeneroMusical.Select(i => i.GeneroMusical.Nome)
            }));
        }

        [Route("lista-nome")]
        [HttpPost]
        public IHttpActionResult ListaNome(BuscaNomeDto dados)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userid = User.Identity.GetUserId<int>();
            var lista = RepositorioUsuario.BuscarUsuarioPorNome(dados.Nome, userid);

            return Ok(lista.Select(p => new {
                nome = p.Nome,
                id = p.Id,
                foto = p.FotoPerfil,
                criadoEm = p.CriadoEm.ToString("dd/MM/yyyy"),
                generos = p.UsuarioGeneroMusical.Select(i => i.GeneroMusical.Nome)
            }));
        }

        [Route("seguir/{id:int}/{tipo:int}")]
        [HttpPost]
        public IHttpActionResult SeguirUsuario(int id, int tipo)
        {
            var usuario = RepositorioUsuario.BuscarUsuarioPorId(id);
            var userid = User.Identity.GetUserId<int>();

            if (tipo == 1)
            {

                if (!usuario.UsuarioFollow.Any(p => p.UsuarioFollowedFK == userid))
                {
                    usuario.UsuarioFollow.Add(new UsuarioFollow
                    {
                        UsuarioFollowedFK = User.Identity.GetUserId<int>(),
                        CriadoEm = DateTime.Now
                    });

                    RepositorioUsuario.SalvarUsuario(usuario);
                }

            }
            else
            {
                RepositorioUsuario.RemoverFollowUsuario(id, userid);
            }

            return Ok(new { seguidores = usuario.UsuarioFollow.Count });
        }
    }
}
