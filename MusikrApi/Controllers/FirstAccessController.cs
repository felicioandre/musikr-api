using MusikrApi.Core;
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
using System.Web;
using System.IO;

namespace MusikrApi.Controllers
{
    [RoutePrefix("primeiro-acesso")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class FirstAccessController : ApiController
    {
        private IRepositorioUsuario RepositorioUsuario
        {
            get; set;
        }

        public FirstAccessController()
        {
            RepositorioUsuario = new RepositorioUsuario(new db_musikr_andreEntities());
        }

        [Route("step01")]
        [HttpPost]
        public IHttpActionResult Step01(Step01Dto dados)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = RepositorioUsuario.BuscarUsuarioPorId(User.Identity.GetUserId<int>());

            if(user.FirstStep != 1)
            {
                return BadRequest();
            }

            user.DataNascimento = dados.DataNascimento;
            user.Sexo = dados.Sexo;
            user.FirstStep = 2;
            user.EhVocalista = false;

            if(user.FotoPerfil == null)
            {
                user.FotoPerfil = Constantes.Servidor.URL + "Content/user-default.jpg";
            }

            RepositorioUsuario.SalvarUsuario(user);

            if (!string.IsNullOrWhiteSpace(dados.FotoBase64))
            {
                var folder = HttpContext.Current.Server.MapPath("~/") + Constantes.Servidor.UPLOADFOTO;
                var fileName = user.Id + ".jpeg";
                var arquivoApi = folder + fileName;
                var caminhoFoto = Constantes.Servidor.URL + Constantes.Servidor.UPLOADFOTO + fileName;

                var bytes = Convert.FromBase64String(dados.FotoBase64);
                using (var imageFile = new FileStream(arquivoApi, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

                user.FotoPerfil = caminhoFoto + "?t=" + DateTime.Now.Ticks;

                RepositorioUsuario.SalvarUsuario(user);
            }

            return Ok(new { fotoPerfil = user.FotoPerfil });
        }

        [Route("step02")]
        [HttpPost]
        public IHttpActionResult Step02(Step02Dto dados)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = RepositorioUsuario.BuscarUsuarioPorId(User.Identity.GetUserId<int>());

            if (user.FirstStep != 2)
            {
                return BadRequest();
            }

            RepositorioUsuario.RemoverGenerosMusicais(user);

            foreach (var genero in dados.GeneroMusical)
            {
                user.UsuarioGeneroMusical.Add(new UsuarioGeneroMusical { GeneroMusicalFk = genero });
            }

            user.FirstStep = 3;

            RepositorioUsuario.SalvarUsuario(user);

            return Ok();
        }

        [Route("step03")]
        [HttpPost]
        public IHttpActionResult Step03(Step03Dto dados)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = RepositorioUsuario.BuscarUsuarioPorId(User.Identity.GetUserId<int>());

            if (user.FirstStep != 3)
            {
                return BadRequest();
            }

            RepositorioUsuario.RemoverInstrumentos(user);

            foreach (var id in dados.Instrumento)
            {
                user.UsuarioInstrumento.Add(new UsuarioInstrumento { InstrumentoFk = id });
            }

            user.EhVocalista = dados.Cantor;
            user.FirstStep = 4;

            RepositorioUsuario.SalvarUsuario(user);

            return Ok();
        }

        [Route("avancar-step")]
        [HttpPost]
        public IHttpActionResult SkipStep()
        {
            var user = RepositorioUsuario.BuscarUsuarioPorId(User.Identity.GetUserId<int>());
            
            user.FirstStep++;

            RepositorioUsuario.SalvarUsuario(user);

            return Ok();
        }


    }
}
