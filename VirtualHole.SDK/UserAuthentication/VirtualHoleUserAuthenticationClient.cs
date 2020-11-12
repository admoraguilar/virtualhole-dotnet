using System;
using System.Security.Authentication;

namespace VirtualHole.UserAuthentication
{
	public class VirtualHoleUserAuthenticationClient
	{
		public readonly string Name = string.Empty;
		public readonly string Email = string.Empty;

		public string IdToken { get; private set; } = string.Empty;
		public string AccessToken { get; private set; } = string.Empty;
		public bool IsAuthenticated { get; private set; } = false;

		public VirtualHoleUserAuthenticationClient(string name, string email)
		{
			Name = name;
			Email = email;
		}

		public bool Login(string password)
		{
			if(IsAuthenticated) { return IsAuthenticated; }

			UserAuthenticator authenticator = null;

			if(string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Email) ||
			   string.IsNullOrEmpty(password)) {
				authenticator = new AnonymousAuthenticator();
			}

			AuthenticationResponse response = authenticator.Authenticate(this, password);
			if(response.IsError) {
				IsAuthenticated = false;
				throw new AuthenticationException("User authentication failed!");
			} else {
				IsAuthenticated = true;
				IdToken = response.IdToken;
				AccessToken = response.AccessToken;
			}

			return IsAuthenticated;
		}

		public void Logout()
		{
			IsAuthenticated = false;
		}

		public void Register(string password)
		{
			throw new NotImplementedException();
		}
	}
}
