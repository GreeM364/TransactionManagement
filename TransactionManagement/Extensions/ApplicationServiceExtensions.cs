﻿using Microsoft.EntityFrameworkCore;
using TransactionManagement.DatabaseManager;
using TransactionManagement.DatabaseManager.Interfaces;
using TransactionManagement.Persistence;
using TransactionManagement.Services;
using TransactionManagement.Services.Interfaces;

namespace TransactionManagement.Extensions
{
    /// <summary>
    /// Static class for configuring application services.
    /// </summary>
    public static class ApplicationServiceExtensions
    {
        /// <summary>
        /// Adds application services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="configuration">The configuration options.</param>
        /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TransactionManagementDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton(_ =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection")!;

                return new SqlConnectionFactory(connectionString);
            });

            services.Configure<IpInfoOptions>(configuration.GetSection(nameof(IpInfoOptions)));

            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ITransactionManager, TransactionManager>();
            services.AddScoped<IIpInfoService, IpInfoService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }
    }
}
