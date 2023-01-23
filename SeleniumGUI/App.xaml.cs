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
                var ad = new ActiveDirectory();
                bool admin = ad.IsDomainAdministrator();

                if (admin == false)
                {
                    MessageBox.Show("Hey! You are not admin. Bye.", "GO ahead", MessageBoxButton.OK, MessageBoxImage.Error);
                    Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ha! Something went wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }
        }
    }
}
