namespace MessengerService
{
    using System.ServiceProcess;

    static class Program
    {
        #region Methods

        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new MessengerService() };
            ServiceBase.Run(ServicesToRun);
        }

        #endregion //Methods
    }
}
