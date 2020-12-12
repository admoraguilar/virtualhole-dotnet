using System.Web.Configuration;
using VirtualHole.Storage;

namespace VirtualHole.API.Services
{
	public class VirtualHoleStorageService
	{
		public VirtualHoleStorageClient Client { get; private set; } = null;

		public VirtualHoleStorageService(Settings settings = null)
		{
			if(settings == null) { settings = Settings.Default; }
			Client = new VirtualHoleStorageClient(settings.Root);
		}

		public class Settings
		{
			public static Settings Default
			{
				get {
					return new Settings {
						Root = WebConfigurationManager.AppSettings["VirtualHoleStorageClientDomain"]
					};
				}
			}

			public string Root { get; set; } = string.Empty;
		}
	}
}