using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using SAMSBusinessLayer.Reports;
using CrystalDecisions.CrystalReports.Engine;
using System.Linq;

/// <summary>
/// Form For  Customer Wise (DSR) Report
/// </summary>
public partial class Forms_RptCustomerDSR : System.Web.UI.Page
{
    UserController mUserController = new UserController();
    SkuHierarchyController mHer_Controller = new SkuHierarchyController();
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
        if (!IsPostBack)
        {
            this.LoadDistributor();
            this.LoadAssingned();
            this.LoadPrincipal();
            this.LoadArea();
            this.LoadOrderBooker();
            this.LoadChannelType();
            this.LoadDeliveryman();
            Populate_drpSKUCategory();
            RbReportType_SelectedIndexChanged(null, null);
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
    /// Loads Order Bookers To OrderBooker Combo
    /// </summary>
    private void LoadOrderBooker()
    {
        this.DrpOrderbooker.Items.Clear();
        if (drpDistributor.Items.Count > 0 && DrpRoute.Items.Count > 0) /* && DrpPrincipal.Items.Count > 0)*/
        {
            if (PrincipalWiseSale == 0)
            {
                mPrincipalId = Constants.IntNullValue;
            }
            else
            {
                mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
            }
            SaleForceController mDController = new SaleForceController();
            DataTable m_dt = mDController.SelectSaleForceAssignedArea(Constants.SALES_FORCE_ORDERBOOKER, int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpRoute.SelectedValue.ToString()), int.Parse(this.Session["CompanyId"].ToString()), mPrincipalId);
            DrpOrderbooker.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
            clsWebFormUtil.FillDropDownList(this.DrpOrderbooker, m_dt, 0, 3);
        }
    }

