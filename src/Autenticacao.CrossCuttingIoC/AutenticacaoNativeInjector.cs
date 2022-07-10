using Autenticacao.CrossCuttingIoC.Extensions;
using Autenticacao.Data.Context;
using Autenticacao.Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Autenticacao.CrossCuttingIoC
{
    public class AutenticacaoNativeInjector
    {
        public static void ConfigurarDependencias(IServiceCollection services, IConfiguration configuration)
        {
            ConfigurarDependenciasBancoDados(services, configuration);
            ConfigurarDependenciasJwtToken(services, configuration);
        }

        private static void ConfigurarDependenciasJwtToken(IServiceCollection services, IConfiguration configuration)
        {
            var configuracapAplicacao = configuration.GetSection("AppSettings");
            services.Configure<ConfiguracaoAplicacao>(configuracapAplicacao);

            var appSettings = configuracapAplicacao.Get<ConfiguracaoAplicacao>();
            var key = Encoding.ASCII.GetBytes(appSettings.Segredo);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = appSettings.ValidoEm,
                    ValidIssuer = appSettings.Emissor
                };
            });
        }

        private static void ConfigurarDependenciasBancoDados(IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContext<AutenticacaoDbContext>(options => options.UseInMemoryDatabase("database"));
            services.AddDbContext<AutenticacaoDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("SqlConnection")));

            services
                .AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AutenticacaoDbContext>()
                .AddErrorDescriber<IdentityMensagensPortugues>()
                .AddDefaultTokenProviders();
        }
    }
}
