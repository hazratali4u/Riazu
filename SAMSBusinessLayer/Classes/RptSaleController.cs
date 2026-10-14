using System;
using System.Data;
using SAMSCommon.Classes;
using SAMSDataAccessLayer.Classes;
using SAMSDatabaseLayer.Classes;
using System.Collections.Generic;
using SAMSDatabaseLayer.InputClasses;
using SAMSDatabaseLayer.AtlasClasses;
using SAMSDatabaseLayer.ReportClasses;

namespace SAMSBusinessLayer.Classes
{
    /// <summary>
    /// Class For Fetching Data Of Sales Reports
    /// </summary>
    public class RptSaleController
    {
        #region Constructor

        /// <summary>
        /// Constructor for RptSaleController
        /// </summary>
        public RptSaleController()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#endregion

        #region Public Methods

        #region Print Sale Document Report

        /// <summary>
        /// Gets Data For Print Sale Document Report(Orders, Invoices, Sale Returns And Delivery Chalans)
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Areaid">Route</param>
        /// <param name="p_Principal_Id">Principal</param>
        /// <param name="FromDocNo">FromDate</param>
        /// <param name="ToDocNo">ToDate</param>
        /// <param name="DocumentTypeId">Type</param>
        /// <param name="p_DOCUMENT_ID">Document</param>
        /// <param name="p_IS_REGISTERED">IsRegistered</param>
        /// <param name="p_CUSTOMER_ID">Customer</param>
        /// <param name="p_Route_ID">Route</param>
        /// <returns>DataSet</returns>
        public DataSet SelectDocumentforPrint(string p_Distributor_ID, int p_Areaid, int p_Principal_Id, DateTime FromDocNo, DateTime ToDocNo, int DocumentTypeId, long p_DOCUMENT_ID, int p_IS_REGISTERED, int p_CUSTOMER_ID, int p_Route_ID, int p_PRINTTYPE)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspDocumentPrinting ObjPrint = new UspDocumentPrinting();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBTOR_ID = p_Distributor_ID;
                ObjPrint.AREA_ID = p_Areaid;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = FromDocNo;
                ObjPrint.TO_DATE = ToDocNo;
                ObjPrint.TYPE_ID = DocumentTypeId;
                ObjPrint.DOCUMENT_ID = p_DOCUMENT_ID;
                ObjPrint.IS_REGISTERED = p_IS_REGISTERED;
                ObjPrint.CUSTOMER_ID = p_CUSTOMER_ID;
                ObjPrint.ROUTE_ID = p_Route_ID;
                ObjPrint.PRINTTYPE = p_PRINTTYPE;
                
                DataTable dt = ObjPrint.ExecuteTable();

                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["SALE_DOCUMENTPRINT"].ImportRow(dr);
                }

                uspPrintSALE_ORDER_PROMOTION Promotion = new uspPrintSALE_ORDER_PROMOTION();

                Promotion.Connection = mConnection;
                Promotion.DISTRIBTOR_ID = p_Distributor_ID;
                Promotion.AREA_ID = p_Areaid;
                Promotion.PRINCIPAL_ID = p_Principal_Id;
                Promotion.FROM_DATE = FromDocNo;
                Promotion.TO_DATE = ToDocNo;
                Promotion.TYPE_ID = DocumentTypeId;
                DataTable dtPro = Promotion.ExecuteTable();

                foreach (DataRow dr in dtPro.Rows)
                {
                    ds.Tables["SALE_PROMOTIONPRINT"].ImportRow(dr);
                }
                return ds;
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

        public DataSet SelectDocumentforPrintForInvoiceNo(string p_Distributor_ID, int p_Areaid, int p_Principal_Id, DateTime FromDocNo, DateTime ToDocNo, int DocumentTypeId, long p_DOCUMENT_ID, int p_IS_REGISTERED, int p_CUSTOMER_ID, int p_Route_ID, int p_PRINTTYPE)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspDocumentPrintingForInvoiceNo ObjPrint = new UspDocumentPrintingForInvoiceNo();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBTOR_ID = p_Distributor_ID;
                ObjPrint.AREA_ID = p_Areaid;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = FromDocNo;
                ObjPrint.TO_DATE = ToDocNo;
                ObjPrint.TYPE_ID = DocumentTypeId;
                ObjPrint.DOCUMENT_ID = p_DOCUMENT_ID;
                ObjPrint.IS_REGISTERED = p_IS_REGISTERED;
                ObjPrint.CUSTOMER_ID = p_CUSTOMER_ID;
                ObjPrint.ROUTE_ID = p_Route_ID;
                ObjPrint.PRINTTYPE = p_PRINTTYPE;

