using Autenticacao.Domain.Enums;
using System;

namespace Autenticacao.Domain.Interfaces
{
    public interface IUsuario
    {
        Guid UsuarioId { get; }
        string Nome { get; }
        string Email { get; }
        ETipoUsuario ETipoUsuario { get; }
        bool EstaAutenticado();
    }
}
