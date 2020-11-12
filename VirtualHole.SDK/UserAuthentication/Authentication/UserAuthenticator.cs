
namespace VirtualHole.UserAuthentication
{
	public abstract class UserAuthenticator
	{
		public abstract AuthenticationResponse Authenticate(VirtualHoleUserAuthenticationClient userClient, string password);
	}
}
