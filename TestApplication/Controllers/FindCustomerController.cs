using Microsoft.AspNetCore.Mvc;
using Mtd.DbmsRandomizer.DatabaseManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestApplication.Pages;
using TestApplication.Pages.Shared;

namespace TestApplication.Controllers
{
	[Route("/findCustomer")]
	public class FindCustomerController : Controller
	{
		private readonly IDatabaseManager _databaseManager;

		public IActionResult Index()
		{
			return View();
		}

		public FindCustomerController(IDatabaseManager databaseManager)
		{
			_databaseManager = databaseManager;
		}
		[HttpPost]
		public ActionResult FindCustomer(string surname)
		{
			var result = _databaseManager.Context.ExecuteQueryAsync<CustomerModel>($"select * from dbo.customers where surname = {{0}}", new CancellationToken(), surname);
			var r = result.ToListAsync();
			var model = new FindCustomerModel()
			{
				List = r.Result
			};
			return View(model);
		}

	}
}
