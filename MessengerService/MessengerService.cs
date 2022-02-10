namespace MessengerService
{
    using System.ServiceProcess;

    using MessengerServer;

    public partial class MessengerService : ServiceBase
    {
        #region Fields

        private WsServer _wsServer;

        #endregion //Fields

        #region Constructors

        public MessengerService()
        {
            InitializeComponent();
        }

        #endregion //Constructors

        #region Methods

        protected override void OnStart(string[] args)
        {
            _wsServer = new WsServer();
            _wsServer.Start();
        }

        protected override void OnStop()
        {
            _wsServer.Stop();
        }

        #endregion //Methods
    }
}
