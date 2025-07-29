
using Microsoft.EntityFrameworkCore;

namespace Progress.Navireo
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Console.Title = "Navireo server";
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
      builder.Services.AddDbContext<Database.NavireoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Navireo")));
      builder.Services.AddTransient<Managers.DocumentManager>();
			builder.Services.AddTransient<Managers.CustomerManager>();
			builder.Services.AddTransient<Managers.FinanceManager>();
			builder.Services.AddTransient<Helpers.Logger>();
			builder.Services.AddSingleton<Navireo.NavireoApplication>();
			builder.Services.AddHostedService<Navireo.NavireoService>();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			//app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
