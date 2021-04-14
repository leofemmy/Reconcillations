using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Reconcillations.Entity;
using Reconcillations.Reports;
using Reconcillations.Repository;
using Reconcillations.Services;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Reconcillations.Pages.Normalise
{
    public class IndexModel : PageModel
    {
        ITransactionRepository _transactionRepository;

        private IHostEnvironment _hostingEnvironment;

        public SelectList AccountSelectlist { get; set; }

        [BindProperty]
        public NorRec norrec { get; set; }

        [BindProperty]
        public Entity.Exceptions bankimport { get; set; }

        [BindProperty]
        public Accountlists actlist { get; set; }

        public IndexModel(IHostEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        public void OnGet()
        {
            var dat = (from d in _transactionRepository.GetAccountlists()
                       select d).ToList();

            AccountSelectlist = new SelectList(dat, "AccountID", "AccountName");

            HttpContext.Session.Set("actlist", dat);
        }

        public IActionResult OnGetAgencyAll()
        {
            List<Agency> data = (from d in _transactionRepository.GetAgencylist()
                                 select d).ToList();
            return new JsonResult(data);
        }

        public IActionResult OnGetAccountlist()
        {
            List<Accountlists> data = (from d in _transactionRepository.GetAccountlists()
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
                    else if (item.Key.ToString() == "address")
                    {
                        _normaliseRec.Address = item.Value.ToString();
                    }
                }

                dstnorm = _transactionRepository.SaveNormaliseRecord(HttpContext.Session.GetString("UserEmail"), _normaliseRec.paymentref, _normaliseRec.payername, _normaliseRec.agencyname, _normaliseRec.agencycode, _normaliseRec.revenuename, _normaliseRec.revenuecode, _normaliseRec.Address);

            }

            return new JsonResult(dstnorm);
        }

        public IActionResult OnPostGetRecord([FromBody] JObject objexception)
        {
            var _bankrecod = bankimport;

            if (!string.IsNullOrWhiteSpace(objexception.ToString()))
            {
                foreach (var item in objexception)
                {
                    Console.WriteLine(item.Key + " " + item.Value.ToString());

                    if (item.Key.ToString() == "accountID")
                    {
                        _bankrecod.AccountID = Convert.ToInt64(item.Value.ToString());
                    }
                    else if (item.Key.ToString() == "startdate")
                    {
                        //_recondays.Startdate = DateTime.ParseExact(item.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        _bankrecod.Startdate = DateTime.ParseExact(item.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    }
                    else if (item.Key.ToString() == "transID")
                    {
                        _bankrecod.TransID = Convert.ToInt64(item.Value.ToString());
                    }
                    else if (item.Key.ToString() == "enddate")
                    {
                        _bankrecod.Enddate = DateTime.ParseExact(item.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }

                }
            }
            DataTable dt = _transactionRepository.viewExceptionNor(_bankrecod);

            return new JsonResult(dt);
        }
        //NormaliseRec
    }
}
