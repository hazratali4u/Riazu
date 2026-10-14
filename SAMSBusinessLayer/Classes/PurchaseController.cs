using System;
using System.Data;
using SAMSCommon.Classes;
using SAMSDataAccessLayer.Classes;
using SAMSDatabaseLayer.Classes;
using SAMSDatabaseLayer.InputClasses;
using System.Data.SqlClient;
using SAMSDatabaseLayer.AtlasClasses;

namespace SAMSBusinessLayer.Classes
{
    /// <summary>
    /// Class For Purchase, TranferOut, Purchase Return, TranferIn And Damage Related Tasks
    /// <example>
    /// <list type="bullet">
    /// <item>
    /// Insert Purchase, TranferOut, Purchase Return, TranferIn And Damage
    /// </item>
    /// <term>
    /// Update Purchase, TranferOut, Purchase Return, TranferIn And Damage
    /// </term>
    /// <item>
    /// Get Purchase, TranferOut, Purchase Return, TranferIn And Damage
    /// </item>
    /// </list>
    /// </example>
    /// </summary>
    public class PurchaseController
    {
        #region Constructor

        /// <summary>
        /// Constructor for PurchaseController
        /// </summary>
        public PurchaseController()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#endregion

        #region Private Variables
        
        IDbTransaction mTransaction;
        IDbConnection mConnection;

        #endregion

        #region Public Methods

        #region Select

        /// <summary>
        /// Get Purchase, TranferOut, Purchase Return, TranferIn And Damage Detail
        /// </summary>
        /// <remarks>
        /// Returns Purchase, TranferOut, Purchase Return, TranferIn And Damage Detail as Datatable
        /// </remarks>
        /// <param name="p_DISTRIBUTOR_ID">Location</param>
        /// <param name="p_PURCHASE_MASTER_ID">Purchase</param>
        /// <param name="PConnection">Connection</param>
        /// <param name="PTransaction">Transaction</param>
        /// <returns>Purchase, TranferOut, Purchase Return, TranferIn And Damage Detail as Datatable</returns>
        public DataTable SelectPrivousePurchaseDetail(int p_DISTRIBUTOR_ID, long p_PURCHASE_MASTER_ID, IDbConnection PConnection, IDbTransaction PTransaction)
        {
            try
            {
                spSelectPURCHASE_DETAIL mPurchaseDetail = new spSelectPURCHASE_DETAIL();
                mPurchaseDetail.Connection = PConnection;
                mPurchaseDetail.Transaction = PTransaction;
                mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseDetail.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                DataTable dt = mPurchaseDetail.ExecuteTable();
                return dt;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
        }

        public DataTable GetPurchaseOrder(long pPurchaseMasterId, int pDistributorId, int pUserId, DateTime pDocumentDate)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                GetPurchaseOrder mReserve = new GetPurchaseOrder();
                mReserve.Connection = mConnection;
                mReserve.DISTRIBUTOR_ID = pDistributorId;

                mReserve.PURCHASE_MASTER_ID = pPurchaseMasterId;
                mReserve.USER_ID = pUserId;
                mReserve.DOCUMENT_DATE = pDocumentDate;
                DataTable dt = mReserve.ExecuteTable();
                return dt;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }


        public long UpdatePurchaseDocument2(long pPurchaseMasterId, int pDistributorId, int pPrincipalId, Decimal pTotalAmount, int pTypeId, DateTime pDocumentDate, DataTable dtPurchaseDetail, int pPosting, int pUserId, string pREMARKS)
        {
            try
            {


                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                spUpdatePURCHASE_MASTER mPurchaseMaster = new spUpdatePURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.PURCHASE_MASTER_ID = pPurchaseMasterId;
                mPurchaseMaster.DISTRIBUTOR_ID = pDistributorId;
                mPurchaseMaster.TYPE_ID = pTypeId;
                mPurchaseMaster.ORDER_NUMBER = "0";
                mPurchaseMaster.DOCUMENT_DATE = pDocumentDate;
                mPurchaseMaster.SOLD_FROM = pPrincipalId;
                mPurchaseMaster.TOTAL_AMOUNT = pTotalAmount;
                mPurchaseMaster.SOLD_TO = 0;
                mPurchaseMaster.USER_ID = pUserId;
                mPurchaseMaster.TIME_STAMP = DateTime.Now;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = pPosting;
                mPurchaseMaster.BUILTY_NO = "N/A";
                mPurchaseMaster.POSTING = pPosting;
               // mPurchaseMaster.REMARKS = pREMARKS;
                mPurchaseMaster.ExecuteQuery();
                DataTable dt = SelectPrivousePurchaseDetail(pDistributorId, pPurchaseMasterId, mConnection, mTransaction);

                foreach (DataRow dr in dt.Rows)
                {
                    UspUpdatePurchaseDetailStock mPurchaseStock = new UspUpdatePurchaseDetailStock();
                    mPurchaseStock.Connection = mConnection;
                    mPurchaseStock.Transaction = mTransaction;
                    mPurchaseStock.TYPEID = pTypeId;
                    mPurchaseStock.DISTRIBUTOR_ID = pDistributorId;
                    mPurchaseStock.PURCHASE_DETAIL_ID = long.Parse(dr["PURCHASE_DETAIL_ID"].ToString());
                    mPurchaseStock.PURCHASE_MASTER_ID = pPurchaseMasterId;
                    mPurchaseStock.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                   // mPurchaseStock.document_date = pDocumentDate;
                    mPurchaseStock.ExecuteQuery();
                }

                #region insert Purchase Detail
                spInsertPURCHASE_DETAIL mPurchaseDetail = new spInsertPURCHASE_DETAIL();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = pDistributorId;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                  //  mPurchaseDetail.RACK_NO = dr["RACK_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                   // mPurchaseDetail.TRADE_PRICE = decimal.Parse(dr["TRADE_PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["FREE_SKU"].ToString());
                   // mPurchaseDetail.ORDER_QUANTITY = decimal.Parse(dr["QUANTITY"].ToString());
                    mPurchaseDetail.FREE_SKU = 0;
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;

                   // mPurchaseDetail.DATE_EFFECTED = Convert.ToDateTime(dr["DATE_EFFECTED"]);
                    mPurchaseDetail.TIME_STAMP = pDocumentDate;
                    mPurchaseDetail.ExecuteQuery();



                    #endregion

                    UspProcessSKU_Prices mSkuPrices = new UspProcessSKU_Prices();

                    mSkuPrices.Connection = mConnection;
                    mSkuPrices.Transaction = mTransaction;

                    mSkuPrices.DISTRIBUTOR_ID = pDistributorId;
                    mSkuPrices.DISTRIBUTOR_PRICE = mPurchaseDetail.PRICE;
                    mSkuPrices.SKU_ID = mPurchaseDetail.SKU_ID;
                    mSkuPrices.RETAIL_PRICE = 0;

                 //   mSkuPrices.TRADE_PRICE = mPurchaseDetail.TRADE_PRICE;//p_Trade_Price;
                    mSkuPrices.DISTRIBUTOR_DISCOUNT = 0;
                    mSkuPrices.TIME_STAMP = System.DateTime.Now;
                    mSkuPrices.LASTUPDATE_DATE = System.DateTime.Now;
                 //   mSkuPrices.DATE_EFFECTED = mPurchaseDetail.DATE_EFFECTED;

                    mSkuPrices.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = pPrincipalId;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = 0;
                   // mStockUpdate.BATCHNO = mPurchaseDetail.RACK_NO;
                    mStockUpdate.ExecuteQuery();


                }


                mTransaction.Commit();
                return mPurchaseMaster.PURCHASE_MASTER_ID;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();

                return -2;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        public long InsertPurchaseDocument2(long pPoId, int pDistributorId, int pPrincipalId, Decimal pTotalAmount, int pTypeId, DateTime pDocumentDate, bool pIsDelete, DataTable dtPurchaseDetail, int pPosting, int pUserId, string pREMARKS)
        {
            try
            {


                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);
                spInsertPURCHASE_MASTER mPurchaseMaster = new spInsertPURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.DISTRIBUTOR_ID = pDistributorId;
                mPurchaseMaster.TYPE_ID = pTypeId;
                mPurchaseMaster.ORDER_NUMBER = "0";
                mPurchaseMaster.DOCUMENT_DATE = pDocumentDate;
                mPurchaseMaster.SOLD_FROM = pPrincipalId;
                mPurchaseMaster.TOTAL_AMOUNT = pTotalAmount;
                mPurchaseMaster.SOLD_TO = 0;
                mPurchaseMaster.USER_ID = pUserId;
                mPurchaseMaster.TIME_STAMP = DateTime.Now;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = pPosting;
                mPurchaseMaster.PRINCIPAL_ID = pPrincipalId;
                mPurchaseMaster.BUILTY_NO = "N/A";
              //  mPurchaseMaster.PO_ID = pPoId;
              //  mPurchaseMaster.REMARKS = pREMARKS;
                mPurchaseMaster.ExecuteQuery();
                spInsertPURCHASE_DETAIL mPurchaseDetail = new spInsertPURCHASE_DETAIL();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = pDistributorId;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = "0";
                   // mPurchaseDetail.TRADE_PRICE = decimal.Parse(dr["TRADE_PRICE"].ToString());
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["FREE_SKU"].ToString());
                   // mPurchaseDetail.ORDER_QUANTITY = decimal.Parse(dr["QUANTITY"].ToString());
                    mPurchaseDetail.FREE_SKU = 0;
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;

                    //mPurchaseDetail.DATE_EFFECTED = Convert.ToDateTime(dr["DATE_EFFECTED"]);
                    mPurchaseDetail.TIME_STAMP = pDocumentDate;
                    mPurchaseDetail.ExecuteQuery();

                    UspProcessSKU_Prices mSkuPrices = new UspProcessSKU_Prices();

                    mSkuPrices.Connection = mConnection;
                    mSkuPrices.Transaction = mTransaction;
                    mSkuPrices.DISTRIBUTOR_ID = pDistributorId;
                    mSkuPrices.DISTRIBUTOR_PRICE = mPurchaseDetail.PRICE;
                    mSkuPrices.SKU_ID = mPurchaseDetail.SKU_ID;
                    mSkuPrices.RETAIL_PRICE = 0;

                    //mSkuPrices.TRADE_PRICE = mPurchaseDetail.TRADE_PRICE;//p_Trade_Price;
                    mSkuPrices.DISTRIBUTOR_DISCOUNT = 0;
                    mSkuPrices.TIME_STAMP = DateTime.Now;
                    mSkuPrices.LASTUPDATE_DATE = DateTime.Now;
                   mSkuPrices.DATE_EFFECTED = Convert.ToDateTime(dr["DATE_EFFECTED"]); ;

                    mSkuPrices.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = pPrincipalId;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = 0;
                    mStockUpdate.BATCHNO = "0";
                    mStockUpdate.ExecuteQuery();

                }

                mTransaction.Commit();
                return mPurchaseMaster.PURCHASE_MASTER_ID;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();

                return -2;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }


        public DataSet SelectPurchaseOrderNo(long pPurchaseOrderId, int pTypeId, DateTime pDocumentDate)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                var mPurchaseMaster = new spSelectPURCHASE_ORDER
                {
                    Connection = mConnection,
                    PURCHASE_ORDER_ID = pPurchaseOrderId
                    ,
                    TYPE_ID = pTypeId
                    ,
                    document_date = pDocumentDate

                };

