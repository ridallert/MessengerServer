using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer
{
    class Program
    {
        static void Main()
        {
            try
            {
                var networkManager = new NetworkManager();
                networkManager.Start();

                Console.ReadLine();

                networkManager.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }
    }
}
