using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;
using System;
using TrocaArquivo.GUI.Widgets;
using TrocaArquivo.GUI.Sprites;
using TrocaArquivo.Classes;
using TrocaArquivo.Comunicacao;
using TrocaArquivo.GUI.Wrappers;
using Myra.Graphics2D.UI.File;
using System.IO;

namespace TrocaArquivo.GUI
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Desktop _desktop;
        private List<Texture2D> _textures;
        private SpriteFont _font;
        public FileDialog _dialog;
        public bool _dialogOpen = false;
        public List<Component> _players { get; set; }
        public List<Component> _ambients { get; set; }
        public Player _player;
        public DispositivoUsuario _dispositivo;
        private int _width = 800;
        private int _height = 600;
        public int _currentWindowMarker = 0;
        public enum _currentWindow
        {
            Login = 0,
            Game = 1
        }
        public Comunication _comunicacao = null;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _textures = new List<Texture2D>();
            _players = new List<Component>();
            _ambients = new List<Component>();
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            // window name
            Window.Title = "Login";
            // Custom game resolution
            _graphics.PreferredBackBufferWidth = _width; //width
            _graphics.PreferredBackBufferHeight = _height; // height
            _graphics.ApplyChanges();

            // allow window to resize
            Window.AllowUserResizing = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            /* Myra setup */
            MyraEnvironment.Game = this;
            var TelaLogin = new TelaLogin(setWindowTitle, setCurrentWindowMarker, setComunicacao);
            // Add it to the desktop
            _desktop = new Desktop();
            // _desktop.Root = telaPrincipal.ReturnTelaPrincipal(amigos);
            _desktop.Root = TelaLogin.returnTelaPrincipal();
            _desktop.HasExternalTextInput = true;
            // Provide that text input
            Window.TextInput += (s, a) =>
            {
                _desktop.OnChar(a.Character);
            };

            /***************/
            /*load textures*/
            // for (int i = 1; i < 9; i++)
            // {
            //     _textures.Add(Content.Load<Texture2D>($"Textures/{i}"));
            // }
            /***************/
            /*load font*/
            // _font = Content.Load<SpriteFont>("Fonts/Ubuntu32");
            /***********/
        }
        public void setWindowTitle(string text)
        {
            Window.Title = text;
        }
        public void setCurrentWindowMarker(int window)
        {
            _currentWindowMarker = window;
        }
        public void setComunicacao(Comunication comunicacao)
        {
            _comunicacao = comunicacao;
            Random rnd = new Random();
            var x = rnd.Next(100, 500);
            var y = rnd.Next(100, 500);
            _comunicacao.ConectarCom(x, y);
            Console.WriteLine("Conexao efetuada");
            // var ambientSprite = rnd.Next(5, 8);
            _dispositivo = _comunicacao.AdicionarDevice(x, y);
            Console.WriteLine("Dispositivo inserido");

            // setPlayer(
            //     new Player(
            //         Content.Load<Texture2D>($"Textures/{_comunicacao._sprite.ToString()}"),
            //         new Input()
            //         {
            //             Up = Keys.Up,
            //             Down = Keys.Down,
            //             Left = Keys.Left,
            //             Right = Keys.Right
            //         },
            //         Content.Load<SpriteFont>("Fonts/Ubuntu32"),
            //         _dispositivo.Nome,
            //         ClickPlayer,
            //         _dispositivo,
            //         true
            //     ));

            // );
            // _players.Add(_player);
            // _ambients.Add(new Ambient(
            //     Content.Load<Texture2D>($"Textures/5"),
            //     _dispositivo.X,
            //     _dispositivo.Y
            // ));
            // var threadAtualizaDevices = new Thread(() => atualizaListaDispositivos());
            // threadAtualizaDevices.Start();
            // var threadAtualizaAmbientes = new Thread(() => atualizaListaAmbientes());
            // threadAtualizaAmbientes.Start();
        }
        public void setPlayer(Player p)
        {
            _player = p;
        }
        public DispositivoUsuario findDeviceByName(string ambiente, string nome)
        {
            var _currentDevices = _players;
            foreach (var player in _currentDevices)
            {
                if (((Player)player)._dispositivo.Nome == nome && ((Player)player)._dispositivo.Ambiente == ambiente)
                    return ((Player)player)._dispositivo;
            }
            return null;
        }
        public void ClickPlayer(object sender, System.EventArgs e)
        {
            if (_currentWindowMarker == (int)_currentWindow.Game)
            {
                // Console.WriteLine(((PlayerButton)sender).Text);
                if (!_dialogOpen &&
                    (
                    ((PlayerButton)sender)._dispositivo.Nome != _player._dispositivo.Nome &&
                    ((PlayerButton)sender)._dispositivo.Ambiente == _player._dispositivo.Ambiente
                    )
                )
                // if (!_dialogOpen)
                {
                    // pegar o device, caso ele esteja no mesmo ambiente que o dispositivo
                    // do usuario
                    var device = findDeviceByName(_player._dispositivo.Ambiente, ((PlayerButton)sender)._dispositivo.Nome);

                    if (device != null)
                    {

                        _dialog = new FileDialog(FileDialogMode.OpenFile)
                        {
                            Filter = "*",
                        };
                        _dialog.Closed += (s, a) =>
                        {
                            if (!_dialog.Result)
                            {
                                // "Cancel" or Escape
                                _dialogOpen = false;
                                return;
                            }

                            // "Ok" or Enter
                            var filePath = ((FileDialog)s).FilePath;
                            FileStream fileStream = new FileStream(filePath, FileMode.Open);
                            var bytes = File.ReadAllBytes(filePath);

                            // Console.WriteLine();

                            _comunicacao.MandarArquivo(filePath.Split("/").Last(), bytes, device);

                            _dialogOpen = false;
                            // using (StreamReader reader = new StreamReader(fileStream))
                            // {
                            //     string line = reader.ReadLine();
                            // }
                            // using (var fileWriter = new System.IO.StreamWriter(fileStream)) { 
                            //     fileWriter.WriteLine("your text here");
                            //  }
                            // ...
                        };
                        _dialogOpen = true;
                        _dialog.ShowModal(_desktop);
                    }

                }

            }
        }
        public void atualizaListaDispositivos()
        {
            List<DispositivoUsuario> dispositivos = null;
            List<Component> novaLista = null;
            bool isMine = false;

            dispositivos = _comunicacao.RequisitarUsuarios();
            novaLista = new List<Component>();
            Random rnd = new Random();
            // foreach (var player in _players)
            // {
            //     novaLista.Add((Player)player);
            // }
            foreach (var dispositivo in dispositivos)
            {
                if (
                    dispositivo.Ip == _comunicacao._ip1 &&
                    dispositivo.Porta == _comunicacao._port1
                    )
                {
                    setPlayer(
                    new Player(
                        Content.Load<Texture2D>($"Textures/{_comunicacao._sprite.ToString()}"),
                        new Input()
                        {
                            Up = Keys.Up,
                            Down = Keys.Down,
                            Left = Keys.Left,
                            Right = Keys.Right
                        },
                        Content.Load<SpriteFont>("Fonts/Ubuntu32"),
                        dispositivo.Nome,
                        ClickPlayer,
                        dispositivo,
                        true,
                        ((float)dispositivo.Rotation) / 1000
                    ));
                    novaLista.Add(_player);
                }
                else
                    novaLista.Add(new Player(
                        Content.Load<Texture2D>($"Textures/{dispositivo.Sprite.ToString()}"),
                        new Input()
                        {
                            Up = Keys.Up,
                            Down = Keys.Down,
                            Left = Keys.Left,
                            Right = Keys.Right
                        },
                        Content.Load<SpriteFont>("Fonts/Ubuntu32"),
                        dispositivo.Nome,
                        ClickPlayer,
                        dispositivo,
                        false,
                        ((float)dispositivo.Rotation) / 1000
                    ));
            }
            _players = novaLista;
        }
        public void atualizaListaAmbientes()
        {
            List<AmbienteUsuario> ambientes = null;
            List<Component> novaLista = null;
            ambientes = _comunicacao.RequisitarAmbientes();
            novaLista = new List<Component>();
            Random rnd = new Random();
            // foreach (var player in _players)
            // {
            //     novaLista.Add((Player)player);
            // }
            foreach (var ambiente in ambientes)
            {

                novaLista.Add(
                    new Ambient(
                        Content.Load<Texture2D>($"Textures/5"),
                        ambiente.X,
                        ambiente.Y
                    )
                );
            }
            _ambients = novaLista;
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (_comunicacao != null)
            {
                var currentPlayers = _players;
                foreach (var component in currentPlayers)
                {
                    component.Update(gameTime);
                    if (((Player)component).IsMine)
                    {
                        _comunicacao.MoverDispositivo(
                            Convert.ToInt32(((Player)component).Position.X),
                            Convert.ToInt32(((Player)component).Position.Y),
                            Convert.ToInt32(((Player)component)._rotation * 1000),
                            ((Player)component)._dispositivo.Ambiente
                        );

                    }
                }
                atualizaListaDispositivos();
                atualizaListaAmbientes();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (_currentWindowMarker == (int)_currentWindow.Login)
            {
                if (_desktop.Root != null)
                    _desktop.Root.Visible = true;
                IsMouseVisible = true;
                GraphicsDevice.Clear(Color.Black);
                _desktop.Render();
            }
            else
            {
                if (_desktop.Root != null)
                    _desktop.Root.Visible = false;

                _spriteBatch.Begin();
                var currentPlayers = _players;
                foreach (var component in currentPlayers)
                {
                    component.Draw(gameTime, _spriteBatch);
                }
                var currentAmbients = _ambients;
                foreach (var component in currentAmbients)
                {
                    component.Draw(gameTime, _spriteBatch);
                }
                _spriteBatch.End();

                IsMouseVisible = true;
                _desktop.Render();
            }

            base.Draw(gameTime);
        }
        protected override void OnExiting(object sender, EventArgs args)
        {
            Console.WriteLine("Saindo");
            Exit();
            Environment.Exit(0);
        }
    }
}
