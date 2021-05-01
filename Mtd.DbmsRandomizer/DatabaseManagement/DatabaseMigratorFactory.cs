using Mtd.IOCUtility;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	[Singleton]
	internal class DatabaseMigratorFactory : IDatabaseMigratorFactory
	{
		public IDatabaseMigrator Create(IDatabase @from, IDatabase to)
		{
			return new DatabaseMigrator(from, to);
		}
	}
}