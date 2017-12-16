namespace DocWorkflow.Areas.Admin.Models.DataModels
{
    using DocWorkflow;
    using DocWorkflow.Areas.Admin.Models.DataModels;
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Web;


    /// <summary>
    /// Контекст данных EntityFramework
    /// </summary>
    public partial class DataContext : DbContext
    {
        public DataContext()
            : base(ConfigurationManager.ConnectionStrings["MsSqlConnectionString"].ConnectionString.Replace("{ROOT_DIR}", HttpContext.Current.Server.MapPath("~/")))
        {
            Database.SetInitializer<DataContext>(null);
        }

        /// <summary>
        /// Получение уникального идентификатора из sequence
        /// </summary>
        /// <returns></returns>
        public static int GetUniqueId(String sequenceName)
        {
            using (DataContext ctx = new DataContext())
            {
                var query = ctx.Database.SqlQuery(typeof(int), 
                    String.Format("select next value for {0}", sequenceName), 
                    new object[0]);
                var queryResult = query.GetEnumerator();
                queryResult.MoveNext();
                var value = queryResult.Current;
                return Convert.ToInt32(value);
            }
        }


        /// <summary>
        /// Получение уникального идентификатора из sequence SEQ_GENERAL_ID
        /// </summary>
        /// <returns></returns>
        public static int GetUniqueId()
        {
            return GetUniqueId("SEQ_GENERAL_ID");
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>()
                .HasMany(e => e.Persons)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DocStatus>()
                .HasMany(e => e.Document)
                .WithRequired(e => e.DocStatus)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DocType>()
                .HasMany(e => e.Documents)
                .WithRequired(e => e.DocType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.DepartmentsOwned)
                .WithOptional(e => e.DepartmentHead)
                .HasForeignKey(e => e.DepartmentHeadId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.DocumentsCreated)
                .WithRequired(e => e.Creator)
                .HasForeignKey(e => e.CreatorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.DocumentsExecuted)
                .WithRequired(e => e.Executor)
                .HasForeignKey(e => e.ExecutorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.DocumentsSigned)
                .WithRequired(e => e.Signer)
                .HasForeignKey(e => e.SignerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserGroup>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.UserGroup)
                .HasForeignKey(e => e.UserGroup_UserGroupId);

            modelBuilder.Entity<UserGroup>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.UserGroup)
                .HasForeignKey(e => e.UserGroup_UserGroupId);
        }

        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<DocStatus> DocStatus { get; set; }
        public virtual DbSet<DocType> DocType { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserGroup> UserGroup { get; set; }        
    }
}
