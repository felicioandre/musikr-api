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
    public class RepositorioPublicacao : IRepositorioPublicacao
    {

        private db_musikr_andreEntities Context { get; set; }

        public RepositorioPublicacao(db_musikr_andreEntities context)
        {
            Context = context;
        }

        public void SalvarPublicacao(Publicacao dados)
        {
            if (dados.PublicacaoId == 0)
            {
                Context.Publicacao.Add(dados);
            }

            Context.SaveChanges();
        }

        public List<DetalhePublicacaoDto> BuscarPublicacaoPorUsuario(int id)
        {
            //return Context.BandaUsuario.Where(x => x.UsuarioFK == id && x.Banda.Ativo).Select(x => x.Banda).ToList();

            return Context.Publicacao.Where(x => x.Ativo &&
                ((x.UsuarioFK == id || (Context.UsuarioFollow.Where(u => u.UsuarioFK == id).Select(u => u.UsuarioFollowId).Contains(x.UsuarioFK.Value))) ||
                (Context.BandaFollow.Where(b => b.UsuarioFK == id).Select(b => b.BandaFK).Contains(x.BandaFK.Value))))
                .OrderByDescending(p => p.CriadoEm)
                .Select(x => new DetalhePublicacaoDto
                {
                    FotoPerfil = x.UsuarioFK.HasValue ? x.Usuario.FotoPerfil : x.Banda.Logotipo,
                    Imagem = x.Imagem,
                    Video = x.Video,
                    Texto = x.Texto,
                    IdPublicacao = x.PublicacaoId,
                    QtdLikes = x.PublicacaoLike.Count(),
                    UsuarioJaCurtiu = x.PublicacaoLike.Any(p => p.UsuarioFK == id),
                    PublicacaoCriadoEm = x.CriadoEm,
                    NomePerfil = x.BandaFK.HasValue ? x.Banda.Nome : x.Usuario.Nome,
                    isBanda = x.BandaFK.HasValue,
                    IdPerfil = x.BandaFK.HasValue ? x.Banda.BandaId : x.Usuario.Id
                }).ToList();

        }

        public Publicacao BuscarPublicacaoPorId(int id)
        {
            return Context.Publicacao.Where(x => x.PublicacaoId == id && x.Ativo).FirstOrDefault();
        }

        public void RemoverLike(int publicacaoid, int usuarioid)
        {
            var publicacaolike = Context.PublicacaoLike.FirstOrDefault(p => p.UsuarioFK == usuarioid && p.PublicacaoFK == publicacaoid);
            if (publicacaolike != null)
            {
                Context.PublicacaoLike.Remove(publicacaolike);
                Context.SaveChanges();
            }

        }
    }
}
