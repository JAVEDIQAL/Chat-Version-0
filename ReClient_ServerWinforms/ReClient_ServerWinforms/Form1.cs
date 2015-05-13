using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
namespace ReClient_ServerWinforms
{
    public partial class Form1 : Form
    {
        private static string shortFileName = "";
        private static string fileName = "";
        TcpClient clientSocket = new TcpClient();
        public Form1()
        {
            InitializeComponent();
        }

        public void Thread()
        {


            IPAddress ipaddr = IPAddress.Parse("127.0.0.1");

            TcpListener serverSocket = new TcpListener(ipaddr, 8004);

            int requestCount = 0;

            TcpClient clientSocket = default(TcpClient);

            serverSocket.Start();

            Console.WriteLine(" >> Server Started");

            clientSocket = serverSocket.AcceptTcpClient();

            Console.WriteLine(" >> Accept connection from client");

            requestCount = 0;

            #pragma warning disable

            while ((true))
            {

                try
                {

                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
                    networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    MessageBox.Show(dataFromClient);
                    string serverResponse = " Last Message from client " + dataFromClient;
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                    Console.WriteLine(" >> " + serverResponse);

                }

                catch (Exception ex)
                {

                    Console.WriteLine(ex.ToString());

                }

            }

            //  clientSocket.Close();

            // serverSocket.Stop();

            Console.WriteLine(" >> exit");
            #pragma warning restore

            Console.ReadLine();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

            ThreadStart threaddelegate = new ThreadStart(Thread);
             Thread newthread = new Thread(threaddelegate);
             newthread.Start(); 

            msg("Client Started");

            clientSocket.Connect("127.0.0.1", 8004);

            label1.Text = "Client Socket Program - Server Connected ...";

        }

        public void msg(string mesg)
        {

            textBox1.Text = textBox1.Text + Environment.NewLine + " >> " + mesg;

        }

        private void send_Click(object sender, EventArgs e)
        {

            NetworkStream serverStream = clientSocket.GetStream();

            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(textBox2.Text + "$");

            serverStream.Write(outStream, 0, outStream.Length);

            serverStream.Flush();



            byte[] inStream = new byte[clientSocket.ReceiveBufferSize];

            serverStream.Read(inStream, 0, inStream.Length);

            string returndata = System.Text.Encoding.ASCII.GetString(inStream);

            msg(returndata);

            textBox2.Text = "";

            textBox2.Focus();
            


        }
        private void btnBrowse_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "File Sharing Client";
            dlg.ShowDialog();
            textBox3.Text = dlg.FileName;
            fileName = dlg.FileName;
            shortFileName = dlg.SafeFileName;

        }

        private void btnSend_Click_1(object sender, EventArgs e)
        {
            string ipAddress = "127.0.0.1";
            int port = 8004;
            string fileName = textBox3.Text;
            
                Task.Factory.StartNew(() => SendFile(ipAddress, port, fileName, shortFileName));
                MessageBox.Show("File Sent");
          
            

        }
        public void SendFile(string remoteHostIP, int remoteHostPort, string longFileName, string shortFileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(remoteHostIP))
                {
                    byte[] fileNameByte = Encoding.ASCII.GetBytes(shortFileName);
                    byte[] fileData = File.ReadAllBytes(longFileName);
                    byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
                    byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);                    
                    fileNameLen.CopyTo(clientData, 0);
                    fileNameByte.CopyTo(clientData, 4);
                    fileData.CopyTo(clientData, 4 + fileNameByte.Length);
                    TcpClient clientSocket = new TcpClient(remoteHostIP, remoteHostPort);
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Write(clientData, 0, clientData.GetLength(0));
                    networkStream.Close();
                }
            }
            catch (Exception exx)
            {

                MessageBox.Show(exx.Message);
            }
        }
























        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

       

       















    }
}
