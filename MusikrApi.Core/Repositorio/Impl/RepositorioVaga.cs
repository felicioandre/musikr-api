using MusikrApi.Core.Database;
using MusikrApi.Core.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Repositorio.Impl
{
    public class RepositorioVaga : IRepositorioVaga
    {

        private db_musikr_andreEntities Context { get; set; }

        public RepositorioVaga(db_musikr_andreEntities context)
        {
            Context = context;
        }

        public void SalvarVaga(Vaga dados)
        {
            if (dados.VagaId == 0)
            {
                Context.Vaga.Add(dados);
            }

            Context.SaveChanges();
        }

        public Vaga BuscarVagaPorInstrumentoBanda(int bandaid, int? instrumentoid)
        {
            //return Context.BandaUsuario.Where(x => x.UsuarioFK == id && x.Banda.Ativo).Select(x => x.Banda).ToList();

            return Context.Vaga.Where(p => p.Ativo && p.BandaFK == bandaid && p.InstrumentoFK == instrumentoid).FirstOrDefault();

        }

        public List<ListaVagasDto> BuscarVagaPorBandaDoUsuario(int userid)
        {
            //return Context.Vaga.Where(p => p.Ativo && p.BandaFK == bandaid && p.InstrumentoFK == instrumentoid).FirstOrDefault();

            //return Context.BandaUsuario.Where(x => x.UsuarioFK == id && x.Banda.Ativo).Select(x => x.Banda).ToList();

            return Context.Vaga.Where(x => x.Ativo && Context.BandaUsuario.Where(p => p.UsuarioFK == userid && p.Banda.Ativo).Select(p => p.BandaFK).Contains(x.BandaFK))
                .OrderByDescending(p => p.CriadoEm).Select(p => new ListaVagasDto
                {
                    CriadoEm = p.CriadoEm,
                    Instrumento = p.Instrumento.Nome,
                    NomeBanda = p.Banda.Nome,
                    IdVaga = p.VagaId
                })
                .ToList();
        }

        public DetalheVagaDto BuscarDetalheVaga(int id, int userId)
        {
            return Context.Vaga.Where(x => x.VagaId == id).Select(v => new DetalheVagaDto {
                CriadoEm = v.CriadoEm,
                Titulo = v.Titulo,
                Descricao = v.Descricao,
                NomeBanda = v.Banda.Nome,
                LogoBanda = v.Banda.Logotipo,
                IdBanda = v.Banda.BandaId,
                isUserAdmin = v.Banda.BandaUsuario.Any(p => p.isAdmin && p.UsuarioFK == userId),
                isUserIntegranteBanda = v.Banda.BandaUsuario.Any(p => p.UsuarioFK == userId),
                isUserCandidato = v.VagaCandidato.Any(p => p.UsuarioFK == userId)
            }).FirstOrDefault();
        }

        public void DeletarVaga(int id, int userId)
        {
            var vaga = BuscarVagaPorIdParaAdmin(id, userId);

            if (vaga != null)
            {
                vaga.Ativo = false;
                Context.SaveChanges();
            }
        }

        public Vaga BuscarVagaPorIdParaAdmin(int id, int userId)
        {
            return Context.Vaga.Where(p => p.VagaId == id && p.Banda.BandaUsuario.Any(x => x.isAdmin && x.UsuarioFK == userId)).FirstOrDefault();
        }

        public Vaga BuscarVagaPorId(int id)
        {
            return Context.Vaga.Where(p => p.VagaId == id && p.Ativo).FirstOrDefault();
        }

        public List<ListaVagasDto> BuscarVagasAtivas()
        {
            //return Context.Vaga.Where(p => p.Ativo && p.BandaFK == bandaid && p.InstrumentoFK == instrumentoid).FirstOrDefault();

            //return Context.BandaUsuario.Where(x => x.UsuarioFK == id && x.Banda.Ativo).Select(x => x.Banda).ToList();

            return Context.Vaga.Where(x => x.Ativo)
                .OrderByDescending(p => p.CriadoEm).Select(p => new ListaVagasDto
                {
                    CriadoEm = p.CriadoEm,
                    Instrumento = p.Instrumento.Nome,
                    NomeBanda = p.Banda.Nome,
                    IdVaga = p.VagaId
                })
                .ToList();
        }

        public List<ListaVagasDto> BuscarVagasAtivasPorInstrumento(int? instrumentoid)
        {
            return Context.Vaga.Where(x => x.Ativo && x.InstrumentoFK == instrumentoid)
               .OrderByDescending(p => p.CriadoEm).Select(p => new ListaVagasDto
               {
                   CriadoEm = p.CriadoEm,
                   Instrumento = p.Instrumento.Nome,
                   NomeBanda = p.Banda.Nome,
                   IdVaga = p.VagaId
               })
               .ToList();
        }

        public List<Usuario> BuscarCandidatosPorVaga(int vagaid)
        {
            return Context.VagaCandidato.Where(x => x.VagaFK == vagaid && x.Usuario.Ativo)
               .OrderBy(p => p.Usuario.Nome)
               .Select(x => x.Usuario).ToList();
        }

        public VagaCandidato BuscarCandidatoVaga(int userid, int vagaid)
        {
            return Context.VagaCandidato.Where(p => p.VagaFK == vagaid && p.UsuarioFK == userid).FirstOrDefault();
        }

        public void RemoverCandidatoVaga(int userid, int vagaid)
        {
            var candidatoVaga = Context.VagaCandidato.FirstOrDefault(p => p.VagaFK == vagaid && p.UsuarioFK == userid);
            if (candidatoVaga != null)
            {
                Context.VagaCandidato.Remove(candidatoVaga);
                Context.SaveChanges();
            }
        }
    }
}
