using DevExpress.DataAccess.Native.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NPOI.SS.Formula.Functions;
using Reconcillations.Entity;
using Reconcillations.Repository;
using System.Collections.Generic;

namespace Reconcillations.Pages
{

    public class ReemsRecordsModel : PageModel
    {
        private ITransactionRepository _transactionRepository;
        public ReemsRecordsModel(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        [BindProperty]
        public List<Entity.ReemsRec> _reemsrec { get; set; }
        public void OnGet()
        {
            _reemsrec = _transactionRepository.GetReems();
        }
        public IActionResult OnPostPushed()
        {
            string useremail = string.Empty; string retval = string.Empty;

            if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("UserEmail").ToString()))
            {
                useremail = HttpContext.Session.GetString("UserEmail").ToString();
            }

            retval = _transactionRepository.PushedRecordtoReems(useremail);

            
            //return new PageResult();
            OnGet();
            return new JsonResult(retval);
        }

    }
}
