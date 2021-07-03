using MusikrApi.Core.Database;
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
    [RoutePrefix("genero-musical")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class GeneroMusicalController : ApiController
    {
        private IRepositorioGeneroMusical RepositorioGeneroMusical
        {
            get; set;
        }

        public GeneroMusicalController()
        {
            RepositorioGeneroMusical = new RepositorioGeneroMusical(new db_musikr_andreEntities());
        }

        [Route("lista-genero-musical")]
        [HttpGet]
        public IHttpActionResult ListaGeneroMusical()
        {
            var lista = RepositorioGeneroMusical.ListarGenerosMusicais();
            
            return Ok(lista.Select(p => new { text = p.Nome, value = p.GeneroMusicalId, @checked = false }));
        }
    }
}
