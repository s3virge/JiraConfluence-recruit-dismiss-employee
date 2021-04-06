using ActiveDirectoryLibrary;
using SeleniumAutomationLibrary;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Windows;
using System.Windows.Controls;

namespace SeleniumGUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Employee _employee = new Employee();

        public MainWindow() {
            InitializeComponent();
            tbUserName.Focus();
            MouseDown += delegate { DragMove(); };
        }

        private void btnRecruit_Click(object sender, RoutedEventArgs e) {
            try {                
                if (string.IsNullOrEmpty(_employee.Login) == true) {
                    tbUserName.Focus();
                    throw new Exception("Employee to recruit does not selected.");
                }
                
                if (string.IsNullOrEmpty(_employee.Country) == true) {
                    radiobtnUkraine.Focus();
                    throw new Exception("Employee country from does not selected.");
                }

                var result = MessageBox.Show($"Coworker {_employee.FullName} will be created. \n\nDo you want to continue?", "Continue", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes) {
                    StartCreatingAnEmployee(_employee);
                }                
            }
            catch (Exception seleniumExeption) {
                MessageBox.Show(seleniumExeption.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDismiss_Click(object sender, RoutedEventArgs e) {
            try {
                if (string.IsNullOrEmpty(_employee.Login) == true) {
                    tbUserName.Focus();
                    throw new Exception("Employee to dismiss does not selected.");
                }

                var result = MessageBox.Show($"Coworker {_employee.FullName} will be dissmis. \n\nDo you want to continue?", "Continue", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes) {
                    StartDismissingEmployee(_employee);
                }
            }
            catch (Exception seleniumExeption) {
                MessageBox.Show(seleniumExeption.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartDismissingEmployee(Employee employee) {
            var dismissingAnEmployee = new DismissingAnEmployee();
            dismissingAnEmployee.ProcessCompleted += Selenium_ProcessCompleted;
            dismissingAnEmployee.Launch(employee);
        }        

        private void StartCreatingAnEmployee(Employee userToWorkWith) {
            var creatingAnEmployee = new CreatingAnEmployee();
            creatingAnEmployee.ProcessCompleted += Selenium_ProcessCompleted;
            creatingAnEmployee.Launch(userToWorkWith);            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void Selenium_ProcessCompleted(object sender, EventArgs e) {
            MessageBox.Show($"The employee {_employee.FullName} ({_employee.Login}) was successfully created in jira & confluence.");
        }

        private void CheckeHandler(object sender, RoutedEventArgs e) {
            RadioButton rb = sender as RadioButton;
            string rbContent = rb.Content.ToString();
            if (rbContent.Equals(Country.BY) == true ) {
                _employee.Country = Country.BY;
            }
            else if (rbContent.Equals(Country.UA) == true) {
                _employee.Country = Country.UA;
            }
        }

        private void btnFind_Click(object sender, RoutedEventArgs e) {            
            try {
                using (new WaitCursor()) {
                    lstEmployees.Items.Clear();
                    tbtnSubcontractor.IsChecked = false;
                    radiobtnUkraine.IsChecked = false;
                    radiobtnBelarus.IsChecked = false;
                    string currentLDAPDomain = $"LDAP://{Domain.GetComputerDomain()}";
                    string employeeToSeek = tbUserName.Text;
                    List<string> listOfEmployees = new ActiveDirectory().GetListOfEmployee(currentLDAPDomain, employeeToSeek);

                    if (listOfEmployees.Count > 0) {
                        foreach (var item in listOfEmployees) {
                            lstEmployees.Items.Add(item);
                        }
                    }

                    lstEmployees.Focus();
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lstEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            string emp = lstEmployees.SelectedItem?.ToString();

            if (emp != null) {
                string currentDomain = $"LDAP://{Domain.GetComputerDomain()}";
                _employee = new ActiveDirectory().GetEmployee(currentDomain, emp);
                tbEmployeeInfo.Text = _employee.ToString(); 
            }
        }

        private void tbtnSubcontractor_Click(object sender, RoutedEventArgs e) {
            if (tbtnSubcontractor.IsChecked == true) {
                _employee.Subcontractor = true;
            }
            else {
                _employee.Subcontractor = false;
            }
        }

        private void tbtnAnotheDomain_Click(object sender, RoutedEventArgs e) {

        }
    }
}
