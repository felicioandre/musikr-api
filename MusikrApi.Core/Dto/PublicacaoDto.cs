using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Dto
{
    public class PublicacaoDto
    {
        public string Texto { get; set; }
        public string Video { get; set; }
        public string ImagemBase64 { get; set; }
        public int TipoPublicacao { get; set; }
        public int BandaId { get; set; }
    }
}
