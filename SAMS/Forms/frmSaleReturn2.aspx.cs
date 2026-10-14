using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;

/// <summary>
/// Form To Rollback Order, Invoice, Sale Return And Realized Cheque
/// </summary>
public partial class Forms_frmSaleReturn2 : System.Web.UI.Page
{
    /// <summary>
    /// Page_Load Function Populates All Combos And Grid On The Page
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            this.LoadDistributor();
            this.LoadOrderBooker();
            txtFromDate.Text = Convert.ToDateTime(Session["CurrentWorkDate"]).ToString("dd-MMM-yyyy");
            txtToDate.Text = Convert.ToDateTime(Session["CurrentWorkDate"]).ToString("dd-MMM-yyyy");
            drpDistributor.Focus();
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
    /// Load OrderBookers To OrderBooker Combo
    /// </summary>
    private void LoadOrderBooker()
    {
        if (drpDistributor.Items.Count > 0)
        {
            SaleForceController mDController = new SaleForceController();
            DataTable m_dt = mDController.SelectSaleForceAssignedArea(Constants.SALES_FORCE_ORDERBOOKER, int.Parse(drpDistributor.SelectedValue.ToString()), Constants.IntNullValue, int.Parse(this.Session["CompanyId"].ToString()), Constants.IntNullValue);
            clsWebFormUtil.FillDropDownList(this.DrpOrderBooker, m_dt, 0, 3, true);
        }
        else
        {
            DrpOrderBooker.Items.Clear();
        }
    }
    
    /// <summary>
    /// Loads Rollback Order, Invoice And Sale Return Data To Grid
    /// </summary>
    private void LoadInvoices()
    {
        OrderEntryController or = new OrderEntryController();
        DataTable dtOrder = or.GetDSRInvoices(int.Parse(drpDistributor.SelectedValue.ToString()),int.Parse(DrpOrderBooker.SelectedValue.ToString()),Convert.ToDateTime(txtFromDate.Text),Convert.ToDateTime(txtToDate.Text));
        GrdOrder.DataSource = dtOrder;
        GrdOrder.DataBind();
    }
    
    /// <summary>
    /// Loads OrderBookers To OrderBooker Combo And Principals To Principal Combo
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadOrderBooker();         
    }

    /// <summary>
    /// Rollbacks Order, Invoice, Sale Return And Realized Cheque
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnReturn_Click(object sender, EventArgs e)
    {
        DataTable dtMaster = new DataTable();
        DataTable dtDetail = new DataTable();
        DataTable dtFreeSKU = new DataTable();
        bool flag = false;
        foreach (GridViewRow dr in GrdOrder.Rows)
        {
            CheckBox ChbInvoice = (CheckBox)dr.FindControl("ChbInvoice");
            if (ChbInvoice.Checked == true)
            {
                OrderEntryController ORD = new OrderEntryController();
                dtMaster = ORD.GetInvoicesDetail(Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToInt64(GrdOrder.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), 1);
                if(dtMaster.Rows.Count > 0)
                {
                    dtDetail = ORD.GetInvoicesDetail(Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToInt64(GrdOrder.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), 2);
                    dtFreeSKU = ORD.GetInvoicesDetail(Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToInt64(GrdOrder.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), 3);
                    flag = ORD.Add_SaleReturnInvoiceWise(Convert.ToInt64(GrdOrder.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToInt32(dtMaster.Rows[0]["TOWN_ID"]), Convert.ToInt64(dtMaster.Rows[0]["AREA_ID"]), Convert.ToInt32(dtMaster.Rows[0]["PRINCIPAL_ID"]), Convert.ToInt64(dtMaster.Rows[0]["SOLD_TO"])
                        , Convert.ToInt32(dtMaster.Rows[0]["ORDERBOOKER_ID"]), Convert.ToInt32(dtMaster.Rows[0]["DELIVERYMAN_ID"]), Convert.ToDecimal(dtMaster.Rows[0]["TOTAL_AMOUNT"]), Convert.ToDecimal(dtMaster.Rows[0]["EXTRA_DISCOUNT_AMOUNT"]), Convert.ToDecimal(dtMaster.Rows[0]["STANDARD_DISCOUNT_AMOUNT"])
                        , Convert.ToDecimal(dtMaster.Rows[0]["GST_AMOUNT"]), Convert.ToDecimal(dtMaster.Rows[0]["TOTAL_NET_AMOUNT"]), Convert.ToInt32(Session["UserID"]), Convert.ToDateTime(Session["CurrentWorkDate"]), Convert.ToDecimal(dtMaster.Rows[0]["TST_AMOUNT"])
                        , Convert.ToDecimal(dtMaster.Rows[0]["SED_AMOUNT"]), Convert.ToDecimal(dtMaster.Rows[0]["EXTRA_TAX"]), Convert.ToDecimal(dtMaster.Rows[0]["STANDARD_DISCOUNT_BY_VALUE"]), Convert.ToDecimal(dtMaster.Rows[0]["STANDARD_DISCOUNT_PER_VALUE"]),Convert.ToDecimal(dtMaster.Rows[0]["SED_AMOUNT_BY_VALUE"]), Convert.ToDecimal(dtMaster.Rows[0]["SED_AMOUNT_PER_VALUE"]), dtDetail, dtFreeSKU, Convert.ToDecimal(dtMaster.Rows[0]["STANDARD_DISCOUNT_BY_VALUE_TO"]), Convert.ToDecimal(dtMaster.Rows[0]["STANDARD_DISCOUNT_BY_VALUE_AD"]), Convert.ToDecimal(dtMaster.Rows[0]["STANDARD_DISCOUNT_PER_VALUE_TO"]), Convert.ToDecimal(dtMaster.Rows[0]["STANDARD_DISCOUNT_PER_VALUE_AD"]), Convert.ToDecimal(dtMaster.Rows[0]["SED_AMOUNT_BY_VALUE_TO"]), Convert.ToDecimal(dtMaster.Rows[0]["SED_AMOUNT_BY_VALUE_AD"]), Convert.ToDecimal(dtMaster.Rows[0]["SED_AMOUNT_PER_VALUE_TO"]), Convert.ToDecimal(dtMaster.Rows[0]["SED_AMOUNT_PER_VALUE_AD"]));
                }
            }
        }
        this.LoadInvoices();
    }

    /// <summary>
    /// Loads Order, Invoice, Sale Return And Realized Cheque Data To Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnGetOrder_Click(object sender, EventArgs e)
    {
        GrdOrder.DataSource = null;
        GrdOrder.DataBind();        
        GrdOrder.Visible = true;
        this.LoadInvoices();
    }
}