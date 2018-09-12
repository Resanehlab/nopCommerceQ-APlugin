using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;
using Resanehlab.Plugin.Products.ProductComments.Data;

namespace Resanehlab.Plugin.Products.ProductComments.Infrastructure
{
    public class PluginDbStartup : INopStartup
    {

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            services.AddDbContext<ProductCommentsObjectContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServerWithLazyLoading(services);
            });
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 11;
    }
}
