using MessengerBase.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MessengerClient.Views
{
    public partial class MainWindow : Form
    {
        List<Chat> chats = new List<Chat>();
        List<string> onlineUsers = new List<string>();
        TcpClient clientSocket = Program.clientSocket;
        NetworkStream serverStream = Program.serverStream;
        string readData = null;
        int bytesRead;
        Package pack = new Package();

        public MainWindow()
        {
            InitializeComponent();
            drawOnlineUsers();
            drawChatPage();

            readData = "Conected to Chat Server";
            Thread ctThread = new Thread(getPackage);
            ctThread.Start();
            Program.activeThreads.Add(ctThread);

        }
        void drawOnlineUsers()
        {
            panel1.RowCount = 0;
            int locationCount = 0;
            var activeChat = getActiveChat();

            #region DRAW ONLINE USERS
            if (onlineUsers.Count > 0)
            {
                foreach (string username in onlineUsers)
                {
                    if (username.Equals(Program.user.username))
                        continue;
                    User user = new User(username);
                    Chat chat = new Chat();
                    chat.client1 = Program.user;
                    chat.client2 = user;
                    chats.Add(chat);

                    #region components

                    Label statusLabel = new Label();
                    Label nicknamelabel = new Label();
                    PictureBox userPP = new PictureBox();
                    Label message = new Label();
                    Panel messagePanel = new Panel();
                    Panel panel = new Panel();

                    // 
                    // statusLabel
                    // 
                    statusLabel.AutoSize = true;
                    statusLabel.ForeColor = Color.White;
                    statusLabel.Location = new System.Drawing.Point(35, 20);
                    statusLabel.Name = "statusLabel";
                    statusLabel.Size = new System.Drawing.Size(37, 13);
                    statusLabel.TabIndex = 3;
                    statusLabel.Text = "Online";
                    statusLabel.MouseEnter += (s, e) =>
                    {
                        ((Label)s).Parent.BackColor = Color.FromArgb(66, 66, 66);
                    };
                    statusLabel.MouseLeave += (s, e) =>
                    {
                        if (activeChat != null)
                            if (activeChat.client2 != null)
                                if (((Label)s).Parent.Name.Equals(activeChat.client2.username))
                                    return;
                        ((Label)s).Parent.BackColor = Color.FromArgb(33, 33, 33);
                    };
                    statusLabel.MouseClick += (s, e) =>
                    {
                        mouseClick(s, e, user);
                    };

                    // 
                    // nicknamelabel
                    // 
                    nicknamelabel.AutoSize = true;
                    nicknamelabel.ForeColor = Color.White;
                    nicknamelabel.Location = new System.Drawing.Point(35, 7);
                    nicknamelabel.Name = "nicknamelabel";
                    nicknamelabel.Size = new System.Drawing.Size(55, 13);
                    nicknamelabel.TabIndex = 4;
                    nicknamelabel.Text = user.username;
                    nicknamelabel.MouseEnter += (s, e) =>
                    {
                        ((Label)s).Parent.BackColor = Color.FromArgb(66, 66, 66);
                    };
                    nicknamelabel.MouseLeave += (s, e) =>
                    {
                        if (v != null)
                            if (activeChat.client2 != null)
                                if (((Label)s).Parent.Name.Equals(activeChat.client2.username))
                                    return;
                        ((Label)s).Parent.BackColor = Color.FromArgb(33, 33, 33);
                    };
                    nicknamelabel.MouseClick += (s, e) =>
                    {
                        mouseClick(s, e, user);
                    };


                    // 
                    // userPP
                    // 
                    userPP.Location = new System.Drawing.Point(3, 3);
                    userPP.Name = "userPP";
                    userPP.Size = new System.Drawing.Size(34, 34);
                    userPP.SizeMode = PictureBoxSizeMode.CenterImage;
                    userPP.TabIndex = 2;
                    userPP.TabStop = false;
                    userPP.MouseEnter += (s, e) =>
                    {
                        ((PictureBox)s).Parent.BackColor = Color.FromArgb(66, 66, 66);
                    };
                    userPP.MouseLeave += (s, e) =>
                    {
                        if (activeChat != null)
                            if (activeChat.client2 != null)
                                if (((PictureBox)s).Parent.Name.Equals(activeChat.client2.username))
                                    return;
                        ((PictureBox)s).Parent.BackColor = Color.FromArgb(33, 33, 33);
                    };
                    userPP.MouseClick += (s, e) =>
                    {
                        mouseClick(s, e, user);
                    };

                    // 
                    // message
                    // 
                    message.AutoSize = true;
                    message.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(162)));
                    message.ForeColor = Color.White;
                    message.Location = new System.Drawing.Point(4, 4);
                    message.Name = "message";
                    message.Size = new System.Drawing.Size(14, 13);
                    message.TabIndex = 1;
                    message.Text = "0";
                    // 
                    // messagePanel
                    // 
                    messagePanel.Controls.Add(message);
                    messagePanel.Location = new System.Drawing.Point(156, 9);
                    messagePanel.Name = "messagePanel";
                    messagePanel.Size = new System.Drawing.Size(22, 22);
                    messagePanel.TabIndex = 2;
                    messagePanel.Visible = false;
                    // 
                    // panel
                    // 
                    panel.Anchor = ((System.Windows.Forms.AnchorStyles)((AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top)));
                    panel.Controls.Add(userPP);
                    panel.Controls.Add(statusLabel);
                    panel.Controls.Add(nicknamelabel);
                    panel.Name = user.username;
                    panel.Size = new System.Drawing.Size(205, 40);
                    panel.TabIndex = 6;
                    panel.MouseEnter += (s, e) =>
                    {
                        ((Panel)s).BackColor = Color.FromArgb(66, 66, 66);
                    };
                    panel.MouseLeave += (s, e) =>
                    {
                        if (activeChat != null)
                            if (activeChat.client2 != null)
                                if (((Panel)s).Name.Equals(activeChat.client2.username))
                                    return;
                        ((Panel)s).BackColor = Color.FromArgb(33, 33, 33);
                    };
                    panel.MouseClick += (s, e) =>
                    {
                        mouseClick(s, e, user);
                    };

                    #endregion

                    panel.Controls.Add(messagePanel);
                    panel.Controls.Add(statusLabel);
                    panel.Controls.Add(nicknamelabel);
                    panel.Controls.Add(userPP);

                    panel1.RowCount++;
                    panel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
                    panel1.Controls.Add(panel, 0, locationCount);
                    statusLabel = null;
                    nicknamelabel = null;
                    userPP = null;
                    message = null;
                    messagePanel = null;
                    panel = null;
                    locationCount++;
                }
            }
            #endregion
        }

        /// <summary>
        /// Mouse Click event for online user list. Basically sets right chat pane for selected chat.
        /// </summary>
        private void mouseClick(object s, EventArgs e, User user)
        {
            Control p = new Control(); ;
            if (s is Label)
            {
                p = ((Label)s).Parent.Controls.Find("messagePanel", true)[0];
                setActiveChat(new Chat(Program.user, new User(((Label)s).Name)));
            }
            else if (s is Panel)
            {
                p = ((Panel)s).Controls.Find("messagePanel", true)[0];
                setActiveChat(new Chat(Program.user, new User(((Panel)s).Name)));
            }
            else if (s is PictureBox)
            {
                p = ((PictureBox)s).Parent.Controls.Find("messagePanel", true)[0];
                setActiveChat(new Chat(Program.user, new User(((PictureBox)s).Name)));
            }
            p.Visible = false;
            p.Controls.Find("message", true)[0].Text = "0";
            redrawOnlineList();
            drawChatPage();
        }
        void drawChatPage()
        {
            TableLayoutPanel panel = new TableLayoutPanel();
            Label warninglabel = new Label();
            var activeChat = getActiveChat();
            #region noactivechat
            if (activeChat is null)
            {
                activeChatPage.Visible = false;
                // 
                // warninglabel
                // 
                warninglabel.AutoSize = true;
                warninglabel.Dock = DockStyle.Fill;
                warninglabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(162)));
                warninglabel.ForeColor = Color.White;
                warninglabel.Location = new System.Drawing.Point(3, 0);
                warninglabel.Name = "label2";
                warninglabel.Size = new System.Drawing.Size(877, 750);
                warninglabel.TabIndex = 1;
                warninglabel.Text = "Choise a friend to start chating!";
                warninglabel.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // panel
                // 
                panel.ColumnCount = 1;
                panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(SizeType.Percent, 100F));
                panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(SizeType.Absolute, 20F));
                panel.Controls.Add(warninglabel, 0, 0);
                panel.Dock = DockStyle.Fill;
                panel.Location = new System.Drawing.Point(3, 3);
                panel.Name = "offpanel";
                panel.RowCount = 1;
                panel.RowStyles.Add(new System.Windows.Forms.RowStyle(SizeType.Percent, 100F));
                panel.RowStyles.Add(new System.Windows.Forms.RowStyle(SizeType.Absolute, 20F));
                panel.Size = new System.Drawing.Size(883, 750);
                panel.TabIndex = 0;
                ChatPage.Controls.Add(panel);
            }
            #endregion
            else
            {
                activeChatPage.Visible = true;
                messages.Text = activeChat.chatHistory;
                ChatPage.Controls.Find("offpanel", true)[0].Visible = false;
            }

        }
        Chat getChat(Chat chat)
        {
            var c = chats.Find(c => c.client1.username.Equals(chat.client1.username) && c.client2.username.Equals(chat.client2.username));
            if (c is null)
            {
                chats.Add(chat);
                return chat;
            }
            return c;
        }

        // ToDo: Find a better way to handle active chat.
        Chat getActiveChat()
        {
            return chats.Find(c => c.isActive);
        }
        void setActiveChat(Chat chat)
        {
            chats.ForEach(c =>
            {
                if (c.client1.username.Equals(chat.client1.username) && c.client2.username.Equals(chat.client2.username))
                    c.isActive = true;
                else
                    c.isActive = false;
            });

        }

        private void getPackage()
        {
            try
            {
                while (true)
                {
                    bytesRead = 0;
                    serverStream = clientSocket.GetStream();
                    int buffSize = 0;
                    byte[] inStream = new byte[10025];
                    buffSize = clientSocket.ReceiveBufferSize;
                    bytesRead = serverStream.Read(inStream, 0, inStream.Length);
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    string returndata = encoder.GetString(inStream, 0, bytesRead);

                    Package pck = new Package();
                    pck = pck.Deserialize(returndata);

                    switch (pck.packageType)
                    {
                        case 1:
                            // INCOMING MESSAGE
                            readData = pck.context;
                            pack = pck;
                            processMessage();
                            break;

                        case 2:
                            // USER CONNECTED/DISCONNECTED
                            onlineUsers = pck.DeserializeClients(pck.context);
                            redrawOnlineList();
                            break;

                        case 4:
                            // SEND FAILED
                            readData = "Couldn't deliver message.";
                            processMessage();
                            break;
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("Lost connection with the server.", "Error!");
                Console.WriteLine(ex);
                foreach (Thread th in Program.activeThreads)
                {
                    th.Abort();
                }
                Environment.Exit(0);

            }
        }
        private void processMessage()
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(processMessage));
            else
            {
                try
                {
                    Chat c = new Chat(pack.receiverUser, pack.senderUser);
                    getChat(c).chatHistory += Environment.NewLine + " << " + readData;
                    messagePopUp();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        private void messagePopUp()
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(messagePopUp));
            else
            {
                string username = pack.senderUser.username;
                foreach (Control c in panel1.Controls)
                {
                    var activeChat = getActiveChat();
                    if (activeChat != null && activeChat.client2 != null)
                    {
                        if (activeChat.client2.username == c.Name)
                            redrawChatPage();
                        return;
                    }
                    if (c.Name == username)
                    {
                        c.Controls.Find("messagePanel", true)[0].Visible = true;
                        c.Controls.Find("message", true)[0].Text = (Convert.ToInt32(c.Controls.Find("message", true)[0].Text) + 1).ToString();
                    }
                }
            }
        }
        private void redrawOnlineList()
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(redrawOnlineList));
            else
            {
                panel1.Controls.Clear();
                drawOnlineUsers();
            }

        }
        private void redrawChatPage()
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(redrawChatPage));
            else
            {
                messages.Text = getActiveChat().chatHistory;
            }

        }
        private void sendButton_Click(object sender, EventArgs e)
        {
            var activeChat = getActiveChat();
            Package message = new Package(messageTextBox.Text.TrimEnd(), activeChat.client1, activeChat.client2, 1);
            message.sendPackage(serverStream);

            messages.AppendText(Environment.NewLine);
            messages.AppendText(String.Format(">> {0}", messageTextBox.Text));

            chats[chats.FindIndex(x => x.Equals(activeChat))].chatHistory += Environment.NewLine + ">> " + messageTextBox.Text;
            messageTextBox.Clear();
        }
        private void messages_TextChanged(object sender, EventArgs e)
        {
            messages.SelectionStart = messages.Text.Length;
            messages.ScrollToCaret();
        }

        private void options_Click(object sender, EventArgs e)
        {

        }

        private void messageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                if (!String.IsNullOrEmpty(messageTextBox.Text))
                {
                    sendButton_Click(sender, new EventArgs());
                    messageTextBox.Clear();
                }
        }

        private void messageTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                messageTextBox.Clear();
        }
    }
}