namespace Common.Services.Caching;

/// <summary>
/// Centralized cache key definitions and patterns
/// </summary>
public static class CacheKeys
{
    // Authorization
    public static class Authorization
    {
        private const string Prefix = "auth:";

        public static string UserRoles(Guid userId) => $"{Prefix}user-roles:{userId}";
        public static string UserPermissions(Guid userId) => $"{Prefix}user-permissions:{userId}";
        public static string ResourcePermissions(string resourceNamespace) => $"{Prefix}resource:{resourceNamespace}";
        public static string SystemAdmin(Guid userId) => $"{Prefix}system-admin:{userId}";
        public static string UserRolesPattern => $"{Prefix}user-roles:*";
        public static string UserPermissionsPattern => $"{Prefix}user-permissions:*";
    }

    // Reference Data
    public static class Reference
    {
        private const string Prefix = "ref:";

        public static string AnimalBreeds => $"{Prefix}animal-breeds";
        public static string AnimalBreedById(Guid id) => $"{Prefix}animal-breed:{id}";
        public static string AnimalBreedsPattern => $"{Prefix}animal-breed*";

        public static string Skills(string language = "tr") => $"{Prefix}skills:{language}";
        public static string SkillById(Guid id) => $"{Prefix}skill:{id}";
        public static string SkillsPattern => $"{Prefix}skill*";

        public static string ServiceAreas => $"{Prefix}service-areas";
        public static string ServiceAreaById(Guid id) => $"{Prefix}service-area:{id}";
        public static string ServiceAreasPattern => $"{Prefix}service-area*";
    }

    // Location-based
    public static class Location
    {
        private const string Prefix = "loc:";

        public static string NearbyJobs(double latitude, double longitude, int radiusKm) =>
            $"{Prefix}nearby-jobs:{latitude:F4}:{longitude:F4}:{radiusKm}";

        public static string NearbyWorkers(double latitude, double longitude, int radiusKm) =>
            $"{Prefix}nearby-workers:{latitude:F4}:{longitude:F4}:{radiusKm}";

        public static string AvailableVeterinarians(Guid farmId) =>
            $"{Prefix}available-vets:{farmId}";

        public static string NearbyJobsPattern => $"{Prefix}nearby-jobs:*";
        public static string NearbyWorkersPattern => $"{Prefix}nearby-workers:*";
    }

    // User Context
    public static class User
    {
        private const string Prefix = "user:";

        public static string Profile(Guid userId) => $"{Prefix}profile:{userId}";
        public static string Company(Guid userId) => $"{Prefix}company:{userId}";
        public static string ProfilePattern => $"{Prefix}profile:*";
    }

    // Pick/Dropdown Data
    public static class Pick
    {
        private const string Prefix = "pick:";

        public static string Companies => $"{Prefix}companies";
        public static string Farms(Guid userId) => $"{Prefix}farms:{userId}";
        public static string Users(Guid companyId) => $"{Prefix}users:{companyId}";
        public static string FarmsPattern => $"{Prefix}farms:*";
    }

    // Entities
    public static class Entity
    {
        private const string Prefix = "entity:";

        public static string Job(Guid id) => $"{Prefix}job:{id}";
        public static string Farm(Guid id) => $"{Prefix}farm:{id}";
        public static string Animal(Guid id) => $"{Prefix}animal:{id}";
        public static string Payment(Guid id) => $"{Prefix}payment:{id}";
    }
}
