using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using SAMSBusinessLayer.Reports;
using System.Linq;

/// <summary>
/// Form For Booking vs Execution Report
/// </summary>
public partial class Forms_RptBookingSupplyMonitoring : System.Web.UI.Page
{
    /// <summary>
    /// Page_Load Function
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    bool ViewAccess;
    UserController mUserController = new UserController();
    public static int PrincipalWiseSale = 0;
    private static int mPrincipalId;
    protected void Page_Load(object sender, EventArgs e)
    {
        ViewAccess = ValidateScreen();
        if (ViewAccess == false)
        {
            Response.Redirect("../Forms/AccessDenied.aspx");
        }
        if (!Page.IsPostBack)
        {
            Configration();
            LoadPrincipal();
            LoadLocation();
            LoadSalesForce();
            this.LoadDeliveryman();
            SAMSCommon.Classes.Configuration.SystemCurrentDateTime = (DateTime)Session["CurrentWorkDate"];
            txtFromDate.Text = SAMSCommon.Classes.Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtToDate.Text = SAMSCommon.Classes.Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
        }
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
    /// Loads Principals To Principal Combo
    /// </summary>
    protected void LoadPrincipal()
    {
        try
        {
            SKUPriceDetailController PController = new SKUPriceDetailController();
            DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
            this.drpPrincipal.DataSource = m_dt;
            this.drpPrincipal.DataTextField = "Company_Name";
            this.drpPrincipal.DataValueField = "Company_Id";
            this.drpPrincipal.DataBind();
        }
        catch (Exception ex)
        {
            ex.ToString();
        }
    }

    /// <summary>
    /// Loads Locations To Location Combo
    /// </summary>
    protected void LoadLocation()
    {
        try
        {
            DistributorController DController = new DistributorController();
            DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
            this.DrpLocation.DataSource = dt;
            this.DrpLocation.DataTextField = "DISTRIBUTOR_NAME";
            this.DrpLocation.DataValueField = "DISTRIBUTOR_ID";
            this.DrpLocation.DataBind();
        }
        catch (Exception ex)
        {
            ex.ToString();
        }
    }

    /// <summary>
    /// Loads Order Bookers To OrderBooker Combo
    /// </summary>
    protected void LoadSalesForce()
    {
        SaleForceController ds = new SaleForceController();
        DataTable dt = ds.SelectSaleForceAssignedArea(Constants.SALES_FORCE_ORDERBOOKER, int.Parse(DrpLocation.SelectedValue.ToString()), Constants.IntNullValue, int.Parse(this.Session["CompanyId"].ToString()), Convert.ToInt32(drpPrincipal.SelectedValue));
        if (dt.Rows.Count > 0)
        {
            this.drpSaleForce.Items.Clear();
            this.drpSaleForce.Items.Add(new ListItem("All", "0"));
            clsWebFormUtil.FillDropDownList(this.drpSaleForce, dt, "USER_ID", "USER_NAME");
        }
        else
        {
            this.drpSaleForce.Items.Clear();
        }
    }

    /// <summary>
    /// Loads Deliverymen To Deliverman Combo
    /// </summary>
    private void LoadDeliveryman()
    {
        this.DrpDeliveryMan.Items.Clear();
        if (DrpLocation.Items.Count > 0)
        {
            SaleForceController mDController = new SaleForceController();
            DataTable m_dt = mDController.SelectSaleForceAssignedArea(int.Parse(DrpLocation.SelectedValue.ToString()), Constants.IntNullValue, int.Parse(this.Session["CompanyId"].ToString()));
            DrpDeliveryMan.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
            clsWebFormUtil.FillDropDownList(this.DrpDeliveryMan, m_dt, 0, 3);
        }
    }

    /// <summary>
    /// Loads Deliverymen And OrderBookers
    /// </summary>
    protected void DrpLocation_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSalesForce();
        this.LoadDeliveryman();
    }

