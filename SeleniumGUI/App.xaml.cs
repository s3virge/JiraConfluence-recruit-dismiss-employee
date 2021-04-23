using ActiveDirectoryLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SeleniumGUI {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        App() {
            bool admin = new ActiveDirectory().IsDomainAdministrator();

            if (admin == false) {
                MessageBox.Show("Hey! You are not admin. Bye.");
                Current.Shutdown();
            }
        }
    }
}
