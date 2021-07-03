using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Dto
{
    public class BuscaNomeDto
    {
        [Required(ErrorMessage = "Você precisa digitar um nome para efetuar a busca.")]
        public string Nome { get; set; }

    }
}

