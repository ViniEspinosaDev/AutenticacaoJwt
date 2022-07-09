using Autenticacao.Domain.Enums;
using Autenticacao.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace Autenticacao.Api.Extensions
{
    public class AspNetUser : IUsuario
    {
        private readonly IHttpContextAccessor _accessor;

        public AspNetUser(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public Guid UsuarioId => EstaAutenticado() ? Guid.Parse(_accessor.HttpContext.User.RecuperarIdUsuario()) : Guid.Empty;

        public string Nome => EstaAutenticado() ? _accessor.HttpContext.User.RecuperarNomeUsuario() : "";

        public string Email => EstaAutenticado() ? _accessor.HttpContext.User.RecuperarEmailUsuario() : "";

        public ETipoUsuario ETipoUsuario => EstaAutenticado() ? _accessor.HttpContext.User.RecuperarTipoUsuario() : ETipoUsuario.Nenhum;

        public bool EstaAutenticado()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }
    }

    public static class ClaimsPrincipalExtensions
    {
        public static string RecuperarIdUsuario(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentException(nameof(principal));
            }

            var claim = principal.FindFirst(ClaimTypeExtension.UsuarioId);
            return claim?.Value;
        }

        public static string RecuperarNomeUsuario(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentException(nameof(principal));
            }

            var claim = principal.FindFirst(ClaimTypeExtension.Nome);
            return claim?.Value;
        }

        public static string RecuperarEmailUsuario(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentException(nameof(principal));
            }

            var claim = principal.FindFirst(ClaimTypeExtension.Email);
            return claim?.Value;
        }

        public static ETipoUsuario RecuperarTipoUsuario(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentException(nameof(principal));
            }

            var claim = principal.FindFirst(ClaimTypeExtension.TipoUsuario);

            if (string.IsNullOrEmpty(claim?.Value))
                return ETipoUsuario.Nenhum;

            Enum.TryParse(claim.Value, out ETipoUsuario tipoUsuario);

            return tipoUsuario;
        }
    }
}
