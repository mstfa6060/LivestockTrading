namespace Common.Definitions.Domain.Entities
{
    public class SystemAdmin
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
        public bool IsAllModulePermitted { get; set; }

        // İlişkilendirme
        public List<RelSystemUserModule> RelSystemUserModules { get; set; }
    }
}
