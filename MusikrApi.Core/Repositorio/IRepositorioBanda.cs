using MusikrApi.Core.Database;
using MusikrApi.Core.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Repositorio
{
    public interface IRepositorioBanda
    {
        void SalvarBanda(Banda dados);
        void SalvarBandaIntegrante(BandaUsuario dados);
        List<Banda> BuscarBandaPorGenero(int id);
        BandaDetalheDto BuscarDetalheBanda(int id, int userId);
        void DeletarBanda(int id, int userId);
        Banda BuscarBandaPorIdParaAdmin(int id, int userId);
        Banda BuscarBandaPorId(int id);
        void RemoverGenerosMusicais(Banda banda);
        void RemoverFollowBanda(int bandaid, int userid);
        void RemoverIntegranteBanda(int userid, int bandaid);
        List<Usuario> BuscarSeguidores(int id);
        List<BandaIntegranteDto> BuscarIntegrantesBanda(int id);
        List<Usuario> BuscarSeguidoresNaoIntegrante(int id);
        bool VerificaUsuarioAdminBanda(int userid, int bandaid);
        BandaUsuario BuscarBandaIntegrante(int id, int bandaid);
    }
}