                return mPurchaseMaster.ExecuteDataSet();

            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }


        public long InsertPurchaseOrder(int pDistributorId, int pPrincipalId, string pContainerNo, string pRemarks, DateTime pArrivalDate, DateTime pDocumentDate, int pUserId,
          DataTable dtPurchaseDetail, DateTime pTimeStamp, DateTime pInvoiceDate, string p_BAG_NO)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);
                spInsertPURCHASE_ORDER mPurchaseMaster = new spInsertPURCHASE_ORDER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.DISTRIBUTOR_ID = pDistributorId;
                mPurchaseMaster.PRINCIPAL_ID = pPrincipalId;
                mPurchaseMaster.CONTAINER_NO = pContainerNo;
                mPurchaseMaster.REMARKS = pRemarks;
                mPurchaseMaster.ARRIVAL_DATE = pArrivalDate;
                mPurchaseMaster.DOCUMENT_DATE = pDocumentDate;
                mPurchaseMaster.USER_ID = pUserId;
                mPurchaseMaster.TIME_STAMP = pTimeStamp;  // as document date
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.INVOICE_DATE = pInvoiceDate;
                mPurchaseMaster.BAG_NO = p_BAG_NO;
                mPurchaseMaster.ExecuteQuery();

                spInsertPURCHASE_ORDER_DETAIL mPurchaseDetail = new spInsertPURCHASE_ORDER_DETAIL();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    if (decimal.Parse(dr["QUANTITY"].ToString()) > 0)
                    {
                        mPurchaseDetail.PURCHASE_ORDER_ID = mPurchaseMaster.PURCHASE_ORDER_ID;
                        mPurchaseDetail.DISTRIBUTOR_ID = pDistributorId;
                        mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                        mPurchaseDetail.QUANTITY = decimal.Parse(dr["QUANTITY"].ToString());
                        // mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                        //  mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                        mPurchaseDetail.TIME_STAMP = DateTime.Now;
                        mPurchaseDetail.ExecuteQuery();
                    }
                }
                mTransaction.Commit();
                return mPurchaseMaster.PURCHASE_ORDER_ID;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                return -2;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }


        public long UpdatePurchaseOrderDetail(long pPurchaseOrderId, int pDistributorId, string pContainerNo, string pRemarks, DateTime pArrivalDate, int pUserId, DataTable dtPurchaseDetail, DateTime pInvoiceDate, string p_BAG_NO)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);
                spDeletePURCHASE_ORDER_DETAIL mDelete = new spDeletePURCHASE_ORDER_DETAIL();
                mDelete.Connection = mConnection;
                mDelete.Transaction = mTransaction;
                mDelete.PURCHASE_ORDER_ID = pPurchaseOrderId;
                mDelete.DISTRIBUTOR_ID = pDistributorId;
                mDelete.ExecuteQuery();


                spUpdatePURCHASE_ORDER mPurchaseMaster = new spUpdatePURCHASE_ORDER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.PURCHASE_ORDER_ID = pPurchaseOrderId;
                mPurchaseMaster.DISTRIBUTOR_ID = pDistributorId;
                mPurchaseMaster.REMARKS = pRemarks;
                mPurchaseMaster.CONTAINER_NO = pContainerNo;
                mPurchaseMaster.ARRIVAL_DATE = pArrivalDate;
                mPurchaseMaster.USER_ID = pUserId;
                mPurchaseMaster.INVOICE_DATE = pInvoiceDate;
                mPurchaseMaster.BAG_NO = p_BAG_NO;
                mPurchaseMaster.ExecuteQuery();

                spInsertPURCHASE_ORDER_DETAIL mPurchaseDetail = new spInsertPURCHASE_ORDER_DETAIL();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    if (decimal.Parse(dr["QUANTITY"].ToString()) > 0)
                    {
                        mPurchaseDetail.PURCHASE_ORDER_ID = mPurchaseMaster.PURCHASE_ORDER_ID;
                        mPurchaseDetail.DISTRIBUTOR_ID = pDistributorId;
                        mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                        mPurchaseDetail.QUANTITY = decimal.Parse(dr["QUANTITY"].ToString());
                        //  mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                        // mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                        mPurchaseDetail.TIME_STAMP = DateTime.Now;
                        mPurchaseDetail.ExecuteQuery();
                    }
                }
                mTransaction.Commit();
                return mPurchaseMaster.PURCHASE_ORDER_ID;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                return -2;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }


        /// <summary>
        /// Gets Purchase, TranferOut, Purchase Return, TranferIn And Damage Document No
        /// </summary>
        /// <remarks>
        /// Returns Purchase, TranferOut, Purchase Return, TranferIn And Damage Document No as Datatable
        /// </remarks>
        /// <param name="p_TYPE_ID">Type</param>
        /// <param name="p_DISTRIBUTOR_ID">Location</param>
        /// <param name="p_PURCHASE_MASTER_ID">Purchase</param>
        /// <param name="p_User_Id">InsertedBy</param>
        /// <param name="p_Posting">Posting</param>
        /// <returns>Purchase, TranferOut, Purchase Return, TranferIn And Damage  Document No as Datatable</returns>
        public DataTable SelectPurchaseDocumentNo(int p_TYPE_ID, int p_DISTRIBUTOR_ID, long p_PURCHASE_MASTER_ID, int p_User_Id, int p_Posting)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                spSelectPURCHASE_MASTER mPurchaseMaster = new spSelectPURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                mPurchaseMaster.USER_ID = p_User_Id;
                mPurchaseMaster.POSTING = p_Posting;
                DataTable dt = mPurchaseMaster.ExecuteTable();
                return dt;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        /// <summary>
        /// Gets Purchase, TranferOut, Purchase Return, TranferIn And Damage Document No
        /// </summary>
        /// <remarks>
        /// Returns Purchase, TranferOut, Purchase Return, TranferIn And Damage Document No as Datatable
        /// </remarks>
        /// <param name="p_TYPE_ID">Type</param>
        /// <param name="p_DISTRIBUTOR_ID">Location</param>
        /// <param name="P_DocumentDate">Date</param>
        /// <returns>Purchase, TranferOut, Purchase Return, TranferIn And Damage Document No as Datatable</returns>
        public DataTable SelectPurchaseDocumentNo(int p_TYPE_ID, int p_DISTRIBUTOR_ID, DateTime P_DocumentDate)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                spSelectPURCHASE_MASTER mPurchaseMaster = new spSelectPURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.DOCUMENT_DATE = P_DocumentDate;
                DataTable dt = mPurchaseMaster.ExecuteTable();
                return dt;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        /// <summary>
        /// Gets Purchase, TranferOut, Purchase Return, TranferIn And Damage Document No
        /// </summary>
        /// <remarks>
        /// Returns Purchase, TranferOut, Purchase Return, TranferIn And Damage Document No as Datatable
        /// </remarks>
        /// <param name="p_TYPE_ID">Type</param>
        /// <param name="p_DISTRIBUTOR_ID">Location</param>
        /// <param name="p_PURCHASE_MASTER_ID">Purchase</param>
        /// <param name="p_User_Id">InsertedBy</param>
        /// <param name="p_Posting">Posting</param>
        /// <param name="p_SOLD_TO">SoldTo</param>
        /// <returns>Purchase, TranferOut, Purchase Return, TranferIn And Damage Document No as Datatable</returns>
        public DataTable SelectPurchaseDocumentNo(int p_TYPE_ID, int p_DISTRIBUTOR_ID, long p_PURCHASE_MASTER_ID, int p_User_Id, int p_Posting, int p_SOLD_TO)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                spSelectPURCHASE_MASTER mPurchaseMaster = new spSelectPURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                mPurchaseMaster.USER_ID = Constants.IntNullValue;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                DataTable dt = mPurchaseMaster.ExecuteTable();
                return dt;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        public DataTable SelectPurchaseDocumentNo(int p_TYPE_ID, int p_DISTRIBUTOR_ID, long p_PURCHASE_MASTER_ID, int p_User_Id, int p_Posting, int p_SOLD_TO,DateTime P_DocumentDate)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                spSelectPURCHASE_MASTER mPurchaseMaster = new spSelectPURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                mPurchaseMaster.USER_ID = Constants.IntNullValue;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.DOCUMENT_DATE = P_DocumentDate;
                DataTable dt = mPurchaseMaster.ExecuteTable();
                return dt;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        /// <summary>
        /// Get Purchase, TranferOut, Purchase Return, TranferIn And Damage Detail
        /// </summary>
        /// <remarks>
        /// Returns Purchase, TranferOut, Purchase Return, TranferIn And Damage Detail as Datatable
        /// </remarks>
        /// <param name="p_DISTRIBUTOR_ID">Location</param>
        /// <param name="p_PURCHASE_MASTER_ID">Purchase</param>
        /// <returns>Purchase, TranferOut, Purchase Return, TranferIn And Damage Detail as Datatable</returns>
        public DataTable SelectPurchaseDetail(int p_DISTRIBUTOR_ID, long p_PURCHASE_MASTER_ID)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                
                spSelectPURCHASE_DETAIL mPurchaseDetail = new spSelectPURCHASE_DETAIL();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseDetail.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                DataTable dt = mPurchaseDetail.ExecuteTable();
                return dt;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        public DataTable SelectPurchaseDetailOpening(int p_DISTRIBUTOR_ID, long p_PURCHASE_MASTER_ID)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                spSelectPURCHASE_DETAILOpnong mPurchaseDetail = new spSelectPURCHASE_DETAILOpnong();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseDetail.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                DataTable dt = mPurchaseDetail.ExecuteTable();
                return dt;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }




        public DataTable SelectPurchaseDetailstock(int p_DISTRIBUTOR_ID, long p_PURCHASE_MASTER_ID)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                
                spSelectPURCHASE_DETAIL2 mPurchaseDetail = new spSelectPURCHASE_DETAIL2();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseDetail.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                DataTable dt = mPurchaseDetail.ExecuteTable();
                return dt;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }
        #endregion

        #region Insert, Update

        /// <summary>
        /// Inserts Purchase, TranferOut, Purchase Return, TranferIn And Damage Document
        /// </summary>
        /// <param name="p_DISTRIBUTOR_ID">Location</param>
        /// <param name="p_ORDER_NUMBER">DocumentNo</param>
        /// <param name="p_TYPE_ID">Type</param>
        /// <param name="p_DOCUMENT_DATE">Date</param>
        /// <param name="p_SOLD_TO">SoldTo</param>
        /// <param name="p_SOLD_FROM">SoldFrom</param>
        /// <param name="p_TOTAL_AMOUNT">Amount</param>
        /// <param name="p_IS_DELETE">IsDeleted</param>
        /// <param name="dtPurchaseDetail">PurchaseDetailDatatable</param>
        /// <param name="p_Posting">Posting</param>
        /// <param name="p_BuiltyNo">Builty</param>
        /// <param name="p_UserId">InsertedBy</param>
        /// <param name="p_PrincipalId">Principal</param>
        /// <returns>True On Success And False On Failure</returns>
        public bool InsertPurchaseDocument(int p_DISTRIBUTOR_ID,string p_ORDER_NUMBER,int p_TYPE_ID,DateTime p_DOCUMENT_DATE,int p_SOLD_TO,int p_SOLD_FROM,decimal p_TOTAL_AMOUNT,bool p_IS_DELETE,DataTable dtPurchaseDetail,int p_Posting,string p_BuiltyNo,int p_UserId,int p_PrincipalId, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name) 
		{
			try/////////////////
			{                 
				mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
				mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spInsertTransferIn mPurchaseMaster = new spInsertTransferIn();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;  
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.TIME_STAMP = DateTime.Now;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;   
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.PRINCIPAL_ID = p_PrincipalId;
                mPurchaseMaster.ExecuteQuery();
                #endregion

                #region Purchase Detail
                spInsertPURCHASE_DETAIL mPurchaseDetail = new spInsertPURCHASE_DETAIL();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;
                
                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Quantity"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                   
                    mPurchaseDetail.ExecuteQuery();
                    
                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_PrincipalId; 
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;   
                    mStockUpdate.ExecuteQuery();   
                }
                #endregion

                #region GL Voucher
                p_Voucher_No = "";
                LedgerController LController = new LedgerController();
                Configuration.GetAccountHead();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2) // Purchases
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        //"Purchases " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                        //p_PrincipalAccount_Name + " " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 3)// Purchase Return
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 4)// Transfer IN
                {
                    //string Remarks = "Default Transfer In Voucher";
                    //string DocumentType = "Transfer IN";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 5)// Transfer OUT
                {
                    //string Remarks = "Default Transfer Out Voucher";
                    //string DocumentType = "Transfer Out";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 6)// Damage
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 10)// Damage claimable
                {
                    //string Remarks = "Default Stock Damage(Claimable) Voucher";
                    //string DocumentType = "Stock Damage(claimable)";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 897, 0, mPurchaseMaster.TOTAL_AMOUNT, "Purchases " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 845, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 7) // Opening
                {
                    string Remarks = "Default Opening Voucher";
                    string DocumentType = "Stock Opening";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Share_Capital, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Share_Capital_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 8)// Short
                {
                    string Remarks = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Short_Excess_Adjustment,  mPurchaseMaster.TOTAL_AMOUNT, 0,Constants.Short_Excess_Adjustment_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand,0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 9)// Excess
                {
                    string Remarks = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Short_Excess_Adjustment, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Short_Excess_Adjustment_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                #endregion

                mTransaction.Commit();
                return true; 
			}
			catch(Exception exp)
			{
				ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
			}
			finally
			{
				if(mConnection != null && mConnection.State == ConnectionState.Open)
				{
					mConnection.Close();
				}
			}
			
		}

        public bool InsertStockAdjustment(int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_PrincipalId, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name)
        {
            try/////////////////
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spInsertTransferIn mPurchaseMaster = new spInsertTransferIn();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.TIME_STAMP = DateTime.Now;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.PRINCIPAL_ID = p_PrincipalId;
                mPurchaseMaster.ExecuteQuery();
                #endregion

                #region Purchase Detail
                spInsertPURCHASE_DETAIL mPurchaseDetail = new spInsertPURCHASE_DETAIL();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;

                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_PrincipalId;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                #endregion

                #region GL Voucher
                p_Voucher_No = "";
                LedgerController LController = new LedgerController();
                Configuration.GetAccountHead();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2) // Purchases
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        //"Purchases " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                        //p_PrincipalAccount_Name + " " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 3)// Purchase Return
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 4)// Transfer IN
                {
                    //string Remarks = "Default Transfer In Voucher";
                    //string DocumentType = "Transfer IN";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 5)// Transfer OUT
                {
                    //string Remarks = "Default Transfer Out Voucher";
                    //string DocumentType = "Transfer Out";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 6)// Damage
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 10)// Damage claimable
                {
                    //string Remarks = "Default Stock Damage(Claimable) Voucher";
                    //string DocumentType = "Stock Damage(claimable)";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 897, 0, mPurchaseMaster.TOTAL_AMOUNT, "Purchases " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 845, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 7) // Opening
                {
                    string Remarks = "Default Opening Voucher";
                    string DocumentType = "Stock Opening";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Share_Capital, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Share_Capital_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 8)// Short
                {
                    string Remarks = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Short_Excess_Adjustment, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Short_Excess_Adjustment_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 9)// Excess
                {
                    string Remarks = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Short_Excess_Adjustment, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Short_Excess_Adjustment_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                #endregion

                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        public bool UpdateStockAdjustment(long p_PURCHASE_MASTER_ID, int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_Principal, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spUpdatePURCHASE_MASTER mPurchaseMaster = new spUpdatePURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.ExecuteQuery();
                #endregion

                //Get Privouse Update Purchase Detail and Rollback
                //LedgerController LController = new LedgerController();
                //string VoucherNo = LController.SelectLedgerMaxDocumentId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID);

                #region Purchase Detail
                DataTable dt = SelectPrivousePurchaseDetail(p_DISTRIBUTOR_ID, p_PURCHASE_MASTER_ID, mConnection, mTransaction);

                foreach (DataRow dr in dt.Rows)
                {
                    UspUpdatePurchaseDetailStock mPurchaseStock = new UspUpdatePurchaseDetailStock();
                    mPurchaseStock.Connection = mConnection;
                    mPurchaseStock.Transaction = mTransaction;
                    mPurchaseStock.TYPEID = p_TYPE_ID;
                    mPurchaseStock.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseStock.PURCHASE_DETAIL_ID = long.Parse(dr["PURCHASE_DETAIL_ID"].ToString());
                    mPurchaseStock.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                    mPurchaseStock.BATCH_NO = dr["BATCH_NO"].ToString().Trim();
                    mPurchaseStock.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseStock.ExecuteQuery();
                }

                spInsertPURCHASE_DETAIL mPurchaseDetail = new spInsertPURCHASE_DETAIL();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {

                    //update stock;
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());

                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_Principal;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                #endregion

                #region GL Voucher
                p_Voucher_No = "";
                LedgerController LController = new LedgerController();
                Configuration.GetAccountHead();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2) // Purchases
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        //"Purchases " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                        //p_PrincipalAccount_Name + " " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 3)// Purchase Return
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 4)// Transfer IN
                {
                    //string Remarks = "Default Transfer In Voucher";
                    //string DocumentType = "Transfer IN";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 5)// Transfer OUT
                {
                    //string Remarks = "Default Transfer Out Voucher";
                    //string DocumentType = "Transfer Out";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 6)// Damage
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 10)// Damage claimable
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 7) // Opening
                {
                    string Remarks = "Default Opening Voucher";
                    string DocumentType = "Stock Opening";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Share_Capital, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Share_Capital_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 8)// Short
                {
                    string Remarks = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Short_Excess_Adjustment, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Short_Excess_Adjustment_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);

                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 9)// Excess
                {
                    string Remarks = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Short_Excess_Adjustment, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Short_Excess_Adjustment_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                #endregion
                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        public bool InsertTransferIn(int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_PrincipalId, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name, long p_TranferOutID)
        {
            try/////////////////
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spInsertTransferIn mPurchaseMaster = new spInsertTransferIn();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.TIME_STAMP = DateTime.Now;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.PRINCIPAL_ID = p_PrincipalId;
                mPurchaseMaster.TRANSFER_OUT_ID = p_TranferOutID;
                mPurchaseMaster.ExecuteQuery();
                #endregion

                #region Purchase Detail
                spInsertPURCHASE_DETAIL mPurchaseDetail = new spInsertPURCHASE_DETAIL();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Quantity"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;

                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_PrincipalId;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                #endregion

                #region GL Voucher
                p_Voucher_No = "";
                LedgerController LController = new LedgerController();
                Configuration.GetAccountHead();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2) // Purchases
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        //"Purchases " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                        //p_PrincipalAccount_Name + " " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 3)// Purchase Return
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 4)// Transfer IN
                {
                    //string Remarks = "Default Transfer In Voucher";
                    //string DocumentType = "Transfer IN";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 5)// Transfer OUT
                {
                    //string Remarks = "Default Transfer Out Voucher";
                    //string DocumentType = "Transfer Out";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 6)// Damage
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 10)// Damage claimable
                {
                    //string Remarks = "Default Stock Damage(Claimable) Voucher";
                    //string DocumentType = "Stock Damage(claimable)";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 897, 0, mPurchaseMaster.TOTAL_AMOUNT, "Purchases " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 845, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 7) // Opening
                {
                    string Remarks = "Default Opening Voucher";
                    string DocumentType = "Stock Opening";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Share_Capital, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Share_Capital_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 8)// Short
                {
                    string Remarks = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Short_Excess_Adjustment, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Short_Excess_Adjustment_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 9)// Excess
                {
                    string Remarks = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Short_Excess_Adjustment, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Short_Excess_Adjustment_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                #endregion

                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }
        public bool InsertPurchaseDocumentGST(int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_PrincipalId, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name,decimal p_tstAMount,decimal p_gstAMount)
        {
            try/////////////////
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spInsertPURCHASE_MASTER mPurchaseMaster = new spInsertPURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.TIME_STAMP = DateTime.Now;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.PRINCIPAL_ID = p_PrincipalId;
                mPurchaseMaster.ExecuteQuery();
                #endregion

                #region Purchase Detail
                spInsertPURCHASE_DETAIL mPurchaseDetail = new spInsertPURCHASE_DETAIL();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;

                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_PrincipalId;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                #endregion

                #region GL Voucher
                p_Voucher_No = "";
                LedgerController LController = new LedgerController();
                Configuration.GetAccountHead();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2) // Purchases
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        //"Purchases " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                        //p_PrincipalAccount_Name + " " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 3)// Purchase Return
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 4)// Transfer IN
                {
                    //string Remarks = "Default Transfer In Voucher";
                    //string DocumentType = "Transfer IN";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 5)// Transfer OUT
                {
                    //string Remarks = "Default Transfer Out Voucher";
                    //string DocumentType = "Transfer Out";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 6)// Damage
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 10)// Damage claimable
                {
                    //string Remarks = "Default Stock Damage(Claimable) Voucher";
                    //string DocumentType = "Stock Damage(claimable)";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 897, 0, mPurchaseMaster.TOTAL_AMOUNT, "Purchases " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 845, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 7) // Opening
                {
                    string Remarks = "Default Opening Voucher";
                    string DocumentType = "Stock Opening";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, (mPurchaseMaster.TOTAL_AMOUNT-p_tstAMount-p_gstAMount), 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Sales_Tax_payable_against_supplies, (p_tstAMount + p_gstAMount), 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);

                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Share_Capital, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Share_Capital_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 8)// Short
                {
                    string Remarks = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 9)// Excess
                {
                    string Remarks = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_PrincipalId, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_PrincipalId, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                #endregion

                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }
        public bool InsertPurchaseDocumentStock(int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_PrincipalId, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spInsertPURCHASE_MASTER mPurchaseMaster = new spInsertPURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.TIME_STAMP = DateTime.Now;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.PRINCIPAL_ID = p_PrincipalId;
              
                mPurchaseMaster.ExecuteQuery();
                #endregion

                #region Purchase Detail

                spInsertPURCHASE_DETAIL2 mPurchaseDetail = new spInsertPURCHASE_DETAIL2();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.TDAMAGE = int.Parse(dr["TDAMAGE"].ToString());
                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_PrincipalId;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                #endregion

                #region GL Voucher
                p_Voucher_No = "";
                LedgerController LController = new LedgerController();
                Configuration.GetAccountHead();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2) // Purchases
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        //"Purchases " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                        //p_PrincipalAccount_Name + " " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 3)// Purchase Return
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 4)// Transfer IN
                {
                    string Remarks = "Default Transfer In Voucher";
                    string DocumentType = "Transfer In";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.In_Transit_Stock, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.In_Transit_Stock_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 5)// Transfer OUT
                {
                    string Remarks = "Default Transfer Out Voucher";
                    string DocumentType = "Transfer Out";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.In_Transit_Stock, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.In_Transit_Stock_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 6)// Damage
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 10)// Damage claimable
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 8)// Short
                {
                    string Remarks = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 9)// Excess
                {
                    string Remarks = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                #endregion


                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        public bool InsertPurchaseDocumentStockGST(int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_PrincipalId, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name, decimal p_Gst_Amount, decimal p_Tst_Amount)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spInsertPURCHASE_MASTER mPurchaseMaster = new spInsertPURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.TIME_STAMP = DateTime.Now;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.PRINCIPAL_ID = p_PrincipalId;

                mPurchaseMaster.ExecuteQuery();
                #endregion

                #region Purchase Detail

                spInsertPURCHASE_DETAIL2 mPurchaseDetail = new spInsertPURCHASE_DETAIL2();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.TDAMAGE = int.Parse(dr["TDAMAGE"].ToString());
                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_PrincipalId;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                #endregion

                #region GL Voucher
                p_Voucher_No = "";
                LedgerController LController = new LedgerController();
                Configuration.GetAccountHead();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2) // Purchases
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        //"Purchases " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                        //p_PrincipalAccount_Name + " " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 3)// Purchase Return
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, (mPurchaseMaster.TOTAL_AMOUNT-p_Tst_Amount-p_Gst_Amount), Remarks2, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Sales_Tax_payable_against_supplies, 0, (p_Tst_Amount + p_Gst_Amount), Remarks2, mTransaction, mConnection);

                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 4)// Transfer IN
                {
                    string Remarks = "Default Transfer In Voucher";
                    string DocumentType = "Transfer In";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.In_Transit_Stock, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.In_Transit_Stock_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 5)// Transfer OUT
                {
                    string Remarks = "Default Transfer Out Voucher";
                    string DocumentType = "Transfer Out";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.In_Transit_Stock, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.In_Transit_Stock_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Gujranwala_Stock_In_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Gujranwala_Stock_In_Hand_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 6)// Damage
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 10)// Damage claimable
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 8)// Short
                {
                    string Remarks = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 9)// Excess
                {
                    string Remarks = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                #endregion


                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        public bool InsertPurchaseDocumentStock(int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_PrincipalId, decimal p_FMR, decimal p_MR, decimal p_Pur_Discount,decimal p_Gst_Amount,decimal p_Tst_Amount,decimal p_total_Net_Amount)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);
                spInsertPURCHASE_MASTER mPurchaseMaster = new spInsertPURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.TIME_STAMP = DateTime.Now;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.PRINCIPAL_ID = p_PrincipalId;
                mPurchaseMaster.FMR = p_FMR;
                mPurchaseMaster.MR = p_MR;
                mPurchaseMaster.PUR_DISCOUNT = p_Pur_Discount;
                mPurchaseMaster.GST_AMOUNT = p_Gst_Amount;
                mPurchaseMaster.TST_AMOUNT = p_Tst_Amount;
                mPurchaseMaster.TOTAL_NET_AMOUNT = p_total_Net_Amount;
                mPurchaseMaster.IS_DELETED = false;
                mPurchaseMaster.ExecuteQuery();

                spInsertPURCHASE_DETAIL2 mPurchaseDetail = new spInsertPURCHASE_DETAIL2();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());

                    mPurchaseDetail.GST_AMOUNT = decimal.Parse(dr["GST_AMOUNT"].ToString());
                    mPurchaseDetail.TST_AMOUNT = decimal.Parse(dr["TST_AMOUNT"].ToString());
                    mPurchaseDetail.GST_RATE = decimal.Parse(dr["GST_RATE"].ToString());

                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.TDAMAGE = int.Parse(dr["TDAMAGE"].ToString());
                    mPurchaseDetail.FMR = decimal.Parse(dr["FMR"].ToString());
                    mPurchaseDetail.MR = decimal.Parse(dr["MR"].ToString());
                    mPurchaseDetail.PUR_DISCOUNT = decimal.Parse(dr["PUR_DISCOUNT"].ToString());
                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_PrincipalId;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }



        public bool InsertPurchase(int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_PrincipalId, decimal p_FMR, decimal p_MR, decimal p_Pur_Discount, decimal p_Gst_Amount, decimal p_Tst_Amount, decimal p_total_Net_Amount, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name, decimal p_AdjustmentAmount)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spInsertPURCHASE_MASTER mPurchaseMaster = new spInsertPURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_ADJUSTMENT_AMOUNT = p_AdjustmentAmount;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.TIME_STAMP = DateTime.Now;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.PRINCIPAL_ID = p_PrincipalId;
                mPurchaseMaster.FMR = p_FMR;
                mPurchaseMaster.MR = p_MR;
                mPurchaseMaster.PUR_DISCOUNT = p_Pur_Discount;
                mPurchaseMaster.GST_AMOUNT = p_Gst_Amount;
                mPurchaseMaster.TST_AMOUNT = p_Tst_Amount;
                mPurchaseMaster.TOTAL_NET_AMOUNT = p_total_Net_Amount;
                mPurchaseMaster.IS_DELETED = false;
                mPurchaseMaster.REMARKS = p_REMARKS;
                mPurchaseMaster.ExecuteQuery();
                #endregion

                #region Purchase Detail

                spInsertPURCHASE_DETAIL2 mPurchaseDetail = new spInsertPURCHASE_DETAIL2();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());

                    mPurchaseDetail.GST_AMOUNT = decimal.Parse(dr["GST_AMOUNT"].ToString());
                    mPurchaseDetail.TST_AMOUNT = decimal.Parse(dr["TST_AMOUNT"].ToString());
                    mPurchaseDetail.GST_RATE = decimal.Parse(dr["GST_RATE"].ToString());
                    mPurchaseDetail.ADJUSTMENT_AMOUNT = decimal.Parse(dr["ADJUSTMENT"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.TDAMAGE = int.Parse(dr["TDAMAGE"].ToString());
                    mPurchaseDetail.PUR_DISCOUNT = decimal.Parse(dr["PUR_DISCOUNT"].ToString());
                    mPurchaseDetail.ExecuteQuery();

                    //if (p_TYPE_ID == 2)
                    //{
                    //    UspUpdateProcessSKU_Prices mSkuPrices = new UspUpdateProcessSKU_Prices();
                    //    mSkuPrices.Connection = mConnection;
                    //    mSkuPrices.Transaction = mTransaction;
                    //    mSkuPrices.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    //    mSkuPrices.DISTRIBUTOR_PRICE = decimal.Parse(dr["AVG_PRICE"].ToString());
                    //    mSkuPrices.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    //    mSkuPrices.ExecuteQuery();
                    //}

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_PrincipalId;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }

                #endregion

                #region Mapped Acount Selection
                //CustomerDataController cd = new CustomerDataController();

                //long p_PrincipalAccount_ID = 88;//2002080001	Payable to (Principal Wise)
                //string p_PrincipalAccount_Name = "";
                //if (mPurchaseMaster.TYPE_ID == 2 || mPurchaseMaster.TYPE_ID == 3)// purchase and purchase return
                //{
                //    //DataTable dtAccount = cd.SelectAccountHeadMapping(1, Constants.IntNullValue, mPurchaseMaster.PRINCIPAL_ID, Constants.LongNullValue, Constants.IntNullValue);
                //    //if (dtAccount != null && dtAccount.Rows.Count > 0)
                //    //{
                //    //    p_PrincipalAccount_ID = long.Parse(dtAccount.Rows[0]["Account_head_ID"].ToString());
                //    //    p_PrincipalAccount_Name = dtAccount.Rows[0]["ACCOUNT_NAME"].ToString();

                //    //}
                //    p_PrincipalAccount_ID = p_Account_Head_ID;
                //    p_PrincipalAccount_Name = p_Account_Head_Name;

                //}
                //else if (mPurchaseMaster.TYPE_ID == 4 || mPurchaseMaster.TYPE_ID == 5)// transfer in ,out
                //{
                //    p_PrincipalAccount_ID = 844;// 3002010002 Stock Movements
                //    p_PrincipalAccount_Name = "Stock Movements";
                //}
                //else if (mPurchaseMaster.TYPE_ID == 6)//Damage
                //{
                //    p_PrincipalAccount_ID = 843;//5002020013//Damage and Expiries Expense
                //    p_PrincipalAccount_Name = "Damage and Expiries Expense";
                //}
                //else if (mPurchaseMaster.TYPE_ID == 10)//Damage claimable
                //{
                //    p_PrincipalAccount_ID = 845;//5002020013//Damage and Expiries Expense
                //    p_PrincipalAccount_Name = "Damage & Expiry - Claimable";
                //}
                //else if (mPurchaseMaster.TYPE_ID == 8)// Short
                //{
                //    p_PrincipalAccount_ID = 898;//3002020076 Warehouse Incharge
                //    p_PrincipalAccount_Name = "Warehouse Incharge";
                //}
                //else
                //{
                //    DataTable dtAccount = cd.SelectAccountHeadMapping(1, Constants.IntNullValue, mPurchaseMaster.PRINCIPAL_ID, Constants.LongNullValue, Constants.IntNullValue);
                //    if (dtAccount != null && dtAccount.Rows.Count > 0)
                //    {
                //        p_PrincipalAccount_ID = long.Parse(dtAccount.Rows[0]["Account_head_ID"].ToString());
                //        p_PrincipalAccount_Name = dtAccount.Rows[0]["ACCOUNT_NAME"].ToString();

                //    }
                //}
                #endregion


                #region Mapped Acount Selection

                CustomerDataController cd = new CustomerDataController();
                string CodeFileName = "";
                int CodeFileType = 0;
                DataTable dtMappSetting = cd.SelectMappingSetting(Constants.IntNullValue, Constants.IntNullValue);
                if (dtMappSetting != null || dtMappSetting.Rows.Count > 0)
                {
                    CodeFileName = dtMappSetting.Rows[0]["CODE_FILE"].ToString();
                    Configuration.GetAppMapping(CodeFileName);
                    CodeFileType = int.Parse(dtMappSetting.Rows[0]["CODE_TYPE"].ToString());
                }
                long p_PrincipalAccount_ID = 88;    //2002080001	Payable to (Principal Wise)
                string p_PrincipalAccount_Name = "";
                if (mPurchaseMaster.TYPE_ID == 2 || mPurchaseMaster.TYPE_ID == 3 || mPurchaseMaster.TYPE_ID == 4 ||
                    mPurchaseMaster.TYPE_ID == 5 || mPurchaseMaster.TYPE_ID == 6 || mPurchaseMaster.TYPE_ID == 8 ||
                    mPurchaseMaster.TYPE_ID == 9 || mPurchaseMaster.TYPE_ID == 10)
                {
                    DataTable dtAccount = cd.SelectAccountHeadMapping(1, Constants.IntNullValue, mPurchaseMaster.PRINCIPAL_ID, Constants.LongNullValue, Constants.IntNullValue);
                    if (dtAccount != null && dtAccount.Rows.Count > 0)
                    {
                        p_PrincipalAccount_ID = long.Parse(dtAccount.Rows[0]["Account_head_ID"].ToString());
                        p_PrincipalAccount_Name = dtAccount.Rows[0]["ACCOUNT_NAME"].ToString();
                    }
                }
                #endregion



                #region GL Voucher
                p_Voucher_No = "";
                LedgerController LController = new LedgerController();
                Configuration.GetAccountHead();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2) // Purchases
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 103, mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                        else
                        {
                            // For Other Clients
                            //"Purchases " + Remarks2
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, (mPurchaseMaster.TOTAL_AMOUNT - p_Tst_Amount - p_Gst_Amount), 0, Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Sales_Tax_payable_against_supplies, (p_Tst_Amount + p_Gst_Amount), 0, Remarks2, mTransaction, mConnection);

                            //p_PrincipalAccount_Name + " " + Remarks2
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                        }
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 3)// Purchase Return
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 103, 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                        else
                        {
                            // For Other Clients
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                        }
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 4)// Transfer IN
                {
                    string Remarks = "Default Transfer In Voucher";
                    string DocumentType = "Transfer IN";
                    if (CodeFileType == 1)
                    {
                        // For Atlas Equipment
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 103, mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.TransferInAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                    else
                    {
                        // For Other Clients
                        //string Remarks = "Default Transfer In Voucher";
                        //string DocumentType = "Transfer IN";
                        //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                        //{
                        //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                        //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        //}
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 5)// Transfer OUT
                {
                    string Remarks = "Default Transfer In Voucher";
                    string DocumentType = "Transfer IN";
                    if (CodeFileType == 1)
                    {
                        // For Atlas Equipment
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.TransferOutAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 103, 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    }
                    else
                    {
                        // For Other Clients
                        //string Remarks = "Default Transfer Out Voucher";
                        //string DocumentType = "Transfer Out";
                        //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                        //{
                        //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                        //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        //}
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 6)// Damage
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 103, 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        }
                        else
                        {
                            // For Other Clients
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                        }
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 10)// Damage claimable
                {
                    string Remarks = "Default Damage claimable Voucher";
                    string DocumentType = "Damage claimable";
                    if (CodeFileType == 1)
                    {
                        // For Atlas Equipment
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 103, 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                    else
                    {
                        // For Other Clients
                        //string Remarks = "Default Stock Damage(Claimable) Voucher";
                        //string DocumentType = "Stock Damage(claimable)";
                        //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                        //{
                        //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 897, 0, mPurchaseMaster.TOTAL_AMOUNT, "Purchases " + Remarks, mTransaction, mConnection);
                        //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 845, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        //});
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 8)// Short
                {
                    string Remarks = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, 103, 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        }
                        else
                        {
                            // For Other Clients
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                        }
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 9)// Excess
                {
                    string Remarks = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, 103, mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock In Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        }
                        else
                        {
                            // For Other Clients
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                        }
                    }
                }
                #endregion
                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        public bool InsertPurchaseforConfident(int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_PrincipalId, decimal p_FMR, decimal p_MR, decimal p_Pur_Discount, decimal p_Gst_Amount, decimal p_Tst_Amount, decimal p_total_Net_Amount, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spInsertPURCHASE_MASTER mPurchaseMaster = new spInsertPURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.TIME_STAMP = DateTime.Now;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.PRINCIPAL_ID = p_PrincipalId;
                mPurchaseMaster.FMR = p_FMR;
                mPurchaseMaster.MR = p_MR;
                mPurchaseMaster.PUR_DISCOUNT = p_Pur_Discount;
                mPurchaseMaster.GST_AMOUNT = p_Gst_Amount;
                mPurchaseMaster.TST_AMOUNT = p_Tst_Amount;
                mPurchaseMaster.TOTAL_NET_AMOUNT = p_total_Net_Amount;
                mPurchaseMaster.IS_DELETED = false;
                mPurchaseMaster.ExecuteQuery();
                #endregion

                #region Purchase Detail

                spInsertPURCHASE_DETAIL2 mPurchaseDetail = new spInsertPURCHASE_DETAIL2();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());

                    mPurchaseDetail.GST_AMOUNT = decimal.Parse(dr["GST_AMOUNT"].ToString());
                    mPurchaseDetail.TST_AMOUNT = decimal.Parse(dr["TST_AMOUNT"].ToString());
                    mPurchaseDetail.GST_RATE = decimal.Parse(dr["GST_RATE"].ToString());

                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.TDAMAGE = int.Parse(dr["TDAMAGE"].ToString());
                    mPurchaseDetail.PUR_DISCOUNT = decimal.Parse(dr["PUR_DISCOUNT"].ToString());
                    mPurchaseDetail.TRADE_PRICE = decimal.Parse(dr["SKU_PRICE"].ToString());
                    mPurchaseDetail.ExecuteQuery();

                    if (p_TYPE_ID == 2)
                    {
                        UspUpdateProcessSKU_Prices mSkuPrices = new UspUpdateProcessSKU_Prices();
                        mSkuPrices.Connection = mConnection;
                        mSkuPrices.Transaction = mTransaction;
                        mSkuPrices.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                        mSkuPrices.DISTRIBUTOR_PRICE = decimal.Parse(dr["AVG_PRICE"].ToString());
                        mSkuPrices.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                        mSkuPrices.ExecuteQuery();
                    }

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_PrincipalId;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }

                #endregion

                #region Mapped Acount Selection
                CustomerDataController cd = new CustomerDataController();

                long p_PrincipalAccount_ID = 88;//2002080001	Payable to (Principal Wise)
                string p_PrincipalAccount_Name = "Payable to (Principal Wise)";
                if (mPurchaseMaster.TYPE_ID == 2 || mPurchaseMaster.TYPE_ID == 3)// purchase and purchase return
                {
                    DataTable dtAccount = cd.SelectAccountHeadMapping(1, Constants.IntNullValue, mPurchaseMaster.PRINCIPAL_ID, Constants.LongNullValue, Constants.IntNullValue);
                    if (dtAccount != null && dtAccount.Rows.Count > 0)
                    {
                        p_PrincipalAccount_ID = long.Parse(dtAccount.Rows[0]["ACCOUNT_HEAD_ID"].ToString());
                        p_PrincipalAccount_Name = dtAccount.Rows[0]["ACCOUNT_NAME"].ToString();

                    }
                }
                //else if (mPurchaseMaster.TYPE_ID == 4 || mPurchaseMaster.TYPE_ID == 5)// transfer in ,out
                //{
                //    p_PrincipalAccount_ID = 844;// 3002010002 Stock Movements
                //    p_PrincipalAccount_Name = "Stock Movements";
                //}
                //else if (mPurchaseMaster.TYPE_ID == 6)//Damage
                //{
                //    p_PrincipalAccount_ID = 843;//5002020013//Damage and Expiries Expense
                //    p_PrincipalAccount_Name = "Damage and Expiries Expense";
                //}
                //else if (mPurchaseMaster.TYPE_ID == 10)//Damage claimable
                //{
                //    p_PrincipalAccount_ID = 845;//5002020013//Damage and Expiries Expense
                //    p_PrincipalAccount_Name = "Damage & Expiry - Claimable";
                //}
                //else if (mPurchaseMaster.TYPE_ID == 8)// Short
                //{
                //    p_PrincipalAccount_ID = 898;//3002020076 Warehouse Incharge
                //    p_PrincipalAccount_Name = "Warehouse Incharge";
                //}
                //else
                //{
                //    DataTable dtAccount = cd.SelectAccountHeadMapping(1, Constants.IntNullValue, mPurchaseMaster.PRINCIPAL_ID, Constants.LongNullValue, Constants.IntNullValue);
                //    if (dtAccount != null && dtAccount.Rows.Count > 0)
                //    {
                //        p_PrincipalAccount_ID = long.Parse(dtAccount.Rows[0]["Account_head_ID"].ToString());
                //        p_PrincipalAccount_Name = dtAccount.Rows[0]["ACCOUNT_NAME"].ToString();

                //    }
                //}
                #endregion

                #region GL Voucher
                p_Voucher_No = "";
                LedgerController LController = new LedgerController();
                Configuration.GetAccountHead();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2) // Purchases
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        //"Purchases " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                        //p_PrincipalAccount_Name + " " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 3)// Purchase Return
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 4)// Transfer IN
                {
                    //string Remarks = "Default Transfer In Voucher";
                    //string DocumentType = "Transfer IN";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 5)// Transfer OUT
                {
                    //string Remarks = "Default Transfer Out Voucher";
                    //string DocumentType = "Transfer Out";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 6)// Damage
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 10)// Damage claimable
                {
                    //string Remarks = "Default Stock Damage(Claimable) Voucher";
                    //string DocumentType = "Stock Damage(claimable)";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 897, 0, mPurchaseMaster.TOTAL_AMOUNT, "Purchases " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 845, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 8)// Short
                {
                    string Remarks = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 9)// Excess
                {
                    string Remarks = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                #endregion
                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }


        public bool InsertPurchaseDocumentStock(int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_PrincipalId, int p_Damage_Type, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spInsertPURCHASE_MASTER mPurchaseMaster = new spInsertPURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.TIME_STAMP = DateTime.Now;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.PRINCIPAL_ID = p_PrincipalId;
                mPurchaseMaster.DAMAGE_TYPE = p_Damage_Type;
                mPurchaseMaster.ExecuteQuery();
                #endregion


                #region Purchase Detail
                spInsertPURCHASE_DETAIL2 mPurchaseDetail = new spInsertPURCHASE_DETAIL2();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.TDAMAGE = int.Parse(dr["TDAMAGE"].ToString());
                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_PrincipalId;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                #endregion

                #region GL Voucher
                p_Voucher_No = "";
                LedgerController LController = new LedgerController();
                Configuration.GetAccountHead();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2) // Purchases
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        //"Purchases " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                        //p_PrincipalAccount_Name + " " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 3)// Purchase Return
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 4)// Transfer IN
                {
                    string Remarks = "Default Transfer In Voucher";
                    string DocumentType = "Transfer In";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.In_Transit_Stock, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.In_Transit_Stock_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 5)// Transfer OUT
                {
                    string Remarks = "Default Transfer Out Voucher";
                    string DocumentType = "Transfer Out";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.In_Transit_Stock, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.In_Transit_Stock_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Gujranwala_Stock_In_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Gujranwala_Stock_In_Hand_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 6)// Damage
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 10)// Damage claimable
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 8)// Short
                {
                    string Remarks = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 9)// Excess
                {
                    string Remarks = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                #endregion

                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }
        public bool UpdatePurchaseDocument(long p_PURCHASE_MASTER_ID, int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId,int p_Principal, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spUpdatePURCHASE_MASTER mPurchaseMaster = new spUpdatePURCHASE_MASTER(); 
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;  
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;   
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;  
                mPurchaseMaster.ExecuteQuery();
                #endregion

                //Get Privouse Update Purchase Detail and Rollback
                //LedgerController LController = new LedgerController();
                //string VoucherNo = LController.SelectLedgerMaxDocumentId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID);

                #region Purchase Detail
                DataTable dt = SelectPrivousePurchaseDetail(p_DISTRIBUTOR_ID, p_PURCHASE_MASTER_ID,mConnection,mTransaction);
                
                foreach (DataRow dr in dt.Rows)
                {
                    UspUpdatePurchaseDetailStock mPurchaseStock = new UspUpdatePurchaseDetailStock();
                    mPurchaseStock.Connection = mConnection;
                    mPurchaseStock.Transaction = mTransaction;
                    mPurchaseStock.TYPEID = p_TYPE_ID;
                    mPurchaseStock.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseStock.PURCHASE_DETAIL_ID = long.Parse(dr["PURCHASE_DETAIL_ID"].ToString());
                    mPurchaseStock.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID; 
                    mPurchaseStock.BATCH_NO = dr["BATCH_NO"].ToString().Trim();
                    mPurchaseStock.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseStock.ExecuteQuery();
                }
                                
                spInsertPURCHASE_DETAIL mPurchaseDetail = new spInsertPURCHASE_DETAIL();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    
                   //update stock;
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                   
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;  
                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_Principal; 
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                #endregion

                #region GL Voucher
                p_Voucher_No = "";
                LedgerController LController = new LedgerController();
                Configuration.GetAccountHead();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2) // Purchases
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        //"Purchases " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                        //p_PrincipalAccount_Name + " " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 3)// Purchase Return
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 4)// Transfer IN
                {
                    //string Remarks = "Default Transfer In Voucher";
                    //string DocumentType = "Transfer IN";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 5)// Transfer OUT
                {
                    //string Remarks = "Default Transfer Out Voucher";
                    //string DocumentType = "Transfer Out";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 6)// Damage
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 10)// Damage claimable
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 7) // Opening
                {
                    string Remarks = "Default Opening Voucher";
                    string DocumentType = "Stock Opening";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Share_Capital, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Share_Capital_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 8)// Short
                {
                    string Remarks = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Short_Excess_Adjustment, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Short_Excess_Adjustment_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);

                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 9)// Excess
                {
                    string Remarks = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Short_Excess_Adjustment, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Short_Excess_Adjustment_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                #endregion
                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        public bool UpdatePurchaseDocumentGST(long p_PURCHASE_MASTER_ID, int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_Principal, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name,decimal p_GSTAmount,decimal p_TST_Amount)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spUpdatePURCHASE_MASTER mPurchaseMaster = new spUpdatePURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.ExecuteQuery();
                #endregion

                //Get Privouse Update Purchase Detail and Rollback
                //LedgerController LController = new LedgerController();
                //string VoucherNo = LController.SelectLedgerMaxDocumentId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID);

                #region Purchase Detail
                DataTable dt = SelectPrivousePurchaseDetail(p_DISTRIBUTOR_ID, p_PURCHASE_MASTER_ID, mConnection, mTransaction);

                foreach (DataRow dr in dt.Rows)
                {
                    UspUpdatePurchaseDetailStock mPurchaseStock = new UspUpdatePurchaseDetailStock();
                    mPurchaseStock.Connection = mConnection;
                    mPurchaseStock.Transaction = mTransaction;
                    mPurchaseStock.TYPEID = p_TYPE_ID;
                    mPurchaseStock.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseStock.PURCHASE_DETAIL_ID = long.Parse(dr["PURCHASE_DETAIL_ID"].ToString());
                    mPurchaseStock.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                    mPurchaseStock.BATCH_NO = dr["BATCH_NO"].ToString().Trim();
                    mPurchaseStock.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseStock.ExecuteQuery();
                }

                spInsertPURCHASE_DETAIL mPurchaseDetail = new spInsertPURCHASE_DETAIL();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {

                    //update stock;
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());

                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_Principal;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                #endregion

                #region GL Voucher
                p_Voucher_No = "";
                LedgerController LController = new LedgerController();
                Configuration.GetAccountHead();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2) // Purchases
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        //"Purchases " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                        //p_PrincipalAccount_Name + " " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 3)// Purchase Return
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 4)// Transfer IN
                {
                    //string Remarks = "Default Transfer In Voucher";
                    //string DocumentType = "Transfer IN";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 5)// Transfer OUT
                {
                    //string Remarks = "Default Transfer Out Voucher";
                    //string DocumentType = "Transfer Out";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 6)// Damage
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 10)// Damage claimable
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 7) // Opening
                {
                    string Remarks = "Default Opening Voucher";
                    string DocumentType = "Stock Opening";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, (mPurchaseMaster.TOTAL_AMOUNT - p_TST_Amount - p_GSTAmount), 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Sales_Tax_payable_against_supplies, (p_TST_Amount + p_GSTAmount), 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);

                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Share_Capital, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Share_Capital_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 8)// Short
                {
                    string Remarks = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 9)// Excess
                {
                    string Remarks = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                #endregion
                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }
        public bool UpdatePurchaseDocumentStock(long p_PURCHASE_MASTER_ID, int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_Principal, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spUpdatePURCHASE_MASTER mPurchaseMaster = new spUpdatePURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.PRINCIPAL_ID = p_Principal;
                mPurchaseMaster.ExecuteQuery();
                #endregion
                
                //Get Privouse Update Purchase Detail and Rollback
                //LedgerController LController = new LedgerController();
                //string VoucherNo = LController.SelectLedgerMaxDocumentId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID);

                #region Purchase Detail
                DataTable dt = SelectPrivousePurchaseDetail(p_DISTRIBUTOR_ID, p_PURCHASE_MASTER_ID, mConnection, mTransaction);

                foreach (DataRow dr in dt.Rows)
                {
                    UspUpdatePurchaseDetailStock mPurchaseStock = new UspUpdatePurchaseDetailStock();
                    mPurchaseStock.Connection = mConnection;
                    mPurchaseStock.Transaction = mTransaction;
                    mPurchaseStock.TYPEID = p_TYPE_ID;
                    mPurchaseStock.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseStock.PURCHASE_DETAIL_ID = long.Parse(dr["PURCHASE_DETAIL_ID"].ToString());
                    mPurchaseStock.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                    mPurchaseStock.BATCH_NO = dr["BATCH_NO"].ToString().Trim();
                    mPurchaseStock.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseStock.ExecuteQuery();
                }

                spInsertPURCHASE_DETAIL2 mPurchaseDetail = new spInsertPURCHASE_DETAIL2();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {

                    //update stock;
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TDAMAGE = int.Parse(dr["TDAMAGE"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_Principal;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                #endregion

                #region GL Voucher
                p_Voucher_No = "";
                LedgerController LController = new LedgerController();
                Configuration.GetAccountHead();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2) // Purchases
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        //"Purchases " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                        //p_PrincipalAccount_Name + " " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 3)// Purchase Return
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 4)// Transfer IN
                {
                    string Remarks = "Default Transfer In Voucher";
                    string DocumentType = "Transfer In";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.In_Transit_Stock, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.In_Transit_Stock_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 5)// Transfer OUT
                {
                    string Remarks = "Default Transfer Out Voucher";
                    string DocumentType = "Transfer Out";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.In_Transit_Stock, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.In_Transit_Stock_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 6)// Damage
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 10)// Damage claimable
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 8)// Short
                {
                    string Remarks = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 9)// Excess
                {
                    string Remarks = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                #endregion

                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        public bool UpdatePurchaseDocumentStockGST(long p_PURCHASE_MASTER_ID, int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_Principal, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name, decimal p_GST_Amount, decimal p_Tst_Amount)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spUpdatePURCHASE_MASTER mPurchaseMaster = new spUpdatePURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.PRINCIPAL_ID = p_Principal;
                mPurchaseMaster.ExecuteQuery();
                #endregion

                //Get Privouse Update Purchase Detail and Rollback
                //LedgerController LController = new LedgerController();
                //string VoucherNo = LController.SelectLedgerMaxDocumentId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID);

                #region Purchase Detail
                DataTable dt = SelectPrivousePurchaseDetail(p_DISTRIBUTOR_ID, p_PURCHASE_MASTER_ID, mConnection, mTransaction);

                foreach (DataRow dr in dt.Rows)
                {
                    UspUpdatePurchaseDetailStock mPurchaseStock = new UspUpdatePurchaseDetailStock();
                    mPurchaseStock.Connection = mConnection;
                    mPurchaseStock.Transaction = mTransaction;
                    mPurchaseStock.TYPEID = p_TYPE_ID;
                    mPurchaseStock.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseStock.PURCHASE_DETAIL_ID = long.Parse(dr["PURCHASE_DETAIL_ID"].ToString());
                    mPurchaseStock.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                    mPurchaseStock.BATCH_NO = dr["BATCH_NO"].ToString().Trim();
                    mPurchaseStock.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseStock.ExecuteQuery();
                }

                spInsertPURCHASE_DETAIL2 mPurchaseDetail = new spInsertPURCHASE_DETAIL2();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {

                    //update stock;
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TDAMAGE = int.Parse(dr["TDAMAGE"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_Principal;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                #endregion

                #region GL Voucher
                p_Voucher_No = "";
                LedgerController LController = new LedgerController();
                Configuration.GetAccountHead();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2) // Purchases
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        //"Purchases " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                        //p_PrincipalAccount_Name + " " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 3)// Purchase Return
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, (mPurchaseMaster.TOTAL_AMOUNT - p_Tst_Amount - p_GST_Amount), Remarks2, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Sales_Tax_payable_against_supplies, 0, (p_Tst_Amount + p_GST_Amount), Remarks2, mTransaction, mConnection);

                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);

                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 4)// Transfer IN
                {
                    string Remarks = "Default Transfer In Voucher";
                    string DocumentType = "Transfer In";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.In_Transit_Stock, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.In_Transit_Stock_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 5)// Transfer OUT
                {
                    string Remarks = "Default Transfer Out Voucher";
                    string DocumentType = "Transfer Out";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.In_Transit_Stock, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.In_Transit_Stock_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Gujranwala_Stock_In_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Gujranwala_Stock_In_Hand_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 6)// Damage
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 10)// Damage claimable
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 8)// Short
                {
                    string Remarks = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 9)// Excess
                {
                    string Remarks = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                #endregion

                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }
        public bool UpdatePurchaseDocumentStock(long p_PURCHASE_MASTER_ID, int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_Principal, decimal p_FMR, decimal p_MR, decimal p_Pur_Discount,decimal p_GST_Amount, decimal p_Tst_Amount,decimal p_Total_Net_Amount)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);
                spUpdatePURCHASE_MASTER mPurchaseMaster = new spUpdatePURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.FMR = p_FMR;
                mPurchaseMaster.MR = p_MR;
                mPurchaseMaster.PUR_DISCOUNT = p_Pur_Discount;
                mPurchaseMaster.GST_AMOUNT = p_GST_Amount;
                mPurchaseMaster.TST_AMOUNT = p_Tst_Amount;
                mPurchaseMaster.TOTAL_NET_AMOUNT = p_Total_Net_Amount;
                mPurchaseMaster.IS_DELETED = false;
               
                mPurchaseMaster.ExecuteQuery();

                //Get Privouse Update Purchase Detail and Rollback
                //LedgerController LController = new LedgerController();

                //string VoucherNo = LController.SelectLedgerMaxDocumentId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID);

                DataTable dt = SelectPrivousePurchaseDetail(p_DISTRIBUTOR_ID, p_PURCHASE_MASTER_ID, mConnection, mTransaction);

                foreach (DataRow dr in dt.Rows)
                {
                    UspUpdatePurchaseDetailStock mPurchaseStock = new UspUpdatePurchaseDetailStock();
                    mPurchaseStock.Connection = mConnection;
                    mPurchaseStock.Transaction = mTransaction;
                    mPurchaseStock.TYPEID = p_TYPE_ID;
                    mPurchaseStock.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseStock.PURCHASE_DETAIL_ID = long.Parse(dr["PURCHASE_DETAIL_ID"].ToString());
                    mPurchaseStock.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                    mPurchaseStock.BATCH_NO = dr["BATCH_NO"].ToString().Trim();
                    mPurchaseStock.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseStock.ExecuteQuery();
                }

                spInsertPURCHASE_DETAIL2 mPurchaseDetail = new spInsertPURCHASE_DETAIL2();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {

                    //update stock;
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TDAMAGE = int.Parse(dr["TDAMAGE"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.FMR = decimal.Parse(dr["FMR"].ToString());
                    mPurchaseDetail.MR = decimal.Parse(dr["MR"].ToString());
                    mPurchaseDetail.PUR_DISCOUNT = decimal.Parse(dr["PUR_DISCOUNT"].ToString());

                    mPurchaseDetail.GST_AMOUNT = decimal.Parse(dr["GST_AMOUNT"].ToString());
                    mPurchaseDetail.TST_AMOUNT = decimal.Parse(dr["TST_AMOUNT"].ToString());
                    mPurchaseDetail.GST_RATE = decimal.Parse(dr["GST_RATE"].ToString());

                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_Principal;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }


        public bool UpdatePurchase(long p_PURCHASE_MASTER_ID, int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_Principal, decimal p_FMR, decimal p_MR, decimal p_Pur_Discount, decimal p_GST_Amount, decimal p_Tst_Amount, decimal p_Total_Net_Amount, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name, decimal p_AdjustmentAmount)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spUpdatePURCHASE_MASTER mPurchaseMaster = new spUpdatePURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_ADJUSTMENT_AMOUNT = p_AdjustmentAmount;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.FMR = p_FMR;
                mPurchaseMaster.MR = p_MR;
                mPurchaseMaster.PUR_DISCOUNT = p_Pur_Discount;
                mPurchaseMaster.GST_AMOUNT = p_GST_Amount;
                mPurchaseMaster.TST_AMOUNT = p_Tst_Amount;
                mPurchaseMaster.TOTAL_NET_AMOUNT = p_Total_Net_Amount;
                mPurchaseMaster.IS_DELETED = false;
                mPurchaseMaster.REMARKS = p_REMARKS;
                mPurchaseMaster.PRINCIPAL_ID = p_Principal;
                mPurchaseMaster.ExecuteQuery();
                #endregion

                #region Purchase Detail

                DataTable dt = SelectPrivousePurchaseDetail(p_DISTRIBUTOR_ID, p_PURCHASE_MASTER_ID, mConnection, mTransaction);

                foreach (DataRow dr in dt.Rows)
                {
                    UspUpdatePurchaseDetailStock mPurchaseStock = new UspUpdatePurchaseDetailStock();
                    mPurchaseStock.Connection = mConnection;
                    mPurchaseStock.Transaction = mTransaction;
                    mPurchaseStock.TYPEID = p_TYPE_ID;
                    mPurchaseStock.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseStock.PURCHASE_DETAIL_ID = long.Parse(dr["PURCHASE_DETAIL_ID"].ToString());
                    mPurchaseStock.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                    mPurchaseStock.BATCH_NO = dr["BATCH_NO"].ToString().Trim();
                    mPurchaseStock.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseStock.ExecuteQuery();
                }

                spInsertPURCHASE_DETAIL2 mPurchaseDetail = new spInsertPURCHASE_DETAIL2();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {

                    //update stock;
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TDAMAGE = int.Parse(dr["TDAMAGE"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.PUR_DISCOUNT = decimal.Parse(dr["PUR_DISCOUNT"].ToString());
                    mPurchaseDetail.ADJUSTMENT_AMOUNT = decimal.Parse(dr["ADJUSTMENT"].ToString());
                    mPurchaseDetail.GST_AMOUNT = decimal.Parse(dr["GST_AMOUNT"].ToString());
                    mPurchaseDetail.TST_AMOUNT = decimal.Parse(dr["TST_AMOUNT"].ToString());
                    mPurchaseDetail.GST_RATE = decimal.Parse(dr["GST_RATE"].ToString());
                    mPurchaseDetail.ExecuteQuery();
                    //if (p_TYPE_ID == 2)
                    //{
                    //    UspUpdateProcessSKU_Prices mSkuPrices = new UspUpdateProcessSKU_Prices();
                    //    mSkuPrices.Connection = mConnection;
                    //    mSkuPrices.Transaction = mTransaction;
                    //    mSkuPrices.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    //    mSkuPrices.DISTRIBUTOR_PRICE = decimal.Parse(dr["AVG_PRICE"].ToString());
                    //    mSkuPrices.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    //    mSkuPrices.ExecuteQuery();
                    //}
                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_Principal;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }

                #endregion

                #region Mapped Acount Selection

                CustomerDataController cd = new CustomerDataController();
                string CodeFileName = "";
                int CodeFileType = 0;
                DataTable dtMappSetting = cd.SelectMappingSetting(Constants.IntNullValue, Constants.IntNullValue);
                if (dtMappSetting != null || dtMappSetting.Rows.Count > 0)
                {
                    CodeFileName = dtMappSetting.Rows[0]["CODE_FILE"].ToString();
                    Configuration.GetAppMapping(CodeFileName);
                    CodeFileType = int.Parse(dtMappSetting.Rows[0]["CODE_TYPE"].ToString());
                }
                long p_PrincipalAccount_ID = 88;    //2002080001	Payable to (Principal Wise)
                string p_PrincipalAccount_Name = "";
                if (mPurchaseMaster.TYPE_ID == 2 || mPurchaseMaster.TYPE_ID == 3 || mPurchaseMaster.TYPE_ID == 4 ||
                    mPurchaseMaster.TYPE_ID == 5 || mPurchaseMaster.TYPE_ID == 6 || mPurchaseMaster.TYPE_ID == 8 ||
                    mPurchaseMaster.TYPE_ID == 9 || mPurchaseMaster.TYPE_ID == 10)
                {
                    DataTable dtAccount = cd.SelectAccountHeadMapping(1, Constants.IntNullValue, mPurchaseMaster.PRINCIPAL_ID, Constants.LongNullValue, Constants.IntNullValue);
                    if (dtAccount != null && dtAccount.Rows.Count > 0)
                    {
                        p_PrincipalAccount_ID = long.Parse(dtAccount.Rows[0]["Account_head_ID"].ToString());
                        p_PrincipalAccount_Name = dtAccount.Rows[0]["ACCOUNT_NAME"].ToString();
                    }
                }
                #endregion

                #region GL Voucher
                p_Voucher_No = "";
                LedgerController LController = new LedgerController();
                Configuration.GetAccountHead();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2) // Purchases
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 103, mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                        else
                        {
                            // For Other Clients
                            //"Purchases " + Remarks2
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, (mPurchaseMaster.TOTAL_AMOUNT - p_Tst_Amount - p_GST_Amount), 0, Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Sales_Tax_payable_against_supplies, (p_Tst_Amount + p_GST_Amount), 0, Remarks2, mTransaction, mConnection);

                            //p_PrincipalAccount_Name + " " + Remarks2
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                        }
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 3)// Purchase Return
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 103, 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                        else
                        {
                            // For Other Clients
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                        }
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 4)// Transfer IN
                {
                    string Remarks = "Default Transfer In Voucher";
                    string DocumentType = "Transfer IN";
                    if (CodeFileType == 1)
                    {
                        // For Atlas Equipment
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 103, mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.TransferInAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                    else
                    {
                        // For Other Clients
                        //string Remarks = "Default Transfer In Voucher";
                        //string DocumentType = "Transfer IN";
                        //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                        //{
                        //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                        //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        //}
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 5)// Transfer OUT
                {
                    string Remarks = "Default Transfer In Voucher";
                    string DocumentType = "Transfer IN";
                    if (CodeFileType == 1)
                    {
                        // For Atlas Equipment
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.TransferOutAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 103, 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    }
                    else
                    {
                        // For Other Clients
                        //string Remarks = "Default Transfer Out Voucher";
                        //string DocumentType = "Transfer Out";
                        //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                        //{
                        //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                        //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        //}
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 6)// Damage
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 103, 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        }
                        else
                        {
                            // For Other Clients
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                        }
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 10)// Damage claimable
                {
                    string Remarks = "Default Damage claimable Voucher";
                    string DocumentType = "Damage claimable";
                    if (CodeFileType == 1)
                    {
                        // For Atlas Equipment
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 103, 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                    else
                    {
                        // For Other Clients
                        //string Remarks = "Default Stock Damage(Claimable) Voucher";
                        //string DocumentType = "Stock Damage(claimable)";
                        //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                        //{
                        //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 897, 0, mPurchaseMaster.TOTAL_AMOUNT, "Purchases " + Remarks, mTransaction, mConnection);
                        //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 845, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        //});
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 8)// Short
                {
                    string Remarks = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, 103, 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        }
                        else
                        {
                            // For Other Clients
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                        }
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 9)// Excess
                {
                    string Remarks = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, 103, mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock In Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        }
                        else
                        {
                            // For Other Clients
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                        }
                    }
                }
                #endregion
                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        public bool UpdatePurchaseforConfodent(long p_PURCHASE_MASTER_ID, int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_Principal, decimal p_FMR, decimal p_MR, decimal p_Pur_Discount, decimal p_GST_Amount, decimal p_Tst_Amount, decimal p_Total_Net_Amount, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spUpdatePURCHASE_MASTER mPurchaseMaster = new spUpdatePURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.FMR = p_FMR;
                mPurchaseMaster.MR = p_MR;
                mPurchaseMaster.PUR_DISCOUNT = p_Pur_Discount;
                mPurchaseMaster.GST_AMOUNT = p_GST_Amount;
                mPurchaseMaster.TST_AMOUNT = p_Tst_Amount;
                mPurchaseMaster.TOTAL_NET_AMOUNT = p_Total_Net_Amount;
                mPurchaseMaster.IS_DELETED = false;

                mPurchaseMaster.ExecuteQuery();
                #endregion

                #region Purchase Detail

                DataTable dt = SelectPrivousePurchaseDetail(p_DISTRIBUTOR_ID, p_PURCHASE_MASTER_ID, mConnection, mTransaction);

                foreach (DataRow dr in dt.Rows)
                {
                    UspUpdatePurchaseDetailStock mPurchaseStock = new UspUpdatePurchaseDetailStock();
                    mPurchaseStock.Connection = mConnection;
                    mPurchaseStock.Transaction = mTransaction;
                    mPurchaseStock.TYPEID = p_TYPE_ID;
                    mPurchaseStock.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseStock.PURCHASE_DETAIL_ID = long.Parse(dr["PURCHASE_DETAIL_ID"].ToString());
                    mPurchaseStock.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                    mPurchaseStock.BATCH_NO = dr["BATCH_NO"].ToString().Trim();
                    mPurchaseStock.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseStock.ExecuteQuery();
                }

                spInsertPURCHASE_DETAIL2 mPurchaseDetail = new spInsertPURCHASE_DETAIL2();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {

                    //update stock;
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TDAMAGE = int.Parse(dr["TDAMAGE"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;

                    mPurchaseDetail.PUR_DISCOUNT = decimal.Parse(dr["PUR_DISCOUNT"].ToString());

                    mPurchaseDetail.GST_AMOUNT = decimal.Parse(dr["GST_AMOUNT"].ToString());
                    mPurchaseDetail.TST_AMOUNT = decimal.Parse(dr["TST_AMOUNT"].ToString());
                    mPurchaseDetail.GST_RATE = decimal.Parse(dr["GST_RATE"].ToString());
                    mPurchaseDetail.TRADE_PRICE = decimal.Parse(dr["SKU_PRICE"].ToString());
                    mPurchaseDetail.ExecuteQuery();
                    if (p_TYPE_ID == 2)
                    {
                        UspUpdateProcessSKU_Prices mSkuPrices = new UspUpdateProcessSKU_Prices();
                        mSkuPrices.Connection = mConnection;
                        mSkuPrices.Transaction = mTransaction;
                        mSkuPrices.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                        mSkuPrices.DISTRIBUTOR_PRICE = decimal.Parse(dr["AVG_PRICE"].ToString());
                        mSkuPrices.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                        mSkuPrices.ExecuteQuery();
                    }
                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_Principal;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }

                #endregion

                #region GL Voucher
                p_Voucher_No = "";
                LedgerController LController = new LedgerController();
                Configuration.GetAccountHead();

                CustomerDataController cd = new CustomerDataController();
                long p_PrincipalAccount_ID = 88;//2002080001	Payable to (Principal Wise)
                string p_PrincipalAccount_Name = "Payable to (Principal Wise)";
                if (mPurchaseMaster.TYPE_ID == 2 || mPurchaseMaster.TYPE_ID == 3)// purchase and purchase return
                {
                    DataTable dtAccount = cd.SelectAccountHeadMapping(1, Constants.IntNullValue, mPurchaseMaster.SOLD_FROM, Constants.LongNullValue, Constants.IntNullValue);
                    if (dtAccount != null && dtAccount.Rows.Count > 0)
                    {
                        p_PrincipalAccount_ID = long.Parse(dtAccount.Rows[0]["ACCOUNT_HEAD_ID"].ToString());
                        p_PrincipalAccount_Name = dtAccount.Rows[0]["ACCOUNT_NAME"].ToString();
                    }
                }

                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2) // Purchases
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        //"Purchases " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                        //p_PrincipalAccount_Name + " " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 3)// Purchase Return
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 4)// Transfer IN
                {
                    //string Remarks = "Default Transfer In Voucher";
                    //string DocumentType = "Transfer IN";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 5)// Transfer OUT
                {
                    //string Remarks = "Default Transfer Out Voucher";
                    //string DocumentType = "Transfer Out";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.StockInHand), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 6)// Damage
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 10)// Damage claimable
                {
                    //string Remarks = "Default Stock Damage(Claimable) Voucher";
                    //string DocumentType = "Stock Damage(claimable)";
                    //if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    //{
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 897, 0, mPurchaseMaster.TOTAL_AMOUNT, "Purchases " + Remarks, mTransaction, mConnection);
                    //    LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 845, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                    //}
                }
                else if (mPurchaseMaster.TYPE_ID == 8)// Short
                {
                    string Remarks = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 9)// Excess
                {
                    string Remarks = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                #endregion
                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }




        public bool UpdatePurchaseDocumentStock(long p_PURCHASE_MASTER_ID, int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_Principal, int p_Damage_Type, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spUpdatePURCHASE_MASTER mPurchaseMaster = new spUpdatePURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.PRINCIPAL_ID = p_Principal;
                mPurchaseMaster.DAMAGE_TYPE = p_Damage_Type;
                mPurchaseMaster.ExecuteQuery();
                #endregion

                //Get Privouse Update Purchase Detail and Rollback
                //LedgerController LController = new LedgerController();
                //string VoucherNo = LController.SelectLedgerMaxDocumentId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID);

                #region Purchase Detail
                DataTable dt = SelectPrivousePurchaseDetail(p_DISTRIBUTOR_ID, p_PURCHASE_MASTER_ID, mConnection, mTransaction);

                foreach (DataRow dr in dt.Rows)
                {
                    UspUpdatePurchaseDetailStock mPurchaseStock = new UspUpdatePurchaseDetailStock();
                    mPurchaseStock.Connection = mConnection;
                    mPurchaseStock.Transaction = mTransaction;
                    mPurchaseStock.TYPEID = p_TYPE_ID;
                    mPurchaseStock.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseStock.PURCHASE_DETAIL_ID = long.Parse(dr["PURCHASE_DETAIL_ID"].ToString());
                    mPurchaseStock.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                    mPurchaseStock.BATCH_NO = dr["BATCH_NO"].ToString().Trim();
                    mPurchaseStock.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseStock.ExecuteQuery();
                }

                spInsertPURCHASE_DETAIL2 mPurchaseDetail = new spInsertPURCHASE_DETAIL2();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {

                    //update stock;
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TDAMAGE = int.Parse(dr["TDAMAGE"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_Principal;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                #endregion

                #region GL Voucher
                p_Voucher_No = "";
                LedgerController LController = new LedgerController();
                Configuration.GetAccountHead();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2) // Purchases
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        //"Purchases " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                        //p_PrincipalAccount_Name + " " + Remarks2
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 3)// Purchase Return
                {
                    string ManualRemarks = "";
                    if (p_REMARKS.Equals(""))
                    {
                        ManualRemarks = p_REMARKS;
                    }
                    else
                    {
                        ManualRemarks = " / " + p_REMARKS;
                    }
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER + " / " + p_Principal_Name + " / INV # " + Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID) + ManualRemarks;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Remarks2, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Payable_to_Principal_Wise, mPurchaseMaster.TOTAL_AMOUNT, 0, Remarks2, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 4)// Transfer IN
                {
                    string Remarks = "Default Transfer In Voucher";
                    string DocumentType = "Transfer In";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);

                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.In_Transit_Stock, 0,mPurchaseMaster.TOTAL_AMOUNT, Constants.In_Transit_Stock_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 5)// Transfer OUT
                {
                    string Remarks = "Default Transfer Out Voucher";
                    string DocumentType = "Transfer Out";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.In_Transit_Stock, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.In_Transit_Stock_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, Constants.Gujranwala_Stock_In_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Gujranwala_Stock_In_Hand_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 6)// Damage
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 10)// Damage claimable
                {
                    string Remarks = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, Constants.Ancillary_Cost, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Ancillary_Cost_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 8)// Short
                {
                    string Remarks = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                else if (mPurchaseMaster.TYPE_ID == 9)// Excess
                {
                    string Remarks = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Stock_in_Hand, mPurchaseMaster.TOTAL_AMOUNT, 0, Constants.Stock_in_Hand_Name + " " + Remarks, mTransaction, mConnection);
                        LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, Constants.Special_Discount, 0, mPurchaseMaster.TOTAL_AMOUNT, Constants.Special_Discount_Name + " " + Remarks, mTransaction, mConnection);
                    }
                }
                #endregion
                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }
        public bool PostPendingDocument(long p_PURCHASE_MASTER_ID,int p_Type_Id,int p_Distributor_Id,int p_Posting)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                spUpdatePURCHASE_MASTER mPurchaseMaster = new spUpdatePURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                mPurchaseMaster.DISTRIBUTOR_ID = p_Distributor_Id;
                mPurchaseMaster.TYPE_ID = p_Type_Id;  
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.ExecuteQuery();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }
        public DataTable SelectPurchaseOrderDocumentNo(long p_PURCHASE_ORDER_ID, int p_DISTRIBUTOR_ID, int p_TYPE_ID, int saleOrderID)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                spSelectPURCHASE_ORDER mPurchaseMaster = new spSelectPURCHASE_ORDER();
                mPurchaseMaster.Connection = mConnection;
                //mPurchaseMaster.PURCHASE_ORDER_ID = p_PURCHASE_ORDER_ID;
                //mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                //mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.PURCHASE_ORDER_ID = saleOrderID;
                DataTable dt = mPurchaseMaster.ExecuteTable();
                return dt;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }
        public DataTable SelectPurchaseOrderDocumentNo2(long p_PURCHASE_ORDER_ID, int p_DISTRIBUTOR_ID, int p_TYPE_ID, int saleInvoiceID)
        { // by safdar 
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                spSelectPURCHASE_ORDER3 mPurchaseMaster = new spSelectPURCHASE_ORDER3();
                mPurchaseMaster.Connection = mConnection;
                //mPurchaseMaster.PURCHASE_ORDER_ID = p_PURCHASE_ORDER_ID;
                //mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                //mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.DOCUMENT_DATE = Constants.DateNullValue;
                mPurchaseMaster.SALE_INVOICE_ID = saleInvoiceID;
                DataTable dt = mPurchaseMaster.ExecuteTable();
                return dt;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }
        public DataTable SelectPurchaseOrderDocumentNo(long p_PURCHASE_ORDER_ID, int p_DISTRIBUTOR_ID, int p_TYPE_ID, DateTime p_DOCUMENT_DATE)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
               
                spSelectPURCHASE_ORDER mPurchaseMaster = new spSelectPURCHASE_ORDER();
                mPurchaseMaster.Connection = mConnection;
               
                mPurchaseMaster.document_date = p_DOCUMENT_DATE;
                DataTable dt = mPurchaseMaster.ExecuteTable();
                return dt;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }
        public DataTable SelectPurchaseOrderDocumentNo2(int p_SALE_INVOICE_ID, DateTime p_DOCUMENT_DATE)
        {
            // by safdar to use in view load pass form
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                // Origional is spSelectPURCHASE_ORDER ..by safdar .. from sale invoice master instead of sale order master
                spSelectPURCHASE_ORDER3 mPurchaseMaster = new spSelectPURCHASE_ORDER3();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.SALE_INVOICE_ID = p_SALE_INVOICE_ID;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                DataTable dt = mPurchaseMaster.ExecuteTable();
                return dt;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }
        public DataTable SelectPurchaseOrderDocumentNo4(int UserID, DateTime p_DOCUMENT_DATE)
        {
            // by safdar to use in view load pass form
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                // Origional is spSelectPURCHASE_ORDER ..by safdar .. from sale invoice master instead of sale order master
                spSelectPURCHASE_ORDER4 mPurchaseMaster = new spSelectPURCHASE_ORDER4();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.SALE_INVOICE_ID = UserID;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                DataTable dt = mPurchaseMaster.ExecuteTable();
                return dt;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        public DataTable SelectPurchaseOrderDetail(long p_SALE_ORDER_ID)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                spSelectPURCHASE_ORDER_DETAIL mPurchaseDetail = new spSelectPURCHASE_ORDER_DETAIL();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.SALE_ORDER_ID = p_SALE_ORDER_ID;
                //mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                DataTable dt = mPurchaseDetail.ExecuteTable();
                return dt;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        public DataTable SelectPurchaseOrderDetail2(int p_SALE_INVOICE_ID)
        {// by safdar to use in load pass 
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                // Origional was spSelectPURCHASE_ORDER_DETAIL
                spSelectPURCHASE_INVOICE_DETAIL mPurchaseDetail = new spSelectPURCHASE_INVOICE_DETAIL();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.SALE_INVOICE_ID = p_SALE_INVOICE_ID;
                //mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                DataTable dt = mPurchaseDetail.ExecuteTable();
                return dt;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }


        public bool UpdatePurchaseOrder(long p_PURCHASE_ORDER_ID, int p_TYPE_ID)
        {
            try
            {
                //mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                //mConnection.Open();

                //mTransaction = ProviderFactory.GetTransaction(mConnection);
                //spUpdatePURCHASE_ORDER mPurchaseMaster = new spUpdatePURCHASE_ORDER();
                //mPurchaseMaster.Connection = mConnection;
                //mPurchaseMaster.Transaction = mTransaction;
                //mPurchaseMaster.PURCHASE_ORDER_ID = p_PURCHASE_ORDER_ID;
                //mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                //mPurchaseMaster.ExecuteQuery();

                //mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        public DataTable selectLoadPassDocumentNo(int p_Distributor_id,int p_TOWN_ID, long p_AREA_ID, int p_PRINCIPAL_ID, int p_ORDERBOOKER_ID, int p_DELIVERYMAN_ID,DateTime p_DOCUMENT_DATE)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                spSelectLoadPassDocumentNo mPurchaseDetail = new spSelectLoadPassDocumentNo();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.DISTRIBUTOR_ID = p_Distributor_id;
                mPurchaseDetail.TOWN_ID = p_TOWN_ID;
                mPurchaseDetail.AREA_ID = p_AREA_ID;
                mPurchaseDetail.PRINCIPAL_ID = p_PRINCIPAL_ID;
                mPurchaseDetail.ORDERBOOKER_ID = p_ORDERBOOKER_ID;
                mPurchaseDetail.DELIVERYMAN_ID = p_DELIVERYMAN_ID;
                mPurchaseDetail.DOCUMENT_DATE = p_DOCUMENT_DATE;
                //mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                DataTable dt = mPurchaseDetail.ExecuteTable();
                return dt;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }

        #endregion

        #endregion

        #region M. IJAZ JAMIL AKHTAR
        public bool InsertPurchase_AE(int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_PrincipalId, decimal p_FMR, decimal p_MR, decimal p_Pur_Discount, decimal p_Gst_Amount, decimal p_Tst_Amount, decimal p_total_Net_Amount, DateTime p_INVOICE_DATE)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);
                spInsertPURCHASE_MASTER_AE mPurchaseMaster = new spInsertPURCHASE_MASTER_AE();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.TIME_STAMP = DateTime.Now;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.PRINCIPAL_ID = p_PrincipalId;
                mPurchaseMaster.FMR = p_FMR;
                mPurchaseMaster.MR = p_MR;
                mPurchaseMaster.PUR_DISCOUNT = p_Pur_Discount;
                mPurchaseMaster.GST_AMOUNT = p_Gst_Amount;
                mPurchaseMaster.TST_AMOUNT = p_Tst_Amount;
                mPurchaseMaster.TOTAL_NET_AMOUNT = p_total_Net_Amount;
                mPurchaseMaster.IS_DELETED = false;
                mPurchaseMaster.INVOICE_DATE = p_INVOICE_DATE;
                mPurchaseMaster.ExecuteQuery();

                spInsertPURCHASE_DETAIL2 mPurchaseDetail = new spInsertPURCHASE_DETAIL2();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.GST_AMOUNT = decimal.Parse(dr["GST_AMOUNT"].ToString());
                    mPurchaseDetail.TST_AMOUNT = decimal.Parse(dr["TST_AMOUNT"].ToString());
                    mPurchaseDetail.GST_RATE = decimal.Parse(dr["GST_RATE"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.TDAMAGE = int.Parse(dr["TDAMAGE"].ToString());
                    mPurchaseDetail.FMR = decimal.Parse(dr["FMR"].ToString());
                    mPurchaseDetail.MR = decimal.Parse(dr["MR"].ToString());
                    mPurchaseDetail.PUR_DISCOUNT = decimal.Parse(dr["PUR_DISCOUNT"].ToString());
                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_PrincipalId;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                #region Mapped Acount Selection

                CustomerDataController cd = new CustomerDataController();
                string p_Voucher_No = "";
                string CodeFileName = "";
                int CodeFileType = 0;
                DataTable dtMappSetting = cd.SelectMappingSetting(Constants.IntNullValue, Constants.IntNullValue);
                if (dtMappSetting != null || dtMappSetting.Rows.Count > 0)
                {
                    CodeFileName = dtMappSetting.Rows[0]["CODE_FILE"].ToString();
                    Configuration.GetAppMapping(CodeFileName);
                    CodeFileType = int.Parse(dtMappSetting.Rows[0]["CODE_TYPE"].ToString());
                }
                long p_PrincipalAccount_ID = 88;    //2002080001	Payable to (Principal Wise)
                string p_PrincipalAccount_Name = "";
                if (mPurchaseMaster.TYPE_ID == 2 || mPurchaseMaster.TYPE_ID == 3 || mPurchaseMaster.TYPE_ID == 4 ||
                    mPurchaseMaster.TYPE_ID == 5 || mPurchaseMaster.TYPE_ID == 6 || mPurchaseMaster.TYPE_ID == 8 ||
                    mPurchaseMaster.TYPE_ID == 9 || mPurchaseMaster.TYPE_ID == 10)
                {
                    DataTable dtAccount = cd.SelectAccountHeadMapping(1, Constants.IntNullValue, mPurchaseMaster.PRINCIPAL_ID, Constants.LongNullValue, Constants.IntNullValue);
                    if (dtAccount != null && dtAccount.Rows.Count > 0)
                    {
                        p_PrincipalAccount_ID = long.Parse(dtAccount.Rows[0]["Account_head_ID"].ToString());
                        p_PrincipalAccount_Name = dtAccount.Rows[0]["ACCOUNT_NAME"].ToString();
                    }
                }
                #endregion

                #region GL Voucher
                LedgerController LController = new LedgerController();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2)// Purchase
                {
                    #region Purchases
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 3)  // Purchase Return
                {
                    #region Purchase Return
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 4)  // Transfer IN
                {
                    #region Transfer In
                    string Remarks = "Default Transfer In Voucher";
                    string DocumentType = "Transfer IN";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.TransferInAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 5)  // Transfer OUT
                {
                    #region Transfer Out
                    string Remarks = "Default Transfer Out Voucher";
                    string DocumentType = "Transfer Out";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.TransferOutAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 6)  // Damage
                {
                    #region Damage
                    string Remarks = "Default Stock Damage Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 7) // Opening Stock
                {
                    #region Opening Stock
                    string Remarks = "Default Opening Stock Voucher";
                    string DocumentType = "Opening Stock";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock In Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.OpeningStock), 0, mPurchaseMaster.TOTAL_AMOUNT, "Opening at 31/8/19 " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 10) // Damage claimable
                {
                    #region Damage Claimable
                    string Remarks = "Default Stock Damage(Claimable) Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Damage(Claimable) Voucher";
                    string DocumentType = "Stock Damage(Claimable)";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 8)  // Short
                {
                    #region Short
                    string Remarks = "Default Stock Short Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 9)  // Excess
                {
                    #region Excess
                    string Remarks = "Default Stock Excess Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                #endregion
                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }
        public bool UpdatePurchase_AE(long p_PURCHASE_MASTER_ID, int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_Principal, decimal p_FMR, decimal p_MR, decimal p_Pur_Discount, decimal p_GST_Amount, decimal p_Tst_Amount, decimal p_Total_Net_Amount, DateTime p_INVOICE_DATE)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);
                spUpdatePURCHASE_MASTER_AE mPurchaseMaster = new spUpdatePURCHASE_MASTER_AE();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.FMR = p_FMR;
                mPurchaseMaster.MR = p_MR;
                mPurchaseMaster.PUR_DISCOUNT = p_Pur_Discount;
                mPurchaseMaster.GST_AMOUNT = p_GST_Amount;
                mPurchaseMaster.TST_AMOUNT = p_Tst_Amount;
                mPurchaseMaster.TOTAL_NET_AMOUNT = p_Total_Net_Amount;
                mPurchaseMaster.IS_DELETED = false;
                mPurchaseMaster.INVOICE_DATE = p_INVOICE_DATE;
                mPurchaseMaster.ExecuteQuery();

                //Get Privouse Update Purchase Detail and Rollback
                //LedgerController LController = new LedgerController();
                //string VoucherNo = LController.SelectLedgerMaxDocumentId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID);
                DataTable dt = SelectPrivousePurchaseDetail(p_DISTRIBUTOR_ID, p_PURCHASE_MASTER_ID, mConnection, mTransaction);
                foreach (DataRow dr in dt.Rows)
                {
                    UspUpdatePurchaseDetailStock mPurchaseStock = new UspUpdatePurchaseDetailStock();
                    mPurchaseStock.Connection = mConnection;
                    mPurchaseStock.Transaction = mTransaction;
                    mPurchaseStock.TYPEID = p_TYPE_ID;
                    mPurchaseStock.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseStock.PURCHASE_DETAIL_ID = long.Parse(dr["PURCHASE_DETAIL_ID"].ToString());
                    mPurchaseStock.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                    mPurchaseStock.BATCH_NO = dr["BATCH_NO"].ToString().Trim();
                    mPurchaseStock.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseStock.ExecuteQuery();
                }
                spInsertPURCHASE_DETAIL2 mPurchaseDetail = new spInsertPURCHASE_DETAIL2();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    //update stock;
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TDAMAGE = int.Parse(dr["TDAMAGE"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.FMR = decimal.Parse(dr["FMR"].ToString());
                    mPurchaseDetail.MR = decimal.Parse(dr["MR"].ToString());
                    mPurchaseDetail.PUR_DISCOUNT = decimal.Parse(dr["PUR_DISCOUNT"].ToString());
                    mPurchaseDetail.GST_AMOUNT = decimal.Parse(dr["GST_AMOUNT"].ToString());
                    mPurchaseDetail.TST_AMOUNT = decimal.Parse(dr["TST_AMOUNT"].ToString());
                    mPurchaseDetail.GST_RATE = decimal.Parse(dr["GST_RATE"].ToString());
                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_Principal;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }

                #region Mapped Acount Selection
                CustomerDataController cd = new CustomerDataController();
                string p_Voucher_No = "";
                string CodeFileName = "";
                int CodeFileType = 0;
                DataTable dtMappSetting = cd.SelectMappingSetting(Constants.IntNullValue, Constants.IntNullValue);
                if (dtMappSetting != null || dtMappSetting.Rows.Count > 0)
                {
                    CodeFileName = dtMappSetting.Rows[0]["CODE_FILE"].ToString();
                    Configuration.GetAppMapping(CodeFileName);
                    CodeFileType = int.Parse(dtMappSetting.Rows[0]["CODE_TYPE"].ToString());
                }
                long p_PrincipalAccount_ID = 88;    //2002080001	Payable to (Principal Wise)
                string p_PrincipalAccount_Name = "";
                if (mPurchaseMaster.TYPE_ID == 2 || mPurchaseMaster.TYPE_ID == 3 || mPurchaseMaster.TYPE_ID == 4 ||
                    mPurchaseMaster.TYPE_ID == 5 || mPurchaseMaster.TYPE_ID == 6 || mPurchaseMaster.TYPE_ID == 8 ||
                    mPurchaseMaster.TYPE_ID == 9 || mPurchaseMaster.TYPE_ID == 10)
                {
                    DataTable dtAccount = cd.SelectAccountHeadMapping(1, Constants.IntNullValue, p_Principal, Constants.LongNullValue, Constants.IntNullValue);
                    if (dtAccount != null && dtAccount.Rows.Count > 0)
                    {
                        p_PrincipalAccount_ID = long.Parse(dtAccount.Rows[0]["Account_head_ID"].ToString());
                        p_PrincipalAccount_Name = dtAccount.Rows[0]["ACCOUNT_NAME"].ToString();
                    }
                }
                #endregion

                #region GL Voucher
                LedgerController LController = new LedgerController();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2)// Purchase
                {
                    #region Purchases
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 3)  // Purchase Return
                {
                    #region Purchase Return
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 4)  // Transfer IN
                {
                    #region Transfer In
                    string Remarks = "Default Transfer In Voucher";
                    string DocumentType = "Transfer IN";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.TransferInAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 5)  // Transfer OUT
                {
                    #region Transfer Out
                    string Remarks = "Default Transfer Out Voucher";
                    string DocumentType = "Transfer Out";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.TransferOutAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 6)  // Damage
                {
                    #region Damage
                    string Remarks = "Default Stock Damage Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 7) // Opening Stock
                {
                    #region Opening Stock
                    string Remarks = "Default Opening Stock Voucher";
                    string DocumentType = "Opening Stock";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock In Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.OpeningStock), 0, mPurchaseMaster.TOTAL_AMOUNT, "Opening at 31/8/19 " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 10) // Damage claimable
                {
                    #region Damage Claimable
                    string Remarks = "Default Stock Damage(Claimable) Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Damage(Claimable) Voucher";
                    string DocumentType = "Stock Damage(Claimable)";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 8)  // Short
                {
                    #region Short
                    string Remarks = "Default Stock Short Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 9)  // Excess
                {
                    #region Excess
                    string Remarks = "Default Stock Excess Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                #endregion
                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }
        public bool InsertPurchaseDocument_AE(int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_PrincipalId)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);
                spInsertPURCHASE_MASTER mPurchaseMaster = new spInsertPURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.TIME_STAMP = DateTime.Now;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.PRINCIPAL_ID = p_PrincipalId;
                mPurchaseMaster.ExecuteQuery();

                spInsertPURCHASE_DETAIL mPurchaseDetail = new spInsertPURCHASE_DETAIL();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;
                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_PrincipalId;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }

                CustomerDataController cd = new CustomerDataController();
                string p_Voucher_No = "";
                string CodeFileName = "";
                int CodeFileType = 0;
                DataTable dtMappSetting = cd.SelectMappingSetting(Constants.IntNullValue, Constants.IntNullValue);

                if (dtMappSetting != null || dtMappSetting.Rows.Count > 0)
                {
                    CodeFileName = dtMappSetting.Rows[0]["CODE_FILE"].ToString();
                    Configuration.GetAppMapping(CodeFileName);
                    CodeFileType = int.Parse(dtMappSetting.Rows[0]["CODE_TYPE"].ToString());
                }

                #region Mapped Acount Selection
                long p_PrincipalAccount_ID = 88;    //2002080001	Payable to (Principal Wise)
                string p_PrincipalAccount_Name = "";
                if (mPurchaseMaster.TYPE_ID == 8 || mPurchaseMaster.TYPE_ID == 9)   // Short & Access
                {
                    DataTable dtAccount = cd.SelectAccountHeadMapping(1, Constants.IntNullValue, mPurchaseMaster.PRINCIPAL_ID, Constants.LongNullValue, Constants.IntNullValue);
                    if (dtAccount != null && dtAccount.Rows.Count > 0)
                    {
                        p_PrincipalAccount_ID = long.Parse(dtAccount.Rows[0]["Account_head_ID"].ToString());
                        p_PrincipalAccount_Name = dtAccount.Rows[0]["ACCOUNT_NAME"].ToString();

                    }
                }
                #endregion

                #region GL Voucher
                LedgerController LController = new LedgerController();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2)// Purchase
                {
                    #region Purchases
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 3)  // Purchase Return
                {
                    #region Purchase Return
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 4)  // Transfer IN
                {
                    #region Transfer In
                    string Remarks = "Default Transfer In Voucher";
                    string DocumentType = "Transfer IN";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.TransferInAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 5)  // Transfer OUT
                {
                    #region Transfer Out
                    string Remarks = "Default Transfer Out Voucher";
                    string DocumentType = "Transfer Out";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.TransferOutAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 6)  // Damage
                {
                    #region Damage
                    string Remarks = "Default Stock Damage Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 7) // Opening Stock
                {
                    #region Opening Stock
                    string Remarks = "Default Opening Stock Voucher";
                    string DocumentType = "Opening Stock";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock In Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.OpeningStock), 0, mPurchaseMaster.TOTAL_AMOUNT, "Opening at 31/8/19 " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 10) // Damage claimable
                {
                    #region Damage Claimable
                    string Remarks = "Default Stock Damage(Claimable) Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Damage(Claimable) Voucher";
                    string DocumentType = "Stock Damage(Claimable)";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 8)  // Short
                {
                    #region Short
                    string Remarks = "Default Stock Short Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 9)  // Excess
                {
                    #region Excess
                    string Remarks = "Default Stock Excess Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                #endregion
                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }
        public bool UpdatePurchaseDocument_AE(long p_PURCHASE_MASTER_ID, int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_Principal)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);
                spUpdatePURCHASE_MASTER_AE mPurchaseMaster = new spUpdatePURCHASE_MASTER_AE();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.ExecuteQuery();

                //Get Privouse Update Purchase Detail and Rollback
                //LedgerController LController = new LedgerController();
                //string VoucherNo = LController.SelectLedgerMaxDocumentId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID);
                DataTable dt = SelectPrivousePurchaseDetail(p_DISTRIBUTOR_ID, p_PURCHASE_MASTER_ID, mConnection, mTransaction);
                foreach (DataRow dr in dt.Rows)
                {
                    UspUpdatePurchaseDetailStock mPurchaseStock = new UspUpdatePurchaseDetailStock();
                    mPurchaseStock.Connection = mConnection;
                    mPurchaseStock.Transaction = mTransaction;
                    mPurchaseStock.TYPEID = p_TYPE_ID;
                    mPurchaseStock.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseStock.PURCHASE_DETAIL_ID = long.Parse(dr["PURCHASE_DETAIL_ID"].ToString());
                    mPurchaseStock.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                    mPurchaseStock.BATCH_NO = dr["BATCH_NO"].ToString().Trim();
                    mPurchaseStock.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseStock.ExecuteQuery();
                }

                spInsertPURCHASE_DETAIL mPurchaseDetail = new spInsertPURCHASE_DETAIL();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    //update stock;
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_Principal;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                CustomerDataController cd = new CustomerDataController();
                string p_Voucher_No = "";
                string CodeFileName = "";
                int CodeFileType = 0;
                DataTable dtMappSetting = cd.SelectMappingSetting(Constants.IntNullValue, Constants.IntNullValue);
                if (dtMappSetting != null || dtMappSetting.Rows.Count > 0)
                {
                    CodeFileName = dtMappSetting.Rows[0]["CODE_FILE"].ToString();
                    Configuration.GetAppMapping(CodeFileName);
                    CodeFileType = int.Parse(dtMappSetting.Rows[0]["CODE_TYPE"].ToString());
                }
                #region Mapped Acount Selection
                long p_PrincipalAccount_ID = 88;    //2002080001	Payable to (Principal Wise)
                string p_PrincipalAccount_Name = "";
                if (mPurchaseMaster.TYPE_ID == 8 || mPurchaseMaster.TYPE_ID == 9)   // Short & Access
                {
                    DataTable dtAccount = cd.SelectAccountHeadMapping(1, Constants.IntNullValue, p_Principal, Constants.LongNullValue, Constants.IntNullValue);
                    if (dtAccount != null && dtAccount.Rows.Count > 0)
                    {
                        p_PrincipalAccount_ID = long.Parse(dtAccount.Rows[0]["Account_head_ID"].ToString());
                        p_PrincipalAccount_Name = dtAccount.Rows[0]["ACCOUNT_NAME"].ToString();
                    }
                }
                #endregion

                #region GL Voucher
                LedgerController LController = new LedgerController();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2)// Purchase
                {
                    #region Purchases
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 3)  // Purchase Return
                {
                    #region Purchase Return
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 4)  // Transfer IN
                {
                    #region Transfer In
                    string Remarks = "Default Transfer In Voucher";
                    string DocumentType = "Transfer IN";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.TransferInAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 5)  // Transfer OUT
                {
                    #region Transfer Out
                    string Remarks = "Default Transfer Out Voucher";
                    string DocumentType = "Transfer Out";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.TransferOutAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 6)  // Damage
                {
                    #region Damage
                    string Remarks = "Default Stock Damage Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 7) // Opening Stock
                {
                    #region Opening Stock
                    string Remarks = "Default Opening Stock Voucher";
                    string DocumentType = "Opening Stock";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock In Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.OpeningStock), 0, mPurchaseMaster.TOTAL_AMOUNT, "Opening at 31/8/19 " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 10) // Damage claimable
                {
                    #region Damage Claimable
                    string Remarks = "Default Stock Damage(Claimable) Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Damage(Claimable) Voucher";
                    string DocumentType = "Stock Damage(Claimable)";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, p_Principal, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, p_Principal, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 8)  // Short
                {
                    #region Short
                    string Remarks = "Default Stock Short Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 9)  // Excess
                {
                    #region Excess
                    string Remarks = "Default Stock Excess Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                #endregion
                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }
        public bool UpdatePurchaseDocumentStock_AE(long p_PURCHASE_MASTER_ID, int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_Principal, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spUpdatePURCHASE_MASTER_AE mPurchaseMaster = new spUpdatePURCHASE_MASTER_AE();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.PRINCIPAL_ID = p_Principal;
                mPurchaseMaster.ExecuteQuery();
                #endregion

                //Get Privouse Update Purchase Detail and Rollback
                //LedgerController LController = new LedgerController();
                //string VoucherNo = LController.SelectLedgerMaxDocumentId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID);

                #region Purchase Detail
                DataTable dt = SelectPrivousePurchaseDetail(p_DISTRIBUTOR_ID, p_PURCHASE_MASTER_ID, mConnection, mTransaction);

                foreach (DataRow dr in dt.Rows)
                {
                    UspUpdatePurchaseDetailStock mPurchaseStock = new UspUpdatePurchaseDetailStock();
                    mPurchaseStock.Connection = mConnection;
                    mPurchaseStock.Transaction = mTransaction;
                    mPurchaseStock.TYPEID = p_TYPE_ID;
                    mPurchaseStock.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseStock.PURCHASE_DETAIL_ID = long.Parse(dr["PURCHASE_DETAIL_ID"].ToString());
                    mPurchaseStock.PURCHASE_MASTER_ID = p_PURCHASE_MASTER_ID;
                    mPurchaseStock.BATCH_NO = dr["BATCH_NO"].ToString().Trim();
                    mPurchaseStock.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseStock.ExecuteQuery();
                }

                spInsertPURCHASE_DETAIL2 mPurchaseDetail = new spInsertPURCHASE_DETAIL2();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {

                    //update stock;
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TDAMAGE = int.Parse(dr["TDAMAGE"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_Principal;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                #endregion

                #region Mapped Acount Selection

                CustomerDataController cd = new CustomerDataController();
                string CodeFileName = "";
                int CodeFileType = 0;
                DataTable dtMappSetting = cd.SelectMappingSetting(Constants.IntNullValue, Constants.IntNullValue);
                if (dtMappSetting != null || dtMappSetting.Rows.Count > 0)
                {
                    CodeFileName = dtMappSetting.Rows[0]["CODE_FILE"].ToString();
                    Configuration.GetAppMapping(CodeFileName);
                    CodeFileType = int.Parse(dtMappSetting.Rows[0]["CODE_TYPE"].ToString());
                }
                long p_PrincipalAccount_ID = 88;    //2002080001	Payable to (Principal Wise)
                long p_PrincipalDiscountAccount_ID = 0;    //2002080001	Payable to (Principal Wise)
                string p_PrincipalAccount_Name = "";
                string p_PrincipalDiscountAccount_Name = "";
                if (mPurchaseMaster.TYPE_ID == 2 || mPurchaseMaster.TYPE_ID == 3 || mPurchaseMaster.TYPE_ID == 4 ||
                    mPurchaseMaster.TYPE_ID == 5 || mPurchaseMaster.TYPE_ID == 6 || mPurchaseMaster.TYPE_ID == 8 ||
                    mPurchaseMaster.TYPE_ID == 9 || mPurchaseMaster.TYPE_ID == 10)
                {
                    DataTable dtAccount = cd.SelectAccountHeadMapping(1, Constants.IntNullValue, mPurchaseMaster.PRINCIPAL_ID, Constants.LongNullValue, Constants.IntNullValue);
                    if (dtAccount != null && dtAccount.Rows.Count > 0)
                    {
                        p_PrincipalAccount_ID = long.Parse(dtAccount.Rows[0]["Account_head_ID"].ToString());
                        p_PrincipalAccount_Name = dtAccount.Rows[0]["ACCOUNT_NAME"].ToString();
                        p_PrincipalDiscountAccount_ID = long.Parse(dtAccount.Rows[0]["DISCOUNT_HEAD_ID"].ToString());
                        p_PrincipalDiscountAccount_Name = dtAccount.Rows[0]["DISCOUNT_HEAD_NAME"].ToString();
                    }
                }
                #endregion

                #region GL Voucher
                LedgerController LController = new LedgerController();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2)// Purchase
                {
                    #region Purchases
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 3)  // Purchase Return
                {
                    #region Purchase Return
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 4)  // Transfer IN
                {
                    #region Transfer In
                    string Remarks = "Default Transfer In Voucher";
                    string DocumentType = "Transfer IN";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.TransferInAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 5)  // Transfer OUT
                {
                    #region Transfer Out
                    string Remarks = "Default Transfer Out Voucher";
                    string DocumentType = "Transfer Out";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.TransferOutAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 6)  // Damage
                {
                    #region Damage
                    string Remarks = "Default Stock Damage Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 890, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 7) // Opening Stock
                {
                    #region Opening Stock
                    string Remarks = "Default Opening Stock Voucher";
                    string DocumentType = "Opening Stock";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock In Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.OpeningStock), 0, mPurchaseMaster.TOTAL_AMOUNT, "Opening at 31/8/19 " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 10) // Damage claimable
                {
                    #region Damage Claimable
                    string Remarks = "Default Stock Damage(Claimable) Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Damage(Claimable) Voucher";
                    string DocumentType = "Stock Damage(Claimable)";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 890, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 8)  // Short
                {
                    #region Short
                    string Remarks = "Default Stock Short Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 9)  // Excess
                {
                    #region Excess
                    string Remarks = "Default Stock Excess Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                #endregion

                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }
        public bool InsertPurchaseDocumentStock_AE(int p_DISTRIBUTOR_ID, string p_ORDER_NUMBER, int p_TYPE_ID, DateTime p_DOCUMENT_DATE, int p_SOLD_TO, int p_SOLD_FROM, decimal p_TOTAL_AMOUNT, bool p_IS_DELETE, DataTable dtPurchaseDetail, int p_Posting, string p_BuiltyNo, int p_UserId, int p_PrincipalId, out string p_Voucher_No, string p_REMARKS, string p_Principal_Name)
        {
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                mTransaction = ProviderFactory.GetTransaction(mConnection);

                #region Purchase Master
                spInsertPURCHASE_MASTER mPurchaseMaster = new spInsertPURCHASE_MASTER();
                mPurchaseMaster.Connection = mConnection;
                mPurchaseMaster.Transaction = mTransaction;
                mPurchaseMaster.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mPurchaseMaster.TYPE_ID = p_TYPE_ID;
                mPurchaseMaster.ORDER_NUMBER = p_ORDER_NUMBER;
                mPurchaseMaster.SOLD_FROM = p_SOLD_FROM;
                mPurchaseMaster.DOCUMENT_DATE = p_DOCUMENT_DATE;
                mPurchaseMaster.SOLD_TO = p_SOLD_TO;
                mPurchaseMaster.TOTAL_AMOUNT = p_TOTAL_AMOUNT;
                mPurchaseMaster.USER_ID = p_UserId;
                mPurchaseMaster.TIME_STAMP = DateTime.Now;
                mPurchaseMaster.LAST_UPDATE = DateTime.Now;
                mPurchaseMaster.POSTING = p_Posting;
                mPurchaseMaster.BUILTY_NO = p_BuiltyNo;
                mPurchaseMaster.PRINCIPAL_ID = p_PrincipalId;

                mPurchaseMaster.ExecuteQuery();
                #endregion

                #region Purchase Detail

                spInsertPURCHASE_DETAIL2 mPurchaseDetail = new spInsertPURCHASE_DETAIL2();
                mPurchaseDetail.Connection = mConnection;
                mPurchaseDetail.Transaction = mTransaction;

                foreach (DataRow dr in dtPurchaseDetail.Rows)
                {
                    mPurchaseDetail.PURCHASE_MASTER_ID = mPurchaseMaster.PURCHASE_MASTER_ID;
                    mPurchaseDetail.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                    mPurchaseDetail.SKU_ID = int.Parse(dr["SKU_ID"].ToString());
                    mPurchaseDetail.BATCH_NO = dr["BATCH_NO"].ToString();
                    mPurchaseDetail.PRICE = decimal.Parse(dr["PRICE"].ToString());
                    mPurchaseDetail.QUANTITY = int.Parse(dr["Total_Unit"].ToString());
                    mPurchaseDetail.FREE_SKU = int.Parse(dr["FREE_SKU"].ToString());
                    mPurchaseDetail.AMOUNT = decimal.Parse(dr["AMOUNT"].ToString());
                    mPurchaseDetail.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mPurchaseDetail.TIME_STAMP = p_DOCUMENT_DATE;
                    mPurchaseDetail.TDAMAGE = int.Parse(dr["TDAMAGE"].ToString());
                    mPurchaseDetail.ExecuteQuery();

                    UspProcessStockRegister mStockUpdate = new UspProcessStockRegister();
                    mStockUpdate.Connection = mConnection;
                    mStockUpdate.Transaction = mTransaction;
                    mStockUpdate.PRINCIPAL_ID = p_PrincipalId;
                    mStockUpdate.TYPE_ID = mPurchaseMaster.TYPE_ID;
                    mStockUpdate.DISTRIBUTOR_ID = mPurchaseMaster.DISTRIBUTOR_ID;
                    mStockUpdate.STOCK_DATE = mPurchaseMaster.DOCUMENT_DATE;
                    mStockUpdate.SKU_ID = mPurchaseDetail.SKU_ID;
                    mStockUpdate.STOCK_QTY = mPurchaseDetail.QUANTITY;
                    mStockUpdate.FREE_QTY = mPurchaseDetail.FREE_SKU;
                    mStockUpdate.BATCHNO = mPurchaseDetail.BATCH_NO;
                    mStockUpdate.ExecuteQuery();
                }
                #endregion

                #region Mapped Acount Selection

                CustomerDataController cd = new CustomerDataController();
                string CodeFileName = "";
                int CodeFileType = 0;
                DataTable dtMappSetting = cd.SelectMappingSetting(Constants.IntNullValue, Constants.IntNullValue);
                if (dtMappSetting != null || dtMappSetting.Rows.Count > 0)
                {
                    CodeFileName = dtMappSetting.Rows[0]["CODE_FILE"].ToString();
                    Configuration.GetAppMapping(CodeFileName);
                    CodeFileType = int.Parse(dtMappSetting.Rows[0]["CODE_TYPE"].ToString());
                }
                long p_PrincipalAccount_ID = 88;    //2002080001	Payable to (Principal Wise)
                long p_PrincipalDiscountAccount_ID = 0;    //2002080001	Payable to (Principal Wise)
                string p_PrincipalAccount_Name = "";
                string p_PrincipalDiscountAccount_Name = "";
                if (mPurchaseMaster.TYPE_ID == 2 || mPurchaseMaster.TYPE_ID == 3 || mPurchaseMaster.TYPE_ID == 4 ||
                    mPurchaseMaster.TYPE_ID == 5 || mPurchaseMaster.TYPE_ID == 6 || mPurchaseMaster.TYPE_ID == 8 ||
                    mPurchaseMaster.TYPE_ID == 9 || mPurchaseMaster.TYPE_ID == 10)
                {
                    DataTable dtAccount = cd.SelectAccountHeadMapping(1, Constants.IntNullValue, mPurchaseMaster.PRINCIPAL_ID, Constants.LongNullValue, Constants.IntNullValue);
                    if (dtAccount != null && dtAccount.Rows.Count > 0)
                    {
                        p_PrincipalAccount_ID = long.Parse(dtAccount.Rows[0]["Account_head_ID"].ToString());
                        p_PrincipalAccount_Name = dtAccount.Rows[0]["ACCOUNT_NAME"].ToString();
                        p_PrincipalDiscountAccount_ID = long.Parse(dtAccount.Rows[0]["DISCOUNT_HEAD_ID"].ToString());
                        p_PrincipalDiscountAccount_Name = dtAccount.Rows[0]["DISCOUNT_HEAD_NAME"].ToString();
                    }
                }
                #endregion

                #region GL Voucher
                LedgerController LController = new LedgerController();
                string VoucherNo = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_DISTRIBUTOR_ID, DateTime.Now, mTransaction, mConnection);
                p_Voucher_No = VoucherNo;
                if (mPurchaseMaster.TYPE_ID == 2)// Purchase
                {
                    #region Purchases
                    string Remarks = "Default Purchase Voucher / Document # " + p_ORDER_NUMBER;
                    string Remarks2 = "Default Purchase Voucher";
                    string DocumentType = "Purchase";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 3)  // Purchase Return
                {
                    #region Purchase Return
                    string Remarks = "Default Purchase Return Voucher / Document # " + p_ORDER_NUMBER;
                    string Remarks2 = "Default Purchase Return Voucher";
                    string DocumentType = "Purchase Return";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 4)  // Transfer IN
                {
                    #region Transfer In
                    string Remarks = "Default Transfer In Voucher";
                    string DocumentType = "Transfer IN";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock in Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.TransferInAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 5)  // Transfer OUT
                {
                    #region Transfer Out
                    string Remarks = "Default Transfer Out Voucher";
                    string DocumentType = "Transfer Out";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.TransferOutAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock in Hand " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 6)  // Damage
                {
                    #region Damage
                    string Remarks = "Default Stock Damage Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Damage Voucher";
                    string DocumentType = "Stock Damage";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 890, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 7) // Opening Stock
                {
                    #region Opening Stock
                    string Remarks = "Default Opening Stock Voucher";
                    string DocumentType = "Opening Stock";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock In Hand " + Remarks, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.OpeningStock), 0, mPurchaseMaster.TOTAL_AMOUNT, "Opening at 31/8/19 " + Remarks, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 10) // Damage claimable
                {
                    #region Damage Claimable
                    string Remarks = "Default Stock Damage(Claimable) Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Damage(Claimable) Voucher";
                    string DocumentType = "Stock Damage(Claimable)";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, mPurchaseMaster.PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo, 890, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 8)  // Short
                {
                    #region Short
                    string Remarks = "Default Stock Short Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Short Voucher";
                    string DocumentType = "Stock Short";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), 0, mPurchaseMaster.TOTAL_AMOUNT, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, mPurchaseMaster.TOTAL_AMOUNT, 0, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                else if (mPurchaseMaster.TYPE_ID == 9)  // Excess
                {
                    #region Excess
                    string Remarks = "Default Stock Excess Voucher / Document # " + mPurchaseMaster.PURCHASE_MASTER_ID;
                    string Remarks2 = "Default Stock Excess Voucher";
                    string DocumentType = "Stock Excess";
                    if (LController.PostingGLMaster(mPurchaseMaster.DISTRIBUTOR_ID, 0, VoucherNo, Constants.Journal_Voucher, mPurchaseMaster.DOCUMENT_DATE, mPurchaseMaster.TYPE_ID, Convert.ToString(mPurchaseMaster.PURCHASE_MASTER_ID), Remarks, p_UserId, DocumentType, mPurchaseMaster.PURCHASE_MASTER_ID, mPurchaseMaster.TYPE_ID, mTransaction, mConnection))
                    {
                        if (CodeFileType == 1)
                        {
                            // For Atlas Equipment
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, long.Parse(Configuration.PurchaseAccount), mPurchaseMaster.TOTAL_AMOUNT, 0, "Stock In Hand " + Remarks2, mTransaction, mConnection);
                            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo, p_PrincipalAccount_ID, 0, mPurchaseMaster.TOTAL_AMOUNT, p_PrincipalAccount_Name + " " + Remarks2, mTransaction, mConnection);
                        }
                    }
                    #endregion
                }
                #endregion


                mTransaction.Commit();
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                mTransaction.Rollback();
                p_Voucher_No = "";
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }

        }
        #endregion
    }
}
