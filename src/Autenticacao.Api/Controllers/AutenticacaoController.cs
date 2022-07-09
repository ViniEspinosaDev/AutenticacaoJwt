using Autenticacao.Api.Controllers.InputModel;
using Autenticacao.Api.Controllers.ViewModel;
using Autenticacao.Api.Extensions;
using Autenticacao.Domain.Enums;
using Autenticacao.Domain.Interfaces;
using Autenticacao.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Autenticacao.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AutenticacaoController : MainController
    {
        private readonly ConfiguracaoAplicacao _configuracaoAplicacao;

        public AutenticacaoController(IOptions<ConfiguracaoAplicacao> appSettings, IUsuario usuario) : base(usuario)
        {
            _configuracaoAplicacao = appSettings.Value;
        }

        [HttpPost("nova-conta")]
        public ActionResult NovaConta(NovaContaInputModel novaConta)
        {
            if (novaConta.Senha != novaConta.ConfirmacaoSenha) return new BadRequestResult();

            var usuarioViewModel = new UsuarioViewModel()
            {
                Id = Guid.NewGuid(),
                Nome = novaConta.Nome,
                Email = novaConta.Email,
                TipoUsuario = ETipoUsuario.Administrador
            };

            return CustomResponse(GerarJwt(usuarioViewModel));
        }

        private LoginViewModel GerarJwt(UsuarioViewModel usuario)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypeExtension.UsuarioId, usuario.Id.ToString()));
            claims.Add(new Claim(ClaimTypeExtension.Nome, usuario.Nome));
            claims.Add(new Claim(ClaimTypeExtension.Email, usuario.Email));
            claims.Add(new Claim(ClaimTypeExtension.TipoUsuario, usuario.TipoUsuario.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuracaoAplicacao.Segredo);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _configuracaoAplicacao.Emissor,
                Audience = _configuracaoAplicacao.ValidoEm,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(_configuracaoAplicacao.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandler.WriteToken(token);

            var response = new LoginViewModel
            {
                AccessToken = encodedToken,
                ExpiresIn = TimeSpan.FromHours(_configuracaoAplicacao.ExpiracaoHoras).TotalSeconds,
                UserToken = new UserTokenViewModel
                {
                    Id = usuario.Id.ToString(),
                    Email = usuario.Email,
                    Claims = claims.Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value })
                }
            };

            return response;
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}
