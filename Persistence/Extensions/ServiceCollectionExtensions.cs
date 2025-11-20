using Persistence.Repositories.Interfaces;
using Persistence.Interfaces;
using Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using Persistence.Mappers;

namespace Persistence.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IFileRepository, FileRepository>();
        }

        public static void AddDatabase(this IServiceCollection services)
        {
            SqlMapper.Settings.CommandTimeout = 60;
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            
            SetupMappers();

            services.AddScoped<IConnectionManager, ConnectionManager>();
            services.AddScoped<ITransactionManager, TransactionManager>();
            services.AddRepositories();
        }
        
        private static void SetupMappers()
        {
            SqlMapper.AddTypeHandler(new DateTimeMapper());
        }
    }
}