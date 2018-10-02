using AspNetCore.TencentCos;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
                //DI: IOptions<TencentCosOptions>
                services.Configure(setupAction);
            }

            services.TryAddSingleton<ITencentCosHandler, TencentCosHandler>();

            return services;
        }
    }
}
