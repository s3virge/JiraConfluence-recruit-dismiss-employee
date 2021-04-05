using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace ActiveDirectoryLibrary { 
    public class Employee : INotifyPropertyChanged {
        private string _password;
        private string _login;
        private string _fullName;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Login {
            get => _login;

            set {
                _login = value.Trim().ToLower();
                OnPropertyChanged(nameof(Login));
            }
        }

        public string FullName {
            get => _fullName;
            set {
                _fullName = value.Trim();
                if (_fullName.LastIndexOf(' ') > 0) {
                    Login = $"{_fullName[0]}.{_fullName.Substring(_fullName.LastIndexOf(' ') + 1)}";
                }
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string Password {
            get => _password;

            set {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string ID { get; set; }
        public string Department { get; set; }
        public string Office { get; set; }
        public string Job { get; set; }
        public string Mail { get; set; }
        public string Mobile { get; set; }
        public string CanonicalName { get; set; }
        public string WhenCreated { get; set; }
        public string WhenChanged { get; set; }
        public string PasswordLastSet { get; set; }
        public string LastLogon { get; set; }
        public List<string> MemderOf { get; set; }
        public string Description { get; set; }
        public string DistinguishedName { get; set; }
        public bool Locked { get; set; }
        public bool Disabled { get; set; }
        public string Manager { get; set; }
        public string SID { get; set; }

        private void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Debug.WriteLine($" {propertyName} property  has changed");
        }

        /// <returns>return string of formated information about employee</returns>
        public override string ToString() {
            string employeeInfo = "";

            employeeInfo = string.Format(Output.format, "Name", $"{FullName}\n");
            employeeInfo += Output.lineSeparator;
            employeeInfo += string.Format(Output.format, "login", $"{Login}\n");
            employeeInfo += string.Format(Output.format, "department", $"{Department}\n");
            employeeInfo += string.Format(Output.format, "office", $"{Office}\n");
            employeeInfo += string.Format(Output.format, "job", $"{Job}\n");
            employeeInfo += string.Format(Output.format, "mail", $"{Mail}\n");
            employeeInfo += string.Format(Output.format, "mobile", $"{Mobile}\n");
            employeeInfo += string.Format(Output.format, "CanonicalName", $"{CanonicalName}\n");
            employeeInfo += string.Format(Output.format, "whenCreated", $"{WhenCreated}\n");
            employeeInfo += string.Format(Output.format, "whenChanged", $"{WhenChanged}\n");
            employeeInfo += string.Format(Output.format, "pwdLastSet", $"{PasswordLastSet}\n");
            employeeInfo += string.Format(Output.format, "lastLogon", $"{LastLogon}\n");
            employeeInfo += string.Format(Output.format, "description", $"{Description}\n");
            employeeInfo += string.Format(Output.format, "manager", $"{Manager}\n");
            employeeInfo += Output.lineSeparator;

            List<string> groups = MemderOf;

            if (groups != null && groups.Count > 0) {
                employeeInfo += string.Format(Output.format, "Member off", $"{groups[0]};\n");

                for (int c = 1; c <= groups.Count - 1; c++) {
                    employeeInfo += string.Format("{0, -14}\t\t {1}", "\t", $"{groups[c]};\n");
                }
            }

            return employeeInfo;
        }

        public bool IsEmpty() {
            return Login == null && DistinguishedName == null;
        }
    }
}
