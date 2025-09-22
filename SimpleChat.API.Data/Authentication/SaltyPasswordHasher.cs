using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace SimpleChat.API.Data.Authentication;

public sealed class SaltyPasswordHasher(int saltSeed) : IPasswordHasher
{
	public string GetPasswordHash(string password)
	{
		var random = new Random(saltSeed);
		var salt = new byte[16];
		random.NextBytes(salt);
		var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
			password,
			salt,
			KeyDerivationPrf.HMACSHA256,
			100000,
			32));
		return hashed;
	}
}