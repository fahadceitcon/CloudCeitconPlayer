using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string sM = DateTime.Now.TimeOfDay.Minutes.ToString();
            if (sM.Length == 1)
                sM = "0" + sM;
            string sSe = DateTime.Now.TimeOfDay.Seconds.ToString();
            if (sSe.Length == 1)
                sSe = "0" + sSe;

            string sTime = DateTime.Now.TimeOfDay.Hours.ToString() + ":" + sM;
            string sInput = System.IO.File.ReadAllText("C:\\Users\\Ceitcon-Dev\\Downloads\\Modon_HQ.cdp");
            string sResult = Crypt.Decrypt(sInput);

            Console.ReadLine();


        }

        static void DeleteOldRecords()
        {
            Console.ReadLine();
        }

    }
}
