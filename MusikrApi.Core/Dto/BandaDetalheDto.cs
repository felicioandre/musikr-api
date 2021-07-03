using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Dto
{
    public class BandaDetalheDto
    {
        public BandaDetalheDto()
        {
            Integrantes = new List<BandaIntegranteDto>();
            Publicacoes = new List<DetalhePublicacaoDto>();
        }

        public int TotalSeguidores { get; set; }
        public string FotoBanda { get; set; }
        public string LogoBanda { get; set; }
        //public string NomeBanda { get; set; }
        public bool isUserAdmin { get; set; }
        public bool isUserFollowing { get; set; }
        public List<BandaIntegranteDto> Integrantes { get; set; }
        public List<DetalhePublicacaoDto> Publicacoes { get; set; }
    }
}

