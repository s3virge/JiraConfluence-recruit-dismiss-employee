using System.DirectoryServices.ActiveDirectory;

namespace ActiveDirectoryLibrary {
    public class Domains {
        private const string UADomainName = "intetics.com.ua";
        private const string BYDomainName = "atwss.com";
        private static string anotherDomain;

        public static string UAName { get; private set; } = UADomainName;
        public static string BYName { get; private set; } = BYDomainName;

        /// <summary>
        /// name of current domain like atwss.com
        /// </summary>
        private static string Current { 
            get {
                //return Domain.GetComputerDomain().ToString();
                return Domains.BYDomainName;
            } 
        } 
      
        public static string LdapPathToCurrentDomain { 
            get {
                if (Current.Equals(UADomainName)) {
                    currentDomainCountry = Country.UA;
                }
                else if (Current.Equals(BYDomainName)) {
                    currentDomainCountry = Country.BY;
                }
                return $"LDAP://{Current}"; 
            } 
        }

        private static string currentDomainCountry = null;
        public static string CurrentDomainCountry { 
            get {               
                if (Current.Equals(UADomainName)) {
                    currentDomainCountry = Country.UA;
                }
                else if (Current.Equals(BYDomainName)) {
                    currentDomainCountry = Country.BY;
                }
                return currentDomainCountry;                
            }
            private set { currentDomainCountry = value; }
        }

        private static string anotherDomainCountry = null;
        public static string AnotherDomainCountry { 
            get {               
                if (Current.Equals(UADomainName)) {
                    anotherDomainCountry = Country.BY;
                }
                else if (Current.Equals(BYDomainName)) {
                    anotherDomainCountry = Country.UA;
                }
                return anotherDomainCountry;                
            }
            private set { anotherDomainCountry = value; }
        }

        public static string LdapPathToAnotherDomain {
            get {
                if (Current.Equals(UADomainName) == true) {
                    anotherDomain = BYDomainName;
                    anotherDomainCountry = Country.BY;
                }
                else if (Current.Equals(BYDomainName) == true) {
                    anotherDomain = UADomainName;
                    anotherDomainCountry = Country.UA;
                }
                return $"LDAP://{anotherDomain}";
            }
        }
    }
}
