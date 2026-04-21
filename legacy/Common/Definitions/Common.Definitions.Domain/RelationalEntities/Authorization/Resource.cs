using Common.Definitions.Base.Entity;

namespace Common.Definitions.Domain.Entities;

public class Resource : BaseEntity
{
	public string Name { get; set; }
	public string Namespace { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public bool IsSystemAdminPermitted { get; set; } // Sistem yoneticisi icin izin verilsin mi?
	public bool IsEndUserPermitted { get; set; }// Son kullanici icin izin verilsin mi?

	// varligini sorulayalim.
	public bool IsEveryoneAllowed { get; set; }// Herkes icin izin verilsin mi?

	public Guid ModuleId { get; set; }
	public Module Module { get; set; }


	// TODO: will be deleted
	public bool IsCompanyAdminPermitted { get; set; }
	public bool IsModuleAdminPermitted { get; set; }

}