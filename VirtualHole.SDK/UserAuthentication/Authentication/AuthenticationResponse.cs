
namespace VirtualHole.UserAuthentication
{
	public class AuthenticationResponse
	{
		public bool IsError = false;
		public string Message = string.Empty;

		public string IdToken = string.Empty;
		public string AccessToken = string.Empty;
	}
}
