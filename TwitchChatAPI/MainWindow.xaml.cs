using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Newtonsoft.Json;


namespace TwitchChatAPI
{
    
    public partial class MainWindow : Window
    {
        
        #region Variables
        
        private bool m_autoScroll = true;
        private readonly IrcClient irc = new IrcClient(6667, "irc.chat.twitch.tv", UserName, Password, Channel);

        #region AccountInfomation

        //Your user name in twitch.com
        private const string UserName = @"";

        //Get IT in https://twitchapps.com/tmi/
        private const string Password = @"";

        //Channle Twitch ID
        private const string Channel = @"";

        private const string TwitchClientId = @"";

        #endregion

        private static readonly Dictionary<string, IEmotic> EmotionsHashtable = new Dictionary<string, IEmotic>();
        
        //private readonly List<string> BannedWords = new List<string> { "cat", "kitty" };
        //private static readonly InFile Pointsini = new InFile(@"D:\Prog\TwitchChatAPI\TwitchChatAPI\Points.ini");
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            EndChatBtn.Visibility = Visibility.Hidden;
            EmoticonName.Content = "";
            var mTimer = new DispatcherTimer();
            mTimer.Tick += TimerTick;
            mTimer.Interval = new TimeSpan(0, 0, 1, 30);
            mTimer.Start();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var t = new Task(LoadEmotion);
            t.Start();
            

            var chatTask = new Task(GetChatMessages);
            chatTask.Start();

            TimerTick(null,null);

        }

        #region EmotiLoad

        private static void LoadEmotion()
        {
            EmoticonAdd();
            EmoticonBttvAdd();
        }

        private static void EmoticonBttvDownload(out object btv)
        {
            var ebttvdown = new Bttv();
            var req = WebRequest.Create(new Uri($@"https://api.betterttv.net/emotes"));
            //req.Timeout = 3000;
            var resp = req.GetResponseAsync();
            try
            {
                using (Stream s = resp.Result.GetResponseStream())
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    
                    ebttvdown = serializer.Deserialize<Bttv>(reader);
                }
            }
            catch (Exception)
            {
                EmoticonBttvDownload(out btv);
            }
            finally
            {
                btv = ebttvdown;
            }
        }
        private static void EmoticonDownload(out object e)
        {
            var edown = new Twitch();
            var req = WebRequest.Create(new Uri($@"https://api.twitch.tv/kraken/chat/emoticons"));
            //req.Timeout = 3000;
            Task<WebResponse> resp = req.GetResponseAsync();
            try
            {
                using (Stream s = resp.Result.GetResponseStream())
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    edown = serializer.Deserialize<Twitch>(reader);
                }
            }
            catch (Exception)
            {
                EmoticonDownload(out e);
            }
            finally
            {
                e = edown;
            }
        }

        private static void EmoticonAdd()
        {
            EmoticonDownload(out object p);

            foreach (var emoticon in ((Twitch)p).Emoticons)
            {
                try
                {
                    EmotionsHashtable.Add(emoticon.Regex, emoticon.Images[0]);
                }
                catch (ArgumentException)
                {
                    // ignored
                }
            }
        }
        private static void EmoticonBttvAdd()
        {
            EmoticonBttvDownload(out object bttv);
            foreach (var emote in ((Bttv)bttv).Emotes)
            {
                try
                {
                    EmotionsHashtable.Add(emote.Regex, emote);
                }
                catch (ArgumentException)
                {
                    // ignore
                }
            }
        }

        

        #endregion


        #region Chatter/Viwers

        private static void ChattersDowload(out Chatters c)
        {
            var req = WebRequest.Create(
                new Uri($@"http://tmi.twitch.tv/group/user/{Channel}/chatters"));
            // req.Headers.Add($"Client-ID:{TwitchClientId}");

            var response = req.GetResponseAsync();
            try
            {
                using (System.IO.Stream s = response.Result.GetResponseStream())
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    c = serializer.Deserialize<Chatters>(reader);
                }
                
            }
            catch (Exception)
            {
                ChattersDowload(out c);
            }

        }

        private static void TwitchStreamInfoLoad(ref TwitchStreamInfo t)
        {
            var req = WebRequest.Create(new Uri($"https://api.twitch.tv/kraken/streams/{Channel}"));
            req.Headers.Add($"Client-ID: {TwitchClientId}");
            //req.Timeout = 3000;
            var resp = req.GetResponse();
            using (Stream s = resp.GetResponseStream())
            using (StreamReader sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                //Clipboard.SetText(sr.ReadToEnd());
                JsonSerializer serializer = new JsonSerializer();
                t = serializer.Deserialize<TwitchStreamInfo>(reader);
            }
        }
        

        private void TimerTick(object sender, EventArgs e)
        {         

            ChattersDowload(out Chatters allChatters);
            var tsi = new TwitchStreamInfo();
            TwitchStreamInfoLoad(ref tsi);
            if (tsi.Stream != null)
            ViewerLabel.Content = "Viewers " + ((tsi.Stream.Viewers) ?? 1);
            else
            {
                ViewerLabel.Content = "Viewers " + 0;
            }
            var props = allChatters.Chatter.GetType().GetRuntimeProperties();

            ViewerList.Items.Clear();

            foreach (var prop in props)
            {
                var getMethodProperty = prop.GetMethod;
                var chatters = (IList)getMethodProperty.Invoke(allChatters.Chatter, null);
                if (chatters.Count < 1) continue;


                ViewerList.Items.Add(new ListBoxItem
                {
                    Content = prop.Name,
                    FontSize = 18,
                    Foreground = Brushes.DarkRed
                    
                });

                foreach (var chatter in chatters)
                {
                    
                    ViewerList.Items.Add(new ListBoxItem
                    {
                        Content = chatter.ToString(),
                        Foreground = prop.Name.ToLower().ColorViewer()
                    });
                }

                ViewerList.Items.Add(Environment.NewLine);

            }
            irc.PingResponse();

        }

        #endregion


        #region Events

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            Environment.Exit(0);
        }

        private void BotChatMessage_GotFocus(object sender, RoutedEventArgs e)
        {
            BotChatMessage.Text = "";
        }

        private void BotChatMessage_LostFocus(object sender, RoutedEventArgs e)
        {
            if (BotChatMessage.Text.Equals(""))
            {
                BotChatMessage.Text = "Message Bot...";
            }
        }

        private void BotChatMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            //ChatTwitch.Title = BotChatMessage.Text;
            BotChatMessage.Text = "";
            
        }

        private void ChatBox_SourceUpdated(object sender, TextChangedEventArgs e)
        {
            if (m_autoScroll)
                ChatBox.ScrollToEnd();
        }

        private void ChatBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            m_autoScroll = false;
            EndChatBtn.Visibility = Visibility.Visible;
        }

        private void EndChatBtn_Click(object sender, RoutedEventArgs e)
        {
            EndChatBtn.Visibility = Visibility.Hidden;
            ChatBox.ScrollToEnd();
            m_autoScroll = true;
        }

        #endregion

    }
}
