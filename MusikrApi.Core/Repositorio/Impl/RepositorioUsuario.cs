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
    public class RepositorioUsuario : IRepositorioUsuario
    {

        private db_musikr_andreEntities Context { get; set; }

        public RepositorioUsuario(db_musikr_andreEntities context)
        {
            Context = context;
        }

        public List<Usuario> ListarUsuarios()
        {
            var lista = Context.Usuario.ToList();

            return lista;
        }

        public void SalvarUsuario(Usuario dados)
        {
            if (dados.Id == 0)
            {
                Context.Usuario.Add(dados);
            }

            Context.SaveChanges();
        }

        public Usuario BuscarUsuario(string email)
        {
            return Context.Usuario.Where(x => x.Email == email && x.Ativo).FirstOrDefault();
        }

        public Usuario BuscarUsuarioEmailSenha(string email, string senha)
        {
            return Context.Usuario.Where(x => x.Email == email && x.Senha == senha && x.Ativo).FirstOrDefault();
        }

        public Usuario BuscarUsuarioPorId(int id)
        {
            return Context.Usuario.Where(x => x.Id == id).FirstOrDefault();
        }

        public void RemoverGenerosMusicais(Usuario user)
        {

            Context.UsuarioGeneroMusical.RemoveRange(user.UsuarioGeneroMusical);
            Context.SaveChanges();
        }

        public void RemoverInstrumentos(Usuario user)
        {
            Context.UsuarioInstrumento.RemoveRange(user.UsuarioInstrumento);
            Context.SaveChanges();
        }

        public List<Banda> BuscarBandasPorUsuario(int id)
        {
            return Context.BandaUsuario.Where(x => x.UsuarioFK == id && x.Banda.Ativo)
                .OrderByDescending(p => p.Banda.CriadoEm)
                .Select(x => x.Banda).ToList();
        }

        public List<Usuario> BuscarUsuarioPorGenero(int id, int userid)
        {
            return Context.UsuarioGeneroMusical.Where(x => x.GeneroMusicalFk == id && x.Usuario.Ativo && x.UsuarioFk != userid)
                .OrderBy(p => p.Usuario.Nome)
                .Select(x => x.Usuario).ToList();

        }

        public List<Usuario> BuscarUsuarioPorInstrumento(int id, int userid)
        {
            return Context.UsuarioInstrumento.Where(x => x.InstrumentoFk == id && x.Usuario.Ativo && x.UsuarioFk != userid)
                .OrderBy(p => p.Usuario.Nome)
                .Select(x => x.Usuario).ToList();

        }
        public List<Usuario> BuscarUsuarioVocalista(int userid)
        {
            return Context.Usuario.Where(x => x.Ativo && x.Id != userid && x.EhVocalista)
                .OrderBy(p => p.Nome)
                .Select(x => x).ToList();

        }

        public List<Usuario> BuscarUsuarioPorNome(string nome, int userid)
        {
            return Context.Usuario.Where(x =>
                x.Nome.ToLower().Contains(nome.ToLower()) &&
                x.Ativo &&
                x.Id != userid &&
                x.FirstStep > 3
            ).OrderBy(p => p.Nome).Select(u => u).ToList();

        }

        public PerfilDetalheDto BuscarDetalheUsuario(int id, int userId)
        {
            var usuario = Context.Usuario.Where(x => x.Id == id).Select(x => new PerfilDetalheDto
            {
                TotalSeguidores = x.UsuarioFollow.Count(),
                TotalSeguindo = x.UsuarioFollow1.Count(),
                isUserFollowing = x.UsuarioFollow.Any(p => p.UsuarioFollowedFK == userId),
                FotoPerfil = x.FotoPerfil,
                CriadoEm = x.CriadoEm,
                EhVocalista = x.EhVocalista,
                isUserProfile = x.Id == userId,
                Instrumentos = x.UsuarioInstrumento.OrderBy(i => i.Instrumento.Nome).Select(i => i.Instrumento.Nome).ToList(),
                GenerosMusicais = x.UsuarioGeneroMusical.OrderBy(g => g.GeneroMusical.Nome).Select(g => g.GeneroMusical.Nome).ToList(),
                Publicacoes = x.Publicacao.OrderByDescending(p => p.CriadoEm).Select(p => new DetalhePublicacaoDto
                {
                    Imagem = p.Imagem,
                    Texto = p.Texto,
                    Video = p.Video,
                    PublicacaoCriadoEm = p.CriadoEm,
                    FotoPerfil = p.Usuario.FotoPerfil,
                    IdPublicacao = p.PublicacaoId,
                    QtdLikes = p.PublicacaoLike.Count(),
                    UsuarioJaCurtiu = p.PublicacaoLike.Any(u => u.UsuarioFK == userId),
                    NomePerfil = p.Usuario.Nome,
                    isBanda = false,
                    IdPerfil = p.Usuario.Id
                }).ToList(),
                Bandas = x.BandaFollow.Select(b => new PerfilBandaDetalheDto
                {
                    BandaLogo = b.Banda.Logotipo,
                    BandaId = b.Banda.BandaId,
                    BandaCriadoEm = b.Banda.CriadoEm,
                    BandaNome = b.Banda.Nome
                }).ToList()
            }).FirstOrDefault();

            return usuario;
        }

        public void RemoverFollowUsuario(int usuarioid, int userseguidorid)
        {
            var usuarioFollow = Context.UsuarioFollow.FirstOrDefault(p => p.UsuarioFK == usuarioid && p.UsuarioFollowedFK == userseguidorid);
            if (usuarioFollow != null)
            {
                Context.UsuarioFollow.Remove(usuarioFollow);
                Context.SaveChanges();
            }

        }

        public List<Banda> BuscarBandasPorUsuarioAdmin(int id)
        {
            var bandas = Context.BandaUsuario.Where(x => x.UsuarioFK == id && x.Banda.Ativo && x.isAdmin)
                .OrderBy(p => p.Banda.Nome)
                .Select(x => x.Banda).ToList();

            return bandas;
        }

        public List<Usuario> BuscarSeguidores(int id)
        {
            return Context.UsuarioFollow.Where(p => p.UsuarioFK == id).Select(p => p.Usuario1).OrderBy(p => p.Nome).ToList();
        }

        public List<Usuario> BuscarSeguindo(int id)
        {
            return Context.UsuarioFollow.Where(p => p.UsuarioFollowedFK == id).Select(p => p.Usuario).OrderBy(p => p.Nome).ToList();
        }
    }
}
