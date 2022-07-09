using Autenticacao.Domain.Enums;
using Autenticacao.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Autenticacao.Api.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        protected readonly IUsuario _usuario;

        protected Guid UsuarioId { get; }
        protected bool UsuarioAutenticado { get; }
        protected bool AcessoAdministrador { get; }

        public MainController(IUsuario usuario)
        {
            _usuario = usuario;

            if (usuario.EstaAutenticado())
            {
                UsuarioId = usuario.UsuarioId;
                UsuarioAutenticado = true;
                AcessoAdministrador = usuario.ETipoUsuario == ETipoUsuario.Administrador;
            }
        }

        protected ActionResult CustomResponse(object result = null)
        {
            return Ok(new
            {
                success = true,
                data = result
            });
        }
    }
}
