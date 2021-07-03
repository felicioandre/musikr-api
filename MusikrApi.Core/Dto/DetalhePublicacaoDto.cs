using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Dto
{
    public class DetalhePublicacaoDto
    {
        public string Texto { get; set; }
        public string Video { get; set; }
        public string Imagem { get; set; }
        public string NomePerfil { get; set; }
        public string FotoPerfil { get; set; }
        public bool isBanda { get; set; }
        public bool UsuarioJaCurtiu { get; set; }
        public int IdPerfil { get; set; }
        public int IdPublicacao { get; set; }
        public int QtdLikes { get; set; }
        public DateTime PublicacaoCriadoEm { get; set; }
    }
}
