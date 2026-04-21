namespace Common.Definitions.Infrastructure.RelationalDB;

public static class CommonModelBuilder
{
    public static void Build(ModelBuilder modelBuilder)
    {
        RoleRelations.Build(modelBuilder);
    }
}