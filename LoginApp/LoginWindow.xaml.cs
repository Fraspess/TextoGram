using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using LoginApp;

namespace AllWindows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        string fileName = "userData.Json";
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Minimize_Button(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Close_Button(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Register_Button(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerApp = new();
            registerApp.Show();
            this.Close();
        }

        private void Login_Button(object sender, RoutedEventArgs e)
        {
            var users = GetUsers(fileName);

            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Password;

            foreach(var user in users)
            {
                if (user.Login == login && user.Password == password)
                {
                    MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow mainApp = new(login);
                    mainApp.Show();
                    this.Close();
                    return;
                }
            }

            MessageBox.Show("Invalid login or password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private List<User> GetUsers(string fileName)
        {
            List<User> users = new List<User>();
            try
            {
                string json = File.ReadAllText(fileName);
                users = JsonConvert.DeserializeObject<List<User>>(json);

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }

          return users;

        }
            
       

    }


}
