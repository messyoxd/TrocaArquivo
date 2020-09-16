using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TrocaArquivo.Classes;

namespace TrocaArquivo.GUI
{
    public class PlayerButton : Component
    {
        #region Fields
        protected MouseState _currentMouse;
        protected SpriteFont _font;
        protected bool _isHovering;
        protected MouseState _previousMouse;
        public Texture2D _texture;
        public float _rotation;
        #endregion
        #region Properties
        public event EventHandler Click;
        public bool Clicked { get; private set; }
        public Color PenColour { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X - (_texture.Width / 2), (int)Position.Y - (_texture.Height / 2), _texture.Width, _texture.Height);
            }
        }
        public string Text { get; set; }
        public Vector2 Origin;
        public DispositivoUsuario _dispositivo { get; set; }
        #endregion
        #region Methods
        public PlayerButton(Texture2D texture, SpriteFont font, DispositivoUsuario dispositivo)
        {
            _texture = texture;
            _font = font;
            _dispositivo = dispositivo;
            PenColour = Color.White;
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var colour = Color.Transparent;

            if (_isHovering)
                colour = Color.Gold;
            spriteBatch.Draw(_texture, Position, null, colour, _rotation, Origin, 1, SpriteEffects.None, 0f);
            if (!string.IsNullOrEmpty(Text))
            {
                var aux = Text + $"({Position.X.ToString()},{Position.Y.ToString()})";
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(aux).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(aux).Y / 2);
                spriteBatch.DrawString(_font, aux, new Vector2(x, y), PenColour);
            }
        }
        public override void Update(GameTime gameTime)
        {
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();
            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);
            var myRectangle = new Rectangle(_dispositivo.X, _dispositivo.Y, 50, 50);
            _isHovering = false;
            if (mouseRectangle.Intersects(Rectangle))
            {
                _isHovering = true;
                // if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                if (_currentMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                    // if (Click != null)
                    // {
                    //     Click(this, new EventArgs());
                    // }
                }
            }
        }
        #endregion
    }
}