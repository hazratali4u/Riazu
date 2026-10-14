using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using CrystalDecisions.CrystalReports.Engine;
using System.Linq;

/// <summary>
/// Form For Customer Invoice Wise Sales Report
/// </summary>
public partial class Forms_RptCustomerInvoiceReport : System.Web.UI.Page
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
            this.LoadPrincipal();
            this.LoadDistributor();

            SAMSCommon.Classes.Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            this.txtStartDate.Text = SAMSCommon.Classes.Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            this.txtEndDate.Text = SAMSCommon.Classes.Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
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
        drpDistributor.Items.Clear();
   
        if (RblCustomer.SelectedIndex == 1)
        {
            DistributorController DController = new DistributorController();
            DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
            drpDistributor.Items.Add(new ListItem("All", "0"));
            clsWebFormUtil.FillDropDownList(this.drpDistributor, dt, 0, 2);
        }
        else
        {
            DistributorController DController = new DistributorController();
            DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
            clsWebFormUtil.FillDropDownList(this.drpDistributor, dt, 0, 2);
        }
    }

    /// <summary>
    /// Shows Customer Invoice Wise Sales Either in PDF Or in Excel
    /// </summary>
    /// <param name="p_Report_Type">ReportType</param>
    private void ShowReport(int p_Report_Type)
    {
        string CustomerId = "0";
        string DistributorId = "0";

        DocumentPrintController mDocumentPrntControl = new DocumentPrintController();
        RptCustomerController RptCustomerCtl = new RptCustomerController();
        
        if (RblCustomer.SelectedIndex == 0)
        {
            ReportDocument CrpReport = new ReportDocument();
            if (PrincipalWiseSale == 0)
            {
                mPrincipalId = Constants.IntNullValue;
                CrpReport = new SAMSBusinessLayer.Reports.CrpBillCustomerWiseReportWithoutPrincipal();
            }
            else
            {
                mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
                CrpReport = new SAMSBusinessLayer.Reports.CrpBillCustomerWiseReport();
            }
            DataSet ds = RptCustomerCtl.SelectBillCustomerDetail2(mPrincipalId, CustomerId, drpDistributor.SelectedValue.ToString(), DateTime.Parse(this.txtStartDate.Text + " 00:00:00"), DateTime.Parse(this.txtEndDate.Text + " 23:59:59"), 0);
            DataTable dt = mDocumentPrntControl.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));

            
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();

            if (PrincipalWiseSale == 0)
            {
                CrpReport.SetParameterValue("PRINCIPAL", "");
            }
            else
            {
                CrpReport.SetParameterValue("PRINCIPAL", DrpPrincipal.SelectedItem.Text);
            }
            //CrpReport.SetParameterValue("PRINCIPAL", DrpPrincipal.SelectedItem.Text);
            CrpReport.SetParameterValue("FROM_DATE", txtStartDate.Text);
            CrpReport.SetParameterValue("TO_DATE", txtEndDate.Text);
            CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());

            this.Session.Add("CrpReport", CrpReport);
            this.Session.Add("ReportType", p_Report_Type);
            string url = "'Default.aspx'";
            string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterStartupScript(cstype, "OpenWindow", script);
        }
        else
        {
            ReportDocument CrpReport = new ReportDocument();
            if (PrincipalWiseSale == 0)
            {
                mPrincipalId = Constants.IntNullValue;
                CrpReport = new SAMSBusinessLayer.Reports.CrpBillCustomerWiseReportWithoutPrincipal();
            }
            else
            {
                mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
                CrpReport = new SAMSBusinessLayer.Reports.CrpBillCustomerWiseReport();
            }
            foreach (GridViewRow dr in Grid_users.Rows)
            {
                CheckBox chbSelect = (CheckBox)(dr.Cells[0].FindControl("ChbCustomer"));
                if (chbSelect.Checked)
                {
                    CustomerId = CustomerId + "," + dr.Cells[1].Text;
                    DistributorId = DistributorId + "," + dr.Cells[2].Text;
                }
            }

            DataSet ds = RptCustomerCtl.SelectBillCustomerDetail2(mPrincipalId, CustomerId, DistributorId, DateTime.Parse(this.txtStartDate.Text + " 00:00:00"), DateTime.Parse(this.txtEndDate.Text + " 23:59:59"), 1);

            DataTable dt = mDocumentPrntControl.SelectReportTitle(Constants.IntNullValue);

            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();

            if (PrincipalWiseSale == 0)
            {
                CrpReport.SetParameterValue("PRINCIPAL", "");
            }
            else
            {
                CrpReport.SetParameterValue("PRINCIPAL", DrpPrincipal.SelectedItem.Text);
            }
            //CrpReport.SetParameterValue("PRINCIPAL", DrpPrincipal.SelectedItem.Text);
            CrpReport.SetParameterValue("FROM_DATE", txtStartDate.Text);
            CrpReport.SetParameterValue("TO_DATE", txtEndDate.Text);
            CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());

            this.Session.Add("CrpReport", CrpReport);
            this.Session.Add("ReportType", p_Report_Type);
            string url = "'Default.aspx'";
            string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterStartupScript(cstype, "OpenWindow", script);
        }
    }

    /// <summary>
    /// Shows Customer Invoice Wise Sales in PDF
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void BtnViewPdf_Click(object sender, EventArgs e)
    {
        ShowReport(0);
        
    }

    /// <summary>
    /// Shows Customer Invoice Wise Sales in Excel
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewExcel_Click(object sender, EventArgs e)
    {
        ShowReport(1);
    }

    /// <summary>
    /// Loads Locations
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void RblCustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadDistributor();
    }

    /// <summary>
    /// Loads Customers To Customer Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnFilter_Click(object sender, EventArgs e)
    {
        CustomerDataController mController = new CustomerDataController();
        DataTable dt = mController.UspSelectCustomer(int.Parse(this.Session["UserId"].ToString()), int.Parse(drpDistributor.SelectedValue.ToString()), ddSearchType.SelectedValue.ToString(), txtSeach.Text);
        this.Grid_users.DataSource = dt;
        this.Grid_users.DataBind();
    }
}
