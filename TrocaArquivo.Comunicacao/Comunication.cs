using System;
using System.Collections.Generic;
using System.Threading;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using Grpc.Core;
using TrocaArquivo.Classes;
using TrocaArquivo.RPC.ClientImplementation;
using TrocaArquivo.RPC.ServerImplementation;
using TrocaArquivo.TupleSpace;

namespace TrocaArquivo.Comunicacao
{
    public class Comunication
    {
        public readonly string _ip1;
        public readonly string _port1;
        public readonly string _nome;
        public readonly string _ip2;
        public readonly string _port2;
        public readonly int _sprite;
        public List<Conexao> _conexoes;
        public Server _server;
        public bool _IsUser;
        public TrocaArquivoRPCClientImpl _client;
        public Comunication(bool isUser, string ip, string port, string ipRemoto, string portRemoto, string nome = "", int sprite = 1)
        {
            _IsUser = isUser;
            _ip1 = ip;
            _port1 = port;
            _ip2 = ipRemoto;
            _port2 = portRemoto;
            _sprite = sprite;
            if (_IsUser)
                _nome = nome;
            else
                _conexoes = new List<Conexao>();
        }
        public void servidorMandarArquivo(Arquivo arquivo)
        {
            var channel = new Channel($"{arquivo.DispositivoReceptor.IP}:{arquivo.DispositivoReceptor.Porta}", ChannelCredentials.Insecure);
            // conectar o servidor com o usuario
            _client = new TrocaArquivoRPCClientImpl(new TrocaArquivoRPC.TrocaArquivoRPCClient(channel));
            _client.ReceberArquivo(arquivo.Nome, arquivo.Conteudo.ToByteArray(), new DispositivoUsuario(
                arquivo.DispositivoReceptor.Nome,
                arquivo.DispositivoReceptor.IP,
                arquivo.DispositivoReceptor.Porta,
                arquivo.DispositivoReceptor.X,
                arquivo.DispositivoReceptor.Y,
                arquivo.DispositivoReceptor.Rotation,
                arquivo.DispositivoReceptor.DispositivoSprite,
                arquivo.DispositivoReceptor.Ambiente.Nome
            ));
        }
        public void IniciarServidorRPC()
        {
            try
            {
                _server = new Server
                {

                    Services = { TrocaArquivoRPC.BindService(new TrocaArquivoRPCServerImpl(
                        _IsUser,
                        ConectarDevolta,
                        servidorMandarArquivo,
                        _ip1, _ip2, _port1, _port2
                    )) },
                    Ports = { new ServerPort(_ip1, int.Parse(_port1), ServerCredentials.Insecure) }
                };
                _server.Start();
                Console.WriteLine($"Current RPC Server endpoint: {_ip1}:{_port1}");
                Console.WriteLine($"Begin RPC listening");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void IniciarServidorTuplas(string ip = "127.0.0.1", string port = "8989")
        {
            SpaceRepository repository = new SpaceRepository();
            repository.AddGate($"tcp://{ip}:{port}?KEEP");
            repository.AddSpace("TrocaArquivo", new SequentialSpace());

            // Servidor s = new Servidor(ip, port, repository);

            Console.Read();
        }
        public void ConectarCom(int x, int y)
        {
            // essa função conecta o usuario com o servidor

            // criar canal de conexao com o servidor
            var channel = new Channel($"{_ip2}:{_port2}", ChannelCredentials.Insecure);
            // conectar com o servidor
            _client = new TrocaArquivoRPCClientImpl(new TrocaArquivoRPC.TrocaArquivoRPCClient(channel));
            // fazer o servidor conectar devolta
            _client.ConectarDevolta(_nome, _ip1, _port1, _ip2, _port2, x, y);
            Thread t = new Thread(() => IniciarServidorRPC());
            t.Start();
            // 

        }
        private bool checkConexaoRepetida(Conexao con)
        {
            foreach (var item in _conexoes)
            {
                if (item.Ip == con.Ip && item.Porta == con.Porta)
                    return true;
            }
            return false;
        }
        public void ConectarDevolta(string nome, string ip, string porta, string ipServidor, string portaServidor, int x, int y)
        {
            // o servidor recebeu um pedido de um usuario para conectar devolta com ele

            // salvar nova conexao
            var con = new Conexao(nome, ip, porta); // ip e porta do usuario
            if (!checkConexaoRepetida(con))
                _conexoes.Add(con);
            // criar canal de conexao do servidor para o usuario
            var channel = new Channel($"{ip}:{porta}", ChannelCredentials.Insecure);
            // conectar o servidor com o usuario
            _client = new TrocaArquivoRPCClientImpl(new TrocaArquivoRPC.TrocaArquivoRPCClient(channel));
            _client.Conectar(_nome, ipServidor, portaServidor, ip, porta, x, y);
        }
        public List<DispositivoUsuario> RequisitarUsuarios()
        {
            return _client.RequisitarUsuarios(_nome, _ip1, _port1, 0, 0, 0);
        }
        public List<AmbienteUsuario> RequisitarAmbientes()
        {
            return _client.RequisitarAmbientes(_nome, _ip1, _port1, 0, 0, 0, _sprite);
        }
        public DispositivoUsuario AdicionarDevice(int x, int y)
        {
            // usuario insere um novo dispositivo
            return _client.AdicionarDispositivoEmAmbiente(_nome, _ip1, _port1, x, y, _sprite);
        }
        public void MoverDispositivo(int x, int y, int rotation, string AmbienteNome)
        {
            _client.MoverDispositivo(_nome, _ip1, _port1, x, y, rotation, _sprite, AmbienteNome);
        }
        public void MandarArquivo(string nomeArquivo, byte[] conteudo, DispositivoUsuario disp)
        {
            _client.MandarArquivo(nomeArquivo, conteudo, disp);
        }
    }
}