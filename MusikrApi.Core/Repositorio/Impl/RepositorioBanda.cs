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
    public class RepositorioBanda : IRepositorioBanda
    {

        private db_musikr_andreEntities Context { get; set; }

        public RepositorioBanda(db_musikr_andreEntities context)
        {
            Context = context;
        }

        public void SalvarBanda(Banda dados)
        {
            if (dados.BandaId == 0)
            {
                Context.Banda.Add(dados);
            }

            Context.SaveChanges();
        }

        public void SalvarBandaIntegrante(BandaUsuario dados)
        {
            if (dados.BandaUsuarioId == 0)
            {
                Context.BandaUsuario.Add(dados);
            }

            Context.SaveChanges();
        }

        public List<Banda> BuscarBandaPorGenero(int id)
        {
            //return Context.BandaUsuario.Where(x => x.UsuarioFK == id && x.Banda.Ativo).Select(x => x.Banda).ToList();

            return Context.BandaGeneroMusical.Where(x => x.GeneroMusicalFK == id && x.Banda.Ativo)
                .OrderByDescending(p => p.Banda.CriadoEm)
                .Select(x => x.Banda).ToList();

        }

        public BandaDetalheDto BuscarDetalheBanda(int id, int userId)
        {
            //return Context.BandaUsuario.Where(x => x.UsuarioFK == id && x.Banda.Ativo).Select(x => x.Banda).ToList();

            //return Context.Banda.Where(x => x.BandaId == id && x.Ativo).Select(x => x.Ban)
            return Context.Banda.Where(x => x.BandaId == id).Select(x => new BandaDetalheDto
            {
                TotalSeguidores = x.BandaFollow.Count(),
                isUserAdmin = x.BandaUsuario.Any(p => p.isAdmin && p.UsuarioFK == userId),
                isUserFollowing = x.BandaFollow.Any(p => p.UsuarioFK == userId),
                FotoBanda = x.Foto,
                LogoBanda = x.Logotipo,
                Publicacoes = x.Publicacao.OrderByDescending(p => p.CriadoEm).Select(p => new DetalhePublicacaoDto
                {
                    Imagem = p.Imagem,
                    Texto = p.Texto,
                    Video = p.Video,
                    PublicacaoCriadoEm = p.CriadoEm,
                    FotoPerfil = p.Banda.Logotipo,
                    NomePerfil = p.Banda.Nome,
                    IdPublicacao = p.PublicacaoId,
                    QtdLikes = p.PublicacaoLike.Count(),
                    UsuarioJaCurtiu = p.PublicacaoLike.Any(u => u.UsuarioFK == userId),
                    isBanda = true,
                    IdPerfil = p.Banda.BandaId
                }).ToList(),
                Integrantes = x.BandaUsuario.OrderBy(p => p.Usuario.Nome).Select(p => new BandaIntegranteDto {
                    FotoIntegrante = p.Usuario.FotoPerfil,
                    IdIntegrante = p.Usuario.Id,
                    Instrumentos = p.Usuario.UsuarioInstrumento.Select(z => z.Instrumento.Nome).ToList(),
                    NomeIntegrante = p.Usuario.Nome,
                    isCantor = p.Usuario.EhVocalista
                }).ToList()
            }).FirstOrDefault();
        }

        public void DeletarBanda(int id, int userId)
        {
            var banda = BuscarBandaPorIdParaAdmin(id, userId);

            if (banda != null)
            {
                banda.Ativo = false;
                Context.SaveChanges();
            }
        }

        public Banda BuscarBandaPorIdParaAdmin(int id, int userId)
        {
            return Context.Banda.Where(p => p.BandaId == id && p.BandaUsuario.Any(x => x.isAdmin && x.UsuarioFK == userId)).FirstOrDefault();
        }

        public Banda BuscarBandaPorId(int id)
        {
            return Context.Banda.Where(p => p.BandaId == id).FirstOrDefault();
        }

        public void RemoverGenerosMusicais(Banda banda)
        {

            Context.BandaGeneroMusical.RemoveRange(banda.BandaGeneroMusical);
            Context.SaveChanges();
        }

        public void RemoverFollowBanda(int bandaid, int userid)
        {
            var bandaFollow = Context.BandaFollow.FirstOrDefault(p => p.BandaFK == bandaid && p.UsuarioFK == userid);
            if(bandaFollow != null)
            {
                Context.BandaFollow.Remove(bandaFollow);
                Context.SaveChanges();
            }
           
        }

        public void RemoverIntegranteBanda(int userid, int bandaid)
        {
            var bandaUsuario = Context.BandaUsuario.FirstOrDefault(p => p.BandaFK == bandaid && p.UsuarioFK == userid);
            if (bandaUsuario != null)
            {
                Context.BandaUsuario.Remove(bandaUsuario);
                Context.SaveChanges();
            }

        }

        public List<Usuario> BuscarSeguidores(int id)
        {
            return Context.BandaFollow.Where(p => p.BandaFK == id).Select(p => p.Usuario).OrderBy(p => p.Nome).ToList();
        }

        public List<BandaIntegranteDto> BuscarIntegrantesBanda(int id)
        {
            return Context.BandaUsuario.Where(i => i.BandaFK == id).Select(i => new BandaIntegranteDto
            {
                FotoIntegrante = i.Usuario.FotoPerfil,
                NomeIntegrante = i.Usuario.Nome,
                IdIntegrante = i.Usuario.Id,
                isAdmin = i.isAdmin
            }).OrderBy(i => i.NomeIntegrante).ToList();
        }

        public List<Usuario> BuscarSeguidoresNaoIntegrante(int id)
        {
            return Context.BandaFollow.Where(p => p.BandaFK == id &&
                    !Context.BandaUsuario.Where(z => z.BandaFK == id).Select(z => z.UsuarioFK).Contains(p.UsuarioFK))
                .Select(p => p.Usuario).OrderBy(p => p.Nome).ToList();
        }

        public bool VerificaUsuarioAdminBanda(int userid, int bandaid)
        {
            return Context.BandaUsuario.Any(p => p.UsuarioFK == userid && p.isAdmin && p.BandaFK == bandaid);
        }

        public BandaUsuario BuscarBandaIntegrante(int id, int bandaid)
        {
            return Context.BandaUsuario.Where(p => p.BandaFK == bandaid && p.UsuarioFK == id).FirstOrDefault();
        }
    }
}
