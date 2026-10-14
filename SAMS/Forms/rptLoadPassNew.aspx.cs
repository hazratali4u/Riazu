using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSBusinessLayer.Reports;
using SAMSCommon.Classes;

public partial class Forms_rptLoadPassNew : Page
{
    readonly SKUPriceDetailController _pController = new SKUPriceDetailController();
    readonly Distributor_UserController _mDController = new Distributor_UserController();
    readonly DistributorController _dController = new DistributorController();
    readonly DocumentPrintController _dPrint = new DocumentPrintController();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LoadPrincipal();
            LoadLocation();
            LoadOrderBooker();
            Configuration.SystemCurrentDateTime = (DateTime)Session["CurrentWorkDate"];
            txtFromDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
           
        }
    }
    protected void LoadPrincipal()
    {
        try
        {
           
            DataTable mDt = _pController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
            this.drpPrincipal.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
            clsWebFormUtil.FillDropDownList(this.drpPrincipal, mDt, "Company_Id", "Company_Name");
        }
        catch (Exception ex)
        {
            ex.ToString();
        }
    }
    private void LoadOrderBooker()
    {
        DrpOrderBooker.Items.Clear();
        if (DrpLocation.Items.Count > 0)
        {            
            DataTable mDt = _mDController.SelectSaleForceAssignedArea(int.Parse(DrpLocation.SelectedValue), Constants.IntNullValue, int.Parse(this.Session["CompanyId"].ToString()));
            DrpOrderBooker.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
            clsWebFormUtil.FillDropDownList(this.DrpOrderBooker, mDt, 0, 3);
        }        
    }
    protected void LoadLocation()
    {
        try
        {
            
            DataTable dt = _dController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
            DrpLocation.DataSource = dt;
            DrpLocation.DataTextField = "DISTRIBUTOR_NAME";
            DrpLocation.DataValueField = "DISTRIBUTOR_ID";
            DrpLocation.DataBind();
        }
        catch (Exception ex)
        {
            ex.ToString();
        }
    }
    protected void Button2_Click(object sender, EventArgs e)
    {

        try
        {
            DataTable dt = _dPrint.SelectReportTitle(Convert.ToInt32(DrpLocation.SelectedValue));
            if (rblReport.SelectedIndex == 0)
            {
                DataSet ds = null;
                ds = _dPrint.LoadPass(int.Parse(drpPrincipal.SelectedValue), DateTime.Parse(txtFromDate.Text + " 00:00:00"), int.Parse(DrpOrderBooker.SelectedValue.ToString()), int.Parse(DrpLocation.SelectedValue.ToString()), int.Parse(ddlReportBY.SelectedValue));
                CrystalDecisions.CrystalReports.Engine.ReportClass CrpReport = new CrystalDecisions.CrystalReports.Engine.ReportClass();
                if (cbType.Checked)
                {
                    CrpReport = new CrpPrintDSR();
                }
                else
                {
                    CrpReport = new CrpPrintDSR2();
                }
                CrpReport.SetDataSource(ds);

                DataView view = new DataView(ds.Tables["RptPrintDSR"]);
                DataTable distinctValues = view.ToTable(true, "customer_code");

                CrpReport.Refresh();
                CrpReport.SetParameterValue("SalemanName", DrpOrderBooker.SelectedItem.Text);
                CrpReport.SetParameterValue("Date", txtFromDate.Text);
                CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
                CrpReport.SetParameterValue("TotalNoOfCustomers", distinctValues.Rows.Count.ToString());
                Session.Add("CrpReport", CrpReport);
                Session.Add("ReportType", 0);
                const string url = "'Default.aspx'";
                const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(cstype, "OpenWindow", script);
            }
            else
            {
                DataSet ds = null;
                ds = _dPrint.LoadPass(int.Parse(drpPrincipal.SelectedValue), DateTime.Parse(txtFromDate.Text + " 00:00:00"), Convert.ToInt32(DrpOrderBooker.SelectedValue), int.Parse(DrpLocation.SelectedValue.ToString()), int.Parse(ddlReportBY.SelectedValue));
                CrpLoadPassDM CrpReport = new CrpLoadPassDM();
                CrpReport.SetDataSource(ds);
                DataView view = new DataView(ds.Tables["RptPrintDSR"]);
                DataTable distinctValues = view.ToTable(true, "sku_code");

                
                CrpReport.Refresh();
                CrpReport.SetParameterValue("Principal", this.drpPrincipal.SelectedItem.Text);
                CrpReport.SetParameterValue("Date", this.txtFromDate.Text);
                CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
                CrpReport.SetParameterValue("TotalNoOfProduct", distinctValues.Rows.Count.ToString());
                Session.Add("CrpReport", CrpReport);
                Session.Add("ReportType", 0);
                const string url = "'Default.aspx'";
                const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
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
    protected void Button1_Click(object sender, EventArgs e)
    {
        try
        {
            DataTable dt = _dPrint.SelectReportTitle(Convert.ToInt32(DrpLocation.SelectedValue));
            if (rblReport.SelectedIndex == 0)
            {
              
                DataSet ds = null;

                ds = _dPrint.LoadPass(int.Parse(drpPrincipal.SelectedValue), DateTime.Parse(txtFromDate.Text), int.Parse(DrpOrderBooker.SelectedValue.ToString()), int.Parse(DrpLocation.SelectedValue.ToString()), int.Parse(ddlReportBY.SelectedValue));
                CrpPrintDSR CrpReport = new CrpPrintDSR();
                CrpReport.SetDataSource(ds);
                CrpReport.Refresh();

               CrpReport.SetParameterValue("SalemanName", DrpOrderBooker.SelectedItem.Text);
                CrpReport.SetParameterValue("Date", txtFromDate.Text);
                CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());

                
                Session.Add("CrpReport", CrpReport);
                Session.Add("ReportType", 1);
                const string url = "'Default.aspx'";
                const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(cstype, "OpenWindow", script);
            }
            else
            {
               DataSet ds = null;

                ds = _dPrint.LoadPass(int.Parse(drpPrincipal.SelectedValue), DateTime.Parse(txtFromDate.Text), Convert.ToInt32(DrpOrderBooker.SelectedValue), int.Parse(DrpLocation.SelectedValue.ToString()), int.Parse(ddlReportBY.SelectedValue));
                CrpLoadPassDM CrpReport = new CrpLoadPassDM();
                CrpReport.SetDataSource(ds);
                CrpReport.Refresh();

                CrpReport.SetParameterValue("Principal", DrpOrderBooker.SelectedItem.Text);
                CrpReport.SetParameterValue("Date", txtFromDate.Text);
                
                CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());

                Session.Add("CrpReport", CrpReport);
                Session.Add("ReportType", 1);
                const string url = "'Default.aspx'";
                const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
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
    protected void DrpLocation_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadOrderBooker();
    }


    protected void rblReport_SelectedIndexChanged(object sender, EventArgs e)
    {
        if(rblReport.SelectedValue == "0")
        {
            cbType.Visible = true;
        }
        else
        {
            cbType.Visible = false;
        }
    }
}