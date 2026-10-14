using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using SAMSBusinessLayer.Reports;
using System.Linq;

/// <summary>
/// Form To Add, Edit Vochers
/// </summary>
public partial class Forms_frmVoucherEntry : System.Web.UI.Page
{
    DataTable dtVoucher;
    private static int RowId = -1;

    /// <summary>
    /// Page_Load Function Populates All Combos And Grid On The Page
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    bool ViewAccess;
    protected void Page_Load(object sender, EventArgs e)
    {
        ViewAccess = ValidateScreen();
        if (ViewAccess == false)
        {
            Response.Redirect("../Forms/AccessDenied.aspx");
        }
        if (!Page.IsPostBack)
        {
                this.LoadDistributor();
                this.LoadAccountDetail();
                this.LoadPrincipal();
                this.CreatTable();
                this.LoadAccountHead();
                this.LoadSubVoucher();
                DateTime pOrderDate = DateTime.Parse(this.Session["CurrentWorkDate"].ToString());
                txtVoucherDate.Text = pOrderDate.ToString("dd/MM/yyyy");
                btnSave.Attributes.Add("onclick", "return ValidateForm();");
                RowId = -1;
                Session["MANUAL_VOUCHER"] = "0";
        }
    }
    public bool ValidateScreen()
    {
        RoleManagementController mController = new RoleManagementController();
        int RoleID = (int)Session["RoleID"];
        DataTable dt = mController.GetAssignUnAssignModule(RoleID, Constants.IntNullValue, Constants.IntNullValue, 2);
        var PageURL = this.Page.ToString().Substring(4, this.Page.ToString().Substring(4).Length - 5) + ".aspx";
        string M_Key = Convert.ToString(PageURL.Replace("_", "/"));
        string[] reqArray = M_Key.Split('/');
        M_Key = reqArray[reqArray.Count() - 1];
        DataRow[] foundRows = dt.Select("MODULE_KEY  = '" + M_Key.Trim() + "'");
        if (foundRows.Length > 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Loads Account Heads To Bank Combo
    /// </summary>
    private void LoadAccountHead()
    {
        SAMSCommon.Classes.Configuration.GetAccountHead();
        if (int.Parse(DrpVoucherType.SelectedValue.ToString()) == 14  )
        {
            AccountHeadController mAccountController = new AccountHeadController();
            DataTable dt = mAccountController.SelectAccountHead(Constants.AC_AccountHeadId, long.Parse(SAMSCommon.Classes.Configuration.CashDefaultType));
            clsWebFormUtil.FillDropDownList(drpBanks, dt, 0, 4, true);
        }
        else if (int.Parse(DrpVoucherType.SelectedValue.ToString()) == 15)    
        {
            AccountHeadController mAccountController = new AccountHeadController();
            DataTable dt = mAccountController.SelectAccountHead(Constants.AC_AccountHeadId, long.Parse(SAMSCommon.Classes.Configuration.BankDefaultType));
            clsWebFormUtil.FillDropDownList(drpBanks, dt, 0, 4, true);
            //drpBanks.SelectedValue = "599";
        }
        else
        {
            drpBanks.Items.Clear();
            drpBanks.Items.Add(new ListItem("N/A", Constants.IntNullValue.ToString()));
        }
    }



    private void LoadSubVoucher()
    {
        if (int.Parse(DrpVoucherType.SelectedValue.ToString()) == 14)// Cash Voucher
        {
            DrpSubVoucherType.Items.Clear();
            DrpSubVoucherType.Items.Add(new ListItem("Cash Receipt Voucher", 14.ToString()));
            DrpSubVoucherType.Items.Add(new ListItem("Cash Payment Voucher", 24.ToString()));
        }
        else if (int.Parse(DrpVoucherType.SelectedValue.ToString()) == 15)//Bank Voucher
        {
            DrpSubVoucherType.Items.Clear();
            DrpSubVoucherType.Items.Add(new ListItem("Bank Receipt Voucher", 15.ToString()));
            DrpSubVoucherType.Items.Add(new ListItem("Bank Payment Voucher", 17.ToString()));
        }
        else
        {
            DrpSubVoucherType.Items.Clear();
            DrpSubVoucherType.Items.Add(new ListItem("N/A", Constants.IntNullValue.ToString()));
        }
    }

    /// <summary>
    /// Creates Datatable To Hold Voucher Data
    /// </summary>
    private void CreatTable()
    {
        dtVoucher = new DataTable();
        dtVoucher.Columns.Add("LEDGER_ID", typeof(long));
        dtVoucher.Columns.Add("Account_Head_Id", typeof(long));
        dtVoucher.Columns.Add("Account_Code", typeof(string));
        dtVoucher.Columns.Add("Account_Name", typeof(string));
        dtVoucher.Columns.Add("Debit", typeof(decimal));
        dtVoucher.Columns.Add("Credit", typeof(decimal));
        dtVoucher.Columns.Add("Remarks", typeof(string));
        dtVoucher.Columns.Add("InvoiceNo", typeof(string));
        dtVoucher.Columns.Add("Principal_Id", typeof(string));
        dtVoucher.Columns.Add("Principal", typeof(string));
        this.Session.Add("dtVoucher", dtVoucher);
    }
    
    /// <summary>
    /// Loads Principals To Principal Combo
    /// </summary>
    private void LoadPrincipal()
    {
        SKUPriceDetailController PController = new SKUPriceDetailController();
        DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        this.DrpPrincipal.Items.Add(new ListItem("General Entry","0"));
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, m_dt, 0, 1);
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
    /// Loads Account Heads To ListBox
    /// </summary>
    private void LoadAccountDetail()
    {
        //Load Voucher Type by User Rights 

        LedgerController mLedgerController = new LedgerController();
        DataTable dt = mLedgerController.SelectVoucherType(int.Parse(this.Session["UserId"].ToString()));
        clsWebFormUtil.FillDropDownList(this.DrpVoucherType, dt, 0, 1, true);

        // Load Account Head 
        AccountHeadController mAccountController = new AccountHeadController();
        DataTable dtHead = mAccountController.SelectAccountHead(Constants.AC_AccountHeadId, Constants.LongNullValue);
       // clsWebFormUtil.FillListBox(this.LstAccountHead, dtHead, "ACCOUNT_HEAD", "ACCOUNT_HEAD", true);
       clsWebFormUtil.FillDropDownList(DrpAccountCode, dtHead, "Account_Head_Id", "ACCOUNT_HEAD", true);
        this.Session.Add("dtHead", dtHead);

    }
    
    /// <summary>
    /// Resets Form Controls
    /// </summary>
    private void ClearAll()
    {
        txtTotalCredit.Text = "0";
        txtTotalDebit.Text = "0";
       // txtAccountCode.Text = "";
      //  txtAccountName.Text = "";
        txtRemarks.Text = "";
        txtChequeDate.Text = "";
        txtChequeNo.Text = "";
        txtDebitAmount.Text = "";
        txtCreditAmount.Text = "";
        txtAccountDes.Text = "";
        GrdOrder.DataSource = null;
        GrdOrder.DataBind();
        this.Session.Remove("dtVoucher");
        this.CreatTable();
        txtpayeesName.Text = "";
        drpDistributor.Enabled = true;
        DrpVoucherType.Enabled = true;
        DrpSubVoucherType.Enabled = true;
    }
    
    /// <summary>
    /// Shows Vouchers in Crystal Report For Print Purpose
    /// </summary>
    /// <param name="VoucherNo">Voucher</param>
    private void PrintVoucher(string VoucherNo)
    {
        DocumentPrintController DPrint = new DocumentPrintController();
        RptAccountController RptAccountCtl = new RptAccountController();
        SAMSBusinessLayer.Reports.CrpVoucherViewNew CrpReport = new SAMSBusinessLayer.Reports.CrpVoucherViewNew();
        int VoucherType = Constants.IntNullValue;
        string VoucherType2 = "";
        if (DrpVoucherType.SelectedValue == "16")
        {
            VoucherType = Convert.ToInt32(DrpVoucherType.SelectedValue);
            VoucherType2 = DrpVoucherType.SelectedItem.Text;
        }
        else
        {
            VoucherType = Convert.ToInt32(DrpSubVoucherType.SelectedValue);
            VoucherType2 = DrpSubVoucherType.SelectedItem.Text;
        }
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));
        ds = RptAccountCtl.SelectUnpostVoucherForPrint(int.Parse(drpDistributor.SelectedValue.ToString()), VoucherNo, VoucherType);
        CrpReport.SetDataSource(ds);
        CrpReport.Refresh();
        CrpReport.SetParameterValue("reportType", "Un Post Voucher");
        CrpReport.SetParameterValue("Company_Name", dt.Rows[0]["COMPANY_NAME"].ToString());
        CrpReport.SetParameterValue("DISTRIBUTOR_NAME", dt.Rows[0]["DISTRIBUTOR_NAME"].ToString());
        CrpReport.SetParameterValue("VOUCHER_TYPE", VoucherType2);
        CrpReport.SetParameterValue("USER_NAME", Session["UserName2"].ToString());
        this.Session.Add("CrpReport", CrpReport);
        this.Session.Add("ReportType", 0);
        string url = "'Default.aspx'";
        string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        Type cstype = this.GetType();
        ClientScriptManager cs = Page.ClientScript;
        cs.RegisterStartupScript(cstype, "OpenWindow", script);
    }
    
