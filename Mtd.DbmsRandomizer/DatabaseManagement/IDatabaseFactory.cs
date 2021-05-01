using System.Data.Common;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	public interface IDatabaseFactory
	{
		DbType HandledType { get; }
		DbConnection Create(string connectionString);
	}
}
