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

namespace Reconcillations.Pages.Bank
{
    public class CreateModel : PageModel
    {
        ITransactionRepository _transactionRepository;

        public SelectList BankSelectList { get; set; }
        public SelectList CurrencySelectList { get; set; }
        public SelectList Banktypecode { get; set; }

        [BindProperty]
        public Entity.Bank Bank { get; set; }

        [TempData]
        public string Message { get; set; }
        public Currencys currency { get; set; }
        public BankType bankType { get; set; }
        public Classtypes classtypes { get; set; }
        public CreateModel(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        public void OnGet()
        {//load currency into a combo
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
            //PopulateType();
            //PopulateCurrency();
        }
        void PopulateCurrency()
        {
            var ele = from d in _transactionRepository.GetCurrencies() select d;
            CurrencySelectList = new SelectList(ele, "CurrencyID", "CurrencyName");

        }
        void PopulateType()
        {
            var ele = from d in _transactionRepository.GeBanklists() select d;
            BankSelectList = new SelectList(ele, "BankCode", "BankName");
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                //var count = _productRepository.Add(product);
                var count = _transactionRepository.AddBankAccount(Bank);

                if (count > 0)
                {
                    Message = "Successfully !";
                    return RedirectToPage("/Bank/Index");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Message = errors.ToString();
            }

            return Page();
        }
    }
}
