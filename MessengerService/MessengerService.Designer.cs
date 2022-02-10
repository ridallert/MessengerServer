namespace MessengerService
{
    partial class MessengerService
    {
        #region Fields

        private System.ComponentModel.IContainer _components = null;

        #endregion //Fields

        #region Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // 
            // MessengerService
            // 
            _components = new System.ComponentModel.Container();
            this.ServiceName = "MessengerService";

        }

        #endregion //Methods
    }
}
