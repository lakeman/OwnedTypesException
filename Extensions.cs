using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq.Expressions;

namespace OwnedTypes
{
    public static class Extensions
    {
        public static Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder<string> HasEnum<T, C>(this EntityTypeBuilder<C> builder, Expression<Func<C, EnumWrapper<T>>> navigationExpression)
            where C : class
            where T : struct
        {
            var entityBuilder = builder.OwnsOne(navigationExpression);
            entityBuilder.Ignore(e => e.Value);
            var propBuilder = entityBuilder
                .Property(e => e._Raw);
            return propBuilder
                .HasColumnName(Database.ToSnakeCase(entityBuilder.OwnedEntityType.DefiningNavigationName))
                .IsRequired()
                .HasMaxLength(20);
        }

        public static void HasAudit<C>(this EntityTypeBuilder<C> builder)
            where C : class, IAudit
        {
            var entityBuilder = builder.OwnsOne(a => a.Audit);
            var propBuilder = entityBuilder
                .Property(e => e.Modified)
                .IsRequired();
        }

    }
}
