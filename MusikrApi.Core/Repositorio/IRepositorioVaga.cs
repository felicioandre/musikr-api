using MusikrApi.Core.Database;
using MusikrApi.Core.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Repositorio
{
    public interface IRepositorioVaga
    {
        void SalvarVaga(Vaga dados);
        Vaga BuscarVagaPorInstrumentoBanda(int bandaid, int? instrumentoid);
        List<ListaVagasDto> BuscarVagaPorBandaDoUsuario(int userid);
        DetalheVagaDto BuscarDetalheVaga(int id, int userId);
        void DeletarVaga(int id, int userId);
        Vaga BuscarVagaPorIdParaAdmin(int id, int userId);
        Vaga BuscarVagaPorId(int id);
        List<ListaVagasDto> BuscarVagasAtivas();
        List<ListaVagasDto> BuscarVagasAtivasPorInstrumento(int? instrumentoid);
        List<Usuario> BuscarCandidatosPorVaga(int vagaid);
        VagaCandidato BuscarCandidatoVaga(int id, int vagaid);
        void RemoverCandidatoVaga(int userid, int vagaid);
    }
}
