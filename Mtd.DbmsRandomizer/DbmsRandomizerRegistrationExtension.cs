using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Mtd.DbmsRandomizer.DatabaseManagement;
using Mtd.IOCUtility;
namespace Mtd.DbmsRandomizer
{
	[DontRegister]
	public static class DbmsRandomizerRegistrationExtension
	{
		public static IServiceCollection WithDbmsRandomizer(this IServiceCollection serviceCollection)
		{
			var assembly = Assembly.GetAssembly(typeof(DbmsRandomizerRegistrationExtension));
			var types = assembly?.GetTypes().Where(x => x.IsClass && !x.IsInterface) ?? throw new MissingRegistrationException();
			foreach (var type in types)
			{
				if (type.GetCustomAttribute<DontRegisterAttribute>() is not null
				   || type.GetCustomAttribute<CompilerGeneratedAttribute>() is not null
				   || type.GetInterfaces().All(x => x.Assembly != assembly))
					continue;
				if (type.GetCustomAttribute<SingletonAttribute>() is not null)
					foreach (var @interface in type.GetInterfaces())
						serviceCollection.AddSingleton(@interface, type);
				else
					if (type.GetCustomAttribute<TransientAttribute>() is not null)
						foreach (var @interface in type.GetInterfaces())
							serviceCollection.AddTransient(@interface, type);
				else
					throw new MissingRegistrationException();
			}

			return serviceCollection;
		}

		public static IServiceCollection ConfigureDbmsRadnomization(this IServiceCollection serviceCollection, IConfiguration configuration)
		{
			return serviceCollection.Configure<DatabaseSwitchOptions>(value =>
				configuration.GetSection(DatabaseSwitchOptions.SectionName).Bind(value));
		}
	}

}
