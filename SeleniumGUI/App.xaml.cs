using ActiveDirectoryLibrary;
using System;
using System.Windows;

namespace SeleniumGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        App() {
            try
            {
                bool admin = new ActiveDirectory().IsDomainAdministrator();

                if (admin == false)
                {
                    MessageBox.Show("Hey! You are not admin. Bye.", "GO ahead", MessageBoxButton.OK, MessageBoxImage.Error);
                    Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Domain is missing", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }
        }
    }
}
