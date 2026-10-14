using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using CrystalDecisions.CrystalReports.Engine;
using System.Linq;

/// <summary>
/// Form For Cheque Detail Report
/// </summary>
public partial class Forms_RptChequeDetailReport : System.Web.UI.Page
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
            DrpStatus.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
            DrpStatus.Items.Add(new ListItem("Cheque Received", Constants.Cheque_Pending.ToString()));
            DrpStatus.Items.Add(new ListItem("Cheque Depsoit", Constants.Cheque_Deposit.ToString()));
            DrpStatus.Items.Add(new ListItem("Cheque Realize", Constants.Cheque_Clear.ToString()));
            DrpStatus.Items.Add(new ListItem("Cheque Bounce", Constants.Cheque_Bons.ToString()));
            DrpCustomer.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));

            this.LoadPrincipal();
            this.LoadDistributor();
            this.LoadArea();
            this.LoadChannelType();

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
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, m_dt, 0, 1);
    }

    /// <summary>
    /// Loads Locations To Location Combo
    /// </summary>
    private void LoadDistributor()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDropDownList(this.drpDistributor, dt, 0, 2);
    }

    /// <summary>
    /// Loads Routes To Route Combo
    /// </summary>
    private void LoadArea()
    {
        if (drpDistributor.Items.Count > 0)
        {
            DrpRoute.Items.Clear();
            DistributorAreaController mController = new DistributorAreaController();
            DataTable dt = mController.SelectDist_Area(Constants.LongNullValue, Constants.DateNullValue, Constants.DateNullValue, int.Parse(drpDistributor.SelectedValue.ToString()), Constants.IntNullValue, null, null);
            DrpRoute.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
            clsWebFormUtil.FillDropDownList(DrpRoute, dt, 0, 6);
        }
        else
        {
            DrpRoute.Items.Clear();
        }
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

    /// <summary>
    /// Shows Cheque Detail Report in PDF
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewPDF_Click(object sender, EventArgs e)
    {
        if (rblReportFor.SelectedItem.Value == "0")
        {
            ShowCHequeDetail(0);
        }
        else
        {
            ShowCustomerCredit(0);
        }
    }

    /// <summary>
    /// Loads Routes
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadArea();
    }

    /// <summary>
    /// Shows Cheque Detail Report in Excel
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewExcel_Click(object sender, EventArgs e)
    {
        if (rblReportFor.SelectedItem.Value == "0")
        {
            ShowCHequeDetail(1);
        }
        else
        {
            ShowCustomerCredit(1);
        }
    }

    protected void rblReportFor_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rblReportFor.SelectedValue == "1")
        {
            divDetail.Visible = false;
            Label2.Visible = false;
            RbReportFilter.Visible = false;
            Label4.Visible = false;
        }
        else
        {
            divDetail.Visible = true;
            Label2.Visible = true;
            RbReportFilter.Visible = true;
            Label4.Visible = true;
        }
    }

    public void ShowCHequeDetail(int Type)
    {
        ReportDocument CrpReport = new ReportDocument();
        if (PrincipalWiseSale == 0)
        {
            mPrincipalId = Constants.IntNullValue;
            CrpReport = new SAMSBusinessLayer.Reports.CrpChequeDetailReportWithoutPrincipal();
        }
        else
        {
            mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
            CrpReport = new SAMSBusinessLayer.Reports.CrpChequeDetailReport();
        }
        DocumentPrintController mController = new DocumentPrintController();
        RptCustomerController RptCustomerCtl = new RptCustomerController();
        CrpReport = new SAMSBusinessLayer.Reports.CrpChequeDetailReport();
        DataTable dt = mController.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));

        DataSet ds = RptCustomerCtl.SelectCustomerCreditReport(int.Parse(drpDistributor.SelectedValue.ToString()), mPrincipalId, int.Parse(DrpRoute.SelectedValue.ToString()),
            int.Parse(DrpCustomer.SelectedValue.ToString()), int.Parse(drpChannelType.SelectedValue.ToString()), int.Parse(DrpStatus.SelectedValue.ToString()), DateTime.Parse(txtStartDate.Text), DateTime.Parse(txtEndDate.Text), int.Parse(this.Session["UserId"].ToString()), RbReportFilter.SelectedIndex);
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
        CrpReport.SetParameterValue("from_date", txtStartDate.Text);
        CrpReport.SetParameterValue("To_date", txtEndDate.Text);
        CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());

        this.Session.Add("CrpReport", CrpReport);
        this.Session.Add("ReportType", Type);
        string url = "'Default.aspx'";
        string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        Type cstype = this.GetType();
        ClientScriptManager cs = Page.ClientScript;
        cs.RegisterStartupScript(cstype, "OpenWindow", script);
    }

    public void ShowCustomerCredit(int p_Type)
    {
        ReportDocument CrpReport = new ReportDocument();
        if (PrincipalWiseSale == 0)
        {
            mPrincipalId = Constants.IntNullValue;
            CrpReport = new SAMSBusinessLayer.Reports.CrpCustomerCreditWithoutPrincipal();
        }
        else
        {
            mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
            CrpReport = new SAMSBusinessLayer.Reports.CrpCustomerCredit();
        }
        RptCustomerController RptCustomerCtl = new RptCustomerController();
        DocumentPrintController mController = new DocumentPrintController();
        DataTable dt = mController.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));


        DataSet ds = RptCustomerCtl.GetCustomerCredit(int.Parse(drpDistributor.SelectedValue.ToString()), mPrincipalId, DateTime.Parse(txtStartDate.Text), DateTime.Parse(txtStartDate.Text));
        CrpReport.SetDataSource(ds);
        CrpReport.Refresh();

        CrpReport.SetParameterValue("Distributor", drpDistributor.SelectedItem.Text);
        if (PrincipalWiseSale == 0)
        {
            CrpReport.SetParameterValue("Principal", "");
        }
        else
        {
            CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
        }
        //CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
        CrpReport.SetParameterValue("From_date", txtStartDate.Text);
        CrpReport.SetParameterValue("To_Date", txtEndDate.Text);
        CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
        this.Session.Add("CrpReport", CrpReport);
        this.Session.Add("ReportType", p_Type);
        string url = "'Default.aspx'";
        string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        Type cstype = this.GetType();
        ClientScriptManager cs = Page.ClientScript;
        cs.RegisterStartupScript(cstype, "OpenWindow", script);
    }
}
