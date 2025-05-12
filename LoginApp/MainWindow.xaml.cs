using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoginApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int port = 4040;
        string ip = "127.0.0.1";
        private string _nickname;

        byte[] attachment;
        public ObservableCollection<ChatsInfo> chatsinfo { get; set; }

        private ChatsInfo _selectedChat;
        public ChatsInfo SelectedChat
        {
            get => _selectedChat;
            set
            {
                _selectedChat = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public MainWindow(string login)
        {
            InitializeComponent();
            //WindowState = WindowState.Maximized;
            _nickname = login;
            MessageBox.Show(login);
            chatsinfo = new ObservableCollection<ChatsInfo>();

            ObservableCollection<MessageInfo> messageinfo = new();
            MessageInfo messageInfo = new() { message = "" };
            messageinfo.Add(messageInfo);

            ChatsInfo testchat = new() { Title = "Group chat", Messages = messageinfo };
            chatsinfo.Add(testchat);            

            SelectedChat = chatsinfo.FirstOrDefault();
            Task.Run(() => Listener());
            DataContext = this;

        }

        private void Close_Button(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Minimize_Button(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Maximize_Button(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void ShowChat_Button(object sender, RoutedEventArgs e)
        {

        }

        private void SendMessage_Btn(object sender, RoutedEventArgs e)
        {
            using(TcpClient client = new(ip,port))
            {
                NetworkStream ns = client.GetStream();
                StreamWriter sw = new(ns);
                sw.AutoFlush = true;
                if(attachment != null)
                {
                    sw.Write(attachment);
                    attachment = null;
                }
                sw.Write($"{_nickname} : {MessageTextBox.Text}");
                MessageTextBox.Text = "";
            }
        }



        private void Listener()
        {
            while (true)
            {
                try
                {
                    using (TcpClient client = new(ip, port))
                    {
                        NetworkStream ns = client.GetStream();
                        StreamReader sr = new(ns);

                        string? line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            Dispatcher.Invoke(() =>
                            {
                            SelectedChat.Messages.Add(new MessageInfo { message = line, Time = DateTime.Now });
                            });
                        }
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ChatListBoxSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (ChatListBox.SelectedItem is ChatsInfo chat)
            {
                SelectedChat = chat;
            }
        }


    

    }



    [AddINotifyPropertyChangedInterface]
    public class ChatsInfo
    {
        public string Title { get; set; }
        public ObservableCollection<MessageInfo> Messages { get; set; } = new ObservableCollection<MessageInfo>();

        public string LastMessage => Messages.LastOrDefault()?.message;

    }

    [AddINotifyPropertyChangedInterface]
    public class MessageInfo
    {
        public string message { get; set; }

        public DateTime Time { get; set; }

        public string time { get => Time.ToShortTimeString(); }
    }
}

    

