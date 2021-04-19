using System.Threading;
using System.Threading.Tasks;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	internal interface IDatabaseMigrator
	{
		Task MigrateAsync(CancellationToken cancellationToken);
	}
}