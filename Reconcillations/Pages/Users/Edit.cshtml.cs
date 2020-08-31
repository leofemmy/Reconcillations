using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reconcillations.Entity;
using Reconcillations.Repository;
using Reconcillations.Services;

namespace Reconcillations.Pages.Users
{
    public class EditModel : PageModel
    {
        ITransactionRepository _transactionRepository;

        [BindProperty]
        public UsersList users { get; set; }
        [TempData]
        public string Message { get; set; }

        public SelectList UsertypeSelectlist { get; set; }

        public Usertypelist pstlist { get; set; }

        public EditModel(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        public void OnGet(int id)
        {
            users = _transactionRepository.GetUserid(id);

            var vb = (from g in _transactionRepository.GetUsertypelist() select g).ToList();

            UsertypeSelectlist = new SelectList(vb, "TypeCode", "Description");

            HttpContext.Session.Set("psList", vb);
        }
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                var data = users;

                var count = _transactionRepository.EditUsersAccount(data);

                if (count > 0)
                {
                    return RedirectToPage("/Users/Index");
                }
            }

            return Page();
        }
    }
}
