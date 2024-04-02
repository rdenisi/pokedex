using Microsoft.OpenApi.Models;
using Pokedex.Middleware;
using Pokedex.Services;

namespace Pokedex
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Pokedex API",
                        Description = "A sample API that provides some informations about pokemon.",
                        Version = "v1"
                    });

                var filePath = Path.Combine(AppContext.BaseDirectory, "Pokedex.xml");
                s.IncludeXmlComments(filePath);
            });

            builder.Services.AddHttpClient();

            builder.Services.AddScoped<IPokemonService, PokemonService>();

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
