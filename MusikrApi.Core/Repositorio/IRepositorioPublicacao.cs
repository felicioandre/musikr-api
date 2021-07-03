using MusikrApi.Core.Database;
using MusikrApi.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Repositorio
{
    public interface IRepositorioPublicacao
    {
        void SalvarPublicacao(Publicacao dados);
        List<DetalhePublicacaoDto> BuscarPublicacaoPorUsuario(int id);
        Publicacao BuscarPublicacaoPorId(int id);
        void RemoverLike(int publicacaoid, int usuarioid);
    }
}
