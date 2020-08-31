using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reconcillations.Repository;

namespace Reconcillations.Pages.Transaction
{
    public class IndexModel : PageModel
    {
        private ITransactionRepository _transactionRepository;
        [BindProperty]
        public List<Entity.Transaction> _Transactionslist { get; set; }

        [BindProperty]
        public Entity.Transaction _Transaction { get; set; }

        [TempData]
        public string Message { get; set; }
        public IndexModel(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        public void OnGet()
        {
            _Transactionslist = _transactionRepository.GetTransactions();
        }

        public IActionResult OnPostDelete(int id)
        {
            if (id > 0)
            {
                var count = _transactionRepository.DisableTransction(id);/* _productRepository.DeleteProdcut(id);*/
                if (count > 0)
                {
                    Message = "Record Disable Successfully !";
                    return RedirectToPage("/Transaction/Index");
                }
            }

            return Page();

        }
    }
}
