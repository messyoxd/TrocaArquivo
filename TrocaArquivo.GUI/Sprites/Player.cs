using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TrocaArquivo.Classes;
using TrocaArquivo.GUI.Wrappers;

namespace TrocaArquivo.GUI.Sprites
{
    public class Player : PlayerButton
    {
        public float RotationVelocity = 4f;
        public float LinearVelocity = 4f;
        public Input Input;
        public bool IsMine { get; }

        public Player(Texture2D texture, Input input, SpriteFont font, string text, EventHandler clicked, DispositivoUsuario dispositivo, bool isMine, float rotation) : base(texture, font, dispositivo)
        {
            this.IsMine = isMine;
            _rotation = rotation;
            Text = text;
            Click += clicked;
            Position = new Vector2(dispositivo.X, dispositivo.Y);
            // Position = new Vector2(_texture.Width, texture.Height);
            Input = input;
            Origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsMine)
                Move();
            base.Update(gameTime);
        }
        public void Move()
        {
            if (Keyboard.GetState().IsKeyDown(Input.Left))
            {
                // turn left
                _rotation -= MathHelper.ToRadians(RotationVelocity);
            }
            else if (Keyboard.GetState().IsKeyDown(Input.Right))
            {
                // turn right
                _rotation += MathHelper.ToRadians(RotationVelocity);
            }

            var direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - _rotation), -(float)Math.Sin(MathHelper.ToRadians(90) - _rotation));

            if (Keyboard.GetState().IsKeyDown(Input.Up))
            {
                // go ahead
                Position += direction * LinearVelocity;
            }
            else if (Keyboard.GetState().IsKeyDown(Input.Down))
            {
                // go back
                Position -= direction * LinearVelocity;
            }

        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, null, Color.White, _rotation, Origin, 1, SpriteEffects.None, 0f);
            base.Draw(gameTime, spriteBatch);
        }
    }
}