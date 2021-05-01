using System.Data.Common;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	internal interface IDatabaseConnectionFactory
	{
		DbConnection Create(DatabaseReference reference);
	}
}