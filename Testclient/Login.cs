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
using System.Text.RegularExpressions;

namespace Testclient
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }
        public Socket socketclient = null;
        Thread threadclient = null;
        Thread listenserver;
        IPAddress ip;
        private static int Port = 8885;//这是服务器的端口
        int state = 0;//用来存放登录状态

        private void Login_Load(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            socketclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ips = new IPAddress(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].Address);
            // IPAddress ip = IPAddress.Parse("192.168.170.147");//过时就过时吧
            IPAddress ip = IPAddress.Parse("123.206.34.221");
            // 192.168.1.100
            //ip = IPAddress.Parse("192.168.4.3");//服务器公网IP123.206.34.221

            try
            {
                socketclient.Connect(ip, Port);
                textBox3.AppendText("连接服务器成功\r\n");
                IDmessage();
                this.button1.Enabled = true;
                this.button1.Enabled = true;
                this.button2.Enabled = true;
                this.button3.Enabled = true;
                this.button4.Enabled = true;
                this.button5.Enabled = true;
                this.button6.Enabled = true;
                this.button8.Enabled = true;

            }
            catch (Exception)
            {
                textBox3.AppendText("连接服务器失败\r\n");
                
                this.button1.Enabled = false;
                this.button2.Enabled = false;
                this.button3.Enabled = false;
                this.button4.Enabled = false;
                this.button5.Enabled = false;
                this.button6.Enabled = false;
                this.button8.Enabled = false;
                return;
            }

            Control.CheckForIllegalCrossThreadCalls = false;
           
            threadclient = new Thread(recieveserver);
            threadclient.IsBackground = true;
            threadclient.Start();
        }//窗体载入事件结束
        private void button2_Click(object sender, EventArgs e)//注册
        {
           
        }

        private void button1_Click(object sender, EventArgs e)//登录
        {
            login(textBox1.Text.Trim()+"@"+textBox2.Text.Trim()+"!");//先发送明文请求进行测试 后期可以设计一个加密类，将明文转换成密文发送
        }

        private void login(string sendMsg)//登录消息
        {

            //默认向服务器发送信息
            //sendMsg = "***" + socketclient.ToString() + "###" + sendMsg;
            sendMsg = "login" + sendMsg;/*读取选定项并且转换成字符串，首尾加标记 里面包含了要发送到的客户端的IP以及端口信息*/
            byte[] arrClientsendMsg = Encoding.UTF8.GetBytes(sendMsg);//转换成字节数组
            
            socketclient.Send(arrClientsendMsg);//调用套接字的send方法是将数据发送到连接的Socket，接下来服务器负责解码Z???连接中断了
               
        }
        private void test(string sendMsg)
        {
            //sendMsg = sendMsg;/*读取选定项并且转换成字符串，首尾加标记 里面包含了要发送到的客户端的IP以及端口信息*/
            byte[] arrClientsendMsg = Encoding.UTF8.GetBytes(sendMsg);//转换成字节数组

            socketclient.Send(arrClientsendMsg);
        }
        private void registemessage(string sendMsg)//注册消息
        {
            sendMsg = "regist" + sendMsg;/*读取选定项并且转换成字符串，首尾加标记 里面包含了要发送到的客户端的IP以及端口信息*/
            byte[] arrClientsendMsg = Encoding.UTF8.GetBytes(sendMsg);//转换成字节数组
            socketclient.Send(arrClientsendMsg);//调用套接字的send方法是将数据发送到连接的Socket，接下来服务器负责解码Z???连接中断了
        }
        private void IDmessage()//身份消息
        {
           // sendMsg = "phone";/*读取选定项并且转换成字符串，首尾加标记 里面包含了要发送到的客户端的IP以及端口信息*/
            byte[] arrClientsendMsg = Encoding.UTF8.GetBytes("phone");//转换成字节数组
            socketclient.Send(arrClientsendMsg);//调用套接字的send方法是将数据发送到连接的Socket，接下来服务器负责解码Z???连接中断了
        }
        private void namecheckmessage(string sendMsg)//用户名检查消息
        {
            sendMsg = "namecheck" + sendMsg;
            byte[] arrClinentsendMsg = Encoding.UTF8.GetBytes(sendMsg);
            socketclient.Send(arrClinentsendMsg);
        }
        private void GetSLockfromDB(string username)//获取智能锁列表消息
        {
            username = "getlock" + username;
            byte[] arrClinentsendMsg = Encoding.UTF8.GetBytes(username);
            socketclient.Send(arrClinentsendMsg);
        }
        private void BindLockMAC(string lockmac)
        {
            lockmac = "bindmac" + lockmac;
            byte[] arrClinentsenMsg = Encoding.UTF8.GetBytes(lockmac);
            socketclient.Send(arrClinentsenMsg);
        }
        private void recieveserver()//client 接收信息 
        {
            //  int x = 0;//用x来判断clientipt是不是已经存放过了
            textBox3.AppendText("消息接收线程启动");
            while (true)
            {
               
                try
                {
                    byte[] arrRecvmsg = new byte[100];//接收缓冲区  里面存放的是每个字符的ASCII码 原来的 1024*1024大小，我觉得内存占太大
                    int length = socketclient.Receive(arrRecvmsg);//返回值为接收到的字节数
                    string strRevMsg = Encoding.UTF8.GetString(arrRecvmsg, 0, length);//将字节序列解码成字符串
                    switch(strRevMsg)
                    {
                        case "login_1\n":
                            {
                                state = 1;
                                MessageBox.Show("登录成功");
                                label8.Text = textBox1.Text;
                                GetSLockfromDB(textBox1.Text.Trim()+"!"); //发出查询智能锁的请求
                            } break;//登录成功
                        case "login_0":
                            {
                                state = 0;
                                MessageBox.Show("登录失败");
                            } break;//登陆失败
                        case "regist_1\n": MessageBox.Show("注册成功") ;break;
                        case "regist_0\n": MessageBox.Show("注册失败") ; break;
                        case "namecheck_1\n":  MessageBox.Show("用户名已存在！"); break;
                        case "namecheck_0\n": MessageBox.Show("可以注册！"); break;
                        default:
                            {
                                byte[] arrClinentsendMsg = Encoding.UTF8.GetBytes("yesiamhere");
                                socketclient.Send(arrClinentsendMsg);
                                textBox3.AppendText(strRevMsg);
                            }
                            break;
                        
                    }
                       
                
                
                }
                catch (Exception ex)
                {

                    textBox3.AppendText ("远程服务器已经中断连接" + "\r\n");
                    label8.Text = "未登录";
                    this.button1.Enabled = false;
                    this.button2.Enabled = false;
                    this.button3.Enabled = false;
                    this.button4.Enabled = false;
                    this.button5.Enabled = false;
                    this.button6.Enabled = false;
                    this.button8.Enabled = false;
                    MessageBox.Show(ex.Message);
                    break;
                }
            }
        }



        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)//注册
        {
            if(textBox5.Text==textBox6.Text&&textBox5.Text!="")
            registemessage(textBox4.Text.Trim() + "@" + textBox5.Text.Trim() + "!");//先发送明文请求进行测试 后期可以设计一个加密类，将明文转换成密文发送
            else
            {
                MessageBox.Show("两次密码输入不一致或为空！请重新输入！");
                textBox6.Text = "";
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (textBox4.Text != "")
                namecheckmessage(textBox4.Text + "!");
            else
            {
                MessageBox.Show("用户名不能为空！");

            }
        }

        

        private void button8_Click(object sender, EventArgs e)//绑定智能锁
        {
            BindLockMAC(textBox7.Text+"!");
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            test(textBox8.Text.ToString());
        }
    }
}
