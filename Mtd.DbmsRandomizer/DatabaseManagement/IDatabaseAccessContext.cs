using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	public interface IDatabaseAccessContext
	{
		ILiteral CreateLiteral(string value);
		ILiteral CreateLiteral(DateTime value);

		Task<T> ExecuteQueryAsync<T>(string query, CancellationToken token, params ILiteral[] literals);

		Task ExecuteNonQueryAsync(string nonQuery, CancellationToken token, params ILiteral[] literals);
	}
}