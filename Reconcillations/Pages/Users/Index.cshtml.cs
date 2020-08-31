using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reconcillations.Repository;

namespace Reconcillations.Pages.Users
{
    public class IndexModel : PageModel
    {
        private ITransactionRepository _transactionRepository;

        [BindProperty]
        public List<Entity.UsersList> _usersLists { get; set; }

        [TempData]
        public string Message { get; set; }
        public IndexModel(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        public void OnGet()
        {
            _usersLists = _transactionRepository.GetUserAccountlist();
        }
        public IActionResult OnPostDelete(long UserID)
        {
            if (UserID > 0)
            {
                var count = _transactionRepository.DisableUserAccount(UserID);

                if (count > 0)
                {
                    Message = "Record Disable Successfully !";
                    return RedirectToPage("/Users/Index");
                }
            }

            return Page();

        }
    }
}