    /// <summary>
    /// Loads Account Heads To ListBox
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpVoucherType_SelectedIndexChanged(object sender, EventArgs e)
    {
        Session["MANUAL_VOUCHER"] = null;
        if (DrpVoucherType.SelectedIndex == 2)
        {
            lblChequeNo.Text = "Invoice No";
            lblChequedate.Text = "Invoice Date";
            DrpPaymentMode.SelectedItem.Text = "Cash";
            DrpPaymentMode.Enabled = false;
            Session["MANUAL_VOUCHER"] = "0";
            txtDebitAmount.Enabled = true;
            txtCreditAmount.Enabled = true;
        }
        else
        {
            lblChequeNo.Text = "Cheque No";
            lblChequedate.Text = "Cheque Date";
            DrpPaymentMode.Enabled = true;
            Session["MANUAL_VOUCHER"] = "0";
            txtDebitAmount.Enabled = false;
           
        }
        this.ClearAll();
        this.LoadAccountHead();
        this.LoadSubVoucher();
        if (DrpVoucherType.SelectedIndex != 2)
        {
            DrpSubVoucherType_SelectedIndexChanged(null, null);
        }
    }
    protected void DrpSubVoucherType_SelectedIndexChanged(object sender, EventArgs e)
    {
        DrpPaymentMode.Items.Clear();
        if (DrpSubVoucherType.SelectedIndex == 0)
        {
            if (DrpVoucherType.SelectedValue == "14")
            {
                DrpPaymentMode.Items.Add(new ListItem("Cash", "19"));
                DrpPaymentMode.Items.Add(new ListItem("Online Transfer", "30"));
            }
            else if (DrpVoucherType.SelectedValue == "15")
            {
                DrpPaymentMode.Items.Add(new ListItem("Cheque", "18"));
                DrpPaymentMode.Items.Add(new ListItem("Online Transfer", "30"));
            }

            txtDebitAmount.Enabled = false;
            txtCreditAmount.Enabled = true;
        }
        else if (DrpSubVoucherType.SelectedIndex == 1)
        {
            if (DrpVoucherType.SelectedValue == "14")
            {
                DrpPaymentMode.Items.Add(new ListItem("Cash", "19"));
                DrpPaymentMode.Items.Add(new ListItem("Online Transfer", "30"));
            }
            else if (DrpVoucherType.SelectedValue == "15")
            {
                DrpPaymentMode.Items.Add(new ListItem("Cheque", "18"));
                DrpPaymentMode.Items.Add(new ListItem("Pay Order", "21"));
            }

            txtDebitAmount.Enabled = true;
            txtCreditAmount.Enabled = false;
        }
    }

