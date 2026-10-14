using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;

public partial class Forms_frmGiftSKUNew : System.Web.UI.Page
{
private static int  prvqntity;
    private static long SaleInvoicePromotionId;
    private void LoadPrincipal()
    {
        SKUPriceDetailController PController = new SKUPriceDetailController();
        DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, m_dt, 0, 1, true);
    }
    private void LoadDistributor()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDropDownList(this.drpDistributor, dt, 0, 2, true);
    }
    private void LoadInvoiceNo()
    {
        DocumentPrintController DPrint = new DocumentPrintController();
        drpDocumentNo.Items.Clear();   
        
        DataTable dtOrder = DPrint.SelectDocumentforView(int.Parse(drpDistributor.SelectedValue.ToString()), Constants.IntNullValue, int.Parse(DrpPrincipal.SelectedValue.ToString()),
                       DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()),1);
                 
        clsWebFormUtil.FillDropDownList(this.drpDocumentNo, dtOrder, 8, 9);
        this.Session.Add("dtOrder", dtOrder);  

    }
    private void LoadCustomerDetail()
    {
        if (drpDocumentNo.Items.Count > 0)
        {
            DataTable dt = (DataTable)this.Session["dtOrder"];
            DataRow[] foundRows = dt.Select("DocumentID  = '" + drpDocumentNo.SelectedItem.Text + "'");
            if (foundRows.Length > 0)
            {

                txtOutletCode.Text = foundRows[0]["CUSTOMER_CODE"].ToString();
                txtOutletName.Text = foundRows[0]["SOLD_TO"].ToString();
                ScriptManager.GetCurrent(Page).SetFocus(ddlSKU);
            }
        }
    }
    private void LoadSKUDetail()
    {
        SKUPriceDetailController PController = new SKUPriceDetailController();
        if (DrpPrincipal.Items.Count    > 0)
        {
            DataTable Dtsku_Price = PController.SelectDataPrice(int.Parse(DrpPrincipal.SelectedValue.ToString()), Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 1, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
            clsWebFormUtil.FillDropDownList(this.ddlSKU, Dtsku_Price, "SKU_ID", "SKUDetail", true);
            this.Session.Add("Dtsku_Price", Dtsku_Price);
        }
    }
    private void LoadFreeSKU()
    {
        if (drpDocumentNo.Items.Count > 0)
        {
            OrderEntryController or = new OrderEntryController();
            DataTable dt = or.SelectInvoicePromotion(int.Parse(drpDistributor.SelectedValue.ToString()), long.Parse(drpDocumentNo.SelectedValue.ToString()));
            GrdPurchase.DataSource = dt;
            GrdPurchase.DataBind();  
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {           
            this.LoadDistributor();
            this.LoadPrincipal();
            this.LoadSKUDetail();
            this.LoadInvoiceNo();
            this.LoadCustomerDetail();
            this.LoadFreeSKU();           
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (drpDocumentNo.Items.Count > 0)
        {
            OrderEntryController or = new OrderEntryController();
            DataControl dc = new DataControl();
            string msg = "";
            bool stockavail = true;
            DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
            DataRow[] foundRows = Dtsku_Price.Select("SKU_ID = '" + ddlSKU.SelectedValue + "'");
            PhaysicalStockController mController = new PhaysicalStockController();
            DataTable dtstock = mController.SelectSKUClosingStock(int.Parse(drpDistributor.SelectedValue), int.Parse(foundRows[0]["SKU_ID"].ToString()), "N/A", DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), 16);
            int Qty = int.Parse(txtQuantity.Text);
            if (btnSave.Text == "Update")
            {
                Qty = Qty - prvqntity;
            }
            
           
            if (dtstock.Rows.Count > 0)
            {
                if (int.Parse(dtstock.Rows[0][0].ToString()) < Qty)
                {
                    msg = msg + foundRows[0]["SKU_CODE"].ToString() + " Current Stock is " + dtstock.Rows[0][0].ToString() + " \\n";
                    stockavail = false;
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert(' + " + foundRows[0]["SKU_Code"].ToString() + "No Stock Found');", true);
                return;
            }
            if (!stockavail)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('" + msg.ToString() + "');", true);
                return;
            }
            if (foundRows.Length > 0)
            {
                decimal mTradePrice = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString()));
                float mTaxPrice = float.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString()));
                float mSEDPrice = float.Parse(dc.chkNull_0(foundRows[0]["SED_PRICE"].ToString()));
                decimal mAmount = mTradePrice * int.Parse(txtQuantity.Text);
                decimal mTaxAmt = 0;
                decimal mTSTAmt = 0;
                decimal mSEDAmt = 0;
                if (ChbAllTax.Checked == true)
                {
                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                    {
                        mTaxAmt = (decimal.Parse(mTaxPrice.ToString()) / 100) * mAmount;
                        mSEDAmt = (decimal.Parse(mSEDPrice.ToString()) / 100) * mAmount;

                    }
                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                    {
                        mTSTAmt = (decimal.Parse(mTaxPrice.ToString())) * int.Parse(txtQuantity.Text);
                        mSEDAmt = 0;
                        //mSEDAmt = (decimal.Parse(mSEDPrice.ToString())) * int.Parse(txtQuantity.Text); 
                    }
                            
                    
                }
                if (btnSave.Text == "Add Sku")
                {
                    or.InsertFreeSKU(int.Parse(drpDistributor.SelectedValue.ToString()), long.Parse(drpDocumentNo.SelectedValue.ToString()), int.Parse(foundRows[0]["SKU_ID"].ToString()), int.Parse(txtQuantity.Text)
                    , mTradePrice, mAmount, mTaxPrice, mTaxAmt, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), mTSTAmt, mSEDAmt);
                }
                else if (btnSave.Text == "Update")
                {
                    or.DeleteFreeSKUFromInvoice(int.Parse(drpDistributor.SelectedValue.ToString()), long.Parse(drpDocumentNo.SelectedValue.ToString()), SaleInvoicePromotionId, int.Parse(ddlSKU.SelectedValue), prvqntity);

                    or.InsertFreeSKU(int.Parse(drpDistributor.SelectedValue.ToString()), long.Parse(drpDocumentNo.SelectedValue.ToString()), int.Parse(foundRows[0]["SKU_ID"].ToString()), int.Parse(txtQuantity.Text)
                  , mTradePrice, mAmount, mTaxPrice, mTaxAmt, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), mTSTAmt, mSEDAmt);
                }
                this.LoadFreeSKU();
                txtQuantity.Text = "";
                txtUnitRate.Text = "";
                btnSave.Text = "Add Sku";
                drpDistributor.Enabled = true;
                drpDocumentNo.Enabled = true;
                DrpPrincipal.Enabled = true;
                ScriptManager.GetCurrent(Page).SetFocus(ddlSKU);

            }
        }
                
    }
    protected void Button1_Click(object sender, EventArgs e)
    {

    }
    protected void Button2_Click(object sender, EventArgs e)
    {

    }
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadInvoiceNo();
    }
    protected void DrpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadSKUDetail();
        this.LoadInvoiceNo();
        this.LoadFreeSKU();
        this.LoadCustomerDetail();
    }
    protected void drpDocumentNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadFreeSKU();
        this.LoadCustomerDetail();
    }
    protected void GrdPurchase_RowEditing(object sender, GridViewEditEventArgs e)
    {
        drpDistributor.Enabled = false;
        DrpPrincipal.Enabled = false;
        drpDocumentNo.Enabled = false;
         SaleInvoicePromotionId = long.Parse(GrdPurchase.Rows[e.NewEditIndex].Cells[0].Text);
        ddlSKU.SelectedValue= (GrdPurchase.Rows[e.NewEditIndex].Cells[1].Text);
        txtQuantity.Text= (GrdPurchase.Rows[e.NewEditIndex].Cells[4].Text);
        txtBatchNo.Text = (GrdPurchase.Rows[e.NewEditIndex].Cells[6].Text);
        prvqntity=int.Parse (GrdPurchase.Rows[e.NewEditIndex].Cells[4].Text);
        btnSave.Text = "Update";
    }
    protected void GrdPurchase_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        OrderEntryController or = new OrderEntryController();
        or.DeleteFreeSKUFromInvoice(int.Parse(drpDistributor.SelectedValue.ToString()), long.Parse(drpDocumentNo.SelectedValue.ToString()), long.Parse(GrdPurchase.Rows[e.RowIndex].Cells[0].Text), int.Parse(GrdPurchase.Rows[e.RowIndex].Cells[1].Text), int.Parse(GrdPurchase.Rows[e.RowIndex].Cells[4].Text));
        this.LoadFreeSKU(); 
    }
}
