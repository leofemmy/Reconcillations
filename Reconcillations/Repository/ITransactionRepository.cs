﻿using Reconcillations.Entity;
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

        DataSet pushrecordtoogun(DataSet ds);
        int AddBankAccount(Bank bank);

        int SaveBankmport(DataTable dts, long RecperID);

        Int32 UpdateUserlog(string usermailaddress, Int32 blFlag);

        int SaveAllocatetrans(DataTable dts);

        int DoInsertPushtoReemsRequest(PushExceptRequest piExceptRequest);

        int DoExecuatePushRequesttoReems(PushToReemsExceution reemsExceution);

        public int EditUsersAccount(UsersList users);

        List<Transaction> GetTransactions();

        DataSet SearchUserbyEmial(string useremail);

        string PushedRecordtoReems(string usermail);

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

        List<Periodlist> GetPeriodlist();

        List<AccountBal> GetAccountBals(double Accountid);

        List<Reconcilelist> Getrecocilelists();

        List<RequestPushReemList> GetRequestPushReemLists();

        AccountBal GetAccountBalance(long Accountid);

        List<Agency> GetAgencylist();

        List<PeriodYear> GetPeriodYear();

        ResponseMessage<BankImport> Savebankstatement(BankImport bankImport, System.Data.DataTable dts);

        List<TransactionList> GeTransactionLists();

        List<Normalisereclists> GetNormalisereclists();

        List<Postinglist> GetAllPostingRequesting();

        List<PushException> GetPushExceptions();

        DataSet FinishedReconcile(long reconcileId, string UsersId);

        DataSet CompareBankStatement(long reconcileId);

        DataSet GetUserLogin(string Emailaddress);

        DataSet CheckUserid(string usermailaddress);

        DataSet SaveCompareStatement(long reconcileId, double closebal, DataTable dtbank, DataTable dtColl, DataTable dtMatc, string EmailAddress);

        DataSet Reclassified(long reconcileId);

        DataSet AllocateTransaction(long reconcileId, string UsersId);

        DataSet AllocateModify(string userid, DataTable dbvalues);

        DataSet SearchReconcilizedRecord(string referencenumber);

        DataSet PostingRequestSent(long reconcileId, string UsersId);

        DataSet CategoryCheck(DataTable dbvalues);

        DataSet DisapproveNormalise(string userid, string modifyid, string strreasons);

        DataSet SaveNormaliseRecord(string usermail, string paymentref, string payername, string agencyname,
            string agencycode, string revenuename, string revenusecode, string address);

        DataTable viewException(Entity.Exceptions exception);

        DataTable viewExceptionNor(Entity.Exceptions exception);

        DataTable ViewPostingRequest(long reconcileId); DataSet Disapprovemodify(string userid, long modifyid, string strreasons);

        DataTable ViewAgencies(Summarys sumagency); DataTable ViewMonth(Summarys sumagency);

        DataTable ViewAgencydetails(string strAgency, long lngPeriodMonth, long lngPeriodYear);

        DataTable Viewdetails(string strrevenue, long lngPeriodMonth, long lngPeriodYear);

        DataTable viewBankdetails(string accountnumber, long lngPeriodMonth, long lngPeriodYear);

        DataTable ViewVaricesAgencies(Summarys sumagency);

        DataTable ViewBankAllocation(long reconcileId);

        DataTable Viewunpostedcollection(long reconcileId);

        DataTable ViewSummary(Summarys marsy);

        DataTable ViewBanksCollection(Summarys sumbanks);

        DataTable viewReversedTransaction(string accountnumber, long lngPeriodMonth, long lngPeriodYear);

        string posttransactiondisapprove(string emailaddress, double reconcileId);

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
        List<ReemsRec> GetReems();
    }
}