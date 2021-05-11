using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	public interface IDatabaseManager : IDisposable
	{
		void AppendConnection(DbConnection connection);
		Task StartAsync();
		IDatabaseAccessContext Context { get; }

	}
}