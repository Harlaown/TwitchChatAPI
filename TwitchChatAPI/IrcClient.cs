using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;


namespace TwitchChatAPI
{

    public partial class  MainWindow
    {
        private void GetChatMessages()
        {
            
            while (true)
            {
                try
                {
                    var readData = irc.ReadMessage();
                    Message(readData);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    throw;
                }
            }
        }
        private void Message(string data)
        {
            if (!CheckAccess())
            {
                Action<string> myAction = Message;

                this.Dispatcher.Invoke(myAction, data);
            }
            else
            {
                if (data == null) return;
                if (!data.Contains("PRIVMSG")) return;

                var separetorList = new [] {$"#{Channel} :"};
                var singlesep = new[] {":", "!"};

                

                Paragraph para = new Paragraph();

                string username = data.Split(singlesep, StringSplitOptions.None)[1];
                string message = data.Split(separetorList, StringSplitOptions.None)[1];
                
                
                //if (BannedFilterWord(username, message)) return; 

                //if (message[0].Equals('!')) Command(username,message);

                para.Inlines.Add(username +": ");
                IEmotic img;
                foreach (string s in message.Split(' '))
                {
                    if (EmotionsHashtable.TryGetValue(s, out img))
                    {
                        para.Inlines.Add(Emotion(img, s));
                        para.Inlines.Add(" ");
                    }
                    else
                    {
                        para.Inlines.Add(s + " ");
                    }
                }
                para.Inlines.Add(Environment.NewLine);
                para.LineHeight = 0.01;
                
                ChatBox.Document.Blocks.Add(para);
                
               
            }
        }

        private Image Emotion(IEmotic img,string s)
        {
            
            Image image = new Image();
            BitmapImage bm1 = new BitmapImage();

            
            bm1.BeginInit();
            bm1.UriSource = new Uri(img.Url);
            bm1.CacheOption = BitmapCacheOption.OnLoad;
            bm1.EndInit();

            image.BeginInit();
            image.Source = bm1;
            image.Height = img.Height ?? 0;
            image.Width = img.Width ?? 0;
            image.Stretch = Stretch.UniformToFill;
            image.StretchDirection = StretchDirection.Both;
            image.Tag = s;
            if (img.ImageType.Equals("gif"))
            {
                ImageBehavior.SetAnimatedSource(image, bm1);
            }
            image.MouseEnter += Image_MouseEnter;
            image.MouseLeave += Image_MouseLeave;
            image.EndInit();

            return image;
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            EmoticonName.Content = "";
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            EmoticonName.Content = ((Image)sender)?.Tag; 
        }

       

        
        //private  void Command(string username, string message)
        //{
        //    string command = message.Split(new[] {' ', '!'}, StringSplitOptions.None)[1];

        //    switch (command.ToLower())
        //    {
        //        case "dotabuff":
        //        {
        //           // Console.WriteLine("Dotabaff");
        //                break;
        //        }
        //        case "reward":
        //            if (username == UserName || username == "any of your chat mods")
        //            {
        //                string recipent = message.Split(new string[] {" "}, StringSplitOptions.None)[1];
        //                if (recipent == username) break;
        //                if (recipent[0].Equals('@'))
        //                {
        //                    recipent = recipent.Split(new[] {"@"}, StringSplitOptions.None)[1];
        //                }
        //                string pointsToTranseFersting = message.Split(new string[] { " " }, StringSplitOptions.None)[2];
        //                double pointstotransfer = 0;
        //                bool validnumber =
        //                    double.TryParse(
        //                        pointsToTranseFersting.Split(new string[] {" "}, StringSplitOptions.None)[0],
        //                        out pointstotransfer);
        //                if (!validnumber) break;
        //                if (pointstotransfer > 0)
        //                {
        //                    AddPoint(recipent,pointstotransfer);
        //                }



        //            }
        //            break;
        //        case "charge":
        //            if (username == UserName || username == "any of your chat mods")
        //            {
        //                string recipent = message.Split(new string[] { " " }, StringSplitOptions.None)[1];
        //                if (recipent == username) break;
        //                if (recipent[0].Equals('@'))
        //                {
        //                    recipent = recipent.Split(new[] { "@" }, StringSplitOptions.None)[1];
        //                }
        //                string pointsToTranseFersting = message.Split(new string[] { " " }, StringSplitOptions.None)[2];
        //                double pointstotransfer = 0;
        //                bool validnumber =
        //                    double.TryParse(
        //                        pointsToTranseFersting.Split(new string[] { " " }, StringSplitOptions.None)[0],
        //                        out pointstotransfer);
        //                if (!validnumber) break;
        //                if (pointstotransfer > 0)
        //                {
        //                    AddPoint(recipent, -pointstotransfer);
        //                }



