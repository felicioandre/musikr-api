using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Dto
{
    public class BandaIntegranteDto
    {
        public string FotoIntegrante { get; set; }
        public string NomeIntegrante { get; set; }
        public int IdIntegrante { get; set; }
        public List<string> Instrumentos { get; set; }
        public bool isAdmin { get; set; }
        public bool isCantor { get; set; }
    }
}
