using System;
using System.Collections.Generic;
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

namespace ChatClient.Auntefication
{
    /// <summary>
    /// Interaction logic for RegisterN.xaml
    /// </summary>
    public partial class RegisterN : Window
    {
        public RegisterN()
        {
            InitializeComponent();
        }
        ClientHelper helper = new ClientHelper();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            helper.Option("Register");
            var client = new ClientDTO
            {
                Username = username.Text,
                Email = email.Text,
                Password = password.Password,
                Port = 2020,
                address = "",

            };
            helper.SendClient(client);
            helper.Option("CheckRegister");
            var callbackString = helper.AcceptCallback();
            helper.CheckResult(callbackString);
            this.Close();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
