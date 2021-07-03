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
    public class RepositorioGeneroMusical : IRepositorioGeneroMusical
    {

        private db_musikr_andreEntities Context { get; set; }

        public RepositorioGeneroMusical(db_musikr_andreEntities context)
        {
            Context = context;
        }

        public List<GeneroMusical> ListarGenerosMusicais()
        {
            return Context.GeneroMusical
                .OrderBy(p => p.Nome)
                .ToList();
        }

    }
}
