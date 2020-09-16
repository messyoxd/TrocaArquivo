namespace TrocaArquivo.Comunicacao
{
    public class Conexao
    {
        public string Nome { get; set; }
        public string Ip { get; set; }
        public string Porta { get; set; }
        public Conexao(string nome, string ip, string porta)
        {
            this.Porta = porta;
            this.Ip = ip;
            this.Nome = nome;
        }
    }
}