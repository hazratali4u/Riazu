using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Linq;
using CrystalDecisions.CrystalReports.Engine;

/// <summary>
/// Form For Credit Aging Report
/// </summary>
public partial class Forms_RptCreditAging : System.Web.UI.Page
{
    /// <summary>
    /// Page_Load Function
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    DocumentPrintController mDocumentPrntControl = new DocumentPrintController();
    RptCustomerController RptCustomerCtl = new RptCustomerController();
    public int PrincipalWiseSale = 0;
    private static int mPrincipalId;
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
            this.DistributorType();
            this.LoadAssingned();
            this.LoadPrincipal();
            this.LoadOrderBooker();
            ReportTypes();
            this.LoadChannelType();
            rbtSotBy_SelectedIndexChanged(null, null);
            RbtReportType_SelectedIndexChanged(null, null);
            SAMSCommon.Classes.Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtDocmentDate.Text = SAMSCommon.Classes.Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
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
    /// Loads Location Types To LocationType Combo
    /// </summary>
    private void DistributorType()
    {
        DistributorController dController = new DistributorController();
        DataTable dt = dController.SelectDistributorTypeInfo(Constants.IntNullValue);
        clsWebFormUtil.FillDropDownList(ddDistributorType, dt, 0, 2);
    }

    /// <summary>
    /// Loads Locations To Location Combo
    /// </summary>
    private void LoadAssingned()
    {
        if (ddDistributorType.Items.Count > 0)
        {
            drpDistributor.Items.Clear();
            if (rbtSotBy.SelectedIndex == 1)
            {
                UserController mUserController = new UserController();
                DataTable dt = mUserController.SelectUserAssignment(int.Parse(this.Session["UserId"].ToString()), int.Parse(ddDistributorType.SelectedValue.ToString()), 1, int.Parse(this.Session["CompanyId"].ToString()));
                clsWebFormUtil.FillDropDownList(drpDistributor, dt, 0, 1);

            }
            else
            {
                UserController mUserController = new UserController();
                DataTable dt = mUserController.SelectUserAssignment(int.Parse(this.Session["UserId"].ToString()), int.Parse(ddDistributorType.SelectedValue.ToString()), 1, int.Parse(this.Session["CompanyId"].ToString()));
                drpDistributor.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
                clsWebFormUtil.FillDropDownList(drpDistributor, dt, 0, 1);
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
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, m_dt, 0, 1);
    }

    /// <summary>
    /// Loads Channel Types To ChannelType Combo
    /// </summary>
    private void LoadChannelType()
    {
        SLASHCodesController mController = new SLASHCodesController();
        DataTable dt = mController.SelectSlashCodes(Constants.IntNullValue, null, Constants.CustomerChannelType, null, Constants.IntNullValue, bool.Parse("True"));
        drpChannelType.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
        clsWebFormUtil.FillDropDownList(drpChannelType, dt, 0, 2);

    }
    private void LoadOrderBooker()
    {
        if (drpDistributor.Items.Count > 0)
        {
            DrpOrderBooker.Items.Clear();
            Distributor_UserController mDController = new Distributor_UserController();
            DataTable m_dt = mDController.SelectDistributorUser(Constants.SALES_FORCE_ORDERBOOKER, int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(this.Session["CompanyId"].ToString()));
            DrpOrderBooker.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
            clsWebFormUtil.FillDropDownList(this.DrpOrderBooker, m_dt, 0, 6);
        }
    }
    private void ReportTypes()
    {
        if (PrincipalWiseSale == 0)
        {
            mPrincipalId = Constants.IntNullValue;
        }
        else
        {
            mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
        }
        if (mPrincipalId != Constants.IntNullValue)
        {
            rbtSotBy.Items.Add(new ListItem("Principal Wise", "0"));
            rbtSotBy.Items.Add(new ListItem("Customer Wise", "1"));
            rbtSotBy.Items.Add(new ListItem("Date Wise", "2"));
            rbtSotBy.SelectedIndex = 0;
        }
        else
        {
            rbtSotBy.Items.Add(new ListItem("Customer Wise", "1"));
            rbtSotBy.Items.Add(new ListItem("Date Wise", "2"));
            rbtSotBy.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Loads Locations
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void ddDistributorType_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadAssingned();
    }

    protected void btnViewPDF_Click(object sender, EventArgs e)
    {
        if (PrincipalWiseSale == 0)
        {
            mPrincipalId = Constants.IntNullValue;
        }
        else
        {
            mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
        }
        if (RbtReportType.SelectedValue == "1")
        {
            DataTable dt = mDocumentPrntControl.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));
            DataSet ds = RptCustomerCtl.GetCreditAgingReport(int.Parse(this.ddDistributorType.SelectedItem.Value), mPrincipalId, int.Parse(drpDistributor.SelectedValue.ToString())
                , int.Parse(this.Session["UserId"].ToString()), int.Parse(rbtSotBy.SelectedValue.ToString()), DateTime.Parse(txtDocmentDate.Text), DateTime.Parse(txtEndDate.Text + " 23:59:59")
                , int.Parse(drpChannelType.SelectedValue.ToString()), int.Parse(DrpOrderBooker.SelectedValue));
            SAMSBusinessLayer.Reports.CrpCreditAgingReport CrpReport = new SAMSBusinessLayer.Reports.CrpCreditAgingReport();
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();
            CrpReport.SetParameterValue("UptoDate", txtEndDate.Text);
            CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
            CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
            CrpReport.SetParameterValue("ReportType", rbtSotBy.SelectedValue.ToString());
            CrpReport.SetParameterValue("ChannelType", drpChannelType.SelectedItem.Text);
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
          ShowReport_Summary(0);
        }
        
    }
    protected void btnViewExcel_Click(object sender, EventArgs e)
    {
        if (PrincipalWiseSale == 0)
        {
            mPrincipalId = Constants.IntNullValue;
        }
        else
        {
            mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
        }
        if (RbtReportType.SelectedValue == "1")
        {
            DataTable dt = mDocumentPrntControl.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));
            DataSet ds = RptCustomerCtl.GetCreditAgingReport(int.Parse(this.ddDistributorType.SelectedItem.Value), mPrincipalId
                , int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(this.Session["UserId"].ToString()), int.Parse(rbtSotBy.SelectedValue.ToString()), DateTime.Parse(txtDocmentDate.Text), DateTime.Parse(txtEndDate.Text + " 23:59:59")
                , int.Parse(drpChannelType.SelectedValue.ToString()), int.Parse(DrpOrderBooker.SelectedValue));
            SAMSBusinessLayer.Reports.CrpCreditAgingReport CrpReport = new SAMSBusinessLayer.Reports.CrpCreditAgingReport();
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();
            CrpReport.SetParameterValue("UptoDate", txtEndDate.Text);
            CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
            CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
            CrpReport.SetParameterValue("ReportType", rbtSotBy.SelectedValue.ToString());
            CrpReport.SetParameterValue("ChannelType", drpChannelType.SelectedItem.Text);
            string path = SAMSCommon.Classes.Configuration.GetAppInstallationPath() + "\\ExportedFile.xls";
            CrpReport.SetDatabaseLogon("sa", "Laislabonitamac2065");
            CrpReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.Excel, path);
            System.IO.FileInfo file = new System.IO.FileInfo(path);
            if (file.Exists)
            {
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.ContentType = "application/octet-stream";
                Response.WriteFile(file.FullName);
                Response.End();
            }
            else
            {
                Response.Write("This file does not exist.");
            }
        }
        else
        {
            ShowReport_Summary(1);
        }
        
    }

    public void ShowReport_Summary(int p_TYPE_ID)
    {
        if (PrincipalWiseSale == 0)
        {
            mPrincipalId = Constants.IntNullValue;
        }
        else
        {
            mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
        }
        ReportDocument CrpReport = new ReportDocument();
        DataTable dt = mDocumentPrntControl.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));
        DataSet ds = mDocumentPrntControl.GetCreditAgingSummary(int.Parse(drpDistributor.SelectedValue.ToString()), mPrincipalId,
               Constants.LongNullValue, DateTime.Parse(txtEndDate.Text + " 23:59:59")
                   , Convert.ToInt32(ddlSortBy.SelectedValue), Convert.ToInt32(ddlOrderBy.SelectedValue), int.Parse(Session["UserId"].ToString())
                   ,int.Parse(DrpOrderBooker.SelectedValue));

        //SAMSBusinessLayer.Reports.CrpCreditAgingSummary CrpReport = new SAMSBusinessLayer.Reports.CrpCreditAgingSummary();
        //SAMSBusinessLayer.Reports.CrpCreditAgingSummaryNew CrpReport = new SAMSBusinessLayer.Reports.CrpCreditAgingSummaryNew();
        if (RbSubTotal.SelectedValue == "2")
        {
            CrpReport = new SAMSBusinessLayer.Reports.CrpCreditAgingSummaryOrderBookerWise();
        }
        else
        {
            CrpReport = new SAMSBusinessLayer.Reports.CrpCreditAgingSummary();
        }

        //CrpReport = new SAMSBusinessLayer.Reports.CrpCreditAgingSummaryOrderBookerWise();
        CrpReport.SetDataSource(ds);
        CrpReport.Refresh();
        CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
        CrpReport.SetParameterValue("Location", drpDistributor.SelectedItem.Text);
        CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
        CrpReport.SetParameterValue("ReportType", RbSubTotal.SelectedValue);
        CrpReport.SetParameterValue("Customer", DrpOrderBooker.SelectedItem.Text);
        CrpReport.SetParameterValue("Date_To", txtEndDate.Text);
        this.Session.Add("CrpReport", CrpReport);
        this.Session.Add("ReportType", p_TYPE_ID);
        string url = "'Default.aspx'";
        string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        Type cstype = this.GetType();
        ClientScriptManager cs = Page.ClientScript;
        cs.RegisterStartupScript(cstype, "OpenWindow", script);
    }

    /// <summary>
    /// Sets Visibility of Controls as Per Rpeort Type
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void rbtSotBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadAssingned();
        if (int.Parse(rbtSotBy.SelectedValue.ToString()) == 2)
        {
            //txtDocmentDate.Visible = true;
            //ibtnStartDate.Visible = true;
            //Label3.Visible = true;
            PanelStartDate.Visible = true;
            Label4.Text = "End Date";
        }
        else
        {
            //txtDocmentDate.Visible = false;
            //ibtnStartDate.Visible = false;
            //Label3.Visible = false;
            PanelStartDate.Visible = false;
            Label4.Text = "As On Date";
        }
    }

    protected void RbtReportType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (RbtReportType.SelectedValue == "1")
        {
            PenalReportType.Visible = true;
            PanelSortandOrder.Visible = false;
            PanelOrderBooker.Visible = false;
            PanelSubTotal.Visible = false;
        }
        else
        {
            PenalReportType.Visible = false;
            PanelSortandOrder.Visible = true;
            PanelOrderBooker.Visible = true;
            PanelSubTotal.Visible = true;
        }
    }
}
