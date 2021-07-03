using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Dto
{
    public class EditarGeneroMusicalDto
    {
        public EditarGeneroMusicalDto()
        {
            GeneroMusical = new List<int>();
        }

        public List<int> GeneroMusical { get; set; }
    }

    public class EditarInstrumentoDto
    {
        public EditarInstrumentoDto()
        {
            Instrumento = new List<int>();
        }

        public List<int> Instrumento { get; set; }
        public bool Cantor { get; set; }
    }

    public class EditarPerfilDto
    {
        [Required(ErrorMessage = "Você precisa escolher o seu sexo.")]
        public string Sexo { get; set; }
        [Required(ErrorMessage = "Você precisa dizer a data do seu aniversário.")]
        public DateTime? DataNascimento { get; set; }
        [Required(ErrorMessage = "Você precisa dizer qual seu nome.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Você precisa dizer qual seu e-mail.")]
        public string Email { get; set; }
        public string FotoBase64 { get; set; }
    }
    public class EditarSenhaDto
    {
        [Required(ErrorMessage = "Você precisa digitar a sua senha atual.")]
        public string SenhaAtual { get; set; }
        [Required(ErrorMessage = "Você precisa digitar a sua nova senha.")]
        public string SenhaNova { get; set; }

    }
}
