using MusikrApi.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Repositorio
{
    public interface IRepositorioGeneroMusical
    {

        List<GeneroMusical> ListarGenerosMusicais();
    }
}
