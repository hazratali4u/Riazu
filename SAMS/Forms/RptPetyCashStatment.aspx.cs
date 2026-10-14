using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using CrystalDecisions.CrystalReports.Engine;
using System.Linq;

/// <summary>
/// Form For Petty Expense Report
/// </summary>
public partial class Forms_RptPetyCashStatment : System.Web.UI.Page
{
    UserController mUserController = new UserController();
    public int PrincipalWiseSale = 0;
    private static int mPrincipalId;
    string Catagories_IDs = null;
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
            this.LoadAccountParent();
            this.LoadAccountHead();
            SAMSCommon.Classes.Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtStartDate.Text = SAMSCommon.Classes.Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = SAMSCommon.Classes.Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
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
        DrpPrincipal.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
        DrpPrincipal.Items.Add(new ListItem("General Expense", "0"));
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, m_dt, 0, 1);
    }

    /// <summary>
    /// Loads Parent Accounts To Account Combo
    /// </summary>
    private void LoadAccountParent()
    {
        //SAMSCommon.Classes.Configuration.GetAccountHead();
        //AccountHeadController mAccountController = new AccountHeadController();
        ////DataTable dt = mAccountController.SelectAccountHead(Constants.AC_DetailTypeId, long.Parse(SAMSCommon.Classes.Configuration.ExpensDefaultType));
        //DrpMasterHead.Items.Add(new ListItem("All", "55,56,61"));
        ////clsWebFormUtil.FillDropDownList(DrpMasterHead, dt, 0, 4);
    }
    /// <summary>
    /// Loads Account Heaeds To Account Combo
    /// </summary>
    private void LoadAccountHead()
    {
        if (DrpMasterHead.Items.Count > 0)
        {
            AccountHeadController mAccountController = new AccountHeadController();
            DataTable dt = mAccountController.SelectAccountHead2(Constants.AC_AccountHeadId, DrpMasterHead.SelectedValue.ToString());
            clsWebFormUtil.FillListBox(LstAccountHead, dt, 0, 4, true);
        }
    }
    /// <summary>
    /// Loads Locations To Location Combo
    /// </summary>
    private void LoadDistributor()
    {
        drpDistributor.Items.Clear();   
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        if (RbReportType.SelectedIndex == 1)
        {
            drpDistributor.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
        }
        clsWebFormUtil.FillDropDownList(this.drpDistributor, dt, 0, 2);
    }
    
    /// <summary>
    /// Loads Locations
    /// </summary>
    protected void RbReportType_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadDistributor();
    }

    /// <summary>
    /// Shows Petty Expense Report in PDF
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewPDF_Click(object sender, EventArgs e)
    {
        
        DocumentPrintController mController = new DocumentPrintController();
        RptAccountController RptAccountCtl = new RptAccountController();
        ReportDocument CrpReport = new ReportDocument();
        DataTable dt = mController.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));
        for (int i = 0; i < this.LstAccountHead.Items.Count; i++)
        {
            if (this.LstAccountHead.Items[i].Selected == true)
            {
                Catagories_IDs += this.LstAccountHead.Items[i].Value.ToString() + ",";
            }
        }
        if (PrincipalWiseSale == 0)
        {
            mPrincipalId = Constants.IntNullValue;
        }
        else
        {
            mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
        }
        if (RbReportType.SelectedIndex == 0)
        {

            if (PrincipalWiseSale == 0)
            {
                CrpReport = new SAMSBusinessLayer.Reports.CrpPetyCashSummaryWithoutPrincipal();
            }
            else
            {
                CrpReport = new SAMSBusinessLayer.Reports.CrpPetyCashSummary();
            }
            
            DataSet ds = RptAccountCtl.SelectPetyCashStatment(int.Parse(drpDistributor.SelectedValue.ToString()), mPrincipalId, DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(this.Session["UserId"].ToString()),Convert.ToString(Catagories_IDs));
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();

            if (PrincipalWiseSale == 0)
            {
                CrpReport.SetParameterValue("Principal", "");
            }
            else
            {
                CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
            }
            //CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
            CrpReport.SetParameterValue("Distributor", drpDistributor.SelectedItem.Text);
            CrpReport.SetParameterValue("FROM_DATE", txtStartDate.Text);
            CrpReport.SetParameterValue("TO_DATE", txtEndDate.Text);
            CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());

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
            CrpReport = new SAMSBusinessLayer.Reports.CrpPettyCaahSummery();

            DataSet ds = RptAccountCtl.SelectPetyCashSummary(int.Parse(drpDistributor.SelectedValue.ToString()), DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(this.Session["UserId"].ToString()));
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();

            CrpReport.SetParameterValue("FROM_DATE", txtStartDate.Text);
            CrpReport.SetParameterValue("TO_DATE", txtEndDate.Text);
            CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());

            this.Session.Add("CrpReport", CrpReport);
            this.Session.Add("ReportType", 0);
            string url = "'Default.aspx'";
            string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterStartupScript(cstype, "OpenWindow", script);
        }
    }

    /// <summary>
    /// Shows Petty Expense Report in Excel
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewExcel_Click(object sender, EventArgs e)
    {
        RptAccountController RptAccountCtl = new RptAccountController();
        DocumentPrintController mController = new DocumentPrintController();
        ReportDocument CrpReport = new ReportDocument();
        DataTable dt = mController.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));
        for (int i = 0; i < this.LstAccountHead.Items.Count; i++)
        {
            if (this.LstAccountHead.Items[i].Selected == true)
            {
                Catagories_IDs += this.LstAccountHead.Items[i].Value.ToString() + ",";
            }
        }
        if (PrincipalWiseSale == 0)
        {
            mPrincipalId = Constants.IntNullValue;
        }
        else
        {
            mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
        }
        if (RbReportType.SelectedIndex == 0)
        {
            if (PrincipalWiseSale == 0)
            {
                CrpReport = new SAMSBusinessLayer.Reports.CrpPetyCashSummaryWithoutPrincipal();
            }
            else
            {
                CrpReport = new SAMSBusinessLayer.Reports.CrpPetyCashSummary();
            }
            DataSet ds = RptAccountCtl.SelectPetyCashStatment(int.Parse(drpDistributor.SelectedValue.ToString()), mPrincipalId, DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(this.Session["UserId"].ToString()), Catagories_IDs);
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();

            if (PrincipalWiseSale == 0)
            {
                CrpReport.SetParameterValue("Principal", "");
            }
            else
            {
                CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
            }
            //CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
            CrpReport.SetParameterValue("Distributor", drpDistributor.SelectedItem.Text);
            CrpReport.SetParameterValue("FROM_DATE", txtStartDate.Text);
            CrpReport.SetParameterValue("TO_DATE", txtEndDate.Text);
            CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());

            this.Session.Add("CrpReport", CrpReport);
            this.Session.Add("ReportType", 1);
            string url = "'Default.aspx'";
            string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterStartupScript(cstype, "OpenWindow", script);
        }
        else
        {
            CrpReport = new SAMSBusinessLayer.Reports.CrpPettyCaahSummery();
            DataSet ds = RptAccountCtl.SelectPetyCashSummary(int.Parse(drpDistributor.SelectedValue.ToString()), DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(this.Session["UserId"].ToString()));
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();

            CrpReport.SetParameterValue("FROM_DATE", txtStartDate.Text);
            CrpReport.SetParameterValue("TO_DATE", txtEndDate.Text);
            CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());

            this.Session.Add("CrpReport", CrpReport);
            this.Session.Add("ReportType", 1);
            string url = "'Default.aspx'";
            string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterStartupScript(cstype, "OpenWindow", script);
        }
    }

    /// <summary>
    /// Loads Account Heads
    /// </summary>
    protected void DrpMasterHead_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadAccountHead();
    }

}
