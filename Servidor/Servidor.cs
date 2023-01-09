using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace Servidor
{
    public class Servidor
    {
        static UserModel _user = new UserModel();
        static void Main(string[] args)
        {
            IPAddress localAddress = GetLocalIPAddress();
            // Crea un socket del servidor y espera a recibir conexiones de múltiples clientes
            TcpListener server = new TcpListener(localAddress, 5000);
            server.Start();
            Console.WriteLine("Esperando a conexiones de clientes...");

            while (true)
            {
                // Acepta una conexión del cliente y crea un hilo de ejecución para manejar la conexión con ese cliente
                TcpClient client = server.AcceptTcpClient();
                Thread thread = new Thread(new ParameterizedThreadStart(HandleClient));
                thread.Start(client);
            }
        }

        static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            Console.WriteLine("Conexión establecida con el cliente.");

            NetworkStream stream = client.GetStream();
            do
            {
                try
                {
                    string recivo = receiveMessage(stream);
                    string[] _peticion=recivo.Split(',');
                    /*
                     * Aqui listare las distintas peticiones que se realizaran al server:
                     * 1) Verificar ID y telefono para login
                     * 2) Ingresar nueva IP
                     * 3) Validar un numero de telefono esta en uso
                     * 4) Ingresar nuevo usuario con ID, telefono y nombre
                     * 5) Eliminar ip de las IPs
                     * 6) Enviar las IPs
                     * 7) Cerra conexion
                     */
                    if (_peticion[0] == "1")
                    {
                        string _resp = _user.validLogin(int.Parse(_peticion[1]), int.Parse(_peticion[2]));
                        if (_resp != null)
                            sendMessage(_resp,stream);
                        else
                            sendMessage("false",stream);
                    }
                    if (_peticion[0] == "2")
                    {
                        _user.insertIP(_peticion[1]);
                        List<string> IPs = _user.getIPs();
                        string message = String.Join(",", IPs);
                        sendMessage(message, stream);
                        break;
                    }
                    if (_peticion[0] == "3")
                    {
                        if (!_user.validNumber(int.Parse(_peticion[1])))
                            sendMessage("true", stream);
                        else
                            sendMessage("false", stream);
                    }
                    if (_peticion[0] == "4")
                    {
                        int id = _user.insertUser(_peticion[1], int.Parse(_peticion[2]));
                        sendMessage(id.ToString(), stream);
                    }
                    if (_peticion[0] == "5")
                    {
                        _user.eliminarIP(_peticion[1]);
                        List<string> IPs = _user.getIPs();
                        string message = String.Join(",", IPs);
                        sendMessage(message, stream);
                        break;
                    }

                    if (_peticion[0] == "6")
                    {
                        List<string> IPs = _user.getIPs();
                        string message = String.Join(",", IPs);
                        sendMessage(message, stream);
                    }

                    if (_peticion[0]=="7")
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            } while (true);
            stream.Close();
        }

        static void sendMessage(string message, NetworkStream stream)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
            catch
            {
            }
        }

        static string receiveMessage(NetworkStream stream)
        {
            try
            {
                byte[] responseData = new byte[1024];
                int bytesReceived = stream.Read(responseData, 0, responseData.Length);
                string response = Encoding.UTF8.GetString(responseData, 0, bytesReceived);
                return response;
            }
            catch
            {
                return null;
            }
        }

        static IPAddress GetLocalIPAddress()
        {
            // Obtener la dirección IP de la interfaz de red local
            IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress address in addresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(address))
                {
                    return address;
                }
            }
            throw new Exception("No se ha podido obtener la dirección IP local.");
        }

    }
}
