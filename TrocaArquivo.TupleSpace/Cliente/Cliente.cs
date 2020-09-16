using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;

namespace TrocaArquivo.TupleSpace.Cliente
{
    public class TpCliente
    {
        private readonly string _space;
        private readonly string _port;
        private readonly string _ip;
        private readonly RemoteSpace _remotespace;
        public bool ContinueToCheckOnline { get; set; }
        public bool ContinueToCheckGroupMessage { get; set; }
        public bool pingCompleted { get; set; }
        public TpCliente(string ip, string port, string space)
        {
            _ip = ip;
            _port = port;
            _space = space;
            _remotespace = new RemoteSpace($"tcp://{ip}:{port}/{space}?KEEP");
            ContinueToCheckOnline = false;
            ContinueToCheckGroupMessage = false;
        }
        public void BeCheckedOnline(string name)
        {
            while (ContinueToCheckOnline)
            {
                // fica travado até a seguinte tupla existir
                // caso exista, significa que alguem checou se esse usuario esta online
                _remotespace.Get("User", name, "ping");
                // colocar a seguinte tupla para indicar que esta online
                _remotespace.Put("User", name, "ping", true);
            }
        }
        // public void CheckGroupMessage(string groupName)
        // {
        //     while (ContinueToCheckGroupMessage)
        //     {
        //         // pegar mensagens
        //         var tuplas = _remotespace.QueryAll("Group", "Sender", "Message", "Timestamp", "IsWhisper", "Receiver",
        //                                         groupName, typeof(string), typeof(string), typeof(string), typeof(bool), typeof(string));

