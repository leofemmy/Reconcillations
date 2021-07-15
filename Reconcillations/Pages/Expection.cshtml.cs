using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Hosting;
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

namespace Reconcillations.Pages
{
    public class ExpectionModel : PageModel
    {
        ITransactionRepository _transactionRepository;

        private IHostingEnvironment _hostingEnvironment;

        //private IHostingEnvironment  _hostingEnvironment;

        public XtraReport Report { get; set; }

        public SelectList AccountSelectlist { get; set; }

        public SelectList PeriodSelectList { get; set; }

        public SelectList definitionlist { get; set; }

        public SelectList peryear { get; set; }

        [BindProperty]
        public Entity.Exceptions bankimport { get; set; }

        [BindProperty]
        public Accountlists actlist { get; set; }

        [BindProperty]
        public Definitionlist _definitionlist { get; set; }

        [BindProperty]
        public Periodlist Periodlist { get; set; }

        [BindProperty]
        public PeriodYear PeriodYear { get; set; }

        public ExpectionModel(IHostingEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult OnGetDefinition()
        {
            List<Definitionlist> data = (from d in _transactionRepository.GetTransactionDefinitionlist()
                                         select d).ToList();
            return new JsonResult(data);
        }

        public void OnGet()
        {
            //GetPeriodYear

            var dat = (from d in _transactionRepository.GetAccountlists()
                       select d).ToList();
            AccountSelectlist = new SelectList(dat, "AccountID", "AccountName");
            HttpContext.Session.Set("actlist", dat);

            var per = (from e in _transactionRepository.GetPeriodlist() select e).ToList();
            PeriodSelectList = new SelectList(per, "PeriodMonth", "PeriodName");
            HttpContext.Session.Set("Periodlist", per);

            var datas = (from d in _transactionRepository.GetTransactionDefinitionlist()
                         select d).ToList();
            definitionlist = new SelectList(datas, "TransID", "Description");

            HttpContext.Session.Set("_definitionlist", datas);

            var yer = (from y in _transactionRepository.GetPeriodYear() select y).ToList();
            peryear = new SelectList(yer, "PeridYear", "PeridYear");
            HttpContext.Session.Set("PeriodYear", yer);
        }

        public void OnPost()
        {
            var _bankrecod = bankimport;

            if (ModelState.IsValid)
            {
                DataTable dt = _transactionRepository.viewException(_bankrecod);

                Report = createreport(dt);
            }

        }

        public void OnPostException([FromBody] JObject objexception)
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
                    //else if (item.Key.ToString() == "startdate")
                    //{
                    //    _bankrecod.Startdate = DateTime.ParseExact(item.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //}
                    else if (item.Key.ToString() == "transID")
                    {
                        _bankrecod.TransID = Convert.ToInt64(item.Value.ToString());
                    }
                    //else if (item.Key.ToString() == "enddate")
                    //{
                    //    _bankrecod.Enddate = DateTime.ParseExact(item.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //}

                }
            }
            DataTable dt = _transactionRepository.viewException(_bankrecod);

            Report = createreport(dt);
        }

        XtraReport createreport(DataTable dts)
        {
            XtraRepExpection report = new XtraRepExpection();
            report.DataSource = dts;
            report.DataMember = "vwTransactionDefiniations";
            report.CreateDocument();
            return report;
        }

        public IActionResult OnGetAccountlist()
        {
            List<Accountlists> data = (from d in _transactionRepository.GetAccountlists()
                                       select d).ToList();
            return new JsonResult(data);
        }
    }
}
