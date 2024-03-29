﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectoryLibrary
{
    public class Settings
    {
        private const string regPath = @"SOFTWARE\OurSettings";
        private const string regLogin = "Settings1";
        private const string regPassword = "Settings2";

        static public void WriteLoginPasswordToRegistry(string login, string password)
        {
            string base64login = Convert.ToBase64String(Encoding.UTF8.GetBytes(login));
            string base64password = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

            RegistryKey key = Registry.CurrentUser.CreateSubKey(regPath);
            //storing the values  
            key.SetValue(regLogin, base64login);
            key.SetValue(regPassword, base64password);

            key.Close();
        }

        static public void ReadLoginPasswordFromRegestry(out string login, out string password)
        {
            login = password = null;

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
            else
            {
                throw new Exception("User password is empty.");
            }
        }
    }
}
