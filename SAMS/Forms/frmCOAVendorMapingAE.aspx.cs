using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Forms_frmCOAVendorMapingAE : System.Web.UI.Page
{

    readonly DistributorController _dController = new DistributorController();
    readonly DistributorAreaController _mController = new DistributorAreaController();
    readonly CustomerDataController _mCdControllerontroller = new CustomerDataController();
    readonly SKUPriceDetailController _pController = new SKUPriceDetailController();
    readonly DocumentPrintController _dPrint = new DocumentPrintController();
    readonly RptCustomerController _rptCustomerCtl = new RptCustomerController();
    readonly SKUPriceDetailController PController = new SKUPriceDetailController();
    readonly CustomerDataController cus = new CustomerDataController();

    bool ViewAccess;
    protected void Page_Load(object sender, EventArgs e)
    {
        //ViewAccess = ValidateScreen();
        //if (ViewAccess == false)
        //{
        //    Response.Redirect("../Forms/AccessDenied.aspx");
        //}

        if (!Page.IsPostBack)
        {
            LoadPrincipal();
            LoadDistributor();
            LoadAccount();
            LoadDisccountAccount();
            LoadCustomer();
            rbType_SelectedIndexChanged(null, null);
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
    private void LoadDistributor()
    {

        DataTable dt = _dController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDropDownList(this.drpDistributor, dt, 0, 2, true);
    }
    private void LoadAccount()
    {

        AccountHeadController mAccountController = new AccountHeadController();
        DataTable dt = mAccountController.SelectAccountHead_UnMapped(Constants.AC_AccountHeadId, 28);//
        clsWebFormUtil.FillDropDownList(DrpAccountDetail, dt, 0, 4, true);
    }
    private void LoadDisccountAccount()
    {

        AccountHeadController mAccountController = new AccountHeadController();
        DataTable dt = mAccountController.SelectAccountHead_UnMapped(Constants.AC_AccountHeadId, 56);//
        clsWebFormUtil.FillDropDownList(DrpDisccountAccount, dt, 0, 4, true);
    }
    private void LoadCustomer()
    {
        DrpCustomer.Items.Clear();
        if (drpDistributor.Items.Count > 0)
        {
            DataTable dt = _mCdControllerontroller.SelectAllCustomer_UnMapped(int.Parse(drpDistributor.SelectedValue.ToString()), Constants.IntNullValue, Constants.IntNullValue);
            clsWebFormUtil.FillDropDownList(this.DrpCustomer, dt, 0, 3, true);
            //SKUPriceDetailController _skuPriceController = new SKUPriceDetailController();
            //var mDt = _skuPriceController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
            //clsWebFormUtil.FillDropDownList(this.DrpPrincipal, mDt, 0, 1, true);
        }
    }
    private void LoadPrincipal()// ONLY UNASIGNED PRINCIPALS
    {
        DrpPrincipal.Items.Clear();

        var mDt = _pController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 4, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        // DrpPrincipal.Items.Add(new ListItem("All",Constants.IntNullValue.ToString()));
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, mDt, 0, 1);

    }
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadCustomer();
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (rbType.SelectedValue == "0")//Customer Mapping
        {
            if (drpDistributor.Items.Count > 0 && DrpCustomer.Items.Count > 0 && DrpAccountDetail.Items.Count > 0 && DrpDisccountAccount.Items.Count > 0)
            {
                
                var insert = cus.AccountHeadMapping(long.Parse(DrpCustomer.SelectedValue), int.Parse(drpDistributor.SelectedValue), Constants.IntNullValue, int.Parse(DrpAccountDetail.SelectedValue), int.Parse(rbType.SelectedValue), int.Parse(this.Session["UserID"].ToString()), int.Parse(DrpDisccountAccount.SelectedValue));
                if (insert > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Account Head Mapped Successfully ! ');", true);
                    LoadPrincipal();
                    LoadCustomer();
                    LoadAccount();
                    LoadDisccountAccount();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Error,Account / Customer Already Mapped ! ');", true);
                }
            }
        }
        if (rbType.SelectedValue == "1")//Principal Mapping
        {
            if (DrpPrincipal.Items.Count > 0 && DrpAccountDetail.Items.Count > 0)
            {
                var insert = cus.AccountHeadMapping(Constants.LongNullValue, Constants.IntNullValue, int.Parse(DrpPrincipal.SelectedValue), int.Parse(DrpAccountDetail.SelectedValue), int.Parse(rbType.SelectedValue), int.Parse(this.Session["UserID"].ToString()), int.Parse(DrpDisccountAccount.SelectedValue));
                if (insert > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Account Head Mapped Successfully ! ');", true);
                    LoadPrincipal();
                    LoadCustomer();
                    LoadAccount();
                    LoadDisccountAccount();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Error,Account / Customer Already Mapped ! ');", true);
                }
            }
        }
    }
    protected void rbType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rbType.SelectedValue == "0")
        {
            lblPrincipal.Visible = false;
            DrpPrincipal.Visible = false;
            lblCustomer.Visible = true;
            DrpCustomer.Visible = true;
            lblLocation.Visible = true;
            drpDistributor.Visible = true;
        }
        else
        {
            lblPrincipal.Visible = true;
            DrpPrincipal.Visible = true;
            lblCustomer.Visible = false;
            DrpCustomer.Visible = false;
            lblLocation.Visible = false;
            drpDistributor.Visible = false;
        }
    }
    protected void btnViewReport_Click(object sender, EventArgs e)
    {
        if (rbType.SelectedValue == "0")
        {
            DocumentPrintController DPrint = new DocumentPrintController();
            DataSet ds = DPrint.SelectAccountHeadMappingReport(int.Parse(rbType.SelectedValue), int.Parse(drpDistributor.SelectedValue));
            DataTable dt = DPrint.SelectReportTitle(Constants.IntNullValue);
            SAMSBusinessLayer.Reports.CrpAccountMapping CrpReport = new SAMSBusinessLayer.Reports.CrpAccountMapping();
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();
            CrpReport.SetParameterValue("Company_Name", dt.Rows[0]["COMPANY_NAME"].ToString());
            this.Session.Add("CrpReport", CrpReport);
            this.Session.Add("ReportType", 0);
            string url = "'Default.aspx'";
            string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterStartupScript(cstype, "OpenWindow", script);
        }
        else
        {
            DocumentPrintController DPrint = new DocumentPrintController();
            DataSet ds = DPrint.SelectAccountHeadMappingReport(int.Parse(rbType.SelectedValue), Constants.IntNullValue);
            DataTable dt = DPrint.SelectReportTitle(Constants.IntNullValue);
            SAMSBusinessLayer.Reports.CrpAccountMappingPrincipal CrpReport = new SAMSBusinessLayer.Reports.CrpAccountMappingPrincipal();
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();
            CrpReport.SetParameterValue("Company_Name", dt.Rows[0]["COMPANY_NAME"].ToString());
            this.Session.Add("CrpReport", CrpReport);
            this.Session.Add("ReportType", 0);
            string url = "'Default.aspx'";
            string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterStartupScript(cstype, "OpenWindow", script);
        }
    }
}