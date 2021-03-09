using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.Logging;
//using Oracle.DataAccess.Client;
using Movilway.API.Data;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    public static class Utils
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(Utils));

        public static DateTime FromEpochToLocalTime(long epochTimeStamp)
        {
            var newDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            newDateTime = newDateTime.AddMilliseconds(epochTimeStamp);
            newDateTime = newDateTime.ToLocalTime();
            return (newDateTime);
        }

        public static int FromDateTimeToEpoch(DateTime normalDateTime)
        {
            var t = (normalDateTime.ToUniversalTime() - new DateTime(1970, 1, 1));
            return (int)t.TotalSeconds;
        }

        public static String GenerateHash(String sessionID, String user, String password)
        {
            var sResult = GetSHA1(user.ToLower() + password);
            sResult = (GetSHA1(sessionID + sResult.ToLower())).ToUpper();
            return sResult;
        }

        public static string GetSHA1(string str)
        {
            var sha1 = SHA1.Create();
            var encoding = new ASCIIEncoding();
            byte[] stream = null;
            var sb = new StringBuilder();
            stream = sha1.ComputeHash(encoding.GetBytes(str));
            foreach (var t in stream)
                sb.AppendFormat("{0:x2}", t);
            return sb.ToString();
        }

        public static string GetSessionForUserKey(String username, String password)
        {
            return "sessionForUser:" + GetSHA1(username) + ":" + GetSHA1(password);
        }

        public static string GetUserForSessionKey(String sessionID)
        {
            return "userForSession:" + GetSHA1(sessionID);
        }

        public static MoviPinDetails GetMoviPinDetails(int transactionNumber)
        {
             return null;
            /*
           var oraConnection = new OracleConnection(ConfigurationManager.ConnectionStrings["utibaMarket"].ConnectionString);
            MoviPinDetails details = null;
            try
            {
                var query = String.Format(( "select amount.value-nvl(used.value,0)\"remaining\"," +
                                            "   to_char(ar.reference)\"phonenumber\"," +
                                            "   to_timestamp('1970-01-01', 'YYYY-MM-DD') + (expiry.value/86400000)\"expirydate\"," +
                                            "   to_number(amount.value)\"amount\"," +
                                            "   pin.value\"movipin\" " +
                                            "from trans t " +
                                            "join trans_data pin on t.id=pin.transid and pin.td_key='couponid' " +
                                            "join trans_data expiry on t.id=expiry.transid and expiry.td_key='expiry' " +
                                            "join trans_party tp on t.id=tp.transid and tp.type_=111 " +
                                            "join agent_ref ar on ar.id=tp.agentid " +
                                            "join trans tc on t.id=tc.parent and tc.type_='coupon' " +
                                            "join trans_data amount on t.id=amount.transid and amount.td_key='amount' " +
                                            "left join trans_data used on tc.id=used.transid and used.td_key='used_amount' " +
                                            "where t.type_='createcoupon' and t.id={0}"), transactionNumber);

                oraConnection.Open();

                var command = new OracleCommand(query, oraConnection);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        details = new MoviPinDetails()
                        {
                            Number = reader.GetString(4),
                            IsValid = reader.GetDecimal(0) > 0
                        };
                        if (details.IsValid == null || !details.IsValid.Value)
                            continue;
                        details.Agent = reader.GetString(1);
                        details.InitialAmount = reader.GetDecimal(3);
                        details.RemainingAmount = reader.GetDecimal(0);
                        details.ExpiryDate = reader.GetDateTime(2);
                    }
                else
                {
                    details = new MoviPinDetails()
                    {
                        Number = "000000000000",
                        IsValid = false
                    };
                }
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el MoviPin en la base de datos de Utiba"));
                throw;
            }
            finally
            {
                oraConnection.Close();
            }
            return (details);*/
        }

        public static MoviPinDetails GetMoviPinDetails(String moviPinNumber)
        {
            return null;
            /*
            var oraConnection = new OracleConnection(ConfigurationManager.ConnectionStrings["utibaMarket"].ConnectionString);
            MoviPinDetails details = null;
            try
            {
                var query =
                    String.Format(
                        ("select amount.value-nvl(used.value,0)\"remaining\"," +
                                    "to_char(ar.reference)\"phonenumber\"," +
                                    "to_timestamp('1970-01-01', 'YYYY-MM-DD') + (expiry.value/86400000)\"expirydate\"," +
                                    "to_number(amount.value)\"amount\" " +
                            "from trans t " +
                            "join trans_data pin on t.parent=pin.transid and pin.td_key='couponid' and pin.value='{0}' " +
                            "join trans_data amount on t.id=amount.transid and amount.td_key='amount' " +
                            "join trans_data expiry on t.parent=expiry.transid and expiry.td_key='expiry' " +
                            "join trans_party tp on tp.transid=t.parent and tp.type_=111 " +
                            "join agent_ref ar on ar.id=tp.agentid " +
                            "left join trans_data used on t.id=used.transid and used.td_key='used_amount'"), moviPinNumber);

                oraConnection.Open();

                var command = new OracleCommand(query, oraConnection);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                    while(reader.Read())
                    {
                        details = new MoviPinDetails()
                                      {
                                          Number = moviPinNumber,
                                          IsValid = reader.GetDecimal(0) > 0
                                      };
                        if (details.IsValid == null || !details.IsValid.Value)
                            continue;
                        details.Agent = reader.GetString(1);
                        details.InitialAmount = reader.GetDecimal(3);
                        details.RemainingAmount = reader.GetDecimal(0);
                        details.ExpiryDate = reader.GetDateTime(2);
                    }
                else
                {
                    details = new MoviPinDetails()
                                  {
                                      Number = moviPinNumber,
                                      IsValid = false
                                  };
                }
            }
            catch (Exception e)
            {
                logger.ErrorLow(()=> TagValue.New().Exception(e).Message("Error trantando de consultar el MoviPin en la base de datos de Utiba"));
                throw;
            }
            finally
            {
                oraConnection.Close();
            }
            return (details);*/
        }

        public static SummaryItems SalesSummary(String agentReference, DateTime summaryDate, int walletType)
        {
            return null;
            /*
            var oraConnection = new OracleConnection(ConfigurationManager.ConnectionStrings["utibaMarket"].ConnectionString);
            SummaryItems summaries = new SummaryItems();
            try
            {
                //var query =
                //    String.Format(
                //        (@"select Fecha""Fecha"", Tipo ""Tipo"", count(*)""Aprobadas"", SUM(Monto)""Monto Aprobado""
                //            from (select to_char(t.created,'YYYY-MM-DD') as Fecha,t.type_ || (CASE WHEN t.type_='buy' THEN ' - ' || td.value END) as Tipo, tp.amount as Monto
                //                    from trans t 
                //                    left join trans_data td on t.id=td.transid and td_key='mno'
                //                    join trans_party tp on t.id=tp.transid and tp.amount>0
                //                    join agent_ref ar on t.agentid=ar.id and ar.reference='{0}'
                //                    join trans_party tp2 on t.id=tp2.transid and tp2.agentid=ar.id
                //                    join account ac on ac.id=tp2.accountid
                //                    where  to_date(to_char(t.created,'YYYY-MM-DD'),'YYYY-MM-DD') >= to_date('{1}','YYYY-MM-DD')
                //                    and to_date(to_char(t.created,'YYYY-MM-DD'),'YYYY-MM-DD') < to_date('{2}','YYYY-MM-DD')
                //                    and t.type_ in ('adjustment','bonus','buy','buy_stock','fee','prepaid_stock','transfer','sell','cashin','cashout') 
                //                    and t.result=0 and t.state=2 and ac.type_={3})
                //            group by Fecha, Tipo"), agentReference, summaryDate.ToString("yyyy-MM-dd"), summaryDate.AddDays(1).ToString("yyyy-MM-dd"), walletType);

                var query =
                    String.Format(
                        (@" WITH myTrans as (
                                select t.id,to_char(t.created,'YYYY-MM-DD') as Fecha,t.type_ || (CASE WHEN t.type_='buy' THEN ' - ' || td.value END) as Tipo  from trans t 
                                left join trans_data td on t.id=td.transid and td_key='mno'
                                join agent_ref ar on t.agentid=ar.id and ar.reference='{0}'
                                where  t.last_modified >= to_date('{1}','YYYY-MM-DD')
                                        and t.last_modified < to_date('{2}','YYYY-MM-DD') 
                                        and t.state=2 
                                        and t.type_ in ('adjustment','bonus','buy','buy_stock','fee','prepaid_stock','transfer','sell','cashin','cashout') 
                                        and t.result=0  
                            ), myParty as (
                                select transid,amount as Monto
                                from trans_party 
                                where last_modified >= to_date('{1}','YYYY-MM-DD') 
                                        and last_modified < to_date('{2}','YYYY-MM-DD')
                                        and amount>0 
                            )

                            select t.Fecha,t.Tipo, count(1) as Aprobadas,sum(p.Monto) as Monto from 
                            myTrans t 
                            join myParty p
                            on t.id=p.transid
                            group by t.Fecha,t.Tipo"), agentReference, summaryDate.ToString("yyyy-MM-dd"), summaryDate.AddDays(1).ToString("yyyy-MM-dd"), walletType);

                logger.InfoHigh(query);
                oraConnection.Open();

                var command = new OracleCommand(query, oraConnection);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        summaries.Add(new SummaryItem()
                        {
                            TransactionType = GetTransactionTypeName(reader.GetString(1).Split('-')[0]) + (reader.GetString(1).Split('-').Count() > 1 ? " - " + reader.GetString(1).Split('-')[1].Trim() : ""),
                            TotalAmount = reader.GetDecimal(3),
                            TransactionCount = int.Parse(reader.GetDecimal(2).ToString())
                        });
                    }
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el resúmen en la base de datos de Utiba"));
                throw;
            }
            finally
            {
                oraConnection.Close();
            }
            return (summaries);*/
        }

        private static TransactionType ConvertToTransactionType(string transactionType)
        {
            switch (transactionType)
            {
                case "buy":
                    return TransactionType.buy;
                case "buystock" :
                    return TransactionType.buystock;
                case "cashin" :
                    return TransactionType.cashin;
                case "cashout" :
                    return TransactionType.cashout;
                case "createcoupon" :
                    return TransactionType.createcoupon;
                case "paystock" :
                    return TransactionType.paystock;
                case "topup" :
                    return TransactionType.topup;
                case "transfercoupon" :
                    return TransactionType.transfercoupon;
                case "transfer" :
                    return TransactionType.transfer;
                case "transferstock" :
                    return TransactionType.transferstock;
                case "sell":
                    return TransactionType.sell;
            }
            return TransactionType.NotSpecified;
        }

        public static String GetTransactionTypeName(string transactionType)
        {
            switch (transactionType.ToLower().Trim())
            {
                case "bonus":
                    return "Bono";
                case "coupontransfer":
                    return "Transferencia de cupón";
                case "fee":
                    return "Cargo";
                case "account_transfer":
                    return "Transferencia de cuenta";
                case "buy":
                    return "Recarga";
                case "buy_stock":
                    return "Compra de stock";
                case "cashin":
                    return "Recepción de efectivo";
                case "cashout":
                    return "Retiro de efectivo";
                case "createcoupon":
                    return "Creación de cupón";
                case "paystock":
                    return "Pago de stock";
                case "topup":
                    return "Recarga";
                case "transfercoupon":
                    return "Redención de cupón";
                case "transfer":
                    return "Transferencia";
                case "transfer_stock":
                    return "Transferencia de stock";
                case "sell":
                    return "Venta";
            }
            return "Desconocido (" + transactionType + ")";
        }

        public static WalletType GetWalletType(string walletType)
        {
            switch (walletType.ToLower())
            {
                case "stock":
                    return WalletType.Stock;
                case "ewallet":
                    return WalletType.eWallet;
                case "points":
                    return WalletType.Points;
                case "debt":
                    return WalletType.Debt;
                case "airtime":
                    return WalletType.AirTime;
            }
            //System.Threading.Thread.Sleep(10000);
            return WalletType.NotSpecified;
        }

        public static int BuildResponseCode(int responseCode, string nameSpace)
        {
            return responseCode == 0 ? 0 : int.Parse(responseCode + "" + GetNameSpaceNumber(nameSpace));
        }

        private static string GetNameSpaceNumber(string nameSpace)
        {
            foreach (string item in ConfigurationManager.AppSettings["NameSpaces"].Split(';'))
                if (item.Split(',')[0] == nameSpace.ToLower())
                    return item.Split(',')[1];
            return "0";
        }
    }
}