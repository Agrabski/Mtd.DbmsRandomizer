using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TestApplication.Pages
{
	public class CustomerModel
	{
		[Required]
		[BindProperty]
		public string Name { get; set; }
		[Required]
		[BindProperty]
		public string Surname { get; set; }
		[Required]
		[BindProperty]
		public string Favorite_Number { get; set; }

	}
}
