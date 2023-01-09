using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Common.Cache;

namespace Presentation
{
    public partial class Mensajes : Form
    {
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private Login login1;
        
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();
        public Mensajes(Login log)
        {
            InitializeComponent();
            login1 = log;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            EnviarMensaje("5," + DataLogin.IP, clientSocket);
            Application.Exit();
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            string currentDir = Environment.CurrentDirectory;
            string imageDir = Path.Combine(currentDir, "..", "..", "Resources");
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                btnMaximize.Image = Image.FromFile(Path.Combine(imageDir, "dentro.png"));
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                btnMaximize.Image = Image.FromFile(Path.Combine(imageDir, "fuera.png"));
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState= FormWindowState.Minimized;
        }

        private void Mensajes_Load(object sender, EventArgs e)
        {
            string currentDir = Environment.CurrentDirectory;
            string imageDir = Path.Combine(currentDir, "..", "..", "Resources");
            btnClose.Image = Image.FromFile(Path.Combine(imageDir, "cerrar.png"));
            btnMinimize.Image = Image.FromFile(Path.Combine(imageDir, "menos.png"));
            btnMaximize.Image = Image.FromFile(Path.Combine(imageDir, "fuera.png"));
            btnClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            btnMaximize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            btnMinimize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse("192.168.100.9"), 5000));
            try
            {
                Thread hiloRecibir = new Thread(() => Conectar_Cliente_Recibir(90,txtChat));
                hiloRecibir.Start();

            }
            catch
            {
                Console.WriteLine("No se pudo conectar al servidor");
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
        }

        private void Mensajes_ResizeEnd(object sender, EventArgs e)
        {
            if (this.Top <= 5)
            {
                this.WindowState = FormWindowState.Maximized;
                string currentDir = Environment.CurrentDirectory;
                string imageDir = Path.Combine(currentDir, "..", "..", "Resources");
                btnMaximize.Image = Image.FromFile(Path.Combine(imageDir, "dentro.png"));
            }
        }

        static void EnviarMensaje(string message, Socket socket)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                socket.Send(data);
            }
            catch
            { }
        }

        static string RecibirMensaje(Socket socket)
        {
            try
            {
                byte[] receiveData = new byte[1024];
                int bytesReceived = socket.Receive(receiveData);
                string receiveMessage = Encoding.UTF8.GetString(receiveData, 0, bytesReceived);
                return receiveMessage;
            }
            catch
            {
                return null;
            }
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            if(txtText.Text==null)
                return;
            string mensaje=DataLogin.UserName+": "+txtText.Text;
            txtChat.Text = txtChat.Text + "\n" + mensaje;
            foreach (string ip in DataLogin.IPs.Split(','))
            {
                if (ip != DataLogin.IP&&ip!="")
                {
                    Conectar_Cliente_Enviar(ip, 90, mensaje);
                }
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            EnviarMensaje("6", clientSocket);
            DataLogin.IPs = RecibirMensaje(clientSocket);
        }

        static public void Conectar_Cliente_Enviar(string direccion_IP, int puerto, string mensajeEnviar)
        {
            try
            {
                IPEndPoint IP_end = new IPEndPoint(IPAddress.Parse(direccion_IP), puerto);
                Socket socket_cliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket_cliente.Connect(IP_end);
                string mensaje_enviar = mensajeEnviar;
                byte[] msg_enviar = System.Text.Encoding.ASCII.GetBytes(mensaje_enviar);
                int bytes_enviados = socket_cliente.Send(msg_enviar);
                socket_cliente.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Conectar_Cliente_Recibir(int puerto,TextBox txt)
        {
            while (true)
            {
                try
                {
                    IPEndPoint IP_end = new IPEndPoint(IPAddress.Any, puerto);
                    Socket socket_servidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket_servidor.Bind(IP_end);
                    socket_servidor.Listen(5);
                    //"Esperando por conexión del cliente"
                    Console.WriteLine("esperando mensaje en el hilo 2");
                    Socket socket_cliente = socket_servidor.Accept();
                    //"Conectado"
                    byte[] msg_recibido = new byte[1024];
                    int bytes_recibidos = socket_cliente.Receive(msg_recibido);
                    if (Encoding.ASCII.GetString(msg_recibido, 0, bytes_recibidos) != "vacio")
                    {
                        string mensaje = Encoding.ASCII.GetString(msg_recibido, 0, bytes_recibidos);
                        socket_cliente.Close();
                        socket_servidor.Close();
                        txt.Text = txt.Text + "\n" + mensaje;
                    }

                }
                catch       
                {
                    // return e.ToString();
                }
            }
        }
    }
}
