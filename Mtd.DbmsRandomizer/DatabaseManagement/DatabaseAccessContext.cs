using System;
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

		public Task<T> ExecuteQueryAsync<T>(string query, CancellationToken token, params ILiteral[] literals)
		{
			return _queryExecutor.Execute(
				async connection =>
					await connection.ExecuteAsync<T>(string.Format(query, literals.Select(FillLiteral)), token), token);
		}

		public Task ExecuteNonQueryAsync(string nonQuery, CancellationToken token, params ILiteral[] literals)
		{
			return _queryExecutor.Execute(
				async connection =>
					await connection.ExecuteAsync(string.Format(nonQuery, literals.Select(FillLiteral)), token), token);

		}

		private string FillLiteral(ILiteral literal)
		{
			return literal.ToString(_queryExecutor.DbmsType);
		}
	}
}