    /// <summary>
    /// Shows Booking vs Execution Either in PDF Or in Excel
    /// </summary>
    /// <param name="p_Report_Type">ReportType</param>
    private void ShowReport(int p_Report_Type)
    {
        try
        {
            if (PrincipalWiseSale == 0)
            {
                mPrincipalId = Constants.IntNullValue;
            }
            else
            {
                mPrincipalId = int.Parse(drpPrincipal.SelectedValue.ToString());
            }
            RptCustomerController RptCustomerCtl = new RptCustomerController();
            if (DrpReportType.SelectedIndex == 0)
            {
                SAMSBusinessLayer.Classes.DocumentPrintController DPrint = new SAMSBusinessLayer.Classes.DocumentPrintController();

                DataTable dt = DPrint.SelectReportTitle(int.Parse(DrpLocation.SelectedValue.ToString()));

                string FromDate = null;
                string ToDate = null;
                DataSet ds = new DataSet();

                SAMSBusinessLayer.Reports.CrpBookingSupplyMonitoring CrpReport = new SAMSBusinessLayer.Reports.CrpBookingSupplyMonitoring();

                DateTime parsed_date_fromdate = DateTime.Parse(this.txtFromDate.Text);
                DateTime parsed_date_todate = DateTime.Parse(this.txtToDate.Text);
                FromDate = parsed_date_fromdate.ToShortDateString();
                ToDate = parsed_date_todate.ToShortDateString();
                ds = RptCustomerCtl.BookingVsExecution(mPrincipalId, int.Parse(DrpLocation.SelectedValue.ToString()), int.Parse(this.drpSaleForce.SelectedValue.ToString()),Convert.ToInt32(DrpDeliveryMan.SelectedValue), Convert.ToDateTime(FromDate + " 00:00:00"), Convert.ToDateTime(ToDate + " 00:00:00"));

                CrpReport.SetDataSource(ds);
                CrpReport.Refresh();

                CrpReport.SetParameterValue("Branch", this.DrpLocation.SelectedItem.Text.ToString());
                CrpReport.SetParameterValue("Principal", this.drpPrincipal.SelectedItem.Text.ToString());
                CrpReport.SetParameterValue("FromDate", this.txtFromDate.Text);
                CrpReport.SetParameterValue("ToDate", this.txtToDate.Text);
                CrpReport.SetParameterValue("OrderBooker", this.drpSaleForce.SelectedItem.Text.ToString());
                CrpReport.SetParameterValue("DELIVERYMAN", this.DrpDeliveryMan.SelectedItem.Text);
                CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());

                this.Session.Add("CrpReport", CrpReport);
                this.Session.Add("ReportType", p_Report_Type);
                string url = "'Default.aspx'";
                string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(cstype, "OpenWindow", script);
            }
            else //if (DrpReportType.SelectedIndex == 1)
            {
                SAMSBusinessLayer.Classes.DocumentPrintController DPrint = new SAMSBusinessLayer.Classes.DocumentPrintController();

                DataTable dt = DPrint.SelectReportTitle(int.Parse(DrpLocation.SelectedValue.ToString()));

                string FromDate = null;
                string ToDate = null;
                DataSet ds = new DataSet();

                SAMSBusinessLayer.Reports.CrpBookingVsExecutionSKUWise CrpReport = new SAMSBusinessLayer.Reports.CrpBookingVsExecutionSKUWise();

                DateTime parsed_date_fromdate = DateTime.Parse(this.txtFromDate.Text);
                DateTime parsed_date_todate = DateTime.Parse(this.txtToDate.Text);
                FromDate = parsed_date_fromdate.ToShortDateString();
                ToDate = parsed_date_todate.ToShortDateString();
                if (DrpReportType.SelectedIndex == 3)
                {
                    ds = RptCustomerCtl.SKUWiseBookingVsExecution(mPrincipalId, int.Parse(DrpLocation.SelectedValue.ToString()), int.Parse(this.drpSaleForce.SelectedValue.ToString()),Convert.ToInt32(DrpDeliveryMan.SelectedValue), Convert.ToDateTime(FromDate + " 00:00:00"), Convert.ToDateTime(ToDate + " 00:00:00"), Constants.IntNullValue);
                }
                else
                {
                    ds = RptCustomerCtl.SKUWiseBookingVsExecution(mPrincipalId, int.Parse(DrpLocation.SelectedValue.ToString()), int.Parse(this.drpSaleForce.SelectedValue.ToString()), Convert.ToInt32(DrpDeliveryMan.SelectedValue), Convert.ToDateTime(FromDate + " 00:00:00"), Convert.ToDateTime(ToDate + " 00:00:00"), 0);
                }
                CrpReport.SetDataSource(ds);
                CrpReport.Refresh();

                CrpReport.SetParameterValue("Branch", this.DrpLocation.SelectedItem.Text.ToString());
                CrpReport.SetParameterValue("Principal", this.drpPrincipal.SelectedItem.Text.ToString());
                CrpReport.SetParameterValue("FromDate", this.txtFromDate.Text);
                CrpReport.SetParameterValue("ToDate", this.txtToDate.Text);
                CrpReport.SetParameterValue("OrderBooker", this.drpSaleForce.SelectedItem.Text.ToString());
                CrpReport.SetParameterValue("DELIVERYMAN", this.DrpDeliveryMan.SelectedItem.Text);
                CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());

                if (DrpReportType.SelectedIndex == 1)
                {
                    CrpReport.SetParameterValue("ReportTitle", "(Units)");
                }
                else if (DrpReportType.SelectedIndex == 2)
                {
                    CrpReport.SetParameterValue("ReportTitle", "(Cartons)");
                }
                else if (DrpReportType.SelectedIndex == 3)
                {
                    CrpReport.SetParameterValue("ReportTitle", "(Values)");
                }
                

                this.Session.Add("CrpReport", CrpReport);
                this.Session.Add("ReportType", p_Report_Type);
                string url = "'Default.aspx'";
                string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(cstype, "OpenWindow", script);
            }
        }
        catch (Exception ex)
        {
            ex.ToString();
        }
    }

    /// <summary>
    /// Shows Booking vs Execution in PDF
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewPDF_Click(object sender, EventArgs e)
    {
        ShowReport(0);
    }

    /// <summary>
    /// Shows Booking vs Execution in PDF
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewExcel(object sender, EventArgs e)
    {
        ShowReport(1);
    }

    /// <summary>
    /// Loads Order Bookers
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void drpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadSalesForce();
    }
}
