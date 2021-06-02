using ActiveDirectoryLibrary;
using log4net;
using log4net.Config;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SeleniumAutomationLibrary
{
    public class AddListOfEmployees
    {
        // Define a static logger variable so that it references the
        // Logger instance named "MyApp".
        private static readonly ILog log = LogManager.GetLogger(typeof(AddListOfEmployees));

        // declaring an event using built-in EventHandler
        public event EventHandler ProcessCompleted;

        protected virtual void OnProcessCompleted(EventArgs e)
        {
            ProcessCompleted?.Invoke(this, e);
        }

        // declaring an event using built-in EventHandler
        public event EventHandler<string> PrintToOutput;
        protected virtual void OnPrintToOutput(string e)
        {
            PrintToOutput?.Invoke(this, e);
        }

        public AddListOfEmployees()
        {
            // Set up a simple configuration that logs on the console.
            BasicConfigurator.Configure();
        }

        /// <summary>
        /// receive list of employees separeted by comma as string
        /// </summary>
        /// <returns>List of employees</returns>
        private List<string> ParseListOfEmployee(string listOfEmployees)
        {
            var employeesList = new List<string>();

            listOfEmployees = listOfEmployees.Replace("\r\n", "");

            string[] employees = listOfEmployees.Split(',');

            foreach (var e in employees)
            {
                if (string.IsNullOrEmpty(e) == false)
                {
                    employeesList.Add(e);
                }
            }
            return employeesList;
        }

        /// <summary>
        /// performs creation of employees in jira-confluence
        /// </summary>
        public void Launch(string listOfEmployees)
        {
            List<string> employeesList = ParseListOfEmployee(listOfEmployees);

            if (employeesList.Count == 0)
            {
                throw new Exception("List of employees is empty");
            }

            var driverService = ChromeDriverService.CreateDefaultService();
            //var driverService = ChromeDriverService.CreateDefaultService(@"D:\VSProjects\SeleniumAutomation\SeleniumAutomation");
            driverService.HideCommandPromptWindow = true;

            using (IWebDriver driver = new ChromeDriver(driverService, new ChromeOptions()))
            {
                try
                {
                    try
                    {
                        new Microsoft(driver).Login();
                    }
                    catch (Exception msExeption)
                    {
                        throw new Exception(msExeption.Message);
                    }

                    foreach (var em in employeesList)
                    {
                        //check if employee exists in AD
                        Employee employee = new ActiveDirectory().GetEmployee(Domains.LdapPathToCurrentDomain, em);

                        if (employee.IsEmpty())
                        {
                            log.Error($"{em} does not exist in Active Directory");
                            OnPrintToOutput($"{em} does not exist in Active Directory");
                            continue;
                        }

                        try
                        {
                            bool result = new Jira(driver).Recruit(employee);
                            if (result)
                            {
                                OnPrintToOutput($"{employee.FullName} ({employee.Login}) was created in jira");
                            }
                        }
                        catch (Exception jiraExeption)
                        {
                            log.Error($"Something went wrong when creating {em} in jira");
                            OnPrintToOutput($"Something went wrong when creating {em} in jira");

                            MessageBoxResult result = MessageBox.Show($"{jiraExeption.Message} \n\n Continue the execution of the script?", "Something went wrong!", MessageBoxButton.YesNo, MessageBoxImage.Error);
                            if (result == MessageBoxResult.No)
                            {
                                throw new Exception(jiraExeption.Message);
                            }
                        }

                        try
                        {
                            driver.SwitchTo().NewWindow(WindowType.Tab);
                            new Confluence(driver).Recruit(employee);
                            OnPrintToOutput($"{employee.FullName} ({employee.Login}) was created in confluence");
                        }
                        catch (Exception confluenceExeption)
                        {
                            log.Error($"Something went wrong when creating {em} in confluence");
                            OnPrintToOutput($"Something went wrong when creating {em} in confluence");
                            throw new Exception(confluenceExeption.Message);
                        }
                    }

                    OnProcessCompleted(EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
