using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using SAMSBusinessLayer.Reports;
using System.Linq;

/// <summary>
/// Form For Voucher View Report
/// </summary>
public partial class Forms_frmVoucherView : System.Web.UI.Page
{
    UserController mUserController = new UserController();
    public int PrincipalWiseSale = 0;
    private static int mPrincipalId;
    /// <summary>
    /// Page_Load Function
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
            this.LoadPrincipal();
            this.VoucherType();
            btnView.Attributes.Add("onclick", "return ValidateForm();");
            SAMSCommon.Classes.Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtStartDate.Text = SAMSCommon.Classes.Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = SAMSCommon.Classes.Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            DrpVoucherType_SelectedIndexChanged(null, null);
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
    /// Loads Principals To Principal Combo
    /// </summary>
    private void LoadPrincipal()
    {
        SKUPriceDetailController PController = new SKUPriceDetailController();
        DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        this.DrpPrincipal.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
        //this.DrpPrincipal.Items.Add(new ListItem("GENERAL ENTRY", "0"));
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
    /// Loads Voucher Types To VoucherType Combo
    /// </summary>
    private void VoucherType()
    {
        LedgerController LController = new LedgerController();
        DataTable dt = LController.SelectVoucherType(int.Parse(this.Session["UserId"].ToString()));
        clsWebFormUtil.FillDropDownList(this.DrpVoucherType, dt, 0, 1, true);
    }

    /// <summary>
    /// Loads Vouchers To Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnView_Click(object sender, EventArgs e)
    {
        LedgerController mController = new LedgerController();
      //  int VoucherType = Constants.IntNullValue;
       // int VoucherType2 = Constants.IntNullValue;
        int PaymentMode = Constants.IntNullValue;

        //if (DrpVoucherType.SelectedValue == "16")
        //{
        //    VoucherType = Convert.ToInt32(DrpVoucherType.SelectedValue);
        //    PaymentMode = Convert.ToInt32(DrpVoucherSubType.SelectedValue);
        //}
        //else if (DrpVoucherType.SelectedValue == "14" || DrpVoucherType.SelectedValue == "15")
        //{
        //    if (DrpVoucherSubType.SelectedValue == Constants.IntNullValue.ToString())
        //    {
        //        VoucherType2 = Convert.ToInt32(DrpVoucherType.SelectedValue);
        //    }
        //    else
        //    {
        //        VoucherType = Convert.ToInt32(DrpVoucherSubType.SelectedValue);
        //    }
        //}
        //int mUserID = Constants.IntNullValue;

        //DataTable dt = mController.SelectUnPostVoucherNoByIjaz(VoucherType, int.Parse(drpDistributor.SelectedValue.ToString()), mUserID, bool.Parse(RbdList.SelectedValue.ToString()), DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"),
        //    0, VoucherType2, PaymentMode);
        int voucherType, voucherSubType;
        voucherType = int.Parse(DrpVoucherType.SelectedValue.ToString());
        voucherSubType = int.Parse(DrpVoucherSubType.SelectedValue.ToString());
        if (voucherSubType == Constants.IntNullValue && voucherType == 14)
        {
            voucherSubType = 24;
        }
        else if (voucherSubType == Constants.IntNullValue && voucherType == 15)
        {
            voucherSubType = 17;
        }
        else if (voucherSubType == Constants.IntNullValue && voucherType == 16)
        {
            voucherSubType = 16;
        }
        else if (voucherSubType != Constants.IntNullValue && voucherType != voucherSubType)
        {
            voucherType = voucherSubType;
        }
        else if (voucherSubType != Constants.IntNullValue && voucherType == 16)
        {
            voucherType = voucherSubType;
        }
        DataTable dt = mController.SelectUnPostVoucherNoNew(voucherType, int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()), bool.Parse(RbdList.SelectedValue.ToString()), DateTime.Parse(txtStartDate.Text + " 00:00:00"),
            DateTime.Parse(txtEndDate.Text + " 23:59:59"), 6, voucherSubType);
        GrdLedger.DataSource = dt;
        GrdLedger.DataBind();
        //int mUserID = Constants.IntNullValue;
        //if(this.Session["RoleID"].ToString() == "3")
        //{
        //    mUserID = Convert.ToInt32(this.Session["UserID"]);
        //}
        //DataTable dt = mController.SelectUnPostVoucherNo(int.Parse(DrpVoucherType.SelectedValue.ToString()), int.Parse(drpDistributor.SelectedValue.ToString()), mUserID, bool.Parse(RbdList.SelectedValue.ToString()), DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"),0);
        //GrdLedger.DataSource = dt;
        //GrdLedger.DataBind();  
    }
    
