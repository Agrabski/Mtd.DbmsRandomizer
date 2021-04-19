using System.Data.Common;

namespace Mtd.DbmsRandomizer.Query
{
	internal interface IQuerierFactory
	{
		IQuerier Create(DbConnection connection);
	}
}