using System.Web.Configuration;

namespace VirtualHole.API.Services
{
	using DB;

	public class VirtualHoleDBService
	{
		private static VirtualHoleDBClient client { get; set; } = null;

		public VirtualHoleDBClient Client 
		{ 
			get { return client; }
			private set { client = value; }
		}

		public VirtualHoleDBService(VirtualHoleDBServiceSettings settings = null)
		{
			if(settings == null) { settings = VirtualHoleDBServiceSettings.Default; }

			if(Client == null) {
				Client = new VirtualHoleDBClient(
					settings.ConnectionString, settings.UserName,
					settings.Password);
			}
		}

		public class VirtualHoleDBServiceSettings
		{
			public static VirtualHoleDBServiceSettings Default
			{
				get {
					return new VirtualHoleDBServiceSettings {
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