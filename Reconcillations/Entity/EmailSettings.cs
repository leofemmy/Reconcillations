using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Reconcillations.Entity
{
    public class EmailSettings
    {
        public string MailServer { get; set; }
        public int MailPort { get; set; }
        public string SenderName { get; set; }
        public string Sender { get; set; }
        public string Password { get; set; }
    }

    public class ReconcilePeriod
    {
        [StringLength(60, MinimumLength = 3), Display(Name = "Period Name")]
        [Required]
        public string Period { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(1, 100)]
        [DataType(DataType.Currency), Display(Name = "Opening Balance ")]
        public decimal Balance { get; set; }

        [Required]
        [Display(Name = "Account Name")]
        public long AccountID { get; set; }

        [Required]
        [Display(Name = "Bank  Name")]
        public string BankCode { get; set; }

        public decimal ClosingBal { get; set; }
    }

    public class NormaliseRec
    {
        public string useremail
        {
            get; set;
        }

        public string payername
        {
            get; set;
        }

        public string paymentref
        {
            get; set;
        }

        public string agencyname
        {
            get; set;
        }

        public string agencycode
        {
            get; set;
        }

        public string revenuename
        {
            get; set;
        }

        public string revenuecode
        {
            get; set;
        }
        public string Address
        {
            get; set;
        }
    }

    public class BankImport
    {
        public string AccountID { get; set; }
        public decimal OpeningBal { get; set; }
        public decimal ClosingBal { get; set; }
        public decimal CalClosingBal { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DateEnd { get; set; }
        public long RecperID { get; set; }
    }

    public class NorRec
    {
        public string RefNo
        {
            get; set;
        }

        public string Payerid
        {
            get; set;
        }

        [Required(ErrorMessage = "Email is required")]
        public string PayerName
        {
            get; set;
        }

        public string Address
        {
            get; set;
        }

        public double Amount
        {
            get; set;
        }

        public string SlipNo
        {
            get; set;
        }

        public DateTime PaymentDate
        {
            get; set;
        }

        public Int32 AgencyID
        {
            get; set;
        }

        public string RevenueCode
        {
            get; set;
        }
    }

    public class Pass
    {
        [Required(ErrorMessage = "Email is required")]
        //[StringLength(16, ErrorMessage = "Must be between 5 and 1000 characters", MinimumLength = 5)]
        [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Must be a valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Userpass { get; set; }
    }

    public class BankTypecode
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class Banklist
    {
        public string BankCode { get; set; }
        public string BankName { get; set; }
    }
    //public class BankImport
    //{
    //    public string AccountID { get; set; }
    //    public decimal OpeningBal { get; set; }
    //    public decimal ClosingBal { get; set; }
    //    public DateTime StartDate { get; set; }
    //    public DateTime EndDate { get; set; }
    //}

    public class fpass
    {

        [Required(ErrorMessage = "Email is required")]
        //[StringLength(16, ErrorMessage = "Must be between 5 and 1000 characters", MinimumLength = 5)]
        [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Must be a valid email")]
        public string Email { get; set; }
    }

    public class Changepasss
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Must be a valid email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string currentPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }

    public class Cuserpass
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Must be a valid email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class TransactionList
    {
        //[Key]
        //[Display(Name = "Account Id")]
        public Int64 RecperID
        {
            get; set;
        }

        //[Required]
        [Display(Name = "Account")]
        [StringLength(500, ErrorMessage = "Name should be 1 to 500 char in length")]
        public string AccountName
        {
            get; set;
        }

        [Display(Name = "Bank")]
        [StringLength(500, ErrorMessage = "Name should be 1 to 500 char in length")]
        public string BankName
        {
            get; set;
        }

        public DateTime StartDate
        {
            get; set;
        }

        public DateTime EndDate
        {
            get; set;
        }

        public Int64 AccountID
        {
            get; set;
        }

        public Int64 StagesID
        {
            get; set;
        }

        public double OpeningBal
        {
            get; set;
        }

        public double ClosingBal
        {
            get; set;
        }

        public bool IsClosed
        {
            get; set;
        }

        public string Symbol
        {
            get; set;
        }

        public string Stages
        {
            get; set;
        }

        public string Currency
        {
            get; set;
        }

        public string Period
        {
            get; set;
        }

        public long Year
        {
            get; set;
        }

    }

    public class SearchUser
    {
        public string UserName { get; set; }

        public string Email { get; set; }
    }

    public class Account
    {
        public double AccountID { get; set; }

        public string AccountName { get; set; }
    }

    public class Revenuelist
    {
        public string RevenueCode { get; set; }

        public string RevenueName { get; set; }
    }

    public class Reconcileperiodid
    {
        public decimal OpeningBal { get; set; }

        public DateTime startdate { get; set; }

        public DateTime enddate { get; set; }

        public string accountname { get; set; }

        public decimal ClosingBal { get; set; }

        public decimal AccountID { get; set; }
    }

    public class AccountBal
    {
        public double OpenBal { get; set; }

        public DateTime Startdate { get; set; }
    }

    public class Summarys
    {
        //[Required(ErrorMessage = "Please choose Start date.")]
        //[Display(Name = "Start Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[CustomAdmissionDate(ErrorMessage = "Admission Date must be less than or equal to Today's Date.")]
        public DateTime Startdate { get; set; }

        //[Required(ErrorMessage = "Please choose end date.")]
        //[Display(Name = "End Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[CustomAdmissionDate(ErrorMessage = "Admission Date must be less than or equal to Today's Date.")]
        public DateTime Enddate { get; set; }

        [Required(ErrorMessage = "Select Period")]
        public long periodMonth
        {
            get; set;
        }

        [Required(ErrorMessage = "Select Year")]
        public long periodYear
        {
            get; set;
        }

    }

    public class ReemsRec
    {
        [Key]
        public string PaymentRefNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public string payername { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string RevenueName { get; set; }
        public string AgencyName { get; set; }
        public string BankName { get; set; }
        public bool PushToReemsSuccessful { get; set; }
        public DateTime PushToReemsOn { get; set; }
        public string PushToReemsBy { get; set; }
    }
    public class Bank
    {
        [Key]
        [Display(Name = "Account Id")]
        public Int64 AccountID { get; set; }

        [Required]
        [Display(Name = "Account Name")]
        [StringLength(500, ErrorMessage = "Name should be 1 to 500 char in length")]
        public string AccountName { get; set; }

        [Required]
        [Display(Name = "Account Number")]
        [StringLength(12, ErrorMessage = "Number should be 1 to 12 char in length")]
        //[DataType(DataType.Currency)]
        public string AccountNumber { get; set; }

        //[Required]
        ////[Range(1, 100)]
        ////[DataType(DataType.Currency)]
        ////[Column(TypeName = "decimal(18, 2)")]
        //[StringLength(12, ErrorMessage = "Opening Balance should be Number")]
        //public decimal OpeningBal { get; set; }


        [Display(Name = "Account Link")]
        public bool IsLink { get; set; }
        [Required, Display(Name = "Bank Type")]
        public string code { get; set; }

        //[Display(Name = "Is Bank Commerical")]
        //public bool IsCommerical { get; set; }


        [Display(Name = "Is Acount Active")]
        public bool IsActive { get; set; }

        //[Required]
        //[Display(Name = "Bank Name")]
        [Required]
        [Display(Name = "Bank Name")]
        public string BankCode { get; set; }

        [Required]
        [Display(Name = "Currency Type")]
        public int? CurrencyID { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:n2", ApplyFormatInEditMode = true)]
        //[Column(TypeName = "decimal(18, 2)")]
        public decimal OpeningBal { get; set; }

        public DateTime CreatedDate { get; set; }
        public string BankName { get; set; }
        public string Description { get; set; }
    }

    public class Posting
    {
        public long reconileID { get; set; }
        public string Userid { get; set; }
    }

    public class PushExceptRequest
    {
        public string Userid
        {
            get; set;
        }

        public string PaymentRefNumber
        {
            get; set;
        }

        public string RequestCommet
        {
            get; set;
        }
    }

    public class ExcelEment
    {
        public DateTime Date
        {
            get; set;
        }

        public decimal Debit
        {
            get; set;
        }

        public decimal Credit
        {
            get; set;
        }

        public decimal Balance
        {
            get; set;
        }

        public string Tellerno
        {
            get; set;
        }

        public string Revenuecode
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public decimal TransID
        {
            get; set;
        }

    }

    public class Reconcileday
    {
        public long Accountid { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
    }

    public class Reconcilelist
    {
        public long RecperID { get; set; }
        public string BankName { get; set; }
        public string Description { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Stages { get; set; }
        public int days { get; set; }
    }

    public class Normalisereclists
    {
        public string PaymentRefNumber { get; set; }
        public string payername { get; set; }
        public decimal Amount { get; set; }
        public string AgencyName { get; set; }
        public string AgencyCode { get; set; }
        public string RevenueName { get; set; }
        public string RevenueCode { get; set; }
        public string NewPayerName { get; set; }
        public string NormalisedBy { get; set; }
        public DateTime NormaliseDate { get; set; }
    }

    public class RequestPushReemList
    {
        public DateTime PaymentDate
        {
            get;
            set;
        }

        public string PaymentRefNumber
        {
            get;
            set;
        }
        public string payername
        {
            get;
            set;
        }
        public decimal Amount
        {
            get;
            set;
        }
        public string RevenueName
        {
            get;
            set;
        }
        public string PushToReemsRequestBy
        {
            get;
            set;
        }
        public DateTime PushToReemsRequestOn
        {
            get;
            set;
        }
    }

    public class ModifyRecords
    {
        public long ModifyID { get; set; }

        public DateTime BSDate { get; set; }
        public decimal Amount { get; set; }
        public string oldDescription
        {
            get;
            set;
        }
        public string NewDescription { get; set; }
        public string ModifyBy { get; set; }
        public DateTime ModifyDate { get; set; }
    }

    public class PushToReemsExceution
    {
        public string PaymentRefNumber
        {
            get; set;
        }

        public string Comment
        {
            get; set;
        }

        public string Userid
        {
            get; set;
        }

        public int Execution
        {
            get; set;
        }
    }

    public class PushException
    {
        public string PaymentRefNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public string payername { get; set; }
        public decimal Amount { get; set; }
        public string RevenueName { get; set; }
    }

    public class RequestApprove
    {
        public long RecperID
        {
            get; set;
        }

        public string BankName
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public string AccountName
        {
            get; set;
        }

        public string Period
        {
            get;
            set;
        }

        public long Year
        {
            get;
            set;
        }
        public DateTime StartDate
        {
            get; set;
        }

        public DateTime EndDate
        {
            get; set;
        }

        public string Stages
        {
            get; set;
        }

        public int days
        {
            get; set;
        }

        public decimal openingbal
        {
            get; set;
        }

        public decimal closebal
        {
            get; set;
        }

        public decimal credit
        {
            get; set;
        }

        public decimal debit
        {
            get; set;
        }
    }

    public class Requesntsent
    {
        public long RecperID { get; set; }
        public string BankName { get; set; }
        public string Description { get; set; }
        public string AccountName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Stages { get; set; }
        public int days { get; set; }
        public decimal openingbal { get; set; }
        public decimal closebal { get; set; }
        public decimal credit { get; set; }
        public decimal debit { get; set; }
        public string Requestby { get; set; }
    }

    public class ResponseInfo
    {
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }

    public class ResponseInfo<TObject> where TObject : class
    {
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public TObject ResponseObject { get; set; }
    }

    public class ResponseMessage<T>
    {
        public int StatusId { get; set; }
        public long Reconcileid { get; set; }
        public string StatusMessage { get; set; }
        public T RecordResponseObject { get; set; }
        public bool IsSuccessful => StatusId == 1;
    }

    public class ReconcileEntity
    {
        public long reconileID { get; set; }
        public double closebal { get; set; }
    }

    public class Categorys
    {
        public DataTable dbvalue { get; set; }
    }

    public class Normalise
    {
        public string RefNumb { get; set; }

        public string reasons { get; set; }
    }

    public class Disapprove
    {
        public long modifyid { get; set; }

        public string reasons { get; set; }
    }

    public class AllocateModify
    {
        public string Userid { get; set; }
        public DataTable dbvalue { get; set; }
    }

    public class Allocate
    {
        public long reconileID { get; set; }
        public string Userid { get; set; }
        public DataTable dtDebit { get; set; }
        public DataTable dtCredit { get; set; }
    }

    public class Currency
    {
        public int CurrencyID { get; set; }
        public string CurrencyName { get; set; }
    }

    public class Transaction
    {
        [Key]
        [Display(Name = "Transaction Id")]
        public Int64 TransID { get; set; }

        [Required]
        [Display(Name = "Transaction Definition")]
        [StringLength(500, ErrorMessage = "Name should be 1 to 500 char in lenght")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Transaction Definition Type")]
        public string TypeCode { get; set; }
        //public IEnumerable<SelectListItem> TypeSelectList { get; set; }

        public DateTime CreatedDate { get; set; }
        public string TypeName { get; set; }
        public string ElementName { get; set; }
        public string ListName { get; set; }

        public Boolean Active { get; set; }
        //[Required]
        //[Display(Name = "ACtive Type")]
        //public Int64 ListId { get; set; }

        [Required]
        [Display(Name = "Element Category")]
        public Int64 ElementCategoryID { get; set; }
    }

    public class Currencys
    {
        public Int64 CurrencyID { get; set; }
        public string CurrencyName { get; set; }
    }

    public class Classtypes
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class BankType
    {
        public string BankCode { get; set; }
        public string BankName { get; set; }
    }

    public class ActionType
    {
        public int Id { set; get; }
        public string Name { set; get; }
    }

    public class Type
    {
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
    }

    public class Element
    {
        public int? ElementCategoryID { get; set; }

        public string Name { get; set; }
    }

    public class CUsers
    {
        [Required]
        [StringLength(2000)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(2000)]
        public string Surname { get; set; }

        [Required]
        [StringLength(11)]
        public string Mobile { get; set; }

        [Required]
        [EmailAddress]
        [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Must be a valid email")]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }
        public string Usercode { get; set; }

    }

    public class Exceptions
    {
        [Required(ErrorMessage = "Transaction Type")]
        public long TransID
        {
            get; set;
        }

        [Required(ErrorMessage = "Account Name")]
        public long AccountID
        {
            get; set;
        }

        [Required(ErrorMessage = "Select Period")]
        public long periodMonth
        {
            get; set;
        }

        [Required(ErrorMessage = "Select Year")]
        public long periodYear
        {
            get; set;
        }

        //[Required(ErrorMessage = "Please choose Start date.")]
        //[Display(Name = "Start Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]

        public DateTime Startdate
        {
            get; set;
        }

        //[Required(ErrorMessage = "Please choose end date.")]
        //[Display(Name = "End Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Enddate
        {
            get; set;
        }

    }

    public class Accountlists
    {
        public long AccountID
        {
            get; set;
        }

        public string AccountName
        {
            get; set;
        }
    }

    public class Periodlist
    {
        public long PeriodMonth
        {
            get; set;
        }

        public string PeriodName
        {
            get; set;
        }
    }

    public class Definitionlist
    {
        public long TransID
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }
    }

    public class PeriodYear
    {
        public Int64 PeridYear
        {
            get; set;
        }
    }

    public class Postinglist
    {
        public long RecperID { get; set; }
        public string Description { get; set; }
    }

    public class Agency
    {
        public Int64 AgencyId { get; set; }
        public string AgencyName { get; set; }
    }

    public class element
    {
        public Int64 ElementCategoryID { get; set; }
        public string Name { get; set; }
    }

    public class typeuser
    {
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
    }

    public class Usertypelist
    {
        public string TypeCode { get; set; }
        public string Description { get; set; }
    }

    public class UsersList
    {
        public long UserId { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Mobile { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public Boolean IsActive { get; set; }
        public string Usercode { get; set; }

    }
}
