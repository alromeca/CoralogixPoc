
using CoralogixPoc.Configurations;
using Microsoft.OpenApi.Models;
using NLog.Config;
using NLog.Coralogix;
using NLog.Web;
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

            Environment.SetEnvironmentVariable("CORALOGIX_LOG_URL", coraOpts.Url);

            LoggingConfiguration config = new();

            CoralogixTarget coralogixTarget = new()
            {
                PrivateKey = coraOpts.PrivateKey,
                ApplicationName = coraOpts.ApplicationName,
                SubsystemName = coraOpts.SubsystemName,

                Layout = @"${date:format=HH\\:mm\\:ss} ${logger} ${message}"
            };
            config.AddTarget("Coralogix", coralogixTarget);


            //Configure the Level filter for the Coralogix Target 
            var logginRule = new LoggingRule("*", NLog.LogLevel.Debug, coralogixTarget);
            config.LoggingRules.Add(logginRule);

            // Define the actual NLog logger which through it all log entires should be reported
            NLog.LogManager.Configuration = config;

            NLog.Logger nlogger = NLog.LogManager.GetLogger("CoralogixPoc");

            builder.Logging.ClearProviders();
            builder.Host.UseNLog();
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
