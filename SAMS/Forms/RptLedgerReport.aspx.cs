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
/// Form For General Ledger Report
/// </summary>
public partial class Forms_RptLedgerReport : System.Web.UI.Page
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
            this.LoadAccountDetail();
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
        this.DrpPrincipal.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, m_dt, 0, 1);
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
    /// Loads Accont Heads To Account ListBox
    /// </summary>
    private void LoadAccountDetail()
    {
        AccountHeadController mAccountController = new AccountHeadController();
        DataTable dtHead = mAccountController.SelectAccountHead(Constants.AC_AccountHeadId, Constants.LongNullValue);
      //  clsWebFormUtil.FillListBox(this.LstAccountHead, dtHead, "ACCOUNT_HEAD", "ACCOUNT_HEAD", true);
        clsWebFormUtil.FillDropDownList(this.drpAccountHead, dtHead, 0, 10);
       // this.Session.Add("dtHead", dtHead);
    }

    /// <summary>
    /// Shows General Ledger in PDF
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewPDF_Click(object sender, EventArgs e)
    {
      //  DataTable dtHead = (DataTable)this.Session["dtHead"]; 
     //   DataRow[] foundRows = dtHead.Select("ACCOUNT_CODE  = '" + txtAccountCode.Text + "'");
        DocumentPrintController DPrint = new DocumentPrintController();
        RptAccountController RptAccountCtl = new RptAccountController();
        ReportDocument CrpReport = new ReportDocument();
        if (PrincipalWiseSale == 0)
        {
            mPrincipalId = Constants.IntNullValue;
        }
        else
        {
            mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
        }
        DataSet ds = null;

         if (drpAccountHead.Items.Count > 0)
         {
            long AccountHeadID =long.Parse(drpAccountHead.SelectedValue) ;//long.Parse(foundRows[0]["ACCOUNT_HEAD_ID"].ToString());
             
             DataControl dc = new DataControl();

             ds = RptAccountCtl.GeneralLedger_View(mPrincipalId, AccountHeadID, int.Parse(drpDistributor.SelectedValue.ToString()),
                       DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(rbPosted.SelectedValue.ToString())  );

             DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));

             decimal OpeningValue = RptAccountCtl.GeneralLedgerOpening(mPrincipalId, AccountHeadID, int.Parse(drpDistributor.SelectedValue.ToString()),
                       DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(rbPosted.SelectedValue.ToString()));

            if (PrincipalWiseSale == 0)
            {
                CrpReport = new SAMSBusinessLayer.Reports.CrpLedgerViewWithoutPrincipal();
            }
            else
            {
                CrpReport = new SAMSBusinessLayer.Reports.CrpLedgerView();
            }
            
             CrpReport.SetDataSource(ds);
             CrpReport.Refresh();

             CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
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
            
             CrpReport.SetParameterValue("OpeningValue", OpeningValue.ToString());
            
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
    /// Shows General Ledger in Excel
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnViewExcel_Click(object sender, EventArgs e)
    {
       // DataTable dtHead = (DataTable)this.Session["dtHead"];
       // DataRow[] foundRows = dtHead.Select("ACCOUNT_CODE  = '" + txtAccountCode.Text + "'");
        DocumentPrintController DPrint = new DocumentPrintController();
        RptAccountController RptAccountCtl = new RptAccountController();
        ReportDocument CrpReport = new ReportDocument();
        DataSet ds = null;
        if (PrincipalWiseSale == 0)
        {
            mPrincipalId = Constants.IntNullValue;
        }
        else
        {
            mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
        }
        if (drpAccountHead.Items.Count > 0)
        {
            long AccountHeadID = long.Parse(drpAccountHead.SelectedValue);//long.Parse(foundRows[0]["ACCOUNT_HEAD_ID"].ToString());

            DataControl dc = new DataControl();

            ds = RptAccountCtl.GeneralLedger_View(mPrincipalId, AccountHeadID, int.Parse(drpDistributor.SelectedValue.ToString()),
                       DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(rbPosted.SelectedValue.ToString()));

            decimal OpeningValue = RptAccountCtl.GeneralLedgerOpening(mPrincipalId, AccountHeadID, int.Parse(drpDistributor.SelectedValue.ToString()),
                      DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(rbPosted.SelectedValue.ToString()));  

            DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));

            if (PrincipalWiseSale == 0)
            {
                CrpReport = new SAMSBusinessLayer.Reports.CrpLedgerViewWithoutPrincipal();
            }
            else
            {
                CrpReport = new SAMSBusinessLayer.Reports.CrpLedgerView();
            }
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();

            CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
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
            CrpReport.SetParameterValue("OpeningValue", OpeningValue.ToString());

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
    }
}
