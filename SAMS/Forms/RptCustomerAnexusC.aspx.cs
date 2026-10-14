using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using CrystalDecisions.CrystalReports.Engine;

public partial class Forms_RptCustomerAnexusC : System.Web.UI.Page
{
    public int PrincipalWiseSale = 0;
    private static int mPrincipalId;
    protected void Page_Load(object sender, EventArgs e)
    {
        Configration();
        if (!Page.IsPostBack)
        {
            this.LoadPrincipal();
            this.LoadDistributor();

            this.txtStartDate.Text = System.DateTime.Today.ToString("dd-MMM-yyyy");
            this.txtEndDate.Text = System.DateTime.Today.ToString("dd-MMM-yyyy");
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

        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        drpDistributor.Items.Add(new ListItem("All", "0"));
        clsWebFormUtil.FillDropDownList(this.drpDistributor, dt, 0, 2);

    }

    /// <summary>
    /// Shows Customer Invoice Wise Sales Either in PDF Or in Excel
    /// </summary>
    /// <param name="p_Report_Type">ReportType</param>
    private void ShowReport(int p_Report_Type)
    {
        ReportDocument CrpReport = new ReportDocument();
        string distid = "";
        if (drpDistributor.SelectedValue.ToString() == "0")
        {
            foreach (ListItem li in drpDistributor.Items)
            {
                distid = distid + li.Value.ToString() + ",";
            }
        }
        else {
            distid = drpDistributor.SelectedValue.ToString();
        }
        if (PrincipalWiseSale == 0)
        {
            mPrincipalId = Constants.IntNullValue;
            CrpReport = new SAMSBusinessLayer.Reports.CrpCustomerAnexusrptWithOutPrincipal();
        }
        else
        {
            mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
            CrpReport = new SAMSBusinessLayer.Reports.CrpCustomerAnexusrpt();
        }
        DocumentPrintController mDocumentPrntControl = new DocumentPrintController();
        RptCustomerController RptCustomerCtl = new RptCustomerController();
        
        DataSet ds = RptCustomerCtl.SelectBillCustomerDetail(mPrincipalId, (DrpCUstomerType.SelectedValue).ToString(), distid, DateTime.Parse(this.txtStartDate.Text + " 00:00:00"), DateTime.Parse(this.txtEndDate.Text + " 23:59:59"), 3);

        DataTable dt = mDocumentPrntControl.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));

        
        CrpReport.SetDataSource(ds);
        CrpReport.Refresh();

        CrpReport.SetParameterValue("PRINCIPAL", DrpPrincipal.SelectedItem.Text);
        CrpReport.SetParameterValue("FROM_DATE", txtStartDate.Text);
        CrpReport.SetParameterValue("TO_DATE", txtEndDate.Text);
        CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
        CrpReport.SetParameterValue("location", drpDistributor.SelectedItem.Text);
        CrpReport.SetParameterValue("printuser", Session["UserName"].ToString());

        this.Session.Add("CrpReport", CrpReport);
        this.Session.Add("ReportType", p_Report_Type);
        string url = "'Default.aspx'";
        string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        Type cstype = this.GetType();
        ClientScriptManager cs = Page.ClientScript;
        cs.RegisterStartupScript(cstype, "OpenWindow", script);



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
}