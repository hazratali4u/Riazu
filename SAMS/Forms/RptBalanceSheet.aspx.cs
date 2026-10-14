using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.Shared;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Linq;

/// <summary>
/// Form For Gross Profit Report
/// </summary>
public partial class Forms_RptBalanceSheet : System.Web.UI.Page
{
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
        if (!Page.IsPostBack)
        {
            this.LoadDistributor();
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
    /// <summary>
    /// Loads Locations To Location Combo
    /// </summary>
    private void LoadDistributor()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        this.drpDistributor.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
        clsWebFormUtil.FillDropDownList(this.drpDistributor, dt, 0, 2);
    }

    /// <summary>
    /// Shows Gross Profit Report in PDF
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewPDF_Click(object sender, EventArgs e)
    {
        ShowReport(0);
    }

    /// <summary>
    /// Shows Gross Profit Report in PDF
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewExcel_Click(object sender, EventArgs e)
    {
        ShowReport(1);
        //DocumentPrintController DPrint = new DocumentPrintController();
        //RptAccountController RptSaleCtl = new RptAccountController();
        //DataSet ds = RptSaleCtl.GetBalanceSheet(0, Convert.ToInt32(drpDistributor.SelectedValue), DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(this.Session["UserId"].ToString()), int.Parse(rbPosted.SelectedValue));
        //DataTable dt = null;
        //if (drpDistributor.SelectedIndex == 0)
        //{ dt = DPrint.SelectReportTitle(Constants.IntNullValue); }
        //else
        //{ dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString())); }
        //CrystalDecisions.CrystalReports.Engine.ReportDocument CrpReport = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
        //CrystalDecisions.CrystalReports.Engine.ReportDocument srAssets = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
        //CrystalDecisions.CrystalReports.Engine.ReportDocument srLiabilities = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
        //if (DrpLevel.SelectedValue == "4")
        //{
        //    CrpReport = new SAMSBusinessLayer.Reports.CrpBalanceSheet4();
        //}
        //else if (DrpLevel.SelectedValue == "3")
        //{
        //    CrpReport = new SAMSBusinessLayer.Reports.CrpBalanceSheet3();
        //}
        //else if (DrpLevel.SelectedValue == "2")
        //{
        //    CrpReport = new SAMSBusinessLayer.Reports.CrpBalanceSheet2();
        //}
        //else if (DrpLevel.SelectedValue == "1")
        //{
        //    CrpReport = new SAMSBusinessLayer.Reports.CrpBalanceSheet1();
        //}
        //srAssets = CrpReport.OpenSubreport("srAssets");
        //srLiabilities = CrpReport.OpenSubreport("srLiabilities");
        ////CrpReport.SetDataSource(ds);
        //srAssets.SetDataSource(ds);
        //srLiabilities.SetDataSource(ds);
        //CrpReport.Refresh();
        //CrpReport.SetParameterValue("Company_name", dt.Rows[0]["COMPANY_NAME"].ToString());
        //CrpReport.SetParameterValue("FromDate", Convert.ToDateTime(txtStartDate.Text));
        //CrpReport.SetParameterValue("ToDate", Convert.ToDateTime(txtEndDate.Text));
        //this.Session.Add("CrpReport", CrpReport);
        //this.Session.Add("ReportType", 1);
        //string url = "'Default.aspx'";
        //string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        //Type cstype = this.GetType();
        //ClientScriptManager cs = Page.ClientScript;
        //cs.RegisterStartupScript(cstype, "OpenWindow", script);
    }
    public void ShowReport(int ReportType)
    {
        DocumentPrintController DPrint = new DocumentPrintController();
        RptAccountController RptSaleCtl = new RptAccountController();

        DataSet ds = RptSaleCtl.GetBalanceSheet(0, Convert.ToInt32(drpDistributor.SelectedValue), DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(this.Session["UserId"].ToString()), int.Parse(rbPosted.SelectedValue.ToString()));

        DataTable dt = null;
        if (drpDistributor.SelectedIndex == 0)
        { dt = DPrint.SelectReportTitle(Constants.IntNullValue); }
        else
        { dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString())); }

        CrystalDecisions.CrystalReports.Engine.ReportDocument CrpReport = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
        CrystalDecisions.CrystalReports.Engine.ReportDocument srAssets = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
        CrystalDecisions.CrystalReports.Engine.ReportDocument srLiabilities = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
        CrystalDecisions.CrystalReports.Engine.ReportDocument srAssetsLiabilities = new CrystalDecisions.CrystalReports.Engine.ReportDocument();

        if (DrpLevel.SelectedValue == "4")
        {
            //CrpReport = new SAMSBusinessLayer.Reports.CrpBalanceSheet4();
            //CrpReport = new SAMSBusinessLayer.Reports.CrpBalanceSheet5();
            CrpReport = new SAMSBusinessLayer.Reports.CrpBalanceSheet6();
            //CrpReport = new SAMSBusinessLayer.Reports.CrpBalanceSheet4New();
        }
        else if (DrpLevel.SelectedValue == "3")
        {
            CrpReport = new SAMSBusinessLayer.Reports.CrpBalanceSheet3();
        }
        else if (DrpLevel.SelectedValue == "2")
        {
            CrpReport = new SAMSBusinessLayer.Reports.CrpBalanceSheet2();
        }
        else if (DrpLevel.SelectedValue == "1")
        {
            CrpReport = new SAMSBusinessLayer.Reports.CrpBalanceSheet1();
        }

        srAssets = CrpReport.OpenSubreport("srAssets");
        srLiabilities = CrpReport.OpenSubreport("srLiabilities");
        srAssetsLiabilities = CrpReport.OpenSubreport("SrAssetsLiabilities");
        //CrpReport.SetDataSource(ds);
        srAssets.SetDataSource(ds);
        srLiabilities.SetDataSource(ds);
        srAssetsLiabilities.SetDataSource(ds);
        CrpReport.Refresh();

        CrpReport.SetParameterValue("Company_name", dt.Rows[0]["COMPANY_NAME"].ToString());
        CrpReport.SetParameterValue("FromDate", Convert.ToDateTime(txtStartDate.Text));
        CrpReport.SetParameterValue("ToDate", Convert.ToDateTime(txtEndDate.Text));

        this.Session.Add("CrpReport", CrpReport);
        this.Session.Add("ReportType", ReportType);
        string url = "'Default.aspx'";
        string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        Type cstype = this.GetType();
        ClientScriptManager cs = Page.ClientScript;
        cs.RegisterStartupScript(cstype, "OpenWindow", script);
    }
}
