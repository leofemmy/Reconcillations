using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reconcillations.Repository;

namespace Reconcillations.Pages.Transaction
{
    public class EditModel : PageModel
    {
        ITransactionRepository _transactionRepository;
        public SelectList TypeSelectList { get; set; }
        public SelectList ActionList { get; set; }
        public SelectList ElementSelectList { get; set; }
        [BindProperty]
        public Entity.Transaction trans { get; set; }

        [TempData]
        public string Message { get; set; }

        public EditModel(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        public void OnGet(int id)
        {
            PopulateType();
            PopulateElement();
            PopulateActionlist();
            trans = _transactionRepository.GetTransactionID(id);
        }
        private void PopulateElement()
        {
            var element = from c in _transactionRepository.GetElements()
                          select c;
            ElementSelectList = new SelectList(element, "ElementCategoryID", "Name");
        }
        void PopulateType()
        {
            var ele = from d in _transactionRepository.GetTypes() select d;
            TypeSelectList = new SelectList(ele, "TypeCode", "TypeName");
        }
        void PopulateActionlist()
        {
            var ctpe = from a in _transactionRepository.GetActionTypes() select a;
            ActionList = new SelectList(ctpe, "Id", "Name");
        }
        public IActionResult OnPost()
        {
            var data = trans;

            if (ModelState.IsValid)
            {
                var count = _transactionRepository.EditTransaction(data);
                if (count > 0)
                {
                    Message = "Record Modified Successfully";
                    //return RedirectToPage("/Transaction/Index");
                }
            }

            return Page();
        }
    }
}
