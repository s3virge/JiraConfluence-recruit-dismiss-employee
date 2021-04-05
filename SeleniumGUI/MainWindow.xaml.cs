using SeleniumAutomation;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SeleniumGUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private User _user = new User();

        public MainWindow() {
            InitializeComponent();
            tbUserName.Focus();
            MouseDown += delegate { DragMove(); };
        }

        private void btnOK_Click(object sender, RoutedEventArgs e) {
            _user.Login = tbFullLogin.Text;
            _user.Name = tbUserName.Text;
            _user.Mail = tbMail.Text;
            _user.Subcontractor = (bool)chBoxSubcontractor.IsChecked;            

            try {
                if (string.IsNullOrEmpty(_user.Login) == true) {
                    throw new Exception("User login cannot be empty.");
                }
                if (string.IsNullOrEmpty(_user.Name) == true) {
                    throw new Exception("User name cannot be empty");
                }
                if (string.IsNullOrEmpty(_user.Country) == true) {
                    throw new Exception("County youser from does not selected.");
                }
                if (string.IsNullOrEmpty(_user.Mail) == true) {
                    throw new Exception("User mailbox cannot be empty");
                }

                RunSeleniumScript(_user);
            }
            catch (Exception seleniumExeption) {
                MessageBox.Show(seleniumExeption.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void RunSeleniumScript(User userToWorkWith) {
            var selenium = new Selenium();
            selenium.ProcessCompleted += Selenium_ProcessCompleted;
            selenium.Launch(userToWorkWith);            
        }

        private void Selenium_ProcessCompleted(object sender, EventArgs e) {
            //MessageBoxResult result = MessageBox.Show($"{_user.Name} ({_user.Login}) was saccessfuly created.\n\nClose the program?", "What to do?",
            //        MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

            //if (result == MessageBoxResult.Yes) {
            //    Close();
            //}
            
            MessageBox.Show($"The employee {_user.Name} ({_user.Login}) was successfully created in jira & confluence.");
        }

        private void CheckeHandler(object sender, RoutedEventArgs e) {
            RadioButton rb = sender as RadioButton;
            _user.Country = rb.Content.ToString();
        }

        //private void TextBoxGotFocus(object sender, RoutedEventArgs e) {
        //    TextBox tb = sender as TextBox;
        //    //if user name fiels is not empty
        //    if (string.IsNullOrEmpty(tbUserName.Text.Trim()) == false) {
        //        //if text feild is empty 
        //        if (string.IsNullOrEmpty(tb.Text) == true) {
        //            //then generate user login
        //            string[] name = tbUserName.Text.ToLower().Split();
        //            tb.Text = $"{name[0].Substring(0, 1)}.{name[1]}@intetics.com";
        //        }
        //        //else do nothing
        //    }
        //}

        private void TextBoxLostFocus(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(tbUserName.Text.Trim()) == false) {
                //then generate user login
                string[] name = tbUserName.Text.ToLower().Split();
                try {
                    tbFullLogin.Text = tbMail.Text = $"{name[0].Substring(0, 1)}.{name[1]}@intetics.com";
                }
                catch (Exception) {
                    MessageBox.Show($"Something wrong with user name {tbUserName.Text}");
                }                
            }
        }
    }
}
