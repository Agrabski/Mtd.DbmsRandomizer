using Mtd.IOCUtility;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	[DontRegister]
	public class DatabaseReference
	{
		public DbType Type { get; set; }
		public string ConnectionString { get; set; }
	}
}