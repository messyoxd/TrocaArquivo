namespace TrocaArquivo.Classes
{
    public class AmbienteUsuario
    {
        public string Nome { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Sprite { get; set; }
        public AmbienteUsuario(string Nome, int X, int Y, int Sprite)
        {
            this.Sprite = Sprite;
            this.Y = Y;
            this.X = X;
            this.Nome = Nome;
        }
    }
}