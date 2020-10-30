using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Reconcillations.Repository;
using Newtonsoft.Json.Linq;
using Reconcillations.Entity;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Reconcillations.Pages.Normalise
{
    public class IndexModel : PageModel
    {
        ITransactionRepository _transactionRepository;

        private IHostEnvironment _hostingEnvironment;

        public IndexModel(IHostEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        public void OnGet()
        {
        }

        public IActionResult OnGetAgencyAll()
        {
            List<Agency> data = (from d in _transactionRepository.GetAgencylist()
                                 select d).ToList();
            return new JsonResult(data);
        }

        public IActionResult OnPostSearch([FromBody] JObject referno)
        {
            string strreferno = string.Empty;

            if (!string.IsNullOrWhiteSpace(referno.ToString()))
            {
                foreach (var item in referno)
                {
                    Console.WriteLine(item.Key + " " + item.Value.ToString());

                    if (item.Key.ToString() == "referno")
                    {
                        strreferno = item.Value.ToString();
                    }
                }
            }

            DataSet dssearch = _transactionRepository.SearchReconcilizedRecord(strreferno);

            return new JsonResult(dssearch);
        }

        public IActionResult OnGetRevenueByid(long agancyid)
        {
            List<Revenuelist> revelist = (from d in _transactionRepository.GetRevenuelist(agancyid) select d).ToList();

            return new JsonResult(revelist);
        }

        public ActionResult OnPostNormaliseRec([FromBody] JObject objnormalRec)
        {
            DataSet dstnorm = new DataSet();
            //var _bankrecod = NormaliseRec;
            //NormaliseRec
            NormaliseRec _normaliseRec = new NormaliseRec();

            List<BankImport> bn = new List<BankImport>();

            ResponseMessage<BankImport> hc = new ResponseMessage<BankImport>();

            if (!string.IsNullOrWhiteSpace(objnormalRec.ToString()))
            {

                foreach (var item in objnormalRec)
                {
                    Console.WriteLine(item.Key + " " + item.Value.ToString());

                    if (item.Key.ToString() == "useremail")
                    {
                        _normaliseRec.useremail = item.Value.ToString();
                    }
                    else if (item.Key.ToString() == "payername")
                    {
                        _normaliseRec.payername = item.Value.ToString();
                    }
                    else if (item.Key.ToString() == "paymentref")
                    {
                        _normaliseRec.paymentref = item.Value.ToString();
                    }
                    else if (item.Key.ToString() == "agencyname")
                    {
                        _normaliseRec.agencyname = item.Value.ToString();
                    }
                    else if (item.Key.ToString() == "agencycode")
                    {
                        _normaliseRec.agencycode = item.Value.ToString();
                    }
                    else if (item.Key.ToString() == "revenuename")
                    {
                        _normaliseRec.revenuename = item.Value.ToString();
                    }
                    else if (item.Key.ToString() == "revenuecode")
                    {
                        _normaliseRec.revenuecode = item.Value.ToString();
                    }
                }

                dstnorm = _transactionRepository.SaveNormaliseRecord(HttpContext.Session.GetString("UserEmail"), _normaliseRec.paymentref, _normaliseRec.payername, _normaliseRec.agencyname, _normaliseRec.agencycode, _normaliseRec.revenuename, _normaliseRec.revenuecode);

            }

            return new JsonResult(dstnorm);
        }
        //NormaliseRec
    }
}
