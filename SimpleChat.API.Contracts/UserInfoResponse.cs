namespace SimpleChat.API.Contracts;

public sealed class UserInfoResponse
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
}