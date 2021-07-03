using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Dto
{
    public class CriarVagaDto
    {

        
        [Required(ErrorMessage = "Você precisa digitar um título para a vaga.")]
        public string Titulo { get; set; }
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Você precisa escolher uma banda.")]
        public int? Banda { get; set; }
        [Required(ErrorMessage = "Você precisa escolher um instrumento.")]
        public int? Instrumento { get; set; }
    }

    public class ListaVagasDto
    {
        public string NomeBanda { get; set; }
        public string Instrumento { get; set; }
        public DateTime CriadoEm { get; set; }
        public int IdVaga { get; set; }
    }

    public class DetalheVagaDto
    {
        public string NomeBanda { get; set; }
        public int IdBanda { get; set; }
        public string LogoBanda { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public DateTime CriadoEm { get; set; }
        public bool isUserIntegranteBanda { get; set; }
        public bool isUserAdmin { get; set; }
        public bool isUserCandidato { get; set; }
        
    }
}
