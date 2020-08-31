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

namespace Reconcillations.Pages.Transaction
{
    public class CreateModel : PageModel
    {

        ITransactionRepository _transactionRepository;
        public SelectList TypeSelectList { get; set; }
        public SelectList ActionList { get; set; }
        public SelectList ElementSelectList { get; set; }
        public element elements { get; set; }
        public typeuser typeuser { get; set; }

        [BindProperty]
        public Entity.Transaction transaction { get; set; }

        [TempData]
        public string Message { get; set; }
        public CreateModel(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        public void OnGet()
        {
            PopulateElement();
            PopulateType();
            PopulateActionlist();
        }
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                //var count = _productRepository.Add(product);
                var count = _transactionRepository.AddTransaction(transaction);

                if (count > 0)
                {
                    Message = "New Product Added Successfully !";
                    // return RedirectToPage("/Transaction/Index");
                }
            }

            return Page();
        }
        void PopulateActionlist()
        {

            var ctpe = from a in _transactionRepository.GetActionTypes() select a;

            ActionList = new SelectList(ctpe, "Id", "Name");
        }
        void PopulateType()
        {
            var ele = from d in _transactionRepository.GetTypes() select d;
            TypeSelectList = new SelectList(ele, "TypeCode", "TypeName");

            HttpContext.Session.Set("psList", ele);
        }
        private void PopulateElement()
        {
            var eleme = from c in _transactionRepository.GetElements() select c;

            ElementSelectList = new SelectList(eleme, "ElementCategoryID", "Name");

            HttpContext.Session.Set("Lists", eleme);
        }
    }
}
