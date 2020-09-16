using System;
using System.Linq;
using System.Threading;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;

namespace TrocaArquivo.TupleSpace
{
    public class TpServidor
    {
        private readonly string port;
        private readonly string ip;
        private SpaceRepository repository;
        public bool pingCompleted = false;
        public RemoteSpace remotespace;
        public TpServidor(string ip, string port, SpaceRepository space)
        {
            this.ip = ip;
            this.port = port;
            this.repository = space;
            // instanciar space
            this.remotespace = new RemoteSpace($"tcp://{this.ip}:{this.port}/chat?KEEP");

        }

        public void StartSpace()
        {
            this.repository = new SpaceRepository();
            this.repository.AddGate($"tcp://{this.ip}:{this.port}?KEEP");
            this.repository.AddSpace("TrocaArquivo", new SequentialSpace());
            Console.ReadKey();
        }


        private void checkOnline(string name)
        {
            // a requisicao para a tupla aguardará até que haja alguma tupla que se encaixe 
            var tupla = this.remotespace.Get("User", name, "ping", typeof(bool));
            pingCompleted = true;
        }
        private static bool WaitUntil(int numberOfMiliSeconds, Func<bool> condition)
        {
            int waited = 0;
            while (!condition() && waited < numberOfMiliSeconds)
            {
                Thread.Sleep(100);
                waited += 100;
            }

            return condition();
        }
    }
}