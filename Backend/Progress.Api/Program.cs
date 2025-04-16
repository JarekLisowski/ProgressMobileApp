using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Progress.BusinessLogic;
using Progress.Infrastructure.Database.Repository;

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

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseCors("AllowAllPolicy");

			//app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