    /// <summary>
    /// Adds Voucher Data To Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        decimal TotalDebit = 0;
        decimal TotalCredit = 0;

        DataControl dc = new DataControl();
        DataTable dtHead = (DataTable)this.Session["dtHead"];
        dtVoucher = (DataTable)this.Session["dtVoucher"];
        DataRow[] foundRows = dtHead.Select("Account_Head_Id  = '" + DrpAccountCode.SelectedValue.ToString() + "'");
        if (foundRows.Length > 0)
        {
            if (RowId >= 0)
            {
                DataRow dr = dtVoucher.Rows[RowId];
                dr["ACCOUNT_HEAD_ID"] = foundRows[0]["ACCOUNT_HEAD_ID"];
                dr["ACCOUNT_CODE"] = foundRows[0]["ACCOUNT_CODE"];
                dr["ACCOUNT_NAME"] = foundRows[0]["ACCOUNT_NAME"];
                dr["DEBIT"] = decimal.Parse(dc.chkNull_0(txtDebitAmount.Text));
                dr["CREDIT"] = decimal.Parse(dc.chkNull_0(txtCreditAmount.Text));
                dr["Principal"] = DrpPrincipal.SelectedItem.Text;
                dr["Principal_id"] = DrpPrincipal.SelectedItem.Value;
                dr["REMARKS"] = txtAccountDes.Text;

            }
            else
            {
                DataRow dr = dtVoucher.NewRow();
                dr["Ledger_ID"] = Constants.LongNullValue.ToString();
                dr["ACCOUNT_HEAD_ID"] = foundRows[0]["ACCOUNT_HEAD_ID"];
                dr["ACCOUNT_CODE"] = foundRows[0]["ACCOUNT_CODE"];
                dr["ACCOUNT_NAME"] = foundRows[0]["ACCOUNT_NAME"];
                dr["DEBIT"] = decimal.Parse(dc.chkNull_0(txtDebitAmount.Text));
                dr["CREDIT"] = decimal.Parse(dc.chkNull_0(txtCreditAmount.Text));
                dr["REMARKS"] = txtAccountDes.Text;
                dr["Principal"] = DrpPrincipal.SelectedItem.Text;
                dr["Principal_id"] = DrpPrincipal.SelectedItem.Value;
                dtVoucher.Rows.Add(dr);
            }
        }

