using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TestApplication.Pages.Shared
{
	public class FindCustomerModel : PageModel
    {
        [BindProperty]
        public string X { get; set; } = "abc";

        public List<CustomerModel> List { get; set; } = new List<CustomerModel>();

    }
}
