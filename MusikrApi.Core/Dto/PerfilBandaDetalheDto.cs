using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Dto
{
    public class PerfilBandaDetalheDto
    {
        public string BandaLogo { get; set; }
        public string BandaNome { get; set; }
        public DateTime BandaCriadoEm { get; set; }
        public string CriadoEmFormatado { get { return BandaCriadoEm.ToString("dd/MM/yyyy"); } }
        public int BandaId { get; set; }

    }
}

