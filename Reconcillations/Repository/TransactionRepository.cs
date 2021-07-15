using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Digests;
using Reconcillations.Entity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using NPOI.SS.Formula.Functions;

namespace Reconcillations.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        IConfiguration _configuration;

        public TransactionRepository(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public string GetConnection()
        {
            var connection = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            return connection;
        }

        public IEnumerable<Element> GetElements()
        {
            var connectionString = this.GetConnection();
            List<Element> elements = new List<Element>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("SpIRecgetElementCategory", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Element _element = new Element
                        {
                            ElementCategoryID = Convert.ToInt32(rdr["ElementCategoryID"].ToString()),
                            Name = rdr["Name"].ToString()
                        }
                            ;

                        elements.Add(_element);
                    }
                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Element lists  ");
                    loggers.Information(JsonConvert.SerializeObject(elements));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Element lists thrown an error - {e.Message}");
            }

            return elements;
        }

        public List<Transaction> GetTransactions()
        {
            var connectionString = this.GetConnection();

            List<Transaction> _transaction = new List<Transaction>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("sptransactiondefinitionlist", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Transaction trans = new Transaction
                        {
                            Description = rdr["Description"].ToString(),
                            TransID = Convert.ToInt64(rdr["TransID"].ToString()),
                            ElementCategoryID = Convert.ToInt32(rdr["ElementCategoryID"].ToString()),
                            //ListId = Convert.ToInt64(rdr["ListId"].ToString()),
                            TypeCode = rdr["TypeCode"].ToString(),
                            CreatedDate = Convert.ToDateTime(rdr["CreatedDate"].ToString()),
                            TypeName = rdr["TypeName"].ToString(),
                            ElementName = rdr["Name"].ToString(),
                            ListName = rdr["ListName"].ToString(),
                            Active = Convert.ToBoolean(rdr["IsDisable"].ToString())


                        };
                        _transaction.Add(trans);
                    }
                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Transaction lists  ");
                    loggers.Information(JsonConvert.SerializeObject(_transaction));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Transaction lists thrown an error - {e.Message}");

            }

            return _transaction;
        }

        public IEnumerable<ActionType> GetActionTypes()
        {
            var connectionString = this.GetConnection();
            List<ActionType> actpes = new List<ActionType>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spGetActionList", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        ActionType _actionType = new ActionType
                        {
                            Id = Convert.ToInt32(rdr["ListId"].ToString()),
                            Name = rdr["ListName"].ToString()
                        }
                            ;

                        actpes.Add(_actionType);
                    }
                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Action Type lists  ");
                    loggers.Information(JsonConvert.SerializeObject(actpes));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Action Type lists thrown an error - {e.Message}"); ;
            }

            return actpes;
        }

        public IEnumerable<Entity.Type> GetTypes()
        {
            var connectionString = this.GetConnection();
            List<Entity.Type> elements = new List<Entity.Type>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spTypeGetCode", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Entity.Type _element = new Entity.Type
                        {
                            TypeCode = rdr["TypeCode"].ToString(),
                            TypeName = rdr["TypeName"].ToString()
                        }
                            ;

                        elements.Add(_element);
                    }
                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Transaction Type lists  ");
                    loggers.Information(JsonConvert.SerializeObject(elements));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get TRansaction Types lists thrown an error - {e.Message}");
            }

            return elements;
        }

        public int DoExecuatePushRequesttoReems(PushToReemsExceution reemsExceution)
        {
            var connectionString = this.GetConnection();

            int count = 0;

            SqlDataAdapter _adp;

            System.Data.DataSet response = new System.Data.DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spDoUpdatePushRequestReems", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@userid ", reemsExceution.Userid);
                    cmd.Parameters.AddWithValue("@PaymentRefNumber", reemsExceution.PaymentRefNumber);
                    cmd.Parameters.AddWithValue("@RequestComment", reemsExceution.Comment);
                    cmd.Parameters.AddWithValue("@bit", reemsExceution.Execution);

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {
                        count = -1;
                    }
                    else
                    {
                        count = 1;
                    }
                    //cmd.ExecuteNonQuery();
                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Add Push To Request ");
                    loggers.Information(JsonConvert.SerializeObject(count));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Add Push to reems request thrown an error - {e.Message}");
            }
            return count;
        }

        public int DoInsertPushtoReemsRequest(PushExceptRequest piExceptRequest)
        {
            var connectionString = this.GetConnection();
            int count = 0;

            SqlDataAdapter _adp;
            System.Data.DataSet response = new System.Data.DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spDoInsertPushtoReemsRequest", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@userid", piExceptRequest.Userid);
                    cmd.Parameters.AddWithValue("@PaymentRefNumber", piExceptRequest.PaymentRefNumber);
                    cmd.Parameters.AddWithValue("@RequestComment", piExceptRequest.RequestCommet);

                    //cmd.Parameters.AddWithValue("@IsActive", bank.IsActive);

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {
                        count = -1;
                    }
                    else
                    {
                        count = 1;
                    }
                    //cmd.ExecuteNonQuery();
                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Add Push To Request ");
                    loggers.Information(JsonConvert.SerializeObject(count));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Add Push to reems request thrown an error - {e.Message}");
            }
            return count;
        }

        public int AddBankAccount(Bank bank)
        {
            var connectionString = this.GetConnection();
            int count = 0;

            SqlDataAdapter _adp;
            System.Data.DataSet response = new System.Data.DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spInsertBankAccount", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@AccountName", bank.AccountName);
                    cmd.Parameters.AddWithValue("@AccountNumber", bank.AccountNumber);
                    cmd.Parameters.AddWithValue("@OpeningBal", Convert.ToDecimal(bank.OpeningBal));
                    cmd.Parameters.AddWithValue("@BankCode", bank.BankCode);
                    cmd.Parameters.AddWithValue("@CurrencyID", bank.CurrencyID);
                    cmd.Parameters.AddWithValue("@IsLink", bank.IsLink);
                    cmd.Parameters.AddWithValue("@banktypecode", bank.code);
                    //cmd.Parameters.AddWithValue("@IsActive", bank.IsActive);

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {
                        count = -1;
                    }
                    else
                    {
                        count = 1;
                    }
                    //cmd.ExecuteNonQuery();
                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Add Bank Account  ");
                    loggers.Information(JsonConvert.SerializeObject(count));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Add Bank Account thrown an error - {e.Message}");
            }
            return count;
        }

        public int ResetUserPassword(string email, string pass, string guidid)
        {
            var connectionString = this.GetConnection(); int count = 0;

            SqlDataAdapter _adp;

            System.Data.DataSet response = new System.Data.DataSet();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spRestUserPasword", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@EmailAddress", email);
                    cmd.Parameters.AddWithValue("@Password", pass);
                    cmd.Parameters.AddWithValue("@UserGuid", guidid);

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {
                        count = -1;
                    }
                    else
                    {
                        count = 1;
                    }
                    //cmd.ExecuteNonQuery();
                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"User Password reset ");
                    loggers.Information(JsonConvert.SerializeObject(count));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
      .WriteTo.MSSqlServer(connectionString, "Logs")
      .CreateLogger();
                logger.Fatal($"User Password reset thrown an error - {e.Message}");
            }
            return count;
        }

        public int UserConfirmation(string email, string code, string pass, string guidid)
        {
            var connectionString = this.GetConnection(); int count = 0;

            SqlDataAdapter _adp;

            System.Data.DataSet response = new System.Data.DataSet();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spUserAccountPassword", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@EmailAddress", email);
                    cmd.Parameters.AddWithValue("@ActivationCode", code);
                    cmd.Parameters.AddWithValue("@Password", pass);
                    cmd.Parameters.AddWithValue("@UserGuid", guidid);

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {
                        count = -1;
                    }
                    else
                    {
                        count = 1;
                    }
                    //cmd.ExecuteNonQuery();
                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Add Users Account  ");
                    loggers.Information(JsonConvert.SerializeObject(count));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
      .WriteTo.MSSqlServer(connectionString, "Logs")
      .CreateLogger();
                logger.Fatal($"Add User confirmation thrown an error - {e.Message}");
            }
            return count;
        }

        public int UpdateUserActivationCode(string email, string code)
        {
            var connectionString = this.GetConnection();

            int count = 0;

            SqlDataAdapter _adp;
            System.Data.DataSet response = new System.Data.DataSet();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spUpdateUserActioncode", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@useemail", email);

                    cmd.Parameters.AddWithValue("@activationcode", code);

                    //@ActivationCode

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {
                        count = -1;
                    }
                    else
                    {
                        count = 1;
                    }
                    //cmd.ExecuteNonQuery();
                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"User Activation code ");
                    loggers.Information(JsonConvert.SerializeObject(count));
                }
            }
            catch (Exception e)
            {

                var logger = new LoggerConfiguration()
                   .WriteTo.MSSqlServer(connectionString, "Logs")
                   .CreateLogger();
                logger.Fatal($"User Activation thrown an error - {e.Message}");
            }
            return count;
        }

        public int ValidateEmailAddress(fpass fpass)
        {
            var connectionString = this.GetConnection();

            int count = 0;

            SqlDataAdapter _adp;
            System.Data.DataSet response = new System.Data.DataSet();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spValidateEmailaddress", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@EmailAddress", fpass.Email);

                    //@ActivationCode

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {
                        count = -1;
                    }
                    else
                    {
                        count = 1;
                    }
                    //cmd.ExecuteNonQuery();
                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Add Users Account  ");
                    loggers.Information(JsonConvert.SerializeObject(count));
                }
            }
            catch (Exception e)
            {

                var logger = new LoggerConfiguration()
                   .WriteTo.MSSqlServer(connectionString, "Logs")
                   .CreateLogger();
                logger.Fatal($"Add User Account thrown an error - {e.Message}");
            }
            return count;
        }

        public int AddUserAccount(CUsers _users, string stractivationCode)
        {
            var connectionString = this.GetConnection();
            int count = 0;

            SqlDataAdapter _adp;
            System.Data.DataSet response = new System.Data.DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spInsertUserAccount", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Surname", _users.Surname);
                    cmd.Parameters.AddWithValue("@Firstname", _users.FirstName);
                    cmd.Parameters.AddWithValue("@Mobile", _users.Mobile);
                    cmd.Parameters.AddWithValue("@TypeCode", _users.Usercode);
                    cmd.Parameters.AddWithValue("@EmailAddress", _users.Email);
                    cmd.Parameters.AddWithValue("@Address", _users.Address);
                    cmd.Parameters.AddWithValue("@ActivationCode", stractivationCode);
                    //@ActivationCode

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {
                        count = -1;
                    }
                    else
                    {
                        count = 1;
                    }
                    //cmd.ExecuteNonQuery();
                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Add Users Account  ");
                    loggers.Information(JsonConvert.SerializeObject(count));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Add User Account thrown an error - {e.Message}");
            }
            return count;
        }

        public int AddTransaction(Transaction transaction)
        {
            var connectionString = this.GetConnection();
            int count = 0;

            SqlDataAdapter _adp;
            System.Data.DataSet response = new System.Data.DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spInsertTransactionDefinition", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Description", transaction.Description);
                    cmd.Parameters.AddWithValue("@ElementCategoryID", transaction.ElementCategoryID);
                    cmd.Parameters.AddWithValue("@TypeCode", transaction.TypeCode);
                    //cmd.Parameters.AddWithValue("@ListId", transaction.ListId);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {
                        count = -1;
                    }
                    else
                    {
                        count = 1;
                    }
                    //cmd.ExecuteNonQuery();
                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Add Transaction  ");
                    loggers.Information(JsonConvert.SerializeObject(count));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Add Transaction thrown an error - {e.Message}");
            }
            return count;
        }

        public int EditTransaction(Transaction transaction)
        {
            var connectionString = this.GetConnection();

            int count = 0;

            SqlDataAdapter _adp;

            System.Data.DataSet response = new System.Data.DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spTransactionDefinitionUpdate", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TransID", transaction.TransID);
                    cmd.Parameters.AddWithValue("@Description", transaction.Description);
                    cmd.Parameters.AddWithValue("@ElementCategoryID", transaction.ElementCategoryID);
                    cmd.Parameters.AddWithValue("@TypeCode", transaction.TypeCode);
                    //cmd.Parameters.AddWithValue("@ListId", transaction.ListId);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {
                        count = -1;
                    }
                    else
                    {
                        count = 1;
                    }
                    //cmd.ExecuteNonQuery();
                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Edit Transaction with id- {transaction.TransID} ");
                    loggers.Information(JsonConvert.SerializeObject(count));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Edit Transaction thrown an error - {e.Message}");
            }
            return count;
        }

        public Transaction GetTransactionID(long id)
        {
            var connectionString = this.GetConnection();

            Transaction _transact = new Transaction();

            SqlDataAdapter _adp;

            System.Data.DataSet response = new System.Data.DataSet();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("sptransactiondefintionwithID", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TransID", id);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow rows in response.Tables[0].Rows)
                        {
                            _transact.ElementCategoryID = Convert.ToInt64(rows["ElementCategoryID"].ToString());
                            _transact.Description = rows["Description"].ToString();
                            _transact.ElementName = rows["Name"].ToString();
                            //_transact.ListId = Convert.ToInt32(rows["ListId"].ToString());
                            _transact.ListName = rows["ListName"].ToString();
                            _transact.TypeCode = rows["TypeCode"].ToString();
                            _transact.TypeName = rows["TypeName"].ToString();
                            _transact.TransID = Convert.ToInt32(rows["TransID"].ToString());
                        }
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Transaction with id- {id} ");
                    loggers.Information(JsonConvert.SerializeObject(_transact));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Transaction with ID thrown an error - {e.Message}");
            }
            return _transact;

        }

        public List<Currency> GetCurrencies()
        {
            var connectionString = this.GetConnection();

            List<Currency> _currencies = new List<Currency>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spGetCurrency", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Currency currency = new Currency
                        {
                            CurrencyID = Convert.ToInt32(rdr["CurrencyID"].ToString()),
                            CurrencyName = rdr["CurrencyName"].ToString()
                        };
                        _currencies.Add(currency);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get currency ");
                    loggers.Information(JsonConvert.SerializeObject(_currencies));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Currency thrown an error - {ex.Message}");
            }
            return _currencies;
        }

        public List<Revenuelist> GetRevenuelist(long agancyid)
        {
            var connectionString = this.GetConnection();

            List<Revenuelist> revenuelist = new List<Revenuelist>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(string.Format("SELECT RevenueCode,RevenueName FROM dbo.vwRevenuelist WHERE AgencyId={0} ORDER BY RevenueName asc",
                        agancyid), con);

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();


                    //con.Open();

                    //SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Revenuelist reve = new Revenuelist
                        {
                            RevenueName = rdr["RevenueName"].ToString(),
                            RevenueCode = rdr["RevenueCode"].ToString()
                        };
                        revenuelist.Add(reve);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get revenue list ");
                    loggers.Information(JsonConvert.SerializeObject(revenuelist));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Brevenue list- {e.Message}");
            }
            return revenuelist;
        }

        public Reconcileperiodid GetReconcileid(long id)
        {
            var connectionString = this.GetConnection();

            //Reconcileperiodid _accounts = new Reconcileperiodid();
            Reconcileperiodid _accounts = new Reconcileperiodid();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spGetReconcilePeriodwithid", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@RecperID", id);

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        //Reconcileperiodid account = new Reconcileperiodid
                        //{
                        _accounts.OpeningBal = Convert.ToDecimal(rdr["OpeningBal"].ToString());
                        _accounts.startdate = Convert.ToDateTime(rdr["StartDate"].ToString());
                        _accounts.enddate = Convert.ToDateTime(rdr["EndDate"].ToString());
                        _accounts.accountname = rdr["AccountName"].ToString();
                        _accounts.ClosingBal = Convert.ToDecimal(rdr["ClosingBal"].ToString());
                        _accounts.AccountID = Convert.ToDecimal(rdr["AccountID"].ToString());
                        //AccountID
                        //};
                        //_accounts.Add(account);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Bank Account ");
                    loggers.Information(JsonConvert.SerializeObject(_accounts));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Bank Account thrown an error - {e.Message}");
            }

            return _accounts;
        }

        public List<Account> GetAccountslList(string bankcode)
        {
            var connectionString = this.GetConnection();

            List<Account> _accounts = new List<Account>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spGetBankAccountwithBankCode", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@BankCode", bankcode);

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Account account = new Account
                        {
                            AccountID = Convert.ToDouble(rdr["AccountID"].ToString()),
                            AccountName = rdr["AccountName"].ToString()
                        };
                        _accounts.Add(account);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Bank Account ");
                    loggers.Information(JsonConvert.SerializeObject(_accounts));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Bank Account thrown an error - {e.Message}");
            }

            return _accounts;
        }

        public List<Postinglist> GetReconciledClosed()
        {
            var connectionString = this.GetConnection();

            List<Postinglist> postinglists = new List<Postinglist>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spGetReconciledClosed", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Postinglist banklist = new Postinglist
                        {
                            RecperID = Convert.ToInt64(rdr["RecperID"].ToString()),
                            Description = rdr["Description"].ToString()
                        };
                        postinglists.Add(banklist);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Bank ");
                    loggers.Information(JsonConvert.SerializeObject(postinglists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
        .WriteTo.MSSqlServer(connectionString, "Logs")
        .CreateLogger();
                logger.Fatal($"Get Bank thrown an error - {ex.Message}");
            }
            return postinglists;
        }

        public List<Postinglist> GetAllPostingRequesting()
        {
            var connectionString = this.GetConnection();

            List<Postinglist> postinglists = new List<Postinglist>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spGetPostingRequestlist", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Postinglist banklist = new Postinglist
                        {
                            RecperID = Convert.ToInt64(rdr["RecperID"].ToString()),
                            Description = rdr["Description"].ToString()
                        };
                        postinglists.Add(banklist);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Bank ");
                    loggers.Information(JsonConvert.SerializeObject(postinglists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
        .WriteTo.MSSqlServer(connectionString, "Logs")
        .CreateLogger();
                logger.Fatal($"Get Bank thrown an error - {ex.Message}");
            }
            return postinglists;
        }

        public List<BankTypecode> GetBankTypecodes()
        {
            var connectionString = this.GetConnection();

            List<BankTypecode> _banklists = new List<BankTypecode>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spGetBankTpeCode", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        BankTypecode banklist = new BankTypecode
                        {
                            Code = rdr["Code"].ToString(),
                            Description = rdr["Description"].ToString()
                        };
                        _banklists.Add(banklist);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Bank ");
                    loggers.Information(JsonConvert.SerializeObject(_banklists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Bank thrown an error - {ex.Message}");
            }

            return _banklists;
        }

        public List<PeriodYear> GetPeriodYear()
        {
            var connectionString = this.GetConnection();

            List<PeriodYear> _definitionlists = new List<PeriodYear>();

            try
            {
                string sqlquery = string.Format("SELECT DISTINCT PeriodYear from Reconcile.ReconcilePeriod WHERE IsClosed = 1 ORDER BY PeriodYear ASC");

                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand(sqlquery, con)
                    {
                        CommandType = CommandType.Text
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        PeriodYear banklist = new PeriodYear
                        {
                            PeridYear = Convert.ToInt64(rdr["PeriodYear"].ToString()),
                            //PeriodName = rdr["PeriodName"].ToString()
                        };
                        _definitionlists.Add(banklist);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Bank ");
                    loggers.Information(JsonConvert.SerializeObject(_definitionlists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Bank thrown an error - {ex.Message}");
            }

            return _definitionlists;
        }

        public List<Periodlist> GetPeriodlist()
        {
            var connectionString = this.GetConnection();

            List<Periodlist> _definitionlists = new List<Periodlist>();

            try
            {
                string sqlquery = string.Format("SELECT DISTINCT PeriodName,PeriodMonth from Reconcile.ReconcilePeriod WHERE IsClosed = 1 ORDER BY PeriodMonth ASC");

                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand(sqlquery, con)
                    {
                        CommandType = CommandType.Text
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Periodlist banklist = new Periodlist
                        {
                            PeriodMonth = Convert.ToInt64(rdr["PeriodMonth"].ToString()),
                            PeriodName = rdr["PeriodName"].ToString()
                        };
                        _definitionlists.Add(banklist);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Bank ");
                    loggers.Information(JsonConvert.SerializeObject(_definitionlists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Bank thrown an error - {ex.Message}");
            }

            return _definitionlists;
        }

        public List<Accountlists> GetAccountlists()
        {
            var connectionString = this.GetConnection();

            List<Accountlists> _definitionlists = new List<Accountlists>();

            try
            {
                string sqlquery = string.Format("SELECT AccountID,AccountName from vwAccountlist ORDER BY AccountName ASC");

                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand(sqlquery, con)
                    {
                        CommandType = CommandType.Text
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Accountlists banklist = new Accountlists
                        {
                            AccountID = Convert.ToInt64(rdr["AccountID"].ToString()),
                            AccountName = rdr["AccountName"].ToString()
                        };
                        _definitionlists.Add(banklist);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Bank ");
                    loggers.Information(JsonConvert.SerializeObject(_definitionlists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Bank thrown an error - {ex.Message}");
            }

            return _definitionlists;
        }

        public List<Definitionlist> GetTransactionDefinitionlist()
        {
            var connectionString = this.GetConnection();

            List<Definitionlist> _definitionlists = new List<Definitionlist>();

            try
            {
                string sqlquery = string.Format("select TransID,Description from vwTransactiondefinitionlist  ORDER BY Description asc");

                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand(sqlquery, con)
                    {
                        CommandType = CommandType.Text
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Definitionlist banklist = new Definitionlist
                        {
                            TransID = Convert.ToInt64(rdr["TransID"].ToString()),
                            Description = rdr["Description"].ToString()
                        };
                        _definitionlists.Add(banklist);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Bank ");
                    loggers.Information(JsonConvert.SerializeObject(_definitionlists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Bank thrown an error - {ex.Message}");
            }

            return _definitionlists;
        }

        public List<Agency> GetAgencylist()
        {
            var connectionString = this.GetConnection();

            List<Agency> _banklists = new List<Agency>();
            try
            {
                //SqlDataAdapter _adp;

                DataSet response = new DataSet();

                response.Clear();

                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    //SqlCommand cmd = new SqlCommand("spGetReconcilePeriod", con)
                    //{
                    //    CommandType = CommandType.StoredProcedure
                    //};
                    SqlCommand cmd = new SqlCommand(string.Format("SELECT AgencyId,AgencyName FROM dbo.vwAgencylist ORDER BY AgencyName asc"), con);

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Agency banklist = new Agency
                        {
                            AgencyId = Convert.ToInt64(rdr["AgencyId"].ToString()),
                            AgencyName = rdr["AgencyName"].ToString()
                        };
                        _banklists.Add(banklist);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Bank ");
                    loggers.Information(JsonConvert.SerializeObject(_banklists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Bank thrown an error - {ex.Message}");
            }
            return _banklists;
        }

        public List<Postinglist> GetRecocileperiod()
        {
            var connectionString = this.GetConnection();

            List<Postinglist> _banklists = new List<Postinglist>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spGetReconcilePeriod", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Postinglist banklist = new Postinglist
                        {
                            RecperID = Convert.ToInt64(rdr["RecperID"].ToString()),
                            Description = rdr["Description"].ToString()
                        };
                        _banklists.Add(banklist);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Bank ");
                    loggers.Information(JsonConvert.SerializeObject(_banklists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Bank thrown an error - {ex.Message}");
            }

            return _banklists;
        }

        public List<Banklist> GeBanklists()
        {
            var connectionString = this.GetConnection();

            List<Banklist> _banklists = new List<Banklist>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spGetBanklists", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Banklist banklist = new Banklist
                        {
                            BankCode = rdr["BankCode"].ToString(),
                            BankName = rdr["BankName"].ToString()
                        };
                        _banklists.Add(banklist);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Bank ");
                    loggers.Information(JsonConvert.SerializeObject(_banklists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Bank thrown an error - {ex.Message}");
            }

            return _banklists;
        }

        public List<Usertypelist> GetUsertypelist()
        {
            var connectionString = this.GetConnection();

            List<Usertypelist> usertypelists = new List<Usertypelist>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spUsertypelist", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Usertypelist banklist = new Usertypelist
                        {
                            TypeCode = rdr["TypeCode"].ToString(),
                            Description = rdr["Description"].ToString()
                        };
                        usertypelists.Add(banklist);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get User Type List ");
                    loggers.Information(JsonConvert.SerializeObject(usertypelists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Bank thrown an error - {ex.Message}");
            }

            return usertypelists;
        }

        public List<Normalisereclists> GetNormalisereclists()
        {
            var connectionString = this.GetConnection();

            List<Normalisereclists> _modifylists = new List<Normalisereclists>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spGetMormaliserecordlist", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Normalisereclists modify = new Normalisereclists
                        {

                            PaymentRefNumber = rdr["PaymentRefNumber"].ToString(),
                            Amount = Convert.ToDecimal(rdr["Amount"].ToString()),
                            payername = rdr["payername"].ToString(),
                            AgencyCode = rdr["AgencyCode"].ToString(),
                            RevenueName = rdr["RevenueName"].ToString(),
                            RevenueCode = rdr["RevenueCode"].ToString(),
                            NewPayerName = rdr["NewPayerName"].ToString(),
                            AgencyName = rdr["AgencyName"].ToString(),
                            NormalisedBy = rdr["NormalisedBy"].ToString(),
                            NormaliseDate = Convert.ToDateTime(rdr["NormaliseDate"].ToString())
                        };

                        _modifylists.Add(modify);

                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Request sent List ");
                    loggers.Information(JsonConvert.SerializeObject(_modifylists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Request sent List thrown an error - {ex.Message}");
            }

            return _modifylists;
        }

        public List<RequestPushReemList> GetRequestPushReemLists()
        {
            var connectionString = this.GetConnection();

            List<RequestPushReemList> _modifylists = new List<RequestPushReemList>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spDoGetPushRequestMade", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        RequestPushReemList modify = new RequestPushReemList
                        {
                            PaymentDate = Convert.ToDateTime(rdr["PaymentDate"].ToString()),
                            PaymentRefNumber = rdr["PaymentRefNumber"].ToString(),
                            payername = rdr["payername"].ToString(),
                            Amount = Convert.ToDecimal(rdr["Amount"].ToString()),
                            RevenueName = rdr["RevenueName"].ToString(),
                            PushToReemsRequestBy = rdr["PushToReemsRequestBy"].ToString(),
                            PushToReemsRequestOn = Convert.ToDateTime(rdr["PushToReemsRequestOn"].ToString())
                        };

                        _modifylists.Add(modify);

                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Request sent List ");
                    loggers.Information(JsonConvert.SerializeObject(_modifylists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Request sent List thrown an error - {ex.Message}");
            }

            return _modifylists;
        }

        public List<ModifyRecords> GetModifyRecords()
        {
            var connectionString = this.GetConnection();

            List<ModifyRecords> _modifylists = new List<ModifyRecords>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spGetModifyRequestsentlist", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        ModifyRecords modify = new ModifyRecords
                        {
                            ModifyID = Convert.ToInt64(rdr["ModifyID"].ToString()),
                            ModifyBy = rdr["ModifyBy"].ToString(),
                            ModifyDate = Convert.ToDateTime(rdr["ModifyDate"].ToString()),
                            BSDate = Convert.ToDateTime(rdr["BSDate"].ToString()),
                            Amount = Convert.ToDecimal(rdr["Amount"].ToString()),
                            oldDescription = rdr["oldDescription"].ToString(),
                            NewDescription = rdr["NewDescription"].ToString()
                        };

                        _modifylists.Add(modify);

                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Request sent List ");
                    loggers.Information(JsonConvert.SerializeObject(_modifylists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Request sent List thrown an error - {ex.Message}");
            }

            return _modifylists;
        }

        public List<Requesntsent> GetRequestsentlist()
        {
            var connectionString = this.GetConnection();

            List<Requesntsent> _banklists = new List<Requesntsent>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spGetReconcileRequestsentlist", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Requesntsent banklist = new Requesntsent
                        {
                            RecperID = Convert.ToInt64(rdr["RecperID"].ToString()),
                            Description = rdr["Description"].ToString(),
                            BankName = rdr["BankName"].ToString(),
                            AccountName = rdr["AccountName"].ToString(),
                            StartDate = Convert.ToDateTime(rdr["StartDate"].ToString()),
                            EndDate = Convert.ToDateTime(rdr["EndDate"].ToString()),
                            openingbal = Convert.ToDecimal(rdr["OpeningBal"].ToString()),
                            closebal = Convert.ToDecimal(rdr["ClosingBal"].ToString()),
                            debit = Convert.ToDecimal(rdr["Debit"].ToString()),
                            credit = Convert.ToDecimal(rdr["Credit"].ToString()),
                            Stages = rdr["Stages"].ToString(),
                            Requestby = rdr["RequestBy"].ToString(),
                            days = Convert.ToInt32(rdr["Number"].ToString())


                        };
                        _banklists.Add(banklist);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Request sent List ");
                    loggers.Information(JsonConvert.SerializeObject(_banklists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Request sent List thrown an error - {ex.Message}");
            }

            return _banklists;
        }

        public List<PushException> GetPushExceptions()
        {
            var connectionString = this.GetConnection();

            List<PushException> _banklists = new List<PushException>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spGetPushExceptionRequestlist", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        PushException banklist = new PushException
                        {

                            PaymentRefNumber = rdr["PaymentRefNumber"].ToString(),
                            payername = rdr["payername"].ToString(),
                            RevenueName = rdr["RevenueName"].ToString(),
                            PaymentDate = Convert.ToDateTime(rdr["PaymentDate"].ToString()),
                            Amount = Convert.ToDecimal(rdr["Amount"].ToString())

                        };
                        _banklists.Add(banklist);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Transaction List ");
                    loggers.Information(JsonConvert.SerializeObject(_banklists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Transaction List thrown an error - {ex.Message}");
            }

            return _banklists;
        }

        public List<RequestApprove> GetRequestApprove()
        {
            var connectionString = this.GetConnection();

            List<RequestApprove> _banklists = new List<RequestApprove>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spGetReconcileRequestlist", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        RequestApprove banklist = new RequestApprove
                        {
                            RecperID = Convert.ToInt64(rdr["RecperID"].ToString()),
                            Description = rdr["Description"].ToString(),
                            BankName = rdr["BankName"].ToString(),
                            AccountName = rdr["AccountName"].ToString(),
                            StartDate = Convert.ToDateTime(rdr["StartDate"].ToString()),
                            EndDate = Convert.ToDateTime(rdr["EndDate"].ToString()),
                            openingbal = Convert.ToDecimal(rdr["OpeningBal"].ToString()),
                            closebal = Convert.ToDecimal(rdr["ClosingBal"].ToString()),
                            debit = Convert.ToDecimal(rdr["Debit"].ToString()),
                            credit = Convert.ToDecimal(rdr["Credit"].ToString()),
                            Stages = rdr["Stages"].ToString(),
                            Period = rdr["PeriodName"].ToString(),
                            days = Convert.ToInt32(rdr["Number"].ToString()),
                            Year = Convert.ToInt64(rdr["PeriodYear"].ToString())


                        };
                        _banklists.Add(banklist);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Transaction List ");
                    loggers.Information(JsonConvert.SerializeObject(_banklists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Transaction List thrown an error - {ex.Message}");
            }

            return _banklists;
        }

        public List<Reconcilelist> Getrecocilelists()
        {
            var connectionString = this.GetConnection();

            List<Reconcilelist> _banklists = new List<Reconcilelist>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spGetReconcilelist", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Reconcilelist banklist = new Reconcilelist
                        {
                            RecperID = Convert.ToInt64(rdr["RecperID"].ToString()),
                            Description = rdr["Description"].ToString(),
                            BankName = rdr["BankName"].ToString(),
                            AccountName = rdr["AccountName"].ToString(),
                            StartDate = Convert.ToDateTime(rdr["StartDate"].ToString()),
                            EndDate = Convert.ToDateTime(rdr["EndDate"].ToString()),
                            AccountNumber = rdr["AccountNumber"].ToString(),
                            Stages = rdr["Stages"].ToString(),
                            days = Convert.ToInt32(rdr["Number"].ToString())


                        };
                        _banklists.Add(banklist);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Transaction List ");
                    loggers.Information(JsonConvert.SerializeObject(_banklists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Transaction List thrown an error - {ex.Message}");
            }

            return _banklists;
        }

        public List<TransactionList> GeTransactionLists()
        {
            var connectionString = this.GetConnection();

            List<TransactionList> _banklists = new List<TransactionList>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("SpGetTransactionLists", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        //Datetime DT = DateTime.ParseExact(STRDATE, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat)
                        TransactionList banklist = new TransactionList
                        {
                            RecperID = Convert.ToInt64(rdr["RecperID"].ToString()),
                            BankName = rdr["BankName"].ToString(),
                            AccountName = rdr["AccountName"].ToString(),
                            StartDate = Convert.ToDateTime(rdr["StartDate"].ToString()),
                            EndDate = Convert.ToDateTime(rdr["EndDate"].ToString()),
                            AccountID = Convert.ToInt64(rdr["AccountID"].ToString()),
                            StagesID = Convert.ToInt64(rdr["StagesID"].ToString()),
                            Year = Convert.ToInt64(rdr["PeriodYear"].ToString()),
                            OpeningBal = Convert.ToDouble(rdr["OpeningBal"].ToString()),
                            ClosingBal = Convert.ToDouble(rdr["ClosingBal"].ToString()),
                            IsClosed = Convert.ToBoolean(rdr["IsClosed"].ToString()),
                            Symbol = rdr["Symbol"].ToString(),
                            Stages = rdr["Stages"].ToString(),
                            Period = rdr["PeriodName"].ToString(),
                            Currency = rdr["Currency"].ToString()

                        };
                        _banklists.Add(banklist);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Transaction List ");
                    loggers.Information(JsonConvert.SerializeObject(_banklists));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Transaction List thrown an error - {ex.Message}");
            }

            return _banklists;
        }

        public AccountBal GetAccountBalance(long Accountid)
        {
            var connectionString = this.GetConnection();

            AccountBal _acctbal = new AccountBal();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spDogetAccountOpeningbalanceStartDate", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@AccountId", Accountid);

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    DateTime today = DateTime.Today; // As DateTime
                    DateTime s_today = Convert.ToDateTime(today.ToString("d")); // As String

                    while (rdr.Read())
                    {
                        _acctbal.OpenBal = Convert.ToDouble(rdr["openingbal"].ToString());
                        _acctbal.Startdate = !string.IsNullOrWhiteSpace(rdr["startdate"].ToString()) ? Convert.ToDateTime(rdr["startdate"].ToString()) : s_today;
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Account Opening Balance with startdate ");
                    loggers.Information(JsonConvert.SerializeObject(_acctbal));

                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Account Opening Balance with Start date thrown an error - {e.Message}");
            }
            return _acctbal;
        }

        public List<AccountBal> GetAccountBals(double Accountid)
        {
            var connectionString = this.GetConnection();

            List<AccountBal> _acctbal = new List<AccountBal>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spDogetAccountOpeningbalanceStartDate", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@AccountId", Accountid);

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        AccountBal balact = new AccountBal
                        {
                            OpenBal = Convert.ToDouble(rdr["openingbal"].ToString()),
                            Startdate = !string.IsNullOrWhiteSpace(rdr["startdate"].ToString()) ? Convert.ToDateTime(rdr["startdate"].ToString()) : DateTime.Now
                        };
                        _acctbal.Add(balact);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Account Opening Balance with startdate ");
                    loggers.Information(JsonConvert.SerializeObject(_acctbal));

                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Account Opening Balance with Start date thrown an error - {e.Message}");
            }

            return _acctbal;
        }

        public List<Bank> GetBanklistList()
        {
            var connectionString = this.GetConnection();

            List<Bank> _banks = new List<Bank>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spBankAccountList", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Bank bnk = new Bank
                        {
                            AccountID = Convert.ToInt64(rdr["AccountID"].ToString()),
                            AccountName = rdr["AccountName"].ToString(),
                            AccountNumber = rdr["AccountNumber"].ToString(),
                            OpeningBal = Convert.ToDecimal(rdr["OpeningBal"].ToString()),
                            CreatedDate = Convert.ToDateTime(rdr["CreatedDate"].ToString()),
                            BankName = rdr["BankName"].ToString(),
                            Description = rdr["Description"].ToString(),
                            IsActive = Convert.ToBoolean(rdr["IsActive"].ToString())

                        };
                        _banks.Add(bnk);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Bank Lists");
                    loggers.Information(JsonConvert.SerializeObject(_banks));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Bank Lists thrown an error - {ex.Message}");
            }

            return _banks;
        }

        public List<UsersList> GetUserAccountlist()
        {
            var connectionString = this.GetConnection();

            List<UsersList> _userlist = new List<UsersList>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spUserAccountList", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        UsersList bnk = new UsersList
                        {
                            UserId = Convert.ToInt64(rdr["UserId"].ToString()),
                            Surname = rdr["Surname"].ToString(),
                            Firstname = rdr["Firstname"].ToString(),
                            Mobile = rdr["Mobile"].ToString(),
                            Address = rdr["Address"].ToString(),
                            Description = rdr["Description"].ToString(),
                            EmailAddress = rdr["EmailAddress"].ToString(),
                            IsActive = Convert.ToBoolean(rdr["Active"].ToString())

                        };
                        _userlist.Add(bnk);
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Users Lists");
                    loggers.Information(JsonConvert.SerializeObject(_userlist));
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get user's Lists thrown an error - {ex.Message}");
            }

            return _userlist;
        }

        public int DisableTransction(int id)
        {
            var connectionString = this.GetConnection();
            var count = 0;
            SqlDataAdapter _adp;
            System.Data.DataSet response = new System.Data.DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spTransactionDefinitionDisable", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TransID", id);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {
                        count = -1;
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information($"Disable Transction Error Encounter -{string.Format("{0}{1}", id, response.Tables[0].Rows[0]["returnCode"].ToString())}");
                        loggers.Information(JsonConvert.SerializeObject(string.Format("{0}{1}", id, response.Tables[0].Rows[0]["returnCode"].ToString())));
                    }
                    else
                    {
                        count = 1;

                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information($"Disable Transction ID -{id}");
                        loggers.Information(JsonConvert.SerializeObject(id));
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Disable Transction thrown an error - {ex.Message}");
            }


            return count;
        }

        public ResponseInfo<ReconcilePeriod> CreateReconciledate(ReconcilePeriod reconcilePeriod)
        {
            Entity.ResponseInfo<ReconcilePeriod> responses = new Entity.ResponseInfo<ReconcilePeriod>();

            var connectionString = this.GetConnection();

            try
            {
                //List<RegResponseInfo<Fecthpayer>> regResponse = new List<RegResponseInfo<Fecthpayer>>();
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spCreateReconcileDate", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@AccountID", reconcilePeriod.AccountID);

                    cmd.Parameters.AddWithValue("@StartDate", reconcilePeriod.StartDate.ToString("yyyy-MM-dd"));

                    cmd.Parameters.AddWithValue("@EndDate", reconcilePeriod.EndDate.ToString("yyyy-MM-dd"));

                    cmd.Parameters.AddWithValue("@OpeningBal", reconcilePeriod.Balance);

                    cmd.Parameters.AddWithValue("@closebal", reconcilePeriod.ClosingBal);

                    cmd.Parameters.AddWithValue("@periodname", reconcilePeriod.Period);


                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {
                        //count = -1;

                        responses.StatusCode = response.Tables[0].Rows[0]["returnCode"].ToString();
                        responses.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString();
                        //bc.Reconcileid = Convert.ToInt64(response.Tables[0].Rows[0]["Reconcileid"].ToString());
                    }
                    else
                    {
                        responses.StatusCode = response.Tables[0].Rows[0]["returnCode"].ToString();
                        responses.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString();
                        //bc.Reconcileid = Convert.ToInt64(response.Tables[1].Rows[0]["Reconcileid"].ToString());

                        //count = 1;
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Bank Statement Saved sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information("Save Bank Statement successful");
                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }
            }
            catch (Exception e)
            {
                responses.StatusCode = e.HResult.ToString();
                responses.StatusMessage = e.Message.ToString();
                responses.ResponseObject = null;

                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Save Bank Statement thrown an error - {e.Message}");
            }
            return responses;
        }

        public Entity.ResponseInfo<Reconcileday> Validatereconciledate(Reconcileday reconcliedays)
        {
            Entity.ResponseInfo<Reconcileday> responses = new Entity.ResponseInfo<Reconcileday>();

            var connectionString = this.GetConnection();

            try
            {
                //List<RegResponseInfo<Fecthpayer>> regResponse = new List<RegResponseInfo<Fecthpayer>>();
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spValidateReconcileDate", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@accountid", reconcliedays.Accountid);

                    cmd.Parameters.AddWithValue("@startdate", reconcliedays.Startdate.ToString("yyyy-MM-dd"));

                    cmd.Parameters.AddWithValue("@enddate", reconcliedays.Enddate.ToString("yyyy-MM-dd"));


                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {
                        //count = -1;

                        responses.StatusCode = response.Tables[0].Rows[0]["returnCode"].ToString();
                        responses.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString();
                        //bc.Reconcileid = Convert.ToInt64(response.Tables[0].Rows[0]["Reconcileid"].ToString());
                    }
                    else
                    {
                        responses.StatusCode = response.Tables[0].Rows[0]["returnCode"].ToString();
                        responses.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString();
                        //bc.Reconcileid = Convert.ToInt64(response.Tables[1].Rows[0]["Reconcileid"].ToString());

                        //count = 1;
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Bank Statement Saved sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information("Save Bank Statement successful");
                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }
            }
            catch (Exception e)
            {
                responses.StatusCode = e.HResult.ToString();
                responses.StatusMessage = e.Message.ToString();
                responses.ResponseObject = null;

                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Save Bank Statement thrown an error - {e.Message}");
            }
            return responses;
        }

        public int dorestebankstatemen(long reconcileId)
        {
            int count = 0;

            DataSet dtresult = new DataSet();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spResetBankStatment", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@RecperID", reconcileId);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response;

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Reclassified record found sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                        count = 1;
                    }
                    else
                    {
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);

                        count = -1;
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                                     .WriteTo.MSSqlServer(connectionString, "Logs")
                                     .CreateLogger();
                logger.Fatal($"Save Bank Statement thrown an error - {e.Message}");
            }
            return count;
        }

        public int SaveAllocatetrans(DataTable dts)
        {
            var count = 0;

            var connectionString = this.GetConnection();

            DataTable dtsnew = dts;



            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }
                    else
                    {
                        con.Open();
                    }

                    //using (SqlTransaction sqlTransaction = con.BeginTransaction())
                    //{
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name
                        sqlBulkCopy.DestinationTableName = "Reconcile.BankStatementAllocation";

                        //[OPTIONAL]: Map the Excel columns with that of the database table
                        sqlBulkCopy.ColumnMappings.Add("pBsid", "BSID");
                        sqlBulkCopy.ColumnMappings.Add("pTransid", "TransID");
                        sqlBulkCopy.ColumnMappings.Add("IsCredit", "IsCredit");
                        sqlBulkCopy.ColumnMappings.Add("AllocateBy", "AllocateBy");
                        sqlBulkCopy.ColumnMappings.Add("AllocateDate", "AllocateDate");

                        sqlBulkCopy.BatchSize = 1000000;
                        sqlBulkCopy.BulkCopyTimeout = 0;
                        //con.Open();
                        sqlBulkCopy.WriteToServer(dtsnew);



                        //try
                        //{
                        //    sqlBulkCopy.WriteToServer(dtsnew);

                        //    sqlTransaction.Commit();

                        count = 1;
                        //}
                        //catch
                        //{
                        //    sqlTransaction.Rollback();

                        //    count = 0;
                        //}
                    }
                    //}
                    con.Close();

                }

            }
            catch (Exception e)
            {
                var gh = e.Message;
                count = 0;
            }



            return count;
        }

        public DataSet SaveNormaliseRecord(string usermail, string paymentref, string payername, string agencyname,
            string agencycode, string revenuename, string revenusecode, string address)
        {
            DataSet dtresult = new DataSet();

            var connectionString = this.GetConnection();

            dtresult.Clear();

            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }
                    else
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand("spNormaliserecord", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@useremail", usermail);
                    cmd.Parameters.AddWithValue("@paymentref", paymentref);
                    cmd.Parameters.AddWithValue("@payername", payername);
                    cmd.Parameters.AddWithValue("@agencyname", agencyname);
                    cmd.Parameters.AddWithValue("@agencycode", agencycode);
                    cmd.Parameters.AddWithValue("@revenuename", revenuename);
                    cmd.Parameters.AddWithValue("@revenuecode", revenusecode);
                    cmd.Parameters.AddWithValue("@addresss", address);


                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response;

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        dtresult = response;
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                      .WriteTo.MSSqlServer(connectionString, "Logs")
                      .CreateLogger();
                logger.Fatal($"Normalise thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public int SaveBankmport(DataTable dts, long RecperID)
        {
            var count = 0;

            var connectionString = this.GetConnection();

            DataTable dtsnew = dts;

            dtsnew.Columns.Add("RecperID");

            foreach (DataRow d in dtsnew.Rows)
            {
                d["RecperID"] = RecperID;
            }

            dtsnew.AcceptChanges();

            int gh = dorestebankstatemen(RecperID);

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }
                    else
                    {
                        con.Open();
                    }

                    //using (SqlTransaction sqlTransaction = con.BeginTransaction())
                    //{
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name
                        sqlBulkCopy.DestinationTableName = "Reconcile.BankStatement";

                        //[OPTIONAL]: Map the Excel columns with that of the database table
                        sqlBulkCopy.ColumnMappings.Add("RecperID", "RecperID");
                        sqlBulkCopy.ColumnMappings.Add("Date", "BSDate");
                        sqlBulkCopy.ColumnMappings.Add("Debit", "Debit");
                        sqlBulkCopy.ColumnMappings.Add("Credit", "Credit");
                        sqlBulkCopy.ColumnMappings.Add("Balance", "Balance");
                        sqlBulkCopy.ColumnMappings.Add("Revenuecode", "RevenueCode");
                        sqlBulkCopy.ColumnMappings.Add("Description", "PayerName");
                        sqlBulkCopy.ColumnMappings.Add("Tellerno", "TellerNo");
                        sqlBulkCopy.ColumnMappings.Add("TransID", "TransID");
                        //con.Open();
                        sqlBulkCopy.WriteToServer(dtsnew);




                        //try
                        //{
                        //    sqlBulkCopy.WriteToServer(dtsnew);

                        //    sqlTransaction.Commit();

                        count = 1;
                        //}
                        //catch (Exception e)
                        //{
                        //    var gh = e.Message;

                        //    sqlTransaction.Rollback();

                        //    count = 0;
                        //}

                    }
                    //}
                    con.Close();
                }
            }
            catch (Exception)
            {

                count = 0;
            }



            return count;

        }

        public ResponseMessage<BankImport> Savebankstatement(BankImport bankImport, DataTable dts)
        {
            ResponseMessage<BankImport> bc = new ResponseMessage<BankImport>();

            var connectionString = this.GetConnection();
            //var count = 0;

            try
            {
                //List<RegResponseInfo<Fecthpayer>> regResponse = new List<RegResponseInfo<Fecthpayer>>();
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spSaveBankStatement", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@AccountID", bankImport.AccountID);
                    cmd.Parameters.AddWithValue("@StartDate", bankImport.StartDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@EndDate", bankImport.DateEnd.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@OpeningBal", bankImport.OpeningBal);
                    cmd.Parameters.AddWithValue("@RecperID", bankImport.RecperID);
                    //cmd.Parameters.AddWithValue("@pTransaction", dts);
                    cmd.Parameters.AddWithValue("@closebal", bankImport.ClosingBal);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {
                        //count = -1;

                        bc.StatusId = Convert.ToInt32(response.Tables[0].Rows[0]["returnCode"].ToString());
                        bc.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString();
                        //bc.Reconcileid = Convert.ToInt64(response.Tables[0].Rows[0]["Reconcileid"].ToString());
                    }
                    else
                    {

                        bc.StatusId = Convert.ToInt32(response.Tables[0].Rows[0]["returnCode"].ToString());
                        bc.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString();
                        //bc.Reconcileid = Convert.ToInt64(response.Tables[1].Rows[0]["Reconcileid"].ToString());

                        //count = 1;
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Bank Statement Saved sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information("Save Bank Statement successful");
                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }
            }
            catch (Exception e)
            {
                bc.StatusId = Convert.ToInt32(e.HResult);
                bc.StatusMessage = e.Message.ToString();
                bc.RecordResponseObject = null;

                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Save Bank Statement thrown an error - {e.Message}");
            }

            return bc;
        }

        public int DisableUserAccount(long Userid)
        {
            var connectionString = this.GetConnection();
            var count = 0;
            try
            {
                SqlDataAdapter _adp;
                DataSet response = new DataSet();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spDisableUserAccount", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@userid", Userid);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[1].Rows[0]["returnCode"].ToString() != "00")
                    {
                        count = -1;
                    }
                    else
                    {
                        count = 1;
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Disable Users Account sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information("Login stored procedure response successful; User credentials found");
                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }
                //return count;
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                      .WriteTo.MSSqlServer(connectionString, "Logs")
                      .CreateLogger();
                logger.Fatal($"Disable Users Account thrown an error - {ex.Message}");
            }
            return count;
        }

        public int DisableBankAccount(long AccountID)
        {
            var connectionString = this.GetConnection();
            var count = 0;
            try
            {
                SqlDataAdapter _adp;
                DataSet response = new DataSet();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spBankAccountDisable", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@AccountID", AccountID);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[1].Rows[0]["returnCode"].ToString() != "00")
                    {
                        count = -1;
                    }
                    else
                    {
                        count = 1;
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Disable Bank Account sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information("Login stored procedure response successful; User credentials found");
                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }
                //return count;
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                      .WriteTo.MSSqlServer(connectionString, "Logs")
                      .CreateLogger();
                logger.Fatal($"Disable Bank Account thrown an error - {ex.Message}");
            }

            return count;

        }

        public DataSet GetUserLogin(string Emailaddress)
        {
            DataSet dtresult = new DataSet();

            var connectionString = this.GetConnection();

            dtresult.Clear();

            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }
                    else
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand("spGetUserLogin", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@emailAddress", Emailaddress);


                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response;

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Get User Login sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information("Get User Login  Statement compare successful");
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                      .WriteTo.MSSqlServer(connectionString, "Logs")
                      .CreateLogger();
                logger.Fatal($"Get User accountthrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataSet CompareBankStatement(long reconcileId)
        {
            DataSet dtresult = new DataSet();

            dtresult.Clear();

            var connectionString = this.GetConnection();
            try
            {
                SqlDataAdapter _adp;
                DataSet response = new DataSet();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spCompareBankStatementCollection", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@RecperID", reconcileId);
                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response;

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Bank Statement Compare sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information("Bank Statement compare successful");
                        loggers.Information(serializeReponse);
                    }
                    //else
                    //{
                    //    //bc.StatusId = Convert.ToInt32(response.Tables[0].Rows[0]["returnCode"].ToString());
                    //    //bc.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString();
                    //    //bc.RecordResponseObject = null;

                    //    //count = 1;
                    //    var logger = new LoggerConfiguration()
                    //        .WriteTo.MSSqlServer(connectionString, "Logs")
                    //        .CreateLogger();
                    //    logger.Information("Bank Statement Saved sucessfully");

                    //    var serializeReponse = JsonConvert.SerializeObject(response);
                    //    var loggers = new LoggerConfiguration()
                    //        .WriteTo.MSSqlServer(connectionString, "Logs")
                    //        .CreateLogger();
                    //    loggers.Information("Save Bank Statement successful");
                    //    loggers.Information(serializeReponse);
                    //}

                    con.Close();
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                     .WriteTo.MSSqlServer(connectionString, "Logs")
                     .CreateLogger();
                logger.Fatal($"Save Bank Statement thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataSet SaveCompareStatement(long reconcileId, double closebal, DataTable dtbank, DataTable dtColl, DataTable dtMatc, string EmailAddress)
        {

            DataSet dtresult = new DataSet();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;
                DataSet response = new DataSet();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spDoSaveCompreResult", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@RecondID", reconcileId);
                    cmd.Parameters.AddWithValue("@CloseBal", closebal);
                    cmd.Parameters.AddWithValue("@ptransctCol", dtColl);
                    cmd.Parameters.AddWithValue("@ptranscMatched", dtMatc);
                    cmd.Parameters.AddWithValue("@ptransctBank", dtbank);
                    cmd.Parameters.AddWithValue("@Useremail", EmailAddress);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response;

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Compare Statement sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                                     .WriteTo.MSSqlServer(connectionString, "Logs")
                                     .CreateLogger();
                logger.Fatal($"Save Bank Statement thrown an error - {e.Message}");
            }
            return dtresult;
        }

        public DataSet Reclassified(long reconcileId)
        {
            DataSet dtresult = new DataSet();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spdoReclassified", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@RecperID", reconcileId);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response;

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Reclassified record found sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                                     .WriteTo.MSSqlServer(connectionString, "Logs")
                                     .CreateLogger();
                logger.Fatal($"Save Bank Statement thrown an error - {e.Message}");
            }
            return dtresult;
        }

        public DataSet CategoryCheck(DataTable dbvalues)
        {
            DataSet dtresult = new DataSet();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;
                DataSet response = new DataSet();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spDoCategoryChecker", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@BSID", bsid);
                    cmd.Parameters.AddWithValue("@ptranValue", dbvalues);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response;

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        dtresult = response;

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                                                    .WriteTo.MSSqlServer(connectionString, "Logs")
                                                    .CreateLogger();
                logger.Fatal($"Category Checker thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataSet DisapproveNormalise(string userid, string Refnumb, string strreasons)
        {
            DataSet dtresult = new DataSet();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;
                DataSet response = new DataSet();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spDoDisapproveNormaliserec", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Refnumber", Refnumb);
                    cmd.Parameters.AddWithValue("@Userid", userid);
                    cmd.Parameters.AddWithValue("@reason", strreasons);


                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response;

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                                                    .WriteTo.MSSqlServer(connectionString, "Logs")
                                                    .CreateLogger();
                logger.Fatal($"Normalise approval thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataSet Disapprovemodify(string userid, long modifyid, string strreasons)
        {
            DataSet dtresult = new DataSet();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;
                DataSet response = new DataSet();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spGetModifyDisapprove", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@modifyid", modifyid);
                    cmd.Parameters.AddWithValue("@Userid", userid);
                    cmd.Parameters.AddWithValue("@reason", strreasons);


                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response;

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                                                    .WriteTo.MSSqlServer(connectionString, "Logs")
                                                    .CreateLogger();
                logger.Fatal($"Allocate Modify Transaction thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataSet AllocateModify(string userid, DataTable dbvalues)
        {
            DataSet dtresult = new DataSet();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;
                DataSet response = new DataSet();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spDoModifyAllocatetransation", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Userid", userid);
                    cmd.Parameters.AddWithValue("@ptranValue", dbvalues);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response;

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Alocate Transaction sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                                                    .WriteTo.MSSqlServer(connectionString, "Logs")
                                                    .CreateLogger();
                logger.Fatal($"Allocate Modify Transaction thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataSet SearchReconcilizedRecord(string referencenumber)
        {
            DataSet dtresult = new DataSet();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("SearchReconcliedRecords", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@strRefNumber", referencenumber);

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response;

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Search Reconcilized Record");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        dtresult = response;

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                                                    .WriteTo.MSSqlServer(connectionString, "Logs")
                                                    .CreateLogger();
                logger.Fatal($"Search Reconcilized Record - {e.Message}");
            }

            return dtresult;
        }

        public DataSet AllocateTransaction(long reconcileId, string UsersId)
        {
            DataSet dtresult = new DataSet();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;
                DataSet response = new DataSet();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spDoAllocatetransation", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RecondID", reconcileId);
                    cmd.Parameters.AddWithValue("@Userid", UsersId);
                    //cmd.Parameters.AddWithValue("@ptranDeb", dbDebit);
                    //cmd.Parameters.AddWithValue("@ptranCred", DbCredit);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response;

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Alocate Transaction sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                                                    .WriteTo.MSSqlServer(connectionString, "Logs")
                                                    .CreateLogger();
                logger.Fatal($"Save Bank Statement thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataSet FinishedReconcile(long reconcileId, string UsersId)
        {
            DataSet dtresult = new DataSet();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;
                DataSet response = new DataSet();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spdoFinsihedReconcile", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@recondID", reconcileId);
                    cmd.Parameters.AddWithValue("@emailaddress", UsersId);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response;

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Alocate Transaction sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                                                    .WriteTo.MSSqlServer(connectionString, "Logs")
                                                    .CreateLogger();
                logger.Fatal($"Save Bank Statement thrown an error - {ex.Message}");
            }
            return dtresult;
        }

        public DataSet PostingRequestSent(long reconcileId, string UsersId)
        {

            DataSet dtresult = new DataSet();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;
                DataSet response = new DataSet();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spdoPostingRequest", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@recondID", reconcileId);
                    cmd.Parameters.AddWithValue("@emailaddress", UsersId);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response;

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Alocate Transaction sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                                                    .WriteTo.MSSqlServer(connectionString, "Logs")
                                                    .CreateLogger();
                logger.Fatal($"Save Bank Statement thrown an error - {ex.Message}");
            }
            return dtresult;
        }

        public Int32 UpdateUserlog(string usermailaddress, Int32 blFlag)
        {
            DataSet dtresult = new DataSet();

            Int32 intcount = 0;

            dtresult.Clear(); SqlDataAdapter _adp;

            var connectionString = this.GetConnection();

            string sqlquery = string.Format("UPDATE Reconcile.[User] SET Flag={0} where EmailAddress='{1}'", blFlag, usermailaddress);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                if (con.State != ConnectionState.Closed)
                {
                    con.Close();
                }

                SqlCommand cmd = new SqlCommand(sqlquery, con);

                con.Open();

                cmd.ExecuteNonQuery();

                //cmd.CommandType = CommandType.Text;

                intcount = 1;

                //cmd.CommandTimeout = 0;

                //_adp = new SqlDataAdapter(cmd);

                //_adp.Fill(dtresult);
            }

            return intcount;
        }

        public DataSet SearchUserbyEmial(string useremail)
        {
            DataSet dtresult = new DataSet();


            dtresult.Clear(); SqlDataAdapter _adp;

            var connectionString = this.GetConnection();

            string sqlquery = string.Format("SELECT UPPER(Surname) +' '+ Firstname AS Username,EmailAddress from  Reconcile.[User] where EmailAddress='{0}' AND Flag=1", useremail);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                if (con.State != ConnectionState.Closed)
                {
                    con.Close();
                }

                SqlCommand cmd = new SqlCommand(sqlquery, con);

                cmd.CommandType = CommandType.Text;

                con.Open();

                cmd.CommandTimeout = 0;

                _adp = new SqlDataAdapter(cmd);

                _adp.Fill(dtresult);
            }

            return dtresult;

        }

        public DataSet CheckUserid(string usermailaddress)
        {
            DataSet dtresult = new DataSet();

            dtresult.Clear(); SqlDataAdapter _adp;

            var connectionString = this.GetConnection();

            string sqlquery = string.Format("SELECT * from  Reconcile.[User] where EmailAddress='{0}'", usermailaddress);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                if (con.State != ConnectionState.Closed)
                {
                    con.Close();
                }

                SqlCommand cmd = new SqlCommand(sqlquery, con);

                cmd.CommandType = CommandType.Text;

                con.Open();

                cmd.CommandTimeout = 0;

                _adp = new SqlDataAdapter(cmd);

                _adp.Fill(dtresult);
            }

            return dtresult;
        }

        public DataTable viewExceptionNor(Entity.Exceptions exception)
        {
            DataTable dtresult = new DataTable();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                string sqlquery = string.Format("select * from vwTransactionDefiniations where PeriodMonth={0} and PeriodYear={1} and AccountID={2} and TransID={3} AND PaymentRefNumber NOT IN  (SELECT PaymentRefNumber FROM Reconcile.Normalise WHERE IsNormalised = 1 AND IsNormaliseApproved = 1)", exception.periodMonth, exception.periodYear, exception.AccountID, exception.TransID);

                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                response.Clear();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand(sqlquery, con);

                    cmd.CommandType = CommandType.Text;

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    //dtresult = response.Tables[0];

                    if (response.Tables[0] != null && response.Tables[0].Rows.Count > 0)
                    {
                        dtresult = response.Tables[0];

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("summary view sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        //loggers.Information(response.Tables[0]);
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }

            }
            catch (System.Exception e)
            {
                var logger = new LoggerConfiguration()
                  .WriteTo.MSSqlServer(connectionString, "Logs")
                  .CreateLogger();
                logger.Fatal($"View Summary Report thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataTable viewException(Entity.Exceptions exception)
        {
            DataTable dtresult = new DataTable();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                string sqlquery = string.Format("select * from vwTransactionDefiniations where PeriodMonth={0} and PeriodYear={1} and AccountID={2} and TransID={3}", exception.periodMonth, exception.periodYear, exception.AccountID, exception.TransID);

                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                response.Clear();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand(sqlquery, con);

                    cmd.CommandType = CommandType.Text;

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    //dtresult = response.Tables[0];

                    if (response.Tables[0] != null && response.Tables[0].Rows.Count > 0)
                    {
                        dtresult = response.Tables[0];

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("summary view sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        //loggers.Information(response.Tables[0]);
                        loggers.Information(serializeReponse);
                    }
                    //else
                    //{
                    //    var logger = new LoggerConfiguration()
                    //        .WriteTo.MSSqlServer(connectionString, "Logs")
                    //        .CreateLogger();
                    //    logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                    //    var serializeReponse = JsonConvert.SerializeObject(response);
                    //    var loggers = new LoggerConfiguration()
                    //        .WriteTo.MSSqlServer(connectionString, "Logs")
                    //        .CreateLogger();
                    //    loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                    //    loggers.Information(serializeReponse);
                    //}
                    con.Close();
                }

            }
            catch (System.Exception e)
            {
                var logger = new LoggerConfiguration()
                  .WriteTo.MSSqlServer(connectionString, "Logs")
                  .CreateLogger();
                logger.Fatal($"View Summary Report thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataTable ViewBanksCollection(Summarys sumbanks)
        {
            DataTable dtresult = new DataTable();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                string sqlquery = string.Format("SELECT * from vwBankCollection where PeriodMonth={0} and PeriodYear={1} ORDER BY BankName ASC", sumbanks.periodMonth, sumbanks.periodYear);

                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                response.Clear();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand(sqlquery, con);

                    cmd.CommandType = CommandType.Text;

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    //dtresult = response.Tables[0];

                    if (response.Tables[0] != null && response.Tables[0].Rows.Count > 0)
                    {
                        dtresult = response.Tables[0];

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("summary banks View sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        //loggers.Information(response.Tables[0]);
                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }

            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                  .WriteTo.MSSqlServer(connectionString, "Logs")
                  .CreateLogger();
                logger.Fatal($"View Summary Report thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataTable Viewdetails(string strrevenue, long lngPeriodMonth, long lngPeriodYear)
        {
            DataTable dtresult = new DataTable();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                string sqlquery = string.Format("select * from vwCollectionDetails where PeriodMonth={0} and PeriodYear={1} and RevenueName='{2}' ORDER BY PayerName ASC", lngPeriodMonth, lngPeriodYear, strrevenue.Trim().ToString());

                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                response.Clear();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand(sqlquery, con);

                    cmd.CommandType = CommandType.Text;

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    //dtresult = response.Tables[0];

                    if (response.Tables[0] != null && response.Tables[0].Rows.Count > 0)
                    {
                        dtresult = response.Tables[0];

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("summary Agencies View sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        //loggers.Information(response.Tables[0]);
                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }

            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                  .WriteTo.MSSqlServer(connectionString, "Logs")
                  .CreateLogger();
                logger.Fatal($"View Summary Report thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataTable viewBankdetails(string accountnumber, long lngPeriodMonth, long lngPeriodYear)
        {
            DataTable dtresult = new DataTable();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                string sqlquery = string.Format("SELECT * from vwCollectionDetails where PeriodMonth={0} and PeriodYear={1} and AccountNumber='{2}' ORDER BY  PayerName ASC, RevenueName ASC", lngPeriodMonth, lngPeriodYear, accountnumber.Trim().ToString());

                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                response.Clear();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand(sqlquery, con);

                    cmd.CommandType = CommandType.Text;

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    //dtresult = response.Tables[0];

                    if (response.Tables[0] != null && response.Tables[0].Rows.Count > 0)
                    {
                        dtresult = response.Tables[0];

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("summary bank details View sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        //loggers.Information(response.Tables[0]);
                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }

            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                  .WriteTo.MSSqlServer(connectionString, "Logs")
                  .CreateLogger();
                logger.Fatal($"View bank details Report thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataTable ViewAgencydetails(string strAgency, long lngPeriodMonth, long lngPeriodYear)
        {
            DataTable dtresult = new DataTable();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                string sqlquery = string.Format("select  AgencyCode, AgencyName, RevenueCode,RevenueName,SUM(Amount) Amount,StartDate,EndDate,  PeriodYear,PeriodMonth,PeriodName from vwRevenueCollection where PeriodMonth={0} and PeriodYear={1} and AgencyName='{2}' GROUP BY AgencyCode,AgencyName, RevenueCode,RevenueName,StartDate,EndDate,  PeriodYear,PeriodMonth,PeriodName", lngPeriodMonth, lngPeriodYear, strAgency.Trim().ToString());

                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                response.Clear();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand(sqlquery, con);

                    cmd.CommandType = CommandType.Text;

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    //dtresult = response.Tables[0];

                    if (response.Tables[0] != null && response.Tables[0].Rows.Count > 0)
                    {
                        dtresult = response.Tables[0];

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("summary Agencies View sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        //loggers.Information(response.Tables[0]);
                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }

            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                  .WriteTo.MSSqlServer(connectionString, "Logs")
                  .CreateLogger();
                logger.Fatal($"View Summary Report thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataTable viewReversedTransaction(string accountnumber, long lngPeriodMonth, long lngPeriodYear)
        {
            DataTable dtresult = new DataTable();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                string sqlquery = string.Format("select  * from vwreversedtransaction where PeriodMonth={0} and PeriodYear={1} and AccountNumber='{2}' ", lngPeriodMonth, lngPeriodYear, accountnumber.Trim().ToString());

                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                response.Clear();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand(sqlquery, con);

                    cmd.CommandType = CommandType.Text;

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    //dtresult = response.Tables[0];

                    if (response.Tables[0] != null && response.Tables[0].Rows.Count > 0)
                    {
                        dtresult = response.Tables[0];

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("vwreversedtransaction View sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        //loggers.Information(response.Tables[0]);
                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }

            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                  .WriteTo.MSSqlServer(connectionString, "Logs")
                  .CreateLogger();
                logger.Fatal($"View vwreversedtransaction Report thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataTable ViewVaricesAgencies(Summarys sumagency)
        {
            DataTable dtresult = new DataTable();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                string sqlquery = string.Format("select * from dbo.[vw•VariancesbyAgency] where PeriodMonth={0} and PeriodYear={1}", sumagency.periodMonth, sumagency.periodYear);

                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                response.Clear();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand(sqlquery, con);

                    cmd.CommandType = CommandType.Text;

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    //dtresult = response.Tables[0];

                    if (response.Tables[0] != null && response.Tables[0].Rows.Count > 0)
                    {
                        dtresult = response.Tables[0];

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("summary Agencies View sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        //loggers.Information(response.Tables[0]);
                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }

            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                  .WriteTo.MSSqlServer(connectionString, "Logs")
                  .CreateLogger();
                logger.Fatal($"View Summary Report thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataTable ViewMonth(Summarys sumagency)
        {
            DataTable dtresult = new DataTable();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                string sqlquery = string.Format("select * from vwVariancesMonths where PeriodMonth={0} and PeriodYear={1}", sumagency.periodMonth, sumagency.periodYear);

                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                response.Clear();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand(sqlquery, con);

                    cmd.CommandType = CommandType.Text;

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    //dtresult = response.Tables[0];

                    if (response.Tables[0] != null && response.Tables[0].Rows.Count > 0)
                    {
                        dtresult = response.Tables[0];

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("summary Agencies View sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        //loggers.Information(response.Tables[0]);
                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }

            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                  .WriteTo.MSSqlServer(connectionString, "Logs")
                  .CreateLogger();
                logger.Fatal($"View Summary Report thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataTable ViewAgencies(Summarys sumagency)
        {
            DataTable dtresult = new DataTable();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                string sqlquery = string.Format("SELECT AgencyCode,AgencyName,SUM(Amount) Amount,StartDate,EndDate,PeriodName, PeriodMonth, PeriodYear from vwAgencyCollection where PeriodMonth={0} and PeriodYear={1} GROUP BY  AgencyCode,AgencyName,StartDate,EndDate,PeriodName,PeriodMonth,PeriodYear", sumagency.periodMonth, sumagency.periodYear);

                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                response.Clear();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand(sqlquery, con);

                    cmd.CommandType = CommandType.Text;

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    //dtresult = response.Tables[0];

                    if (response.Tables[0] != null && response.Tables[0].Rows.Count > 0)
                    {
                        dtresult = response.Tables[0];

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("summary Agencies View sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        //loggers.Information(response.Tables[0]);
                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }

            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                  .WriteTo.MSSqlServer(connectionString, "Logs")
                  .CreateLogger();
                logger.Fatal($"View Summary Report thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataTable ViewSummary(Summarys marsy)
        {
            DataTable dtresult = new DataTable();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                string sqlquery = string.Format("select * from vwreconcilesummary where PeriodMonth={0} and PeriodYear={1} ORDER BY BankName ASC", marsy.periodMonth, marsy.periodYear);

                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                response.Clear();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand(sqlquery, con);

                    cmd.CommandType = CommandType.Text;

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    //dtresult = response.Tables[0];

                    if (response.Tables[0] != null && response.Tables[0].Rows.Count > 0)
                    {
                        dtresult = response.Tables[0];

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("summary view sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        //loggers.Information(response.Tables[0]);
                        loggers.Information(serializeReponse);
                    }
                    //else
                    //{
                    //    var logger = new LoggerConfiguration()
                    //        .WriteTo.MSSqlServer(connectionString, "Logs")
                    //        .CreateLogger();
                    //    logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                    //    var serializeReponse = JsonConvert.SerializeObject(response);
                    //    var loggers = new LoggerConfiguration()
                    //        .WriteTo.MSSqlServer(connectionString, "Logs")
                    //        .CreateLogger();
                    //    loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                    //    loggers.Information(serializeReponse);
                    //}
                    con.Close();
                }

            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                  .WriteTo.MSSqlServer(connectionString, "Logs")
                  .CreateLogger();
                logger.Fatal($"View Summary Report thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataTable ViewPostingRequest(long reconcileId)
        {
            DataTable dtresult = new DataTable();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                response.Clear();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spViewPostingRequest", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@recperid", reconcileId);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response.Tables[1];

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Alocate Transaction sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                                    .WriteTo.MSSqlServer(connectionString, "Logs")
                                    .CreateLogger();
                logger.Fatal($"Save Bank Statement thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public UsersList GetUserid(long userid)
        {
            UsersList usersList = new UsersList();

            var connectionString = this.GetConnection();

            SqlDataAdapter _adp;

            System.Data.DataSet response = new System.Data.DataSet();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("SpUserwithID", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Userid", userid);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow rows in response.Tables[0].Rows)
                        {
                            usersList.Address = rows["Address"].ToString();
                            usersList.Description = rows["Description"].ToString(); ;
                            usersList.EmailAddress = rows["EmailAddress"].ToString(); ;
                            usersList.Firstname = rows["Firstname"].ToString(); ;
                            usersList.IsActive = Convert.ToBoolean(rows["IsActive"].ToString());
                            usersList.Mobile = rows["Mobile"].ToString(); ;
                            usersList.Surname = rows["Surname"].ToString(); ;
                            usersList.UserId = Convert.ToInt64(rows["UserId"].ToString());
                            usersList.Usercode = rows["TypeCode"].ToString(); ;
                        }
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get User  with id- {userid} ");
                    loggers.Information(JsonConvert.SerializeObject(usersList));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                  .WriteTo.MSSqlServer(connectionString, "Logs")
                  .CreateLogger();
                logger.Fatal($"Get user list with ID thrown an error - {e.Message}");
            }

            return usersList;
        }

        public Bank GetBankAccountId(long accountid)
        {
            var connectionString = this.GetConnection();

            SqlDataAdapter _adp;

            System.Data.DataSet response = new System.Data.DataSet();

            Bank _bank = new Bank();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spBankAccountWithID", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@AccountID", accountid);

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    if (response.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow rows in response.Tables[0].Rows)
                        {
                            _bank.AccountName = rows["AccountName"].ToString();
                            _bank.AccountNumber = rows["AccountNumber"].ToString();
                            _bank.OpeningBal = Convert.ToDecimal(rows["OpeningBal"].ToString());
                            _bank.BankCode = rows["BankCode"].ToString();
                            _bank.code = rows["BankType"].ToString();
                            _bank.CurrencyID = Convert.ToInt32(rows["CurrencyID"].ToString());
                            _bank.IsLink = Convert.ToBoolean(rows["IsLink"].ToString());
                            //_bank.code = rows["Code"].ToString();
                        }
                    }

                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Get Bank Account with id- {accountid} ");
                    loggers.Information(JsonConvert.SerializeObject(_bank));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Get Bank Account with ID thrown an error - {e.Message}");
            }

            return _bank;
        }

        public int EditUsersAccount(UsersList users)
        {
            var connectionString = this.GetConnection();

            int count = 0;

            SqlDataAdapter _adp;

            System.Data.DataSet response = new System.Data.DataSet();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spUsersupdatewithID", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Surname", users.Surname);
                    cmd.Parameters.AddWithValue("@Firstname", users.Firstname);
                    cmd.Parameters.AddWithValue("@Mobile", users.Mobile);
                    cmd.Parameters.AddWithValue("@TypeCode", users.Usercode);
                    cmd.Parameters.AddWithValue("@Address", users.Address);
                    cmd.Parameters.AddWithValue("@EmailAddress", users.EmailAddress);
                    cmd.Parameters.AddWithValue("@UserId", users.UserId);
                    //cmd.Parameters.AddWithValue("@IsActive", bank.IsActive);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {
                        count = -1;
                    }
                    else
                    {
                        count = 1;
                    }
                    //cmd.ExecuteNonQuery();
                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Edit User Account with id- {users.UserId} ");
                    loggers.Information(JsonConvert.SerializeObject(count));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                   .WriteTo.MSSqlServer(connectionString, "Logs")
                   .CreateLogger();
                logger.Fatal($"Edit User Account thrown an error - {e.Message}");
            }
            return count;
        }

        public int EditBankAccunt(Bank bank)
        {
            var connectionString = this.GetConnection();

            int count = 0;

            SqlDataAdapter _adp;

            System.Data.DataSet response = new System.Data.DataSet();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spBankAccountUpdateWithID", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@AccountName", bank.AccountName);
                    cmd.Parameters.AddWithValue("@AccountNumber", bank.AccountNumber);
                    cmd.Parameters.AddWithValue("@OpeningBal", bank.OpeningBal);
                    cmd.Parameters.AddWithValue("@BankCode", bank.BankCode);
                    cmd.Parameters.AddWithValue("@CurrencyID", bank.CurrencyID);
                    cmd.Parameters.AddWithValue("@IsLink", bank.IsLink);
                    cmd.Parameters.AddWithValue("@banktypecode", bank.code);
                    //cmd.Parameters.AddWithValue("@IsActive", bank.IsActive);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {
                        count = -1;
                    }
                    else
                    {
                        count = 1;
                    }
                    //cmd.ExecuteNonQuery();
                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Edit Transaction with id- {bank.AccountID} ");
                    loggers.Information(JsonConvert.SerializeObject(count));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Edit Transaction thrown an error - {e.Message}");
            }
            return count;
        }

        public string Insertusertoken(string emailaddress)
        {
            DataSet dtresult = new DataSet();
            var count = 0;
            dtresult.Clear();
            string strtoken = string.Empty;
            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spDoCreateToken", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@emailaddress", emailaddress);

                    //cmd.Parameters.AddWithValue("@strtoken", strtoken);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response;
                        strtoken = response.Tables[1].Rows[0]["strtoken"].ToString();
                        count = 1;
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Token Insert sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        count = -1;
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                var logger = new LoggerConfiguration()
                                                    .WriteTo.MSSqlServer(connectionString, "Logs")
                                                    .CreateLogger();
                logger.Fatal($"token inseration thrown an error - {ex.Message}");
            }
            return strtoken;
        }

        public string validateuserotp(string emailaddress, string strtoken)
        {

            string count = String.Empty;

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spOTPValidation", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@emailaddress", emailaddress);

                    cmd.Parameters.AddWithValue("@strtoken", strtoken);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        //dtresult = response;

                        count = response.Tables[0].Rows[0]["returnCode"].ToString();

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Token Insert sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        count = response.Tables[0].Rows[0]["returnCode"].ToString();

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {


                var logger = new LoggerConfiguration()
                                                    .WriteTo.MSSqlServer(connectionString, "Logs")
                                                    .CreateLogger();
                logger.Fatal($"token inseration thrown an error - {ex.Message}");
            }
            return count;
        }

        public string posttransactiondisapprove(string emailaddress, double reconcileId)
        {
            string count = String.Empty;

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spPostingTransactionDisapprove", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Useremail", emailaddress);

                    cmd.Parameters.AddWithValue("@recperid", reconcileId);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        //dtresult = response;

                        count = response.Tables[0].Rows[0]["returnCode"].ToString();

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Token Insert sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        count = response.Tables[0].Rows[0]["returnCode"].ToString();

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {


                var logger = new LoggerConfiguration()
                                                    .WriteTo.MSSqlServer(connectionString, "Logs")
                                                    .CreateLogger();
                logger.Fatal($"token inseration thrown an error - {ex.Message}");
            }
            return count;
        }

        public string posttransactionapproval(string emailaddress, double reconcileId)
        {
            string count = String.Empty;

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spPostingTransactionApprove", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Useremail", emailaddress);

                    cmd.Parameters.AddWithValue("@recperid", reconcileId);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        //dtresult = response;

                        count = response.Tables[0].Rows[0]["returnCode"].ToString();

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Token Insert sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        count = response.Tables[0].Rows[0]["returnCode"].ToString();

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {


                var logger = new LoggerConfiguration()
                                                    .WriteTo.MSSqlServer(connectionString, "Logs")
                                                    .CreateLogger();
                logger.Fatal($"token inseration thrown an error - {ex.Message}");
            }
            return count;
        }

        public DataTable ViewBankAllocation(long reconcileId)
        {
            DataTable dtresult = new DataTable();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                response.Clear();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spViewBankAllocation", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@recperid", reconcileId);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response.Tables[1];

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Alocate Transaction sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                                    .WriteTo.MSSqlServer(connectionString, "Logs")
                                    .CreateLogger();
                logger.Fatal($"Save Bank Statement thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public DataTable Viewunpostedcollection(long reconcileId)
        {
            DataTable dtresult = new DataTable();

            dtresult.Clear();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                response.Clear();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spViewunpostedcollection", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@recperid", reconcileId);

                    con.Open();

                    cmd.CommandTimeout = 0;
                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() == "00")
                    {
                        dtresult = response.Tables[1];

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Alocate Transaction sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    else
                    {
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());
                        loggers.Information(serializeReponse);
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                                    .WriteTo.MSSqlServer(connectionString, "Logs")
                                    .CreateLogger();
                logger.Fatal($"Save Bank Statement thrown an error - {e.Message}");
            }

            return dtresult;
        }

        public ResponseInfo ApproveNormaliseRecord(string modifyid, string usermail)
        {
            ResponseInfo responses = new ResponseInfo();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spDoApproveNormaliseRec", con);

                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.AddWithValue("@Refnumber", modifyid);

                    cmd.Parameters.AddWithValue("@Userid", usermail);


                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {

                        responses.StatusCode = response.Tables[0].Rows[0]["returnCode"].ToString();
                        responses.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString();

                    }
                    else
                    {
                        responses.StatusCode = response.Tables[0].Rows[0]["returnCode"].ToString();
                        responses.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString();

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information(response.Tables[0].Rows[0]["returnMessage"].ToString());

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information(responses.StatusMessage);
                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }
            }
            catch (Exception e)
            {
                responses.StatusCode = e.HResult.ToString();
                responses.StatusMessage = e.Message.ToString();


                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Normalise Approve thrown an error - {e.Message}");
            }
            return responses;
        }

        public ResponseInfo ApproveReclassified(long modifyid, string usermail)
        {
            ResponseInfo responses = new ResponseInfo();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spDoModifyApproveRecord", con);

                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.AddWithValue("@modifyid", modifyid);

                    cmd.Parameters.AddWithValue("@Userid", usermail);


                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {

                        responses.StatusCode = response.Tables[0].Rows[0]["returnCode"].ToString();
                        responses.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString();

                    }
                    else
                    {
                        responses.StatusCode = response.Tables[0].Rows[0]["returnCode"].ToString();
                        responses.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString();

                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Bank Statement Saved sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information("Save Bank Statement successful");
                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }
            }
            catch (Exception e)
            {
                responses.StatusCode = e.HResult.ToString();
                responses.StatusMessage = e.Message.ToString();


                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Save Bank Statement thrown an error - {e.Message}");
            }
            return responses;
        }

        public ResponseInfo RequestComplet(long reconcileId, string usermail)
        {
            ResponseInfo responses = new ResponseInfo();

            var connectionString = this.GetConnection();

            try
            {
                //List<RegResponseInfo<Fecthpayer>> regResponse = new List<RegResponseInfo<Fecthpayer>>();
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spUpdateReconcileRequest", con);

                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.AddWithValue("@RecperID", reconcileId);

                    cmd.Parameters.AddWithValue("@usermail", usermail);


                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {
                        //count = -1;

                        responses.StatusCode = response.Tables[0].Rows[0]["returnCode"].ToString();
                        responses.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString();
                        //bc.Reconcileid = Convert.ToInt64(response.Tables[0].Rows[0]["Reconcileid"].ToString());
                    }
                    else
                    {
                        responses.StatusCode = response.Tables[0].Rows[0]["returnCode"].ToString();
                        responses.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString();
                        //bc.Reconcileid = Convert.ToInt64(response.Tables[1].Rows[0]["Reconcileid"].ToString());

                        //count = 1;
                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Bank Statement Saved sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information("Save Bank Statement successful");
                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }
            }
            catch (Exception e)
            {
                responses.StatusCode = e.HResult.ToString();
                responses.StatusMessage = e.Message.ToString();
                //responses.ResponseObject = null;

                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Save Bank Statement thrown an error - {e.Message}");
            }
            return responses;
        }

        public ResponseInfo DeleteReconcileperiod(long reconcileId)
        {
            ResponseInfo responses = new ResponseInfo();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spDeleteReconcileDate", con);

                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.AddWithValue("@RecperID", reconcileId);


                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {

                        responses.StatusCode = response.Tables[0].Rows[0]["returnCode"].ToString();
                        responses.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString();

                    }
                    else
                    {
                        responses.StatusCode = response.Tables[0].Rows[0]["returnCode"].ToString();
                        responses.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString();


                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Delete Reconcile Period sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);
                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        loggers.Information("Delete Reconcile Period successful");
                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }
            }
            catch (Exception e)
            {
                responses.StatusCode = e.HResult.ToString();
                responses.StatusMessage = e.Message.ToString();
                //responses.ResponseObject = null;

                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Delete Reconcile Period thrown an error - {e.Message}");
            }
            return responses;
        }

        public ResponseInfo Deletereconciletransactionlist(long reconcileId)
        {
            ResponseInfo responses = new ResponseInfo();

            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spDeleteReconcilelistwithid", con);

                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.AddWithValue("@RecperID", reconcileId);


                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();

                    _adp = new SqlDataAdapter(cmd);

                    _adp.Fill(response);

                    if (response.Tables[0].Rows[0]["returnCode"].ToString() != "00")
                    {

                        responses.StatusCode = response.Tables[0].Rows[0]["returnCode"].ToString();

                        responses.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString();

                    }
                    else
                    {
                        responses.StatusCode = response.Tables[0].Rows[0]["returnCode"].ToString();

                        responses.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString();


                        var logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();
                        logger.Information("Delete Reconcile Transaction Lists sucessfully");

                        var serializeReponse = JsonConvert.SerializeObject(response);

                        var loggers = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(connectionString, "Logs")
                            .CreateLogger();

                        loggers.Information("Delete Reconcile Transaction Lists successful");

                        loggers.Information(serializeReponse);
                    }

                    con.Close();
                }
            }
            catch (Exception e)
            {
                responses.StatusCode = e.HResult.ToString();
                responses.StatusMessage = e.Message.ToString();
                //responses.ResponseObject = null;

                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();
                logger.Fatal($"Delete Reconcile Transaction Lists thrown an error - {e.Message}");
            }
            return responses;
        }

        public void DoReemsPush()
        {
            var connectionString = this.GetConnection();

            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spdoGetRecordtoReems", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);


                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Reems Push To Successfully ");
                    //loggers.Information(JsonConvert.SerializeObject(count));
                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();

                logger.Fatal($"Do Reems Push thrown an error - {e.Message}");
            }
        }
    }
}