        //         // mostrar mensagem na GUI
        //         foreach (var tupla in tuplas)
        //         {
        //             _insertMessage(
        //                 (string)tupla[6],
        //                 (string)tupla[7],
        //                 (string)tupla[8],
        //                 (string)tupla[9],
        //                 (bool)tupla[10],
        //                 (string)tupla[11]
        //             );
        //         }
        //         Thread.Sleep(100);
        //     }
        // }
        public void checkOnline(string name)
        {
            // a requisicao para a tupla aguardará até que haja alguma tupla que se encaixe 
            var tupla = _remotespace.Get("User", name, "ping", typeof(bool));
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
        private bool CheckDevice(string name, string ambient)
        {
            var devices = GetAllDevices();
            if (devices.Count > 0)
            {
                foreach (var item in devices)
                {
                    foreach (var device in item)
                    {
                        if ((string)device[8] == ambient && (string)device[9] == name)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public void AddDevice(string name, string ip, string port, int x, int y, int rotation, int sprite, string ambient)
        {
            // checar se há tupla com dispositivo
            if (!this.CheckDevice(name, ambient))
            {
                // se não houver, então colocar
                _remotespace.Put("Ambient", "Name", "Ip", "Port", "X", "Y", "Rotation", "Sprite",
                                ambient, name, ip, port, x.ToString(), y.ToString(), rotation.ToString(), sprite.ToString());
                // comecar com a thread para que o servidor possa checar se o usuario esta online
                ContinueToCheckOnline = true;
                var threadBeCheck = new Thread(() => this.BeCheckedOnline(name));
                threadBeCheck.Start();
            }
            else
            {
                // a tupla com o dispositivo existe
                // checar se o usuario dono da tupla está online colocando uma tupla de ping
                _remotespace.Put("User", name, "ping");
                // caso a resposta demore mais que 2s, assumir que esta offline e pegar a tupla 
                pingCompleted = false;
                var threadCheck = new Thread(() => this.checkOnline(name));
                threadCheck.Start();
                if (!WaitUntil(2000, () => pingCompleted))
                {
                    // offline
                    // se está offline entao pode-se adicionar o usuario

                    // Remover velha tupla
                    _remotespace.GetP("Ambient", "Name", "Ip", "Port", "X", "Y", "Rotation", "Sprite",
                                ambient, name, typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));
                    // Adicionar nova tupla com o nome
                    _remotespace.Put("Ambient", "Name", "Ip", "Port", "X", "Y", "Rotation", "Sprite",
                                ambient, name, ip, port, x.ToString(), y.ToString(), rotation.ToString(), sprite.ToString());
                }
                else
                {
                    // online
                    // se está online, emitir erro!
                    throw new Exception("Dispositivo com mesmo nome ja existe nesse ambiente!");
                }
            }
        }
        private bool CheckAmbient(string ambientName)
        {
            // checa se há um ambiente com o mesmo nome
            var ambient = _remotespace.QueryP("Ambient", "X", "Y", "Sprite", ambientName, typeof(string), typeof(string), typeof(string));
            if (ambient == null)
                return false;
            else
                return true;
        }
        public void AddAmbient(string ambientName, int x, int y, int sprite)
        {
            // checar se há tupla com o nome do ambiente ja criada
            if (!this.CheckAmbient(ambientName))
            {
                // se não houver, então colocar
                _remotespace.Put("Ambient", "X", "Y", "Sprite", ambientName, x.ToString(), y.ToString(), sprite.ToString());
            }
            else
            {
                // a tupla com a sala já existe
                throw new Exception("Ambiente de nome igual ja existe!");
            }
        }
        public void AtualizaDispositivo(string name, string ip, string port, int x, int y, int rotation, int sprite, string ambient)
        {
            // Remover velha tupla
            _remotespace.GetP("Ambient", "Name", "Ip", "Port", "X", "Y", "Rotation", "Sprite",
                        ambient, name, ip, port, typeof(string), typeof(string), typeof(string), sprite.ToString());
            // Adicionar nova tupla com o nome
            _remotespace.Put("Ambient", "Name", "Ip", "Port", "X", "Y", "Rotation", "Sprite",
                        ambient, name, ip, port, x.ToString(), y.ToString(), rotation.ToString(), sprite.ToString());
        }
        // public void InsertUserIntoGroup(string group, string userName)
        // {
        //     // pegar todos as salas
        //     var groups = _remotespace.QueryAll("Group", typeof(string));
        //     var flag = true;
        //     if (groups.Count() > 0 && groups.ElementAt(0) != null)
        //     {
        //         foreach (var item in groups)
        //         {
        //             if (this.CheckUserInGroup((string)item[1], userName))
        //             {
        //                 // isso pode acontecer se um usuario sair do programa e voltar sem sala atual setada
        //                 flag = false;
        //                 // throw new Exception("Usuario ja esta em uma sala!");
        //             }
        //         }
        //         // apos verificar se o usuario nao esta em nenhum outro grupo, inseri-lo
        //         if (flag)
        //         {
        //             _remotespace.Put("Group", "User", group, userName);
        //         }
        //     }
        //     else
        //     {
        //         throw new Exception("Nao ha nenhum grupo!");
        //     }
        // }
        // public void ReceiveMessages(string group)
        // {
        //     // caso haja alguma thread lendo mensagens, esperar ela morrer
        //     ContinueToCheckGroupMessage = false;
        //     Thread.Sleep(200);
        //     // começar a ouvir mensagems daquela sala
        //     ContinueToCheckGroupMessage = true;
        //     var threadCheckGroupMessage = new Thread(() => this.CheckGroupMessage(group));
        //     threadCheckGroupMessage.Start();
        // }
        // private bool CheckUserInGroup(string group, string userName)
        // {
        //     // checar se o usuario está naquele grupo
        //     var usuarios = _remotespace.QueryAll("Group", "User", group, typeof(string));
        //     if (usuarios.Count() > 0 && usuarios.ElementAt(0) != null)
        //     {
        //         foreach (var item in usuarios)
        //         {
        //             if ((string)item[3] == userName)
        //                 return true;
        //         }
        //     }
        //     return false;
        // }
        // public void RemoveUserFromGroup(string group, string userName)
        // {
        //     // checar se o usuario está naquela sala
        //     if (this.CheckUserInGroup(group, userName))
        //     {
        //         // se estiver, então será retirado
        //         _remotespace.Get("Group", "User", group, userName);
        //         // atualizar tupla de usuarios retirados
        //         _remotespace.GetP("Group", "User", "LastRemoved", group, typeof(string));
        //         _remotespace.Put("Group", "User", "LastRemoved", group, DateTime.UtcNow.ToString());
        //     }
        //     else
        //     {

        //         throw new Exception("O usuario nao estava naquela sala!");
        //     }
        // }
        // public IEnumerable<ITuple> GetAllGroupMessages(string group)
        // {
        //     // pegar todas as mensagens
        //     return _remotespace.QueryAll("Group", "Sender", "Message", "Timestamp", "IsWhisper", "Receiver",
        //                                 group, typeof(string), typeof(string), typeof(string), typeof(bool), typeof(string));

        // }
        // private string concatString(List<string> listaString)
        // {
        //     string str = "";
        //     for (int i = 0; i < listaString.Count; i++)
        //     {
        //         if (i == listaString.Count - 1)
        //             str += listaString[i];
        //         else
        //             str += listaString[i] + " ";
        //     }
        //     return str;
        // }
        // public void SendMessage(string group, string senderName, string message)
        // {
        //     // checar se grupo e usuario existem e se o usuario esta no grupo
        //     if (this.CheckGroup(group) && this.CheckUser(senderName) && this.CheckUserInGroup(group, senderName))
        //     {
        //         // pegar data do envio da mensagem
        //         var data = DateTime.UtcNow.ToString();
        //         // checar se é um sussurro. Exemplo: /w Fulano oi fulano
        //         if (message.Split("/w").Length > 1)
        //         {
        //             // se for
        //             // pegar o nome do destinatario
        //             var receiver = message.Split("/w").ElementAt(1).Split(" ").ElementAt(1);
        //             // pegar mensagem
        //             var whisper = concatString(message.Split("/w").ElementAt(1).Split(" ").Skip(2).ToList());
        //             // enviar mensagem para o space
        //             _remotespace.Put("Group", "Sender", "Message", "Timestamp", "IsWhisper", "Receiver",
        //                               group, senderName, whisper, data, true, receiver);
        //             // adicionar mensagem no chat
        //             // _insertMessage(group, senderName, whisper, data, true, receiver);
        //         }
        //         else
        //         {
        //             // se não for
        //             // enviar mensagem para o space
        //             _remotespace.Put("Group", "Sender", "Message", "Timestamp", "IsWhisper", "Receiver",
        //                               group, senderName, message, data, false, "");
        //             // adicionar mensagem no chat
        //             // _insertMessage(group, senderName, message, data, false, "");
        //         }
        //     }
        //     else
        //     {
        //         throw new Exception("Erro ao enviar mensagem!");
        //     }

        // }
        // public IEnumerable<ITuple> GetAllGroups()
        // {
        //     return _remotespace.QueryAll("Group", typeof(string));
        // }
        public List<IEnumerable<ITuple>> GetAllDevices()
        {
            var devices = new List<IEnumerable<ITuple>>();
            var ambients = GetAllAmbients();
            foreach (var ambient in ambients)
            {
                devices.Add(_remotespace.QueryAll("Ambient", "Name", "Ip", "Port", "X", "Y", "Rotation", "Sprite",
                                        ambient[4], typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string)));
            }
            return devices;
        }
        public IEnumerable<ITuple> GetAllAmbients()
        {
            var ambients = _remotespace.QueryAll("Ambient", "X", "Y", "Sprite", typeof(string), typeof(string), typeof(string), typeof(string));
            return ambients;
        }
        // public void ChangeGroup(string lastGroup, string nextGroup, string userName)
        // {
        //     // se usuario estava em um sala antes 
        //     if (lastGroup != "")
        //     {
        //         // parar de receber mensagens de outra sala
        //         ContinueToCheckGroupMessage = false;
        //         // remover usuario da sala
        //         try
        //         {
        //             this.RemoveUserFromGroup(lastGroup, userName);
        //         }
        //         catch (System.Exception)
        //         {
        //             // 
        //         }
        //     }
        //     // inserir usuario na outra sala
        //     this.InsertUserIntoGroup(nextGroup, userName);
        // }
    }
}