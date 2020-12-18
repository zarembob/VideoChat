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

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for CallRequest.xaml
    /// </summary>
    public partial class CallRequest : Window
    {
        public CallRequest(string data)
        {
            InitializeComponent();
            this.DataContext = data;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Data.answer = true;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Data.answer = false;
            this.Close();
        }
    }
}
