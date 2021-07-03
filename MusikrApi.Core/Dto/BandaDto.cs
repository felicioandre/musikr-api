using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Dto
{
    public class CriarBandaDto
    {

        public CriarBandaDto() {
            GeneroMusical = new List<int>();
        }

        public string LogoBase64 { get; set; }
        public string FotoBase64 { get; set; }

        [Required(ErrorMessage = "Você precisa dizer o nome da sua banda.")]
        public string NomeBanda { get; set; }
        public List<int> GeneroMusical { get; set; }
    }
}
