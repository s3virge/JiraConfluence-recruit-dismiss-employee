using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraConfluenceUserAccounts {
    
    public class JiraConfluenceUserAccounts {
        // Define a static logger variable so that it references the
        // Logger instance named "MyApp".
        private static readonly ILog log = LogManager.GetLogger(typeof(JiraConfluenceUserAccounts));

        /// <summary>
        /// receive list of employees separeted by comma as string
        /// </summary>
        /// <returns>List of employees</returns>
        private static List<string> ParseListOfEmployee(string listOfEmployees) {
            var listOfEmployee = new List<string>();

            string[] employees = listOfEmployees.Split(',');

            foreach(var e in employees) {
                if (string.IsNullOrEmpty(e) == false) {
                    listOfEmployee.Add(e);
                }
            }
            return listOfEmployee;
        }

        /// <summary>
        /// performs creation of employees in jira-confluence
        /// </summary>
        public static void Launch(string listOfEmployees) {
            var driverService = ChromeDriverService.CreateDefaultService();
            //var driverService = ChromeDriverService.CreateDefaultService(@"D:\VSProjects\SeleniumAutomation\SeleniumAutomation");
            driverService.HideCommandPromptWindow = true;

            using (IWebDriver driver = new ChromeDriver(driverService, new ChromeOptions())) {
                try {
                    try {
                        new Microsoft(driver).Login();
                    }
                    catch (Exception msExeption) {
                        throw new Exception(msExeption.Message);
                    }

                    try {
                        new Jira(driver).Recruit(coworker);
                    }
                    catch (Exception jiraExeption) {
                        MessageBoxResult result = MessageBox.Show($"{jiraExeption.Message} \n\n Continue the execution of the script?", "Something went wrong!", MessageBoxButton.YesNo, MessageBoxImage.Error);
                        if (result == MessageBoxResult.No) {
                            throw new Exception(jiraExeption.Message);
                        }
                    }

                    try {
                        driver.SwitchTo().NewWindow(WindowType.Tab);
                        new Confluence(driver).Recruit(coworker);
                    }
                    catch (Exception confluenceExeption) {
                        throw new Exception(confluenceExeption.Message);
                    }

                    OnProcessCompleted(EventArgs.Empty);
                }
                catch (Exception ex) {
                    throw new Exception(ex.Message);
                }
            }
            List<string> employeesList = ParseListOfEmployee(listOfEmployees);
            foreach(var em in employeesList) {

            }
        }
    }
}
