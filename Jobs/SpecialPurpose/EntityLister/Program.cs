using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Jobs.RelationalDB.MigrationJob; // ApplicationDbContext burada

class Program
{
    static void Main(string[] args)
    {
        var dbContextType = typeof(ApplicationDbContext);

        var dbSets = dbContextType.GetProperties()
            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .ToList();

        foreach (var dbSet in dbSets)
        {
            var entityType = dbSet.PropertyType.GetGenericArguments().First();
            Console.WriteLine($"Entity: {entityType.Name}");

            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                Console.WriteLine($"  - {property.PropertyType.Name} {property.Name}");
            }

            Console.WriteLine();
        }
    }
}
