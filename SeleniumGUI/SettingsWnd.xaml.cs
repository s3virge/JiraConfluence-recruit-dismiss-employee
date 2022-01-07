using Microsoft.Win32;
using SeleniumAutomationLibrary;
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

namespace SeleniumGUI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SettingsWnd : Window
    {
       
        public SettingsWnd()
        {
            InitializeComponent();
            string login, password = null;
            Settings.ReadLoginPasswordFromRegestry(out login, out password);
            tbLogin.Text = login;
            tbPassword.Password = password;
            tbPassword.Focus();            
        }
     
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Settings.SaveLoginPasswordToRegistry(tbLogin.Text, tbPassword.Password);
            Close();
        }       
    }
}
