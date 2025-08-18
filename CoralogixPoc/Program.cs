using CoralogixPoc.Data.Respositories;
using CoralogixPoc.Domain.Configurations;
using Microsoft.OpenApi.Models;
using NLog.Config;
using NLog.Coralogix;
using NLog.Layouts;
using NLog.Web;
using System.Text.Json.Serialization;
using JsonAttribute = NLog.Layouts.JsonAttribute;

namespace CoralogixPoc.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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
                Layout = new JsonLayout
                {
                    IncludeEventProperties = true,
                    IncludeScopeProperties = true,
                    Attributes =
                    {
                        new JsonAttribute("time", "${date:format=yyyy-MM-ddTHH\\:mm\\:ss.fffK}"),
                        new JsonAttribute("level", "${level:upperCase=true}"),
                        new JsonAttribute("message", "${message}"),
                        new JsonAttribute("exception", "${exception:format=ToString}"),
                        new JsonAttribute("logger", "${logger}"),
                        new JsonAttribute("threadId", "${threadid}"),
                    }
                },
            };
            config.AddTarget("Coralogix", coralogixTarget);
            var logginRule = new LoggingRule("*", NLog.LogLevel.Debug, coralogixTarget);

            //ConsoleTarget consoleTarget = new()
            //{
            //    Layout = new JsonLayout
            //    {
            //        IncludeEventProperties = true,
            //        IncludeScopeProperties = true,
            //        Attributes =
            //            {
            //                new JsonAttribute("time", "${date:format=yyyy-MM-ddTHH\\:mm\\:ss.fffK}"),
            //                new JsonAttribute("level", "${level:upperCase=true}"),
            //                new JsonAttribute("message", "${message}"),
            //                new JsonAttribute("exception", "${exception:format=ToString}"),
            //                new JsonAttribute("logger", "${logger}"),
            //                new JsonAttribute("threadId", "${threadid}"),
            //            }
            //    }
            //};
            //config.AddTarget("Console", consoleTarget);
            //var logginRule = new LoggingRule("*", NLog.LogLevel.Debug, consoleTarget);

            config.LoggingRules.Add(logginRule);

            NLog.LogManager.Configuration = config;

            builder.Logging.ClearProviders();
            builder.Host.UseNLog(new NLogAspNetCoreOptions
            {
                CaptureMessageTemplates = true,
                CaptureMessageParameters = true,
                CaptureMessageProperties = true,
                IncludeScopes = true
            });
            #endregion

            builder.Services.AddScoped<CompanyRepository>();

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
