using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using SAMSBusinessLayer.Reports;
using System.Linq;

/// <summary>
/// Form For Target Vs Achievement Report
/// </summary>
public partial class Forms_RptVolumeSale : System.Web.UI.Page
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
            LoadRegion();
            LoadPrincipal();
            LoadLocation();
            this.LoadTargetForId();
            DateTime dt = (DateTime)this.Session["CurrentWorkDate"];
            this.txtDate.Text = dt.ToString("MMM-yyyy");
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
    /// Loads Regions
    /// </summary>
    protected void LoadRegion()
    {
        GeoHierarchyController GControler = new GeoHierarchyController();
        DataTable dt = GControler.SelectGeoHierarchy(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, null, true, Constants.IntNullValue, Constants.Region, Constants.IntNullValue, Constants.DateNullValue, Constants.DateNullValue, int.Parse(this.Session["CompanyId"].ToString()));
        DrpRegion.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
        clsWebFormUtil.FillDropDownList(this.DrpRegion, dt, 0, 4);
    }

    /// <summary>
    /// Loads Principals To Principal Combo
    /// </summary>
    protected void LoadPrincipal()
    {
        SKUPriceDetailController PController = new SKUPriceDetailController();
        DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        drpPrincipal.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
        clsWebFormUtil.FillDropDownList(this.drpPrincipal, m_dt, 0, 1);
        
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
            this.DrpLocation.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
            clsWebFormUtil.FillDropDownList(this.DrpLocation, dt, 0, 2);
                
        }
        catch(Exception ex)
        {
            ex.ToString();
        }
    }

    private void LoadTargetForId()
    {
        this.DrpTargetFor.Items.Clear();
        if (DrpTargetType.SelectedIndex == 0)
        {
            if (DrpLocation.Items.Count > 0)
            {
                Distributor_UserController Du = new Distributor_UserController();
                DataTable dt = Du.SelectDistributorUser(Constants.SALES_FORCE_ORDERBOOKER, int.Parse(DrpLocation.SelectedValue.ToString()), int.Parse(this.Session["CompanyId"].ToString()));
                this.DrpTargetFor.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
                clsWebFormUtil.FillDropDownList(DrpTargetFor, dt, 0, 6);
            }
        }
        else if (DrpTargetType.SelectedIndex == 1)
        {
            if (DrpLocation.Items.Count > 0)
            {
                Distributor_UserController Du = new Distributor_UserController();
                DataTable dt = Du.SelectDistributorUser(Constants.SALES_FORCE_SALESPERSON, int.Parse(DrpLocation.SelectedValue.ToString()), int.Parse(this.Session["CompanyId"].ToString()));
                this.DrpTargetFor.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
                clsWebFormUtil.FillDropDownList(DrpTargetFor, dt, 0, 6);
            }
        }
        else
        {
            if (DrpLocation.Items.Count > 0)
            {
                DistributorTownController gController = new DistributorTownController();
                DataTable dt = gController.SelectAssignTown(Constants.IntNullValue, int.Parse(DrpLocation.SelectedValue.ToString()), 1);
                this.DrpTargetFor.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
                clsWebFormUtil.FillDropDownList(DrpTargetFor, dt, 0, 1);
            }
        }
    }

    /// <summary>
    /// Loads Locations
    /// </summary>
    protected void DrpRegion_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadLocation();
        this.LoadTargetForId();
    }

    /// <summary>
    /// Shows Target Vs Achievement in PDF
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewPDF_Click(object sender, EventArgs e)
    {
        try
        {
            if (RdoTargetType.SelectedIndex == 0)    // value wise
            {
                SAMSBusinessLayer.Classes.DocumentPrintController DPrint = new SAMSBusinessLayer.Classes.DocumentPrintController();
                RptSaleController RptSaleCtl = new RptSaleController();

                DataTable dt = DPrint.SelectReportTitle(int.Parse(DrpLocation.SelectedValue.ToString()));

                SAMSBusinessLayer.Reports.CrpVolumeSalesDetail CrpReport = new SAMSBusinessLayer.Reports.CrpVolumeSalesDetail();

                DataSet ds = null;

                ds = RptSaleCtl.SelectVolumeSale(int.Parse(drpPrincipal.SelectedValue.ToString()), int.Parse(DrpLocation.SelectedValue.ToString()), Convert.ToDateTime(txtDate.Text), Convert.ToInt32(DrpTargetType.SelectedValue), Convert.ToInt32(DrpTargetFor.SelectedValue), 0, int.Parse(this.Session["UserId"].ToString()));
                CrpReport.SetDataSource(ds);
                CrpReport.Refresh();

                CrpReport.SetParameterValue("Location", this.DrpLocation.SelectedItem.Text.ToString());
                CrpReport.SetParameterValue("Principal", this.drpPrincipal.SelectedItem.Text.ToString());
                CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
                CrpReport.SetParameterValue("TargetDate", txtDate.Text);

                this.Session.Add("CrpReport", CrpReport);
                this.Session.Add("ReportType", 0);
                string url = "'Default.aspx'";
                string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(cstype, "OpenWindow", script);
            }
            else if (RdoTargetType.SelectedIndex == 1)    // SKU Wise
            {
                string TargetFar = string.Empty;
                if(DrpTargetType.SelectedValue == "96")
                {
                    TargetFar = "Order Booker : " + DrpTargetFor.SelectedItem.Text;
                }
                else if (DrpTargetType.SelectedValue == "97")
                {
                    TargetFar = "Sale Person : " + DrpTargetFor.SelectedItem.Text;
                }
                else
                {
                    TargetFar = "Town : " + DrpTargetFor.SelectedItem.Text;
                }
                SAMSBusinessLayer.Classes.DocumentPrintController DPrint = new SAMSBusinessLayer.Classes.DocumentPrintController();
                RptSaleController RptSaleCtl = new RptSaleController();
                DataTable dt = DPrint.SelectReportTitle(int.Parse(DrpLocation.SelectedValue.ToString()));
                SAMSBusinessLayer.Reports.CrpVolumeSalesDetailSkuWise CrpReport = new SAMSBusinessLayer.Reports.CrpVolumeSalesDetailSkuWise();
                DataSet ds = null;
                ds = RptSaleCtl.SelectVolumeSaleSkuWise(int.Parse(drpPrincipal.SelectedValue.ToString()), int.Parse(DrpLocation.SelectedValue.ToString()), Convert.ToDateTime(txtDate.Text), Convert.ToInt32(DrpTargetType.SelectedValue), Convert.ToInt32(DrpTargetFor.SelectedValue), 0);
                CrpReport.SetDataSource(ds);
                CrpReport.Refresh();

                CrpReport.SetParameterValue("Location", this.DrpLocation.SelectedItem.Text.ToString());
                CrpReport.SetParameterValue("Principal", this.drpPrincipal.SelectedItem.Text.ToString());
                CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
                CrpReport.SetParameterValue("TargetDate", txtDate.Text);
                CrpReport.SetParameterValue("TargetFar", TargetFar);

                this.Session.Add("CrpReport", CrpReport);
                this.Session.Add("ReportType", 0);
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
    /// Shows Target Vs Achievement in Excel
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewExcel_Click(object sender, EventArgs e)
    {
        try
        {
            if (RdoTargetType.SelectedIndex == 0)    // value wise
            {
                SAMSBusinessLayer.Classes.DocumentPrintController DPrint = new SAMSBusinessLayer.Classes.DocumentPrintController();
                RptSaleController RptSaleCtl = new RptSaleController();

                DataTable dt = DPrint.SelectReportTitle(int.Parse(DrpLocation.SelectedValue.ToString()));

                SAMSBusinessLayer.Reports.CrpVolumeSalesDetail CrpReport = new SAMSBusinessLayer.Reports.CrpVolumeSalesDetail();

                DataSet ds = null;

                ds = RptSaleCtl.SelectVolumeSale(int.Parse(drpPrincipal.SelectedValue.ToString()), int.Parse(DrpLocation.SelectedValue.ToString()), Convert.ToDateTime(txtDate.Text), Convert.ToInt32(DrpTargetType.SelectedValue), Convert.ToInt32(DrpTargetFor.SelectedValue), 0, int.Parse(this.Session["UserId"].ToString()));
                CrpReport.SetDataSource(ds);
                CrpReport.Refresh();

                CrpReport.SetParameterValue("Location", this.DrpLocation.SelectedItem.Text.ToString());
                CrpReport.SetParameterValue("Principal", this.drpPrincipal.SelectedItem.Text.ToString());
                CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
                CrpReport.SetParameterValue("TargetDate", txtDate.Text);

                this.Session.Add("CrpReport", CrpReport);
                this.Session.Add("ReportType", 1);
                string url = "'Default.aspx'";
                string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(cstype, "OpenWindow", script);
            }
            else if (RdoTargetType.SelectedIndex == 1)    // SKU Wise
            {
                string TargetFar = string.Empty;
                if (DrpTargetType.SelectedValue == "96")
                {
                    TargetFar = "Order Booker : " + DrpTargetFor.SelectedItem.Text;
                }
                else if (DrpTargetType.SelectedValue == "97")
                {
                    TargetFar = "Sale Person : " + DrpTargetFor.SelectedItem.Text;
                }
                else
                {
                    TargetFar = "Town : " + DrpTargetFor.SelectedItem.Text;
                }
                SAMSBusinessLayer.Classes.DocumentPrintController DPrint = new SAMSBusinessLayer.Classes.DocumentPrintController();
                RptSaleController RptSaleCtl = new RptSaleController();
                DataTable dt = DPrint.SelectReportTitle(int.Parse(DrpLocation.SelectedValue.ToString()));
                SAMSBusinessLayer.Reports.CrpVolumeSalesDetailSkuWise CrpReport = new SAMSBusinessLayer.Reports.CrpVolumeSalesDetailSkuWise();
                DataSet ds = null;
                ds = RptSaleCtl.SelectVolumeSaleSkuWise(int.Parse(drpPrincipal.SelectedValue.ToString()), int.Parse(DrpLocation.SelectedValue.ToString()), Convert.ToDateTime(txtDate.Text), Convert.ToInt32(DrpTargetType.SelectedValue), Convert.ToInt32(DrpTargetFor.SelectedValue), 0);
                CrpReport.SetDataSource(ds);
                CrpReport.Refresh();

                CrpReport.SetParameterValue("Location", this.DrpLocation.SelectedItem.Text.ToString());
                CrpReport.SetParameterValue("Principal", this.drpPrincipal.SelectedItem.Text.ToString());
                CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
                CrpReport.SetParameterValue("TargetDate", txtDate.Text);
                CrpReport.SetParameterValue("TargetFar", TargetFar);

                this.Session.Add("CrpReport", CrpReport);
                this.Session.Add("ReportType", 1);
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


        //try
        //{
        //    SAMSBusinessLayer.Classes.DocumentPrintController DPrint = new SAMSBusinessLayer.Classes.DocumentPrintController();
        //    RptSaleController RptSaleCtl = new RptSaleController();

        //    DataTable dt = DPrint.SelectReportTitle(int.Parse(DrpLocation.SelectedValue.ToString()));

        //    SAMSBusinessLayer.Reports.CrpVolumeSalesDetail CrpReport = new SAMSBusinessLayer.Reports.CrpVolumeSalesDetail();

        //    DataSet ds = null;

        //    ds = RptSaleCtl.SelectVolumeSaleSkuWise(int.Parse(drpPrincipal.SelectedValue.ToString()), int.Parse(DrpLocation.SelectedValue.ToString()), Convert.ToDateTime(txtDate.Text), Convert.ToInt32(DrpTargetType.SelectedValue), Convert.ToInt32(DrpTargetFor.SelectedValue), 0);
        //    CrpReport.SetDataSource(ds);
        //    CrpReport.Refresh();

        //    CrpReport.SetParameterValue("Location", this.DrpLocation.SelectedItem.Text.ToString());
        //    CrpReport.SetParameterValue("Principal", this.drpPrincipal.SelectedItem.Text.ToString());
        //    CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
        //    CrpReport.SetParameterValue("TargetDate", txtDate.Text);

        //    this.Session.Add("CrpReport", CrpReport);
        //    this.Session.Add("ReportType", 1);
        //    string url = "'Default.aspx'";
        //    string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        //    Type cstype = this.GetType();
        //    ClientScriptManager cs = Page.ClientScript;
        //    cs.RegisterStartupScript(cstype, "OpenWindow", script);
        //}
        //catch (Exception ex)
        //{
        //    ex.ToString();
        //}
    }

    protected void DrpTargetType_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadTargetForId();
    }

    protected void DrpLocation_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadTargetForId();
    }
}