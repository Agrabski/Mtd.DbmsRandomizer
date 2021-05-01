using System.Data.Common;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	public interface IDatabaseFactory
	{
		DbType HandledType { get; }
		IDatabase Create(string connectionString);
	}
}
