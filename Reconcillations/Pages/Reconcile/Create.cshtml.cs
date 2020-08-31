using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using Reconcillations.Entity;
using Reconcillations.Repository;
using Microsoft.Extensions.Hosting;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Reconcillations.Pages.Reconcile
{
    public class CreateModel : PageModel
    {
        ITransactionRepository _transactionRepository;

        private IHostingEnvironment  _hostingEnvironment;

        public SelectList BankSelectList { get; set; }

        [BindProperty]
        public Entity.ReconcilePeriod reconcilePeriod { get; set; }

        public CreateModel(IHostingEnvironment  hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;
        }
        public void OnGet()
        {
        }
        public IActionResult OnGetSelectByCode(string bankcode)
        {
            List<Entity.Account> _accounts = (from g in
                    _transactionRepository.GetAccountslList(bankcode)
                                              select g).ToList();
            return new JsonResult(_accounts);
        }
        public IActionResult OnGetBankAll()
        {
            List<Banklist> data = (from d in _transactionRepository.GeBanklists()
                                   select d).ToList();
            return new JsonResult(data);
        }
        [HttpPost]
        public IActionResult OnPostValidateDate([FromBody] JObject reconciledays)
        {
            Reconcileday _recondays = new Reconcileday();

            ResponseInfo<Reconcileday> responseInfo = new ResponseInfo<Reconcileday>();

            //IFormatProvider culture = new CultureInfo("en-US", true);

            if (!string.IsNullOrWhiteSpace(reconciledays.ToString()))
            {
                foreach (var item in reconciledays)
                {
                    Console.WriteLine(item.Key + " " + item.Value.ToString());

                    if (item.Key.ToString() == "accountID")
                    {
                        _recondays.Accountid = Convert.ToInt64(item.Value.ToString());
                    }
                    else if (item.Key.ToString() == "startdate")
                    {
                        //string DateString = item.Value.ToString();

                        _recondays.Startdate = DateTime.ParseExact(item.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    else if (item.Key.ToString() == "enddate")
                    {
                        //string DateString = item.Value.ToString();

                        _recondays.Enddate = DateTime.ParseExact(item.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                }

                responseInfo = _transactionRepository.Validatereconciledate(_recondays);
            }
            return new JsonResult(responseInfo);
        }
        public IActionResult OnPostRecords([FromBody] JObject reconcile)
        {
            //CreateReconciledate
            ReconcilePeriod _recondays = new ReconcilePeriod();

            ResponseInfo<ReconcilePeriod> responseInfo = new ResponseInfo<ReconcilePeriod>();

            if (!string.IsNullOrWhiteSpace(reconcile.ToString()))
            {
                foreach (var item in reconcile)
                {
                    Console.WriteLine(item.Key + " " + item.Value.ToString());

                    if (item.Key.ToString() == "accountID")
                    {
                        _recondays.AccountID = Convert.ToInt64(item.Value.ToString());
                    }
                    else if (item.Key.ToString() == "startDate")
                    {
                        _recondays.StartDate = DateTime.ParseExact(item.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    else if (item.Key.ToString() == "endDate")
                    {
                        _recondays.EndDate = DateTime.ParseExact(item.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    else if (item.Key.ToString() == "balance")
                    {
                        _recondays.Balance = Convert.ToDecimal(item.Value.ToString());
                    }
                    else if (item.Key.ToString() == "close")
                    {
                        _recondays.ClosingBal = Convert.ToDecimal(item.Value.ToString());
                    }
                    else if (item.Key.ToString() == "period")
                    {
                        _recondays.Period = item.Value.ToString();
                    }
                }

                responseInfo = _transactionRepository.CreateReconciledate(_recondays);
            }
            return new JsonResult(responseInfo);
        }
    }
}
