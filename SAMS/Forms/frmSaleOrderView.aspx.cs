using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSCommon.Classes;
using SAMSBusinessLayer.Classes;
using System.Data;
using SAMSBusinessLayer.Reports;

public partial class Forms_frmSaleOrderView : System.Web.UI.Page
{

    readonly OrderEntryController oeCtrl = new OrderEntryController();
   
    private DataTable m_SKUDt;
    private DataTable dtInvoice;


    bool ViewAccess;
    protected void Page_Load(object sender, EventArgs e)
    {
        ViewAccess = ValidateScreen();
        if (ViewAccess == false)
        {
            Response.Redirect("../Forms/AccessDenied.aspx");
        }
        if (!IsPostBack)
        {
            Session.Remove("orderId");
            Session.Remove("invoiceId");
            LoadData();
            LoadInvoice();
            LoadGrid();
            
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
    private void LoadData()
    {
        OrderEntryController or = new OrderEntryController();
        DataTable dtOrder = or.SelectPendingOrder(Convert.ToInt32(Session["Distributor_id"].ToString()), Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.Order_Pending_Id, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Convert.ToDateTime(this.Session["CurrentWorkDate"]),0);

        Session.Add("m_SKUDt", dtOrder);
    }

    private void LoadInvoice()
    {
        OrderEntryController or = new OrderEntryController();

        DataTable dtInvoice = or.SelectPendingOrder(Convert.ToInt32(Session["Distributor_id"].ToString()), Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.Order_Pending_Id, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Convert.ToDateTime(this.Session["CurrentWorkDate"]), 1);

        Session.Add("dtInvoice", dtInvoice);

        grdInvoice.DataSource = dtInvoice;
        grdInvoice.DataBind();
    }
    
    private void LoadGrid()
    {
        m_SKUDt = (DataTable)Session["m_SKUDt"];
        if (m_SKUDt != null)
        {

            switch (ddSearchType.SelectedIndex)
            {

                case 1:
                    m_SKUDt.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                case 2:
                    m_SKUDt.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                default:
                    m_SKUDt.DefaultView.RowFilter = "CUSTOMER_NAME" + " like '%" + "" + "%'";
                    break;
            }

            grdSaleOrder.DataSource = m_SKUDt.DefaultView;
            grdSaleOrder.DataBind();
        }


        dtInvoice = (DataTable)Session["dtInvoice"];

        if (dtInvoice != null)
        {
            switch (ddSearchType.SelectedIndex)
            {
                case 1:
                    dtInvoice.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                case 2:
                    dtInvoice.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                default:
                    dtInvoice.DefaultView.RowFilter = "CUSTOMER_NAME" + " like '%" + "" + "%'";
                    break;
            }

            grdInvoice.DataSource = dtInvoice.DefaultView;
            grdInvoice.DataBind();
        }
        
    }

    
    protected void grdSaleOrder_RowEditing(object sender, GridViewEditEventArgs e)
    {

        Session.Add("orderId", grdSaleOrder.Rows[e.NewEditIndex].Cells[6].Text);

        //Response.Redirect("frmDelieveryOrderAdd.aspx?LevelID=34&LevelType=3&TopID=26");
        Response.Redirect("frmCustomerOrderDelivery.aspx?LevelID=" + Request.QueryString["LevelID"].ToString() + "&LevelType=" + Request.QueryString["LevelType"].ToString()+"&TopID=26");
        
    }
    protected void grdSaleOrder_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
       // int m_sku_id = int.Parse(grdSaleOrder.Rows[e.RowIndex].Cells[0].Text);
        this.LoadData();
        this.LoadGrid();
    }

    protected void grdInvoice_RowEditing(object sender, GridViewEditEventArgs e)
    {

        Session.Add("invoiceId", grdInvoice.Rows[e.NewEditIndex].Cells[6].Text);

        Response.Redirect("frmCustomerOrderDelivery.aspx?LevelID=" + Request.QueryString["LevelID"].ToString() + "&LevelType=" + Request.QueryString["LevelType"].ToString()+"&TopID=26");
    }

    protected void grdInvoice_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        // int m_sku_id = int.Parse(grdSaleOrder.Rows[e.RowIndex].Cells[0].Text);
        this.LoadData();
        this.LoadGrid();
    }

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        LoadGrid();
        
    }

    private void GetInvoice(long sId)
    {
        try
        {
            //DocumentPrintController DPrint = new DocumentPrintController();
            //DataTable dt = DPrint.SelectReportTitle(Convert.ToInt32(Session["Distributor_id"].ToString()));

            //DataSet ds = null;

            //ds = oeCtrl.SelectOrderforView(sId, 1);
            ////if (ds.Rows.Count <= 0) return;

            //CrpInvoicePrinting crpOrder = new CrpInvoicePrinting();
            //crpOrder.SetDataSource(ds);
            //crpOrder.Refresh();

            //crpOrder.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString().ToUpper());
            //crpOrder.SetParameterValue("CompanyAddress", dt.Rows[0]["ADDRESS1"].ToString());
            //crpOrder.SetParameterValue("ContactNo", dt.Rows[0]["CONTACT_NUMBER"].ToString());
            ////---------------------------------------------------------------------------------\\

            ////---------------------------------------------------------------------------------\\


            //Session.Add("CrpReport", crpOrder);
            //Session.Add("ReportType", 0);

            ////const string url = "'Default.aspx'";
            ////Response.Write("<script>window.open(" + url + ",'_blank');</script>");
            //Session.Add("CrpReport", crpOrder);
            //Session.Add("ReportType", 0);
            //const string url = "'Default.aspx'";
            //const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
            //Type cstype = GetType();
            //ClientScriptManager cs = Page.ClientScript;
            //cs.RegisterStartupScript(cstype, "OpenWindow", script);


        }
        catch (Exception ex)
        {
            ex.ToString();
        }
    }
}