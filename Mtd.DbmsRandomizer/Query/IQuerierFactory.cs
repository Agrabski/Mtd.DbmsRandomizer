using System.Data.Common;
using Mtd.DbmsRandomizer.DatabaseManagement;

namespace Mtd.DbmsRandomizer.Query
{
	public interface IQuerierFactory
	{
		IQuerier Create(IDatabase connection);
	}
}