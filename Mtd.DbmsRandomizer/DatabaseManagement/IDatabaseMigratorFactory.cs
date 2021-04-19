using System.Data.Common;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	internal interface IDatabaseMigratorFactory
	{
		IDatabaseMigrator Create(DbConnection from, DbConnection to);
	}
}
