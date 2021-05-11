using System.Collections.Generic;
using System.Linq;
using Mtd.IOCUtility;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	[Singleton]
	internal class DatabaseConnectionFactory : IDatabaseConnectionFactory
	{
		private readonly IEnumerable<IDatabaseFactory> _factories;

		public DatabaseConnectionFactory(IEnumerable<IDatabaseFactory> factories)
		{
			_factories = factories;
		}

		public IDatabase Create(DatabaseReference reference)
		{
			return _factories.First(x => x.HandledType == reference.Type).Create(reference.ConnectionString);
		}
	}
}
