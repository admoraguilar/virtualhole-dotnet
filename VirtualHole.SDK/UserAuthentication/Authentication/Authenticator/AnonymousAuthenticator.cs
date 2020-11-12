
namespace VirtualHole.UserAuthentication
{
	public class AnonymousAuthenticator : UserAuthenticator
	{
		public override AuthenticationResponse Authenticate(VirtualHoleUserAuthenticationClient userClient, string password)
		{
			return new AuthenticationResponse() {
				IsError = false,
				Message = string.Empty,
				AccessToken = string.Empty
			};
		}
	}
}
