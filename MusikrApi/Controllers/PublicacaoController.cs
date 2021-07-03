using MusikrApi.Core.Database;
using MusikrApi.Core.Repositorio;
using MusikrApi.Core.Repositorio.Impl;
using MusikrApi.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity;
using System.Web;
using MusikrApi.Core;
using System.IO;

namespace MusikrApi.Controllers
{
    [RoutePrefix("publicacao")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class PublicacaoController : ApiController
    {
        private IRepositorioPublicacao RepositorioPublicacao
        {
            get; set;
        }
        private IRepositorioUsuario RepositorioUsuario
        {
            get; set;
        }
        private IRepositorioBanda RepositorioBanda
        {
            get; set;
        }

        public PublicacaoController()
        {
            RepositorioPublicacao = new RepositorioPublicacao(new db_musikr_andreEntities());
            RepositorioBanda = new RepositorioBanda(new db_musikr_andreEntities());
            RepositorioUsuario = new RepositorioUsuario(new db_musikr_andreEntities());
        }

        [Route("lista-follow")]
        [HttpGet]
        public IHttpActionResult ListaFollow()
        {
            var userid = User.Identity.GetUserId<int>();

            var lista = RepositorioPublicacao.BuscarPublicacaoPorUsuario(userid);
            return Ok(lista);
        }

        [Route("criar")]
        [HttpPost]
        public IHttpActionResult Criar(PublicacaoDto dados)
        {

            if (dados.TipoPublicacao == 1 && string.IsNullOrWhiteSpace(dados.Texto))
            {
                ModelState.AddModelError("", "Você precisa digitar uma publicação.");
            }
            else if (dados.TipoPublicacao == 2 && string.IsNullOrWhiteSpace(dados.ImagemBase64))
            {
                ModelState.AddModelError("", "Você precisa escolher uma imagem.");
            }
            else if (dados.TipoPublicacao == 3 && string.IsNullOrWhiteSpace(dados.Video))
            {
                ModelState.AddModelError("", "Você precisa digitar o link do vídeo.");
            }

            if (!string.IsNullOrWhiteSpace(dados.Video))
            {
                if (!dados.Video.Contains("youtube.com"))
                {
                    if (!dados.Video.Contains("youtu.be"))
                    {
                        ModelState.AddModelError("", "O link digitado não parece ser do Youtube.");
                    }
                }

            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var urlVideo = dados.Video;

            if (!string.IsNullOrWhiteSpace(urlVideo))
            {
                var youtubeUrl = new Uri(urlVideo);

                if (urlVideo.Contains("youtube.com"))
                {
                    var querystringValues = HttpUtility.ParseQueryString(youtubeUrl.Query);
                    urlVideo = "https://www.youtube.com/embed/" + querystringValues["v"];
                }
                else if (urlVideo.Contains("youtu.be"))
                {
                    urlVideo = "https://www.youtube.com/embed/" + youtubeUrl.Segments[1];
                }
            }

            var userid = User.Identity.GetUserId<int>();

            Publicacao publicacao = new Publicacao
            {
                Ativo = true,
                CriadoEm = DateTime.Now,
                Texto = dados.Texto,
                Video = urlVideo,
                UsuarioFK = userid
            };

            if (dados.BandaId != 0)
            {
                publicacao.BandaFK = dados.BandaId;
                publicacao.UsuarioFK = null;
            }

            RepositorioPublicacao.SalvarPublicacao(publicacao);

            if (!string.IsNullOrWhiteSpace(dados.ImagemBase64))
            {
                var folder = HttpContext.Current.Server.MapPath("~/") + Constantes.Servidor.UPLOADFOTO;
                var fileName = "publicacao" + publicacao.PublicacaoId + ".jpeg";
                var arquivoApi = folder + fileName;
                var caminhoFoto = Constantes.Servidor.URL + Constantes.Servidor.UPLOADFOTO + fileName;

                var bytes = Convert.FromBase64String(dados.ImagemBase64);
                using (var imageFile = new FileStream(arquivoApi, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

                publicacao.Imagem = caminhoFoto + "?t=" + DateTime.Now.Ticks;

                RepositorioPublicacao.SalvarPublicacao(publicacao);
            }

            return Ok();
        }

        [Route("curtir/{id:int}")]
        [HttpPost]
        public IHttpActionResult Curtir(int id)
        {
            var publicacao = RepositorioPublicacao.BuscarPublicacaoPorId(id);
            var userid = User.Identity.GetUserId<int>();

            if (!publicacao.PublicacaoLike.Any(p => p.UsuarioFK == userid))
            {
                publicacao.PublicacaoLike.Add(new PublicacaoLike
                {
                    UsuarioFK = userid
                });

                RepositorioPublicacao.SalvarPublicacao(publicacao);
            }
            else
            {
                RepositorioPublicacao.RemoverLike(publicacao.PublicacaoId, userid);
            }

            return Ok(new { qtd = publicacao.PublicacaoLike.Count });
        }

    }
}
