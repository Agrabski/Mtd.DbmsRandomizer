using System;
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

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> AddCustomer(AddCustomerModel model)
		{
			await _databaseManager.Context.ExecuteNonQueryAsync($"insert into dbo.customers (name, surname, favorite_number) values ('{model.Name}', '{model.Surname}', '{model.FavoriteNumber}')", new CancellationToken());
			return View();
		}
	}
}