    /// <summary>
    /// Loads Deliverymen To Deliverman Combo
    /// </summary>
    private void LoadDeliveryman()
    {
        this.DrpSaleForce.Items.Clear();
        if (drpDistributor.Items.Count > 0 && DrpRoute.Items.Count > 0)
        {
            SaleForceController mDController = new SaleForceController();
            DataTable m_dt = mDController.SelectSaleForceAssignedArea(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpRoute.SelectedValue.ToString()), int.Parse(this.Session["CompanyId"].ToString()));
            DrpSaleForce.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
            clsWebFormUtil.FillDropDownList(this.DrpSaleForce, m_dt, 0, 3);
        }
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
            DrpRoute.Items.Add(new ListItem("All",Constants.IntNullValue.ToString()));       
            clsWebFormUtil.FillDropDownList(DrpRoute, dt, 0, 6);
        }
        else
        {
            DrpRoute.Items.Clear();
        }
    }

    private void LoadAssingned()
    {
        ddlStockUsedBy.Items.Clear();
        if (drpDistributor.Items.Count > 0)
        {
            UserController mUserController = new UserController();
            DataTable dt = mUserController.SelectDisToDisAssignment(int.Parse(drpDistributor.SelectedValue), Constants.IntNullValue, 9, int.Parse(this.Session["CompanyId"].ToString()), Convert.ToDateTime(Session["CurrentWorkDate"]));
            clsWebFormUtil.FillDropDownList(ddlStockUsedBy, dt, 0, 2);
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
    /// Loads Order Bookers, Routes And Delivermen
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>    
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadAssingned();
        this.LoadArea();
        this.LoadDeliveryman();
        this.LoadOrderBooker();
    }

    /// <summary>
    /// Loads Order Bookers And Deliverymen
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpRoute_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadDeliveryman();
        this.LoadOrderBooker();
    }

    /// <summary>
    /// Enables/Disables Controls According To Report Type
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void RbReportType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (RbReportType.SelectedIndex == 0)
        { 
            DrpReportType.Enabled = true;
            CheckBox1.Enabled = false;
            PanelCategory.Visible = true;
            //PanelPrincipal.Visible = true;
        }
        else
        {
            DrpReportType.Enabled = false;
            CheckBox1.Enabled = true;
            PanelCategory.Visible = false;
            //PanelPrincipal.Visible = true;
        }
    }
    private void Populate_drpSKUCategory()
    {
        ListCatagory.Items.Clear();
        DataTable m_dt = mHer_Controller.SelectSKUHierarchy_ForCategories(Constants.SKUCategory, Constants.IntNullValue, int.Parse(DrpPrincipal.SelectedValue), null, null, true, int.Parse(this.Session["CompanyId"].ToString()));
        //clsWebFormUtil.FillDropDownList(this.ddskucategory, m_dt, 0, 3, true);
        //DataView dv = new DataView(m_dt);
        //dv.RowFilter = "Company_id = " + Convert.ToInt32(DrpPrincipal.SelectedValue.ToString());
        //m_dt = dv.ToTable();
        clsWebFormUtil.FillListBox(this.ListCatagory, m_dt, 0, 3);


    }

    /// <summary>
    /// Shows Customer Wise (DSR) in PDF
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewPDF_Click(object sender, EventArgs e)
    {
        string Categories_IDs = null;
        DocumentPrintController DPrint = new DocumentPrintController();
        RptCustomerController RptCustomerCtl = new RptCustomerController();
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));
        ReportDocument CrpReport = new ReportDocument();
        if (RbReportType.SelectedIndex == 0)
        {
            if (PrincipalWiseSale == 0)
            {
                mPrincipalId = Constants.IntNullValue;
                CrpReport = new CrpCustomerProductWiseDSRWithoutPrincipal();
            }
            else
            {
                mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
                CrpReport = new CrpCustomerProductWiseDSR();
            }
            for (int i = 0; i < ListCatagory.Items.Count; i++)
            {
                if (ListCatagory.Items[i].Selected == true)
                {
                    Categories_IDs += ListCatagory.Items[i].Value.ToString() + ",";
                }
            }
            if (Categories_IDs == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Category.');", true);
                return;
            }
            DataControl dc = new DataControl();
            ds = RptCustomerCtl.SelectCustomerDSRProDuctWise(int.Parse(drpDistributor.SelectedValue.ToString()), mPrincipalId,
               DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(this.Session["UserId"].ToString()), int.Parse(DrpRoute.SelectedValue.ToString()), int.Parse(drpChannelType.SelectedValue.ToString()), DrpReportType.SelectedIndex
               , int.Parse(DrpSaleForce.SelectedValue.ToString()),Convert.ToInt32(DrpOrderbooker.SelectedValue),Convert.ToInt32(ddlStockUsedBy.SelectedValue), Categories_IDs);

            
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();

            CrpReport.SetParameterValue("Distributor_Name", drpDistributor.SelectedItem.Text);
            CrpReport.SetParameterValue("FROM_DATE", DateTime.Parse(txtStartDate.Text));
            CrpReport.SetParameterValue("TO_DATE", DateTime.Parse(txtEndDate.Text));
            if (PrincipalWiseSale == 0)
            {
                CrpReport.SetParameterValue("Principal", "");
            }
            else
            {
                CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
            }
            //CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
            CrpReport.SetParameterValue("Area", DrpRoute.SelectedItem.Text);
            CrpReport.SetParameterValue("ChannelType", drpChannelType.SelectedItem.Text);
            CrpReport.SetParameterValue("ReportType", "Customer " + DrpReportType.SelectedItem.Text + " Report (Product Wise)");
            CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
            CrpReport.SetParameterValue("SaleForce", DrpSaleForce.SelectedItem.Text);
            CrpReport.SetParameterValue("Orderbooker", DrpOrderbooker.SelectedItem.Text);
            CrpReport.SetParameterValue("StockUsedBy", ddlStockUsedBy.SelectedItem.Text);

            this.Session.Add("CrpReport", CrpReport);
            this.Session.Add("ReportType", 0);
            string url = "'Default.aspx'";
            string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterStartupScript(cstype, "OpenWindow", script);
        }
        else if (RbReportType.SelectedIndex == 1)
        {
            ShowReport(0);
        }
    }



    public void ShowReport(int Type)
    {
        int p_TYPE_ID = 0;
        DocumentPrintController DPrint = new DocumentPrintController();
        RptCustomerController RptCustomerCtl = new RptCustomerController();
        DataSet ds = null;
        ReportDocument CrpReport = new ReportDocument();
        if (PrincipalWiseSale == 0)
        {
            mPrincipalId = Constants.IntNullValue;
            CrpReport = new CrpCustomerDSRWithoutPrincipal();
            p_TYPE_ID = 1;
        }
        else
        {
            mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
            CrpReport = new CrpCustomerDSR();
            p_TYPE_ID = 0;
        }

        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));
        DataControl dc = new DataControl();
        ds = RptCustomerCtl.SelectCustomerDSRValueWise(int.Parse(drpDistributor.SelectedValue.ToString()), mPrincipalId,
            DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(this.Session["UserId"].ToString()), int.Parse(DrpRoute.SelectedValue.ToString()), int.Parse(drpChannelType.SelectedValue.ToString()), CheckBox1.Checked
            , int.Parse(DrpSaleForce.SelectedValue.ToString()), Convert.ToInt32(DrpOrderbooker.SelectedValue), Convert.ToInt32(ddlStockUsedBy.SelectedValue), p_TYPE_ID);

        CrpReport.SetDataSource(ds);
        CrpReport.Refresh();

        CrpReport.SetParameterValue("Distributor_Name", drpDistributor.SelectedItem.Text);
        CrpReport.SetParameterValue("FROM_DATE", DateTime.Parse(txtStartDate.Text));
        CrpReport.SetParameterValue("TO_DATE", DateTime.Parse(txtEndDate.Text));
        if (PrincipalWiseSale == 0)
        {
            CrpReport.SetParameterValue("Principal", "");
        }
        else
        {
            CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
        }
        //CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
        CrpReport.SetParameterValue("Area", DrpRoute.SelectedItem.Text);
        CrpReport.SetParameterValue("ChannelType", drpChannelType.SelectedItem.Text);
        CrpReport.SetParameterValue("SaleForce", DrpSaleForce.SelectedItem.Text);
        CrpReport.SetParameterValue("Orderbooker", DrpOrderbooker.SelectedItem.Text);
        CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
        CrpReport.SetParameterValue("StockUsedBy", ddlStockUsedBy.SelectedItem.Text);
        this.Session.Add("CrpReport", CrpReport);
        this.Session.Add("ReportType", Type);
        string url = "'Default.aspx'";
        string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        Type cstype = this.GetType();
        ClientScriptManager cs = Page.ClientScript;
        cs.RegisterStartupScript(cstype, "OpenWindow", script);
    }
    /// <summary>
    /// Shows Customer Wise (DSR) in Excel
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewExcel_Click(object sender, EventArgs e)
    {
        string Categories_IDs = null;
        DocumentPrintController DPrint = new DocumentPrintController();
        RptCustomerController RptCustomerCtl = new RptCustomerController();
        DataSet ds = null;
        string path = SAMSCommon.Classes.Configuration.GetAppInstallationPath() + "\\CustomerDSR.xls";
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));
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
            ReportInExcel();
            //for (int i = 0; i < ListCatagory.Items.Count; i++)
            //{
            //    if (ListCatagory.Items[i].Selected == true)
            //    {
            //        Categories_IDs += ListCatagory.Items[i].Value.ToString() + ",";
            //    }
            //}
            //if (Categories_IDs == null)
            //{
            //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Category.');", true);
            //    return;
            //}
            //ds = RptCustomerCtl.SelectCustomerDSRProDuctWise(int.Parse(drpDistributor.SelectedValue.ToString()), mPrincipalId,
            //   DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(this.Session["UserId"].ToString()), int.Parse(DrpRoute.SelectedValue.ToString()), int.Parse(drpChannelType.SelectedValue.ToString()), DrpReportType.SelectedIndex, int.Parse(DrpSaleForce.SelectedValue.ToString()), Convert.ToInt32(DrpOrderbooker.SelectedValue),Convert.ToInt32(ddlStockUsedBy.SelectedValue), Categories_IDs);

            //DataSetToExcel.exportToExcel(ds, path, "RptCustomerProductDSR");

            //System.IO.FileInfo file = new System.IO.FileInfo(path);

            //if (file.Exists)
            //{

            //    Response.Clear();

            //    Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);

            //    Response.AddHeader("Content-Length", file.Length.ToString());

            //    Response.ContentType = "application/octet-stream";

            //    Response.WriteFile(file.FullName);

            //    Response.End();

            //}
            //else
            //{
            //    Response.Write("This file does not exist.");
            //}
        }
        else if (RbReportType.SelectedIndex == 1)
        {


            //ShowReport(1);

            int p_TYPE_ID = 0;
            if (PrincipalWiseSale == 0)
            {
                p_TYPE_ID = 1;
            }
            else
            {
                p_TYPE_ID = 0;
            }
            string path2 = SAMSCommon.Classes.Configuration.GetAppInstallationPath() + "\\CustomerDSRValueWise.xls";
            ds = RptCustomerCtl.SelectCustomerDSRValueWise(int.Parse(drpDistributor.SelectedValue.ToString()), mPrincipalId,
            DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(this.Session["UserId"].ToString()), int.Parse(DrpRoute.SelectedValue.ToString()), int.Parse(drpChannelType.SelectedValue.ToString()), CheckBox1.Checked
            , int.Parse(DrpSaleForce.SelectedValue.ToString()), Convert.ToInt32(DrpOrderbooker.SelectedValue), Convert.ToInt32(ddlStockUsedBy.SelectedValue), p_TYPE_ID);

            DataSetToExcel.exportToExcel(ds, path, "CUSTOMER_TRANSCTIONDETAIL");
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
    }

    /// <summary>
    /// Loads Order Bookers
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadOrderBooker();
        Populate_drpSKUCategory();
    }

    public void ReportInExcel()
    {
        string Categories_IDs = null;
        DocumentPrintController DPrint = new DocumentPrintController();
        RptCustomerController RptCustomerCtl = new RptCustomerController();
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));
        ReportDocument CrpReport = new ReportDocument();
        if (RbReportType.SelectedIndex == 0)
        {
            if (PrincipalWiseSale == 0)
            {
                mPrincipalId = Constants.IntNullValue;
                CrpReport = new CrpCustomerProductWiseDSRWithoutPrincipal();
            }
            else
            {
                mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
                CrpReport = new CrpCustomerProductWiseDSR();
            }
            for (int i = 0; i < ListCatagory.Items.Count; i++)
            {
                if (ListCatagory.Items[i].Selected == true)
                {
                    Categories_IDs += ListCatagory.Items[i].Value.ToString() + ",";
                }
            }
            if (Categories_IDs == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Category.');", true);
                return;
            }
            DataControl dc = new DataControl();
            ds = RptCustomerCtl.SelectCustomerDSRProDuctWise(int.Parse(drpDistributor.SelectedValue.ToString()), mPrincipalId,
               DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(this.Session["UserId"].ToString()), int.Parse(DrpRoute.SelectedValue.ToString()), int.Parse(drpChannelType.SelectedValue.ToString()), DrpReportType.SelectedIndex
               , int.Parse(DrpSaleForce.SelectedValue.ToString()), Convert.ToInt32(DrpOrderbooker.SelectedValue), Convert.ToInt32(ddlStockUsedBy.SelectedValue), Categories_IDs);


            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();

            CrpReport.SetParameterValue("Distributor_Name", drpDistributor.SelectedItem.Text);
            CrpReport.SetParameterValue("FROM_DATE", DateTime.Parse(txtStartDate.Text));
            CrpReport.SetParameterValue("TO_DATE", DateTime.Parse(txtEndDate.Text));
            if (PrincipalWiseSale == 0)
            {
                CrpReport.SetParameterValue("Principal", "");
            }
            else
            {
                CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
            }
            //CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
            CrpReport.SetParameterValue("Area", DrpRoute.SelectedItem.Text);
            CrpReport.SetParameterValue("ChannelType", drpChannelType.SelectedItem.Text);
            CrpReport.SetParameterValue("ReportType", "Customer " + DrpReportType.SelectedItem.Text + " Report (Product Wise)");
            CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
            CrpReport.SetParameterValue("SaleForce", DrpSaleForce.SelectedItem.Text);
            CrpReport.SetParameterValue("Orderbooker", DrpOrderbooker.SelectedItem.Text);
            CrpReport.SetParameterValue("StockUsedBy", ddlStockUsedBy.SelectedItem.Text);

            this.Session.Add("CrpReport", CrpReport);
            this.Session.Add("ReportType", 1);
            string url = "'Default.aspx'";
            string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterStartupScript(cstype, "OpenWindow", script);
        }
    }
}