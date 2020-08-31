using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reconcillations.Repository;
using Reconcillations.Services;

namespace Reconcillations.Pages.Bank
{
    public class EditModel : PageModel
    {
        ITransactionRepository _transactionRepository;
        public SelectList BankSelectList { get; set; }
        public SelectList CurrencySelectList { get; set; }
        public SelectList Banktypecode { get; set; }

        [BindProperty]
        public Entity.Bank Bank { get; set; }

        [TempData]
        public string Message { get; set; }

        public EditModel(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        public void OnGet(int id)
        {    //load currency into a combo
            var ele = from d in _transactionRepository.GetCurrencies() select d;
            CurrencySelectList = new SelectList(ele, "CurrencyID", "CurrencyName");
            HttpContext.Session.Set("psList", ele);

            //load banklist into a combox
            var eled = from d in _transactionRepository.GeBanklists() select d;
            BankSelectList = new SelectList(eled, "BankCode", "BankName");
            HttpContext.Session.Set("bnList", eled);

            var bcode = from gh in _transactionRepository.GetBankTypecodes() select gh;
            Banktypecode = new SelectList(bcode, "Code", "Description");
            HttpContext.Session.Set("bcodeList", bcode);

            Bank = _transactionRepository.GetBankAccountId(id);
        }
        public IActionResult OnPost()
        {
            var data = Bank;

            if (ModelState.IsValid)
            {
                var count = _transactionRepository.EditBankAccunt(data);

                if (count > 0)
                {
                    return RedirectToPage("/Bank/Index");
                }
            }

            return Page();
        }
    }
}
