using System.ComponentModel.DataAnnotations.Schema;
using Common.Definitions.Base.Entity;

namespace Common.Definitions.Domain.Entities;

public class Module : CoreEntity
{
    public string Key { get; set; }
    public string DisplayName { get; set; }
    public string Name { get; set; }
    public bool IsDefault { get; set; }

    public ModuleTypes ModuleType { get; set; }
    public ModuleCategories ModuleCategory { get; set; }

    public List<Resource> Resource { get; set; }


}

public enum ModuleCategories
{
    BaseModule = 0,
    BusinessModule = 1,
    SystemModule = 2,
}

public enum ModuleTypes
{
    Hirovo = 0,
    IAM = 1,
    FileProvider = 2,
    AnimalMarket = 3
}