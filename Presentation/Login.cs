using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Common.Cache;
    
namespace Presentation
{
    public partial class Login : Form
    {
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();
        public Login()
        {
            InitializeComponent();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
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
            posicionBotons();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            posicionBotons();
            string currentDir = Environment.CurrentDirectory;
            string imageDir = Path.Combine(currentDir, "..", "..", "Resources");
            btnClose.Image = Image.FromFile(Path.Combine(imageDir, "cerrar.png"));
            btnMinimize.Image= Image.FromFile(Path.Combine(imageDir, "menos.png"));
            btnMaximize.Image= Image.FromFile(Path.Combine(imageDir, "fuera.png"));
            btnClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            btnMaximize.SizeMode= System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            btnMinimize.SizeMode= System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse("192.168.100.9"), 5000));
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

        private void Login_ResizeEnd(object sender, EventArgs e)
        {
            if (this.Top <= 5)
            {
                this.WindowState = FormWindowState.Maximized;
                posicionBotons();
                string currentDir = Environment.CurrentDirectory;
                string imageDir = Path.Combine(currentDir, "..", "..", "Resources");
                btnMaximize.Image = Image.FromFile(Path.Combine(imageDir, "dentro.png"));
            }
        }

        private void btnRegistrarse_Click(object sender, EventArgs e)
        {
            if (btnRegistrarse.Text == "Registrate")
            {
                //Cambio el formulario de un login a uno de un registro
                lblTittle.Text = "Registrate";
                btnConectar.Text = "Cancelar";
                btnRegistrarse.Text = "Registrarse";
                lblId.Text = "Nombre:";
                txtTelefono.Text = "";
                txtId.Text = "";
                lblMessage.Visible = false;
                txtId.Focus();
            }
            else
            {
                // Comprobamos los datos esten llenos para el registro
                if (txtId.Text == "")
                {
                    lblMessage.Text = "Ingresa un nombre de usuario!";
                    lblMessage.Visible=true;
                    txtId.Focus();
                    return;
                }
                if (txtTelefono.Text == "")
                {
                    lblMessage.Text = "Ingresa un telefono!";
                    lblMessage.Visible = true;
                    txtTelefono.Focus();
                    return;
                }
                // Registro un nuevo usuario en la base de datos

                string envio = "3," + txtTelefono.Text;
                EnviarMensaje(envio,clientSocket);
                string recivo = RecibirMensaje(clientSocket);
                if (recivo=="true")
                {
                    lblMessage.Text = "El número de telefono ya esta en uso!";
                    lblMessage.Visible = true;
                    txtTelefono.Focus();
                    return;
                }

                envio = "4," + txtId.Text +","+ txtTelefono.Text;
                EnviarMensaje(envio,clientSocket);
                recivo = RecibirMensaje(clientSocket);
                int id = int.Parse(recivo);
                MessageBox.Show("Usuario nuevo registrado con exito.\nRecuerda que el Id de tu usuario es:" + id,
                    "Usuario Registrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblTittle.Text = "Login";
                btnConectar.Text = "Conectar";
                btnRegistrarse.Text = "Registrate";
                lblId.Text = "ID:";
                txtTelefono.Text = "";
                txtId.Text = "";
                lblMessage.Visible=false;
                txtId.Focus();
            }
        }

        public void posicionBotons()
        {
            lblTittle.Left = (panel1.Width - lblTittle.Width) / 2;
            lblTittle.Top = (panel1.Height - lblTittle.Height) / 2;
            lblMessage.Top = (txtTelefono.Bottom + 10);
            lblMessage.Left = txtTelefono.Left;
            btnConectar.Top = (lblMessage.Bottom + 20);
            btnConectar.Left = (panel2.Width - 2 * btnConectar.Width - 30) / 2;
            btnRegistrarse.Top = (lblMessage.Bottom + 20);
            btnRegistrarse.Left = (btnConectar.Right + 30);
        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            if (btnConectar.Text == "Conectar")
            {
                // Validamos que se ingresen los datos
                if (txtId.Text == "")
                {
                    lblMessage.Text = "Ingresa el ID!";
                    lblMessage.Visible = true;
                    txtId.Focus();
                    return;
                }
                if (txtTelefono.Text == "")
                {
                    lblMessage.Text = "Ingresa un telefono!";
                    lblMessage.Visible = true;
                    txtTelefono.Focus();
                    return;
                }
                /*
                 * No validamos el valor de los datos de entrada ya que
                 * los textbox estan controlados con su evento keypress
                 * para permitir solo ingresar numeros
                 * Validamos que los datos de id y telefono si existan en la base de datos
                 */

                string envio = "1," + txtId.Text + "," + txtTelefono.Text;
                EnviarMensaje(envio, clientSocket);
                string recivo = RecibirMensaje(clientSocket);
                
                if(recivo=="false")
                {
                    lblMessage.Text = "Credenciales incorrectas!";
                    lblMessage.Visible=true;
                    txtTelefono.Text = "";
                    txtId.Text = "";
                    txtId.Focus();
                    return;
                }

                DataLogin.IdUser=int.Parse(txtId.Text);
                DataLogin.Telefono=int.Parse(txtTelefono.Text);
                DataLogin.UserName = recivo;
                //Aqui añadiremos la nueva ip para ello debemos enviar la opcion y la ip
                DataLogin.IP = GetLocalIPAddress().ToString();
                envio ="2,"+DataLogin.IP;
                EnviarMensaje(envio,clientSocket);
                recivo = RecibirMensaje(clientSocket);
                DataLogin.IPs = recivo;
                Mensajes frmMensajes=new Mensajes(this);
                frmMensajes.Show();
                clientSocket.Close();
                this.Hide();
            }
            else
            {
                lblTittle.Text = "Login";
                btnConectar.Text = "Conectar";
                btnRegistrarse.Text = "Registrate";
                lblId.Text = "ID:";
                txtTelefono.Text = "";
                txtId.Text = "";
                lblMessage.Visible = false;
                txtId.Focus();
            }
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)&&lblId.Text=="ID:")
            {
                e.Handled = true;
            }
        }

        private IPAddress GetLocalIPAddress()
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

        static void EnviarMensaje(string message,Socket socket)
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
    }
}
