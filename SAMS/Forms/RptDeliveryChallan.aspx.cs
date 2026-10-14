using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using SAMSBusinessLayer.Reports;
using CrystalDecisions.CrystalReports.Engine;
using System.Linq;

public partial class Forms_RptDeliveryChallan : System.Web.UI.Page
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
        //ViewAccess = ValidateScreen();
        //if (ViewAccess == false)
        //{
        //    Response.Redirect("../Forms/AccessDenied.aspx");
        //}
        Configration();
        if (!Page.IsPostBack)
        {
            LoadCompany();
            this.LoadDistributor();
            this.LoadPrincipal();
            this.LoadSaleForce();
            this.LoadRoute();
            this.LoadCustomer();
            SAMSCommon.Classes.Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtStartDate.Text = SAMSCommon.Classes.Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = SAMSCommon.Classes.Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            DrpPrintPaperSize_SelectedIndexChanged(null, null);

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
    private void LoadCompany()
    {
        CompanyController CompanyCtl = new CompanyController();
        DataTable dt = CompanyCtl.SelectCompany(Constants.IntNullValue, Constants.IntNullValue);
        clsWebFormUtil.FillDropDownList(ddlCompany, dt, "COMPANY_ID", "STRN");
    }
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
        //drpDistributor.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
        clsWebFormUtil.FillDropDownList(this.drpDistributor, dt, 0, 2);
    }

    /// <summary>
    /// Loads Sale Forces To Sale Force Combo
    /// </summary>
    private void LoadSaleForce()
    {
        if (PrincipalWiseSale == 0)
        {
            mPrincipalId = Constants.IntNullValue;
        }
        else
        {
            mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
        }
        DrpArea.Items.Clear();
        SaleForceController mDController = new SaleForceController();
        DataTable m_dt = mDController.SelectRollBackInvoiceSaleForce(int.Parse(DrpLedgerType.SelectedValue.ToString()), mPrincipalId, int.Parse(drpDistributor.SelectedValue.ToString()));
        DrpArea.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
        clsWebFormUtil.FillDropDownList(this.DrpArea, m_dt, 0, 1);
    }

    /// <summary>
    /// Loads Routes To Route Combo
    /// </summary>
    private void LoadRoute()
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
    /// Loads Customers To Customer Combo
    /// </summary>
    private void LoadCustomer()
    {
        DrpCustomer.Items.Clear();
        DrpCustomer.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
        if (drpDistributor.Items.Count > 0 && DrpRoute.Items.Count > 0 && DrpRoute.SelectedValue != Constants.IntNullValue.ToString())
        {
            CustomerDataController mController = new CustomerDataController();
            DataTable dt = mController.SelectAllCustomer(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpRoute.SelectedValue.ToString()), Constants.IntNullValue);
            clsWebFormUtil.FillDropDownList(this.DrpCustomer, dt, 0, 3, false);

        }
    }

    /// <summary>
    /// Loads Sale Forces And Routes
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadSaleForce();
        this.LoadRoute();
    }

    /// <summary>
    /// Loads Sale Forces
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadSaleForce();
    }

    /// <summary>
    /// Loads Sale Forces
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpLedgerType_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadSaleForce();
        if (DrpLedgerType.SelectedValue == "0")
        {
            rbOrderstatus.Visible = true;
        }
        else
        { rbOrderstatus.Visible = false; }
    }

    /// <summary>
    /// Loads Customers
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpRoute_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadCustomer();
    }

    /// <summary>
    /// Shows Print Sale Document in PDF
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewPDF_Click(object sender, EventArgs e)
    {
        ShowReport(0);
    }

    /// <summary>
    /// Gets Print Sale Document And Shows Either in Excel or PDF
    /// </summary>
    /// <param name="p_ReportType">Type</param>
    private void ShowReport(int p_ReportType)
    {
        int p_CustomerType = Constants.IntNullValue;
        if (rblCustomerType.SelectedValue != "-1")
        {
            p_CustomerType = Convert.ToInt32(rblCustomerType.SelectedValue);
        }
        System.Text.StringBuilder sbDistributors = new System.Text.StringBuilder();

        if (drpDistributor.SelectedValue == Constants.IntNullValue.ToString())
        {
            foreach (ListItem li in drpDistributor.Items)
            {
                if (li.Value != Constants.IntNullValue.ToString())
                {
                    sbDistributors.Append(li.Value);
                    sbDistributors.Append(",");
                }
            }
        }
        else
        {
            sbDistributors.Append(drpDistributor.SelectedValue);
        }

        RptSaleController RptSaleCtl = new RptSaleController();
        DocumentPrintController DPrint = new DocumentPrintController();
        DataSet ds = null;

        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));
        if (PrincipalWiseSale == 0)
        {
            mPrincipalId = Constants.IntNullValue;
        }
        else
        {
            mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
        }
        string PrincipalIDs = "";
        int PrincipalSingleId = 0;
        {
            PrincipalSingleId = int.Parse(DrpPrincipal.SelectedValue.ToString());
            foreach (ListItem li in DrpPrincipal.Items)
            {
                if (li.Value != "-2147483648")
                {
                    PrincipalIDs += li.Value + ",";
                }
            }
        }
        var strn = ddlCompany.SelectedItem.Text.Split('|');
        if (dt.Rows.Count > 0)
        {
            if (DrpLedgerType.SelectedIndex == 3)
            {
                if (DrpPrintPaperSize.SelectedValue == "0")
                {
                    DataControl dc = new DataControl();
                    ds = RptSaleCtl.SelectDocumentforPrint(sbDistributors.ToString(), int.Parse(DrpArea.SelectedValue.ToString()), mPrincipalId,
                        DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), DrpLedgerType.SelectedIndex, Constants.LongNullValue, p_CustomerType, Convert.ToInt32(DrpCustomer.SelectedValue), Convert.ToInt32(DrpRoute.SelectedValue), 0);
                    ReportDocument SubReport = new ReportDocument();
                    ReportDocument CrpReport = new ReportDocument();
                    if (DrpSort.SelectedValue == "0")
                    {
                        CrpReport = new SAMSBusinessLayer.Reports.CrpDeliveryChallan();
                    }
                    else
                    {
                        CrpReport = new SAMSBusinessLayer.Reports.CrpDeliveryChallanCustomer();
                    }

                    if (rbtSortOrder.SelectedIndex == 0)
                    {
                        CrpReport.DataDefinition.SortFields[0].SortDirection = CrystalDecisions.Shared.SortDirection.AscendingOrder;
                    }
                    else
                    {
                        CrpReport.DataDefinition.SortFields[0].SortDirection = CrystalDecisions.Shared.SortDirection.DescendingOrder;
                    }
                    SubReport = CrpReport.OpenSubreport("Subreport1");
                    CrpReport.SetDataSource(ds);
                    SubReport.SetDataSource(ds);
                    CrpReport.Refresh();
                    CrpReport.SetParameterValue("COMPANY_NAME", dt.Rows[0]["COMPANY_NAME"].ToString());
                    CrpReport.SetParameterValue("CompanyAddress", dt.Rows[0]["ADDRESS1"].ToString());
                    CrpReport.SetParameterValue("CONTACT_NUMBER", dt.Rows[0]["CONTACT_NUMBER"].ToString());
                    CrpReport.SetParameterValue("USER_NAME", Session["UserName2"].ToString());
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
                    ds = DPrint.SelectDocumentforPrint(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpArea.SelectedValue.ToString()), PrincipalIDs,
                    DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), 3, Constants.LongNullValue, 0, PrincipalSingleId);
                    CrpDeliveryChallanNewMFN crpReport = new CrpDeliveryChallanNewMFN();
                    ReportDocument subReport = crpReport.OpenSubreport("Subreport1");
                    crpReport.SetDataSource(ds);
                    subReport.SetDataSource(ds);
                    crpReport.Refresh();
                    crpReport.SetParameterValue("COMPANY_NAME", strn[0]);
                    crpReport.SetParameterValue("CompanyAddress", dt.Rows[0]["ADDRESS1"]);
                    Session.Add("CrpReport", crpReport);
                    Session.Add("ReportType", 0);
                    const string url = "'Default.aspx'";
                    const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                    Type cstype = GetType();
                    ClientScriptManager cs = Page.ClientScript;
                    cs.RegisterStartupScript(cstype, "OpenWindow", script);
                }


            }
            //else if (DrpLedgerType.SelectedIndex == 4)
            //{

            //    DataControl dc = new DataControl();
            //    ds = RptSaleCtl.SelectCSDUSCDocumentforPrint(sbDistributors.ToString(), int.Parse(DrpArea.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()),
            //        DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), 1, Constants.LongNullValue, p_CustomerType, Convert.ToInt32(DrpCustomer.SelectedValue), Convert.ToInt32(DrpRoute.SelectedValue));


            //    ReportDocument CrpReport = new ReportDocument();
            //    if (DrpSort.SelectedValue == "0")
            //    {
            //        CrpReport = new SAMSBusinessLayer.Reports.CrpPrintUSDInvoice();
            //    }
            //    else
            //    {
            //        CrpReport = new SAMSBusinessLayer.Reports.CrpPrintUSDInvoiceCustomer();
            //    }

            //    CrpReport.SetDataSource(ds);
            //    CrpReport.Refresh();

            //    if (rbtSortOrder.SelectedIndex == 0)
            //    {
            //        CrpReport.DataDefinition.SortFields[0].SortDirection = CrystalDecisions.Shared.SortDirection.AscendingOrder;
            //    }
            //    else
            //    {
            //        CrpReport.DataDefinition.SortFields[0].SortDirection = CrystalDecisions.Shared.SortDirection.DescendingOrder;
            //    }

            //    CrpReport.SetParameterValue("COMPANY_NAME", dt.Rows[0]["COMPANY_NAME"].ToString());
            //    CrpReport.SetParameterValue("CompanyAddress", "Branch Address: " + dt.Rows[0]["ADDRESS1"].ToString());
            //    CrpReport.SetParameterValue("TAXREGISTERATION_NO", dt.Rows[0]["GST_NUMBER"].ToString());
            //    CrpReport.SetParameterValue("DISTRIBUTOR_NAME", dt.Rows[0]["DISTRIBUTOR_NAME"].ToString());
            //    this.Session.Add("CrpReport", CrpReport);
            //    this.Session.Add("ReportType", 0);
            //    string url = "'Default.aspx'";
            //    string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
            //    Type cstype = this.GetType();
            //    ClientScriptManager cs = Page.ClientScript;
            //    cs.RegisterStartupScript(cstype, "OpenWindow", script);
            //}
            else if (DrpLedgerType.SelectedIndex == 5)
            {

                DataControl dc = new DataControl();
                ds = RptSaleCtl.SelectDocumentforPrint(sbDistributors.ToString(), Convert.ToInt32(DrpArea.SelectedValue), mPrincipalId,
                    DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(DrpLedgerType.SelectedValue.ToString()), Constants.LongNullValue, p_CustomerType, Convert.ToInt32(DrpCustomer.SelectedValue), Convert.ToInt32(DrpRoute.SelectedValue), 1);
                ReportDocument CrpReport = new ReportDocument();

                if (DrpSort.SelectedValue == "0")
                {

                    if (rblCustomerType.SelectedValue == "1")// registered
                    {
                        CrpReport = new SAMSBusinessLayer.Reports.CrpPrintDocumentReg2InUnits();
                        //CrpReport = new SAMSBusinessLayer.Reports.CrpPrintDocumentReg2();
                    }
                    else
                    {
                        CrpReport = new SAMSBusinessLayer.Reports.CrpPrintDocumentUnRegInUnits();
                        //CrpReport = new SAMSBusinessLayer.Reports.CrpPrintDocumentUnReg();
                    }
                }
                else
                {
                    CrpReport = new SAMSBusinessLayer.Reports.CrpPrintDocumentCustomer();
                }
                if (rbtSortOrder.SelectedIndex == 0)
                {
                    CrpReport.DataDefinition.SortFields[0].SortDirection = CrystalDecisions.Shared.SortDirection.AscendingOrder;
                }
                else
                {
                    CrpReport.DataDefinition.SortFields[0].SortDirection = CrystalDecisions.Shared.SortDirection.DescendingOrder;
                }
                string strPackSize = "Shade Code";
                DataTable dtAppSetting = (DataTable)Session["dtAppSetting"];
                if (dtAppSetting != null)
                {
                    DataRow[] drAppSetting = dtAppSetting.Select("strColumnName='PackSize'");
                    if (drAppSetting.Length > 0)
                    {
                        strPackSize = drAppSetting[0]["strColumnValue"].ToString();
                    }
                }
                CrpReport.SetDataSource(ds);
                CrpReport.Refresh();
                CrpReport.SetParameterValue("COMPANY_NAME", dt.Rows[0]["COMPANY_NAME"].ToString());
                CrpReport.SetParameterValue("NtnNumber", "NTN :" + dt.Rows[0]["PASSWORD"].ToString());
                CrpReport.SetParameterValue("CompanyAddress", dt.Rows[0]["CompanyAddress"].ToString());
                CrpReport.SetParameterValue("BranchAddress", dt.Rows[0]["ADDRESS1"].ToString());
                CrpReport.SetParameterValue("TAXREGISTERATION_NO", dt.Rows[0]["GST_NUMBER"].ToString());
                CrpReport.SetParameterValue("DISTRIBUTOR_NAME", dt.Rows[0]["DISTRIBUTOR_NAME"].ToString());
                CrpReport.SetParameterValue("CONTACT_NUMBER", "Ph: " + dt.Rows[0]["CONTACT_NUMBER"].ToString());
                CrpReport.SetParameterValue("PrintType", "0");
                CrpReport.SetParameterValue("PackSize", strPackSize);
                this.Session.Add("CrpReport", CrpReport);
                this.Session.Add("ReportType", p_ReportType);
                string url = "'Default.aspx'";
                string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(cstype, "OpenWindow", script);
            }
            else
            {
                if (DrpPrintPaperSize.SelectedValue == "0" || DrpPrintPaperSize.SelectedValue == "1")
                {
                    // Old Type of Invoices
                    DataControl dc = new DataControl();
                    ReportDocument CrpReport = new ReportDocument();
                    ReportDocument crPromotion = new ReportDocument();
                    if (DrpSort.SelectedValue == "0")
                    {
                        if (rblCustomerType.SelectedValue == "1")// registered
                        {
                            ds = RptSaleCtl.SelectDocumentforPrint(sbDistributors.ToString(), Convert.ToInt32(DrpArea.SelectedValue), mPrincipalId,
                        DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(DrpLedgerType.SelectedValue.ToString()), Constants.LongNullValue, p_CustomerType, Convert.ToInt32(DrpCustomer.SelectedValue), Convert.ToInt32(DrpRoute.SelectedValue), int.Parse(rbOrderstatus.SelectedValue));
                            CrpReport = new SAMSBusinessLayer.Reports.CrpPrintDocumentReg2InUnits();
                        }
                        else
                        {
                            ds = RptSaleCtl.SelectDocumentforPrintNew(sbDistributors.ToString(), Convert.ToInt32(DrpArea.SelectedValue), mPrincipalId,
                        DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(DrpLedgerType.SelectedValue.ToString()), Constants.LongNullValue, p_CustomerType, Convert.ToInt32(DrpCustomer.SelectedValue), Convert.ToInt32(DrpRoute.SelectedValue), int.Parse(rbOrderstatus.SelectedValue));
                            if (DrpPrintPaperSize.SelectedValue == "1")
                            {
                                // A4 (With Summary)
                                CrpReport = new SAMSBusinessLayer.Reports.CrpPrintDocumentWithSummary();
                            }
                            else
                            {
                                // A4
                                CrpReport = new SAMSBusinessLayer.Reports.CrpPrintDocumentUnRegInUnitsNew();
                            }
                            crPromotion = CrpReport.OpenSubreport("crPromotion");
                        }
                    }
                    else
                    {
                        ds = RptSaleCtl.SelectDocumentforPrint(sbDistributors.ToString(), Convert.ToInt32(DrpArea.SelectedValue), mPrincipalId,
                        DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(DrpLedgerType.SelectedValue.ToString()), Constants.LongNullValue, p_CustomerType, Convert.ToInt32(DrpCustomer.SelectedValue), Convert.ToInt32(DrpRoute.SelectedValue), int.Parse(rbOrderstatus.SelectedValue));
                        CrpReport = new SAMSBusinessLayer.Reports.CrpPrintDocumentCustomer();
                    }
                    if (rbtSortOrder.SelectedIndex == 0)
                    {
                        CrpReport.DataDefinition.SortFields[0].SortDirection = CrystalDecisions.Shared.SortDirection.AscendingOrder;
                    }
                    else
                    {
                        CrpReport.DataDefinition.SortFields[0].SortDirection = CrystalDecisions.Shared.SortDirection.DescendingOrder;
                    }
                    string strPackSize = "Shade Code";
                    DataTable dtAppSetting = (DataTable)Session["dtAppSetting"];
                    if (dtAppSetting != null)
                    {
                        DataRow[] drAppSetting = dtAppSetting.Select("strColumnName='PackSize'");
                        if (drAppSetting.Length > 0)
                        {
                            strPackSize = drAppSetting[0]["strColumnValue"].ToString();
                        }
                    }
                    CrpReport.SetDataSource(ds);
                    if (DrpSort.SelectedValue == "0")
                    {
                        crPromotion.SetDataSource(ds);
                    }
                    CrpReport.Refresh();
                    CrpReport.SetParameterValue("COMPANY_NAME", dt.Rows[0]["COMPANY_NAME"].ToString());
                    CrpReport.SetParameterValue("NtnNumber", "NTN :" + dt.Rows[0]["PASSWORD"].ToString());
                    CrpReport.SetParameterValue("CompanyAddress", dt.Rows[0]["CompanyAddress"].ToString());
                    CrpReport.SetParameterValue("BranchAddress", dt.Rows[0]["ADDRESS1"].ToString());
                    CrpReport.SetParameterValue("TAXREGISTERATION_NO", dt.Rows[0]["GST_NUMBER"].ToString());
                    CrpReport.SetParameterValue("DISTRIBUTOR_NAME", dt.Rows[0]["DISTRIBUTOR_NAME"].ToString());
                    CrpReport.SetParameterValue("CONTACT_NUMBER", "Phone: " + dt.Rows[0]["CONTACT_NUMBER"].ToString());
                    CrpReport.SetParameterValue("PrintType", "0");
                    CrpReport.SetParameterValue("PackSize", strPackSize);
                    this.Session.Add("CrpReport", CrpReport);
                    this.Session.Add("ReportType", p_ReportType);
                    string url = "'Default.aspx'";
                    string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                    Type cstype = this.GetType();
                    ClientScriptManager cs = Page.ClientScript;
                    cs.RegisterStartupScript(cstype, "OpenWindow", script);
                }
                else
                {
                    // New Type of Invoices (A5)
                    ds = DPrint.SelectDocumentforPrint(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpArea.SelectedValue.ToString()), PrincipalIDs,
                    DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(DrpLedgerType.SelectedValue.ToString()), Constants.LongNullValue, 0, PrincipalSingleId);
                    var crpReport = new CrpPrintDocumentNew();
                    ReportDocument subReport = crpReport.OpenSubreport("SUBREPORT1");
                    crpReport.SetDataSource(ds);
                    subReport.SetDataSource(ds);
                    crpReport.Refresh();
                    crpReport.SetParameterValue("COMPANY_NAME", strn[0]);
                    crpReport.SetParameterValue("DISTRIBUTOR_NAME", dt.Rows[0]["DISTRIBUTOR_NAME"].ToString());
                    crpReport.SetParameterValue("Address", "Address: " + dt.Rows[0]["ADDRESS1"].ToString());
                    crpReport.SetParameterValue("EmailTo", dt.Rows[0]["ADDRESS2"].ToString());
                    crpReport.SetParameterValue("CONTACT_NUMBER", dt.Rows[0]["CONTACT_NUMBER"].ToString());
                    crpReport.SetParameterValue("NTN", "NTN #:" + dt.Rows[0]["NTN_NO"].ToString());
                    crpReport.SetParameterValue("PrintType", 0);
                    crpReport.SetParameterValue("STRN", "STRN #:" + dt.Rows[0]["GST_NUMBER"].ToString());
                    Session.Add("CrpReport", crpReport);
                    Session.Add("ReportType", 0);
                    const string url = "'Default.aspx'";
                    const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                    Type cstype = GetType();
                    ClientScriptManager cs = Page.ClientScript;
                    cs.RegisterStartupScript(cstype, "OpenWindow", script);
                }
            }
        }
    }

    /// <summary>
    /// Shows Print Sale Document in Excel
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewExcel_Click(object sender, EventArgs e)
    {
        ShowReport(1);
    }

    protected void DrpPrintPaperSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DrpPrintPaperSize.SelectedValue == "1")
        {
            PanelCompany.Visible = true;
            DrpPrincipal.Enabled = false;
        }
        else
        {
            PanelCompany.Visible = false;
            DrpPrincipal.Enabled = true;
        }
    }
}