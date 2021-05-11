using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TestApplication.Pages
{
	public class AddCustomerModel
	{
		[Required]
		[BindProperty]
		public string Name { get; set; }
		[Required]
		[BindProperty]
		public string Surname { get; set; }
		[Required]
		[BindProperty]
		public string FavoriteNumber { get; set; }

	}
}
