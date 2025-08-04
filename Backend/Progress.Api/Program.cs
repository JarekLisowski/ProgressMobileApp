using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Progress.BusinessLogic;
using Progress.Infrastructure.Database.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Progress.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllers();

			builder.Services.AddCors(o => o.AddPolicy("AllowAllPolicy", builder =>
			{
				builder.AllowAnyHeader()
					.AllowAnyMethod()
					.SetIsOriginAllowed((host) => true)
					.AllowCredentials()
					.WithExposedHeaders(new[] { "X-Operation", "X-Resource", "X-ResourceId", "X-Total-Count", HeaderNames.ContentDisposition });
			}));

			// Add services to the container.
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddDbContext<Database.NavireoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Navireo")));
			builder.Services.AddAutoMapper(typeof(Infrastructure.Database.AutoMapper));
			builder.Services.AddAutoMapper(typeof(Domain.Api.AutoMapper));
			builder.Services.RegisterRepositories();
			builder.Services.AddScoped<ProductManager>();
			builder.Services.AddScoped<PromoManager>();
			builder.Services.AddScoped<CustomerManager>();
			builder.Services.AddScoped<DocumentManager>();
			builder.Services.AddScoped<FinanceManager>();
			builder.Services.AddScoped<AuthManager>();
			builder.Services.AddScoped<BusinessLogic.ConfigurationManager>();
      builder.Services.AddScoped<Domain.Interfaces.IUserRepository, UserRepository>();
			builder.Services.AddTransient<NavireoConnector>();
			builder.Services.AddSingleton<IPrintService, PrintService>();
			builder.Services.AddHostedService<PrintService>(sp => (PrintService)sp.GetRequiredService<IPrintService>());

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = builder.Configuration["Jwt:Issuer"],
					ValidAudience = builder.Configuration["Jwt:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
				};
			});

			var app = builder.Build();

      var test = app.Configuration.GetConnectionString("Navireo");

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseCors("AllowAllPolicy");

			app.UseAuthentication();
			//app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
