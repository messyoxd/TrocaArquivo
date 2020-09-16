using System.Text;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using TrocaArquivo.Classes;
using TrocaArquivo.TupleSpace.Cliente;
using System.Text.Json;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using TrocaArquivo.Comunicacao;
using System.IO;

namespace TrocaArquivo.RPC.ServerImplementation
{
    public class TrocaArquivoRPCServerImpl : TrocaArquivoRPC.TrocaArquivoRPCBase
    {
        private readonly bool _isUser;
        private readonly Action<string, string, string, string, string, int, int> _conectarDevolta;
        private readonly Action<Arquivo> _servidorMandarArquivo;
        private readonly string _ip1;
        private readonly string _ip2;
        private readonly string _porta1;
        private readonly string _porta2;
        private readonly TpCliente _tpClient;

        public TrocaArquivoRPCServerImpl(
            bool isUser,
            Action<string, string, string, string, string, int, int> conectarDevolta,
            Action<Arquivo> servidorMandarArquivo,
            string ipLocalRPC,
            string ipLocalTupla,
            string portaLocalRPC,
            string portaLocalTupla,
            string tupleSpaceName = "TrocaArquivo"
        )
        {
            _isUser = isUser;
            _conectarDevolta = conectarDevolta;
            _servidorMandarArquivo = servidorMandarArquivo;
            _ip1 = ipLocalRPC;
            _ip2 = ipLocalTupla;
            _porta1 = portaLocalRPC;
            _porta2 = portaLocalTupla;
            if (!_isUser)
            {
                Console.WriteLine("Servidor!");
                _tpClient = new TpCliente(_ip2, _porta2, tupleSpaceName);
            }
        }
        public override Task<RespostaConexao> Conectar(PacoteConexao request, ServerCallContext context)
        {
            // O Usuario recebe a conexão devolta do servidor
            try
            {
                Console.WriteLine($"O servidor {request.Dispositivo.IP}:{request.Dispositivo.Porta} se conectou devolta!");
                return Task.FromResult(TratarConectar(request));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public bool DistanciaMaiorQue10(int x1, int x2, int y1, int y2, int escala)
        {
            var dist = Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
            // if (dist > 10)
            if (dist > 10 * escala)
                return true;
            else
                return false;
        }
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public DispositivoUsuario adicionarDispositivoEmAmbiente(Dispositivo disp)
        {
            // pegar todos os ambientes
            var ambientes = this.pegarTodosOsAmbientes();
            var dispositivoInserir = new DispositivoUsuario(
                        disp.Nome,
                        disp.IP,
                        disp.Porta,
                        disp.X,
                        disp.Y,
                        disp.Rotation,
                        disp.DispositivoSprite
                    );
            if (ambientes.Count > 0)
            {
                // pegar todos os dispositivos
                var dispositivos = this.pegarTodosOsDispositivos();
                if (dispositivos.Count == 1)
                {
                    // se há um dispositivo e n ambientes
                    // tentar inserir o dispositivo no mesmo ambiente que o dispositivo já presente
                    var dispositivoPresente = dispositivos.First();

                    // checar distancia entre os dois dispositivos
                    if (DistanciaMaiorQue10(dispositivoInserir.X,
                                            dispositivoPresente.X,
                                            dispositivoInserir.Y,
                                            dispositivoPresente.Y, 10))
                    {
                        // caso a distancia entre eles seja maior que 10
                        // criar novo ambiente onde o novo dispositivo está

                        // Criar Ambiente
                        string ambientName = "";
                        using (SHA256 sha256Hash = SHA256.Create())
                        {
                            // string de 64 caracteres
                            ambientName = GetHash(sha256Hash, (dispositivoInserir.Nome + DateTime.Now.ToString()));
                            _tpClient.AddAmbient(ambientName, dispositivoInserir.X, dispositivoInserir.Y, 5);
                        }
                        // Criar dispositivo e associar ao ambiente
                        _tpClient.AddDevice(
                            dispositivoInserir.Nome,
                            dispositivoInserir.Ip,
                            dispositivoInserir.Porta,
                            dispositivoInserir.X,
                            dispositivoInserir.Y,
                            (int)dispositivoInserir.Rotation,
                            dispositivoInserir.Sprite,
                            ambientName
                        );


                    }
                    else
                    {
                        // caso a distancia entre eles seja menor/igual que 10
                        // inserir o novo dispositivo no mesmo ambiente que o outro dispositivo

                        dispositivoInserir.Ambiente = dispositivoPresente.Ambiente;
                        _tpClient.AddDevice(
                            dispositivoInserir.Nome,
                            dispositivoInserir.Ip,
                            dispositivoInserir.Porta,
                            dispositivoInserir.X,
                            dispositivoInserir.Y,
                            (int)dispositivoInserir.Rotation,
                            dispositivoInserir.Sprite,
                            dispositivoInserir.Ambiente
                        );
                    }
                }
                else if (dispositivos.Count > 1)
                {
                    bool inseriu = false;
                    // se há n dispositivos e n ambientes
                    // percorrer todos os ambientes buscando um que obedeça as regras
                    // e caso encontre, inserir o dispositivo nele. Senão, criar um novo.
                    foreach (var ambiente in ambientes)
                    {
                        // verificar se a distancia do novo dispositivo com todos os
                        // dispositivos do ambiente é menor que 10
                        if (DistanciaDispostivosAmbienteMenorQue10(ambiente, dispositivoInserir))
                        {
                            // caso obedeça a regra, inserir nesse ambiente e marcar uma flag
                            dispositivoInserir.Ambiente = ambiente.Nome;
                            // adicionar o dispositivo no ambiente
                            _tpClient.AddDevice(
                                dispositivoInserir.Nome,
                                dispositivoInserir.Ip,
                                dispositivoInserir.Porta,
                                dispositivoInserir.X,
                                dispositivoInserir.Y,
                                (int)dispositivoInserir.Rotation,
                                dispositivoInserir.Sprite,
                                dispositivoInserir.Ambiente
                            );
                            inseriu = true;
                            break;
                        }
                    }
                    // checar se foi inserido
                    if (!inseriu)
                    {
                        // se nao conseguiu ser inserido
                        // criar um novo ambiente
                        string ambientName = "";
                        using (SHA256 sha256Hash = SHA256.Create())
                        {
                            // string de 64 caracteres
                            ambientName = GetHash(sha256Hash, (dispositivoInserir.Nome + DateTime.Now.ToString()));
                            _tpClient.AddAmbient(ambientName, dispositivoInserir.X, dispositivoInserir.Y, 5);
                        }
                        dispositivoInserir.Ambiente = ambientName;
                        // inserir o novo dispositivo associado ao novo ambiente
                        _tpClient.AddDevice(
                            dispositivoInserir.Nome,
                            dispositivoInserir.Ip,
                            dispositivoInserir.Porta,
                            dispositivoInserir.X,
                            dispositivoInserir.Y,
                            (int)dispositivoInserir.Rotation,
                            dispositivoInserir.Sprite,
                            dispositivoInserir.Ambiente
                        );

                    }
                }
                else
                {
                    // Se não há dispositivos, mas há ambientes
                    // inserir o dispositivo no primeiro ambiente
                    var ambiente = ambientes.First();
                    dispositivoInserir.Ambiente = ambiente.Nome;
                    _tpClient.AddDevice(
                            dispositivoInserir.Nome,
                            dispositivoInserir.Ip,
                            dispositivoInserir.Porta,
                            dispositivoInserir.X,
                            dispositivoInserir.Y,
                            (int)dispositivoInserir.Rotation,
                            dispositivoInserir.Sprite,
                            dispositivoInserir.Ambiente
                        );
                }
            }
            else
            {
                // criar um ambiente e inserir o novo dispositivo associado à ele
                string ambientName = "";
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    // string de 64 caracteres
                    ambientName = GetHash(sha256Hash, (dispositivoInserir.Nome + DateTime.Now.ToString()));
                    _tpClient.AddAmbient(ambientName, dispositivoInserir.X, dispositivoInserir.Y, 5);
                }
                dispositivoInserir.Ambiente = ambientName;
                // inserir o novo dispositivo associado ao novo ambiente
                _tpClient.AddDevice(
                    dispositivoInserir.Nome,
                    dispositivoInserir.Ip,
                    dispositivoInserir.Porta,
                    dispositivoInserir.X,
                    dispositivoInserir.Y,
                    (int)dispositivoInserir.Rotation,
                    dispositivoInserir.Sprite,
                    dispositivoInserir.Ambiente
                );
            }
            return dispositivoInserir;
        }
        public RespostaConexao TratarConectar(PacoteConexao pkg)
        {
            // fazer as configurações necessarias para iniciar o programa do lado do Usuario

            // enviar ao servidor que está tudo ok
            return new RespostaConexao()
            {
                Mensagem = "ok",
                Resultado = true
            };
        }

        public bool DistanciaDispostivosAmbienteMenorQue10(AmbienteUsuario ambiente, DispositivoUsuario dispositivoInserir)
        {
            // pegar todos os dispositivos daquele ambiente
            var dispositivosAmbiente = pegarTodosOsDispositivosDoAmbiente(ambiente.Nome);
            // checar um por a distancia entre eles e o novo dispositivo
            foreach (var dispositivo in dispositivosAmbiente)
            {
                // checar se a distancia é maior que 10m
                if (DistanciaMaiorQue10(dispositivo.X, dispositivoInserir.Y, dispositivo.Y, dispositivoInserir.Y, 10))
                {
                    // se sim, então retornar falso
                    return false;
                }
            }
            return true;
        }

        public override Task<RespostaConexao> ConectarDevolta(PacoteConexao request, ServerCallContext context)
        {
            // o servidor recebe o pedido do usuario para se conectar devolta com ele
            try
            {
                Console.WriteLine($"Usuario de {request.Dispositivo.IP}:{request.Dispositivo.Porta} se conectou!");
                return Task.FromResult(TratarConectarDevolta(request));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public RespostaConexao TratarConectarDevolta(PacoteConexao pkg)
        {
            try
            {
                _conectarDevolta(
                    pkg.Dispositivo.Nome,
                    pkg.Dispositivo.IP,
                    pkg.Dispositivo.Porta,
                    pkg.IPServidor,
                    pkg.PortaServidor,
                    pkg.Dispositivo.X,
                    pkg.Dispositivo.Y
                );

                return new RespostaConexao()
                {
                    Mensagem = "ok",
                    Resultado = true
                };
            }
            catch (System.Exception e)
            {
                return new RespostaConexao()
                {
                    Mensagem = e.Data.ToString(),
                    Resultado = false
                };
            }
        }
        public override Task<Dispositivo> AdicionarDispositivoEmAmbiente(Dispositivo request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(TratarAdicionarDispositivoEmAmbiente(request));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public Dispositivo TratarAdicionarDispositivoEmAmbiente(Dispositivo pkg)
        {
            var disp = adicionarDispositivoEmAmbiente(pkg);

            return new Dispositivo()
            {
                Nome = disp.Nome,
                IP = disp.Ip,
                Porta = disp.Porta,
                X = disp.X,
                Y = disp.Y,
                Rotation = (int)disp.Rotation,
                Ambiente = new Ambiente() { Nome = disp.Ambiente, X = disp.X, Y = disp.Y }
            };

        }
        public override Task<Ambiente> CriarAmbiente(Ambiente request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(TratarCriarAmbiente(request));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public Ambiente TratarCriarAmbiente(Ambiente pkg)
        {
            return new Ambiente()
            {
                // Nome = pkg.Ambiente.Nome,
                // Resultado = true
            };
        }
        public override Task<RespostaConexao> MoverDispositivo(Dispositivo request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(TratarMoverDispositivo(request));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public RespostaConexao TratarMoverDispositivo(Dispositivo pkg)
        {
            _tpClient.AtualizaDispositivo(pkg.Nome, pkg.IP, pkg.Porta, pkg.X, pkg.Y, pkg.Rotation, pkg.DispositivoSprite, pkg.Ambiente.Nome);
            return new RespostaConexao()
            {
                Mensagem = "ok",
                Resultado = true
            };
        }
        public override Task<RespostaConexao> MandarArquivo(Arquivo request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(TratarMandarArquivo(request));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public RespostaConexao TratarMandarArquivo(Arquivo pkg)
        {
            // servidor
            _servidorMandarArquivo(pkg);
            return new RespostaConexao()
            {
                Mensagem = "ok",
                Resultado = true
            };
        }
        public override Task<RespostaConexao> ReceberArquivo(Arquivo request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(TratarReceberArquivo(request));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public RespostaConexao TratarReceberArquivo(Arquivo pkg)
        {
            // escrever o arquivo em algum canto
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory.Split("/TrocaArquivo.GUI/bin/Debug/netcoreapp3.1/").ElementAt(0);
            if (!Directory.Exists(projectDirectory + "/files/"))
            {
                Directory.CreateDirectory(projectDirectory + "/files/");
            }
            if (!Directory.Exists(projectDirectory + "/files/" + pkg.DispositivoReceptor.Ambiente.Nome))
            {
                Directory.CreateDirectory(projectDirectory + "/files/" + pkg.DispositivoReceptor.Ambiente.Nome);
            }
            string writePath = projectDirectory + "/files/" + pkg.DispositivoReceptor.Ambiente.Nome + "/" + pkg.Nome;

            File.WriteAllBytes(writePath, pkg.Conteudo.ToByteArray());
            return new RespostaConexao()
            {
                Mensagem = "ok",
                Resultado = true
            };
        }
        public override Task<JsonPacket> RequisitarUsuarios(Dispositivo request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(TratarRequisitarUsuarios(request));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private List<DispositivoUsuario> pegarTodosOsDispositivos()
        {
            Console.WriteLine("pegar todos os dispositivos");
            var devices = _tpClient.GetAllDevices();
            var dispositivos = new List<DispositivoUsuario>();
            foreach (var item in devices)
            {
                foreach (var device in item)
                {
                    dispositivos.Add(
                        new DispositivoUsuario(
                            (string)device[9],
                            (string)device[10],
                            (string)device[11],
                            int.Parse((string)device[12]),
                            int.Parse((string)device[13]),
                            int.Parse((string)device[14]),
                            int.Parse((string)device[15]),
                            (string)device[8]
                        )
                    );
                }
            }
            Console.WriteLine("todos os dispositivos foram pegos");
            return dispositivos;
        }
        public List<DispositivoUsuario> pegarTodosOsDispositivosDoAmbiente(string ambiente)
        {
            var devices = _tpClient.GetAllDevices();
            var dispositivos = new List<DispositivoUsuario>();
            foreach (var item in devices)
            {
                foreach (var device in item)
                {
                    if ((string)device[4] == ambiente)
                        dispositivos.Add(
                            new DispositivoUsuario(
                                (string)device[9],
                                (string)device[10],
                                (string)device[11],
                                int.Parse((string)device[12]),
                                int.Parse((string)device[13]),
                                int.Parse((string)device[14]),
                                int.Parse((string)device[15]),
                                (string)device[8]
                            )
                        );
                }
            }
            return dispositivos;
        }
        public JsonPacket TratarRequisitarUsuarios(Dispositivo disp)
        {
            // comunicar com o servidor de tuplas para que ele devolva todos os usuarios
            var devices = _tpClient.GetAllDevices();
            var dispositivos = new List<DispositivoUsuario>();
            foreach (var item in devices)
            {
                foreach (var device in item)
                {
                    // if ((string)device[5] != disp.Nome &&
                    // (string)device[4] != disp.Ambiente.Nome
                    // )
                    //     dispositivos.Add(
                    //         new DispositivoUsuario(
                    //             (string)device[5],
                    //             (string)device[6],
                    //             (string)device[7],
                    //             (int)device[8],
                    //             (int)device[9],
                    //             (string)device[4]
                    //         )
                    //     );
                    dispositivos.Add(
                            new DispositivoUsuario(
                                (string)device[9],
                                (string)device[10],
                                (string)device[11],
                                int.Parse((string)device[12]),
                                int.Parse((string)device[13]),
                                int.Parse((string)device[14]),
                                int.Parse((string)device[15]),
                                (string)device[8]
                            )
                        );
                }
            }
            return new JsonPacket()
            {
                JsonBytes = JsonConvert.SerializeObject(dispositivos)
            };
        }
        public override Task<JsonPacket> RequisitarAmbientes(Dispositivo request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(TratarRequisitarAmbientes(request));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private List<AmbienteUsuario> pegarTodosOsAmbientes()
        {
            var ambients = _tpClient.GetAllAmbients();
            var ambientes = new List<AmbienteUsuario>();
            foreach (var item in ambients)
            {
                ambientes.Add(
                    new AmbienteUsuario((string)item[4], int.Parse((string)item[5]), int.Parse((string)item[6]), int.Parse((string)item[7]))
                );
            }
            return ambientes;
        }
        public JsonPacket TratarRequisitarAmbientes(Dispositivo disp)
        {
            // comunicar com o servidor de tuplas para que envie devolta todos os ambientes
            var ambientes = this.pegarTodosOsAmbientes();
            return new JsonPacket()
            {
                JsonBytes = JsonConvert.SerializeObject(ambientes)
            };
        }
    }
}