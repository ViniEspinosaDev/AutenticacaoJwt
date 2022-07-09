using Autenticacao.Domain.Enums;
using System;

namespace Autenticacao.Api.Controllers.ViewModel
{
    public class UsuarioViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public ETipoUsuario TipoUsuario { get; set; }
    }
}
