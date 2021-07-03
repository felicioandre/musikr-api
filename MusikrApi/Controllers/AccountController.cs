using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http.Cors;
using MusikrApi.Core.Repositorio;
using MusikrApi.Core.Repositorio.Impl;
using MusikrApi.Core.Database;
using MusikrApi.Core.Dto;
using Newtonsoft.Json.Linq;
using MusikrApi.Core.Services;
using MusikrApi.Core.Services.Impl;
using System.IO;

namespace MusikrApi.Controllers
{
    [RoutePrefix("account")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AccountController : ApiController
    {

        private IRepositorioUsuario RepositorioUsuario
        {
            get; set;
        }

        private IServiceEncrypt ServiceEncrypt
        {
            get; set;
        }

        private IServiceEmail ServiceEmail
        {
            get; set;
        }

        public AccountController()
        {
            RepositorioUsuario = new RepositorioUsuario(new db_musikr_andreEntities());
            ServiceEncrypt = new ServiceEncrypt();
            ServiceEmail = new ServiceEmail();
        }

        [Route("lista-usuario")]
        [HttpGet]
        public List<Usuario> ListaUsuario()
        {
            return RepositorioUsuario.ListarUsuarios();
        }

        [Route("criar")]
        [HttpPost]
        public IHttpActionResult Criar(UsuarioDto dados)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var usuarioExiste = RepositorioUsuario.BuscarUsuario(dados.Email);
            if (usuarioExiste != null)
            {
                ModelState.AddModelError("", "Este e-mail já existe!");
                return BadRequest(ModelState);
                //return BadRequest("1");
            }

            var user = new Usuario
            {
                Nome = dados.Nome,
                Email = dados.Email,
                Senha = ServiceEncrypt.Encrypt(dados.Senha),
                CriadoEm = DateTime.Now,
                FirstStep = 1,
                Ativo = true
            };

            RepositorioUsuario.SalvarUsuario(user);

            var access = GenerateLocalAccessTokenResponse(user);

            return Ok(access);
        }

        [Route("login")]
        [HttpPost]
        public IHttpActionResult Login(LoginDto dados)
        {
            //throw new Exception();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var usuario = RepositorioUsuario.BuscarUsuario(dados.Email);

            if (usuario != null)
            {
                var senhaEncrypt = ServiceEncrypt.Encrypt(dados.Senha);
                var access = GenerateLocalAccessTokenResponse(usuario);

                if (usuario.Senha == senhaEncrypt)
                {
                    usuario.SenhaTemporaria = null;
                    RepositorioUsuario.SalvarUsuario(usuario);

                    return Ok(new { token = access, step = usuario.FirstStep });
                }
                else if (usuario.SenhaTemporaria == senhaEncrypt)
                {
                    return Ok(new { token = access, trocaSenha = 1 });
                }

            }

            //return Request.CreateResponse<string.(System.Net.HttpStatusCode.BadRequest, "E-mail ou senha inválidos.");
            //            return BadRequest("1"); //1 = email ou senha inválidos
            ModelState.AddModelError("", "E-mail ou senha inválidos.");
            return BadRequest(ModelState);
        }

        [Route("is-alive")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult ValidaToken() //verifica se o token existe e é válido
        {
            var userID = User.Identity.GetUserId<int>();
            var user = RepositorioUsuario.BuscarUsuarioPorId(userID);

            return Ok(new { step = user.FirstStep });
        }

        [Route("esqueci-senha")]
        [HttpPost]
        public IHttpActionResult EsqueciSenha(EsqueciSenhaDto dados)
        {
            //throw new Exception();
            var usuarioDados = RepositorioUsuario.BuscarUsuario(dados.email);

            if (usuarioDados != null)
            {
                var senhaTemp = Guid.NewGuid().ToString().Substring(0, 6);
                usuarioDados.SenhaTemporaria = ServiceEncrypt.Encrypt(senhaTemp);
                RepositorioUsuario.SalvarUsuario(usuarioDados);

                var body = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/RedefinirSenha.html"));
                body = body.Replace("[SENHA]", senhaTemp);

                ServiceEmail.EnviaEmail(dados.email, "MUSIKR - Sua nova senha de acesso!", body);

                return Ok();
            }

            //return Request.CreateResponse<string.(System.Net.HttpStatusCode.BadRequest, "E-mail ou senha inválidos.");
            return BadRequest("1"); //1 = email inválidos
        }

        [Route("redefinir-senha")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult RedefinirSenha(RecuperarSenhaDto dados)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userID = User.Identity.GetUserId<int>();
            var senhaEncrypt = ServiceEncrypt.Encrypt(dados.Senha);

            var user = RepositorioUsuario.BuscarUsuarioPorId(userID);

            user.Senha = senhaEncrypt;
            user.SenhaTemporaria = null;

            RepositorioUsuario.SalvarUsuario(user);

            //var access = GenerateLocalAccessTokenResponse(user.Nome, user.Id);

            return Ok(new { step = user.FirstStep });
        }

        private JObject GenerateLocalAccessTokenResponse(Usuario user)
        {

            var tokenExpiration = TimeSpan.FromDays(90);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, user.Nome));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

            JObject tokenResponse = new JObject(
                                        //new JProperty("Id", id),
                                        new JProperty("userName", user.Nome),
                                        new JProperty("idUser", user.Id.ToString()),
                                        new JProperty("fotoPerfil", user.FotoPerfil),
                                        new JProperty("access_token", accessToken),
                                        new JProperty("token_type", "bearer"),
                                        new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                        new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                        new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
        );

            return tokenResponse;
        }

    }
}
