﻿using ActiveDirectoryLibrary;
using log4net.Config;
using Microsoft.Win32;
using SeleniumAutomationLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SeleniumGUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Employee _employee = new Employee();
        private string _ldapPathToCurrentDomain = Domains.LdapPathToCurrentDomain;
        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MainWindow() {
            //если отключить  XmlConfigurator.Configure(fileInfo); то лог создаваться на будет. 
            //для всего проекта удобней отключать лог в AssemblyInfo.cs, а не в кождом классе.
            //var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            //FileInfo fileInfo = new FileInfo(baseDir + "log4net.config");
            //XmlConfigurator.Configure(fileInfo);

            InitializeComponent();
            tbUserName.Focus();
            MouseLeftButtonDown += delegate { DragMove(); };
            switch (Domains.CurrentDomainCountry) {
                case Country.BY:
                    radiobtnBelarus.IsChecked = true;
                    break;

                case Country.UA:
                    radiobtnUkraine.IsChecked = true;
                    break;
            }
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
            dismissingAnEmployee.ProcessCompleted += EmployeeDismissing_ProcessCompleted;
            dismissingAnEmployee.Launch(employee);
        }

        private void StartCreatingAnEmployee(Employee userToWorkWith) {
            var creatingAnEmployee = new CreatingAnEmployee();
            creatingAnEmployee.ProcessCompleted += EmployeeCreating_ProcessCompleted;
            creatingAnEmployee.Launch(userToWorkWith);
        }
       
        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void EmployeeCreating_ProcessCompleted(object sender, EventArgs e) {
            MessageBox.Show($"The employee {_employee.FullName} ({_employee.Login}) was successfully created in jira & confluence.");
        }
        
        private void EmployeeDismissing_ProcessCompleted(object sender, EventArgs e) {
            MessageBox.Show($"The employee {_employee.FullName} ({_employee.Login}) was successfully dismissed in jira & confluence.");
        } 
        
        private void Migration_ProcessCompleted(object sender, EventArgs e) {
            MessageBox.Show($"Migration batch for {_employee.FullName} ({_employee.Login}) was created.");
        }
        private void Createexchangeusermailbox_ProcessCompleted(object sender, EventArgs e) {
            MessageBox.Show($"MailBbox for {_employee.FullName} ({_employee.Login}) was created.");
        }

        private void CheckButtonsHandler(object sender, RoutedEventArgs e) {
            RadioButton rb = sender as RadioButton;
            string rbContent = rb.Content.ToString();
            if (rbContent.Equals(Country.BY) == true) {
                _employee.Country = Country.BY;
            }
            else if (rbContent.Equals(Country.UA) == true) {
                _employee.Country = Country.UA;
            }
        }

        private void btnFind_Click(object sender, RoutedEventArgs e) {
            try {
                using (new WaitCursor()) {
                    ResetStateOfControls();

                    SelectCountryOfCurrentDomain();

                    string employeeToSeek = tbUserName.Text.Trim();
                    List<string> listOfEmployees = new ActiveDirectory().GetListOfEmployee(_ldapPathToCurrentDomain, employeeToSeek);

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

        private void ResetStateOfControls() {
            lstEmployees.Items.Clear();
            tbEmployeeInfo.Text = "";
            tbtnSubcontractor.IsChecked = false;
            radiobtnUkraine.IsChecked = false;
            radiobtnBelarus.IsChecked = false;
        }

        private void MarkSelectedListItemAsDisabled(ListBox lbUsers, bool disabled) {
            var selectedItemIndex = lbUsers.SelectedIndex;
            ListBoxItem lbi = (ListBoxItem)lbUsers.ItemContainerGenerator.ContainerFromIndex(selectedItemIndex);

            if (lbi == null) {
                return;
            }

            if (disabled) {
                lbi.Foreground = Brushes.Gray;
            }
            else {
                lbi.Foreground = Brushes.Black;
            }
        }

        private void lstEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            string emp = lstEmployees.SelectedItem?.ToString();

            if (emp != null) {
                _employee = new ActiveDirectory().GetEmployee(_ldapPathToCurrentDomain, emp);

                if (_employee.CanonicalName.Contains("Subcontractors") ||
                    _employee.CanonicalName.Contains("External")) {
                    tbtnSubcontractor.IsChecked = true;
                    _employee.Subcontractor = true;
                }
                else {
                    tbtnSubcontractor.IsChecked = false;
                    _employee.Subcontractor = false;
                }

                if (_employee.CanonicalName.Contains("Disabled") == false) {
                    MarkSelectedListItemAsDisabled(lstEmployees, false);
                }
                else {
                    MarkSelectedListItemAsDisabled(lstEmployees, true);
                }

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
            ResetStateOfControls();
            SelectCountryOfCurrentDomain();
        }

        private void SelectCountryOfCurrentDomain() {
            if (tbtnAnotheDomain.IsChecked == true) {
                _ldapPathToCurrentDomain = Domains.LdapPathToAnotherDomain;

                if (Domains.AnotherDomainCountry.Equals(Country.BY)) {
                    radiobtnBelarus.IsChecked = true;
                }
                else if (Domains.AnotherDomainCountry.Equals(Country.UA)) {
                    radiobtnUkraine.IsChecked = true;
                }
            }
            else {
                _ldapPathToCurrentDomain = Domains.LdapPathToCurrentDomain;
                if (Domains.CurrentDomainCountry.Equals(Country.BY)) {
                    radiobtnBelarus.IsChecked = true;
                }
                else if (Domains.CurrentDomainCountry.Equals(Country.UA)) {
                    radiobtnUkraine.IsChecked = true;
                }
            }
        }

        private void btnMigrate_Click(object sender, RoutedEventArgs e) {
            try {
                if (string.IsNullOrEmpty(_employee.Login) == true) {
                    tbUserName.Focus();
                    throw new Exception("Employee to migrate does not selected.");
                }

                var result = MessageBox.Show($"The mailbox of {_employee.FullName} will be migrate to MO365. \n\nDo you want to continue?", "Continue", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes) {
                    StartMigration(_employee);
                }
            }
            catch (Exception seleniumExeption) {
                MessageBox.Show(seleniumExeption.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartMigration(Employee employee) {
            Microsoft365 ms = new Microsoft365();
            ms.ProcessCompleted += Migration_ProcessCompleted;
            ms.Migrate(employee.FullName);
        }

        private void btnCreateMailBox_Click(object sender, RoutedEventArgs e) {
            try {
                if (string.IsNullOrEmpty(_employee.Login) == true) {
                    tbUserName.Focus();
                    throw new Exception("Employee to create mailbox does not selected.");
                }

                var result = MessageBox.Show($"The mailbox for {_employee.FullName} will be created. \n\nDo you want to continue?", "Continue", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes) {
                    CreateExchangeUserMailbox createexchangeusermailbox = new CreateExchangeUserMailbox();
                    createexchangeusermailbox.ProcessCompleted += Createexchangeusermailbox_ProcessCompleted;
                    createexchangeusermailbox.Launch(_employee);
                }
            }
            catch (Exception seleniumExeption) {
                MessageBox.Show(seleniumExeption.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }

        private void StartCreatingBatchOfEmployee() {
            var employeesBatch = new AddListOfEmployees();
            employeesBatch.PrintToOutput += EmployeesBatch_PrintToOutput;

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.ShowDialog();

            try {
                var text = File.ReadAllText(dlg.FileName);
                employeesBatch.Launch(text);
            }
            catch (Exception ception) {
                MessageBox.Show(ception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }

        private void EmployeesBatch_PrintToOutput(object sender, string message) {
            tbEmployeeInfo.Text += "\n" + message;
        }

        private void btnAddFromFile_Click(object sender, RoutedEventArgs e) {
            StartCreatingBatchOfEmployee();            
        }

        private void btnFireOffFromFile_Click(object sender, RoutedEventArgs e)
        {
            var emplToFireOff = new FireOffListOfEmployee();
            emplToFireOff.PrintToOutput += EmployeesBatch_PrintToOutput;

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.ShowDialog();

            try
            {
                var text = File.ReadAllText(dlg.FileName);
                emplToFireOff.Launch(text);
            }
            catch (Exception ception)
            {
                MessageBox.Show(ception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWnd settings = new SettingsWnd();
            settings.Owner = this;
            settings.ShowDialog();
        }

        private void btnDownloadChromeDriver_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {                
                FileName = "https://googlechromelabs.github.io/chrome-for-testing/#stable",
                UseShellExecute = true
            });
        }
    }
}
