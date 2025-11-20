using Elastic.CommonSchema;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Extensions
{
    internal static class SerilogExtensions
    {
        public static void ConfigureLogger(bool useBootstrap = false)
        {
            var config = new LoggerConfiguration()
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture);

            Serilog.Log.Logger = useBootstrap ? config.CreateBootstrapLogger() : config.CreateLogger();
        }

        public static void UseSerilogLogger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<SerilogApplicationEnricher>();

            builder.Host.UseSerilog((hostBuilderContext, serviceProvider, loggerConfiguration) =>
            {
                var enricher = serviceProvider.GetRequiredService<SerilogApplicationEnricher>();

                loggerConfiguration
                    .ReadFrom.Configuration(hostBuilderContext.Configuration)
                    .ReadFrom.Services(serviceProvider)
                    .Enrich.With(enricher);
            });
        }

        /// <summary>
        /// Use Serilog request logging with additional log enrichments 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            return app.UseSerilogRequestLogging();
        }
    }

    /// <summary>
    /// Enrich with activity and user, request from httpContext
    /// </summary>
    internal sealed class SerilogApplicationEnricher(IHttpContextAccessor httpContextAccessor) : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var context = httpContextAccessor.HttpContext;

            if (context == null)
            {
                return;
            }

            var userName = context.User.Identity?.Name;
            var userId = context.User.Claims.FirstOrDefault(f => f.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var legacyUserId = context.User.Claims.FirstOrDefault(f => f.Type == "user_id")?.Value;
            var ipAddress = context.Connection.RemoteIpAddress;

            if (ipAddress != null && ipAddress.IsIPv4MappedToIPv6)
            {
                ipAddress = ipAddress.MapToIPv4();
            }

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(LogTemplateProperties.ClientAddress, ipAddress?.ToString()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(LogTemplateProperties.UserId, userId));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(LogTemplateProperties.UserName, userName));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(LogTemplateProperties.DestinationUserName, legacyUserId));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(LogTemplateProperties.OrganizationId, context.User.Claims.FirstOrDefault(f => f.Type == "org_id")?.Value));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(LogTemplateProperties.OrganizationName, context.User.Claims.FirstOrDefault(f => f.Type == "org_name")?.Value));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("related", new { user = new string?[] { userName, legacyUserId } }, true));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("EndpointName", context.GetEndpoint()?.DisplayName));
        }
    }
}