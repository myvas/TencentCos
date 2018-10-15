using Microsoft.Extensions.DependencyInjection.Extensions;
using Myvas.AspNetCore.TencentCos;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TencentCosServiceCollectionExtensions
    {
        /// <summary>
        /// Using TencentCos Middleware
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> passed to the configuration method.</param>
        /// <param name="setupAction">Middleware configuration options.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddTencentCos(this IServiceCollection services, Action<TencentCosOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction != null)
            {                
                services.Configure(setupAction); //IOptions<TencentCosOptions>
            }

            services.TryAddScoped<ITencentCosHandler, TencentCosHandler>();

            return services;
        }
    }
}
