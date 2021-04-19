using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Mtd.IOCUtility;

namespace Mtd.DbmsRandomizer
{
	public static class DbmsRandomizerRegistrationExtension
	{
		public static IServiceCollection WithDbmsRandomizer(this IServiceCollection serviceCollection)
		{
			foreach (var type in Assembly.GetAssembly(typeof(DbmsRandomizerRegistrationExtension))?.GetTypes()?
				.Where(x => x.IsClass && !x.IsInterface))
			{
				if (type.GetCustomAttribute<SingletonAttribute>() is not null)
					serviceCollection.AddSingleton(type);
				else
					if (type.GetCustomAttribute<TransientAttribute>() is not null)
						serviceCollection.AddTransient(type);
					else
						throw new MissingRegistrationException();
			}

			return serviceCollection;
		}
	}
}
