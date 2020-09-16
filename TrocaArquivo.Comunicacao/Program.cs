using System;
using System.Threading;

namespace TrocaArquivo.Comunicacao
{
    class Program
    {
        static void Main(string[] args)
        {
            var com = new Comunication(false, args[0], args[1], args[2], args[3]);
            var t = new Thread(() => com.IniciarServidorRPC());
            t.Start();
            var s = new Thread(() => com.IniciarServidorTuplas());
            s.Start();
            Console.Read();
        }
    }
}
