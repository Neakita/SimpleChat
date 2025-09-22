using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleChat.API.Authentication;
using SimpleChat.API.Data;
using SimpleChat.API.Data.Authentication;
using SimpleChat.API.Services.Authentication;

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
			TokenExpiration = TimeSpan.Parse(tokenExpiration, CultureInfo.InvariantCulture),
			SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey))
		};
		configuration.Bind("JWTGenerator", jwtGeneratorConfiguration);
		builder.Services.AddSingleton(jwtGeneratorConfiguration);

		var refreshTokenExpirationTime = configuration["RefreshTokenExpirationTime"] ?? 
		                                 throw new NullReferenceException("Refresh token expiration time is not set");

		var refreshTokenConfiguration = new RefreshTokenConfiguration
		{
			ExpirationTime = TimeSpan.Parse(refreshTokenExpirationTime, CultureInfo.InvariantCulture)
		};

		builder.Services.AddSingleton(refreshTokenConfiguration);


		builder.Services.AddAuthorization();
		builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = jwtGeneratorConfiguration.SecurityKey
				};
			});
		builder.Services.AddMvcCore();

		builder.Services.AddDbContextFactory<AppDbContext>(options =>
		{
			var connectionStringBuilder = new SqliteConnectionStringBuilder();
			connectionStringBuilder.DataSource = "App.db";
			options.UseSqlite(connectionStringBuilder.ToString());
		});
		builder.Services.AddTransient<IAuthenticator, AppDbAuthenticator>();
		builder.Services.AddTransient<IRefreshTokenManager, AppDbRefreshTokenManager>();

		var passwordSaltSeed = builder.Configuration["PasswordSaltSeed"] ??
		                       throw new NullReferenceException("Password salt seed is not set");
		builder.Services.AddSingleton<IPasswordHasher>(new SaltyPasswordHasher(int.Parse(passwordSaltSeed)));

		builder.Services.AddTransient<JWTGenerator>();
	}

	private static void ConfigureApplication(WebApplication application)
	{
		application.UseHttpsRedirection();
		application.UseAuthorization();
		application.MapControllers();
	}
}