                DataTable dt = ObjPrint.ExecuteTable();

                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["SALE_DOCUMENTPRINT"].ImportRow(dr);
                }

                uspPrintSALE_ORDER_PROMOTION Promotion = new uspPrintSALE_ORDER_PROMOTION();

                Promotion.Connection = mConnection;
                Promotion.DISTRIBTOR_ID = p_Distributor_ID;
                Promotion.AREA_ID = p_Areaid;
                Promotion.PRINCIPAL_ID = p_Principal_Id;
                Promotion.FROM_DATE = FromDocNo;
                Promotion.TO_DATE = ToDocNo;
                Promotion.TYPE_ID = DocumentTypeId;
                DataTable dtPro = Promotion.ExecuteTable();

                foreach (DataRow dr in dtPro.Rows)
                {
                    ds.Tables["SALE_PROMOTIONPRINT"].ImportRow(dr);
                }
                return ds;
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

        public DataSet SelectDocumentforPrintNew(string p_Distributor_ID, int p_Areaid, int p_Principal_Id, DateTime FromDocNo, DateTime ToDocNo, int DocumentTypeId, long p_DOCUMENT_ID, int p_IS_REGISTERED, int p_CUSTOMER_ID, int p_Route_ID, int p_PRINTTYPE)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspDocumentPrintingNew ObjPrint = new UspDocumentPrintingNew();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBTOR_ID = p_Distributor_ID;
                ObjPrint.AREA_ID = p_Areaid;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = FromDocNo;
                ObjPrint.TO_DATE = ToDocNo;
                ObjPrint.TYPE_ID = DocumentTypeId;
                ObjPrint.DOCUMENT_ID = p_DOCUMENT_ID;
                ObjPrint.IS_REGISTERED = p_IS_REGISTERED;
                ObjPrint.CUSTOMER_ID = p_CUSTOMER_ID;
                ObjPrint.ROUTE_ID = p_Route_ID;
                ObjPrint.PRINTTYPE = p_PRINTTYPE;

                DataTable dt = ObjPrint.ExecuteTable();

                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["SALE_DOCUMENTPRINT"].ImportRow(dr);
                }

                uspPrintSALE_ORDER_PROMOTION Promotion = new uspPrintSALE_ORDER_PROMOTION();

                Promotion.Connection = mConnection;
                Promotion.DISTRIBTOR_ID = p_Distributor_ID;
                Promotion.AREA_ID = p_Areaid;
                Promotion.PRINCIPAL_ID = p_Principal_Id;
                Promotion.FROM_DATE = FromDocNo;
                Promotion.TO_DATE = ToDocNo;
                Promotion.TYPE_ID = DocumentTypeId;
                DataTable dtPro = Promotion.ExecuteTable();

                foreach (DataRow dr in dtPro.Rows)
                {
                    ds.Tables["SALE_PROMOTIONPRINT"].ImportRow(dr);
                }
                return ds;
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

        public DataSet SelectDocumentforPrintNewWithInvoiceNo(string p_Distributor_ID, int p_Areaid, int p_Principal_Id, DateTime FromDocNo, DateTime ToDocNo, int DocumentTypeId, long p_DOCUMENT_ID, int p_IS_REGISTERED, int p_CUSTOMER_ID, int p_Route_ID, int p_PRINTTYPE)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspDocumentPrintingNewWithInvoiceNo ObjPrint = new UspDocumentPrintingNewWithInvoiceNo();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBTOR_ID = p_Distributor_ID;
                ObjPrint.AREA_ID = p_Areaid;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = FromDocNo;
                ObjPrint.TO_DATE = ToDocNo;
                ObjPrint.TYPE_ID = DocumentTypeId;
                ObjPrint.DOCUMENT_ID = p_DOCUMENT_ID;
                ObjPrint.IS_REGISTERED = p_IS_REGISTERED;
                ObjPrint.CUSTOMER_ID = p_CUSTOMER_ID;
                ObjPrint.ROUTE_ID = p_Route_ID;
                ObjPrint.PRINTTYPE = p_PRINTTYPE;

                DataTable dt = ObjPrint.ExecuteTable();

                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["SALE_DOCUMENTPRINT"].ImportRow(dr);
                }

                uspPrintSALE_ORDER_PROMOTION Promotion = new uspPrintSALE_ORDER_PROMOTION();

                Promotion.Connection = mConnection;
                Promotion.DISTRIBTOR_ID = p_Distributor_ID;
                Promotion.AREA_ID = p_Areaid;
                Promotion.PRINCIPAL_ID = p_Principal_Id;
                Promotion.FROM_DATE = FromDocNo;
                Promotion.TO_DATE = ToDocNo;
                Promotion.TYPE_ID = DocumentTypeId;
                DataTable dtPro = Promotion.ExecuteTable();

                foreach (DataRow dr in dtPro.Rows)
                {
                    ds.Tables["SALE_PROMOTIONPRINT"].ImportRow(dr);
                }
                return ds;
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

        public DataSet SelectDocumentforPrintRollBack(string p_Distributor_ID, int p_Areaid, int p_Principal_Id, DateTime FromDocNo, DateTime ToDocNo, int DocumentTypeId, long p_DOCUMENT_ID, int p_IS_REGISTERED, int p_CUSTOMER_ID, int p_Route_ID, int p_PRINTTYPE)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspDocumentPrintingRollBack ObjPrint = new UspDocumentPrintingRollBack();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBTOR_ID = p_Distributor_ID;
                ObjPrint.AREA_ID = p_Areaid;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = FromDocNo;
                ObjPrint.TO_DATE = ToDocNo;
                ObjPrint.TYPE_ID = DocumentTypeId;
                ObjPrint.DOCUMENT_ID = p_DOCUMENT_ID;
                ObjPrint.IS_REGISTERED = p_IS_REGISTERED;
                ObjPrint.CUSTOMER_ID = p_CUSTOMER_ID;
                ObjPrint.ROUTE_ID = p_Route_ID;
                ObjPrint.PRINTTYPE = p_PRINTTYPE;

                DataTable dt = ObjPrint.ExecuteTable();

                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["SALE_DOCUMENTPRINT"].ImportRow(dr);
                }

              
                return ds;
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


        public DataSet SelectProfitLossReport(int p_Distributor_ID, DateTime p_FromDate, DateTime p_To_Date, int p_UserId, int p_Type)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspProfitLoass ObjPrint = new UspProfitLoass();
                SAMSBusinessLayer.Reports.dsSalesPurchaseRegister ds = new SAMSBusinessLayer.Reports.dsSalesPurchaseRegister();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;

                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.USER_ID = p_UserId;
                ObjPrint.TYPE = 0;
                DataSet dt = ObjPrint.ExecuteTableset();
                foreach (DataRow dr in dt.Tables[0].Rows)
                {
                    ds.Tables["UspProfitLoass"].ImportRow(dr);
                }
                foreach (DataRow dr in dt.Tables[1].Rows)
                {
                    ds.Tables["UspProfitLoss_expense"].ImportRow(dr);
                }
                foreach (DataRow dr in dt.Tables[2].Rows)
                {
                    ds.Tables["UspProfitLoss_income"].ImportRow(dr);
                }

                return ds;
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

        public DataSet GetDMReturnStatement(int p_Distributor_ID, int p_Principal_Id, int p_DELIVERYMAN_ID, DateTime p_FromDate, DateTime p_To_Date, int p_UserId)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                uspGetDMReturnStatement ObjPrint = new uspGetDMReturnStatement();
                SAMSBusinessLayer.Reports.dsSalesPurchaseRegister ds = new SAMSBusinessLayer.Reports.dsSalesPurchaseRegister();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.USER_ID = p_UserId;
                ObjPrint.DELIVERYMAN_ID = p_DELIVERYMAN_ID;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["uspGetDMReturnStatement"].ImportRow(dr);
                }

                ////////////////CREDIT AMOUNT////////////////////////
                uspGetDMStatementsCREDIT mLedgerSub = new uspGetDMStatementsCREDIT();
                mLedgerSub.Connection = mConnection;
                mLedgerSub.DISTRIBUTOR_ID = p_Distributor_ID;
                mLedgerSub.DELIVERYMAN_ID = p_DELIVERYMAN_ID;
                mLedgerSub.FROM_DATE = p_FromDate;
                mLedgerSub.TO_DATE = p_To_Date;
                mLedgerSub.TYPE_ID = p_UserId;
                DataTable C = mLedgerSub.ExecuteTable();
                foreach (DataRow dr in C.Rows)
                {
                    ds.Tables["uspGetDMStatementsCREDIT"].ImportRow(dr);
                }
                ////////////////CREDIT AMOUNT END ///////////////////

                ////////////////CashRealization////////////////////////
                uspGetDMStatements mLedgerSub2 = new uspGetDMStatements();
                mLedgerSub2.Connection = mConnection;
                mLedgerSub2.DISTRIBUTOR_ID = p_Distributor_ID;
                mLedgerSub2.DELIVERYMAN_ID = p_DELIVERYMAN_ID;
                mLedgerSub2.FROM_DATE = p_FromDate;
                mLedgerSub2.TO_DATE = p_To_Date;
                mLedgerSub2.TYPE_ID = p_UserId;
                DataTable cr = mLedgerSub2.ExecuteTable();
                foreach (DataRow dr in cr.Rows)
                {
                    ds.Tables["uspGetDMStatements"].ImportRow(dr);
                }
                ////////////////CashRealization END ///////////////////

                ////////////////DISCOUNT////////////////////////
                uspGetDMStatementsDISCOUNT mLedgerSub3 = new uspGetDMStatementsDISCOUNT();
                mLedgerSub3.Connection = mConnection;
                mLedgerSub3.DISTRIBUTOR_ID = p_Distributor_ID;
                mLedgerSub3.DELIVERYMAN_ID = p_DELIVERYMAN_ID;
                mLedgerSub3.FROM_DATE = p_FromDate;
                mLedgerSub3.TO_DATE = p_To_Date;
                mLedgerSub3.TYPE_ID = p_UserId;
                DataTable D = mLedgerSub3.ExecuteTable();
                foreach (DataRow dr in D.Rows)
                {
                    ds.Tables["uspGetDMStatementsDISCOUNT"].ImportRow(dr);
                }
                ////////////////DISCOUNT END ///////////////////

                return ds;
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

        public DataSet GetLoadpassDetail(int p_Distributor_ID, int p_Principal_Id, int p_DELIVERYMAN_ID, DateTime p_FromDate, DateTime p_To_Date, int p_UserId, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspGetLoadpassDetail ObjPrint = new UspGetLoadpassDetail();
                SAMSBusinessLayer.Reports.dsSalesPurchaseRegister ds = new SAMSBusinessLayer.Reports.dsSalesPurchaseRegister();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.USER_ID = p_UserId;
                ObjPrint.DELIVERYMAN_ID = p_DELIVERYMAN_ID;
                ObjPrint.TYPE_ID = p_TYPE_ID;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["uspGetDMReturnStatement"].ImportRow(dr);
                }
                return ds;
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

        #region Added By Hazrat Ali

        /// <summary>
        /// Gets Data For Print Sale Document Report(USD Invoices)
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Areaid">Route</param>
        /// <param name="p_Principal_Id">Principal</param>
        /// <param name="FromDocNo">DateFrom</param>
        /// <param name="ToDocNo">DateTo</param>
        /// <param name="DocumentTypeId">Type</param>
        /// <param name="p_DOCUMENT_ID">Document</param>
        /// <param name="p_IS_REGISTERED">IsRegistered</param>
        /// <param name="p_CUSTOMER_ID">Customer</param>
        /// <param name="p_ROUTE_ID">Route</param>
        /// <returns>DataSet</returns>
        public DataSet SelectCSDUSCDocumentforPrint(string p_Distributor_ID, int p_Areaid, int p_Principal_Id, DateTime FromDocNo, DateTime ToDocNo, int DocumentTypeId, long p_DOCUMENT_ID, int p_IS_REGISTERED, int p_CUSTOMER_ID, int p_ROUTE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspPrintCSDUSCInvoice ObjPrint = new UspPrintCSDUSCInvoice();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBTOR_ID = p_Distributor_ID;
                ObjPrint.AREA_ID = p_Areaid;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = FromDocNo;
                ObjPrint.TO_DATE = ToDocNo;
                ObjPrint.TYPE_ID = DocumentTypeId;
                ObjPrint.DOCUMENT_ID = p_DOCUMENT_ID;
                ObjPrint.IS_REGISTERED = p_IS_REGISTERED;
                ObjPrint.CUSTOMER_ID = p_CUSTOMER_ID;
                ObjPrint.ROUTE_ID = p_ROUTE_ID;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["SALE_DOCUMENTPRINT"].ImportRow(dr);
                }

                uspPrintSALE_ORDER_PROMOTION Promotion = new uspPrintSALE_ORDER_PROMOTION();

                Promotion.Connection = mConnection;
                Promotion.DISTRIBTOR_ID = p_Distributor_ID;
                Promotion.AREA_ID = p_Areaid;
                Promotion.PRINCIPAL_ID = p_Principal_Id;
                Promotion.FROM_DATE = FromDocNo;
                Promotion.TO_DATE = ToDocNo;
                Promotion.TYPE_ID = DocumentTypeId;
                DataTable dtPro = Promotion.ExecuteTable();

                foreach (DataRow dr in dtPro.Rows)
                {
                    ds.Tables["SALE_PROMOTIONPRINT"].ImportRow(dr);
                }
                return ds;
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
        
        /// <summary>
        /// Gets Data For Route Wise Customer List Report
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Areaid">Route</param>
        /// <param name="p_ChannelType_Id">Type</param>
        /// <param name="p_TownId">Town</param>
        /// <param name="IsRegister">IsRegistered</param>
        /// <param name="p_Principal">Principal</param>
        /// <returns>DataSet</returns>
        public DataSet SelectPrincipalWiseCustomer(int p_Distributor_ID, int p_Areaid, int p_ChannelType_Id, int p_TownId, bool IsRegister, int p_Principal)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspSelectPrincipalWiseCustomer ObjPrint = new UspSelectPrincipalWiseCustomer();
                SAMSBusinessLayer.Reports.dsSalesPurchaseRegister ds = new SAMSBusinessLayer.Reports.dsSalesPurchaseRegister();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.AREA_ID = p_Areaid;
                ObjPrint.TOWN_ID = p_TownId;
                ObjPrint.CHANNEL_TYPE_ID = p_ChannelType_Id;
                ObjPrint.VOLUME_CLASS_ID = p_Principal;
                if (IsRegister)
                { ObjPrint.ROUTE_ID = 1; }
                else
                { ObjPrint.ROUTE_ID = 0; }
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["ViewPrincipalWise_Customer"].ImportRow(dr);
                }

                return ds;
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

        #region Sale Person DSR Report

        /// <summary>
        /// Gets Data For Sale Person DSR Report(Order Booker,Product Wise)
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Principal_Id">Principal</param>
        /// <param name="p_FromDate">DateFrom</param>
        /// <param name="p_To_Date">DateTo</param>
        /// <param name="User_Id">User</param>
        /// <returns>DataSet</returns>
        public DataSet SelectOrderBookerDSRProDuctWise(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date, int User_Id)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                RptOrderBookerDSRProductWise ObjPrint = new RptOrderBookerDSRProductWise();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.USER_ID = User_Id;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["RptSaleReportProductWise"].ImportRow(dr);
                }

                return ds;
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
        /// Gets Data For Sale Person DSR Report(Saleman, Product Wise)
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Principal_Id">Principal</param>
        /// <param name="p_FromDate">DateFrom</param>
        /// <param name="p_To_Date">DateTo</param>
        /// <param name="User_Id">User</param>
        /// <returns>DataSet</returns>
        public DataSet SelectSalePersonDSRProDuctWise(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date, int User_Id)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                RptSalePersonDSRProductWise ObjPrint = new RptSalePersonDSRProductWise();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.USER_ID = User_Id;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["RptSaleReportProductWise"].ImportRow(dr);
                }

                return ds;
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
        /// Gets Data For Sale Person DSR Report(Order Booker,Value Wise)
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Principal_Id">Principal</param>
        /// <param name="p_FromDate">DateFrom</param>
        /// <param name="p_To_Date">DateTo</param>
        /// <param name="p_UserId">User</param>
        /// <returns>DataSet</returns>
        public DataSet SelectOrderBookerDSR(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date, int p_UserId)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspPrintOrderBookerDSR ObjPrint = new UspPrintOrderBookerDSR();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.USER_ID = p_UserId;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["SALEPERSON_TRANSCTIONDETAIL"].ImportRow(dr);
                }

                return ds;
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
        /// Gets Data For Sale Person DSR Report(Saleman,Value Wise)
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Principal_Id">Principal</param>
        /// <param name="p_FromDate">DateFrom</param>
        /// <param name="p_To_Date">DateTo</param>
        /// <param name="p_UserId">User</param>
        /// <returns>DataSet</returns>
        public DataSet SelectSalePersonDSR(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date, int p_UserId,int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspPrintSaleForceDSR ObjPrint = new UspPrintSaleForceDSR();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.USER_ID = p_UserId;
                ObjPrint.TYPE_ID = p_TYPE_ID;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["SALEPERSON_TRANSCTIONDETAIL"].ImportRow(dr);
                }

                return ds;
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

        /// <summary>
        /// Gets Data For Value Reconciliation Report
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Principal_Id">Principal</param>
        /// <param name="p_Type">Type</param>
        /// <param name="p_FromDate">DateFrom</param>
        /// <param name="p_To_Date">DateTo</param>
        /// <param name="p_UserId">User</param>
        /// <returns>DataSet</returns>
        public DataSet SelectValueReconcilation(int p_Distributor_ID, int p_Principal_Id, string p_Type, DateTime p_FromDate, DateTime p_To_Date, int p_UserId)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                // sp_Name =UspRptValueReconcilationGeneralEntry
                UspRptValueReconcilation ObjPrint = new UspRptValueReconcilation();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.TYPE = p_Type;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.USER_ID = p_UserId;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["RptRegionWise_Reconciliation"].ImportRow(dr);
                }

                return ds;
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
        /// Gets Data For NCS vs Bank Deposit Report
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Principal_Id">Principal</param>
        /// <param name="p_FromDate">DateFrom</param>
        /// <param name="p_To_Date">DateTo</param>
        /// <param name="p_UserId">User</param>
        /// <returns>DataSet</returns>
        public DataSet SelectDailyBankDeposit(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date, int p_UserId)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspDailyBankDeposit ObjPrint = new UspDailyBankDeposit();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.USER_ID = p_UserId;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["RptDailyBankDeposit"].ImportRow(dr);
                }

                return ds;
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

        public DataSet selectCustomerBookingVsExe(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date, int p_SaleMenId,int p_CHANNEL_TYPE_ID,long p_CUSTOMER_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                spSelectCustomerBookingVsExecution ObjPrint = new spSelectCustomerBookingVsExecution();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.ORDERBOOKER_ID = p_SaleMenId;
                ObjPrint.CHANNEL_TYPE_ID = p_CHANNEL_TYPE_ID;
                ObjPrint.CUSTOMER_ID = p_CUSTOMER_ID;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["spSelectCustomerBookingVsExecution"].ImportRow(dr);
                }

                return ds;
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
        /// Gets Data For Sales & Closing Stock Report(Sales, Sales Return, Damage, Opening Stock And Closing Stock)
        /// </summary>
        /// <param name="P_DistributorType">LocationType</param>
        /// <param name="P_PrincipalId">Principal</param>
        /// <param name="P_ZoneId">Zone</param>
        /// <param name="P_DistributorId">Location</param>
        /// <param name="P_frmDate">DateFrom</param>
        /// <param name="P_toDate">DateTo</param>
        /// <param name="P_Category">Cateogry</param>
        /// <param name="P_Type">Type</param>
        /// <param name="P_ReportType">ReportType</param>
        /// <param name="p_UserId">User</param>
        /// <returns>DataSet</returns>
        public DataSet GetRegionSaleDetail(int P_DistributorType, int P_PrincipalId, int P_ZoneId, int P_DistributorId, DateTime P_frmDate, DateTime P_toDate, int P_Category, int P_Type, int P_ReportType, int p_UserId)
        {

            IDbConnection mConnection = null;

            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                uspRegionWiseSales obj_regionwise = new uspRegionWiseSales();
                obj_regionwise.Connection = mConnection;
                obj_regionwise.CATEGORY_ID = P_Category;
                obj_regionwise.distributor_id = P_DistributorId;
                obj_regionwise.Distributor_type = P_DistributorType;
                obj_regionwise.Zone_id = P_ZoneId;
                obj_regionwise.FROM_DATE = P_frmDate;
                obj_regionwise.TO_DATE = P_toDate;
                obj_regionwise.Type = P_Type;
                obj_regionwise.ReportType = P_ReportType;
                obj_regionwise.PrincipalId = P_PrincipalId;
                obj_regionwise.user_id = p_UserId;
                DataTable DT = obj_regionwise.ExecuteTable();

                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["RegionWiseSales"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception excp)
            {
                ExceptionPublisher.PublishException(excp);
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
        /// Gets Data For SKU Wise Branch Sales Report
        /// </summary>
        /// <param name="P_DistributorType">LocationType</param>
        /// <param name="P_PrincipalId">Principal</param>
        /// <param name="P_DistributorId">Location</param>
        /// <param name="P_frmDate">DateFrom</param>
        /// <param name="P_toDate">DateTo</param>
        /// <param name="p_UserId">User</param>
        /// <returns>DataSet</returns>
        public DataSet GetDistributorReconcilation(int P_DistributorType, int P_PrincipalId, int P_DistributorId, DateTime P_frmDate, DateTime P_toDate, int p_UserId)
        {

            IDbConnection mConnection = null;

            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                UspSelectDistributorWiseSale obj_regionwise = new UspSelectDistributorWiseSale();
                obj_regionwise.Connection = mConnection;
                obj_regionwise.LocationType = P_DistributorType;
                obj_regionwise.DistributorId = P_DistributorId;
                obj_regionwise.PrincipalId = P_PrincipalId;
                obj_regionwise.FROM_DATE = P_frmDate;
                obj_regionwise.TO_DATE = P_toDate;
                obj_regionwise.USER_ID = p_UserId;
                DataTable DT = obj_regionwise.ExecuteTable();

                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["DistributorReconcilation"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception excp)
            {
                ExceptionPublisher.PublishException(excp);
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


        public DataSet DailyLoadFormReport(int p_Distributor_ID, int p_Principal_Id, int p_SaleForce_Id, DateTime FromDocDate, DateTime ToDocDate, int p_OrderBooker_ID,int p_CHANNEL_TYPE_ID,long p_CUSTOMER_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                rptDailyLoadPass_Report ObjPrint = new rptDailyLoadPass_Report();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBTOR_ID = p_Distributor_ID;
                ObjPrint.DELIVERYMAN_ID = p_SaleForce_Id;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = FromDocDate;
                ObjPrint.TO_DATE = ToDocDate;
                ObjPrint.ORDERBOOKER_ID = p_OrderBooker_ID;
                ObjPrint.type = 1;// for SKu wise Order Invoice Portion
                ObjPrint.CHANNEL_TYPE_ID = p_CHANNEL_TYPE_ID;
                ObjPrint.CUSTOMER_ID = p_CUSTOMER_ID;
                DataTable dt = ObjPrint.ExecuteTable();

                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["rptDailySaleReport_Sale"].ImportRow(dr);
                }
                return ds;
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


        public DataTable DailyLoadPassReportTitle(int p_Distributor_ID, int p_Principal_Id, int p_SaleForce_Id, DateTime FromDocDate, DateTime ToDocDate, int p_OrderBooker_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                rptDailyLoadPass_ReportTitle ObjPrint = new rptDailyLoadPass_ReportTitle();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBTOR_ID = p_Distributor_ID;
                ObjPrint.DELIVERYMAN_ID = p_SaleForce_Id;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = FromDocDate;
                ObjPrint.TO_DATE = ToDocDate;
                ObjPrint.ORDERBOOKER_ID = p_OrderBooker_ID;
                ObjPrint.type = 6;// for Report Title (OrderBookers and areas)

                DataTable dt = ObjPrint.ExecuteTable();


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
        /// Gets Data For Target Vs Achievement Report
        /// </summary>
        /// <param name="p_Principal_ID">Principal</param>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_dateTime">Month</param>
        /// <param name="p_Option_ID">Option</param>
        /// <param name="p_User_id">User</param>
        /// <param name="p_RegionId">Region</param>
        /// <returns></returns>
        public DataSet SelectVolumeSale(int p_Principal_ID, int p_Distributor_ID, DateTime p_dateTime, int p_Option_ID, int p_DrpTargetType, int p_RegionId, int p_USER_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();

                uspSaleVolmeRpt mSaleVolume = new uspSaleVolmeRpt();
                mSaleVolume.Connection = mConnection;

                mSaleVolume.PRINCIPAL_ID = p_Principal_ID;
                mSaleVolume.DISTRIBUTOR_ID = p_Distributor_ID;
                mSaleVolume.TODAY_DATE = p_dateTime;
                mSaleVolume.OPTION = p_Option_ID;
                mSaleVolume.USER_ID = p_DrpTargetType;
                mSaleVolume.REGION_ID = p_RegionId;
                mSaleVolume.LOGED_IN_USER_ID = p_USER_ID;
                DataTable DT = mSaleVolume.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["VolumeSale"].ImportRow(dr);
                }

                return ds;
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
        public DataSet SelectVolumeSaleSkuWise(int p_Principal_ID, int p_Distributor_ID, DateTime p_dateTime, int p_Option_ID, int p_User_id, int p_RegionId)
        {
            IDbConnection mConnection = null;
            try
            {


                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                SAMSBusinessLayer.Reports.dsSalesPurchaseRegister ds = new SAMSBusinessLayer.Reports.dsSalesPurchaseRegister();

                uspSaleVolmeRptbySku mSaleVolume = new uspSaleVolmeRptbySku();
                mSaleVolume.Connection = mConnection;

                mSaleVolume.PRINCIPAL_ID = p_Principal_ID;
                mSaleVolume.DISTRIBUTOR_ID = p_Distributor_ID;
                mSaleVolume.TODAY_DATE = p_dateTime;
                mSaleVolume.OPTION = p_Option_ID;
                mSaleVolume.USER_ID = p_User_id;
                mSaleVolume.REGION_ID = p_RegionId;              
                DataTable DT = mSaleVolume.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["uspSaleVolmeRptbySku"].ImportRow(dr);
                }

                return ds;
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
        /// Gets Data For Export Data Excel Report
        /// </summary>
        /// <param name="p_DISTRIBUTOR_ID">Location</param>
        /// <param name="p_PRINCIPAL_ID">Principal</param>
        /// <param name="P_FromDate">DateFrom</param>
        /// <param name="p_ToDate">DateTo</param>
        /// <param name="p_UserId">User</param>
        /// <returns>DataSet</returns>
        public DataSet SelectPivateTableExcelFile(int p_DISTRIBUTOR_ID, int p_PRINCIPAL_ID, DateTime P_FromDate, DateTime p_ToDate, int p_UserId)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspfinalsaleDetailView mCustData = new UspfinalsaleDetailView();
                mCustData.Connection = mConnection;

                mCustData.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mCustData.PRINCIPAL_ID = p_PRINCIPAL_ID;
                mCustData.FROM_DATE = P_FromDate;
                mCustData.TO_DATE = p_ToDate;
                mCustData.TOWN_ID = p_UserId;
                DataSet dt = mCustData.ExecuteTable();
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

        #region Order Booker Reports

        /// <summary>
        /// Gets Data For Order Booker Reports(Load Pass Summary)
        /// </summary>
        /// <param name="p_Principal_ID">Principal</param>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Fromdate">DateFrom</param>
        /// <param name="p_ToDate">DateTo</param>
        /// <param name="p_OrderBookerId">OrderBookerType</param>
        /// <param name="p_TypeId">Type</param>
        /// <returns>DataSet</returns>
        public DataSet LoadPass(int p_Principal_ID, int p_Distributor_ID, DateTime p_Fromdate, DateTime p_ToDate, int p_OrderBookerId, int p_TypeId)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();

                RptLoadPass mLoadPass = new RptLoadPass();
                mLoadPass.Connection = mConnection;
                mLoadPass.PRINCIPAL_ID = p_Principal_ID;
                mLoadPass.DISTRIBUTOR_ID = p_Distributor_ID;
                mLoadPass.FROM_DATE = p_Fromdate;
                mLoadPass.TO_DATE = p_ToDate;
                mLoadPass.ORDERBOOKER_ID = p_OrderBookerId;
                mLoadPass.USER_ID = p_TypeId;
                DataTable DT = mLoadPass.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["LoadPass"].ImportRow(dr);
                }
                return ds;
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


        public DataSet LoadPassEntry(int p_Principal_ID, int p_Distributor_ID, DateTime p_Fromdate, int p_OrderBookerId, int p_Route, int p_DeliveryManId)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                SAMSBusinessLayer.Reports.dsSalesPurchaseRegister ds = new SAMSBusinessLayer.Reports.dsSalesPurchaseRegister();
               
                RptLoadPassEntry mLoadPassEntry = new RptLoadPassEntry();
                mLoadPassEntry.Connection = mConnection;
                mLoadPassEntry.PRINCIPAL_ID = p_Principal_ID;
                mLoadPassEntry.DISTRIBUTOR_ID = p_Distributor_ID;
                mLoadPassEntry.FROM_DATE = p_Fromdate;
              //  mLoadPass.TO_DATE = p_ToDate;
                mLoadPassEntry.ORDERBOOKER_ID = p_OrderBookerId;
                mLoadPassEntry.ROUTE_ID = p_Route;
                mLoadPassEntry.DELIVERY_MAN_ID = p_DeliveryManId;
                DataTable DT = mLoadPassEntry.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["RptLoadPassEntry"].ImportRow(dr);
                }
                return ds;
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
        public DataSet LoadPassEntry2(int p_Principal_ID, int p_Distributor_ID, DateTime p_Fromdate, int p_OrderBookerId, int p_Route, int p_DeliveryManId)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                SAMSBusinessLayer.Reports.dsSalesPurchaseRegister ds = new SAMSBusinessLayer.Reports.dsSalesPurchaseRegister();
                // ORIGIONAL IS RptLoadPassEntry WHICH IS FROM SALE_ORDER_MASTER/DETAIL
                //RptLoadPassEntry3 inside RptLoadPassEntry2
                RptLoadPassEntry2 mLoadPassEntry = new RptLoadPassEntry2();
                mLoadPassEntry.Connection = mConnection;
                mLoadPassEntry.PRINCIPAL_ID = p_Principal_ID;
                mLoadPassEntry.DISTRIBUTOR_ID = p_Distributor_ID;
                mLoadPassEntry.FROM_DATE = p_Fromdate;
                //  mLoadPass.TO_DATE = p_ToDate;
                mLoadPassEntry.ORDERBOOKER_ID = p_OrderBookerId;
                mLoadPassEntry.ROUTE_ID = p_Route;
                mLoadPassEntry.DELIVERY_MAN_ID = p_DeliveryManId;
                DataTable DT = mLoadPassEntry.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["RptLoadPassEntry"].ImportRow(dr);
                }
                return ds;
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
        /// Gets Data For Order Booker Reports(Order Booker Sheet)
        /// </summary>
        /// <param name="p_Principal_ID">Principal</param>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Fromdate">DateFrom</param>
        /// <param name="p_ToDate">DateTo</param>
        /// <param name="p_OrderBookerId">OrderBooker</param>
        /// <param name="p_User_Id">User</param>
        /// <returns>DataSet</returns>
        public DataSet OrderBookerSheet(int p_Principal_ID, int p_Distributor_ID, DateTime p_Fromdate, DateTime p_ToDate, int p_OrderBookerId, int p_User_Id)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();

                UspOrderBookerSheet mLoadPass = new UspOrderBookerSheet();
                mLoadPass.Connection = mConnection;
                mLoadPass.PRINCIPAL_ID = p_Principal_ID;
                mLoadPass.DISTRIBUTOR_ID = p_Distributor_ID;
                mLoadPass.FROM_DATE = p_Fromdate;
                mLoadPass.TO_DATE = p_ToDate;
                mLoadPass.ORDERBOOKER_ID = p_OrderBookerId;
                mLoadPass.USER_ID = p_User_Id;
                DataTable DT = mLoadPass.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["RptOrderBookerSheet"].ImportRow(dr);
                }
                return ds;
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

        /// <summary>
        /// Gets Data For SKU Price List Report
        /// </summary>
        /// <param name="p_Principal_ID">Principal</param>
        /// <param name="p_Catagory_ID">Category</param>
        /// <param name="p_DistributorId">Location</param>
        /// <returns>DataSet</returns>
        public DataSet PriceList(int p_Principal_ID, int p_Catagory_ID, int p_DistributorId)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();

                rptPriceList mPriceList = new rptPriceList();

                mPriceList.Connection = mConnection;
                mPriceList.PRINCIPAL_ID = p_Principal_ID;
                mPriceList.CATEGORY_ID = p_Catagory_ID;
                mPriceList.DISTRIBUTOR_ID = p_DistributorId;


                DataTable DT = mPriceList.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["PriceList"].ImportRow(dr);
                }
                return ds;
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
        /// Gets Data For Promotion Report(Active, InActive)
        /// </summary>
        /// <param name="p_Principal_ID">Principal</param>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_StartDate">DateStart</param>
        /// <param name="p_EndDate">DateEnd</param>
        /// <param name="p_PromotionType">Type</param>
        /// <returns>DataSet</returns>
        public DataSet PromotionDetail(int p_Principal_ID, int p_Distributor_ID, DateTime p_StartDate, DateTime p_EndDate, int p_PromotionType)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();

                RptPromotionReport mPromotionReport = new RptPromotionReport();

                mPromotionReport.Connection = mConnection;
                mPromotionReport.Principal_ID = p_Principal_ID;
                mPromotionReport.Distributor_ID = p_Distributor_ID;
                mPromotionReport.START_DATE = p_StartDate;
                mPromotionReport.END_DATE = p_EndDate;
                mPromotionReport.ISACTIVE = Convert.ToBoolean(p_PromotionType);
                DataTable DT = mPromotionReport.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["PromotionReport"].ImportRow(dr);
                }
                return ds;
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
        /// Gets Data For Daily Sale Update Report
        /// </summary>
        /// <param name="p_Principal_ID">Principal</param>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_dateTime">Date</param>
        /// <param name="p_Region">Region</param>
        /// <param name="p_Zone">Zone</param>
        /// <param name="p_Territory_Id">Territory</param>
        /// <param name="p_Type">Type</param>
        /// <returns>DataSet</returns>
        public DataSet SelectDailyUpdateSales(int p_Principal_ID, int p_Distributor_ID, DateTime p_dateTime, int p_Region, int p_Zone, int p_Territory_Id, int p_Type)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                UspDailySaleUpdate mSaleVolume = new UspDailySaleUpdate();
                mSaleVolume.Connection = mConnection;
                mSaleVolume.PRINCIPAL_ID = p_Principal_ID;
                mSaleVolume.DISTRIBUTOR_ID = p_Distributor_ID;
                mSaleVolume.TARGET_DATE = p_dateTime;
                mSaleVolume.Region_Id = p_Region;
                mSaleVolume.Zone_Id = p_Zone;
                mSaleVolume.Territory_Id = p_Territory_Id;
                mSaleVolume.TYPE_ID = p_Type;
                DataTable DT = mSaleVolume.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["RptDailySalesUpdated"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if ((mConnection != null) && (mConnection.State == ConnectionState.Open))
                {
                    mConnection.Close();
                }
            }
        }

        /// <summary>
        /// Gets Data For Business Analysis Report
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_FromDate">DateFrom</param>
        /// <param name="p_ToDate">DateTo</param>
        /// <param name="p_UserId">User</param>
        /// <param name="p_PrincipalId">Principal</param>
        /// <returns>DataSet</returns>
        public DataSet SelectBusinessAnalysis(int p_Distributor_ID, DateTime p_FromDate, DateTime p_ToDate, int p_UserId, int p_PrincipalId)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspBusinessAnalysisReport ObjPrint = new UspBusinessAnalysisReport();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_ToDate;
                ObjPrint.UserId = p_UserId;
                ObjPrint.PRINCIPAL_ID = p_PrincipalId;
                DataTable dt = ObjPrint.ExecuteTable();

                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["RptBusiness_Anaysis Report"].ImportRow(dr);
                }

                return ds;
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
        /// Gets Data For Trade Channel Sale Report (SKU Wise)
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Principal_Id">Principal</param>
        /// <param name="p_FromDate">DateFrom</param>
        /// <param name="p_To_Date">DateTo</param>
        /// <param name="User_Id">User</param>
        /// <param name="p_AreaId">Route</param>
        /// <param name="p_SalesForce_Id">Deliverman</param>
        /// <returns>DataSet</returns>
        public DataSet SelectTradeChannelSale(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date, int User_Id, int p_AreaId, int p_SalesForce_Id)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspTradeChannelSale ObjPrint = new UspTradeChannelSale();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.UserId = User_Id;
                ObjPrint.AreaId = p_AreaId;
                ObjPrint.DeliveryManId = p_SalesForce_Id;
                ObjPrint.OrderBookerId = Constants.IntNullValue;

                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["RptTradeChannelSaleDetail"].ImportRow(dr);
                }

                return ds;
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
        /// Gets Data For Trade Channel Sale Report (Branch Wise)
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Principal_Id">Principal</param>
        /// <param name="p_FromDate">DateFrom</param>
        /// <param name="p_To_Date">DateTo</param>
        /// <param name="User_Id">User</param>
        /// <returns>DataSet</returns>
        public DataSet SelectTradeChannelSale(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date, int User_Id)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspTradeChannelSaleBRANCH ObjPrint = new UspTradeChannelSaleBRANCH();
                SAMSBusinessLayer.Reports.DsReport2 ds = new SAMSBusinessLayer.Reports.DsReport2();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.UserId = User_Id;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["TradeChannelSale"].ImportRow(dr);
                }

                return ds;
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
        /// Gets Data For Gross Profit Report
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Principal_Id">Principal</param>
        /// <param name="p_FromDate">DateFrom</param>
        /// <param name="p_To_Date">DateTo</param>
        /// <returns>DataSet</returns>
        public DataSet SelectRptGrossProfit(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date,int p_USER_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspGrossProfitReport ObjPrint = new UspGrossProfitReport();
                SAMSBusinessLayer.Reports.DsReport2 ds = new SAMSBusinessLayer.Reports.DsReport2();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.USER_ID = p_USER_ID;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["GrossPrifitReport"].ImportRow(dr);
                }

                return ds;
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
        public DataSet SelectRptGrossProfitNew(int p_ACCOUNT_CATEGORY_ID, int p_DISTRIBUTOR_ID, DateTime p_FromDate, DateTime p_To_Date, int p_USER_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspGrossProfitReportNew ObjPrint = new UspGrossProfitReportNew();
                SAMSBusinessLayer.Reports.DsReport2 ds = new SAMSBusinessLayer.Reports.DsReport2();
                ObjPrint.Connection = mConnection;
                ObjPrint.ACCOUNT_CATEGORY_ID = p_ACCOUNT_CATEGORY_ID;
                ObjPrint.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.USER_ID = p_USER_ID;
                DataSet ds2 = ObjPrint.ExecuteTable();

                foreach (DataRow dr in ds2.Tables[0].Rows)
                {
                    ds.Tables["IncomeStatement"].ImportRow(dr);
                }

                foreach (DataRow dr in ds2.Tables[1].Rows)
                {
                    ds.Tables["IncomeStatement1"].ImportRow(dr);
                }

                foreach (DataRow dr in ds2.Tables[2].Rows)
                {
                    ds.Tables["IncomeStatement11"].ImportRow(dr);
                }


                return ds;
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
        /// Gets Data For Monthly Sale Report (Trade Price And Purchase Price)
        /// </summary>
        /// <param name="P_DateType">DateType</param>
        /// <param name="P_PrincipalId">Principal</param>
        /// <param name="P_DistributorId">Location</param>
        /// <param name="P_frmDate">DateFrom</param>
        /// <param name="P_toDate">DateTo</param>
        /// <param name="p_UserId">User</param>
        /// <param name="p_Column">ReprotFor</param>
        /// <param name="p_PriceType">PriceType</param>
        /// <returns>DataSet</returns>
        public DataSet GetDistributorReconcilation(byte P_DateType, int P_PrincipalId, int P_DistributorId, DateTime P_frmDate, DateTime P_toDate, int p_UserId, byte p_Column, byte p_PriceType)
        {

            IDbConnection mConnection = null;

            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                SAMSBusinessLayer.Reports.DsReport2 ds = new SAMSBusinessLayer.Reports.DsReport2();
                DataTable DT;
                if (p_PriceType == 0)
                {
                    UspPrintSaleAnalysisSummary obj_regionwise = new UspPrintSaleAnalysisSummary();
                    obj_regionwise.Connection = mConnection;
                    obj_regionwise.DATETYPE = P_DateType;
                    obj_regionwise.DISTRIBUTOR_ID = P_DistributorId;
                    obj_regionwise.PRINCIPAL_ID = P_PrincipalId;
                    obj_regionwise.FROM_DATE = P_frmDate;
                    obj_regionwise.TO_DATE = P_toDate;
                    obj_regionwise.COLUMN = p_Column;
                    DT = obj_regionwise.ExecuteTable();
                }
                else
                {
                    UspPrintSaleAnalysisSummaryPR obj_regionwise = new UspPrintSaleAnalysisSummaryPR();
                    obj_regionwise.Connection = mConnection;
                    obj_regionwise.DATETYPE = P_DateType;
                    obj_regionwise.DISTRIBUTOR_ID = P_DistributorId;
                    obj_regionwise.PRINCIPAL_ID = P_PrincipalId;
                    obj_regionwise.FROM_DATE = P_frmDate;
                    obj_regionwise.TO_DATE = P_toDate;
                    obj_regionwise.COLUMN = p_Column;
                    DT = obj_regionwise.ExecuteTable();
                }
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["RptMonthSaleValues"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception excp)
            {
                ExceptionPublisher.PublishException(excp);
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
        /// Gets Data For  Principal Wise Reconciliation Report
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Principal_Id">Principal</param>
        /// <param name="p_FromDate">DateFrom</param>
        /// <param name="p_To_Date">DateTo</param>
        /// <param name="p_UserId">User</param>
        /// <returns>DataSet</returns>
        public DataSet SelectPrincipalReconcilation(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date, int p_UserId, string p_TYPE)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspRptValueReconcilation ObjPrint = new UspRptValueReconcilation();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.USER_ID = p_UserId;
                ObjPrint.TYPE = p_TYPE;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["RptRegionWise_Reconciliation"].ImportRow(dr);
                }

                UspCreditAgintReportPrincipalWise obj_regionwise = new UspCreditAgintReportPrincipalWise();
                obj_regionwise.Connection = mConnection;
                obj_regionwise.DISTRIBUTOR_ID = p_Distributor_ID;
                obj_regionwise.DISTRIBUTOR_TYPE_ID = Constants.IntNullValue;
                obj_regionwise.TYPE_ID = 0;
                obj_regionwise.PRINCIPAL_ID = p_Principal_Id;
                obj_regionwise.USER_ID = p_UserId;
                obj_regionwise.DOCUMENT_DATEFROM = p_FromDate;
                obj_regionwise.DOCUMENT_DATE = p_To_Date;
                obj_regionwise.CHANNEL_TYPE_ID = Constants.IntNullValue;
                DataTable DT = obj_regionwise.ExecuteTable();

                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["RptCreditAgingReport"].ImportRow(dr);
                }
                return ds;
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
        /// Gets Data For Credit Tagging Report
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Principal_Id">Principal</param>
        /// <param name="p_FromDate">DateFrom</param>
        /// <param name="p_To_Date">DateTo</param>
        /// <param name="p_CreditTypeId">CreditType</param>
        /// <returns>DataSet</returns>
        public DataSet SelectCreditTagging(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date, int p_CreditTypeId)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspCreditTaggingDetail ObjPrint = new UspCreditTaggingDetail();
                SAMSBusinessLayer.Reports.DsReport2 ds = new SAMSBusinessLayer.Reports.DsReport2();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.CREDIT_TYPE = p_CreditTypeId;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["RptCustomerTagging"].ImportRow(dr);
                }

                return ds;
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
        
        #region Added by Hazrat Ali

        /// <summary>
        /// Gets Data For Date Wise Discount Report (Date Wise, Branch Wise)
        /// </summary>
        /// <param name="p_FromDate">DateFrom</param>
        /// <param name="p_To_Date">DateTo</param>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="P_PrincipalID">Principal</param>
        /// <param name="p_TypeId">Type</param>
        /// <returns>DataSet</returns>
        public DataSet SelectDateWiseDiscount(DateTime p_FromDate, DateTime p_To_Date, int p_Distributor_ID, int P_PrincipalID, int p_TypeId)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();

                uspRptDateWiseDiscount ObjDiscount = new uspRptDateWiseDiscount();
                ObjDiscount.Connection = mConnection;
                ObjDiscount.DateFrom = p_FromDate;
                ObjDiscount.DateTo = p_To_Date;
                ObjDiscount.DistributorID = p_Distributor_ID;
                ObjDiscount.PrincipalID = P_PrincipalID;
                ObjDiscount.TYPE_ID = p_TypeId;

                DataTable dt = ObjDiscount.ExecuteTable();

                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["RptDateWiseDiscount"].ImportRow(dr);
                }
                return ds;
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
        /// Gets Data For User Login History Report
        /// </summary>
        /// <param name="p_FromDate">DateFrom</param>
        /// <param name="p_To_Date">DateTo</param>
        /// <param name="p_User_ID">User</param>
        /// <param name="p_User_Log_ID">UserLog</param>
        /// <returns>DataSet</returns>
        public DataSet GetUserLoginDetail(DateTime p_FromDate, DateTime p_To_Date, int p_User_ID, long p_User_Log_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();

                uspGetUserLoginDetail ObjLogin = new uspGetUserLoginDetail();
                ObjLogin.Connection = mConnection;
                ObjLogin.DateFrom = p_FromDate;
                ObjLogin.DateTo = p_To_Date;
                ObjLogin.USER_ID = p_User_ID;
                ObjLogin.User_Log_ID = p_User_Log_ID;

                DataTable dt = ObjLogin.ExecuteTable();

                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["uspGetUserLoginDetail"].ImportRow(dr);
                }
                return ds;
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
        /// Gets Data For SKU Price History Report
        /// </summary>
        /// <param name="p_SKU_ID">SKU</param>
        /// <param name="p_Principal_ID">Principal</param>
        /// <param name="p_Category_ID">Category</param>
        /// <param name="p_DistributorId">Location</param>
        /// <param name="p_From_Date">DateFrom</param>
        /// <param name="p_ToDate">DateTo</param>
        /// <param name="p_TYPE">Type</param>
        /// <returns>DataSet</returns>
        public DataSet GetSKUPriceHistory(int p_SKU_ID, int p_Principal_ID, int p_Category_ID, int p_DistributorId, DateTime p_From_Date, DateTime p_ToDate, int p_TYPE)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();

                uspGetSKUPriceHistory mSKUPriceHistory = new uspGetSKUPriceHistory();

                mSKUPriceHistory.Connection = mConnection;
                mSKUPriceHistory.SKU_ID = p_SKU_ID;
                mSKUPriceHistory.PRINCIPAL_ID = p_Principal_ID;
                mSKUPriceHistory.DISTRIBUTOR_ID = p_DistributorId;
                mSKUPriceHistory.CATEGORY_ID = p_Category_ID;
                mSKUPriceHistory.DATEFROM = p_From_Date;
                mSKUPriceHistory.DATETO = p_ToDate;
                mSKUPriceHistory.TYPE = p_TYPE;

                DataTable DT = mSKUPriceHistory.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["SKUPriceHistory"].ImportRow(dr);
                }
                return ds;
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
        public DataSet GetSKUPriceHistoryPurchaseWise(int p_SKU_ID, int p_Principal_ID, int p_DistributorId, DateTime p_From_Date, DateTime p_ToDate)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.dsSalesPurchaseRegister ds = new Reports.dsSalesPurchaseRegister();
                uspGetSKUPriceHistoryPurchase mSKUPriceHistory = new uspGetSKUPriceHistoryPurchase();
                mSKUPriceHistory.Connection = mConnection;
                mSKUPriceHistory.SKU_ID = p_SKU_ID;
                mSKUPriceHistory.PRINCIPAL_ID = p_Principal_ID;
                mSKUPriceHistory.DISTRIBUTOR_ID = p_DistributorId;
                mSKUPriceHistory.DATEFROM = p_From_Date;
                mSKUPriceHistory.DATETO = p_ToDate;
                DataTable DT = mSKUPriceHistory.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["uspGetSKUPriceHistoryPurchaseWise"].ImportRow(dr);
                }
                return ds;
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
        /// Gets Data For Branch Position Report
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Principal_Id">Principal</param>
        /// <param name="p_FromDate">DateFrom</param>
        /// <param name="p_To_Date">DateTo</param>
        /// <param name="p_UserId">User</param>
        /// <returns>DataSet</returns>
        public DataSet GetBranchPosition(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date, int p_UserId)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                uspGetBranchPositionData ObjPrint = new uspGetBranchPositionData();
                SAMSBusinessLayer.Reports.DsReport2 ds = new SAMSBusinessLayer.Reports.DsReport2();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.USER_ID = p_UserId;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["uspGetBranchPositionData"].ImportRow(dr);
                }

                uspGetSummarizedBranchPositionData ObjPrintSummary = new uspGetSummarizedBranchPositionData();

                ObjPrintSummary.Connection = mConnection;
                ObjPrintSummary.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrintSummary.PRINCIPAL_ID = p_Principal_Id;
                ObjPrintSummary.FROM_DATE = p_FromDate;
                ObjPrintSummary.TO_DATE = p_To_Date;
                ObjPrintSummary.USER_ID = p_UserId;
                DataTable dtSummary = ObjPrintSummary.ExecuteTable();
                foreach (DataRow dr in dtSummary.Rows)
                {
                    ds.Tables["uspGetSummarizedBranchPositionData"].ImportRow(dr);
                }

                return ds;
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
        /// Gets Data For Daily Business Update Report
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Principal_Id">Principal</param>
        /// <param name="p_FromDate">DateFrom</param>
        /// <param name="p_To_Date">DateTo</param>
        /// <returns>DataSet</returns>
        public DataSet GetDailyBusinessUpdate(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspGetDailyBusinessUpdate ObjPrint = new UspGetDailyBusinessUpdate();
                SAMSBusinessLayer.Reports.DsReport2 dsReport = new SAMSBusinessLayer.Reports.DsReport2();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                DataSet ds = ObjPrint.ExecuteDataSet();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dsReport.Tables["UspGetDailyBusinessUpdate"].ImportRow(dr);
                }

                return ds;
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
        /// Gets Data For Sale Person DSR Report(Saleman,Value Wise)
        /// </summary>
        /// <param name="p_Distributor_ID">Location</param>
        /// <param name="p_Principal_Id">Principal</param>
        /// <param name="p_FromDate">DateFrom</param>
        /// <param name="p_To_Date">DateTo</param>
        /// <param name="p_UserId">User</param>
        /// <returns>DataSet</returns>
        public DataSet GetSalePersonDSRDetail(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date, int p_UserId)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                uspGetSalesPersonDSRDetail ObjPrint = new uspGetSalesPersonDSRDetail();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.USER_ID = p_UserId;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["uspGetSalesPersonDSRDetail"].ImportRow(dr);
                }
                return ds;
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


        public DataSet GetShortExcessReport(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date, int p_UserId)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                uspShortExcess ObjPrint = new uspShortExcess();
                SAMSBusinessLayer.Reports.dsSalesPurchaseRegister ds = new SAMSBusinessLayer.Reports.dsSalesPurchaseRegister();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.USER_ID = p_UserId;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    if(decimal.Parse(dr["TempData"].ToString())>0)
                    {
                        dr["Short"] = dr["TempData"];
                        dr["Excess"] = "0";
                    }
                    else
                    {
                        dr["Excess"] = (decimal.Parse(dr["TempData"].ToString()) * -1).ToString();
                        dr["Short"] = "0";
                    }

                    // ds.Tables["uspGetSalesPersonDSRDetail"].ImportRow(dr);
                    ds.Tables["uspShortExcess"].ImportRow(dr);
                }
                return ds;
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


        public DataSet GetSchemeDiscountReport(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date, int p_UserId,int p_RouteID, string p_OrderBooerList)
        {

            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                var objPrint = new UspSchemeDiscountReport();
                var ds = new Reports.dsSalesPurchaseRegister();
                objPrint.Connection = mConnection;
                objPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                objPrint.PRINCIPAL_ID = p_Principal_Id;
                objPrint.FromDate = p_FromDate;
                objPrint.ToDate  = p_To_Date;
                objPrint.USER_ID = p_UserId;
                objPrint.ROUTE_ID = p_RouteID;
                objPrint.ORDERBOOKER_LIST = p_OrderBooerList;
                DataTable dt = objPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                   ds.Tables["UspSchemeDiscountReport"].ImportRow(dr);
                }
                return ds;
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
        public DataSet LoadDispatchNotes(int p_Distributor_ID, int p_Principal_ID, DateTime p_Fromdate, DateTime p_Todate, int p_OrderBookerId, int p_Type)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                SAMSBusinessLayer.Reports.DsReport2 ds = new SAMSBusinessLayer.Reports.DsReport2();
                RptDispatchNotes mLoadPass = new RptDispatchNotes();
                mLoadPass.Connection = mConnection;
                mLoadPass.PRINCIPAL_ID = p_Principal_ID;
                mLoadPass.DATE = p_Fromdate;
                mLoadPass.TO_DATE = p_Todate;
                mLoadPass.SALEMAN_ID = p_OrderBookerId;
                mLoadPass.TYPE = p_Type;
                mLoadPass.DISTRIBUTOR_ID = p_Distributor_ID;

                DataTable DT = mLoadPass.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["RptLoadPass2"].ImportRow(dr);
                }
                return ds;
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
        public DataSet GetRetailSaleDataCollection(int p_Data_Type, int p_DISTRIBUTOR_ID, int p_AREA_ID, int p_CUSTOMER_ID, int p_CUSTOMER_TYPE, int p_PRINCIPAL_ID, int p_ORDERBOOKER_ID, int p_DELIVERYMAN_ID, string p_CHANNEL_TYPE_ID, string p_SKUS, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_REPORT_TYPE)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                uspGetRetailSaleDataCollection ObjPrint = new uspGetRetailSaleDataCollection();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                ObjPrint.AREA_ID = p_AREA_ID;
                ObjPrint.CUSTOMER_ID = p_CUSTOMER_ID;
                ObjPrint.CUSTOMER_TYPE = p_CUSTOMER_TYPE;
                ObjPrint.PRINCIPAL_ID = p_PRINCIPAL_ID;
                ObjPrint.ORDERBOOKER_ID = p_ORDERBOOKER_ID;
                ObjPrint.DELIVERYMAN_ID = p_DELIVERYMAN_ID;
                ObjPrint.CHANNEL_TYPE_ID = p_CHANNEL_TYPE_ID;
                ObjPrint.SKUS = p_SKUS;
                ObjPrint.FROM_DATE = p_FROM_DATE;
                ObjPrint.TO_DATE = p_TO_DATE;
                ObjPrint.REPORT_TYPE = p_REPORT_TYPE;
                ObjPrint.DATA_TYPE = p_Data_Type;
                return ObjPrint.ExecuteDataSet();

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
        public DataSet SelectDocumentforPrint_AE(int p_Distributor_ID, int p_Areaid, int p_Principal_Id, DateTime FromDocNo, DateTime ToDocNo, int DocumentTypeId, long p_DOCUMENT_ID, int p_IS_REGISTERED, int p_CUSTOMER_ID, int p_Route_ID, int p_PRINTTYPE)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspDocumentPrinting_AE ObjPrint = new UspDocumentPrinting_AE();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBTOR_ID = p_Distributor_ID;
                ObjPrint.AREA_ID = p_Areaid;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = FromDocNo;
                ObjPrint.TO_DATE = ToDocNo;
                ObjPrint.TYPE_ID = DocumentTypeId;
                ObjPrint.DOCUMENT_ID = p_DOCUMENT_ID;
                ObjPrint.IS_REGISTERED = p_IS_REGISTERED;
                ObjPrint.CUSTOMER_ID = p_CUSTOMER_ID;
                ObjPrint.ROUTE_ID = p_Route_ID;
                ObjPrint.PRINTTYPE = p_PRINTTYPE;

                DataTable dt = ObjPrint.ExecuteTable();

                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["SALE_DOCUMENTPRINT"].ImportRow(dr);
                }
                if (DocumentTypeId != 5)
                {
                    uspPrintSALE_ORDER_PROMOTION_AE Promotion = new uspPrintSALE_ORDER_PROMOTION_AE();
                    Promotion.Connection = mConnection;
                    Promotion.DISTRIBTOR_ID = p_Distributor_ID;
                    Promotion.AREA_ID = p_Areaid;
                    Promotion.PRINCIPAL_ID = p_Principal_Id;
                    Promotion.FROM_DATE = FromDocNo;
                    Promotion.TO_DATE = ToDocNo;
                    Promotion.TYPE_ID = DocumentTypeId;
                    DataTable dtPro = Promotion.ExecuteTable();
                    foreach (DataRow dr in dtPro.Rows)
                    {
                        ds.Tables["SALE_PROMOTIONPRINT"].ImportRow(dr);
                    }
                }
                return ds;
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
        public DataSet SelectCSDUSCDocumentforPrint_AE(int p_Distributor_ID, int p_Areaid, int p_Principal_Id, DateTime FromDocNo, DateTime ToDocNo, int DocumentTypeId, long p_DOCUMENT_ID, int p_IS_REGISTERED, int p_CUSTOMER_ID, int p_ROUTE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspPrintCSDUSCInvoice_AE ObjPrint = new UspPrintCSDUSCInvoice_AE();
                SAMSBusinessLayer.Reports.DsReport ds = new SAMSBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBTOR_ID = p_Distributor_ID;
                ObjPrint.AREA_ID = p_Areaid;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = FromDocNo;
                ObjPrint.TO_DATE = ToDocNo;
                ObjPrint.TYPE_ID = DocumentTypeId;
                ObjPrint.DOCUMENT_ID = p_DOCUMENT_ID;
                ObjPrint.IS_REGISTERED = p_IS_REGISTERED;
                ObjPrint.CUSTOMER_ID = p_CUSTOMER_ID;
                ObjPrint.ROUTE_ID = p_ROUTE_ID;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["SALE_DOCUMENTPRINT"].ImportRow(dr);
                }
                uspPrintSALE_ORDER_PROMOTION_AE Promotion = new uspPrintSALE_ORDER_PROMOTION_AE();
                Promotion.Connection = mConnection;
                Promotion.DISTRIBTOR_ID = p_Distributor_ID;
                Promotion.AREA_ID = p_Areaid;
                Promotion.PRINCIPAL_ID = p_Principal_Id;
                Promotion.FROM_DATE = FromDocNo;
                Promotion.TO_DATE = ToDocNo;
                Promotion.TYPE_ID = DocumentTypeId;
                DataTable dtPro = Promotion.ExecuteTable();
                foreach (DataRow dr in dtPro.Rows)
                {
                    ds.Tables["SALE_PROMOTIONPRINT"].ImportRow(dr);
                }
                return ds;
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

        public DataSet GetSKUCartonSale(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date, int p_UserId, string p_CategoryIDs, int p_TYPE_ID, int p_CHANNEL_TYPE_ID, int p_CustomerID, int p_SKUTYPE_ID, int p_Town)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                uspGetSKUCtnSales ObjPrint = new uspGetSKUCtnSales();
                SAMSBusinessLayer.Reports.dsSalesPurchaseRegister ds = new SAMSBusinessLayer.Reports.dsSalesPurchaseRegister();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.CUSTOMER_ID = p_CustomerID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.USER_ID = p_UserId;
                ObjPrint.CATEGORY_IDS = p_CategoryIDs;
                ObjPrint.TYPE_ID = p_TYPE_ID;
                ObjPrint.CHANNEL_TYPE_ID = p_CHANNEL_TYPE_ID;
                ObjPrint.SKUTYPE_ID = p_SKUTYPE_ID;
                ObjPrint.AREA_ID = p_Town;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["uspGetSKUCtnSales"].ImportRow(dr);
                }

                return ds;
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

        public DataSet GetSKUWiseNetSale(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date, int p_UserId, string p_CategoryIDs, int p_TYPE_ID, int p_CHANNEL_TYPE_ID, int p_CustomerID, int p_SKUTYPE_ID, int p_Town_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                UspSKUWiseNetSale ObjPrint = new UspSKUWiseNetSale();
                SAMSBusinessLayer.Reports.dsSalesPurchaseRegister ds = new SAMSBusinessLayer.Reports.dsSalesPurchaseRegister();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.CUSTOMER_ID = p_CustomerID;
                ObjPrint.SKUTYPE_ID = p_SKUTYPE_ID;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.USER_ID = p_UserId;
                ObjPrint.CATEGORY_IDS = p_CategoryIDs;
                ObjPrint.TYPE_ID = p_TYPE_ID;
                ObjPrint.CHANNEL_TYPE_ID = p_CHANNEL_TYPE_ID;
                ObjPrint.AREA_ID = p_Town_ID;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["UspSKUWiseNetSale"].ImportRow(dr);
                }

                return ds;
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

        public DataSet GetOrderVsInvoiceDetail(int p_Distributor_ID, string p_CUSTOMER_IDs, DateTime p_FromDate, DateTime p_To_Date)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                uspGetOrderVsInvoice ObjPrint = new uspGetOrderVsInvoice();
                SAMSBusinessLayer.Reports.dsSalesPurchaseRegister ds = new SAMSBusinessLayer.Reports.dsSalesPurchaseRegister();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.CUSTOMER_IDs = p_CUSTOMER_IDs;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["uspGetOrderVsInvoice"].ImportRow(dr);
                }

                return ds;
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
    }
}
