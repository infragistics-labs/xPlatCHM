namespace xPlatCHM.Common
{
	public class User
	{
		public string Email { get; private set; }

		public string Password { get; private set; }

		public User(string email, string password)
		{
			this.Email = email;
			this.Password = password;
		}
	}
}
