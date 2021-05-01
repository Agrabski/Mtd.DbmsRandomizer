using Microsoft.Extensions.DependencyInjection;
using Mtd.DbmsRandomizer.DatabaseManagement;
using Mtd.DbmsRandomizer.Query;

namespace Mtd.DbmsRandomizer.Mssql
{
	public static class MssqlRandomizerRegistrationExtension
	{
		public static IServiceCollection WithMssqlRandomization(this IServiceCollection collection)
		{
			return collection
				.AddSingleton<IDatabaseFactory, MssqlDatabaseFactory>()
				.AddSingleton<IQuerierFactory, MssqlQuerierFactory>();
		}
	}
}
