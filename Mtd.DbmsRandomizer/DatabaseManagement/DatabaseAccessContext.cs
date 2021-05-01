using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mtd.DbmsRandomizer.Query;
using Mtd.IOCUtility;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	[DontRegister]
	internal class DatabaseAccessContext : IDatabaseAccessContext
	{
		private readonly IQueryExecutor _queryExecutor;

		public DatabaseAccessContext(IQueryExecutor queryExecutor)
		{
			_queryExecutor = queryExecutor;
		}

		public ILiteral CreateLiteral(string value)
		{
			throw new NotImplementedException();
		}

		public ILiteral CreateLiteral(DateTime value)
		{
			throw new NotImplementedException();
		}

		public IAsyncEnumerable<T> ExecuteQueryAsync<T>(string query, CancellationToken token, params ILiteral[] literals) where T : new()
		{
			return _queryExecutor.ExecuteAsync(
				connection =>
					connection.ExecuteAsync<T>(string.Format(query, literals.Select(FillLiteral)), token), token);
		}

		public Task ExecuteNonQueryAsync(string nonQuery, CancellationToken token, params ILiteral[] literals)
		{
			return _queryExecutor.ExecuteAsync(
				async connection =>
					await connection.ExecuteAsync(string.Format(nonQuery, literals.Select(FillLiteral)), token), token);

		}

		private string FillLiteral(ILiteral literal)
		{
			return literal.ToString(_queryExecutor.DbmsType);
		}
	}
}
