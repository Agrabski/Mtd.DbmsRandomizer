using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mtd.DbmsRandomizer.DatabaseManagement;
using TestApplication.Pages;

namespace TestApplication.Controllers
{
	[Route("/customer")]
	public class CustomerController : Controller
	{
		private readonly IDatabaseManager _databaseManager;

		public CustomerController(IDatabaseManager databaseManager)
		{
			_databaseManager = databaseManager;
		}

		[HttpPost]
		[ActionName("AddCustomer")]
		public async Task<IActionResult> AddCustomer(CustomerModel model)
		{
			await _databaseManager.Context.ExecuteNonQueryAsync($"insert into dbo.customers (name, surname, favorite_number) values ({{0}}, {{1}}, '{model.Favorite_Number}')", new CancellationToken(), model.Name, model.Surname);
			return View("index");
		}
	}
}