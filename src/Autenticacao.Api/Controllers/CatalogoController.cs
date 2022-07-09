using Autenticacao.Api.Controllers.ViewModel;
using Autenticacao.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Autenticacao.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CatalogoController : MainController
    {
        public CatalogoController(IUsuario usuario) : base(usuario)
        {
        }

        [HttpGet("produtos")]
        public List<ProdutoViewModel> Produtos()
        {
            var usuario = _usuario;

            var produtos = new List<ProdutoViewModel>()
            {
                new ProdutoViewModel() { Id = Guid.NewGuid(), Nome = "Sabonete", Preco = 3.40m },
                new ProdutoViewModel() { Id = Guid.NewGuid(), Nome = "Shampoo", Preco = 12.40m },
                new ProdutoViewModel() { Id = Guid.NewGuid(), Nome = "Pasta de dente", Preco = 3.65m },
                new ProdutoViewModel() { Id = Guid.NewGuid(), Nome = "Escova de dente", Preco = 10.50m }
            };

            return produtos;
        }
    }
}
