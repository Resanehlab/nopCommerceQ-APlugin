using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Resanehlab.Plugin.Products.ProductComments.Domain;
using Microsoft.EntityFrameworkCore;
using Nop.Data.Extensions;
using System.Data.Common;
using System.Data;

namespace Resanehlab.Plugin.Products.ProductComments.Data
{
    public class ProductCommentsObjectContext : DbContext, IDbContext
    {
        #region Ctor

        public ProductCommentsObjectContext(DbContextOptions<ProductCommentsObjectContext> options) : base(options)
        {
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Add entity to the configuration of the model for a derived context before it is locked down
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductCommentMap());
            modelBuilder.ApplyConfiguration(new ProductCommentHelpfulnessMap());

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

        public virtual int ExecuteSqlCommand(RawSqlString sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            using (var transaction = this.Database.BeginTransaction())
            {
                var result = this.Database.ExecuteSqlCommand(sql, parameters);
                transaction.Commit();

                return result;
            }
        }

        #endregion

        #region Methods

        public void Detach<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            throw new NotImplementedException();
        }

        public string CreateDatabaseScript()
        {
            this.DropPluginTable(this.GetTableName<ProductCommentHelpfulness>());
            this.DropPluginTable(this.GetTableName<ProductComment>());

            return this.Database.GenerateCreateScript();
        }

        public void Install()
        {
            #region Create Tables

            var dbScript = CreateDatabaseScript();
            this.ExecuteSqlScript(dbScript);

            #endregion
        }

        public virtual new DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public virtual string GenerateCreateScript()
        {
            return this.Database.GenerateCreateScript();
        }

        public IQueryable<TQuery> QueryFromSql<TQuery>(string sql) where TQuery : class
        {
            return this.Query<TQuery>().FromSql(sql);
        }

        public IQueryable<TEntity> EntityFromSql<TEntity>(string sql, params object[] parameters) where TEntity : BaseEntity
        {
            return this.Set<TEntity>().FromSql(CreateSqlWithParameters(sql, parameters), parameters);
        }

        protected virtual string CreateSqlWithParameters(string sql, params object[] parameters)
        {
            //add parameters to sql
            for (var i = 0; i <= (parameters?.Length ?? 0) - 1; i++)
            {
                if (!(parameters[i] is DbParameter parameter))
                    continue;

                sql = $"{sql}{(i > 0 ? "," : string.Empty)} @{parameter.ParameterName}";

                //whether parameter is output
                if (parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Output)
                    sql = $"{sql} output";
            }

            return sql;
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