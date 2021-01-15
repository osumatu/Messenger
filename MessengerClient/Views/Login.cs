using MessengerBase.Models;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MessengerClient.Views
{
    public partial class Login : Form
    {
        TcpClient clientSocket;
        NetworkStream serverStream;
        public Login()
        {
            InitializeComponent();
            clientSocket = new TcpClient();
            serverStream = default;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if (Program.serverIp == null || Program.serverPort == -1)
            {
                MessageBox.Show("Configuration file is not set correctly.", "Error!");
                Application.Exit();
            }
            try
            {
                User user = new User(username.Text);
                Program.user = user;
                clientSocket = new TcpClient();
                try
                {
                    clientSocket.Connect(Program.serverIp, Program.serverPort);
                    serverStream = clientSocket.GetStream();
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    byte[] outStream = Encoding.ASCII.GetBytes(username.Text);
                    serverStream.Write(outStream, 0, outStream.Length);
                    serverStream.Flush();
                    Program.clientSocket = clientSocket;
                    Program.serverStream = serverStream;

                    Hide();
                    MainWindow mw = new MainWindow
                    {
                        Text = "Home - " + user.Username
                    };
                    mw.Show();
                    mw.FormClosed += (s, ev) =>
                    {
                        try
                        {
                            Show();
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine(ex);
                            Application.Exit();
                        }
                    };
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message, "Error!");
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                Application.Exit();
            }
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (Thread th in Program.activeThreads)
            {
                th.Interrupt();
            }
            Environment.Exit(0);
        }

        private void Login_Load(object sender, EventArgs e)
        {
            clientSocket = new TcpClient();
            serverStream = default;
        }

        private void Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                LoginButton_Click(sender, new EventArgs());
        }

        private void Username_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                LoginButton_Click(sender, new EventArgs());
        }
    }
}
