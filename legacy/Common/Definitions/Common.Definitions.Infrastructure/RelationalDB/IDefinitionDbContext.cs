using Common.Definitions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Definitions.Infrastructure.RelationalDB;

public interface IDefinitionDbContext
{
    // User-Groups
    DbSet<User> AppUsers { get; set; }
    DbSet<UserLocation> AppUserLocations { get; set; }
    DbSet<Role> AppRoles { get; set; }
    DbSet<UserRole> UserRoles { get; set; }
    DbSet<Resource> AppResources { get; set; }
    DbSet<SystemAdmin> AppSystemAdmins { get; set; }
    DbSet<RelSystemUserModule> AppRelSystemUserModules { get; set; }
    DbSet<RolePermission> RolePermissions { get; set; }
    DbSet<RelRoleResource> AppRelRoleResources { get; set; }
    DbSet<UserPushToken> AppUserPushTokens { get; set; }



    DbSet<Module> AppModules { get; set; }

    //AuditLog
    DbSet<AuditLog> AppAuditLogs { get; set; }

    // Mobil Uygulama Versiyon
    DbSet<MobilApplicationVersiyon> MobilApplicationVersiyons { get; set; }

    // Location (İl/İlçe/Mahalle)
    DbSet<Country> Countries { get; set; }
    DbSet<Province> Provinces { get; set; }
    DbSet<District> Districts { get; set; }
    DbSet<Neighborhood> Neighborhoods { get; set; }
}
