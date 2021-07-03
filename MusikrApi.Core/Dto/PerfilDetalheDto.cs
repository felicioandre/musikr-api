using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Dto
{
    public class PerfilDetalheDto
    {

        public PerfilDetalheDto()
        {
            Instrumentos = new List<string>();
            GenerosMusicais = new List<string>();
            Bandas = new List<PerfilBandaDetalheDto>();
            Publicacoes = new List<DetalhePublicacaoDto>();
        }

        public int TotalSeguidores { get; set; }
        public int TotalSeguindo { get; set; }
        public string FotoPerfil { get; set; }
        public DateTime CriadoEm { get; set; }
        public string CriadoEmFormatado { get { return CriadoEm.ToString("dd/MM/yyyy"); } }
        //public string NomeBanda { get; set; }
        public bool EhVocalista { get; set; }
        public bool isUserProfile { get; set; }
        public bool isUserFollowing { get; set; }        
        public List<string> Instrumentos { get; set; }
        public List<string> GenerosMusicais { get; set; }
        public List<PerfilBandaDetalheDto> Bandas { get; set; }
        public List<DetalhePublicacaoDto> Publicacoes { get; set; }
    }

}

