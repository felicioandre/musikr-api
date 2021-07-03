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
    [RoutePrefix("instrumento")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class InstrumentoController : ApiController
    {
        private IRepositorioInstrumento RepositorioInstrumento
        {
            get; set;
        }

        public InstrumentoController()
        {
            RepositorioInstrumento = new RepositorioInstrumento(new db_musikr_andreEntities());
        }

        [Route("lista-instrumento")]
        [HttpGet]
        public IHttpActionResult ListaInstrumento()
        {
            var lista = RepositorioInstrumento.ListaInstrumentos();

            return Ok(lista.Select(p => new { text = p.Nome, value = p.InstrumentoId, @checked = false }));
        }
    }
}
