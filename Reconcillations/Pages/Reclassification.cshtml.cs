using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Reconcillations.Entity;
using Reconcillations.Repository;
using Reconcillations.Services;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace Reconcillations.Pages
{
    public class ReclassificationModel : PageModel
    {
        ITransactionRepository _transactionRepository;

        IHostEnvironment _hostingEnvironment; private readonly IEmailSender _emailSender;

        public SelectList PostingSelectlist { get; set; }
        [BindProperty]
        public Postinglist pstlist { get; set; }


        public ReclassificationModel(IHostEnvironment hostingEnvironment, ITransactionRepository transactionRepository, IEmailSender emailSender)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;

            _emailSender = emailSender;
        }

        public void OnGet()
        {
            TempData["UserEmail"] = HttpContext.Session.GetString("UserEmail");

            var gh = (from g in _transactionRepository.GetReconciledClosed() select g).ToList();

            PostingSelectlist = new SelectList(gh, "RecperID", "Description");

            HttpContext.Session.Set("psList", gh);
        }
        //public DataSet Reclassified(long reconcileId)
        public IActionResult OnGetReclassified(long reconileID)
        {
            DataSet dt = _transactionRepository.Reclassified(reconileID);

            var serializeBankDt = JsonConvert.SerializeObject(dt.Tables[1]);

            TempData["BankData"] = serializeBankDt;

            return new JsonResult(dt);
        }

        public ActionResult OnPostModify([FromBody] AllocateModify modify)
        {
            DataSet dstresult = new DataSet();

            dstresult = _transactionRepository.AllocateModify(HttpContext.Session.GetString("UserEmail"), modify.dbvalue);

            return new JsonResult(dstresult);
        }

        public ActionResult OnPostCategoryCheck([FromBody] Categorys categorys)
        {
            DataSet dstresult = new DataSet();

            dstresult = _transactionRepository.CategoryCheck(categorys.dbvalue);

            return new JsonResult(dstresult);
        }
    }
}
