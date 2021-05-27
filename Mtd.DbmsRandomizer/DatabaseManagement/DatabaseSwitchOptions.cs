using System;
using System.Collections.Generic;
using Mtd.IOCUtility;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	[DontRegister]
	public class DatabaseSwitchOptions
	{
		public const string SectionName = "DatabaseSwitch";
		public int MinimumSwitchInterval { get; set; }
		public int MaximumSwitchInterval { get; set; }
		public List<DatabaseReference> Databases { get; set; }
	}
}
