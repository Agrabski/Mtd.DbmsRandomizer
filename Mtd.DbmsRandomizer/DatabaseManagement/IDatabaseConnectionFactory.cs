using System.Data.Common;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	internal interface IDatabaseConnectionFactory
	{
		IDatabase Create(DatabaseReference reference);
	}
}