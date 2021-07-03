using MusikrApi.Core.Database;
using MusikrApi.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Repositorio
{
    public interface IRepositorioUsuario
    {
        List<Usuario> ListarUsuarios();
        void SalvarUsuario(Usuario dados);
        Usuario BuscarUsuario(string email);
        Usuario BuscarUsuarioEmailSenha(string email, string senha);
        Usuario BuscarUsuarioPorId(int id);
        void RemoverGenerosMusicais(Usuario user);
        void RemoverInstrumentos(Usuario user);
        List<Banda> BuscarBandasPorUsuario(int id);
        List<Usuario> BuscarUsuarioPorGenero(int id, int userid);
        List<Usuario> BuscarUsuarioPorInstrumento(int id, int userid);
        List<Usuario> BuscarUsuarioVocalista(int userid);
        PerfilDetalheDto BuscarDetalheUsuario(int id, int userId);
        void RemoverFollowUsuario(int usuarioid, int userseguidorid);
        List<Usuario> BuscarUsuarioPorNome(string nome, int userid);
        List<Banda> BuscarBandasPorUsuarioAdmin(int id);
        List<Usuario> BuscarSeguidores(int id);
        List<Usuario> BuscarSeguindo(int id);
    }
}
