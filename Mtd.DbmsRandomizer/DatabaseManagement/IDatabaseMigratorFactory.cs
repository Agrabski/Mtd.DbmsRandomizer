using System.Data.Common;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	internal interface IDatabaseMigratorFactory
	{
		IDatabaseMigrator Create(IDatabase from, IDatabase to);
	}
}
