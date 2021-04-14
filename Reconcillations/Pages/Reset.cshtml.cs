using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Reconcillations.Repository;

namespace Reconcillations.Pages
{
    public class ResetModel : PageModel
    {
        ITransactionRepository _transactionRepository;

        private IHostEnvironment _hostingEnvironment;

        public ResetModel(IHostEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        public void OnGet()
        {
        }

        public IActionResult OnGetSearchUser(string useremail)
        {
            //List<Entity.SearchUser> _accounts = (from g in
            //        _transactionRepository.SearchUserbyEmial(useremail)
            //    select g).ToList();
            DataSet dtresult = _transactionRepository.SearchUserbyEmial(useremail);

            return new JsonResult(dtresult);
        }

        public IActionResult OnGetResetUser(string useremail)
        {
            Int32 kj = _transactionRepository.UpdateUserlog(useremail, 0);
            return new JsonResult(kj);
        }
    }
}
