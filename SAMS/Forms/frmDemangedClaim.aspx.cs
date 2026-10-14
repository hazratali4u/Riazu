using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Linq;

/// <summary>
/// Form To Add, Edit Customer Claim
/// </summary>
public partial class Forms_frmDemangedClaim : System.Web.UI.Page
{
    UserController mUserController = new UserController();
    public int PrincipalWiseSale = 0;
    private static int mPrincipalId;
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
        Configration();
        if (!Page.IsPostBack)
        {
            
            this.LoadDistributor();
            this.LoadClaimType();
            this.LoadAccountHead();
            this.LoadPrincipal();
            this.CreatTable();
            this.LoadArea();
            this.LoadCustomer();
            this.LoadGrid();
            btnAddNew.Attributes.Add("onclick", "return ValidateForm();");
            ScriptManager.GetCurrent(Page).SetFocus(drpDistributor);
            lblRowId.Text = "-1";
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
    private void Configration()
    {
        DataTable dtAppSetting = (DataTable)Session["dtAppSetting"];
        PanelPrincipal.Visible = true;
        if (dtAppSetting != null)
        {
            DataRow[] drAppSetting = dtAppSetting.Select("strColumnName='PrincipalWiseSale'");
            if (drAppSetting.Length > 0)
            {
                PrincipalWiseSale = Convert.ToInt32(drAppSetting[0]["strColumnValue"]);
                if (drAppSetting[0]["strColumnValue"].ToString() == "1")
                {
                    PanelPrincipal.Visible = true;
                }
                else
                {
                    PanelPrincipal.Visible = false;
                }
            }
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
    /// Loads Claim Types To RadioButtonList
    /// </summary>
    private void LoadClaimType()
    {
        RbdClaimType.Items.Add(new ListItem("Debit Claim", Constants.DebitClaim.ToString()));
        RbdClaimType.Items.Add(new ListItem("Credit Claim", Constants.CreditClaim.ToString()));
        RbdClaimType.SelectedIndex = 0;         
    }
    
    /// <summary>
    /// Loads Account Heads To AccountHead Combo
    /// Modified by: Hazrat Ali
    /// 2012-Feb-16 10:40 AM
    /// Hard coded Account Heads as per Saddruddin Sb Instructions
    /// This is valid only for FDMPL and invalid for BDN and FDM
    /// Commited old code
    /// </summary>
    private void LoadAccountHead()
    {        
        DrpAccountHead.Items.Clear();
        AccountHeadController mAccountController = new AccountHeadController();
        DataTable dt = mAccountController.SelectClaimHead(Constants.IntNullValue);
        clsWebFormUtil.FillDropDownList(DrpAccountHead, dt, 0, 2, true);
    }

    /// <summary>
    /// Loads Principals To Principal Combo
    /// </summary>
    private void LoadPrincipal()
    {
        SKUPriceDetailController PController = new SKUPriceDetailController();
        DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, m_dt, 0, 1);
    }
    
    /// <summary>
    /// Creates Datatable For Customer Claim Data
    /// </summary>
    private void CreatTable()
    {
        DataTable dtVoucher = new DataTable();
        dtVoucher.Columns.Add("Account_Head_Id", typeof(long));
        dtVoucher.Columns.Add("Account_Code", typeof(string));
        dtVoucher.Columns.Add("Account_Name", typeof(string));
        dtVoucher.Columns.Add("Debit", typeof(decimal));
        dtVoucher.Columns.Add("Credit", typeof(decimal));
        dtVoucher.Columns.Add("Remarks", typeof(string));
        this.Session.Add("dtVoucher", dtVoucher);
        GrdOrder.DataSource = dtVoucher;
        GrdOrder.DataBind();  

    }
    
    /// <summary>
    /// Loads Routes To Route Combo
    /// </summary>
    private void LoadArea()
    {
        DistributorAreaController mController = new DistributorAreaController();
        DataTable dt = mController.SelectDist_Area(Constants.LongNullValue, Constants.DateNullValue, Constants.DateNullValue, int.Parse(drpDistributor.SelectedValue.ToString()), Constants.IntNullValue, null, null);
        clsWebFormUtil.FillDropDownList(DrpRoute, dt, 0, 6, true);
        DrpRoute.Enabled = true;        
    }
    
    /// <summary>
    /// Loads Customers To Customer Combo
    /// </summary>
    private void LoadCustomer()
    {
        if (PrincipalWiseSale == 0)
        {
            mPrincipalId = Constants.IntNullValue;
        }
        else
        {
            mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
        }
        if (drpDistributor.Items.Count > 0 && DrpRoute.Items.Count > 0)
        {
            CustomerDataController mController = new CustomerDataController();
            DataTable dt = mController.SelectPrincipalCustomer(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpRoute.SelectedValue.ToString()), Constants.IntNullValue, mPrincipalId);
            clsWebFormUtil.FillDropDownList(this.DrpCustomer, dt, 0, 4, true);
        }
        else
        {
            DrpCustomer.Items.Clear();   
        }
    }
    
    /// <summary>
    /// Loads Customer Claims To Grid
    /// </summary>
    private void LoadGrid()
    {
        if (drpDistributor.Items.Count > 0)
        {
            LedgerController LController = new LedgerController();
            DataTable dt = LController.SelectClaimDetail(int.Parse(drpDistributor.SelectedValue.ToString()),int.Parse(RbdClaimType.SelectedValue.ToString()),
                DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), PrincipalWiseSale);
            GrdOrder.DataSource = dt;
            GrdOrder.DataBind();
            if (PrincipalWiseSale == 0)
            {
                GrdOrder.Columns[3].Visible = false;
            }
            else
            {
                GrdOrder.Columns[3].Visible = true;
            }
        }
    }

