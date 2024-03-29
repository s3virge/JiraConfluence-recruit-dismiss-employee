﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumAutomationLibrary
{
    public class Settings
    {
        private const string regJiraConfPath = @"SOFTWARE\OurSettings";
        private const string regLogin = "Settings1";
        private const string regPassword = "Settings2";

        private const string regADPath = @"SOFTWARE\TroubleShooting";
        

        static public void SaveLoginPasswordToRegistry(string login, string password, bool isActiveDirectory = false)
        {
            string base64login = Convert.ToBase64String(Encoding.UTF8.GetBytes(login));
            string base64password = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

            string regPath = regJiraConfPath;
            if (isActiveDirectory == true)
            {
                regPath = regADPath;
            }

            RegistryKey key = Registry.CurrentUser.CreateSubKey(regPath);
            //storing the values  
            key.SetValue(regLogin, base64login);
            key.SetValue(regPassword, base64password);

            key.Close();
        }

        static public void ReadLoginPasswordFromRegestry(out string login, out string password, bool isActiveDirectory = false)
        {
            login = password = null;

            string regPath = regJiraConfPath;
            if (isActiveDirectory == true)
            {
                regPath = regADPath;
            }
            //opening the subkey  
            RegistryKey key = Registry.CurrentUser.OpenSubKey(regPath);

            //if it does exist, retrieve the stored values  
            if (key != null)
            {
                byte[] byteLogin = Convert.FromBase64String(key.GetValue(regLogin).ToString());
                byte[] bytePassword = Convert.FromBase64String(key.GetValue(regPassword).ToString());

                login = System.Text.Encoding.Default.GetString(byteLogin);
                password = System.Text.Encoding.Default.GetString(bytePassword);

                key.Close();
            }
        }
    }
}
