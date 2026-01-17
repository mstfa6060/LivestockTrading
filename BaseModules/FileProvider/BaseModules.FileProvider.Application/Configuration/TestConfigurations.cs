namespace BaseModules.FileProvider.Application.Configuration;

public class TestConfigurations : ITestOperations
{
    // private readonly ApplicationDbContext _dbContext;
    private readonly ArfBlocksDependencyProvider _dependencyProvider;

    public TestConfigurations(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dependencyProvider = dependencyProvider;
        // _dbContext = dependencyProvider.GetInstance<ApplicationDbContext>();
    }

    public async Task PreExecuting()
    {
        // NOP:
        await Task.CompletedTask;

        // System.Console.WriteLine();

        // System.Console.WriteLine("- Deleting Old Database Data...");
        // // await _dbContext.Database.EnsureDeletedAsync();
        // System.Console.WriteLine("  Deleted.");

        // System.Console.WriteLine("- Migrating Scheme and Default Data...");
        // // await _dbContext.Database.MigrateAsync();
        // System.Console.WriteLine("  Migrated.");

        // System.Console.WriteLine("- Starting Default DB Seeding...");
        // var defaultSeeder = new TestDataSeeder(_dbContext);
        // await defaultSeeder.Seed();
        // System.Console.WriteLine("  Completed.");

        // System.Console.WriteLine("- Setting Current Station...");
        // // Do not change
        // var testCurrentStation = new CurrentStationModel()
        // {
        //     StationId = DefaultData.Station.Id,
        //     Name = DefaultData.Station.Name,
        //     StationLicenseNo = DefaultData.Station.LicenseNo,
        //     StationSubLicenseNo = DefaultData.Station.SubLicenseNo,
        // };
        // _dependencyProvider.Add<CurrentStationModel>(testCurrentStation);
        // System.Console.WriteLine("  Set.");
    }

    public async Task PostExecuting()
    {
        // NOP:
        await Task.CompletedTask;
    }
}