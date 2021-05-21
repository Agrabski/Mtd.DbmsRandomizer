using Microsoft.Extensions.DependencyInjection;
using Mtd.DbmsRandomizer.DatabaseManagement;
using Mtd.DbmsRandomizer.Query;

namespace Mtd.DbmsRandomizer.Mysql
{
	public static class MysqlRandomizerRegistrationExtension
	{
		public static IServiceCollection WithMysqlRandomization(this IServiceCollection collection)
		{
			return collection
				.AddSingleton<IDatabaseFactory, MysqlDatabaseFactory>()
				.AddSingleton<IQuerierFactory, MysqlQuerierFactory>();
		}
	}
}
