using System.Linq;
using Avalonia.Data.Converters;

namespace SimpleChat.Avalonia.Converters;

internal static class CustomConverters
{
	public static FuncValueConverter<string, string?> Initials =>
		new(str =>
		{
			if (str == null)
				return null;
			var initials = new string(str.Where(char.IsUpper).Take(2).ToArray());
			if (!string.IsNullOrEmpty(initials))
				return initials;
			if (str.Length >= 2)
				return str[..2];
			return str;
		});
}