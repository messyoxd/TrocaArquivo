using System.Threading;
using System;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using TrocaArquivo.TupleSpace.Cliente;

namespace TrocaArquivo.TupleSpace
{
    class Program
    {
        static void Main(string[] args)
        {
            SpaceRepository repository = new SpaceRepository();
            string ip = null;
            string port = null;
            if (args.Length == 1)
            {
                ip = args[0];
                port = "8989";
                repository.AddGate($"tcp://{ip}:8989?KEEP");
            }

            else if (args.Length == 2)
            {
                ip = args[0];
                port = args[1];
                repository.AddGate($"tcp://{ip}:{port}?KEEP");
            }
            else
            {
                Console.WriteLine("Nenhum IP ou porta foi dado!");
                Console.WriteLine("Configuração default será usada!");
                ip = "127.0.0.1";
                port = "8989";
                repository.AddGate("tcp://127.0.0.1:8989?KEEP");
            }

            repository.AddSpace("TrocaArquivo", new SequentialSpace());

            TpServidor s = new TpServidor(ip, port, repository);

            Console.Read();
        }
    }
}
