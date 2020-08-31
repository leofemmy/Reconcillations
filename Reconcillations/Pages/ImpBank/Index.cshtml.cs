using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reconcillations.Entity;
using Reconcillations.Repository;

namespace Reconcillations.Pages.ImpBank
{
    public class IndexModel : PageModel
    {
        private ITransactionRepository _transactionRepository;

        public IndexModel(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        [BindProperty]
        public List<TransactionList> _translist { get; set; }
        [TempData]
        public string Message { get; set; }
        public void OnGet()
        {
            _translist = _transactionRepository.GeTransactionLists();
        }

        public IActionResult OnPostDelete(long id)
        {
            ResponseInfo responses = new ResponseInfo();

            if (id > 0)
            {
                responses = _transactionRepository.Deletereconciletransactionlist(id);

                if (responses.StatusCode == "00")
                {
                    Message = responses.StatusMessage;

                    return RedirectToPage("/Reconcile/Index");
                }
                else
                {
                    Message = responses.StatusMessage;
                }
            }

            return Page();

        }
    }
}
