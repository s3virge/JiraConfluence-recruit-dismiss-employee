using ActiveDirectoryLibrary;
using SeleniumAutomation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
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

        private void btnRecruit_Click(object sender, RoutedEventArgs e) {
            try {
                if (string.IsNullOrEmpty(_user.Login) == true) {
                    throw new Exception("User login cannot be empty.");
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
            MessageBox.Show($"The employee {_user.Name} ({_user.Login}) was successfully created in jira & confluence.");
        }

        private void CheckeHandler(object sender, RoutedEventArgs e) {
            RadioButton rb = sender as RadioButton;
            _user.Country = rb.Content.ToString();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e) {
            using (new WaitCursor())
            try {
                lstEmployees.Items.Clear();
                string currentLDAPDomain = $"LDAP://{Domain.GetComputerDomain()}";
                string employeeToSeek = tbUserName.Text;

                List<string> listOfEmployees = new ActiveDirectory().GetListOfEmployee(currentLDAPDomain, employeeToSeek);

                if (listOfEmployees.Count > 0) {
                    foreach (var item in listOfEmployees) {
                        lstEmployees.Items.Add(item);
                    }
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lstEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ShowEmployeeADInfo(lstEmployees.SelectedItem?.ToString());
        }

        private void ShowEmployeeADInfo(string e) {
            string currentDomain = $"LDAP://{Domain.GetComputerDomain()}";
            Employee empl = new ActiveDirectory().GetEmployee(currentDomain, e);
            tbEmployeeInfo.Text = empl.ToString();
        }
    }
}
