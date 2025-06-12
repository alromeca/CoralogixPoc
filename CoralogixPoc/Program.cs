
using CoralogixPoc.Configurations;
using CoralogixPoc.Providers;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace CoralogixPoc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });

            #region Coralogix
            // bind CoralogixOptions from config
            builder.Services.Configure<CoralogixOptions>(
                builder.Configuration.GetSection("Coralogix"));

            var coraOpts = builder.Configuration
                .GetSection("Coralogix")
                .Get<CoralogixOptions>();

            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(new CoralogixLoggerProvider(coraOpts!));
            #endregion

            var app = builder.Build();

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