        #region Set Total

        foreach (DataRow dr in dtVoucher.Rows)
        {
            TotalDebit += decimal.Parse(dr["DEBIT"].ToString());
            TotalCredit += decimal.Parse(dr["CREDIT"].ToString());
        }
        txtTotalDebit.Text = decimal.Round(TotalDebit, 2).ToString();
        txtTotalCredit.Text = decimal.Round(TotalCredit, 2).ToString();

        #endregion

        #region Clear Txtbox

        //txtAccountCode.Text = "";
       // txtAccountName.Text = "";
        txtDebitAmount.Text = "";
        txtCreditAmount.Text = "";
        RowId = -1;

        #endregion

        GrdOrder.DataSource = dtVoucher;
        GrdOrder.DataBind();
        this.Session.Add("dtVoucher", dtVoucher);
        ScriptManager.GetCurrent(Page).SetFocus(DrpAccountCode);        
    }

    /// <summary>
    /// Deletes Voucher
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void GrdOrder_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        decimal TotalDebit = 0;
        decimal TotalCredit = 0;
        dtVoucher = (DataTable)this.Session["dtVoucher"];
        dtVoucher.Rows.RemoveAt(e.RowIndex);

        #region Set Total

        foreach (DataRow dr in dtVoucher.Rows)
        {
            TotalDebit += decimal.Parse(dr["DEBIT"].ToString());
            TotalCredit += decimal.Parse(dr["CREDIT"].ToString());
        }
        txtTotalDebit.Text = TotalDebit.ToString();
        txtTotalCredit.Text = TotalCredit.ToString();

        #endregion

        GrdOrder.DataSource = dtVoucher;
        GrdOrder.DataBind();
        RowId = -1;
    }

    /// <summary>
    /// Sets Voucher For Edit. This Function Runs When An Existing Voucher Needs To Be Edited
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void GrdOrder_RowEditing(object sender, GridViewEditEventArgs e)
    {
        RowId = e.NewEditIndex;
        //txtAccountCode.Text = GrdOrder.Rows[e.NewEditIndex].Cells[1].Text;
        DrpAccountCode.SelectedValue = GrdOrder.Rows[e.NewEditIndex].Cells[0].Text;
        // txtAccountName.Text = GrdOrder.Rows[e.NewEditIndex].Cells[2].Text;
        txtAccountDes.Text = GrdOrder.Rows[e.NewEditIndex].Cells[3].Text;
        DrpPrincipal.SelectedValue = GrdOrder.Rows[e.NewEditIndex].Cells[7].Text;
        txtDebitAmount.Text = GrdOrder.Rows[e.NewEditIndex].Cells[5].Text;
        txtCreditAmount.Text = GrdOrder.Rows[e.NewEditIndex].Cells[6].Text;        
    }

    /// <summary>
    /// Saves Voucher
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnDone_Click(object sender, EventArgs e)
    {
        LedgerController mLController = new LedgerController();
        DataControl dc = new DataControl();
        dtVoucher = (DataTable)this.Session["dtVoucher"];
        DataTable dtHead = (DataTable)this.Session["dtHead"];

            decimal TotalDebit = 0;
            decimal TotalCredit = 0;

            #region Set Total

            foreach (DataRow dr in dtVoucher.Rows)
            {
                TotalDebit += decimal.Parse(dr["DEBIT"].ToString());
                TotalCredit += decimal.Parse(dr["CREDIT"].ToString());
            }
            #endregion

            #region In Case of Bank & Cash Voucher

            if (drpBanks.SelectedItem.Text != "N/A")
            {
                decimal differnceAmt = TotalDebit - TotalCredit;

                if (differnceAmt > 0)
                {
                    DataRow dr1 = dtVoucher.NewRow();
                    DataRow[] foundSubRows = dtHead.Select("ACCOUNT_HEAD_ID  = " + drpBanks.SelectedValue.ToString());
                    dr1["Ledger_ID"] = Constants.LongNullValue.ToString();
                    dr1["ACCOUNT_HEAD_ID"] = foundSubRows[0]["ACCOUNT_HEAD_ID"];
                    dr1["ACCOUNT_CODE"] = foundSubRows[0]["ACCOUNT_CODE"];
                    dr1["ACCOUNT_NAME"] = foundSubRows[0]["ACCOUNT_NAME"];
                    dr1["REMARKS"] = txtAccountDes.Text;
                    dr1["CREDIT"] = differnceAmt;
                    dr1["DEBIT"] = 0;
                    dr1["Principal"] = "";
                    dr1["Principal_id"] = dtVoucher.Rows[0]["Principal_id"].ToString();
                    dtVoucher.Rows.Add(dr1);

                }
                else if (differnceAmt < 0)
                {
                    DataRow dr1 = dtVoucher.NewRow();
                    DataRow[] foundSubRows = dtHead.Select("ACCOUNT_HEAD_ID  = " + drpBanks.SelectedValue.ToString());
                    dr1["Ledger_ID"] = Constants.LongNullValue.ToString();
                    dr1["ACCOUNT_HEAD_ID"] = foundSubRows[0]["ACCOUNT_HEAD_ID"];
                    dr1["ACCOUNT_CODE"] = foundSubRows[0]["ACCOUNT_CODE"];
                    dr1["ACCOUNT_NAME"] = foundSubRows[0]["ACCOUNT_NAME"];
                    dr1["REMARKS"] = txtAccountDes.Text;
                    dr1["CREDIT"] = 0;
                    dr1["DEBIT"] = -(differnceAmt);
                    dr1["Principal"] = "";
                    dr1["Principal_id"] = dtVoucher.Rows[0]["Principal_id"].ToString();
                    dtVoucher.Rows.Add(dr1);
                }                                                   
            }
        #endregion

        int VoucherType = Constants.IntNullValue;

        if (DrpVoucherType.SelectedValue == "16")
        {
            VoucherType = Convert.ToInt32(DrpVoucherType.SelectedValue);
        }
        else
        {
            VoucherType = Convert.ToInt32(DrpSubVoucherType.SelectedValue);
        }

        DateTime ChequeDate;
        DateTime  Voucherdate;
        DateTime DueDate;

        if (txtVoucherDate.Text.Length < 10)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Must Enter Voucher Date');", true);
            return;
        }
        else
        {
            Voucherdate = DateTime.Parse(ConvertDate.British_To_American(txtVoucherDate.Text));
        }

        if (txtChequeDate.Text.Length == 10)
        {
            ChequeDate = DateTime.Parse(ConvertDate.British_To_American(txtChequeDate.Text));

        }
        else
        {
            ChequeDate = Constants.DateNullValue;
        }
        if (txtDueDate.Text.Length == 10)
        {
            DueDate  = DateTime.Parse(ConvertDate.British_To_American(txtChequeDate.Text));

        }
        else
        {
            DueDate = Constants.DateNullValue;
        }
        dtVoucher = (DataTable)this.Session["dtVoucher"];
        string MaxDocumentId = mLController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedValue.ToString()),Voucherdate);
        string cheque = "";
        if (DrpSubVoucherType.SelectedValue == "17")
        {
            //cheque = drpCheque.SelectedItem.Text;
            cheque = txtChequeNo.Text;
        }
        else if (DrpSubVoucherType.SelectedValue == "15")
        {
            cheque = txtChequeNo.Text;
        }
        else
        {
            cheque = txtChequeNo.Text;
        }
        //bool IsSaveVoucher = mLController.Add_Voucher(
        //    int.Parse(drpDistributor.SelectedValue.ToString()), 
        //    int.Parse(DrpPrincipal.SelectedValue.ToString()), 
        //    MaxDocumentId, 
        //    int.Parse(DrpVoucherType.SelectedValue.ToString()), 
        //    int.Parse(DrpSubVoucherType.SelectedValue.ToString()),
        //    Voucherdate, 
        //    int.Parse(DrpPaymentMode.SelectedValue.ToString()), 
        //    txtpayeesName.Text,
        //    txtRemarks.Text,
        //    ChequeDate, 
        //    txtChequeNo.Text,
        //    dtVoucher, 
        //    int.Parse(this.Session["UserId"].ToString()), 
        //    txtSlipNo.Text, 
        //    DueDate);
        bool IsSaveVoucher = mLController.Add_VoucherByIjaz(
            int.Parse(drpDistributor.SelectedValue.ToString())
            , int.Parse(DrpPrincipal.SelectedValue.ToString())
            , MaxDocumentId
            , VoucherType
            , Voucherdate
            , int.Parse(DrpPaymentMode.SelectedValue.ToString())
            , txtpayeesName.Text
            , txtRemarks.Text
            , ChequeDate
            , cheque
            , dtVoucher
            , int.Parse(this.Session["UserId"].ToString())
            , txtSlipNo.Text
            , DueDate
            , int.Parse(Session["MANUAL_VOUCHER"].ToString())
            );

        if (IsSaveVoucher == true)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " has been Saved');", true);
            this.ClearAll();
            this.PrintVoucher(MaxDocumentId); 

        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Some error occur Wrong Entry');", true);
        }
    }

    /// <summary>
    /// Sets Grid Footer Columns
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void GrdOrder_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[6].Text = txtTotalDebit.Text;
            e.Row.Cells[7].Text = txtTotalCredit.Text;
        }
    }


    protected void DrpAccountCode_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}