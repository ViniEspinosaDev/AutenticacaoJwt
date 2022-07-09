namespace Autenticacao.Domain.Models
{
    public class ConfiguracaoAplicacao
    {
        public string Segredo { get; set; }
        public int ExpiracaoHoras { get; set; }
        public string ValidoEm { get; set; }
        public string Emissor { get; set; }
    }
}
