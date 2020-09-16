namespace TrocaArquivo.Classes
{
    public class DispositivoUsuario
    {
        public string Nome { get; set; }
        public string Ip { get; set; }
        public string Porta { get; set; }
        public string Ambiente { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Sprite { get; set; }
        public int Rotation { get; set; }
        public DispositivoUsuario(string Nome, string Ip, string Porta, int x, int y, int rotation, int sprite = 1, string Ambiente = "")
        {
            this.Rotation = rotation;
            this.Sprite = sprite;
            this.Y = y;
            this.X = x;
            this.Ambiente = Ambiente;
            this.Porta = Porta;
            this.Ip = Ip;
            this.Nome = Nome;
        }
    }
}