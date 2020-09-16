using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrocaArquivo.GUI.Sprites
{
    public class Ambient : Component
    {
        public Texture2D _texture;
        public Vector2 Position;
        public Vector2 Origin;

        public Ambient(Texture2D texture, int x, int y)
        {
            _texture = texture;
            Position = new Vector2(x, y);
            Origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Color.White);
        }

        public override void Update(GameTime gameTime)
        {

        }
    }
}