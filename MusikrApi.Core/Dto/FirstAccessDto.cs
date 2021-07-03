using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Dto
{
    public class Step01Dto
    {
        [Required(ErrorMessage = "Você precisa escolher o seu sexo.")]
        public string Sexo { get; set; }
        [Required(ErrorMessage ="Você precisa dizer a data do seu aniversário.")]
        public DateTime? DataNascimento { get; set; }
        public string FotoBase64 { get; set; }
    }

    public class Step02Dto
    {
        public Step02Dto()
        {
            GeneroMusical = new List<int>();
        }

        public List<int> GeneroMusical { get; set; }
    }

    public class Step03Dto
    {
        public Step03Dto()
        {
            Instrumento = new List<int>();
        }

        public bool Cantor { get; set; }
        public List<int> Instrumento { get; set; }
    }
}
