using System.Reflection;
using Microsoft.OpenApi.Models;

namespace TransactionManagement.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring Swagger documentation generation and usage in an ASP.NET Core application.
    /// </summary>
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Adds Swagger documentation generation to the specified service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to which Swagger documentation generation will be added.</param>
        /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "TransactionManagement", Version = "v1"});

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);
            });

            return services;
        }
    }
}
