using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;

public partial class Forms_frmCustomerRelizationAE : System.Web.UI.Page
{
    readonly LedgerController LController = new LedgerController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            SAMSCommon.Classes.Configuration.GetAccountHead();
            this.LoadPrincipal();
            this.LoadDistributor();
            this.LoadDeliveryman();
            this.LoadArea();
            this.LoadData();
            this.LoadAccountHead();
            this.LoadGrid();
            this.SelectCreditInvoice();
            btnSave.Attributes.Add("onclick", "return ValidateForm();");
            this.SetTableSorter();
        }
    }

    /// <summary>
    /// Loads Locations To Location Combo
    /// </summary>
    private void LoadDistributor()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDropDownList(this.drpDistributor, dt, 0, 2, true);
    }

    /// <summary>
    /// Loads Routes To Route Combo
    /// </summary>
    private void LoadArea()
    {
        DistributorAreaController mController = new DistributorAreaController();
        DataTable dt = mController.SelectDist_Area(Constants.LongNullValue, Constants.DateNullValue, Constants.DateNullValue, int.Parse(drpDistributor.SelectedValue.ToString()), Constants.IntNullValue, null, null);
        clsWebFormUtil.FillDropDownList(DrpRoute, dt, 0, 6, true);
    }

    /// <summary>
    /// Loads Customers To Customer Combo
    /// </summary>
    private void LoadData()
    {

        GrdCredit.DataSource = null;
        GrdCredit.DataBind();
        if (drpDistributor.Items.Count > 0)
        {
            CustomerDataController mController = new CustomerDataController();

            if (DrpAccountType.SelectedIndex == 1 || DrpAccountType.SelectedIndex == 5)
            {

                DataTable dt = mController.SelectPrincipalCustomer2(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpRoute.SelectedValue.ToString()), Constants.IntNullValue, int.Parse(DrpPrincipal.SelectedValue.ToString()));
                clsWebFormUtil.FillDropDownList(this.DrpCustomer, dt, 0, 4, true);
                DrpRoute.Enabled = true;
            }
            else
            {
                //get Customers On Selected Saleforce 
                if (DrpDeliveryMan.SelectedValue == "")
                {
                    LedgerController LedgerCtl = new LedgerController();
                    DataTable dtCredit = LedgerCtl.SelectCreditPendingInvoice2(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()), Constants.LongNullValue, Constants.IntNullValue, Constants.IntNullValue);
                    clsWebFormUtil.FillDropDownList(this.DrpCustomer, dtCredit, 0, 1, true);
                    DrpRoute.Enabled = false;
                }
                else
                {
                    LedgerController LedgerCtl = new LedgerController();
                    DataTable dtCredit = LedgerCtl.SelectCreditPendingInvoice2(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()), Constants.LongNullValue, Constants.IntNullValue, int.Parse(DrpDeliveryMan.SelectedValue));
                    clsWebFormUtil.FillDropDownList(this.DrpCustomer, dtCredit, 0, 1, true);
                    DrpRoute.Enabled = false;
                }
            }
        }
    }

    /// <summary>
    /// Resets Form Controls
    /// </summary>
    private void ClearAll()
    {
        txtChequeNo.Text = "";
        txtAmount.Text = "";
        txtRemarks.Text = "";
        txtSlipNo.Text = "";
        btnSave.Text = "Save";
        chbIsAdvance.Checked = false;
        chbIsAdvance.Visible = false;
    }

    /// <summary>
    /// Loads Bank Transactions To Grid
    /// </summary>
    private void LoadGrid()
    {
        if (DrpAccountType.SelectedIndex != 7)
        {
            string DrpAccountTypeSelectedValue = "";
            if (DrpAccountType.SelectedValue == "222")
            {
                DrpAccountTypeSelectedValue = "22";
            }
            else
            {
                DrpAccountTypeSelectedValue = DrpAccountType.SelectedValue;
            }
            if (drpDistributor.Items.Count > 0)
            {
                
                DataTable dt = LController.SelectBankCashTransction(int.Parse(drpDistributor.SelectedValue.ToString()), Constants.IntNullValue, Convert.ToInt32(DrpAccountTypeSelectedValue),
                    DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
                GrdOrder.DataSource = dt;
                GrdOrder.DataBind();


            }
        }
    }

    /// <summary>
    /// Loads Account Heads To Account Combo
    /// </summary>
    private void LoadAccountHead()
    {
        if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 1 || DrpAccountType.SelectedIndex == 5 || DrpAccountType.SelectedIndex == 7)
        {
            AccountHeadController mAccountController = new AccountHeadController();
            DataTable dt = mAccountController.SelectAccountHeadAssignment(Constants.AC_AccountHeadId, Constants.IntNullValue, Constants.IntNullValue, 1, int.Parse(drpDistributor.SelectedValue.ToString()), 0, 2);
            clsWebFormUtil.FillDropDownList(DrpAccountDetail, dt, 0, 1, true);
            //AccountHeadController mAccountController = new AccountHeadController();
            //DataTable dt = mAccountController.SelectAccountHeadAssigned(Constants.AC_AccountHeadId, long.Parse(Configuration.CashDefaultType), Convert.ToInt32(drpDistributor.SelectedValue));
            //clsWebFormUtil.FillDropDownList(DrpAccountDetail, dt, 0, 4, true);
            ////DataTable dt = mAccountController.SelectAccountHeadAssignment(Constants.AC_AccountHeadId, Constants.IntNullValue, Constants.IntNullValue, 1, int.Parse(drpDistributor.SelectedValue.ToString()), 0, 2);
            ////clsWebFormUtil.FillDropDownList(DrpAccountDetail, dt, 0, 1, true);
        }
        else if (DrpAccountType.SelectedIndex == 3)
        {
            DrpAccountDetail.Items.Clear();
            DrpAccountDetail.Items.Add(new ListItem("Tax Deducted By Parties", "127"));
        }
        else if (DrpAccountType.SelectedIndex == 4)
        {
            DrpAccountDetail.Items.Clear();
            DrpAccountDetail.Items.Add(new ListItem("Credit Transfer Out", "361"));
        }
        else if (DrpAccountType.SelectedIndex == 2 || DrpAccountType.SelectedIndex == 9 || DrpAccountType.SelectedIndex == 6)
        {
            AccountHeadController mAccountController = new AccountHeadController();
            DataTable dt = mAccountController.SelectAccountHeadAssigned(Constants.AC_AccountHeadId, long.Parse(Configuration.BankDefaultType), Convert.ToInt32(drpDistributor.SelectedValue));
            clsWebFormUtil.FillDropDownList(DrpAccountDetail, dt, 0, 4, true);
        }
        else if (DrpAccountType.SelectedValue == "727")
        {
            DrpAccountDetail.Items.Clear();
            AccountHeadController mAccountController = new AccountHeadController();
            DataTable dt = mAccountController.SelectAccountHeadAssigned(Constants.AC_AccountHeadId, 727, Convert.ToInt32(drpDistributor.SelectedValue));
            clsWebFormUtil.FillDropDownList(DrpAccountDetail, dt, 0, 4, true);
        }
        else
        {
            DrpAccountDetail.Items.Clear();
        }
    }

    /// <summary>
    /// Loads Principals To Principal Combo
    /// </summary>
    private void LoadPrincipal()
    {
        SKUPriceDetailController PController = new SKUPriceDetailController();
        DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, m_dt, 0, 1, true);
    }

    /// <summary>
    /// Saves Cash Realization
    /// </summary>
    private void CashRealization()
    {
        LedgerController LController = new LedgerController();
        string MaxDocumentID = LController.SelectLedgerMaxDocumentId(Constants.Cash_Voucher, int.Parse(drpDistributor.SelectedValue.ToString()));
        Session.Add("VoucherNo", MaxDocumentID);
        decimal OfferAmount = decimal.Parse(txtAmount.Text);
        decimal realizeAmount = 0;
        foreach (GridViewRow dr in GrdCredit.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                #region
                DataTable dtLedger = CreateTable();

                //Credit from Account Receivable (Party Wise)   CD
                DataRow drLedger = dtLedger.NewRow();
                drLedger["VOUCHER_TYPE_ID"] = Constants.Cash_Voucher;
                drLedger["VOUCHER_NO"] = MaxDocumentID;
                drLedger["ACCOUNT_HEAD_ID"] = SAMSCommon.Classes.Configuration.AccountReceivable;
                drLedger["Distributor_ID"] = drpDistributor.SelectedValue;
                if (decimal.Parse(dr.Cells[3].Text) >= OfferAmount)
                {
                    drLedger["DEBIT"] = 0;
                    drLedger["CREDIT"] = OfferAmount;
                }
                else if (decimal.Parse(dr.Cells[3].Text) <= OfferAmount)
                {
                    drLedger["DEBIT"] = 0;
                    drLedger["CREDIT"] = dr.Cells[3].Text;
                }
                drLedger["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
                drLedger["Remarks"] = txtRemarks.Text;
                drLedger["TimeStamp"] = DateTime.Now;
                drLedger["Customer_ID"] = DrpCustomer.SelectedValue;
                drLedger["Principal_ID"] = DrpPrincipal.SelectedValue;
                drLedger["Cheque_NO"] = txtChequeNo.Text;
                drLedger["UserID"] = this.Session["UserId"].ToString();
                drLedger["Document_ID"] = GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"].ToString();
                drLedger["Manual_Document_ID"] = dr.Cells[1].Text;
                drLedger["DocumentTypeID"] = Constants.Document_Invoice;
                drLedger["SlipNo"] = txtSlipNo.Text;
                drLedger["ChequeDate"] = Constants.DateNullValue;
                drLedger["PaymentMode"] = 19;
                drLedger["PayeesName"] = DrpDeliveryMan.SelectedValue;
                dtLedger.Rows.Add(drLedger);

                //Debit to selected Account Head
                DataRow drLedger2 = dtLedger.NewRow();
                drLedger2["VOUCHER_TYPE_ID"] = Constants.Cash_Voucher;
                drLedger2["VOUCHER_NO"] = MaxDocumentID;
                drLedger2["ACCOUNT_HEAD_ID"] = DrpAccountDetail.SelectedValue;
                drLedger2["Distributor_ID"] = drpDistributor.SelectedValue;
                if (decimal.Parse(dr.Cells[3].Text) >= OfferAmount)
                {
                    drLedger2["DEBIT"] = OfferAmount;
                    drLedger2["CREDIT"] = 0;
                }
                else if (decimal.Parse(dr.Cells[3].Text) <= OfferAmount)
                {
                    drLedger2["DEBIT"] = dr.Cells[3].Text;
                    drLedger2["CREDIT"] = 0;
                }

                drLedger2["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
                drLedger2["Remarks"] = txtRemarks.Text;
                drLedger2["TimeStamp"] = DateTime.Now;
                drLedger2["Customer_ID"] = DrpCustomer.SelectedValue;
                drLedger2["Principal_ID"] = DrpPrincipal.SelectedValue;
                drLedger2["Cheque_NO"] = txtChequeNo.Text;
                drLedger2["UserID"] = this.Session["UserId"].ToString();
                drLedger2["Document_ID"] = GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"].ToString();
                drLedger2["Manual_Document_ID"] = dr.Cells[1].Text;
                drLedger2["DocumentTypeID"] = Constants.Document_Invoice;
                drLedger2["SlipNo"] = txtSlipNo.Text;
                drLedger2["ChequeDate"] = Constants.DateNullValue;
                drLedger2["PaymentMode"] = 19;
                drLedger2["PayeesName"] = DrpDeliveryMan.SelectedValue;
                dtLedger.Rows.Add(drLedger2);

                if (decimal.Parse(dr.Cells[3].Text) >= OfferAmount)
                {
                    realizeAmount += OfferAmount;
                    OfferAmount = decimal.Parse(dr.Cells[3].Text) - OfferAmount;
                    LController.PostingCash_Bank_Account(dtLedger, Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), int.Parse(drpDistributor.SelectedValue.ToString()), OfferAmount);
                }
                else if (decimal.Parse(dr.Cells[3].Text) <= OfferAmount)
                {
                    realizeAmount += Convert.ToDecimal(dr.Cells[3].Text);
                    OfferAmount = OfferAmount - decimal.Parse(dr.Cells[3].Text);
                    LController.PostingCash_Bank_Account(dtLedger, Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), int.Parse(drpDistributor.SelectedValue.ToString()), 0);
                }
                //break;
                #endregion
            }
        }
        if (realizeAmount < decimal.Parse(txtAmount.Text))
        {
            CashAdvanceRemaining(OfferAmount, MaxDocumentID);
        }
    }

    private bool Is_Advance()
    {
        decimal OfferAmount = decimal.Parse(txtAmount.Text);
        decimal realizeAmount = 0;
        foreach (GridViewRow dr in GrdCredit.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                #region

                if (decimal.Parse(dr.Cells[3].Text) >= OfferAmount)
                {
                    realizeAmount += OfferAmount;
                    OfferAmount = decimal.Parse(dr.Cells[3].Text) - OfferAmount;
                }
                else if (decimal.Parse(dr.Cells[3].Text) <= OfferAmount)
                {
                    realizeAmount += Convert.ToDecimal(dr.Cells[3].Text);
                    OfferAmount = OfferAmount - decimal.Parse(dr.Cells[3].Text);
                }
                //break;
                #endregion
            }
        }
        if (realizeAmount < decimal.Parse(txtAmount.Text))
        {
            return true;
        }
        return false;

    }

    private void OnlineTransfer()
    {
        LedgerController LController = new LedgerController();
        string MaxDocumentID = LController.SelectLedgerMaxDocumentId(Constants.Cash_Voucher, int.Parse(drpDistributor.SelectedValue.ToString()));
        Session.Add("VoucherNo", MaxDocumentID);
        decimal OfferAmount = decimal.Parse(txtAmount.Text);
        decimal realizeAmount = 0;
        foreach (GridViewRow dr in GrdCredit.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                #region
                DataTable dtLedger = CreateTable();

                //Credit from Account Receivable (Party Wise)   CD
                DataRow drLedger = dtLedger.NewRow();
                drLedger["VOUCHER_TYPE_ID"] = Constants.Cash_Voucher;
                drLedger["VOUCHER_NO"] = MaxDocumentID;
                drLedger["ACCOUNT_HEAD_ID"] = SAMSCommon.Classes.Configuration.AccountReceivable;
                drLedger["Distributor_ID"] = drpDistributor.SelectedValue;
                if (decimal.Parse(dr.Cells[3].Text) >= OfferAmount)
                {
                    drLedger["DEBIT"] = 0;
                    drLedger["CREDIT"] = OfferAmount;
                }
                else if (decimal.Parse(dr.Cells[3].Text) <= OfferAmount)
                {
                    drLedger["DEBIT"] = 0;
                    drLedger["CREDIT"] = dr.Cells[3].Text;
                }
                drLedger["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
                drLedger["Remarks"] = txtRemarks.Text;
                drLedger["TimeStamp"] = DateTime.Now;
                drLedger["Customer_ID"] = DrpCustomer.SelectedValue;
                drLedger["Principal_ID"] = DrpPrincipal.SelectedValue;
                drLedger["Cheque_NO"] = txtChequeNo.Text;
                drLedger["UserID"] = this.Session["UserId"].ToString();
                drLedger["Document_ID"] = GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"].ToString();
                drLedger["Manual_Document_ID"] = dr.Cells[1].Text;
                drLedger["DocumentTypeID"] = Constants.Document_Invoice;
                drLedger["SlipNo"] = txtSlipNo.Text;
                drLedger["ChequeDate"] = Constants.DateNullValue;
                drLedger["PaymentMode"] = Constants.OnlineTransfer;
                drLedger["PayeesName"] = DrpDeliveryMan.SelectedValue;
                dtLedger.Rows.Add(drLedger);

                //Debit to selected Account Head
                DataRow drLedger2 = dtLedger.NewRow();
                drLedger2["VOUCHER_TYPE_ID"] = Constants.Cash_Voucher;
                drLedger2["VOUCHER_NO"] = MaxDocumentID;
                drLedger2["ACCOUNT_HEAD_ID"] = DrpAccountDetail.SelectedValue;
                drLedger2["Distributor_ID"] = drpDistributor.SelectedValue;
                if (decimal.Parse(dr.Cells[3].Text) >= OfferAmount)
                {
                    drLedger2["DEBIT"] = OfferAmount;
                    drLedger2["CREDIT"] = 0;
                }
                else if (decimal.Parse(dr.Cells[3].Text) <= OfferAmount)
                {
                    drLedger2["DEBIT"] = dr.Cells[3].Text;
                    drLedger2["CREDIT"] = 0;
                }

                drLedger2["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
                drLedger2["Remarks"] = txtRemarks.Text;
                drLedger2["TimeStamp"] = DateTime.Now;
                drLedger2["Customer_ID"] = DrpCustomer.SelectedValue;
                drLedger2["Principal_ID"] = DrpPrincipal.SelectedValue;
                drLedger2["Cheque_NO"] = txtChequeNo.Text;
                drLedger2["UserID"] = this.Session["UserId"].ToString();
                drLedger2["Document_ID"] = GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"].ToString();
                drLedger2["Manual_Document_ID"] = dr.Cells[1].Text;
                drLedger2["DocumentTypeID"] = Constants.Document_Invoice;
                drLedger2["SlipNo"] = txtSlipNo.Text;
                drLedger2["ChequeDate"] = Constants.DateNullValue;
                drLedger2["PaymentMode"] = Constants.OnlineTransfer;
                drLedger2["PayeesName"] = DrpDeliveryMan.SelectedValue;
                dtLedger.Rows.Add(drLedger2);

                if (decimal.Parse(dr.Cells[3].Text) >= OfferAmount)
                {
                    realizeAmount += OfferAmount;
                    OfferAmount = decimal.Parse(dr.Cells[3].Text) - OfferAmount;
                    LController.PostingCash_Bank_Account(dtLedger, Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), int.Parse(drpDistributor.SelectedValue.ToString()), OfferAmount);
                }
                else if (decimal.Parse(dr.Cells[3].Text) <= OfferAmount)
                {
                    realizeAmount += Convert.ToDecimal(dr.Cells[3].Text);
                    OfferAmount = OfferAmount - decimal.Parse(dr.Cells[3].Text);
                    LController.PostingCash_Bank_Account(dtLedger, Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), int.Parse(drpDistributor.SelectedValue.ToString()), 0);
                }
                //break;
                #endregion
            }
        }
        if (realizeAmount < decimal.Parse(txtAmount.Text))
        {

            CashAdvanceRemaining(OfferAmount, MaxDocumentID);
        }

    }


    /// <summary>
    /// Saves Cash Advance
    /// </summary>
    private void CashAdvance()
    {
        LedgerController LController = new LedgerController();
        string MaxDocumentID = LController.SelectLedgerMaxDocumentId(Constants.Cash_Voucher, int.Parse(drpDistributor.SelectedValue.ToString()));

        DataTable dtLedger = CreateTable();

        //Credit from Account Receivable (Party Wise)
        DataRow dr = dtLedger.NewRow();

        dr["VOUCHER_TYPE_ID"] = Constants.Cash_Voucher;
        dr["VOUCHER_NO"] = MaxDocumentID;
        dr["ACCOUNT_HEAD_ID"] = SAMSCommon.Classes.Configuration.AccountReceivable;
        dr["Distributor_ID"] = drpDistributor.SelectedValue;
        dr["DEBIT"] = 0;
        dr["CREDIT"] = txtAmount.Text;
        dr["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
        dr["Remarks"] = txtRemarks.Text;
        dr["TimeStamp"] = DateTime.Now;
        dr["Customer_ID"] = DrpCustomer.SelectedValue;
        dr["Principal_ID"] = DrpPrincipal.SelectedValue;
        dr["Cheque_NO"] = txtChequeNo.Text;
        dr["UserID"] = this.Session["UserId"].ToString();
        dr["Document_ID"] = Constants.LongNullValue;
        dr["Manual_Document_ID"] = null;
        dr["DocumentTypeID"] = Constants.IntNullValue;
        dr["SlipNo"] = txtSlipNo.Text;
        dr["ChequeDate"] = Constants.DateNullValue;
        dr["PaymentMode"] = 21;
        dr["PayeesName"] = "";
        dtLedger.Rows.Add(dr);

        //Debit to selected Account Head
        DataRow dr2 = dtLedger.NewRow();

        dr2["VOUCHER_TYPE_ID"] = Constants.Cash_Voucher;
        dr2["VOUCHER_NO"] = MaxDocumentID;
        dr2["ACCOUNT_HEAD_ID"] = DrpAccountDetail.SelectedValue;
        dr2["Distributor_ID"] = drpDistributor.SelectedValue;
        dr2["DEBIT"] = txtAmount.Text;
        dr2["CREDIT"] = 0;
        dr2["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
        dr2["Remarks"] = txtRemarks.Text;
        dr2["TimeStamp"] = DateTime.Now;
        dr2["Customer_ID"] = DrpCustomer.SelectedValue;
        dr2["Principal_ID"] = DrpPrincipal.SelectedValue;
        dr2["Cheque_NO"] = txtChequeNo.Text;
        dr2["UserID"] = this.Session["UserId"].ToString();
        dr2["Document_ID"] = Constants.LongNullValue;
        dr2["Manual_Document_ID"] = null;
        dr2["DocumentTypeID"] = Constants.IntNullValue;
        dr2["SlipNo"] = txtSlipNo.Text;
        dr2["ChequeDate"] = Constants.DateNullValue;
        dr2["PaymentMode"] = 21;
        dr2["PayeesName"] = "";
        dtLedger.Rows.Add(dr2);

        LController.PostingCash_Bank_Account(dtLedger);
    }

    private void CashAdvanceRemaining(decimal p_Amount, string MaxDocumentID)
    {
        LedgerController LController = new LedgerController();
        // string MaxDocumentID = LController.SelectLedgerMaxDocumentId(Constants.Cash_Voucher, int.Parse(drpDistributor.SelectedValue.ToString()));

        DataTable dtLedger = CreateTable();

        //Credit from Account Receivable (Party Wise)
        DataRow dr = dtLedger.NewRow();

        dr["VOUCHER_TYPE_ID"] = Constants.Cash_Voucher;
        dr["VOUCHER_NO"] = MaxDocumentID;
        dr["ACCOUNT_HEAD_ID"] = SAMSCommon.Classes.Configuration.AccountReceivable;
        dr["Distributor_ID"] = drpDistributor.SelectedValue;
        dr["DEBIT"] = 0;
        dr["CREDIT"] = p_Amount;
        dr["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
        dr["Remarks"] = txtRemarks.Text;
        dr["TimeStamp"] = DateTime.Now;
        dr["Customer_ID"] = DrpCustomer.SelectedValue;
        dr["Principal_ID"] = DrpPrincipal.SelectedValue;
        dr["Cheque_NO"] = txtChequeNo.Text;
        dr["UserID"] = this.Session["UserId"].ToString();
        dr["Document_ID"] = Constants.LongNullValue;
        dr["Manual_Document_ID"] = null;
        dr["DocumentTypeID"] = Constants.IntNullValue;
        dr["SlipNo"] = txtSlipNo.Text;
        dr["ChequeDate"] = Constants.DateNullValue;
        dr["PaymentMode"] = 21;
        dr["PayeesName"] = "";
        dtLedger.Rows.Add(dr);

        //Debit to selected Account Head
        DataRow dr2 = dtLedger.NewRow();

        dr2["VOUCHER_TYPE_ID"] = Constants.Cash_Voucher;
        dr2["VOUCHER_NO"] = MaxDocumentID;
        dr2["ACCOUNT_HEAD_ID"] = DrpAccountDetail.SelectedValue;
        dr2["Distributor_ID"] = drpDistributor.SelectedValue;
        dr2["DEBIT"] = p_Amount;
        dr2["CREDIT"] = 0;
        dr2["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
        dr2["Remarks"] = txtRemarks.Text;
        dr2["TimeStamp"] = DateTime.Now;
        dr2["Customer_ID"] = DrpCustomer.SelectedValue;
        dr2["Principal_ID"] = DrpPrincipal.SelectedValue;
        dr2["Cheque_NO"] = txtChequeNo.Text;
        dr2["UserID"] = this.Session["UserId"].ToString();
        dr2["Document_ID"] = Constants.LongNullValue;
        dr2["Manual_Document_ID"] = null;
        dr2["DocumentTypeID"] = Constants.IntNullValue;
        dr2["SlipNo"] = txtSlipNo.Text;
        dr2["ChequeDate"] = Constants.DateNullValue;
        dr2["PaymentMode"] = 21;
        dr2["PayeesName"] = "";
        dtLedger.Rows.Add(dr2);

        LController.PostingCash_Bank_Account(dtLedger);
    }

    /// <summary>
    /// Saves Bank Deposits For Branch And Deliveryman
    /// </summary>
    /// <param name="p_SaleForceID"></param>
    private string BankDeposit(string p_SaleForceID)
    {
        LedgerController LController = new LedgerController();
        string MaxDocumentID = LController.SelectLedgerMaxDocumentId(Constants.Bank_Voucher, int.Parse(drpDistributor.SelectedValue.ToString()));
        DataTable dtLedger = CreateTable();

        //Credit from NCS
        DataRow dr = dtLedger.NewRow();
        dr["VOUCHER_TYPE_ID"] = Constants.Bank_Voucher;
        dr["VOUCHER_NO"] = MaxDocumentID;
        dr["ACCOUNT_HEAD_ID"] = Configuration.CashDefault;
        dr["Distributor_ID"] = drpDistributor.SelectedValue;
        dr["DEBIT"] = 0;
        dr["CREDIT"] = txtAmount.Text;
        dr["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
        dr["Remarks"] = txtRemarks.Text;
        dr["TimeStamp"] = DateTime.Now;
        dr["Customer_ID"] = Constants.IntNullValue;
        dr["Principal_ID"] = DrpPrincipal.SelectedValue;
        dr["Cheque_NO"] = txtChequeNo.Text;
        dr["UserID"] = this.Session["UserId"].ToString();
        dr["Document_ID"] = Constants.LongNullValue;
        dr["Manual_Document_ID"] = null;
        dr["DocumentTypeID"] = Constants.IntNullValue;
        dr["SlipNo"] = txtSlipNo.Text;
        dr["ChequeDate"] = Constants.DateNullValue;
        dr["PaymentMode"] = 22;
        dr["PayeesName"] = p_SaleForceID;
        dtLedger.Rows.Add(dr);

        //Debit to selected Account Head
        DataRow dr2 = dtLedger.NewRow();
        dr2["VOUCHER_TYPE_ID"] = Constants.Bank_Voucher;
        dr2["VOUCHER_NO"] = MaxDocumentID;
        dr2["ACCOUNT_HEAD_ID"] = DrpAccountDetail.SelectedValue;
        dr2["Distributor_ID"] = drpDistributor.SelectedValue;
        dr2["DEBIT"] = txtAmount.Text;
        dr2["CREDIT"] = 0;
        dr2["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
        dr2["Remarks"] = txtRemarks.Text;
        dr2["TimeStamp"] = DateTime.Now;
        dr2["Customer_ID"] = Constants.IntNullValue;
        dr2["Principal_ID"] = DrpPrincipal.SelectedValue;
        dr2["Cheque_NO"] = txtChequeNo.Text;
        dr2["UserID"] = this.Session["UserId"].ToString();
        dr2["Document_ID"] = Constants.LongNullValue;
        dr2["Manual_Document_ID"] = null;
        dr2["DocumentTypeID"] = Constants.IntNullValue;
        dr2["SlipNo"] = txtSlipNo.Text;
        dr2["ChequeDate"] = Constants.DateNullValue;
        dr2["PaymentMode"] = 22;
        dr2["PayeesName"] = p_SaleForceID;
        dtLedger.Rows.Add(dr2);

        LController.PostingCash_Bank_Account(dtLedger);
        return MaxDocumentID;
    }

    /// <summary>
    /// Saves Petty Cash
    /// </summary>
    private void PettyCash()
    {
        LedgerController LController = new LedgerController();
        string MaxDocumentId = LController.SelectLedgerMaxDocumentId(Constants.Cash_Voucher, int.Parse(drpDistributor.SelectedValue.ToString()));

        LController.PostingCash_Bank_Account(Constants.Cash_Voucher, long.Parse(MaxDocumentId), long.Parse(SAMSCommon.Classes.Configuration.CashDefault), int.Parse(drpDistributor.SelectedValue.ToString()), decimal.Parse(txtAmount.Text), 0,
                 DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), txtRemarks.Text, DateTime.Now, Constants.IntNullValue, int.Parse(DrpPrincipal.SelectedValue.ToString()),
                 txtChequeNo.Text, int.Parse(this.Session["UserId"].ToString()), Constants.LongNullValue, null, Constants.IntNullValue, txtSlipNo.Text, Constants.DateNullValue, int.Parse(DrpAccountType.SelectedValue.ToString()), "");

    }

    /// <summary>
    /// Loads Deliveryment To Deliveryman Combo
    /// </summary>
    private void LoadDeliveryman()
    {
        if (drpDistributor.Items.Count > 0)
        {
            SaleForceController mDController = new SaleForceController();
            DataTable m_dt = mDController.SelectSaleForceAssignedArea(int.Parse(drpDistributor.SelectedValue.ToString()), Constants.IntNullValue, int.Parse(this.Session["CompanyId"].ToString()));
            clsWebFormUtil.FillDropDownList(this.DrpDeliveryMan, m_dt, 0, 3, true);
        }
    }

    /// <summary>
    /// Loads Credit Invoices To Invoice Grid
    /// </summary>
    private void SelectCreditInvoice()
    {
        LedgerController CDC = new LedgerController();
        GrdCredit.DataSource = null;
        GrdCredit.DataBind();
        if (DrpCustomer.Items.Count > 0 && DrpAccountType.SelectedIndex != 1 && DrpAccountType.SelectedIndex != 5)
        {
            DataTable dtCredit = CDC.SelectCreditPendingInvoice2(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()), long.Parse(DrpCustomer.SelectedValue.ToString()), 0, int.Parse(DrpDeliveryMan.SelectedValue));
            GrdCredit.DataSource = dtCredit;
            GrdCredit.DataBind();
            decimal objtotalAmount = 0;
            
            foreach (GridViewRow dr in GrdCredit.Rows)
            {
                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                chRelized.Checked = true;
                //chRelized.Enabled = false;
            }
            if (dtCredit.Rows.Count > 0)
            {
                foreach (DataRow row in dtCredit.Rows)
                {
                    objtotalAmount += decimal.Parse(row["CURRENT_CREDIT_AMOUNT"].ToString());
                }
                txtTotalCreditAmount.Text = String.Format("{0:N2}", Math.Round(objtotalAmount, 2));
            }
            else
            {
                txtTotalCreditAmount.Text = String.Format("{0:N2}", Math.Round(objtotalAmount, 2));
            }
        }
    }

    /// <summary>
    /// Saves Income Tax
    /// </summary>
    private void IncomeTax()
    {
        LedgerController LController = new LedgerController();
        string MaxDocumentID = LController.SelectLedgerMaxDocumentId(Constants.Journal_Voucher, int.Parse(drpDistributor.SelectedValue.ToString()));
        decimal OfferAmount = decimal.Parse(txtAmount.Text);
        foreach (GridViewRow dr in GrdCredit.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {

                DataTable dtLedger = CreateTable();

                //Credit from Account Receivable (Party Wise)
                DataRow drLedger = dtLedger.NewRow();
                drLedger["VOUCHER_TYPE_ID"] = Constants.Journal_Voucher;
                drLedger["VOUCHER_NO"] = MaxDocumentID;
                drLedger["ACCOUNT_HEAD_ID"] = SAMSCommon.Classes.Configuration.AccountReceivable;
                drLedger["Distributor_ID"] = drpDistributor.SelectedValue;
                if (decimal.Parse(dr.Cells[3].Text) >= OfferAmount)
                {
                    drLedger["DEBIT"] = 0;
                    drLedger["CREDIT"] = OfferAmount;
                }
                else if (decimal.Parse(dr.Cells[3].Text) <= OfferAmount)
                {
                    drLedger["DEBIT"] = 0;
                    drLedger["CREDIT"] = dr.Cells[3].Text;
                }
                drLedger["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
                drLedger["Remarks"] = txtRemarks.Text;
                drLedger["TimeStamp"] = DateTime.Now;
                drLedger["Customer_ID"] = DrpCustomer.SelectedValue;
                drLedger["Principal_ID"] = DrpPrincipal.SelectedValue;
                drLedger["Cheque_NO"] = txtChequeNo.Text;
                drLedger["UserID"] = this.Session["UserId"].ToString();
                drLedger["Document_ID"] = GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"].ToString();
                drLedger["Manual_Document_ID"] = dr.Cells[1].Text;
                drLedger["DocumentTypeID"] = Constants.Document_Invoice;
                drLedger["SlipNo"] = txtSlipNo.Text;
                drLedger["ChequeDate"] = Constants.DateNullValue;
                drLedger["PaymentMode"] = DrpAccountType.SelectedValue;
                drLedger["PayeesName"] = DrpDeliveryMan.SelectedValue;
                dtLedger.Rows.Add(drLedger);

                //Debit to selected Account Head
                DataRow drLedger2 = dtLedger.NewRow();
                drLedger2["VOUCHER_TYPE_ID"] = Constants.Journal_Voucher;
                drLedger2["VOUCHER_NO"] = MaxDocumentID;
                drLedger2["ACCOUNT_HEAD_ID"] = DrpAccountDetail.SelectedValue;
                drLedger2["Distributor_ID"] = drpDistributor.SelectedValue;
                if (decimal.Parse(dr.Cells[3].Text) >= OfferAmount)
                {
                    drLedger2["DEBIT"] = OfferAmount;
                    drLedger2["CREDIT"] = 0;
                }
                else if (decimal.Parse(dr.Cells[3].Text) <= OfferAmount)
                {
                    drLedger2["DEBIT"] = dr.Cells[3].Text;
                    drLedger2["CREDIT"] = 0;
                }

                drLedger2["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
                drLedger2["Remarks"] = txtRemarks.Text;
                drLedger2["TimeStamp"] = DateTime.Now;
                drLedger2["Customer_ID"] = DrpCustomer.SelectedValue;
                drLedger2["Principal_ID"] = DrpPrincipal.SelectedValue;
                drLedger2["Cheque_NO"] = txtChequeNo.Text;
                drLedger2["UserID"] = this.Session["UserId"].ToString();
                drLedger2["Document_ID"] = GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"].ToString();
                drLedger2["Manual_Document_ID"] = dr.Cells[1].Text;
                drLedger2["DocumentTypeID"] = Constants.Document_Invoice;
                drLedger2["SlipNo"] = txtSlipNo.Text;
                drLedger2["ChequeDate"] = Constants.DateNullValue;
                drLedger2["PaymentMode"] = DrpAccountType.SelectedValue;
                drLedger2["PayeesName"] = DrpDeliveryMan.SelectedValue;
                dtLedger.Rows.Add(drLedger2);

                if (decimal.Parse(dr.Cells[3].Text) >= OfferAmount)
                {
                    OfferAmount = decimal.Parse(dr.Cells[3].Text) - OfferAmount;
                    LController.PostingCash_Bank_Account(dtLedger, Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), int.Parse(drpDistributor.SelectedValue.ToString()), OfferAmount);
                }
                else if (decimal.Parse(dr.Cells[3].Text) <= OfferAmount)
                {
                    OfferAmount = OfferAmount - decimal.Parse(dr.Cells[3].Text);
                    LController.PostingCash_Bank_Account(dtLedger, Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), int.Parse(drpDistributor.SelectedValue.ToString()), 0);
                }
                break;
            }
        }
    }

    private void IncomeTax2()
    {
        LedgerController LController = new LedgerController();
        string MaxDocumentID = LController.SelectLedgerMaxDocumentId(Constants.Journal_Voucher, int.Parse(drpDistributor.SelectedValue.ToString()));

        DataTable dtCredit = LController.SelectWHTAX(long.Parse(DrpCustomer.SelectedValue.ToString()));



        decimal WHTAX = decimal.Parse(dtCredit.Rows[0][0].ToString());
        decimal OfferAmount = decimal.Parse((WHTAX * (decimal.Parse(txtAmount.Text)) / 100).ToString());
        foreach (GridViewRow dr in GrdCredit.Rows)
        {

            DataTable dtLedger = CreateTable();

            //Credit from Account Receivable (Party Wise)
            DataRow drLedger = dtLedger.NewRow();
            drLedger["VOUCHER_TYPE_ID"] = Constants.Journal_Voucher;
            drLedger["VOUCHER_NO"] = MaxDocumentID;
            drLedger["ACCOUNT_HEAD_ID"] = SAMSCommon.Classes.Configuration.AccountReceivable;
            drLedger["Distributor_ID"] = drpDistributor.SelectedValue;
            if (decimal.Parse(dr.Cells[3].Text) >= OfferAmount)
            {
                drLedger["DEBIT"] = 0;
                drLedger["CREDIT"] = OfferAmount;
            }
            else if (decimal.Parse(dr.Cells[3].Text) <= OfferAmount)
            {
                drLedger["DEBIT"] = 0;
                drLedger["CREDIT"] = dr.Cells[3].Text;
            }
            drLedger["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
            drLedger["Remarks"] = txtRemarks.Text;
            drLedger["TimeStamp"] = DateTime.Now;
            drLedger["Customer_ID"] = DrpCustomer.SelectedValue;
            drLedger["Principal_ID"] = DrpPrincipal.SelectedValue;
            drLedger["Cheque_NO"] = txtChequeNo.Text;
            drLedger["UserID"] = this.Session["UserId"].ToString();
            drLedger["Document_ID"] = GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"].ToString();
            drLedger["Manual_Document_ID"] = dr.Cells[1].Text;
            drLedger["DocumentTypeID"] = Constants.Document_Invoice;
            drLedger["SlipNo"] = txtSlipNo.Text;
            drLedger["ChequeDate"] = Constants.DateNullValue;
            drLedger["PaymentMode"] = DrpAccountType.SelectedValue;
            drLedger["PayeesName"] = DrpDeliveryMan.SelectedValue;
            dtLedger.Rows.Add(drLedger);

            //Debit to selected Account Head
            DataRow drLedger2 = dtLedger.NewRow();
            drLedger2["VOUCHER_TYPE_ID"] = Constants.Journal_Voucher;
            drLedger2["VOUCHER_NO"] = MaxDocumentID;
            drLedger2["ACCOUNT_HEAD_ID"] = DrpAccountDetail.SelectedValue;
            drLedger2["Distributor_ID"] = drpDistributor.SelectedValue;
            if (decimal.Parse(dr.Cells[3].Text) >= OfferAmount)
            {
                drLedger2["DEBIT"] = OfferAmount;
                drLedger2["CREDIT"] = 0;
            }
            else if (decimal.Parse(dr.Cells[3].Text) <= OfferAmount)
            {
                drLedger2["DEBIT"] = dr.Cells[3].Text;
                drLedger2["CREDIT"] = 0;
            }

            drLedger2["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
            drLedger2["Remarks"] = txtRemarks.Text;
            drLedger2["TimeStamp"] = DateTime.Now;
            drLedger2["Customer_ID"] = DrpCustomer.SelectedValue;
            drLedger2["Principal_ID"] = DrpPrincipal.SelectedValue;
            drLedger2["Cheque_NO"] = txtChequeNo.Text;
            drLedger2["UserID"] = this.Session["UserId"].ToString();
            drLedger2["Document_ID"] = GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"].ToString();
            drLedger2["Manual_Document_ID"] = dr.Cells[1].Text;
            drLedger2["DocumentTypeID"] = Constants.Document_Invoice;
            drLedger2["SlipNo"] = txtSlipNo.Text;
            drLedger2["ChequeDate"] = Constants.DateNullValue;
            drLedger2["PaymentMode"] = DrpAccountType.SelectedValue;
            drLedger2["PayeesName"] = DrpDeliveryMan.SelectedValue;
            dtLedger.Rows.Add(drLedger2);

            if (decimal.Parse(dr.Cells[3].Text) >= OfferAmount)
            {
                OfferAmount = decimal.Parse(dr.Cells[3].Text) - OfferAmount;
                LController.PostingCash_Bank_Account(dtLedger, Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), int.Parse(drpDistributor.SelectedValue.ToString()), OfferAmount);
            }
            else if (decimal.Parse(dr.Cells[3].Text) <= OfferAmount)
            {
                OfferAmount = OfferAmount - decimal.Parse(dr.Cells[3].Text);
                LController.PostingCash_Bank_Account(dtLedger, Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), int.Parse(drpDistributor.SelectedValue.ToString()), 0);
            }
            break;

        }
    }

    /// <summary>
    /// Saves Advance Return
    /// </summary>
    private void AdvanceReturn()
    {

        LedgerController LController = new LedgerController();
        string MaxDocumentID = LController.SelectLedgerMaxDocumentId(Constants.Cash_Voucher, int.Parse(drpDistributor.SelectedValue.ToString()));
        DataTable dtLedger = CreateTable();

        //Debit to Account Receivable (Party Wise)
        DataRow dr = dtLedger.NewRow();
        dr["VOUCHER_TYPE_ID"] = Constants.Cash_Voucher;
        dr["VOUCHER_NO"] = MaxDocumentID;
        dr["ACCOUNT_HEAD_ID"] = SAMSCommon.Classes.Configuration.AccountReceivable;
        dr["Distributor_ID"] = drpDistributor.SelectedValue;
        dr["DEBIT"] = txtAmount.Text;
        dr["CREDIT"] = 0;
        dr["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
        dr["Remarks"] = txtRemarks.Text;
        dr["TimeStamp"] = DateTime.Now;
        dr["Customer_ID"] = DrpCustomer.SelectedValue;
        dr["Principal_ID"] = DrpPrincipal.SelectedValue;
        dr["Cheque_NO"] = txtChequeNo.Text;
        dr["UserID"] = this.Session["UserId"].ToString();
        dr["Document_ID"] = Constants.LongNullValue;
        dr["Manual_Document_ID"] = null;
        dr["DocumentTypeID"] = Constants.IntNullValue;
        dr["SlipNo"] = txtSlipNo.Text;
        dr["ChequeDate"] = Constants.DateNullValue;
        dr["PaymentMode"] = 29;
        dr["PayeesName"] = "";
        dtLedger.Rows.Add(dr);

        //Credit from selected Account Head
        DataRow dr2 = dtLedger.NewRow();
        dr2["VOUCHER_TYPE_ID"] = Constants.Cash_Voucher;
        dr2["VOUCHER_NO"] = MaxDocumentID;
        dr2["ACCOUNT_HEAD_ID"] = DrpAccountDetail.SelectedValue;
        dr2["Distributor_ID"] = drpDistributor.SelectedValue;
        dr2["DEBIT"] = 0;
        dr2["CREDIT"] = txtAmount.Text;
        dr2["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
        dr2["Remarks"] = txtRemarks.Text;
        dr2["TimeStamp"] = DateTime.Now;
        dr2["Customer_ID"] = DrpCustomer.SelectedValue;
        dr2["Principal_ID"] = DrpPrincipal.SelectedValue;
        dr2["Cheque_NO"] = txtChequeNo.Text;
        dr2["UserID"] = this.Session["UserId"].ToString();
        dr2["Document_ID"] = Constants.LongNullValue;
        dr2["Manual_Document_ID"] = null;
        dr2["DocumentTypeID"] = Constants.IntNullValue;
        dr2["SlipNo"] = txtSlipNo.Text;
        dr2["ChequeDate"] = Constants.DateNullValue;
        dr2["PaymentMode"] = 29;
        dr2["PayeesName"] = "";
        dtLedger.Rows.Add(dr2);

        LController.PostingCash_Bank_Account(dtLedger);
    }

    /// <summary>
    /// Loads Routes, Customers, Deliverymen And Bank Transactions
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.DrpAccountType.SelectedIndex != 7)
        {
            this.LoadArea();
            this.LoadData();
            this.LoadGrid();
            gvSaleForceCash.Visible = false;
            GrdOrder.Visible = true;
        }
        else if (this.DrpAccountType.SelectedIndex == 7)
        {
            this.LoadSaleFoceCash();
        }
        this.LoadAccountHead();
        this.LoadDeliveryman();
        this.SetTableSorter();
    }

    /// <summary>
    /// Loads Customers And Credit Invoices
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpRoute_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadData();
        this.SelectCreditInvoice();
        this.SetTableSorter();
    }

    /// <summary>
    /// Loads Customers
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpAccountDetail_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadGrid();
        this.SetTableSorter();
    }

    /// <summary>
    /// Deletes Bank Transaction
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void GrdOrder_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        LedgerController LController = new LedgerController();
        DataControl dc = new DataControl();

        LController.DeleteCashBankTransction(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(dc.chkNull_0(GrdOrder.Rows[e.RowIndex].Cells[2].Text)),
        int.Parse(dc.chkNull_0(GrdOrder.Rows[e.RowIndex].Cells[7].Text)), long.Parse(dc.chkNull_0(GrdOrder.Rows[e.RowIndex].Cells[10].Text.Replace("&nbsp;", ""))), decimal.Parse(dc.chkNull_0(GrdOrder.Rows[e.RowIndex].Cells[8].Text)));
        this.LoadGrid();
        this.SelectCreditInvoice();
        this.SetTableSorter();
    }

    /// <summary>
    /// Loads Account Head, Customers, Deliverymen And Bank Transactions
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpAccountType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.DrpAccountType.SelectedIndex == 2 || this.DrpAccountType.SelectedIndex == 7)
        {
            this.HideShowControls(false);
        }
        else
        {
            this.HideShowControls(true);
        }

        if (this.DrpAccountType.SelectedIndex != 7)
        {
            this.LoadData();
            this.LoadAccountHead();
            this.LoadGrid();
            if (DrpAccountType.SelectedValue == "19" || DrpAccountType.SelectedValue == "30")
            {
                SelectCreditInvoice();
            }
            gvSaleForceCash.Visible = false;
            GrdOrder.Visible = true;

        }
        else
        {
            this.LoadSaleFoceCash();
        }
        this.SetTableSorter();

    }

    /// <summary>
    /// Loads Credit Invoices
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpCustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.SelectCreditInvoice();
        this.SetTableSorter();
    }

    /// <summary>
    /// Loads Credit Invoices And Customers
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.DrpAccountType.SelectedIndex != 7)
        {
            this.LoadData();
            this.SelectCreditInvoice();
            gvSaleForceCash.Visible = false;
            GrdOrder.Visible = true;
        }
        else
        {
            this.LoadSaleFoceCash();
        }
        this.SetTableSorter();
    }

    /// <summary>
    /// Creates Datatable For Bank Transaction
    /// </summary>
    /// <returns></returns>
    private DataTable CreateTable()
    {
        DataTable dtLedger = new DataTable();
        dtLedger.Columns.Add("VOUCHER_TYPE_ID", typeof(int));
        dtLedger.Columns.Add("VOUCHER_NO", typeof(long));
        dtLedger.Columns.Add("ACCOUNT_HEAD_ID", typeof(long));
        dtLedger.Columns.Add("Distributor_ID", typeof(int));
        dtLedger.Columns.Add("Debit", typeof(decimal));
        dtLedger.Columns.Add("Credit", typeof(decimal));
        dtLedger.Columns.Add("Ledger_Date", typeof(DateTime));
        dtLedger.Columns.Add("Remarks", typeof(string));
        dtLedger.Columns.Add("TimeStamp", typeof(DateTime));
        dtLedger.Columns.Add("Customer_ID", typeof(int));
        dtLedger.Columns.Add("Principal_ID", typeof(int));
        dtLedger.Columns.Add("Cheque_NO", typeof(string));
        dtLedger.Columns.Add("UserId", typeof(int));
        dtLedger.Columns.Add("Document_ID", typeof(long));
        dtLedger.Columns.Add("Manual_Document_ID", typeof(string));
        dtLedger.Columns.Add("DocumentTypeID", typeof(int));
        dtLedger.Columns.Add("SlipNo", typeof(string));
        dtLedger.Columns.Add("ChequeDate", typeof(DateTime));
        dtLedger.Columns.Add("PaymentMode", typeof(int));
        dtLedger.Columns.Add("PayeesName", typeof(string));
        this.Session.Add("dtLedger", dtLedger);
        return dtLedger;
    }

    /// <summary>
    /// Saves/Uupdates Bank Transaction
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (IsDayClosed())
        {
            UserController UserCtl = new UserController();

            UserCtl.InsertUserLogoutTime(Convert.ToInt32(Session["User_Log_ID"]), Convert.ToInt32(Session["UserID"]));
            this.Session.Clear();
            System.Web.Security.FormsAuthentication.SignOut();
            Response.Redirect("../Login.aspx");
        }
        else
        {

            if (btnSave.Text == "Update")
            {
                SaleForceController mSaleForce = new SaleForceController();
                mSaleForce.DeleteSaleForceCash(Convert.ToInt32(hfSALE_FORCE_CASH_ID.Value), Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToInt32(hfPRINCIPAL_ID.Value), Convert.ToInt32(hfDELIVERYMAN_ID.Value));
            }

            if (DrpAccountType.SelectedIndex == 0)// cash realization 
            {
                int InvoiceCount = Constants.IntNullValue;
                foreach (GridViewRow dr in GrdCredit.Rows)
                {
                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                    if (chRelized.Checked == true)
                    {
                        InvoiceCount++;
                        break;
                    }
                }

                if (InvoiceCount == Constants.IntNullValue)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Must Select Invoice');", true);
                    return;
                }
                if (Is_Advance())
                {
                    if (!chbIsAdvance.Checked)
                    {
                        chbIsAdvance.Visible = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Advance Amount Alert, Must Allow Advance Amount Entry !');", true);
                        return;
                    }
                }
                this.CashRealization();
                InsertGL();
                this.SelectCreditInvoice();
                //// by safdar add calculate W.H.Tax and insert in database
                //if (this.Session["CompanyName"].ToString().Length > 0)
                //{
                //    if (this.Session["CompanyName"].ToString() != "MFN")
                //    {
                //        this.IncomeTax2();
                //        this.SelectCreditInvoice();
                //    }
                //}
                //else
                //{
                //    this.IncomeTax2();
                //    this.SelectCreditInvoice();
                //}
                ////
            }
            else if (DrpAccountType.SelectedIndex == 9) // Online Transfer 
            {
                int InvoiceCount = Constants.IntNullValue;
                foreach (GridViewRow dr in GrdCredit.Rows)
                {
                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                    if (chRelized.Checked == true)
                    {
                        InvoiceCount++;
                        break;
                    }
                }

                if (InvoiceCount == Constants.IntNullValue)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Must Select Invoice');", true);
                    return;
                }
                if (Is_Advance())
                {
                    if (!chbIsAdvance.Checked)
                    {
                        chbIsAdvance.Visible = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Advance Amount Alert, Must Allow Advance Amount Entry !');", true);
                        return;
                    }
                }
                this.OnlineTransfer();
                InsertGL();
                this.SelectCreditInvoice();
                //// by safdar add calculate W.H.Tax and insert in database
                //this.IncomeTax2();
                //this.SelectCreditInvoice();
                ////
            }
            else if (DrpAccountType.SelectedIndex == 1)
            {
                this.CashAdvance();

            }
            else if (DrpAccountType.SelectedIndex == 2)
            {
                string t_ID = this.BankDeposit("");
                InsertGLBDB(t_ID);
            }
            else if (DrpAccountType.SelectedIndex == 6)
            {
                string t_ID = BankDeposit(DrpDeliveryMan.SelectedValue);
                InsertGLBDB(t_ID);
            }
            else if (DrpAccountType.SelectedIndex == 5)
            {
                this.AdvanceReturn();
            }
            else if (DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4)
            {
                this.IncomeTax();
                this.SelectCreditInvoice();

            }
            else if (DrpAccountType.SelectedIndex == 7)// cash from DM
            {
                int id = this.SaleForceCashReceived();
                this.InsertGLdm(id);
                this.LoadSaleFoceCash();


            }
            else if (DrpAccountType.SelectedIndex == 8)// Claim receivable
            {
                string id = this.ClaimReceivable();
                this.InsertGLClaimRecivable(id);
                this.SelectCreditInvoice();

            }

            else
            {
                this.PettyCash();
            }
            this.ClearAll();
            this.LoadGrid();
            this.SetTableSorter();
        }
    }

    /// <summary>
    /// Saves Sale Force Cash
    /// </summary>
    private int SaleForceCashReceived()
    {
        try
        {
            SaleForceController mSaleForce = new SaleForceController();
            Convert.ToDecimal(txtAmount.Text);
            int id = mSaleForce.InsertSaleForceCash2(Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToInt32(DrpPrincipal.SelectedValue), Convert.ToInt32(DrpDeliveryMan.SelectedValue), Convert.ToDateTime(this.Session["CurrentWorkDate"]), Convert.ToDecimal(txtAmount.Text), Convert.ToInt32(this.Session["UserId"]), txtSlipNo.Text, int.Parse(DrpAccountDetail.SelectedValue), txtRemarks.Text);
            return id;
        }
        catch (Exception)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Amount must be decimal')", true);
        }
        return -1;
    }

    /// <summary>
    /// Deletes Sale Force Cash
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void gvSaleForceCash_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        SaleForceController mSaleForce = new SaleForceController();

        if (mSaleForce.DeleteSaleForceCash(Convert.ToInt32(gvSaleForceCash.Rows[e.RowIndex].Cells[0].Text), Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToInt32(gvSaleForceCash.Rows[e.RowIndex].Cells[1].Text), Convert.ToInt32(gvSaleForceCash.Rows[e.RowIndex].Cells[3].Text)))
        {
            this.LoadSaleFoceCash();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Some error occured. Cash received not deleted.');", true);
        }
        this.SetTableSorter();
    }

    /// <summary>
    /// Sets Sale Force Cash For Edit. This Function Runs When An Existing Sale Force Cash Needs To Be Edited
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void gvSaleForceCash_RowEditing(object sender, GridViewEditEventArgs e)
    {
        DrpPrincipal.SelectedValue = gvSaleForceCash.Rows[e.NewEditIndex].Cells[1].Text;
        DrpDeliveryMan.SelectedValue = gvSaleForceCash.Rows[e.NewEditIndex].Cells[3].Text;
        txtAmount.Text = gvSaleForceCash.Rows[e.NewEditIndex].Cells[6].Text;

        hfSALE_FORCE_CASH_ID.Value = gvSaleForceCash.Rows[e.NewEditIndex].Cells[0].Text;
        hfPRINCIPAL_ID.Value = gvSaleForceCash.Rows[e.NewEditIndex].Cells[1].Text;
        hfDELIVERYMAN_ID.Value = gvSaleForceCash.Rows[e.NewEditIndex].Cells[3].Text;
        btnSave.Text = "Update";
        txtSlipNo.Text = gvSaleForceCash.Rows[e.NewEditIndex].Cells[7].Text;
        txtRemarks.Text = gvSaleForceCash.Rows[e.NewEditIndex].Cells[8].Text;
        DrpAccountDetail.SelectedValue = (gvSaleForceCash.Rows[e.NewEditIndex].Cells[9].Text);

        this.SetTableSorter();
    }

    /// <summary>
    /// Loads Sale Force Cash To Grid
    /// </summary>
    private void LoadSaleFoceCash()
    {
        SaleForceController mSaleForce = new SaleForceController();
        gvSaleForceCash.DataSource = null;
        gvSaleForceCash.DataBind();

        DataTable dt = mSaleForce.GetSaleForceCash(Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToInt32(DrpPrincipal.SelectedValue), Convert.ToInt32(DrpDeliveryMan.SelectedValue), Convert.ToDateTime(this.Session["CurrentWorkDate"]), Convert.ToDateTime(this.Session["CurrentWorkDate"]));
        gvSaleForceCash.DataSource = dt;
        gvSaleForceCash.DataBind();
        gvSaleForceCash.Visible = true;
        GrdOrder.Visible = false;
    }

    /// <summary>
    /// Loads Bank Transactions
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpDeliveryMan_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.DrpAccountType.SelectedIndex == 7)
        {
            this.LoadSaleFoceCash();
        }
        else
        {
            this.LoadData();
            this.SelectCreditInvoice();
            gvSaleForceCash.Visible = false;
            GrdOrder.Visible = true;
        }
    }

    /// <summary>
    /// Hides/Shows Controls
    /// </summary>
    /// <param name="Visible"></param>
    private void HideShowControls(bool Visible)
    {
        if (this.DrpAccountType.SelectedIndex == 2)
        {
            Label2.Enabled = Visible;
            DrpRoute.Enabled = Visible;
            Label4.Enabled = Visible;
            DrpCustomer.Enabled = Visible;
            Panel1.Enabled = Visible;
            Label7.Enabled = Visible;
            DrpAccountDetail.Enabled = true;
            Label10.Enabled = Visible;
            DrpDeliveryMan.Enabled = Visible;
            txtChequeNo.Enabled = true;
        }
        else
        {
            Label2.Enabled = Visible;
            DrpRoute.Enabled = Visible;
            Label4.Enabled = Visible;
            DrpCustomer.Enabled = Visible;
            Panel1.Enabled = Visible;
            Label7.Enabled = Visible;
            // DrpAccountDetail.Enabled = Visible;
            Label5.Enabled = Visible;
            Label3.Enabled = Visible;
            txtChequeNo.Enabled = Visible;
            // txtSlipNo.Enabled = Visible;
            Label9.Enabled = Visible;
            //  txtRemarks.Enabled = Visible;
            Label10.Enabled = true;
            DrpDeliveryMan.Enabled = true;
            LoadAccountHead();
        }
    }

    /// <summary>
    /// Sets Grids Columns For JQury Sorting
    /// </summary>
    private void SetTableSorter()
    {
        if (GrdOrder.Rows.Count > 1)
        {
            GrdOrder.UseAccessibleHeader = true;
            GrdOrder.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        if (gvSaleForceCash.Rows.Count > 1)
        {
            gvSaleForceCash.UseAccessibleHeader = true;
            gvSaleForceCash.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    private bool IsDayClosed()
    {
        bool flag = false;
        DistributorController DistrCtl = new DistributorController();
        DataTable dtDayClose = DistrCtl.MaxDayClose(Convert.ToInt32(drpDistributor.SelectedValue), 3);
        if (Convert.ToDateTime(Session["CurrentWorkDate"]) == Convert.ToDateTime(dtDayClose.Rows[0]["DayClose"]))
        {
            flag = false;
        }
        else
        {
            flag = true;
        }

        return flag;
    }
    
    private void InsertGLdm(int saleForeceId)
    {
        LedgerController LController = new LedgerController();
        SAMSCommon.Classes.Configuration.GetAccountHead();

        DataTable dtVoucher = new DataTable();
        dtVoucher.Columns.Add("LEDGER_ID", typeof(long));
        dtVoucher.Columns.Add("ACCOUNT_HEAD_ID", typeof(long));
        dtVoucher.Columns.Add("DEBIT", typeof(decimal));
        dtVoucher.Columns.Add("CREDIT", typeof(decimal));
        dtVoucher.Columns.Add("REMARKS", typeof(string));
        dtVoucher.Columns.Add("Principal_Id", typeof(string));

        DataRow dr = dtVoucher.NewRow();
        dr["LEDGER_ID"] = Constants.LongNullValue.ToString();
        dr["ACCOUNT_HEAD_ID"] = Convert.ToInt64(Constants.AC_CashINHAND);
        dr["REMARKS"] = DrpAccountDetail.SelectedItem.Text + " received from " + DrpCustomer.SelectedItem.Text;
        dr["DEBIT"] = decimal.Parse(txtAmount.Text);
        dr["CREDIT"] = 0;
        dr["Principal_Id"] = DrpPrincipal.SelectedValue;
        dtVoucher.Rows.Add(dr);
        ///Credit Side Entry
        DataRow dr1 = dtVoucher.NewRow();
        dr1["LEDGER_ID"] = Constants.LongNullValue.ToString();
        dr1["ACCOUNT_HEAD_ID"] = Convert.ToInt64(Constants.AC_NCS);//SAMSCommon.Classes.Configuration.MainCashSaled_Head;
        dr1["DEBIT"] = 0;
        dr1["CREDIT"] = decimal.Parse(txtAmount.Text);
        dr1["Principal_Id"] = DrpPrincipal.SelectedValue;
        dr1["REMARKS"] = DrpAccountDetail.SelectedItem.Text + " received from " + DrpCustomer.SelectedItem.Text;
        dtVoucher.Rows.Add(dr1);


        string MaxDocumentId = LController.SelectMaxVoucherId(Constants.Cash_Voucher, Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToDateTime(Session["CurrentWorkDate"]));

        //Using Payee's Name for reference between Ledger and GL

        LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToInt32(DrpPrincipal.SelectedValue), MaxDocumentId, Constants.Cash_Voucher, Convert.ToDateTime(Session["CurrentWorkDate"]), Constants.Cash_Relization, saleForeceId.ToString(), "Default " + DrpAccountDetail.SelectedItem.Text + " Voucher", Convert.ToDateTime(Session["CurrentWorkDate"]), txtSlipNo.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), txtSlipNo.Text, Constants.DateNullValue);// 
    }
    private void InsertGLClaimRecivable(string saleForeceId)
    {
        LedgerController LController = new LedgerController();
        SAMSCommon.Classes.Configuration.GetAccountHead();

        DataTable dtVoucher = new DataTable();
        dtVoucher.Columns.Add("LEDGER_ID", typeof(long));
        dtVoucher.Columns.Add("ACCOUNT_HEAD_ID", typeof(long));
        dtVoucher.Columns.Add("DEBIT", typeof(decimal));
        dtVoucher.Columns.Add("CREDIT", typeof(decimal));
        dtVoucher.Columns.Add("REMARKS", typeof(string));
        dtVoucher.Columns.Add("Principal_Id", typeof(string));

        DataRow dr = dtVoucher.NewRow();
        dr["LEDGER_ID"] = Constants.LongNullValue.ToString();
        dr["ACCOUNT_HEAD_ID"] = Convert.ToInt64(DrpAccountDetail.SelectedValue);
        dr["REMARKS"] = DrpAccountDetail.SelectedItem.Text + " received from " + DrpCustomer.SelectedItem.Text;
        dr["DEBIT"] = decimal.Parse(txtAmount.Text);
        dr["CREDIT"] = 0;
        dr["Principal_Id"] = DrpPrincipal.SelectedValue;
        dtVoucher.Rows.Add(dr);
        ///Credit Side Entry
        DataRow dr1 = dtVoucher.NewRow();
        dr1["LEDGER_ID"] = Constants.LongNullValue.ToString();
        dr1["ACCOUNT_HEAD_ID"] = SAMSCommon.Classes.Configuration.AccountReceivable;
        dr1["DEBIT"] = 0;
        dr1["CREDIT"] = decimal.Parse(txtAmount.Text);
        dr1["Principal_Id"] = DrpPrincipal.SelectedValue;
        dr1["REMARKS"] = DrpAccountDetail.SelectedItem.Text + " received from " + DrpCustomer.SelectedItem.Text;
        dtVoucher.Rows.Add(dr1);


        string MaxDocumentId = LController.SelectMaxVoucherId(Constants.Journal_Voucher, Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToDateTime(Session["CurrentWorkDate"]));

        //Using Payee's Name for reference between Ledger and GL

        LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToInt32(DrpPrincipal.SelectedValue), MaxDocumentId, Constants.Journal_Voucher, Convert.ToDateTime(Session["CurrentWorkDate"]), Constants.Cash_Relization, saleForeceId.ToString(), "Default " + DrpAccountDetail.SelectedItem.Text + " Voucher", Convert.ToDateTime(Session["CurrentWorkDate"]), txtSlipNo.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), txtSlipNo.Text, Constants.DateNullValue);// 
    }
    private string ClaimReceivable()
    {
        LedgerController LController = new LedgerController();
        string MaxDocumentID = LController.SelectLedgerMaxDocumentId(Constants.Journal_Voucher, int.Parse(drpDistributor.SelectedValue.ToString()));
        decimal OfferAmount = decimal.Parse(txtAmount.Text);
        foreach (GridViewRow dr in GrdCredit.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {

                DataTable dtLedger = CreateTable();

                //Credit from Account Receivable (Party Wise)
                DataRow drLedger = dtLedger.NewRow();
                drLedger["VOUCHER_TYPE_ID"] = Constants.Journal_Voucher;
                drLedger["VOUCHER_NO"] = MaxDocumentID;
                drLedger["ACCOUNT_HEAD_ID"] = SAMSCommon.Classes.Configuration.AccountReceivable;
                drLedger["Distributor_ID"] = drpDistributor.SelectedValue;
                if (decimal.Parse(dr.Cells[3].Text) >= OfferAmount)
                {
                    drLedger["DEBIT"] = 0;
                    drLedger["CREDIT"] = OfferAmount;
                }
                else if (decimal.Parse(dr.Cells[3].Text) <= OfferAmount)
                {
                    drLedger["DEBIT"] = 0;
                    drLedger["CREDIT"] = dr.Cells[3].Text;
                }
                drLedger["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
                drLedger["Remarks"] = txtRemarks.Text;
                drLedger["TimeStamp"] = DateTime.Now;
                drLedger["Customer_ID"] = DrpCustomer.SelectedValue;
                drLedger["Principal_ID"] = DrpPrincipal.SelectedValue;
                drLedger["Cheque_NO"] = txtChequeNo.Text;
                drLedger["UserID"] = this.Session["UserId"].ToString();
                drLedger["Document_ID"] = GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"].ToString();
                drLedger["Manual_Document_ID"] = dr.Cells[1].Text;
                drLedger["DocumentTypeID"] = Constants.Document_Invoice;
                drLedger["SlipNo"] = txtSlipNo.Text;
                drLedger["ChequeDate"] = Constants.DateNullValue;
                drLedger["PaymentMode"] = DrpAccountType.SelectedValue;
                drLedger["PayeesName"] = DrpDeliveryMan.SelectedValue;
                dtLedger.Rows.Add(drLedger);

                //Debit to selected Account Head
                DataRow drLedger2 = dtLedger.NewRow();
                drLedger2["VOUCHER_TYPE_ID"] = Constants.Journal_Voucher;
                drLedger2["VOUCHER_NO"] = MaxDocumentID;
                drLedger2["ACCOUNT_HEAD_ID"] = DrpAccountDetail.SelectedValue;
                drLedger2["Distributor_ID"] = drpDistributor.SelectedValue;
                if (decimal.Parse(dr.Cells[3].Text) >= OfferAmount)
                {
                    drLedger2["DEBIT"] = OfferAmount;
                    drLedger2["CREDIT"] = 0;
                }
                else if (decimal.Parse(dr.Cells[3].Text) <= OfferAmount)
                {
                    drLedger2["DEBIT"] = dr.Cells[3].Text;
                    drLedger2["CREDIT"] = 0;
                }

                drLedger2["Ledger_Date"] = this.Session["CurrentWorkDate"].ToString();
                drLedger2["Remarks"] = txtRemarks.Text;
                drLedger2["TimeStamp"] = DateTime.Now;
                drLedger2["Customer_ID"] = DrpCustomer.SelectedValue;
                drLedger2["Principal_ID"] = DrpPrincipal.SelectedValue;
                drLedger2["Cheque_NO"] = txtChequeNo.Text;
                drLedger2["UserID"] = this.Session["UserId"].ToString();
                drLedger2["Document_ID"] = GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"].ToString();
                drLedger2["Manual_Document_ID"] = dr.Cells[1].Text;
                drLedger2["DocumentTypeID"] = Constants.Document_Invoice;
                drLedger2["SlipNo"] = txtSlipNo.Text;
                drLedger2["ChequeDate"] = Constants.DateNullValue;
                drLedger2["PaymentMode"] = DrpAccountType.SelectedValue;
                drLedger2["PayeesName"] = DrpDeliveryMan.SelectedValue;
                dtLedger.Rows.Add(drLedger2);

                if (decimal.Parse(dr.Cells[3].Text) >= OfferAmount)
                {
                    OfferAmount = decimal.Parse(dr.Cells[3].Text) - OfferAmount;
                    LController.PostingCash_Bank_Account(dtLedger, Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), int.Parse(drpDistributor.SelectedValue.ToString()), OfferAmount);
                }
                else if (decimal.Parse(dr.Cells[3].Text) <= OfferAmount)
                {
                    OfferAmount = OfferAmount - decimal.Parse(dr.Cells[3].Text);
                    LController.PostingCash_Bank_Account(dtLedger, Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), int.Parse(drpDistributor.SelectedValue.ToString()), 0);
                }
                break;
            }
        }
        return MaxDocumentID;
    }
    private void InsertGLBDB(string saleForeceId)
    {
        LedgerController LController = new LedgerController();
        SAMSCommon.Classes.Configuration.GetAccountHead();

        DataTable dtVoucher = new DataTable();
        dtVoucher.Columns.Add("LEDGER_ID", typeof(long));
        dtVoucher.Columns.Add("ACCOUNT_HEAD_ID", typeof(long));
        dtVoucher.Columns.Add("DEBIT", typeof(decimal));
        dtVoucher.Columns.Add("CREDIT", typeof(decimal));
        dtVoucher.Columns.Add("REMARKS", typeof(string));
        dtVoucher.Columns.Add("Principal_Id", typeof(string));

        DataRow dr = dtVoucher.NewRow();
        dr["LEDGER_ID"] = Constants.LongNullValue.ToString();
        dr["ACCOUNT_HEAD_ID"] = Convert.ToInt64(DrpAccountDetail.SelectedValue);
        dr["REMARKS"] = DrpAccountDetail.SelectedItem.Text + " received from " + DrpCustomer.SelectedItem.Text;
        dr["DEBIT"] = decimal.Parse(txtAmount.Text);
        dr["CREDIT"] = 0;
        dr["Principal_Id"] = DrpPrincipal.SelectedValue;
        dtVoucher.Rows.Add(dr);
        ///Credit Side Entry
        DataRow dr1 = dtVoucher.NewRow();
        dr1["LEDGER_ID"] = Constants.LongNullValue.ToString();
        if (DrpAccountType.SelectedValue == "222")
        {
            dr1["ACCOUNT_HEAD_ID"] = Convert.ToInt64(Constants.AC_NCS);
        }
        else
        {
            dr1["ACCOUNT_HEAD_ID"] = Convert.ToInt64(Constants.AC_CashINHAND);
        }
        dr1["DEBIT"] = 0;
        dr1["CREDIT"] = decimal.Parse(txtAmount.Text);
        dr1["Principal_Id"] = DrpPrincipal.SelectedValue;
        dr1["REMARKS"] = DrpAccountDetail.SelectedItem.Text + " received from " + DrpCustomer.SelectedItem.Text;
        dtVoucher.Rows.Add(dr1);


        string MaxDocumentId = LController.SelectMaxVoucherId(Constants.Bank_Voucher, Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToDateTime(Session["CurrentWorkDate"]));

        //Using Payee's Name for reference between Ledger and GL

        LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToInt32(DrpPrincipal.SelectedValue), MaxDocumentId, Constants.Bank_Voucher, Convert.ToDateTime(Session["CurrentWorkDate"]), 22, saleForeceId, "Default " + DrpAccountDetail.SelectedItem.Text + " Voucher", Convert.ToDateTime(Session["CurrentWorkDate"]), txtSlipNo.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), txtSlipNo.Text, Constants.DateNullValue);// 
    }
    private void InsertGL()
    {
        DataTable dtVoucher = new DataTable();
        dtVoucher.Columns.Add("LEDGER_ID", typeof(long));
        dtVoucher.Columns.Add("ACCOUNT_HEAD_ID", typeof(long));
        dtVoucher.Columns.Add("DEBIT", typeof(decimal));
        dtVoucher.Columns.Add("CREDIT", typeof(decimal));
        dtVoucher.Columns.Add("REMARKS", typeof(string));
        dtVoucher.Columns.Add("Principal_Id", typeof(string));

        ///Debit Side Entry
        DataRow dr = dtVoucher.NewRow();
        dr["LEDGER_ID"] = Constants.LongNullValue.ToString();
        dr["ACCOUNT_HEAD_ID"] = Convert.ToInt64(DrpAccountDetail.SelectedValue);
        dr["REMARKS"] = DrpAccountType.SelectedItem.Text + " received from " + DrpCustomer.SelectedItem.Text;
        dr["DEBIT"] = decimal.Parse(txtAmount.Text);
        dr["CREDIT"] = 0;
        dr["Principal_Id"] = 0;
        dtVoucher.Rows.Add(dr);

        ///Credit Side Entry
        DataRow dr1 = dtVoucher.NewRow();
        dr1["LEDGER_ID"] = Constants.LongNullValue.ToString();
        dr1["DEBIT"] = 0;
        dr1["CREDIT"] = decimal.Parse(txtAmount.Text);
        dr1["Principal_Id"] = 0;
        dr1["ACCOUNT_HEAD_ID"] = 107; // 3002020001 Account Receivable (Party Wise)
        dr1["REMARKS"] = DrpAccountType.SelectedItem.Text + " received from " + DrpCustomer.SelectedItem.Text;
        dtVoucher.Rows.Add(dr1);

        LedgerController LController = new LedgerController();
        string MaxDocumentId = "";
        string Remarks = "";
        string remarks = "";
        if (!txtRemarks.Text.Equals(""))
        {
            remarks = " / " + txtRemarks.Text;
        }
        Remarks = "Default " + DrpAccountDetail.SelectedItem.Text + " Voucher" + " / " + DrpCustomer.SelectedItem.Text + remarks;
        if (DrpAccountType.SelectedValue == "19" || DrpAccountType.SelectedValue == "21")// Cash Realization
        {
            MaxDocumentId = LController.SelectMaxVoucherId(Constants.Cash_Voucher, Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToDateTime(Session["CurrentWorkDate"]));
            LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToInt32(DrpPrincipal.SelectedValue), MaxDocumentId, Constants.Cash_Voucher, Convert.ToDateTime(Session["CurrentWorkDate"]), Constants.Cash_Relization, Session["VoucherNo"].ToString(), Remarks, Convert.ToDateTime(Session["CurrentWorkDate"]), txtChequeNo.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), txtSlipNo.Text, Constants.DateNullValue);
            PrintReport(MaxDocumentId);
        }
        else
        {
            MaxDocumentId = LController.SelectMaxVoucherId(Constants.Bank_Voucher, Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToDateTime(Session["CurrentWorkDate"]));
            LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToInt32(DrpPrincipal.SelectedValue), MaxDocumentId, Constants.Bank_Voucher, Convert.ToDateTime(Session["CurrentWorkDate"]), Constants.Bank_Deposit, Session["VoucherNo"].ToString(), Remarks, Convert.ToDateTime(Session["CurrentWorkDate"]), txtChequeNo.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), txtSlipNo.Text, Constants.DateNullValue);
            PrintReport(MaxDocumentId);
        }

    }

    private void PrintReport(string p_Voucher_No)
    {
        DataTable DisDt = (DataTable)this.Session["DisDt"];
        if (DisDt != null)
        {
            if (DisDt.Rows.Count > 0)
            {
                DataRow[] foundRows = DisDt.Select("DISTRIBUTOR_ID  = '" + drpDistributor.SelectedValue + "'");
                if (foundRows.Length > 0)
                {
                    if (foundRows[0]["SUBZONE_ID"].ToString() == "5")// Head Office 
                    {
                        #region print report
                        DocumentPrintController DPrint = new DocumentPrintController();
                        RptAccountController RptAccountCtl = new RptAccountController();
                        SAMSBusinessLayer.Reports.crpVoucherView CrpReport = new SAMSBusinessLayer.Reports.crpVoucherView();

                        DataSet ds = null;
                        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));
                        ds = RptAccountCtl.SelectUnpostVoucherForPrint(int.Parse(drpDistributor.SelectedValue.ToString()), p_Voucher_No, Constants.Cash_Voucher);
                        CrpReport.SetDataSource(ds);
                        CrpReport.Refresh();
                        string VoucherType2 = "Cash Receipt Voucher";

                        CrpReport.SetParameterValue("PRINT_TITLE_C", Convert.ToBoolean(dt.Rows[0]["PRINT_TITLE_C"].ToString()));
                        CrpReport.SetParameterValue("Company_Name", dt.Rows[0]["COMPANY_NAME"].ToString());
                        CrpReport.SetParameterValue("DISTRIBUTOR_NAME", dt.Rows[0]["DISTRIBUTOR_NAME"].ToString());
                        CrpReport.SetParameterValue("Voucher_Type", VoucherType2);
                        CrpReport.SetParameterValue("USERNAME", Session["UserName2"].ToString());
                        this.Session.Add("CrpReport", CrpReport);
                        this.Session.Add("ReportType", 0);
                        string url = "'Default.aspx'";
                        string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                        Type cstype = this.GetType();
                        ClientScriptManager cs = Page.ClientScript;
                        cs.RegisterStartupScript(cstype, "OpenWindow", script);
                        #endregion

                    }

                }
            }
        }
    }


}