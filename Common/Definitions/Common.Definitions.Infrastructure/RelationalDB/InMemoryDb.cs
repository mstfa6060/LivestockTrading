using Common.Definitions.Domain.Entities;
using Common.Definitions.Base.Entity;
using Microsoft.EntityFrameworkCore.Storage;

namespace Common.Definitions.Infrastructure.RelationalDB;

public class InMemoryDb
{
	public static readonly InMemoryDatabaseRoot InMemoryDatabaseRoot = new InMemoryDatabaseRoot();
}