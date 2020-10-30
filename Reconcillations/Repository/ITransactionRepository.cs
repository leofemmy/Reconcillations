using Reconcillations.Entity;
using System;
using System.Collections.Generic;
using System.Data;

namespace Reconcillations.Repository
{
    public interface ITransactionRepository
    {
        IEnumerable<Entity.Type> GetTypes();

        IEnumerable<Element> GetElements();

        IEnumerable<ActionType> GetActionTypes();

        int AddTransaction(Transaction transaction);

        int AddBankAccount(Bank bank);

        int SaveBankmport(DataTable dts, long RecperID);

        int SaveAllocatetrans(DataTable dts);

        public int EditUsersAccount(UsersList users);

        List<Transaction> GetTransactions();

        int ValidateEmailAddress(fpass fpass);

        public UsersList GetUserid(long userid);

        Transaction GetTransactionID(long id);

        int EditTransaction(Transaction transaction);

        int DisableTransction(int id);

        List<Accountlists> GetAccountlists();

        int DisableUserAccount(long Userid);

        int UpdateUserActivationCode(string email, string code);

        int AddUserAccount(CUsers _users, string stractivationCode);

        int UserConfirmation(string email, string code, string pass, string guidid);

        List<Bank> GetBanklistList();

        List<Usertypelist> GetUsertypelist();

        int ResetUserPassword(string email, string pass, string guidid);

        int DisableBankAccount(long AccountID);

        List<Banklist> GeBanklists();

        List<Postinglist> GetRecocileperiod();

        List<ModifyRecords> GetModifyRecords();

        ResponseInfo DeleteReconcileperiod(long reconcileId);

        ResponseInfo ApproveReclassified(long modifyid, string usermail);

        List<Currency> GetCurrencies();

        List<Account> GetAccountslList(string bankcode);

        List<Definitionlist> GetTransactionDefinitionlist();

        List<UsersList> GetUserAccountlist();

        List<RequestApprove> GetRequestApprove();

        List<AccountBal> GetAccountBals(double Accountid);

        List<Reconcilelist> Getrecocilelists();

        AccountBal GetAccountBalance(long Accountid);

        List<Agency> GetAgencylist();

        ResponseMessage<BankImport> Savebankstatement(BankImport bankImport, System.Data.DataTable dts);

        List<TransactionList> GeTransactionLists();

        List<Normalisereclists> GetNormalisereclists();

        List<Postinglist> GetAllPostingRequesting();

        DataSet FinishedReconcile(long reconcileId, string UsersId);

        DataSet CompareBankStatement(long reconcileId);

        DataSet GetUserLogin(string Emailaddress);

        DataSet SaveCompareStatement(long reconcileId, double closebal, DataTable dtbank, DataTable dtColl, DataTable dtMatc);

        DataSet Reclassified(long reconcileId);

        DataSet AllocateTransaction(long reconcileId, string UsersId);

        DataSet AllocateModify(string userid, DataTable dbvalues);

        DataSet SearchReconcilizedRecord(string referencenumber);

        DataSet PostingRequestSent(long reconcileId, string UsersId);

        DataSet CategoryCheck(DataTable dbvalues);

        DataSet DisapproveNormalise(string userid, string modifyid, string strreasons);

        DataSet SaveNormaliseRecord(string usermail, string paymentref, string payername, string agencyname,
            string agencycode, string revenuename, string revenusecode);

        DataTable viewException(Entity.Exceptions exception);

        DataTable ViewPostingRequest(long reconcileId); DataSet Disapprovemodify(string userid, long modifyid, string strreasons);

        DataTable ViewAgencies(Summarys sumagency); DataTable ViewMonth(Summarys sumagency);

        DataTable ViewAgencydetails(string strAgency, DateTime dtstart, DateTime dtenddate);

        DataTable Viewdetails(string strrevenue, DateTime dtstart, DateTime dtenddate);

        DataTable ViewVaricesAgencies(Summarys sumagency);

        DataTable ViewBankAllocation(long reconcileId);

        DataTable Viewunpostedcollection(long reconcileId);

        DataTable ViewSummary(Summarys marsy);

        DataTable ViewBanksCollection(Summarys sumbanks);

        Bank GetBankAccountId(long accountid);

        int EditBankAccunt(Bank bank);

        string Insertusertoken(string emailaddress);

        string validateuserotp(string emailaddress, string strtoken);

        List<BankTypecode> GetBankTypecodes();

        string posttransactionapproval(string emailaddress, double reconcileId);

        List<Postinglist> GetReconciledClosed();

        Entity.ResponseInfo<Reconcileday> Validatereconciledate(Reconcileday reconcliedays);

        ResponseInfo<ReconcilePeriod> CreateReconciledate(ReconcilePeriod reconcilePeriod);

        Reconcileperiodid GetReconcileid(long id);

        ResponseInfo RequestComplet(long reconcileId, string usermail);

        ResponseInfo Deletereconciletransactionlist(long reconcileId);

        ResponseInfo ApproveNormaliseRecord(string Refnumb, string usermail);

        List<Requesntsent> GetRequestsentlist();

        List<Revenuelist> GetRevenuelist(long agancyid);
    }
}