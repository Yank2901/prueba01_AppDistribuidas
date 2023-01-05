using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Domain;

namespace Presentation
{
    public partial class Mensajes : Form
    {
        UserModel _user = new UserModel();
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
            this.Close();
            _user.eliminarIP(GetLocalIPAddress().ToString());
            login1.ShowForm();
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
    }
}
