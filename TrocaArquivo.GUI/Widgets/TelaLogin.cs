using Myra.Graphics2D.UI;
using System;
using TrocaArquivo.Comunicacao;
namespace TrocaArquivo.GUI.Widgets
{
    public class TelaLogin
    {
        public string Nome { get; set; }
        public string IP { get; set; }
        public string Porta { get; set; }
        public string IPServidor { get; set; }
        public string PortaServidor { get; set; }
        public Label labelErro { get; set; }
        public Action<string> SetWindowTitlte { get; }
        public Action<int> SetCurrentWindowMarker { get; }
        public Action<Comunication> SetComunicacao { get; set; }
        public TelaLogin(Action<string> setWindowTitlte, Action<int> setCurrentWindowMarker, Action<Comunication> setComunicacao)
        {
            this.SetComunicacao = setComunicacao;
            this.SetCurrentWindowMarker = setCurrentWindowMarker;
            this.SetWindowTitlte = setWindowTitlte;
        }

        public Grid returnTelaPrincipal()
        {
            var grid = new Grid
            {
                RowSpacing = 8,
                ColumnSpacing = 8
            };
            grid.ColumnsProportions.Add(new Proportion());
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
            grid.ColumnsProportions.Add(new Proportion());
            // espaço
            grid.RowsProportions.Add(new Proportion
            {
                Type = ProportionType.Pixels,
                Value = 80
            });
            //Nome
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // ip local
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // porta local
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // ip servidor
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // porta servidor
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // erros
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // Botao OK
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            var label = new Label
            {
                Text = "Digite o seu nome",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 1,
                GridColumn = 1,
            };


            var textBox = new TextBox
            {
                Width = 400,
                Height = 24,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 2,
                GridColumn = 1,
            };

            textBox.TextChanged += (b, ea) =>
            {
                if (textBox.Text.Length > 33)
                {
                    textBox.Text = textBox.Text.Substring(0, 33);
                    textBox.CursorPosition = 33;
                }
            };

            ///// IP
            var labelIP = new Label
            {
                Text = "Digite o IP local",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 3,
                GridColumn = 1,
            };


            var textBoxIP = new TextBox
            {
                Width = 400,
                Height = 24,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 4,
                GridColumn = 1
            };
            textBoxIP.TextChanged += (b, ea) =>
            {
                if (textBoxIP.Text.Length > 33)
                {
                    textBoxIP.Text = textBox.Text.Substring(0, 33);
                    textBoxIP.CursorPosition = 33;
                }
            };
            ////////////// Porta
            var labelPorta = new Label
            {
                Text = "Digite a porta local",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 5,
                GridColumn = 1,
            };


            var textBoxPorta = new TextBox
            {
                Width = 400,
                Height = 24,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 6,
                GridColumn = 1
            };
            textBoxPorta.TextChanged += (b, ea) =>
            {
                if (textBoxPorta.Text.Length > 33)
                {
                    textBoxPorta.Text = textBox.Text.Substring(0, 33);
                    textBoxPorta.CursorPosition = 33;
                }
            };
            ///// IP Servidor
            var labelIPServer = new Label
            {
                Text = "Digite o IP do servidor",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 7,
                GridColumn = 1,
            };


            var textBoxIPServer = new TextBox
            {
                Width = 400,
                Height = 24,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 8,
                GridColumn = 1
            };
            textBoxIPServer.TextChanged += (b, ea) =>
            {
                if (textBoxIPServer.Text.Length > 33)
                {
                    textBoxIPServer.Text = textBox.Text.Substring(0, 33);
                    textBoxIPServer.CursorPosition = 33;
                }
            };
            ////////////// Porta Servidor
            var labelPortaServer = new Label
            {
                Text = "Digite a porta do servidor",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 9,
                GridColumn = 1,
            };


            var textBoxPortaServer = new TextBox
            {
                Width = 400,
                Height = 24,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 10,
                GridColumn = 1
            };
            textBoxPortaServer.TextChanged += (b, ea) =>
            {
                if (textBoxPortaServer.Text.Length > 33)
                {
                    textBoxPortaServer.Text = textBox.Text.Substring(0, 33);
                    textBoxPortaServer.CursorPosition = 33;
                }
            };
            ////////// erros
            labelErro = new Label
            {
                Text = "",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                GridRow = 11,
                GridColumn = 1,
                TextColor = Microsoft.Xna.Framework.Color.Crimson
            };
            ////////// Botao ok
            var button = new TextButton
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 100,
                Text = "Ok",
                GridRow = 12,
                GridColumn = 1,
            };

            button.Click += (b, ea) =>
            {
                cleanErrors();

                if (string.IsNullOrEmpty(textBox.Text))
                    showErrors("Escreva um nome!");
                else
                    this.Nome = textBox.Text;

                if (string.IsNullOrEmpty(textBoxIP.Text))
                    showErrors("Escreva o IP local!");
                else
                    this.IP = textBoxIP.Text;
                if (string.IsNullOrEmpty(textBoxIPServer.Text))
                    showErrors("Escreva o IP do servidor!");
                else
                    this.IPServidor = textBoxIPServer.Text;

                if (string.IsNullOrEmpty(textBoxPorta.Text))
                    showErrors("Escreva a porta local!");
                else
                    this.Porta = textBoxPorta.Text;
                if (string.IsNullOrEmpty(textBoxPortaServer.Text))
                    showErrors("Escreva a porta do servidor!");
                else
                    this.PortaServidor = textBoxPortaServer.Text;

                if (string.IsNullOrEmpty(labelErro.Text))
                {
                    this.SetCurrentWindowMarker(1);
                    Random rnd = new Random();
                    var playerSprite = rnd.Next(1, 4);
                    SetComunicacao(new Comunication(
                        true, this.IP, this.Porta, this.IPServidor, this.PortaServidor, this.Nome, playerSprite
                    ));
                    SetWindowTitlte($"Transmissao de Arquivos Ubiquo - {this.Nome}");
                }

            };

            grid.Widgets.Add(label);
            grid.Widgets.Add(textBox);
            grid.Widgets.Add(labelIP);
            grid.Widgets.Add(textBoxIP);
            grid.Widgets.Add(labelPorta);
            grid.Widgets.Add(textBoxPorta);
            grid.Widgets.Add(labelIPServer);
            grid.Widgets.Add(textBoxIPServer);
            grid.Widgets.Add(labelPortaServer);
            grid.Widgets.Add(textBoxPortaServer);
            grid.Widgets.Add(labelErro);
            grid.Widgets.Add(button);

            return grid;
        }

        public void showErrors(string erros)
        {
            labelErro.Text += erros + "\n";
        }
        public void cleanErrors()
        {
            labelErro.Text = "";
        }
    }
}