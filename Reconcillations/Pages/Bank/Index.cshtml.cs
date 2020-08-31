using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reconcillations.Repository;

namespace Reconcillations.Pages.Bank
{
    public class IndexModel : PageModel
    {
        private ITransactionRepository _transactionRepository;
        public IndexModel(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        [BindProperty]
        public List<Entity.Bank> _BankLists { get; set; }

        [TempData]
        public string Message { get; set; }
        public void OnGet()
        {
            _BankLists = _transactionRepository.GetBanklistList();
        }
        public IActionResult OnPostDelete(long AccountID)
        {
            if (AccountID > 0)
            {
                var count = _transactionRepository.DisableBankAccount(AccountID);
                if (count > 0)
                {
                    Message = "Record Disable Successfully !";
                    return RedirectToPage("/Bank/Index");
                }
            }

            return Page();

        }
    }
}
