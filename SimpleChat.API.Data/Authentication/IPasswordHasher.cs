namespace SimpleChat.API.Data.Authentication;

public interface IPasswordHasher
{
	string GetPasswordHash(string password);
}