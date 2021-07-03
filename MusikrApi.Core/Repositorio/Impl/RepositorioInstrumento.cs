using MusikrApi.Core.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Repositorio.Impl
{
    public class RepositorioInstrumento : IRepositorioInstrumento
    {

        private db_musikr_andreEntities Context { get; set; }

        public RepositorioInstrumento(db_musikr_andreEntities context)
        {
            Context = context;
        }

        public List<Instrumento> ListaInstrumentos()
        {
            return Context.Instrumento
                .OrderBy(p => p.Nome)
                .ToList();
        }

    }
}
