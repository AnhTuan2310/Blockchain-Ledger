using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.RegularExpressions;

namespace Infrastructure.Persistence.Converters {
    public static class SnakeCaseConvention {
        private static readonly Regex _regex = new Regex("(?<!^)([A-Z])", RegexOptions.Compiled);

        public static string ToSnakeCase(string input) {
            if (string.IsNullOrEmpty(input))
                return input;

            return Regex.Replace(
                input,
                "([a-z0-9])([A-Z])",
                "$1_$2"
            ).ToLower();
        }



        public static void UseSnakeCaseConvention(this ModelBuilder modelBuilder) {
            foreach (var entity in modelBuilder.Model.GetEntityTypes()) {
                // Table name
                entity.SetTableName(ToSnakeCase(entity.GetTableName()!));

                // Columns
                foreach (var property in entity.GetProperties()) {
                    property.SetColumnName(ToSnakeCase(property.Name));
                }

                // Keys
                foreach (var key in entity.GetKeys()) {
                    key.SetName(ToSnakeCase(key.GetName()!));
                }

                // Foreign keys
                foreach (var fk in entity.GetForeignKeys()) {
                    fk.SetConstraintName(ToSnakeCase(fk.GetConstraintName()!));
                }

                // Indexes
                foreach (var index in entity.GetIndexes()) {
                    var indexName = index.GetDatabaseName();
                    if (!string.IsNullOrWhiteSpace(indexName))
                        index.SetDatabaseName(ToSnakeCase(indexName));
                }
            }
        }
    }
}

