using System.Collections.Generic;
using System.Text.Json;
using Google.Protobuf;
using Newtonsoft.Json;
using TrocaArquivo.Classes;

namespace TrocaArquivo.RPC.ClientImplementation
{
    public class TrocaArquivoRPCClientImpl
    {
        private readonly TrocaArquivoRPC.TrocaArquivoRPCClient _client;
        public TrocaArquivoRPCClientImpl(TrocaArquivoRPC.TrocaArquivoRPCClient client)
        {
            _client = client;
        }
        public void Conectar(string nome, string ip, string porta, string ipServidor, string portaServidor, int x, int y)
        {
            // O servidor vai conectar com o usuario
            var resposta = _client.Conectar(
                new PacoteConexao
                {
                    Dispositivo = new Dispositivo()
                    {
                        Nome = nome,
                        IP = ipServidor, // ip do usuario
                        Porta = portaServidor, // porta do usuario
                        X = x,
                        Y = y,
                    },
                    IPServidor = ip, // ip do servidor
                    PortaServidor = porta // porta do servidor
                }
            );
        }
        public void ConectarDevolta(string nome, string ip, string porta, string ipServidor, string portaServidor, int x, int y)
        {
            // o usuario faz com que o servidor se conecte devolta
            var resposta = _client.ConectarDevolta(
                new PacoteConexao
                {
                    Dispositivo = new Dispositivo()
                    {
                        Nome = nome,
                        IP = ip,
                        Porta = porta,
                        X = x,
                        Y = y,
                    },
                    IPServidor = ipServidor,
                    PortaServidor = portaServidor
                }
            );
        }
        public DispositivoUsuario AdicionarDispositivoEmAmbiente(string nome, string ip, string porta, int x, int y, int sprite)
        {
            var resposta = _client.AdicionarDispositivoEmAmbiente(new Dispositivo()
            {
                Nome = nome,
                IP = ip,
                Porta = porta,
                X = x,
                Y = y,
                DispositivoSprite = sprite,
                Rotation = 0,
                Ambiente = new Ambiente() { Nome = "", X = x, Y = y }
            });
            return new DispositivoUsuario(resposta.Nome, resposta.IP, resposta.Porta, resposta.X, resposta.Y, resposta.Rotation, resposta.DispositivoSprite, resposta.Ambiente.Nome);
        }
        public void CriarAmbiente(string nome, int x, int y)
        {
            var resposta = _client.CriarAmbiente(
                new Ambiente()
                {
                    Nome = nome,
                    X = x,
                    Y = y
                }
            );
        }
        public void MoverDispositivo(string nome, string ip, string porta, int x, int y, int rotation, int sprite, string AmbienteNome)
        {
            var resposta = _client.MoverDispositivo(
                new Dispositivo()
                {
                    Nome = nome,
                    IP = ip,
                    Porta = porta,
                    X = x,
                    Y = y,
                    Rotation = rotation,
                    DispositivoSprite = sprite,
                    Ambiente = new Ambiente()
                    {
                        Nome = AmbienteNome
                    }
                }
            );
        }
        public void MandarArquivo(string nome, byte[] conteudo, DispositivoUsuario disp)
        {
            var resposta = _client.MandarArquivo(new Arquivo()
            {
                DispositivoReceptor = new Dispositivo()
                {
                    Nome = disp.Nome,
                    IP = disp.Ip,
                    Porta = disp.Porta,
                    X = disp.X,
                    Y = disp.Y,
                    Rotation = disp.Rotation,
                    DispositivoSprite = disp.Sprite,
                    Ambiente = new Ambiente()
                    {
                        Nome = disp.Ambiente
                    }
                },
                Nome = nome,
                Conteudo = ByteString.CopyFrom(conteudo)
            });
        }
        public void ReceberArquivo(string nome, byte[] conteudo, DispositivoUsuario disp)
        {
            var resposta = _client.ReceberArquivo(new Arquivo()
            {
                DispositivoReceptor = new Dispositivo()
                {
                    Nome = disp.Nome,
                    IP = disp.Ip,
                    Porta = disp.Porta,
                    X = disp.X,
                    Y = disp.Y,
                    Rotation = disp.Rotation,
                    DispositivoSprite = disp.Sprite,
                    Ambiente = new Ambiente()
                    {
                        Nome = disp.Ambiente
                    }
                },
                Nome = nome,
                Conteudo = ByteString.CopyFrom(conteudo)
            });
        }
        public List<DispositivoUsuario> RequisitarUsuarios(string nome, string ip, string porta, int x, int y, int rotation)
        {
            var resposta = _client.RequisitarUsuarios(new Dispositivo()
            {
                Nome = nome,
                IP = ip,
                Porta = porta,
                X = x,
                Y = y,
                Rotation = rotation
            });
            return JsonConvert.DeserializeObject<List<DispositivoUsuario>>(resposta.JsonBytes); //JsonSerializer.Deserialize<List<DispositivoUsuario>>(resposta.JsonBytes);
        }
        public List<AmbienteUsuario> RequisitarAmbientes(string nome, string ip, string porta, int x, int y, int rotation, int sprite)
        {
            var resposta = _client.RequisitarAmbientes(new Dispositivo()
            {
                Nome = nome,
                IP = ip,
                Porta = porta,
                X = x,
                Y = y,
                Rotation = rotation,
                DispositivoSprite = sprite
            });
            return JsonConvert.DeserializeObject<List<AmbienteUsuario>>(resposta.JsonBytes);
        }
    }
}