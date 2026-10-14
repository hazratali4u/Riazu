
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using SAMSBusinessLayer.Classes;
using SAMSBusinessLayer.Reports;
using SAMSCommon.Classes;  
    
public partial class Forms_frmDocumentPrintingNew : System.Web.UI.Page
{
    private void LoadPrincipal()
    {
        SKUPriceDetailController PController = new SKUPriceDetailController();
        DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(Session["CurrentWorkDate"].ToString()));
        clsWebFormUtil.FillListBox(cblPrincipal,m_dt, 0, 1,true);        
    }
    
    private void LoadDistributor()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(Session["UserId"].ToString()), int.Parse(Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDropDownList(drpDistributor, dt, 0, 2,true);
    }
    
    private void LoadSaleForce()
    {
        int TypeID = int.Parse(DrpLedgerType.SelectedValue);
        if(DrpLedgerType.SelectedValue == "5")
        {
            TypeID = 1;
        }
        DrpArea.Items.Clear();   
        Distributor_UserController mDController = new Distributor_UserController();
        DataTable m_dt = mDController.SelectRollBackInvoiceSaleForce(TypeID, Constants.IntNullValue, int.Parse(drpDistributor.SelectedValue.ToString()));
        DrpArea.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));       
        clsWebFormUtil.FillDropDownList(DrpArea, m_dt, 0, 1);
    }
    
    private void LoadCompany()
    {
        CompanyController CompanyCtl = new CompanyController();
        DataTable dt = CompanyCtl.SelectCompany(Constants.IntNullValue, Constants.IntNullValue);

        clsWebFormUtil.FillDropDownList(ddlCompany, dt, "COMPANY_ID", "STRN");
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LoadCompany();
            LoadDistributor();
            LoadPrincipal();
            LoadSaleForce();
            Configuration.SystemCurrentDateTime = (DateTime)Session["CurrentWorkDate"];     
            txtStartDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            UserController mUserController = new UserController();
        }
    }
    
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSaleForce();
    }
    
    protected void DrpLedgerType_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSaleForce();
    }
    
    protected void DrpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSaleForce();
    }

    protected void Button2_Click(object sender, EventArgs e) //View PDF
    {
        string PrincipalIDs = "";
        int PrincipalSingleId = 0;
            PrincipalSingleId = int.Parse(cblPrincipal.SelectedValue.ToString());
        foreach (ListItem li in cblPrincipal.Items)
        {
            if (li.Selected)
            {
                PrincipalIDs += li.Value + ",";
            }
        }
        DocumentPrintController DPrint = new DocumentPrintController();
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));
        if (dt.Rows.Count > 0)
        {
            var strn = ddlCompany.SelectedItem.Text.Split('|');
            if(DrpLedgerType.SelectedIndex == 5)
            {
                ds = DPrint.SelectDocumentforPrint(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpArea.SelectedValue.ToString()), PrincipalIDs,
                    DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(DrpLedgerType.SelectedValue.ToString()), Constants.LongNullValue, 0, PrincipalSingleId);
                var crpReport = new CrpPrintDocumentGST();
                crpReport.SetDataSource(ds);
               
                crpReport.Refresh();
                crpReport.SetParameterValue("COMPANY_NAME", dt.Rows[0]["COMPANY_NAME"].ToString());
                crpReport.SetParameterValue("Address", "Address: " + dt.Rows[0]["ADDRESS1"].ToString());
                crpReport.SetParameterValue("EmailTo", dt.Rows[0]["ADDRESS2"].ToString());
                crpReport.SetParameterValue("CONTACT_NUMBER", dt.Rows[0]["CONTACT_NUMBER"].ToString());
                crpReport.SetParameterValue("NTN", "NTN #: " + dt.Rows[0]["NTN_NO"].ToString());
                crpReport.SetParameterValue("PrintType", 0);

                crpReport.SetParameterValue("STRN", strn[1].Replace(" ",""));

                Session.Add("CrpReport", crpReport);
                Session.Add("ReportType", 0);
                const string url = "'Default.aspx'";
                const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                Type cstype = GetType();
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(cstype, "OpenWindow", script);
            }
            else if (DrpLedgerType.SelectedIndex == 3)
            {
                ds = DPrint.SelectDocumentforPrint(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpArea.SelectedValue.ToString()), PrincipalIDs,
                    DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), 3, Constants.LongNullValue, 0, PrincipalSingleId);
      
                ReportDocument CrpReport = new ReportDocument();
                //CrpReport = new CrpDeliveryChallanNewMFN();
                CrpReport = new CrpDeliveryChallanNew();
                //ReportDocument subReport = CrpReport.OpenSubreport("Subreport1");
                CrpReport.SetDataSource(ds);
                //subReport.SetDataSource(ds);
                CrpReport.Refresh();
                CrpReport.SetParameterValue("COMPANY_NAME", strn[0]);
                CrpReport.SetParameterValue("CompanyAddress", dt.Rows[0]["ADDRESS1"]);
                Session.Add("CrpReport", CrpReport);
                Session.Add("ReportType", 0);
                const string url = "'Default.aspx'";
                const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                Type cstype = GetType();
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(cstype, "OpenWindow", script);
            }
            else if (DrpLedgerType.SelectedIndex == 4)
            {
                ds = DPrint.SelectDocumentforPrint(int.Parse(drpDistributor.SelectedValue), int.Parse(DrpArea.SelectedValue), PrincipalIDs,
                    DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), 1, Constants.LongNullValue, 0, PrincipalSingleId);
                var crpReport = new CrpPrintUSDInvoiceNew();
                crpReport.SetDataSource(ds);
                crpReport.Refresh();
                crpReport.SetParameterValue("COMPANY_NAME", strn[0]);
                crpReport.SetParameterValue("CompanyAddress", "Branch Address: " + dt.Rows[0]["ADDRESS1"].ToString());
                crpReport.SetParameterValue("TAXREGISTERATION_NO", dt.Rows[0]["GST_NUMBER"].ToString());
                crpReport.SetParameterValue("DISTRIBUTOR_NAME", dt.Rows[0]["DISTRIBUTOR_NAME"].ToString());
                Session.Add("CrpReport", crpReport);
                Session.Add("ReportType", 0);
                const string url = "'Default.aspx'";
                const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                Type cstype = GetType();
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(cstype, "OpenWindow", script);
            }            
            else
            {
                ds = DPrint.SelectDocumentforPrint(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpArea.SelectedValue.ToString()), PrincipalIDs,
                    DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(DrpLedgerType.SelectedValue.ToString()), Constants.LongNullValue, 0, PrincipalSingleId);
                //var crpReport = new CrpPrintDocumentNew();
                var crpReport = new CrpPrintDocumentA5();
                ReportDocument subReport = crpReport.OpenSubreport("SUBREPORT1");
                crpReport.SetDataSource(ds);
                subReport.SetDataSource(ds);
    
                crpReport.Refresh();

                int PrintNTN = 0;
                int PrintSTRN = 0;
                crpReport.SetParameterValue("COMPANY_NAME", strn[0]);
                crpReport.SetParameterValue("DISTRIBUTOR_NAME", dt.Rows[0]["DISTRIBUTOR_NAME"].ToString());
                crpReport.SetParameterValue("Address", "Address: " + dt.Rows[0]["ADDRESS1"].ToString());
                crpReport.SetParameterValue("EmailTo", dt.Rows[0]["ADDRESS2"].ToString());
                crpReport.SetParameterValue("CONTACT_NUMBER", dt.Rows[0]["CONTACT_NUMBER"].ToString());
                crpReport.SetParameterValue("NTN", "NTN #:" + dt.Rows[0]["NTN_NO"].ToString());
                if (dt.Rows[0]["NTN_NO"].ToString() == null || dt.Rows[0]["NTN_NO"].ToString() == "" || dt.Rows[0]["NTN_NO"].ToString() == "0")
                {
                    PrintNTN = 1;
                }
                else
                {
                    PrintNTN = 0;
                }
                if (dt.Rows[0]["GST_NUMBER"].ToString() == null || dt.Rows[0]["GST_NUMBER"].ToString() == "" || dt.Rows[0]["GST_NUMBER"].ToString() == "0")
                {
                    PrintSTRN = 1;
                }
                else
                {
                    PrintSTRN = 0;
                }
                crpReport.SetParameterValue("PrintNTN", PrintNTN);
                crpReport.SetParameterValue("PrintSTRN", PrintSTRN);
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
  
    protected void Button1_Click(object sender, EventArgs e)
    {
        DataTable  ds = null;
        DocumentPrintController DPrint = new DocumentPrintController();
        if (DrpLedgerType.SelectedIndex == 3)
        {
            ds = DPrint.SelectDocumentforView(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpArea.SelectedValue.ToString()), int.Parse(cblPrincipal.SelectedValue.ToString()),
                   DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"),1);
        }
        else
        {
            ds = DPrint.SelectDocumentforView(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpArea.SelectedValue.ToString()), int.Parse(cblPrincipal.SelectedValue.ToString()),
                       DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(DrpLedgerType.SelectedValue.ToString()));
        }

        GrdOrder.DataSource = ds;
        GrdOrder.DataBind();      
    }
    
    protected void GrdOrder_RowEditing(object sender, GridViewEditEventArgs e)
    {
        string PrincipalIDs = "";
        foreach (ListItem li in cblPrincipal.Items)
        {
            if (li.Selected)
            {
                PrincipalIDs += li.Value + ",";
            }
        }

        DocumentPrintController DPrint = new DocumentPrintController();
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));

        if (dt.Rows.Count > 0)
        {
            if (DrpLedgerType.SelectedIndex == 3)
            {
                DataControl dc = new DataControl();
                ds = DPrint.SelectDocumentforPrint(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpArea.SelectedValue.ToString()), PrincipalIDs,
                    DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), 1, long.Parse(GrdOrder.Rows[e.NewEditIndex].Cells[3].Text), 0, int.Parse(cblPrincipal.SelectedValue.ToString()));

                CrpDeliveryChallan CrpReport = new CrpDeliveryChallan();
                CrpReport.SetDataSource(ds);
                CrpReport.Refresh();
                CrpReport.SetParameterValue("CompanyAddress", "Branch Address: " + dt.Rows[0]["ADDRESS1"].ToString());
                Session.Add("CrpReport", CrpReport);
                Session.Add("ReportType", 0);  
                string url = "'Default.aspx'";
                string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                Type cstype = GetType();
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(cstype, "OpenWindow", script);

            }
            else
            {
                DataControl dc = new DataControl();
                ds = DPrint.SelectDocumentforPrint(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpArea.SelectedValue.ToString()), PrincipalIDs,
                    DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(DrpLedgerType.SelectedValue.ToString()), long.Parse(GrdOrder.Rows[e.NewEditIndex].Cells[3].Text), 0, int.Parse(cblPrincipal.SelectedValue.ToString()));

                ReportDocument SubReport = new ReportDocument();
                CrpPrintDocument CrpReport = new CrpPrintDocument();

                SubReport = CrpReport.OpenSubreport("SUBREPORT1");
                CrpReport.SetDataSource(ds);
                SubReport.SetDataSource(ds);
                CrpReport.Refresh();
                var strn = ddlCompany.SelectedItem.Text.Split('|');
                CrpReport.SetParameterValue("COMPANY_NAME", strn[0]);
                CrpReport.SetParameterValue("DISTRIBUTOR_NAME", dt.Rows[0]["DISTRIBUTOR_NAME"].ToString());

                Session.Add("CrpReport", CrpReport);
                Session.Add("ReportType", 0);  
                string url = "'Default.aspx'";
                string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                Type cstype = GetType();
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(cstype, "OpenWindow", script);
            }
        }
    }       
    
    protected void btnViewExcel_Click(object sender, EventArgs e)
    {
        string PrincipalIDs = "";
        foreach (ListItem li in cblPrincipal.Items)
        {
            if (li.Selected)
            {
                PrincipalIDs += li.Value + ",";
            }
        }

        DocumentPrintController DPrint = new DocumentPrintController();
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedValue.ToString()));
        if (dt.Rows.Count > 0)
        {
            var strn = ddlCompany.SelectedItem.Text.Split('|');
            if (DrpLedgerType.SelectedIndex == 5)
            {
                ds = DPrint.SelectDocumentforPrint(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpArea.SelectedValue.ToString()), PrincipalIDs,
                    DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(DrpLedgerType.SelectedValue.ToString()), Constants.LongNullValue, 0, int.Parse(cblPrincipal.SelectedValue.ToString()));
                var crpReport = new CrpPrintDocumentGST();
                crpReport.SetDataSource(ds);

                crpReport.Refresh();
                crpReport.SetParameterValue("COMPANY_NAME", strn[0]);
                crpReport.SetParameterValue("Address", "Address: " + dt.Rows[0]["ADDRESS1"].ToString());
                crpReport.SetParameterValue("EmailTo", dt.Rows[0]["ADDRESS2"].ToString());
                crpReport.SetParameterValue("CONTACT_NUMBER", dt.Rows[0]["CONTACT_NUMBER"].ToString());
                crpReport.SetParameterValue("NTN", "NTN #:" + dt.Rows[0]["NTN_NO"].ToString());
                crpReport.SetParameterValue("PrintType", 1);

                crpReport.SetParameterValue("STRN", "STRN #:" + strn[1]);

                Session.Add("CrpReport", crpReport);
                Session.Add("ReportType", 0);
                const string url = "'Default.aspx'";
                const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                Type cstype = GetType();
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(cstype, "OpenWindow", script);
            }
            else if (DrpLedgerType.SelectedIndex == 3)
            {
                DataControl dc = new DataControl();
                ds = DPrint.SelectDocumentforPrint(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpArea.SelectedValue.ToString()), PrincipalIDs,
                    DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), 1, Constants.LongNullValue, 0, int.Parse(cblPrincipal.SelectedValue.ToString()));

                ReportDocument SubReport = new ReportDocument();
                CrpDeliveryChallan CrpReport = new CrpDeliveryChallan();

                SubReport = CrpReport.OpenSubreport("Subreport1");
                CrpReport.SetDataSource(ds);
                SubReport.SetDataSource(ds);
                CrpReport.Refresh();
                CrpReport.SetParameterValue("COMPANY_NAME", strn[0]);
                CrpReport.SetParameterValue("CompanyAddress", "Branch Address: " + dt.Rows[0]["ADDRESS1"].ToString());
                Session.Add("CrpReport", CrpReport);
                Session.Add("ReportType", 1);
                string url = "'Default.aspx'";
                string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                Type cstype = GetType();
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(cstype, "OpenWindow", script);

            }
            else if (DrpLedgerType.SelectedIndex == 4)
            {

                DataControl dc = new DataControl();
                ds = DPrint.SelectDocumentforPrint(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpArea.SelectedValue.ToString()), PrincipalIDs,
                    DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), 1, Constants.LongNullValue, 0, int.Parse(cblPrincipal.SelectedValue.ToString()));

                CrpPrintUSDInvoice CrpReport = new CrpPrintUSDInvoice();
                CrpReport.SetDataSource(ds);
                CrpReport.Refresh();

                CrpReport.SetParameterValue("COMPANY_NAME", strn[0]);
                CrpReport.SetParameterValue("CompanyAddress", "Branch Address: " + dt.Rows[0]["ADDRESS1"].ToString());
                CrpReport.SetParameterValue("TAXREGISTERATION_NO", dt.Rows[0]["GST_NUMBER"].ToString());
                CrpReport.SetParameterValue("DISTRIBUTOR_NAME", dt.Rows[0]["DISTRIBUTOR_NAME"].ToString());
                Session.Add("CrpReport", CrpReport);
                Session.Add("ReportType", 1);
                string url = "'Default.aspx'";
                string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                Type cstype = GetType();
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(cstype, "OpenWindow", script);


            }            
            else
            {
                DataControl dc = new DataControl();
                ds = DPrint.SelectDocumentforPrint(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpArea.SelectedValue.ToString()), PrincipalIDs,
                    DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(DrpLedgerType.SelectedValue.ToString()), Constants.LongNullValue, 0, int.Parse(cblPrincipal.SelectedValue.ToString()));

                ReportDocument SubReport = new ReportDocument();
                CrpPrintDocument CrpReport = new CrpPrintDocument();

                SubReport = CrpReport.OpenSubreport("SUBREPORT1");
                CrpReport.SetDataSource(ds);
                SubReport.SetDataSource(ds);
                CrpReport.Refresh();

                

                CrpReport.SetParameterValue("COMPANY_NAME", strn[0]);
                CrpReport.SetParameterValue("DISTRIBUTOR_NAME", dt.Rows[0]["DISTRIBUTOR_NAME"].ToString());
                CrpReport.SetParameterValue("Address", "Address: " + dt.Rows[0]["ADDRESS1"].ToString());
                CrpReport.SetParameterValue("EmailTo", dt.Rows[0]["ADDRESS2"].ToString());
                CrpReport.SetParameterValue("CONTACT_NUMBER", dt.Rows[0]["CONTACT_NUMBER"].ToString());
                CrpReport.SetParameterValue("NTN", "NTN #:" + dt.Rows[0]["NTN_NO"].ToString());
                CrpReport.SetParameterValue("PrintType", 0);
                
                CrpReport.SetParameterValue("STRN", "STRN #:" + strn[1]);

                Session.Add("CrpReport", CrpReport);
                Session.Add("ReportType", 1);
                string url = "'Default.aspx'";
                string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
                Type cstype = GetType();
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(cstype, "OpenWindow", script);
            }

        }
    }    
}