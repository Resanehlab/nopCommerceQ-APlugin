using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Text;
using Resanehlab.Plugin.Products.ProductComments.Domain;

namespace Resanehlab.Plugin.Products.ProductComments.Data
{
    public class ProductCommentsObjectContext : DbContext, IDbContext
    {
        #region Properties

        public virtual bool AutoDetectChangesEnabled
        {
            get { return base.Configuration.AutoDetectChangesEnabled; }
            set { base.Configuration.AutoDetectChangesEnabled = value; }
        }

        public virtual bool ProxyCreationEnabled
        {
            get { return base.Configuration.ProxyCreationEnabled; }
            set { base.Configuration.ProxyCreationEnabled = value; }
        }

        #endregion

        #region Ctor

        public ProductCommentsObjectContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Add entity to the configuration of the model for a derived context before it is locked down
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ProductCommentMap());
            modelBuilder.Configurations.Add(new ProductCommentHelpfulnessMap());

            //disable EdmMetadata generation
            //modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
            base.OnModelCreating(modelBuilder);
        }

        protected virtual void ExecuteSqlFile(string path)
        {
            var statements = new List<string>();

            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream))
            {
                string statement;
                while ((statement = ReadNextStatementFromStream(reader)) != null)
                    statements.Add(statement);
            }

            foreach (string stmt in statements)
                base.Database.ExecuteSqlCommand(stmt);
        }

        protected virtual string ReadNextStatementFromStream(StreamReader reader)
        {
            var sb = new StringBuilder();
            while (true)
            {
                var lineOfText = reader.ReadLine();
                if (lineOfText == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();

                    return null;
                }

                if (lineOfText.TrimEnd().ToUpper() == "GO")
                    break;

                sb.Append(lineOfText + Environment.NewLine);
            }

            return sb.ToString();
        }

        #endregion

        #region Methods

        public void Detach(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            ((IObjectContextAdapter)this).ObjectContext.Detach(entity);
        }

        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null,
            params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public string CreateDatabaseScript()
        {
            this.DropPluginTable(this.GetTableName<ProductComment>());
            this.DropPluginTable(this.GetTableName<ProductCommentHelpfulness>());

            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        public void Install()
        {
            Database.SetInitializer<ProductCommentsObjectContext>(null);

            #region Create Tables

            var dbScript = CreateDatabaseScript();
            Database.ExecuteSqlCommand(dbScript);

            #endregion
        }

        public new IDbSet<TEntity> Set<TEntity>()
            where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Uninstall()
        {
            //drop the table
            this.DropPluginTable(this.GetTableName<ProductCommentHelpfulness>());
            this.DropPluginTable(this.GetTableName<ProductComment>());
        }

        #endregion
    }
}