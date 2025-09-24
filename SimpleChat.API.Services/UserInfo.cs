namespace SimpleChat.API.Services;

public sealed class UserInfo
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public bool IsOnline { get; set; }
}