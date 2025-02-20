using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDP_Encrpyt_Decrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string sFileName = @"C:\Users\mh_ar\Desktop\Scripts\Project_2bbaf0.cdp";
                string sContent = System.IO.File.ReadAllText(sFileName);
                string DecConten = MyClass.Decrypt(sContent);


            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
