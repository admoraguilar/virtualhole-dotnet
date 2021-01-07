using System.Web.Configuration;
using VirtualHole.DB;

namespace VirtualHole.API.Services
{
	public class VirtualHoleDBService
	{
		public VirtualHoleDBClient Client { get; private set; }

		public VirtualHoleDBService(Settings settings = null)
		{
			if(settings == null) { settings = Settings.Default; }
			Client = new VirtualHoleDBClient(
				settings.ConnectionString, settings.UserName,
				settings.Password);
		}

		public class Settings
		{
			public static Settings Default
			{
				get {
					return new Settings {
						ConnectionString = WebConfigurationManager.AppSettings["VirtualHoleDBClientConnectionString"],
						UserName = WebConfigurationManager.AppSettings["VirtualHoleDBClientUserName"],
						Password = WebConfigurationManager.AppSettings["VirtualHoleDBClientPassword"]
					};
				}
			}

			public string ConnectionString { get; set; } = string.Empty;
			public string UserName { get; set; } = string.Empty;
			public string Password { get; set; } = string.Empty;
		}
	}
}