    /// <summary>
    /// Loads Routes And Customers
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadArea();
        this.LoadCustomer();
    }

    /// <summary>
    /// Loads Customers
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpRoute_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadCustomer(); 
    }

    /// <summary>
    /// Deletes Customer Claim
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void GrdOrder_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        LedgerController LController = new LedgerController();
        DataControl dc = new DataControl();

        LController.DeleteWareHouseLedger(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(dc.chkNull_0(GrdOrder.Rows[e.RowIndex].Cells[2].Text)),
        int.Parse(dc.chkNull_0(GrdOrder.Rows[e.RowIndex].Cells[6].Text)),Constants.LongNullValue  , decimal.Parse(dc.chkNull_0(GrdOrder.Rows[e.RowIndex].Cells[7].Text)));
        this.LoadGrid();
        
    }

    /// <summary>
    /// Saves Customer Claim
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnAddNew_Click(object sender, EventArgs e)
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
            if (PrincipalWiseSale == 0)
            {
                mPrincipalId = Constants.IntNullValue;
            }
            else
            {
                mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
            }
            LedgerController LController = new LedgerController();

            string MaxDocumentId = LController.SelectLedgerMaxDocumentId(Constants.Journal_Voucher, int.Parse(drpDistributor.SelectedValue.ToString()));
            this.Session.Add("VoucherNo", MaxDocumentId);
            if (RbdClaimType.SelectedValue == Constants.DebitClaim.ToString())
            {
                LController.PostingCash_Bank_Account(Constants.Journal_Voucher, long.Parse(MaxDocumentId), long.Parse(DrpAccountHead.SelectedValue.ToString()), int.Parse(drpDistributor.SelectedValue.ToString()), 0, decimal.Parse(txtAmount.Text),
                   DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), txtRemarks.Text, DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), mPrincipalId,
                    null, int.Parse(this.Session["UserId"].ToString()), Constants.LongNullValue, null, Constants.IntNullValue, null, Constants.DateNullValue, Constants.DebitClaim, "");
            }
            else
            {
                LController.PostingCash_Bank_Account(Constants.Journal_Voucher, long.Parse(MaxDocumentId), long.Parse(DrpAccountHead.SelectedValue.ToString()), int.Parse(drpDistributor.SelectedValue.ToString()), decimal.Parse(txtAmount.Text), 0,
                    DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), txtRemarks.Text, DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), mPrincipalId,
                     null, int.Parse(this.Session["UserId"].ToString()), Constants.LongNullValue, null, Constants.IntNullValue, null, Constants.DateNullValue, Constants.CreditClaim, "");
            }
            InsertGL();
            txtAmount.Text = "";
            txtRemarks.Text = "";
            this.LoadGrid();
        }
    }

    /// <summary>
    /// Loads Account Heads And Claim Data
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void RbdClaimType_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadGrid();
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

    private void InsertGL()
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
        if (RbdClaimType.SelectedValue == Constants.CreditClaim.ToString())
        {
            DataRow dr = dtVoucher.NewRow();
            dr["LEDGER_ID"] = Constants.LongNullValue.ToString();
            dr["ACCOUNT_HEAD_ID"] = Convert.ToInt64(DrpAccountHead.SelectedValue);
            dr["REMARKS"] = DrpAccountHead.SelectedItem.Text + " received from " + DrpCustomer.SelectedItem.Text;
            dr["DEBIT"] = decimal.Parse(txtAmount.Text);
            dr["CREDIT"] = 0;
            dr["Principal_Id"] = 0;
            dtVoucher.Rows.Add(dr);

            ///Credit Side Entry
            DataRow dr1 = dtVoucher.NewRow();
            dr1["LEDGER_ID"] = Constants.LongNullValue.ToString();
            dr1["ACCOUNT_HEAD_ID"] = 107; //3002020001-Account Receivable (Party Wise)
            dr1["DEBIT"] = 0;
            dr1["CREDIT"] = decimal.Parse(txtAmount.Text);
            dr1["Principal_Id"] = 0;
            dr1["REMARKS"] = DrpAccountHead.SelectedItem.Text + " received from " + DrpCustomer.SelectedItem.Text;
            dtVoucher.Rows.Add(dr1);
            string MaxDocumentId = LController.SelectMaxVoucherId(Constants.Journal_Voucher, Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToDateTime(Session["CurrentWorkDate"]));
            LController.Add_VoucherPosted(Convert.ToInt32(drpDistributor.SelectedValue), 0, MaxDocumentId, Constants.Journal_Voucher, Convert.ToDateTime(Session["CurrentWorkDate"]), Constants.CashPayment, Session["VoucherNo"].ToString(), "Defualt Customer Adjustment / " + DrpCustomer.SelectedItem.Text + " / " + txtRemarks.Text, Convert.ToDateTime(Session["CurrentWorkDate"]), null, dtVoucher, Convert.ToInt32(Session["UserID"]), null, Constants.DateNullValue);
            PrintVoucher(MaxDocumentId);
        }
        else
        {

            DataRow dr = dtVoucher.NewRow();
            dr["LEDGER_ID"] = Constants.LongNullValue.ToString();
            dr["ACCOUNT_HEAD_ID"] = Convert.ToInt64(DrpAccountHead.SelectedValue);
            dr["REMARKS"] = DrpAccountHead.SelectedItem.Text + " received from " + DrpCustomer.SelectedItem.Text;
            dr["DEBIT"] = 0;
            dr["CREDIT"] = decimal.Parse(txtAmount.Text);
            dr["Principal_Id"] = 0;
            dtVoucher.Rows.Add(dr);

            ///debit Side Entry
            DataRow dr1 = dtVoucher.NewRow();
            dr1["LEDGER_ID"] = Constants.LongNullValue.ToString();
            dr1["ACCOUNT_HEAD_ID"] = 107;//3002020001-Account Receivable (Party Wise)
            dr1["DEBIT"] = decimal.Parse(txtAmount.Text);
            dr1["CREDIT"] = 0;
            dr1["Principal_Id"] = 0;
            dr1["REMARKS"] = DrpAccountHead.SelectedItem.Text + " received from " + DrpCustomer.SelectedItem.Text;
            dtVoucher.Rows.Add(dr1);
            string MaxDocumentId = LController.SelectMaxVoucherId(Constants.Journal_Voucher, Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToDateTime(Session["CurrentWorkDate"]));
            LController.Add_VoucherPosted(Convert.ToInt32(drpDistributor.SelectedValue), 0, MaxDocumentId, Constants.Journal_Voucher, Convert.ToDateTime(Session["CurrentWorkDate"]), Constants.CashPayment, Session["VoucherNo"].ToString(), "Defualt Customer Adjustment / " + DrpCustomer.SelectedItem.Text + " / " + txtRemarks.Text, Convert.ToDateTime(Session["CurrentWorkDate"]), null, dtVoucher, Convert.ToInt32(Session["UserID"]), null, Constants.DateNullValue);
            PrintVoucher(MaxDocumentId);
        }
    }
    private void PrintVoucher(string VoucherNo)
    {
        #region print report
        DocumentPrintController DPrint = new DocumentPrintController();
        RptAccountController RptAccountCtl = new RptAccountController();
        SAMSBusinessLayer.Reports.crpVoucherView CrpReport = new SAMSBusinessLayer.Reports.crpVoucherView();

        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));
        ds = RptAccountCtl.SelectUnpostVoucherForPrint(int.Parse(drpDistributor.SelectedValue.ToString()), VoucherNo, Constants.Journal_Voucher);
        CrpReport.SetDataSource(ds);
        CrpReport.Refresh();
        //   string VoucherType2 = "Customer Claim Voucher";

        CrpReport.SetParameterValue("Company_Name", dt.Rows[0]["COMPANY_NAME"].ToString());
        CrpReport.SetParameterValue("DISTRIBUTOR_NAME", dt.Rows[0]["DISTRIBUTOR_NAME"].ToString());
        CrpReport.SetParameterValue("reportType", "Posted Voucher");
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