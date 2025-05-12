using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AllWindows
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    
    public partial class RegisterWindow : Window
    {
        string fileName = "UserData.json";
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Close_Button(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Minimize_Button(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Login_Button(object sender, RoutedEventArgs e)
        {
            LoginWindow loginApp = new();
            loginApp.Show();
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Register_Button(object sender, RoutedEventArgs e)
        {
            SaveUser(fileName);
            LoginWindow loginApp = new();
            loginApp.Show();
            this.Close();
        }

        private void SaveUser(string fileName)
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Text;
            string password2 = Password2TextBox.Text;

            // провірить чи введені правильні данні
            if (String.IsNullOrWhiteSpace(login) || String.IsNullOrWhiteSpace(password) || password != password2)
            {
                MessageBox.Show("Please enter valid login and password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            } 

            if(!File.Exists(fileName))
            {
                File.Create(fileName);
            }

            // робимо список юзерів щоб десереалізувати і потім додати нового
            List<User> users = new List<User>();
            if(File.Exists(fileName))
            {
                string json = File.ReadAllText(fileName);
                users = JsonConvert.DeserializeObject<List<User>>(json)?? new List<User>();

                
                foreach(var user in users)
                {
                    if(user.Login == login)
                    {
                        MessageBox.Show("User with this login already exists!");
                        return;                       
                    }
                }
                User newUser = new User()
                {
                    Login = login,
                    Password = password
                };
                users.Add(newUser);

                string newJson = JsonConvert.SerializeObject(users);
                File.WriteAllText(fileName, newJson);
                MessageBox.Show("Successfully registered","Succesfully",MessageBoxButton.OK,MessageBoxImage.Information);
            }

        }
    }


}
