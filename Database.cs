using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OwnedTypes
{
    public class Database : DbContext
    {
        public Database()
        {
        }

        public Database(DbContextOptions<Database> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=OwnedTypes;integrated security=True;MultipleActiveResultSets=True;Trusted_Connection=true;");
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasAudit();
                entity.HasEnum(e => e.Status);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasAudit();
                entity.HasEnum(e => e.Genre);
                entity.HasEnum(e => e.Section);
                entity.HasEnum(e => e.Status);
                entity.HasOne(e => e.Author)
                    .WithMany(e => e.Books)
                    .HasForeignKey(e => e.AuthorId);
            });


            // Make sure all tables and columns default to snake case, with id keys
            foreach (var table in modelBuilder.Model.GetEntityTypes())
            {
                if (table.IsOwned())
                    continue;

                var builder = modelBuilder.Entity(table.Name);
                builder.ToTable(ToSnakeCase(table.SqlServer().TableName));

                foreach (var col in builder.Metadata.GetProperties())
                {
                    var newName = ToSnakeCase(col.SqlServer().ColumnName);
                    builder.Property(col.Name).HasColumnName(newName);

                    if (newName == "id" && table.FindPrimaryKey() == null)
                        builder.HasKey("Id");
                }
            }
        }

        private void Validate()
        {
            var serviceProvider = this.GetService<IServiceProvider>();
            var items = new Dictionary<object, object>();

            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                    case EntityState.Modified:
                        {
                            var audit = entry.Entity as IAudit;
                            if (audit != null)
                            {
                                // set audit fields with dummy values to pass validation
                                if (audit.Audit == null)
                                    audit.Audit = new Audit();
                                audit.Audit.Modified = DateTime.Now;
                            }
                            Validator.ValidateObject(entry.Entity, new ValidationContext(entry.Entity, serviceProvider, items), true);
                        }
                        break;

                }
            }
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            Validate();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool accept)
        {
            Validate();
            return base.SaveChanges(accept);
        }

        private static Regex _findUpperCase = new Regex(@"((?<=.)[A-Z][a-zA-Z])|((?<=[a-zA-Z])\d+)");
        public static string ToSnakeCase(string value)
        {
            value = _findUpperCase.Replace(value, @"_$1$2").ToLower();
            while (value.StartsWith("_"))
                value = value.Substring(1);
            return value;
        }

        public virtual DbSet<Author> Author { get; set; }
        public virtual DbSet<Book> Book { get; set; }

    }
}
