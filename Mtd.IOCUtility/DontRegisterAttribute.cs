using System;
using System.Collections.Generic;
using System.Text;

namespace Mtd.IOCUtility
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DontRegisterAttribute : Attribute
	{
	}
}
