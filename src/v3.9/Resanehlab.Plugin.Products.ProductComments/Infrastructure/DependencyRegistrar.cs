using Autofac;
using Autofac.Core;
using Resanehlab.Plugin.Products.ProductComments.Data;
using Resanehlab.Plugin.Products.ProductComments.Domain;
using Resanehlab.Plugin.Products.ProductComments.Services;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Web.Framework.Mvc;

namespace Resanehlab.Plugin.Products.ProductComments.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<ProductCommentService>().As<IProductCommentService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<ProductCommentsObjectContext>(builder, "nop_object_context_product_comments");

            //override required repository with our custom context
            builder.RegisterType<EfRepository<ProductComment>>()
                .As<IRepository<ProductComment>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_product_comments"))
                .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<ProductCommentHelpfulness>>()
                .As<IRepository<ProductCommentHelpfulness>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_product_comments"))
                .InstancePerLifetimeScope();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 10; }
        }
    }
}
