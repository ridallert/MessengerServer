namespace MessengerServer
{
    using System;

    class Program
    {
        #region Methods

        static void Main()
        {
            try
            {
                var wsServer = new WsServer();
                wsServer.Start();

                Console.ReadLine();

                wsServer.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        #endregion //Methods
    }
}
