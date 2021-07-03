using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Dto
{
    public class BandaAdicionarIntegranteDto
    {
        public bool IsAdmin { get; set; }
        [Required(ErrorMessage = "Você precisa escolher um usuário.")]
        public int? UsuarioId { get; set; }
    }

    public class BandaEditarIntegranteDto
    {
        public bool IsAdmin { get; set; }
    }
}
