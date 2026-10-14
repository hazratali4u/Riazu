using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using SAMSBusinessLayer.Reports;
using CrystalDecisions.CrystalReports.Engine;
using System.Linq;

/// <summary>
/// Form For Customer Ledger Report
/// </summary>
public partial class Forms_RptCustomerLedger : System.Web.UI.Page
{
    UserController mUserController = new UserController();
    public int PrincipalWiseSale = 0;
    private static int mPrincipalId;
    public static string opType = "CR";
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
            this.LoadArea();
            this.LoadData();

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
        if (drpDistributor.Items.Count > 0 && DrpRoute.Items.Count > 0)
        {
            CustomerDataController mController = new CustomerDataController();
            DataTable dt = mController.SelectAllAssignedCustomer(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpRoute.SelectedValue.ToString()), Constants.IntNullValue,int.Parse(this.Session["UserID"].ToString()));
            clsWebFormUtil.FillDropDownList(this.DrpCustomer, dt, 0, 17, true);            
        }
    }

    /// <summary>
    /// Gets Customer Opening Balance
    /// </summary>
    private decimal  LoadCustomerOpBalance()
    {
        decimal OpeningBalance = 0;
        if (drpDistributor.Items.Count > 0 && DrpCustomer.Items.Count > 0)
        {
            if (PrincipalWiseSale == 0)
            {
                mPrincipalId = Constants.IntNullValue;
            }
            else
            {
                mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
            }
            CustomerDataController mController = new CustomerDataController();
            DataTable dt = mController.SelectPrincipalCustomerOpBalance(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpCustomer.SelectedValue.ToString()), DateTime.Parse(txtStartDate.Text + " 00:00:00"), mPrincipalId);
            //return decimal.Parse(dt.Rows[0][0].ToString());
            //hfOpeningBal.Value = dt.Rows[0][0].ToString();

            if (Convert.ToDecimal(dt.Rows[0][0]) < 0)
            {
                //hfOpeningBal.Value = Convert.ToString(Math.Abs(Convert.ToDecimal(dt.Rows[0][0])));
                opType = "DR";
            }
            else if (Convert.ToDecimal(dt.Rows[0][0]) == 0)
            {
                opType = "DR";
            }
            else
            {
                opType = "CR";
            }
            return decimal.Parse(dt.Rows[0][0].ToString());
        }
        return 0;
    }

    /// <summary>
    /// Loads Principals To Principal Combo
    /// </summary>
    private void LoadPrincipal()
    {
        DrpPrincipal.Items.Clear();
            SKUPriceDetailController PController = new SKUPriceDetailController();
        DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        DrpPrincipal.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, m_dt, 0, 1, false);
    }

    /// <summary>
    /// Loads Routes And Customers
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadArea();
        this.LoadData();
    }

    /// <summary>
    /// Loads Customers
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpRoute_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadData();
    }

    /// <summary>
    /// Shows Customer Ledger Either in PDF or in Excel
    /// </summary>
    /// <param name="p_Report_Type">ReportType</param>
    private void ShowReport(int p_Report_Type)
    {
        //if (int.Parse (DrpPrincipal.SelectedValue) != Constants.IntNullValue)
        {
            ReportDocument CrpReport = new ReportDocument();
            ReportDocument SubReport = new ReportDocument();
            //if (PrincipalWiseSale == 0)
            //{
            //    mPrincipalId = Constants.IntNullValue;
            //}
            //else
            //{
            //    mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
            //}
            DataControl dc = new DataControl();
            SAMSBusinessLayer.Classes.DocumentPrintController DPrint = new SAMSBusinessLayer.Classes.DocumentPrintController();
            RptCustomerController RptCustomerCtl = new RptCustomerController();
            DataSet ds;
            ds = RptCustomerCtl.CustomerLedger_View(int.Parse(DrpPrincipal.SelectedValue), int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpCustomer.SelectedValue.ToString()),
                      DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"));

            DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));

            if (PrincipalWiseSale == 0)
            {
                CrpReport = new SAMSBusinessLayer.Reports.CrpCustomerLedger1WithoutPrincipal();
            }
            else
            {
                CrpReport = new SAMSBusinessLayer.Reports.CrpCustomerLedger1();
            }
            
            

            SubReport = CrpReport.OpenSubreport("srCustomerLedger");
            CrpReport.SetDataSource(ds);
            SubReport.SetDataSource(ds);
            CrpReport.Refresh();

            CrpReport.SetParameterValue("FromDate", DateTime.Parse(txtStartDate.Text));
            CrpReport.SetParameterValue("To_date", DateTime.Parse(txtEndDate.Text));
            CrpReport.SetParameterValue("Location", drpDistributor.SelectedItem.Text);
            if (PrincipalWiseSale == 0)
            {
                CrpReport.SetParameterValue("Principal", "");
            }
            else
            {
                CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
            }
            //CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
            CrpReport.SetParameterValue("Town_Name", DrpRoute.SelectedItem.Text);
            CrpReport.SetParameterValue("Customer", DrpCustomer.SelectedItem.Text);
            CrpReport.SetParameterValue("Op_Balance", LoadCustomerOpBalance());
            CrpReport.SetParameterValue("opType", opType);
            CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());

            this.Session.Add("CrpReport", CrpReport);
            this.Session.Add("ReportType", p_Report_Type);
            string url = "'Default.aspx'";
            string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterStartupScript(cstype, "OpenWindow", script);
        }

        //else
        //{
        //    ReportDocument CrpReport = new ReportDocument();
        //    if (PrincipalWiseSale == 0)
        //    {
        //        mPrincipalId = Constants.IntNullValue;
        //    }
        //    else
        //    {
        //        mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
        //    }
        //    DataControl dc = new DataControl();
        //    SAMSBusinessLayer.Classes.DocumentPrintController DPrint = new SAMSBusinessLayer.Classes.DocumentPrintController();
        //    RptCustomerController RptCustomerCtl = new RptCustomerController();
        //    DataSet ds;
        //    ds = RptCustomerCtl.CustomerLedgerClosing(mPrincipalId, int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpCustomer.SelectedValue.ToString()),
        //              DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"));

        //    DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));

        //    if (PrincipalWiseSale == 0)
        //    {
        //        CrpReport = new SAMSBusinessLayer.Reports.CrpCustomerLedgerClosingWithoutPrincipal();
        //    }
        //    else
        //    {
        //        CrpReport = new SAMSBusinessLayer.Reports.CrpCustomerLedgerClosing();
        //    }

        //    CrpReport.SetDataSource(ds);
            
        //    CrpReport.Refresh();

        //    CrpReport.SetParameterValue("FromDate", DateTime.Parse(txtStartDate.Text));
        //    CrpReport.SetParameterValue("To_date", DateTime.Parse(txtEndDate.Text));
        //    CrpReport.SetParameterValue("Location", drpDistributor.SelectedItem.Text);
        //    if (PrincipalWiseSale == 0)
        //    {
        //        CrpReport.SetParameterValue("Principal", "");
        //    }
        //    else
        //    {
        //        CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
        //    }

        //    //CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
        //    CrpReport.SetParameterValue("Town_Name", DrpRoute.SelectedItem.Text);
        //    CrpReport.SetParameterValue("Customer", DrpCustomer.SelectedItem.Text);
        //    CrpReport.SetParameterValue("Op_Balance", LoadCustomerOpBalance());
        //    CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());

        //    this.Session.Add("CrpReport", CrpReport);
        //    this.Session.Add("ReportType", p_Report_Type);
        //    string url = "'Default.aspx'";
        //    string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        //    Type cstype = this.GetType();
        //    ClientScriptManager cs = Page.ClientScript;
        //    cs.RegisterStartupScript(cstype, "OpenWindow", script);
        //}
    }
    /// <summary>
    /// Shows Customer Ledger Report in PDF
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewPDF_Click(object sender, EventArgs e)
    {
        ShowReport(0);
    }

    /// <summary>
    /// Shows Customer Ledger Report in Excel
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewExcel_Click(object sender, EventArgs e)
    {
        ShowReport(1);
    }
}
