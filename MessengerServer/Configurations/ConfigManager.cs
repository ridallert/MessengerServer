namespace MessengerServer.Configurations
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Net;

    using Newtonsoft.Json;

    public class ConfigManager
    {
        #region Fields

        private readonly Configurations _defaultConfigs;

        private readonly string _defaultPath;

        #endregion //Fields

        #region Properties

        public int Port { get; }

        public int Timeout { get; }

        public IPAddress IpAddress { get; }

        public ConnectionStringSettings ConnectionSettings { get; }

        #endregion //Properties

        #region Constructors

        public ConfigManager()
        {
            _defaultConfigs = new Configurations
            {
                IpAddress = "0.0.0.0",
                Port = 7890,
                ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=ServerState; Integrated Security=True; Connection Timeout=15",
                Provider = "System.Data.SqlClient",
                DataBaseName = "ServerDataBase",
                Timeout = 600,
            };

            _defaultPath = Environment.CurrentDirectory + "\\Configs.json";

            Configurations configs = LoadFromFile(_defaultPath);

            if (configs.ConnectionString == null)
            {
                Console.WriteLine("Config file not found, file with default settings created");
                SaveToFile(_defaultConfigs, _defaultPath);
                configs = LoadFromFile(_defaultPath);
            }
            else
            {
                if (!configs.ConnectionString.Contains("Connection Timeout"))
                {
                    configs.ConnectionString += "; Connection Timeout=15";
                }
            }

            IpAddress = IPAddress.Parse(configs.IpAddress);
            Port = configs.Port;
            Timeout = configs.Timeout;
            ConnectionSettings = new ConnectionStringSettings(configs.DataBaseName, configs.ConnectionString, configs.Provider);
        }

        #endregion //Constructors

        #region Methods

        public ConnectionStringSettings GetDefaultConnectionString()
        {
            return new ConnectionStringSettings(_defaultConfigs.DataBaseName, _defaultConfigs.ConnectionString, _defaultConfigs.Provider);
        }

        private void SaveToFile(Configurations configs, string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (JsonWriter writer = new JsonTextWriter(new StreamWriter(path)))
            {
                var serializer = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Include,
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Formatting.Indented
                };

                serializer.Serialize(writer, configs);
            }
        }

        private Configurations LoadFromFile(string path)
        {
            var configs = new Configurations();

            try
            {
                using (JsonTextReader reader = new JsonTextReader(new StreamReader(path)))
                {
                    JsonSerializer serializer = new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Include,
                        TypeNameHandling = TypeNameHandling.All,
                        Formatting = Formatting.None
                    };

                    configs = serializer.Deserialize<Configurations>(reader);
                }
            }
            catch (Exception)
            {
                SaveToFile(_defaultConfigs, _defaultPath);
                return _defaultConfigs;
            }

            if (configs == null)
            {
                configs = _defaultConfigs;
                SaveToFile(_defaultConfigs, _defaultPath);
            }

            return configs;
        }

        #endregion //Methods
    }
}