        //            }
        //            break;
        //        case "points":
        //            string yourpoints = Pointsini.InReadValue("#" + Channel + "." + username, "Points");
        //            if (yourpoints.Contains(""))
        //            {
        //                yourpoints = "20";
        //                AddPoint(username, Convert.ToDouble(yourpoints));
        //            }
        //            else
        //            {
        //                double thanpoints = Convert.ToDouble(yourpoints);
        //                if (thanpoints < 20 )
        //                {
        //                    AddPoint(username,20-thanpoints);
        //                    yourpoints = "20";
        //                }
        //            }
        //            break;
        //        default:
        //            break;
                    
        //    }
        //}

        //private  bool BannedFilterWord(string username, string message)
        //{
        //    if (!BannedWords.Any(message.Contains)) return false;
        //    string command = "/timeout " + username + " 10";
        //    return true;
        //}


        //private static void AddPoint(string username, double points)
        //{
        //    try
        //    {
        //        string[] separator = new string[] {@"\r\n"};
        //        username = username.Trim().ToLower();
        //        string pointsOfUser = Pointsini.InReadValue("#" + Channel + "." + username, "Points");
        //        double numberOfPoints = double.Parse(pointsOfUser);
        //        var finalnumber = Convert.ToDouble(numberOfPoints + points);
        //        if(finalnumber >0)
        //            Pointsini.InWriteValue("#" + Channel + "." + username, "Points", finalnumber.ToString(CultureInfo.CurrentCulture));
        //        if(finalnumber <= 0 )
        //            Pointsini.InWriteValue("#" + Channel + "." + username, "Points", "0");

        //    }
        //    catch (Exception)
        //    {
        //        if(points > 0 ) Pointsini.InWriteValue("#" + Channel + "." + username, "Points", points.ToString(CultureInfo.CurrentCulture));

        //    }
        //}

    }

    internal class IrcClient
    {
        private readonly string _userName;
        private  string _channel = "";
        private readonly string _password;

        private readonly TcpClient tcpClient;
        private  StreamReader _input;
        private  StreamWriter _output;

        public IrcClient(int port, string ip, string userName, string password,  string channle)
        {
            tcpClient = new TcpClient(ip, port);
            _password = password;
            _userName = userName;
            
          InitializeComponent();
            JoinRoom(channle);
        }

        private void InitializeComponent()
        {
            _input = new StreamReader(tcpClient.GetStream());
            _output = new StreamWriter(tcpClient.GetStream());
            _output.Write("PASS " + _password + Environment.NewLine
               +"NICK " + _userName + Environment.NewLine
               +"USER " + _userName + " 8 * :" + _userName + Environment.NewLine);
            _output.WriteLine("CAP REQ :twitch.tv/memberships");
            _output.WriteLine("CAP REQ :twitch.tv/commands");
            _output.Flush();
        }

        public void JoinRoom(string channel)
        {
            _channel = channel;
            _output.WriteLine("JOIN #" + channel);
            _output.Flush();
        }

        private void LeaveRoom()
        {
            tcpClient.Close();
            _input.Close();
            _output.Close();
        }

        private  void SendIrcMessage(string message)
        {
            _output.Write(message + Environment.NewLine);
            _output.Flush();
        }

        public void SendChatMessage(string message)
        {
            SendIrcMessage($":{_userName}!{_userName}@{_userName}.tmi.twitch.tv PRIVMSG #{_channel} : {message}");
        }

        public void PingResponse()
        {
            SendIrcMessage($"PONG tmi.twitch.tv\r\n");
        }

        public string ReadMessage()
        {
            string message = _input.ReadLine();
            //PingResponse();
            return message;
        }

        ~IrcClient()
        {
            LeaveRoom();
        }
    }

    internal class InFile
    {
        private readonly string _path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filepath);
        [DllImport("kernel32")]
        private static extern long GetPrivetProfileString(string section, string key, string def, StringBuilder retval,
            int size, string filepath);

        public InFile(string filepath)
        {
            _path = filepath; 
        }

        public void InWriteValue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, this._path);
        }

        public string InReadValue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);

            GetPrivetProfileString(section, key, "", temp, 255, this._path);
            return temp.ToString();
        }

    }



    public static class MyExtensions
    {
        public static Brush ColorViewer(this string viewerTypeName)
        {
            Brush brush;

            if (viewerTypeName.Contains("moderators"))
            {
                brush = Brushes.Red;
                return brush;
            }
            if (viewerTypeName.Contains("staff"))
            {
                brush = Brushes.Green;
                return brush;
            }
            if (viewerTypeName.Contains("admin"))
            {
                brush = Brushes.Blue;
                return brush;
            }
            if (viewerTypeName.Contains("global_mods"))
            {
                brush = Brushes.Green;
                return brush;
            }
            brush = Brushes.Black;
            return brush;
        }
    }
}
