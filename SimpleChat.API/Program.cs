using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SimpleChat.API.Authentication;

namespace SimpleChat.API;

public static class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		ConfigureBuilder(builder);
		var application = builder.Build();
		ConfigureApplication(application);
		application.Run();
	}

	private static void ConfigureBuilder(WebApplicationBuilder builder)
	{
		var configuration = builder.Configuration;
		var jwtSection = configuration.GetSection("JWT");
		var tokenExpiration = jwtSection["TokenExpiration"] ?? throw new NullReferenceException("token expiration is not set");
		var securityKey = jwtSection["SecurityKey"] ?? throw new NullReferenceException("security key is not set");
		var jwtGeneratorConfiguration = new JWTGeneratorConfiguration
		{
			Issuer = jwtSection["Issuer"] ?? throw new NullReferenceException("jwt issuer is not set"),
			Audience = jwtSection["Audience"] ?? throw new NullReferenceException("audience is not set"),
			TokenExpiration = TimeSpan.Parse(tokenExpiration, CultureInfo.InvariantCulture),
			SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey))
		};
		configuration.Bind("JWTGenerator", jwtGeneratorConfiguration);
		builder.Services.AddSingleton(jwtGeneratorConfiguration);
		builder.Services.AddAuthorization();
		builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = jwtGeneratorConfiguration.Issuer,
					ValidateAudience = true,
					ValidAudience = jwtGeneratorConfiguration.Audience,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = jwtGeneratorConfiguration.SecurityKey
				};
			});
	}

	private static void ConfigureApplication(WebApplication application)
	{
		application.UseHttpsRedirection();
		application.UseAuthorization();
	}
}