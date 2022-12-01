using System;
using System.IO;
using System.Linq;
using System.ServiceModel;

namespace Authenticator
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class AuthenticatorImplementation : AuthenticatorInterface
    {
        private static Random rnd = new Random();

        public int Login(string name, string password)
        {
            String tokenfilePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "TokenData.txt");
            String datafilePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "RegistrationData.txt");

            int token = 0;
            String[] fileContents = File.ReadAllLines(datafilePath);
            if (fileContents.Contains(name + " " + password))
            {
                token = rnd.Next(1, 100);
                using (StreamWriter writer = new StreamWriter(tokenfilePath, true))
                {
                    writer.WriteLine(token);
                    writer.Close();
                }
            }
            return token;
        }

        public String Register(String name, String password)
        {
            String datafilePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "RegistrationData.txt");
            String regst_status;
            String[] fileContents = File.ReadAllLines(datafilePath);

            if(fileContents.Contains(name + " " + password))
            {
                regst_status = "Registration Unsuccessful!";
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(datafilePath, true))
                {
                    writer.WriteLine(name + " " + password);
                    writer.Close();
                }
                regst_status = "Successfully Registered";
            } 
            return regst_status;
        }

        public string Validate(int token)
        {
            String tokenfilePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "TokenData.txt");
            string valid_status;
            String[] fileContents = File.ReadAllLines(tokenfilePath);

            if (fileContents.Contains(token.ToString()))
            {
                valid_status = "Validated";
            }
            else
            {
                valid_status = "Not Validated";
            }
            return valid_status;
        }
    }
}

/* REF : https://www.c-sharpcorner.com/UploadFile/mahesh/how-to-read-a-text-file-in-C-Sharp/
 * REF : https://stackoverflow.com/questions/5516870/how-to-write-data-to-a-text-file-without-overwriting-the-current-data
 * REF : https://stackoverflow.com/questions/39970838/getting-a-directory-path-using-project-directory */