    protected void GrdLedger_RowEditing(object sender, GridViewEditEventArgs e)
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
            VoucherType = Convert.ToInt32(DrpVoucherSubType.SelectedValue);
            VoucherType2 = DrpVoucherSubType.SelectedItem.Text;
        }
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));
        //ds = RptAccountCtl.SelectUnpostVoucherForPrint(int.Parse(drpDistributor.SelectedValue.ToString()), GrdLedger.Rows[e.NewEditIndex].Cells[0].Text, int.Parse(DrpVoucherType.SelectedValue.ToString()));
        //CrpReport.SetDataSource(ds);
        //CrpReport.Refresh();
        ds = RptAccountCtl.SelectUnpostVoucherForPrint(int.Parse(drpDistributor.SelectedValue.ToString()), GrdLedger.Rows[e.NewEditIndex].Cells[0].Text, VoucherType);
        CrpReport.SetDataSource(ds);
        CrpReport.Refresh();
        string reporType = "Posted Voucher";
        if (bool.Parse(RbdList.SelectedValue.ToString()) == false)
        {
            reporType = "Unposted Voucher";
        }
        CrpReport.SetParameterValue("reportType", reporType);

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

    protected void DrpVoucherType_SelectedIndexChanged(object sender, EventArgs e)
    {
        DrpVoucherSubType.Items.Clear();
        if (DrpVoucherType.SelectedValue == "14")
        {
            //  DrpVoucherSubType.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
            DrpVoucherSubType.Items.Add(new ListItem("Cash Receipt Voucher", "14"));
            DrpVoucherSubType.Items.Add(new ListItem("Cash Payment Voucher", "24"));
            DrpVoucherSubType.Enabled = true;
        }
        else if (DrpVoucherType.SelectedValue == "16")
        {
            DrpVoucherSubType.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
            DrpVoucherSubType.Items.Add(new ListItem("Purchase Voucher", "2"));
            DrpVoucherSubType.Items.Add(new ListItem("Purchase Return Voucher", "3"));
            DrpVoucherSubType.Items.Add(new ListItem("Stock Damage Voucher", "6"));
            DrpVoucherSubType.Items.Add(new ListItem("Stock Short Voucher", "8"));
            DrpVoucherSubType.Items.Add(new ListItem("Stock Excess Voucher", "9"));
            DrpVoucherSubType.Items.Add(new ListItem("Sale Voucher", "13"));
            DrpVoucherSubType.Items.Add(new ListItem("Sale Return Voucher", "14"));
            DrpVoucherSubType.Items.Add(new ListItem("Customer Claim Voucher", "19"));
            DrpVoucherSubType.Items.Add(new ListItem("Expense Voucher", "27"));
            //DrpVoucherSubType.Items.Add(new ListItem("Freight Voucher", "40"));
            //DrpVoucherSubType.Items.Add(new ListItem("Manual Voucher", "1"));
            DrpVoucherSubType.Enabled = true;
        }
        else
        {
            //   DrpVoucherSubType.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
            DrpVoucherSubType.Items.Add(new ListItem("Bank Receipt Voucher", "15"));
            DrpVoucherSubType.Items.Add(new ListItem("Bank Payment Voucher", "17"));
            DrpVoucherSubType.Enabled = true;
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        var VOUCHER_NO = ((LinkButton)sender).CommandArgument;
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
            VoucherType = Convert.ToInt32(DrpVoucherSubType.SelectedValue);
            VoucherType2 = DrpVoucherSubType.SelectedItem.Text;
        }
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));
        //ds = RptAccountCtl.SelectUnpostVoucherForPrint(int.Parse(drpDistributor.SelectedValue.ToString()), GrdLedger.Rows[e.NewEditIndex].Cells[0].Text, int.Parse(DrpVoucherType.SelectedValue.ToString()));
        //CrpReport.SetDataSource(ds);
        //CrpReport.Refresh();
        ds = RptAccountCtl.SelectUnpostVoucherForPrint(int.Parse(drpDistributor.SelectedValue.ToString()), VOUCHER_NO.ToString(), VoucherType);
        CrpReport.SetDataSource(ds);
        CrpReport.Refresh();
        string reporType = "Posted Voucher";
        if (bool.Parse(RbdList.SelectedValue.ToString()) == false)
        {
            reporType = "Unposted Voucher";
        }
        CrpReport.SetParameterValue("reportType", reporType);

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
}
