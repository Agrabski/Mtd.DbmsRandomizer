using System;
using System.Collections.Generic;
using Mtd.IOCUtility;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	[DontRegister]
	public class DatabaseSwitchOptions
	{
		public const string SectionName = "DatabaseSwitch";
		public TimeSpan SwitchInterval { get; set; }
		public List<DatabaseReference> Databases { get; set; }
	}
}
