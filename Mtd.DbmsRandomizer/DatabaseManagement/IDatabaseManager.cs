using System;
using System.Data.Common;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	public interface IDatabaseManager : IDisposable
	{
		void AppendConnection(DbConnection connection);
		void Start();
		IDatabaseAccessContext Context { get; }

	}
}