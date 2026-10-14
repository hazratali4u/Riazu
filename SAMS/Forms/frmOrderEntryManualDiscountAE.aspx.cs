using System;
using System.Data;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;

public partial class Forms_frmOrderEntryManualDiscountAE : System.Web.UI.Page
{
    #region Variables
    DataTable PurchaseSKU;
    DataTable dtFreeSKU;
    private static int mCustomerTypeId;
    private static int mCustomerVolClassId;
    private int mTownId;
    private static int RowId;
    private static int OrderNo;
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            int OrderNo = (int)this.Session["OrderNo"];
            txtprincipal.Text = this.Session["Route"].ToString();
            txtDeliveryMan.Text = this.Session["SaleMan"].ToString();
            this.Session.Remove("Route");
            this.Session.Remove("SaleMan");
            ScriptManager.GetCurrent(Page).SetFocus(txtBillBookNo);
            if (OrderNo == -1)
            {
                btnSaveOrder.Text = "Save Invoice";
                drpDocumentNo.Enabled = false;
            }
            else if (OrderNo == -2)
            {
                drpDocumentNo.Enabled = false;
                ChbDiscount.Visible = true;
                btnSaveOrder.Text = "Sale Return";
                lblBillBook.Visible = false;
                txtBillBookNo.Visible = false;
            }
            this.LoadCustomerData();
            this.LoadSKUDetail();
            this.CreatTable();
            this.LoadPromotion();
            this.CreateFreeSKU();
            this.LoadFreeGrid();
            this.LoadPendingOrder();
            btnSave.Attributes.Add("onclick", "return ValidateForm();");
        }
    }
    private void CreatTable()
    {
        PurchaseSKU = new DataTable();
        PurchaseSKU.Columns.Add("SALE_ORDER_DETAIL_ID", typeof(long));
        PurchaseSKU.Columns.Add("DistributorId", typeof(int));
        PurchaseSKU.Columns.Add("SALE_ORDER_ID", typeof(int));
        PurchaseSKU.Columns.Add("SKU_ID", typeof(int));
        PurchaseSKU.Columns.Add("SKU_Code", typeof(string));
        PurchaseSKU.Columns.Add("SKU_Name", typeof(string));
        PurchaseSKU.Columns.Add("BATCH_NO", typeof(string));
        PurchaseSKU.Columns.Add("UNIT_PRICE", typeof(decimal));
        PurchaseSKU.Columns.Add("QUANTITY_UNIT", typeof(int));
        PurchaseSKU.Columns.Add("QUANTITY_CTN", typeof(decimal));
        PurchaseSKU.Columns.Add("QUANTITY", typeof(decimal));
        PurchaseSKU.Columns.Add("AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("STANDARD_DISCOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("STANDARD_DISCOUNT_PER", typeof(decimal));
        PurchaseSKU.Columns.Add("EXTRA_DISCOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("RETAIL_AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("GST_RATE", typeof(decimal));
        PurchaseSKU.Columns.Add("GST_AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("TST_AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("EXTRA_TAX", typeof(decimal));
        PurchaseSKU.Columns.Add("CLAIM_EXTRA_AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("CLAIM_STANDARD_DISCOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("CLAIM_PER", typeof(decimal));
        PurchaseSKU.Columns.Add("SED_AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("NET_AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("COST_OF_GOODS", typeof(decimal));
        PurchaseSKU.Columns.Add("IS_DELETED", typeof(bool));
        PurchaseSKU.Columns.Add("DISTRIBUTOR_PRICE", typeof(decimal));
        PurchaseSKU.Columns.Add("FREE_PRICE", typeof(decimal));
        PurchaseSKU.Columns.Add("PRINCIPAL_ID", typeof(int));
        this.Session.Add("PurchaseSKU", PurchaseSKU);
    }
    private void CreateFreeSKU()
    {
        dtFreeSKU = new DataTable();
        dtFreeSKU.Columns.Add("SKU_ID", typeof(int));
        dtFreeSKU.Columns.Add("SKU_Code", typeof(string));
        dtFreeSKU.Columns.Add("SKU_Name", typeof(string));
        dtFreeSKU.Columns.Add("UNIT_PRICE", typeof(decimal));
        dtFreeSKU.Columns.Add("Quantity", typeof(int));
        dtFreeSKU.Columns.Add("AMOUNT", typeof(decimal));
        dtFreeSKU.Columns.Add("GST_RATE", typeof(decimal));
        dtFreeSKU.Columns.Add("GST_AMOUNT", typeof(decimal));
        dtFreeSKU.Columns.Add("TST_AMOUNT", typeof(decimal));
        dtFreeSKU.Columns.Add("PROMOTION_ID", typeof(int));
        dtFreeSKU.Columns.Add("BASKET_ID", typeof(int));
        dtFreeSKU.Columns.Add("BASKET_DETAIL_ID", typeof(int));
        dtFreeSKU.Columns.Add("PROMOTION_OFFER_ID", typeof(int));
        this.Session.Add("dtFreeSKU", dtFreeSKU);
    }
    private void LoadPendingOrder()
    {
        OrderEntryController or = new OrderEntryController();
        this.drpDocumentNo.Items.Clear();
        //DataTable dtOrder = or.SelectPendingOrder(int.Parse(this.Session["DistributorId"].ToString()), int.Parse(this.Session["AreaId"].ToString()), int.Parse(this.Session["PrincipalId"].ToString()), int.Parse(this.Session["OrderBookerId"].ToString()), int.Parse(this.Session["DeliveryManId"].ToString()), Constants.Order_Pending_Id, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Convert.ToDateTime(this.Session["OrderDate"]));
        DataTable dtOrder = or.SelectPendingOrderMultiSelection(int.Parse(this.Session["DistributorId"].ToString()), null, Constants.IntNullValue, int.Parse(this.Session["OrderBookerId"].ToString()), int.Parse(this.Session["DeliveryManId"].ToString()), Constants.Order_Pending_Id, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Convert.ToDateTime(this.Session["OrderDate"]), 0);
        drpDocumentNo.Items.Add(new clsListItems("New", Constants.LongNullValue.ToString()));
        clsWebFormUtil.FillDropDownList(this.drpDocumentNo, dtOrder, "SALE_ORDER_ID", "CUSTOMER_DETAIL");
        this.Session.Add("dtOrder", dtOrder);
    }
    protected void drpDocumentNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (long.Parse(drpDocumentNo.SelectedValue.ToString()) != Constants.LongNullValue)
        {
            txtBillBookNo.Text = string.Empty;
            DataTable dt = (DataTable)this.Session["dtOrder"];
            DataRow[] foundRows = dt.Select("SALE_ORDER_ID  = '" + drpDocumentNo.SelectedValue + "'");
            if (foundRows.Length > 0)
            {
                txtGrossAmount.Text = foundRows[0]["TOTAL_AMOUNT"].ToString();
                numTxtTotalGST.Text = foundRows[0]["GST_AMOUNT"].ToString();
                numTxtTotlAmnt.Text = foundRows[0]["TOTAL_NET_AMOUNT"].ToString();
                numTxtTotalTST.Text = foundRows[0]["TST_AMOUNT"].ToString();
                drpcustomer.SelectedValue = foundRows[0]["CUSTOMER_ID"].ToString();
                numtxtTotalExtraDiscnt.Text = foundRows[0]["STANDARD_DISCOUNT_AMOUNT"].ToString();
                numtxtUnClaimabledist.Text = foundRows[0]["EXTRA_DISCOUNT_AMOUNT"].ToString();
                numTxtTotalExtra_tax.Text = foundRows[0]["EXTRA_TAX"].ToString();
                this.Session.Add("CUSTOMER_ID", long.Parse(foundRows[0]["CUSTOMER_ID"].ToString()));
                this.ExistenOrderDetail(long.Parse(drpDocumentNo.SelectedValue.ToString()));
                txtBillBookNo.Text = foundRows[0]["MANUAL_ORDER_ID"].ToString();
                this.Session.Add("hfBillBookNo", foundRows[0]["MANUAL_ORDER_ID"].ToString());
                txtINcentive.Text = foundRows[0]["INCENTIVE"].ToString();
                txtVBRDDiscount.Text = foundRows[0]["SED_AMOUNT"].ToString();
                txtTradeOfferDisc.Text = foundRows[0]["TRADE_OFFER"].ToString();
                txtRentalDiscount.Text = foundRows[0]["RENTAL"].ToString();
                txtDisplayDiscount.Text = foundRows[0]["DISPLAY"].ToString();
                txtBRDDiscount.Text = foundRows[0]["BRD"].ToString();
                numTxtTotalStndrdDiscnt.Text = foundRows[0]["WHOLE_SALE_DISC"].ToString();
                EnableDisableController(false);
                btnSaveOrder.Text = "Save Invoice";
                btnSaveOrder.Enabled = false;
                btnCalculate.Enabled = false;
                this.ClearAll();
                ScriptManager.GetCurrent(Page).SetFocus(drpskuDetail);
                RblPayMode.SelectedValue = foundRows[0]["ORDER_TYPE_ID"].ToString();
                drpcustomer_SelectedIndexChanged(null, null);
            }
        }
        else
        {
            this.CreateFreeSKU();
            this.CreatTable();
            this.LoadGrid();
            this.LoadFreeGrid();
            ClearMasterALL();
            btnSaveOrder.Text = "Save Order";
            this.Session.Remove("hfBillBookNo");
        }
    }
    private void LoadCustomerData()
    {
        if (int.Parse(this.Session["DistributorId"].ToString()) > 0 && this.Session["AreaId"].ToString() != "")
        {
            drpcustomer.Items.Clear();
            CustomerDataController mController = new CustomerDataController();
            //DataTable dtCustomer = mController.SelectPrincipalCustomer(int.Parse(this.Session["DistributorId"].ToString()), int.Parse(this.Session["AreaId"].ToString()), Constants.IntNullValue, int.Parse(this.Session["PrincipalId"].ToString()));
            DataTable dtCustomer = mController.SelectPrincipalCustomer4MultiSelection(int.Parse(this.Session["DistributorId"].ToString()), this.Session["AreaId"].ToString(), Constants.IntNullValue, int.Parse(this.Session["PrincipalId"].ToString()), 5);
            clsWebFormUtil.FillDropDownList(this.drpcustomer, dtCustomer, "CUSTOMER_ID", "CUSTOMER_DETAIL", true);
            this.Session.Add("dtCustomer", dtCustomer);
            drpcustomer_SelectedIndexChanged(null, null);
        }
        else
        { drpcustomer.Items.Clear(); }
    }
    private void LoadSKUDetail()
    {
        drpskuDetail.Items.Clear();
        SKUPriceDetailController PController = new SKUPriceDetailController();
        if (int.Parse(this.Session["PrincipalId"].ToString()) > 0)
        {
            DataTable Dtsku_Price = PController.SelectDataPrice(int.Parse(this.Session["PrincipalId"].ToString()), Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["DistributorId"].ToString()), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 1, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
            clsWebFormUtil.FillDropDownList(this.drpskuDetail, Dtsku_Price, 0, 10, true);
            this.Session.Add("Dtsku_Price", Dtsku_Price);
        }
    }
    private void LoadPromotion()
    {
        if (int.Parse(this.Session["PrincipalId"].ToString()) > 0)
        {
            SKUPriceDetailController PController = new SKUPriceDetailController();
            DataTable dt = PController.SelectDataPrice(int.Parse(this.Session["PrincipalId"].ToString()), Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["OrderDate"].ToString()));
            if (bool.Parse(dt.Rows[0]["Is_ManualDiscount"].ToString()) == false)
            {
                OrderEntryController orderController = new OrderEntryController();
                PromotionCollections_Controller Arpc = orderController.LoadSchemes(int.Parse(this.Session["DistributorId"].ToString()), int.Parse(this.Session["PrincipalId"].ToString()), Convert.ToDateTime(this.Session["OrderDate"]));
                this.Session.Add("Arpc", Arpc);
                txtDiscountType.Text = "Auto Discount";
                ChbDiscount.Checked = true;
            }
            else
            {
                OrderEntryController orderController = new OrderEntryController();
                PromotionCollections_Controller Arpc = orderController.LoadSchemes(int.Parse(this.Session["DistributorId"].ToString()), int.Parse(this.Session["PrincipalId"].ToString()), Convert.ToDateTime(this.Session["OrderDate"]));
                this.Session.Add("Arpc", Arpc);
                txtDiscountType.Text = "Manual Discount";
                numTxtTotalStndrdDiscnt.ReadOnly = false;
                numtxtTotalExtraDiscnt.ReadOnly = false;
                ChbDiscount.Checked = false;
            }
        }
    }
    private bool DistributorIsRegistered()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.DistributorInfo(int.Parse(this.Session["DistributorId"].ToString()), Constants.IntNullValue, Constants.IntNullValue);
        if (dt.Rows.Count > 0)
        {
            DataRow[] foundRows = dt.Select("DISTRIBUTOR_ID  = " + int.Parse(this.Session["DistributorId"].ToString()) + "");
            if (foundRows.Length > 0)
            {
                if (bool.Parse(foundRows[0]["IS_REGISTERED"].ToString()) == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool CustomerIsRegistered()
    {
        DataTable dtCustomer = (DataTable)this.Session["dtCustomer"];
        DataRow[] foundRows = dtCustomer.Select("CUSTOMER_ID  = '" + drpcustomer.SelectedValue.Trim() + "'");
        if (foundRows.Length > 0)
        {
            if (bool.Parse(foundRows[0]["IS_GST_REGISTERED"].ToString()) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }
    private void LoadGrid()
    {
        decimal ObjStandardDiscount = 0;
        decimal ObjExtraDiscount = 0;
        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
        GrdPurchase.DataSource = PurchaseSKU;
        GrdPurchase.DataBind();
        foreach (DataRow row in PurchaseSKU.Rows)
        {
            ObjStandardDiscount += Convert.ToDecimal(row["STANDARD_DISCOUNT"]);
            ObjExtraDiscount += Convert.ToDecimal(row["EXTRA_DISCOUNT"]);
        }
        numtxtTotalExtraDiscnt.Text = Convert.ToString(Math.Round(ObjExtraDiscount, 4));
        numtxtUnClaimabledist.Text = Convert.ToString(Math.Round(ObjStandardDiscount, 4));
    }
    protected void GrdPurchase_RowEditing(object sender, GridViewEditEventArgs e)
    {
        DataControl dc = new DataControl();
        DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        RowId = e.NewEditIndex;
        drpskuDetail.SelectedValue = GrdPurchase.Rows[e.NewEditIndex].Cells[0].Text;
        txtQuantityCtn.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[3].Text;
        txtQuantity.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[4].Text;
        txtUnitRate.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[5].Text;
        txtDiscount.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[6].Text;
        DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + drpskuDetail.SelectedValue + "'");
        int mPackSize = int.Parse(dc.chkNull_0(foundRows[0]["UNITS_IN_CASE"].ToString()));
        txtTO.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[7].Text; //Math.Round(Convert.ToDecimal(GrdPurchase.Rows[e.NewEditIndex].Cells[7].Text) / ((decimal.Parse(dc.chkNull_0(txtQuantity.Text)) + (decimal.Parse(dc.chkNull_0(txtQuantityCtn.Text)) * mPackSize)) / mPackSize), 2).ToString();
        drpskuDetail.Enabled = false;
        btnSave.Text = "Update SKU";
    }
    protected void GrdPurchase_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
        PurchaseSKU.Rows.RemoveAt(e.RowIndex);
        this.Session.Add("PurchaseSKU", PurchaseSKU);
        this.LoadGrid();
        btnCalculate.Enabled = true;
        btnSaveOrder.Enabled = false;
    }
    private void LoadFreeGrid()
    {
        dtFreeSKU = (DataTable)this.Session["dtFreeSKU"];
        GrdFreeSKU.DataSource = dtFreeSKU;
        GrdFreeSKU.DataBind();
    }
    private bool FindCustomer()
    {
        DataTable dtCustomer = (DataTable)this.Session["dtCustomer"];
        DataRow[] foundRows = dtCustomer.Select("CUSTOMER_ID = '" + drpcustomer.SelectedValue + "'");
        if (foundRows.Length > 0)
        {
            this.Session.Add("CUSTOMER_ID", long.Parse(foundRows[0]["CUSTOMER_ID"].ToString()));
            mCustomerTypeId = int.Parse(foundRows[0]["CHANNEL_TYPE_ID"].ToString());
            mCustomerVolClassId = int.Parse(foundRows[0]["VOLUME_CLASS_ID"].ToString());
            mTownId = int.Parse(foundRows[0]["TOWN_ID"].ToString());
            return true;
        }
        return false;
    }
    private bool CheckDublicateSKU()
    {
        DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
        DataRow[] foundRows = PurchaseSKU.Select("SKU_ID = '" + drpskuDetail.SelectedValue + "'");
        if (foundRows.Length == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private int FreeSKUExist(DataTable dt, int Sku_id)
    {
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (int.Parse(dt.Rows[i]["SKU_ID"].ToString()) == Sku_id)
            {
                return i;
            }
        }
        return -1;
    }
    private void ExistenOrderDetail(long OrderId)
    {
        OrderEntryController ord = new OrderEntryController();
        PurchaseSKU = ord.SelectOrderDetail(int.Parse(this.Session["DistributorId"].ToString()), OrderId);
        dtFreeSKU = ord.SelectOrderPromotion(int.Parse(this.Session["DistributorId"].ToString()), OrderId);
        this.Session.Add("PurchaseSKU", PurchaseSKU);
        this.Session.Add("dtFreeSKU", dtFreeSKU);
        this.LoadGrid();
        this.LoadFreeGrid();
    }
    private bool IsBillBookNoExist()
    {
        bool flag = false;
        if ((long.Parse(drpDocumentNo.SelectedValue) == Constants.LongNullValue || this.Session["hfBillBookNo"].ToString() != txtBillBookNo.Text) && txtBillBookNo.Text.Trim().Length > 0)
        {
            OrderEntryController OEC = new OrderEntryController();
            DataTable dtBillBookNo = OEC.SelectBillBookNo(Convert.ToInt32(this.Session["DistributorId"].ToString()), txtBillBookNo.Text, 0);
            if (dtBillBookNo.Rows.Count > 0)
            {
                flag = true;
            }
            DataTable dtBillBookNo2 = OEC.SelectBillBookNo(Convert.ToInt32(this.Session["DistributorId"].ToString()), txtBillBookNo.Text, 1);
            if (dtBillBookNo2.Rows.Count > 0)
            {
                flag = true;
            }
        }
        return flag;
    }
    private void EnableDisableController(bool CValue)
    {
        if (CValue == true)
        {
            if (decimal.Parse(txtGrossAmount.Text) > 0)
            {
                drpcustomer.Enabled = true;
            }
        }
        else
        {
            drpcustomer.Enabled = false;
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        this.Session.Remove("CUSTOMER_ID");
        this.Session.Remove("PrincipalId");
        this.Session.Remove("AreaId");
        this.Session.Remove("OrderBookerId");
        this.Session.Remove("DeliveryManId");
        Response.Redirect("~/Forms/frmOrderEntryManualDiscountStep1AE.aspx?Status=" + false + "&LevelType=3&LevelID=" + Request.QueryString["LevelID"].ToString());
    }
    protected void btnCalculate_Click(object sender, EventArgs e)
    {
        if (FindCustomer())
        {
            decimal ObjGrossSales = 0;
            decimal ObjStandardDiscount = 0;
            decimal ObjExtraDiscount = 0;
            decimal ObjTotalGST = 0;
            decimal ObjTotalSED = 0;
            decimal ObjTotalEXTRA_TAX = 0;
            decimal ObjTotalTST = 0;
            decimal ObjFreeGSTAmount = 0;
            decimal ObjFreeGrossAmt = 0;
            decimal ObjFreeTST = 0;
            decimal ObjNetSales = 0;
            decimal ProductExtDist = 0;
            decimal ProductStdDist = 0;
            DataControl dc = new DataControl();
            OrderEntryController or = new OrderEntryController();
            SKUGroupController GroupCtl = new SKUGroupController();
            DataTable dt = (DataTable)this.Session["PurchaseSKU"];
            foreach (DataRow dr in dt.Rows)
            {
                //dr["STANDARD_DISCOUNT"] = 0;
                //dr["STANDARD_DISCOUNT_PER"] = 0;
                //dr["EXTRA_DISCOUNT"] = 0;
                dr["CLAIM_EXTRA_AMOUNT"] = 0;
                dr["CLAIM_STANDARD_DISCOUNT"] = 0;
                dr["CLAIM_PER"] = 0;
                dr["SED_AMOUNT"] = 0;
            }
            PromotionCollections_Controller Arpc = (PromotionCollections_Controller)this.Session["Arpc"];
            this.CreateFreeSKU();
            this.LoadFreeGrid();
            if (dt.Rows.Count > 0 && drpcustomer.Items.Count > 0)
            {
                if (ChbDiscount.Checked == true)
                {
                    #region 
                    ArrayList ARlistOffer = or.GetPromotionOffers(Arpc, mCustomerVolClassId, mCustomerTypeId, dt, false);
                    for (int i = 0; i < ARlistOffer.Count; i++)
                    {
                        PromoOffers_Collection pofferCol = (PromoOffers_Collection)ARlistOffer[i];
                        if (pofferCol.Is_Claimable)
                        {
                            if (pofferCol.SKU_ID > 0)
                            {
                                #region slab on SKU
                                foreach (DataRow dr in dt.Rows)
                                {
                                    if (int.Parse(dr["SKU_ID"].ToString()) == pofferCol.SKU_ID)
                                    {
                                        if (pofferCol.Is_And == true)
                                        {
                                            if (pofferCol.Offer_Value > 0)
                                            {
                                                dr["EXTRA_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())) + pofferCol.Offer_Value;
                                                dr["CLAIM_EXTRA_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString())) + pofferCol.Offer_Value;
                                            }
                                            if (pofferCol.Discount > 0)
                                            {
                                                dr["EXTRA_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100) * (decimal.Parse(dr["AMOUNT"].ToString()));
                                                dr["CLAIM_EXTRA_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100) * (decimal.Parse(dr["AMOUNT"].ToString()));
                                            }
                                        }
                                        else
                                        {
                                            if (pofferCol.Offer_Value > 0)
                                            {
                                                //dr["STANDARD_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString())) + pofferCol.Offer_Value;
                                                //dr["CLAIM_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_AMOUNT"].ToString())) + pofferCol.Offer_Value;
                                            }
                                            if (pofferCol.Discount > 0)
                                            {
                                                dr["STANDARD_DISCOUNT_PER"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT_PER"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100);
                                                dr["CLAIM_PER"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_PER"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100);
                                            }
                                        }
                                        if (pofferCol.Free_SKU_ID > 0)
                                        {
                                            #region Free sku
                                            DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
                                            DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
                                            int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
                                            if (foundRows.Length > 0)
                                            {
                                                if (RowId < 0)
                                                {
                                                    DataRow dr1 = dtFreeSKU.NewRow();
                                                    dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
                                                    dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
                                                    dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
                                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        // dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                        dr1["GST_AMOUNT"] = 0;
                                                        dr1["TST_AMOUNT"] = 0;
                                                    }
                                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        //  dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        dr1["GST_AMOUNT"] = 0;
                                                        dr1["TST_AMOUNT"] = 0;
                                                    }
                                                    else
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        dr1["GST_AMOUNT"] = 0;
                                                        dr1["TST_AMOUNT"] = 0;
                                                    }
                                                    dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
                                                    dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
                                                    dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
                                                    dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
                                                    ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
                                                    ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
                                                    ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
                                                    dtFreeSKU.Rows.Add(dr1);
                                                }
                                                else
                                                {
                                                    DataRow dr1 = dtFreeSKU.Rows[RowId];
                                                    ObjFreeGrossAmt = ObjFreeGrossAmt - decimal.Parse(dr1["AMOUNT"].ToString());
                                                    ObjFreeGSTAmount = ObjFreeGSTAmount - decimal.Parse(dr1["GST_AMOUNT"].ToString());
                                                    dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
                                                    dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
                                                    dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
                                                    dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;
                                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        //    dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                        dr1["TST_AMOUNT"] = 0;
                                                        dr1["GST_AMOUNT"] = 0;
                                                    }
                                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        // dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        dr1["TST_AMOUNT"] = 0;
                                                        dr1["GST_AMOUNT"] = 0;
                                                    }
                                                    else
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                    }
                                                    dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
                                                    dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
                                                    dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
                                                    dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
                                                    ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
                                                    ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
                                                    ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
                                                }
                                            }
                                            #endregion
                                        }
                                        this.Session.Add("dtFreeSKU", dtFreeSKU);
                                        this.LoadFreeGrid();
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region slab on SKU Group
                                #region Free SKU
                                if (pofferCol.Free_SKU_ID > 0)
                                {
                                    DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
                                    DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
                                    int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
                                    if (foundRows.Length > 0)
                                    {
                                        if (RowId < 0)
                                        {
                                            DataRow dr1 = dtFreeSKU.NewRow();
                                            dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
                                            dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
                                            dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
                                            dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                            dr1["Quantity"] = pofferCol.Quantity.ToString();
                                            if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                //  dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                dr1["TST_AMOUNT"] = 0;
                                                dr1["GST_AMOUNT"] = 0;
                                            }
                                            else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                //    dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                dr1["TST_AMOUNT"] = 0;
                                                dr1["GST_AMOUNT"] = 0;
                                            }
                                            else
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                dr1["GST_AMOUNT"] = 0;
                                                dr1["TST_AMOUNT"] = 0;
                                            }
                                            dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
                                            dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
                                            dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
                                            dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
                                            ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
                                            ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
                                            ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
                                            dtFreeSKU.Rows.Add(dr1);
                                        }
                                        else
                                        {
                                            DataRow dr1 = dtFreeSKU.Rows[RowId];
                                            ObjFreeGrossAmt = ObjFreeGrossAmt - decimal.Parse(dr1["AMOUNT"].ToString());
                                            ObjFreeGSTAmount = ObjFreeGSTAmount - decimal.Parse(dr1["GST_AMOUNT"].ToString());
                                            dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
                                            dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
                                            dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
                                            dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;
                                            if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                //  dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                dr1["GST_AMOUNT"] = 0;
                                                dr1["TST_AMOUNT"] = 0;
                                            }
                                            else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                //   dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                dr1["TST_AMOUNT"] = 0;
                                                dr1["GST_AMOUNT"] = 0;
                                            }
                                            else
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                dr1["GST_AMOUNT"] = 0;
                                                dr1["TST_AMOUNT"] = 0;
                                            }
                                            dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
                                            dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
                                            dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
                                            dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
                                            ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
                                            ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
                                            ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
                                        }
                                        this.Session.Add("dtFreeSKU", dtFreeSKU);
                                        this.LoadFreeGrid();
                                    }
                                }
                                #endregion
                                foreach (DataRow dr in dt.Rows)
                                {
                                    if (GroupCtl.ExistsInGroup(Constants.IntNullValue, pofferCol.Group_ID, int.Parse(dr["SKU_ID"].ToString())))
                                    {
                                        if (pofferCol.Is_And == true)
                                        {
                                            if (pofferCol.Offer_Value > 0)
                                            {
                                                dr["EXTRA_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())) + pofferCol.Offer_Value;
                                                dr["CLAIM_EXTRA_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString())) + pofferCol.Offer_Value;
                                            }
                                            if (pofferCol.Discount > 0)
                                            {
                                                dr["EXTRA_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100) * (decimal.Parse(dr["AMOUNT"].ToString()));
                                                dr["CLAIM_EXTRA_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100) * (decimal.Parse(dr["AMOUNT"].ToString()));
                                            }
                                        }
                                        else
                                        {
                                            if (pofferCol.Offer_Value > 0)
                                            {
                                                //dr["STANDARD_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString())) + pofferCol.Offer_Value;
                                                //dr["CLAIM_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_AMOUNT"].ToString())) + pofferCol.Offer_Value;
                                            }
                                            if (pofferCol.Discount > 0)
                                            {
                                                dr["STANDARD_DISCOUNT_PER"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT_PER"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100);
                                                dr["CLAIM_PER"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_PER"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100);
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            if (pofferCol.SKU_ID > 0)
                            {
                                #region slab on SKU
                                foreach (DataRow dr in dt.Rows)
                                {
                                    if (int.Parse(dr["SKU_ID"].ToString()) == pofferCol.SKU_ID)
                                    {

                                        if (pofferCol.Is_And == true)
                                        {
                                            if (pofferCol.Offer_Value > 0)
                                            {
                                                dr["EXTRA_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())) + pofferCol.Offer_Value;


                                            }
                                            if (pofferCol.Discount > 0)
                                            {
                                                dr["EXTRA_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100) * (decimal.Parse(dr["AMOUNT"].ToString()));
                                            }
                                        }
                                        else
                                        {
                                            if (pofferCol.Offer_Value > 0)
                                            {
                                                dr["STANDARD_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString())) + pofferCol.Offer_Value;
                                            }
                                            if (pofferCol.Discount > 0)
                                            {
                                                dr["STANDARD_DISCOUNT_PER"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT_PER"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100);
                                            }
                                        }
                                        if (pofferCol.Free_SKU_ID > 0)
                                        {
                                            DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
                                            DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
                                            int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
                                            if (foundRows.Length > 0)
                                            {
                                                if (RowId < 0)
                                                {
                                                    DataRow dr1 = dtFreeSKU.NewRow();
                                                    dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
                                                    dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
                                                    dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];

                                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                                    {

                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        // dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                        dr1["GST_AMOUNT"] = 0;
                                                        dr1["TST_AMOUNT"] = 0;

                                                    }
                                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        //    dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        dr1["TST_AMOUNT"] = 0;
                                                        dr1["GST_AMOUNT"] = 0;
                                                    }
                                                    else
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        dr1["GST_AMOUNT"] = 0;
                                                        dr1["TST_AMOUNT"] = 0;
                                                    }

                                                    dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
                                                    dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
                                                    dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
                                                    dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
                                                    ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
                                                    ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
                                                    ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
                                                    dtFreeSKU.Rows.Add(dr1);
                                                }
                                                else
                                                {

                                                    DataRow dr1 = dtFreeSKU.Rows[RowId];
                                                    ObjFreeGrossAmt = ObjFreeGrossAmt - decimal.Parse(dr1["AMOUNT"].ToString());
                                                    ObjFreeGSTAmount = ObjFreeGSTAmount - decimal.Parse(dr1["GST_AMOUNT"].ToString());
                                                    dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
                                                    dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
                                                    dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
                                                    dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;

                                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                                    {

                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        //    dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                        dr1["GST_AMOUNT"] = 0;
                                                        dr1["TST_AMOUNT"] = 0;

                                                    }
                                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        //   dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        dr1["TST_AMOUNT"] = 0;
                                                        dr1["GST_AMOUNT"] = 0;
                                                    }
                                                    else
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                    }
                                                    dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
                                                    dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
                                                    dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
                                                    dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
                                                    ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
                                                    ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
                                                    ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
                                                }

                                            }
                                        }
                                        this.Session.Add("dtFreeSKU", dtFreeSKU);
                                        this.LoadFreeGrid();
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region slab on SKU Group

                                #region Free SKU
                                if (pofferCol.Free_SKU_ID > 0)
                                {
                                    DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
                                    DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
                                    int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
                                    if (foundRows.Length > 0)
                                    {
                                        if (RowId < 0)
                                        {
                                            DataRow dr1 = dtFreeSKU.NewRow();
                                            dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
                                            dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
                                            dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
                                            dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                            dr1["Quantity"] = pofferCol.Quantity.ToString();
                                            if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                            {

                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                //  dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                dr1["GST_AMOUNT"] = 0;
                                                dr1["TST_AMOUNT"] = 0;

                                            }
                                            else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                // dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                dr1["TST_AMOUNT"] = 0;
                                                dr1["GST_AMOUNT"] = 0;
                                            }
                                            else
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                dr1["GST_AMOUNT"] = 0;
                                                dr1["TST_AMOUNT"] = 0;
                                            }
                                            dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
                                            dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
                                            dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
                                            dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
                                            ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
                                            ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
                                            ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
                                            dtFreeSKU.Rows.Add(dr1);
                                        }
                                        else
                                        {

                                            DataRow dr1 = dtFreeSKU.Rows[RowId];
                                            ObjFreeGrossAmt = ObjFreeGrossAmt - decimal.Parse(dr1["AMOUNT"].ToString());
                                            ObjFreeGSTAmount = ObjFreeGSTAmount - decimal.Parse(dr1["GST_AMOUNT"].ToString());
                                            dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
                                            dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
                                            dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
                                            dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;
                                            if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                            {

                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                //   dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                dr1["GST_AMOUNT"] = 0;
                                                dr1["TST_AMOUNT"] = 0;

                                            }
                                            else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                // dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                dr1["TST_AMOUNT"] = 0;
                                                dr1["GST_AMOUNT"] = 0;
                                            }
                                            else
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                dr1["GST_AMOUNT"] = 0;
                                                dr1["TST_AMOUNT"] = 0;
                                            }
                                            dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
                                            dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
                                            dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
                                            dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
                                            ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
                                            ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
                                            ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
                                        }
                                        this.Session.Add("dtFreeSKU", dtFreeSKU);
                                        this.LoadFreeGrid();

                                    }
                                }
                                #endregion


                                foreach (DataRow dr in dt.Rows)
                                {


                                    if (GroupCtl.ExistsInGroup(Constants.IntNullValue, pofferCol.Group_ID, int.Parse(dr["SKU_ID"].ToString())))
                                    {


                                        if (pofferCol.Is_And == true)
                                        {
                                            if (pofferCol.Offer_Value > 0)
                                            {
                                                dr["EXTRA_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())) + pofferCol.Offer_Value;
                                            }
                                            if (pofferCol.Discount > 0)
                                            {
                                                dr["EXTRA_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())) + ((decimal.Parse(pofferCol.Discount.ToString())) / 100) * (decimal.Parse(dr["AMOUNT"].ToString()) - decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())));
                                            }
                                        }
                                        else
                                        {
                                            if (pofferCol.Offer_Value > 0)
                                            {
                                                dr["STANDARD_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString())) + pofferCol.Offer_Value;
                                            }
                                            if (pofferCol.Discount > 0)
                                            {
                                                dr["STANDARD_DISCOUNT_PER"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT_PER"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100);
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }

                    #region Recalulate Products

                    foreach (DataRow dr in dt.Rows)
                    {
                        decimal TempAmount = 0;
                        decimal TempDiscount = 0;
                        decimal TempExtraDisAmt = 0;
                        decimal TempNetAmount = 0;

                        decimal ObjClaimAblePer = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            ObjGrossSales += decimal.Parse(dc.chkNull_0(row["AMOUNT"].ToString()));
                        }
                        if (decimal.Parse(dc.chkNull_0(numtxtUnClaimabledist.Text)) > 0)
                        {
                            ObjClaimAblePer = decimal.Parse(dc.chkNull_0(numtxtUnClaimabledist.Text)) / ObjGrossSales;
                        }

                        dr["STANDARD_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT_PER"].ToString())) * (decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString())) - decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())));
                        dr["CLAIM_STANDARD_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_PER"].ToString())) * (decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString())) - decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString())));
                        dr["SED_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_STANDARD_DISCOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString()));

                        
                        #region GST Calculation
                        TempAmount = decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        TempDiscount = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString()));
                        TempExtraDisAmt = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString()));
                        TempNetAmount =
                                          TempAmount
                                        - TempDiscount
                                        - TempExtraDisAmt
                                        - decimal.Parse(dc.chkNull_0(txtINcentive.Text))
                                        - decimal.Parse(dc.chkNull_0(txtBRDDiscount.Text))
                                        - decimal.Parse(dc.chkNull_0(numTxtTotalStndrdDiscnt.Text))
                                        - decimal.Parse(dc.chkNull_0(txtVBRDDiscount.Text))
                                        - decimal.Parse(dc.chkNull_0(txtTradeOfferDisc.Text))
                                        - decimal.Parse(dc.chkNull_0(txtDisplayDiscount.Text))
                                        //- decimal.Parse(dc.chkNull_0(numtxtTotalExtraDiscnt.Text))
                                        //- decimal.Parse(dc.chkNull_0(numtxtUnClaimabledist.Text))
                                        - decimal.Parse(dc.chkNull_0(txtRentalDiscount.Text));
                        // Calculate Extra Tax By M. IJAZ JAMIL AKHTAR
                        #region EXTRA TAX
                        if (DistributorIsRegistered())
                        {
                            if (!CustomerIsRegistered())
                            {
                                if (dr["BATCH_NO"].ToString() == "T")
                                {
                                    decimal Discount = (TempNetAmount * 3) / 100;
                                    dr["EXTRA_TAX"] = Discount;
                                }
                                else
                                {
                                    dr["EXTRA_TAX"] = 0;
                                }
                            }
                            else
                            {
                                dr["EXTRA_TAX"] = 0;
                            }
                        }
                        else
                        {
                            dr["EXTRA_TAX"] = 0;
                        }
                        #endregion


                        dr["SED_AMOUNT"] = ObjClaimAblePer * decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        dr["GST_AMOUNT"] = (TempNetAmount * decimal.Parse(dc.chkNull_0(dr["GST_RATE"].ToString()))) / 100;
                        dr["NET_AMOUNT"] = TempNetAmount + decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["EXTRA_TAX"].ToString()));
                        ObjTotalGST += decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString()));
                        ObjTotalTST += decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));
                        #endregion

                        //ObjGrossSales += decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        ObjStandardDiscount += decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString()));
                        ObjExtraDiscount += decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString()));
                        ObjTotalSED += decimal.Parse(dc.chkNull_0(dr["SED_AMOUNT"].ToString()));
                        ObjTotalEXTRA_TAX += decimal.Parse(dc.chkNull_0(dr["EXTRA_TAX"].ToString()));
                        ObjNetSales += decimal.Parse(dc.chkNull_0(dr["NET_AMOUNT"].ToString()));
                    }

                    #endregion
                    #endregion
                    numTxtTotalStndrdDiscnt.Text = Convert.ToString(Math.Round(ObjStandardDiscount, 4));
                    txtTradeOfferDisc.Text = Convert.ToString(Math.Round(ObjExtraDiscount, 4));
                }
                else // Manual Discount
                {
                    // safdar start
                    #region 
                    ArrayList ARlistOffer = or.GetPromotionOffers(Arpc, mCustomerVolClassId, mCustomerTypeId, dt, false);
                    for (int i = 0; i < ARlistOffer.Count; i++)
                    {
                        PromoOffers_Collection pofferCol = (PromoOffers_Collection)ARlistOffer[i];
                        if (pofferCol.Is_Claimable)
                        {
                            if (pofferCol.SKU_ID > 0)
                            {
                                #region slab on SKU
                                foreach (DataRow dr in dt.Rows)
                                {
                                    if (int.Parse(dr["SKU_ID"].ToString()) == pofferCol.SKU_ID)
                                    {
                                        if (pofferCol.Is_And == true)
                                        {
                                            //if (pofferCol.Offer_Value > 0)
                                            //{
                                            //    dr["EXTRA_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())) + pofferCol.Offer_Value;
                                            //    dr["CLAIM_EXTRA_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString())) + pofferCol.Offer_Value;
                                            //}
                                            //if (pofferCol.Discount > 0)
                                            //{
                                            //    dr["EXTRA_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100) * (decimal.Parse(dr["AMOUNT"].ToString()));
                                            //    dr["CLAIM_EXTRA_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100) * (decimal.Parse(dr["AMOUNT"].ToString()));
                                            //}
                                        }
                                        else
                                        {
                                            //if (pofferCol.Offer_Value > 0)
                                            //{
                                            //    //dr["STANDARD_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString())) + pofferCol.Offer_Value;
                                            //    //dr["CLAIM_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_AMOUNT"].ToString())) + pofferCol.Offer_Value;
                                            //}
                                            //if (pofferCol.Discount > 0)
                                            //{
                                            //    dr["STANDARD_DISCOUNT_PER"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT_PER"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100);
                                            //    dr["CLAIM_PER"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_PER"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100);
                                            //}
                                        }
                                        if (pofferCol.Free_SKU_ID > 0)
                                        {
                                            #region Free sku
                                            DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
                                            DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
                                            int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
                                            if (foundRows.Length > 0)
                                            {
                                                if (RowId < 0)
                                                {
                                                    DataRow dr1 = dtFreeSKU.NewRow();
                                                    dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
                                                    dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
                                                    dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
                                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        // dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                        dr1["GST_AMOUNT"] = 0;
                                                        dr1["TST_AMOUNT"] = 0;
                                                    }
                                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        // dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        dr1["TST_AMOUNT"] = 0;
                                                        dr1["GST_AMOUNT"] = 0;
                                                    }
                                                    else
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        dr1["GST_AMOUNT"] = 0;
                                                        dr1["TST_AMOUNT"] = 0;
                                                    }
                                                    dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
                                                    dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
                                                    dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
                                                    dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
                                                    ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
                                                    ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
                                                    ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
                                                    dtFreeSKU.Rows.Add(dr1);
                                                }
                                                else
                                                {
                                                    DataRow dr1 = dtFreeSKU.Rows[RowId];
                                                    ObjFreeGrossAmt = ObjFreeGrossAmt - decimal.Parse(dr1["AMOUNT"].ToString());
                                                    ObjFreeGSTAmount = ObjFreeGSTAmount - decimal.Parse(dr1["GST_AMOUNT"].ToString());
                                                    dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
                                                    dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
                                                    dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
                                                    dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;
                                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        // dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                        dr1["TST_AMOUNT"] = 0;
                                                        dr1["GST_AMOUNT"] = 0;
                                                    }
                                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        // dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        dr1["TST_AMOUNT"] = 0;
                                                        dr1["GST_AMOUNT"] = 0;
                                                    }
                                                    else
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                    }
                                                    dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
                                                    dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
                                                    dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
                                                    dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
                                                    ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
                                                    ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
                                                    ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
                                                }
                                            }
                                            #endregion
                                        }
                                        this.Session.Add("dtFreeSKU", dtFreeSKU);
                                        this.LoadFreeGrid();
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region slab on SKU Group
                                #region Free SKU
                                if (pofferCol.Free_SKU_ID > 0)
                                {
                                    DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
                                    DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
                                    int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
                                    if (foundRows.Length > 0)
                                    {
                                        if (RowId < 0)
                                        {
                                            DataRow dr1 = dtFreeSKU.NewRow();
                                            dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
                                            dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
                                            dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
                                            dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                            dr1["Quantity"] = pofferCol.Quantity.ToString();
                                            if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                //  dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                dr1["TST_AMOUNT"] = 0;
                                                dr1["GST_AMOUNT"] = 0;

                                            }
                                            else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                // dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                dr1["GST_AMOUNT"] = 0;
                                                dr1["TST_AMOUNT"] = 0;
                                            }
                                            else
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                dr1["GST_AMOUNT"] = 0;
                                                dr1["TST_AMOUNT"] = 0;
                                            }
                                            dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
                                            dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
                                            dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
                                            dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
                                            ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
                                            ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
                                            ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
                                            dtFreeSKU.Rows.Add(dr1);
                                        }
                                        else
                                        {
                                            DataRow dr1 = dtFreeSKU.Rows[RowId];
                                            ObjFreeGrossAmt = ObjFreeGrossAmt - decimal.Parse(dr1["AMOUNT"].ToString());
                                            ObjFreeGSTAmount = ObjFreeGSTAmount - decimal.Parse(dr1["GST_AMOUNT"].ToString());
                                            dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
                                            dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
                                            dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
                                            dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;
                                            if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                // dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                dr1["TST_AMOUNT"] = 0;
                                                dr1["GST_AMOUNT"] = 0;
                                            }
                                            else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                // dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                dr1["GST_AMOUNT"] = 0;
                                                dr1["TST_AMOUNT"] = 0;
                                            }
                                            else
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                dr1["GST_AMOUNT"] = 0;
                                                dr1["TST_AMOUNT"] = 0;
                                            }
                                            dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
                                            dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
                                            dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
                                            dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
                                            ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
                                            ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
                                            ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
                                        }
                                        this.Session.Add("dtFreeSKU", dtFreeSKU);
                                        this.LoadFreeGrid();
                                    }
                                }
                                #endregion
                                //foreach (DataRow dr in dt.Rows)
                                //{
                                //    if (GroupCtl.ExistsInGroup(Constants.IntNullValue, pofferCol.Group_ID, int.Parse(dr["SKU_ID"].ToString())))
                                //    {
                                //        if (pofferCol.Is_And == true)
                                //        {
                                //            if (pofferCol.Offer_Value > 0)
                                //            {
                                //                dr["EXTRA_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())) + pofferCol.Offer_Value;
                                //                dr["CLAIM_EXTRA_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString())) + pofferCol.Offer_Value;
                                //            }
                                //            if (pofferCol.Discount > 0)
                                //            {
                                //                dr["EXTRA_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100) * (decimal.Parse(dr["AMOUNT"].ToString()));
                                //                dr["CLAIM_EXTRA_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100) * (decimal.Parse(dr["AMOUNT"].ToString()));
                                //            }
                                //        }
                                //        else
                                //        {
                                //            if (pofferCol.Offer_Value > 0)
                                //            {
                                //                //dr["STANDARD_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString())) + pofferCol.Offer_Value;
                                //                //dr["CLAIM_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_AMOUNT"].ToString())) + pofferCol.Offer_Value;
                                //            }
                                //            if (pofferCol.Discount > 0)
                                //            {
                                //                dr["STANDARD_DISCOUNT_PER"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT_PER"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100);
                                //                dr["CLAIM_PER"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_PER"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100);
                                //            }
                                //        }
                                //    }
                                //}
                                #endregion
                            }
                        }
                        else
                        {
                            if (pofferCol.SKU_ID > 0)
                            {
                                #region slab on SKU
                                foreach (DataRow dr in dt.Rows)
                                {
                                    if (int.Parse(dr["SKU_ID"].ToString()) == pofferCol.SKU_ID)
                                    {
                                        if (pofferCol.Is_And == true)
                                        {
                                            //if (pofferCol.Offer_Value > 0)
                                            //{
                                            //    dr["EXTRA_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())) + pofferCol.Offer_Value;
                                            //}
                                            //if (pofferCol.Discount > 0)
                                            //{
                                            //    dr["EXTRA_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100) * (decimal.Parse(dr["AMOUNT"].ToString()));
                                            //}
                                        }
                                        else
                                        {
                                            //if (pofferCol.Offer_Value > 0)
                                            //{
                                            //    dr["STANDARD_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString())) + pofferCol.Offer_Value;
                                            //}
                                            //if (pofferCol.Discount > 0)
                                            //{
                                            //    dr["STANDARD_DISCOUNT_PER"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT_PER"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100);
                                            //}
                                        }
                                        if (pofferCol.Free_SKU_ID > 0)
                                        {
                                            DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
                                            DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
                                            int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
                                            if (foundRows.Length > 0)
                                            {
                                                if (RowId < 0)
                                                {
                                                    DataRow dr1 = dtFreeSKU.NewRow();
                                                    dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
                                                    dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
                                                    dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
                                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        //   dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                        dr1["TST_AMOUNT"] = 0;
                                                        dr1["GST_AMOUNT"] = 0;
                                                    }
                                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        //  dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        dr1["GST_AMOUNT"] = 0;
                                                        dr1["TST_AMOUNT"] = 0;
                                                    }
                                                    else
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        dr1["GST_AMOUNT"] = 0;
                                                        dr1["TST_AMOUNT"] = 0;
                                                    }
                                                    dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
                                                    dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
                                                    dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
                                                    dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
                                                    ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
                                                    ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
                                                    ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
                                                    dtFreeSKU.Rows.Add(dr1);
                                                }
                                                else
                                                {
                                                    DataRow dr1 = dtFreeSKU.Rows[RowId];
                                                    ObjFreeGrossAmt = ObjFreeGrossAmt - decimal.Parse(dr1["AMOUNT"].ToString());
                                                    ObjFreeGSTAmount = ObjFreeGSTAmount - decimal.Parse(dr1["GST_AMOUNT"].ToString());
                                                    dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
                                                    dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
                                                    dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
                                                    dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;
                                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        //    dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                        dr1["TST_AMOUNT"] = 0;
                                                        dr1["GST_AMOUNT"] = 0;
                                                    }
                                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        //    dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        dr1["GST_AMOUNT"] = 0;
                                                        dr1["TST_AMOUNT"] = 0;
                                                    }
                                                    else
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                    }
                                                    dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
                                                    dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
                                                    dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
                                                    dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
                                                    ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
                                                    ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
                                                    ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
                                                }
                                            }
                                        }
                                        this.Session.Add("dtFreeSKU", dtFreeSKU);
                                        this.LoadFreeGrid();
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region slab on SKU Group
                                #region Free SKU
                                if (pofferCol.Free_SKU_ID > 0)
                                {
                                    DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
                                    DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
                                    int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
                                    if (foundRows.Length > 0)
                                    {
                                        if (RowId < 0)
                                        {
                                            DataRow dr1 = dtFreeSKU.NewRow();
                                            dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
                                            dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
                                            dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
                                            dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                            dr1["Quantity"] = pofferCol.Quantity.ToString();
                                            if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                //  dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                dr1["TST_AMOUNT"] = 0;
                                                dr1["GST_AMOUNT"] = 0;
                                            }
                                            else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                //    dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                dr1["GST_AMOUNT"] = 0;
                                                dr1["TST_AMOUNT"] = 0;
                                            }
                                            else
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                dr1["GST_AMOUNT"] = 0;
                                                dr1["TST_AMOUNT"] = 0;
                                            }
                                            dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
                                            dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
                                            dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
                                            dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
                                            ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
                                            ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
                                            ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
                                            dtFreeSKU.Rows.Add(dr1);
                                        }
                                        else
                                        {
                                            DataRow dr1 = dtFreeSKU.Rows[RowId];
                                            ObjFreeGrossAmt = ObjFreeGrossAmt - decimal.Parse(dr1["AMOUNT"].ToString());
                                            ObjFreeGSTAmount = ObjFreeGSTAmount - decimal.Parse(dr1["GST_AMOUNT"].ToString());
                                            dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
                                            dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
                                            dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
                                            dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;
                                            if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                //     dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                dr1["TST_AMOUNT"] = 0;
                                                dr1["GST_AMOUNT"] = 0;
                                            }
                                            else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                // dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                dr1["GST_AMOUNT"] = 0;
                                                dr1["TST_AMOUNT"] = 0;
                                            }
                                            else
                                            {
                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                dr1["GST_AMOUNT"] = 0;
                                                dr1["TST_AMOUNT"] = 0;
                                            }
                                            dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
                                            dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
                                            dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
                                            dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
                                            ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
                                            ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
                                            ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
                                        }
                                        this.Session.Add("dtFreeSKU", dtFreeSKU);
                                        this.LoadFreeGrid();
                                    }
                                }
                                #endregion
                                #endregion
                            }
                        }
                    }
                    #region Recalulate Products
                    //foreach (DataRow dr in dt.Rows)
                    //{
                    //    dr["STANDARD_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT_PER"].ToString())) * (decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString())) - decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())));
                    //    dr["CLAIM_STANDARD_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_PER"].ToString())) * (decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString())) - decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString())));
                    //    dr["SED_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_STANDARD_DISCOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString()));
                    //    ObjGrossSales += decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                    //    ObjStandardDiscount += decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString()));
                    //    ObjExtraDiscount += decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString()));
                    //    ObjTotalSED += decimal.Parse(dc.chkNull_0(dr["SED_AMOUNT"].ToString()));
                    //    #region GST Calculation
                    //    decimal TempAmount = decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                    //    decimal TempDiscount = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString()));
                    //    decimal TempExtraDisAmt = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString()));
                    //    // dr["GST_AMOUNT"] = (TempAmount - TempDiscount - TempExtraDisAmt) * decimal.Parse(dc.chkNull_0(dr["GST_RATE"].ToString())) / 100;
                    //    // Gst direct on Gross amount
                    //    dr["GST_AMOUNT"] = (TempAmount) * decimal.Parse(dc.chkNull_0(dr["GST_RATE"].ToString())) / 100;
                    //    dr["NET_AMOUNT"] = (TempAmount - TempDiscount - TempExtraDisAmt) + decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));
                    //    ObjTotalGST += decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString()));
                    //    ObjTotalTST += decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));
                    //    #endregion
                    //}
                    #endregion
                    #endregion
                    // SAFDAR END
                    // BY M. IJAZ JAMIL AKHTAR
                    #region Reversecalulate Products
                    decimal ObjClaimAblePer = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        ObjGrossSales += decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                    }
                    if (decimal.Parse(dc.chkNull_0(numtxtUnClaimabledist.Text)) > 0)
                    {
                        ObjClaimAblePer = decimal.Parse(dc.chkNull_0(numtxtUnClaimabledist.Text)) / ObjGrossSales;
                    }
                    ObjGrossSales = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        decimal TempAmount = 0;
                        decimal TempDiscount = 0;
                        decimal TempExtraDisAmt = 0;
                        decimal TempNetAmount = 0;

                        //dr["EXTRA_DISCOUNT"] = ProductExtDist * decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        //dr["STANDARD_DISCOUNT"] = ProductStdDist * decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));

                        // Calculate Extra Tax By M. IJAZ JAMIL AKHTAR

                        #region GST Calculation
                        TempAmount = decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        TempDiscount = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString()));
                        TempExtraDisAmt = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString()));
                        TempNetAmount =
                                          TempAmount
                                        - TempDiscount
                                        - TempExtraDisAmt
                                        - decimal.Parse(dc.chkNull_0(txtINcentive.Text))
                                        - decimal.Parse(dc.chkNull_0(txtBRDDiscount.Text))
                                        - decimal.Parse(dc.chkNull_0(numTxtTotalStndrdDiscnt.Text))
                                        - decimal.Parse(dc.chkNull_0(txtVBRDDiscount.Text))
                                        - decimal.Parse(dc.chkNull_0(txtTradeOfferDisc.Text))
                                        - decimal.Parse(dc.chkNull_0(txtDisplayDiscount.Text))
                                        //- decimal.Parse(dc.chkNull_0(numtxtTotalExtraDiscnt.Text))
                                        //- decimal.Parse(dc.chkNull_0(numtxtUnClaimabledist.Text))
                                        - decimal.Parse(dc.chkNull_0(txtRentalDiscount.Text));

                        #region EXTRA TAX
                        if (DistributorIsRegistered())
                        {
                            if (!CustomerIsRegistered())
                            {
                                if (dr["BATCH_NO"].ToString() == "T")
                                {
                                    decimal Discount = (TempNetAmount * 3) / 100;
                                    dr["EXTRA_TAX"] = Discount;
                                }
                                else
                                {
                                    dr["EXTRA_TAX"] = 0;
                                }
                            }
                            else
                            {
                                dr["EXTRA_TAX"] = 0;
                            }
                        }
                        else
                        {
                            dr["EXTRA_TAX"] = 0;
                        }
                        #endregion

                        //dr["SED_AMOUNT"] = ObjClaimAblePer * decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        dr["GST_AMOUNT"] = (TempNetAmount * decimal.Parse(dc.chkNull_0(dr["GST_RATE"].ToString()))) / 100;
                        dr["NET_AMOUNT"] = TempNetAmount + decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["EXTRA_TAX"].ToString()));
                        #endregion
                        ObjGrossSales += decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        ObjTotalGST += decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString()));
                        ObjTotalTST += decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));
                        //ObjTotalSED += decimal.Parse(dc.chkNull_0(dr["SED_AMOUNT"].ToString()));
                        ObjTotalEXTRA_TAX += decimal.Parse(dc.chkNull_0(dr["EXTRA_TAX"].ToString()));
                        ObjStandardDiscount += decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString()));
                        ObjExtraDiscount += decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString()));
                        //ObjNetSales += decimal.Parse(dc.chkNull_0(dr["NET_AMOUNT"].ToString()));
                    }
                    #endregion
                }
                #region Set Total Values
                txtGrossAmount.Text = Convert.ToString(Math.Round(ObjGrossSales, 4));
                numTxtTotalGST.Text = Convert.ToString(Math.Round(ObjTotalGST + ObjFreeGSTAmount, 4));
                numTxtTotalTST.Text = Convert.ToString(Math.Round(ObjTotalTST + ObjFreeTST, 4));
                numTxtTotalExtra_tax.Text = Convert.ToString(Math.Round((ObjTotalEXTRA_TAX), 4));
                ObjNetSales = Math.Round(ObjGrossSales - ObjStandardDiscount - ObjExtraDiscount - ObjTotalSED + ObjTotalGST + ObjFreeGSTAmount + ObjTotalTST + ObjTotalEXTRA_TAX + ObjFreeTST - decimal.Parse(dc.chkNull_0(numTxtTotalStndrdDiscnt.Text)) - decimal.Parse(dc.chkNull_0(numtxtDiscount.Text)), 4);
                //-decimal.Parse(dc.chkNull_0(numtxtTotalExtraDiscnt.Text))
                numTxtTotlAmnt.Text = Convert.ToString(Math.Round(ObjNetSales, 4));
                #endregion
                btnSaveOrder.Enabled = true;
                if (RblPayMode.SelectedIndex == 1)
                {
                    txtCashReceived.Enabled = true;
                    ScriptManager.GetCurrent(Page).SetFocus(txtCashReceived);
                }
                else
                {
                    txtCashReceived.Enabled = false;
                }
            }
        }
        #region Old Promotion Calculation Code
        //if (FindCustomer())
        //{
        //    decimal ObjGrossSales = 0;
        //    decimal ObjStandardDiscount = 0;
        //    decimal ObjExtraDiscount = 0;
        //    decimal ObjTotalGST = 0;
        //    decimal ObjTotalSED = 0;
        //    decimal ObjTotalTST = 0;
        //    decimal ObjTotalEXTRA_TAX = 0;
        //    decimal ObjFreeGSTAmount = 0;
        //    decimal ObjFreeGrossAmt = 0;
        //    decimal ObjFreeTST = 0;
        //    decimal ProductExtDist = 0;
        //    decimal ProductStdDist = 0;
        //    decimal ObjNetSales = 0;
        //    DataControl dc = new DataControl();
        //    OrderEntryController or = new OrderEntryController();
        //    SKUGroupController GroupCtl = new SKUGroupController();
        //    DataTable dt = (DataTable)this.Session["PurchaseSKU"];
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        dr["CLAIM_EXTRA_AMOUNT"] = 0;
        //        dr["CLAIM_STANDARD_DISCOUNT"] = 0;
        //        dr["CLAIM_PER"] = 0;
        //    }
        //    PromotionCollections_Controller Arpc = (PromotionCollections_Controller)this.Session["Arpc"];
        //    this.CreateFreeSKU();
        //    this.LoadFreeGrid();
        //    if (dt.Rows.Count > 0 && drpcustomer.Items.Count > 0)
        //    {
        //        if (ChbDiscount.Checked == true)
        //        {
        //            #region 
        //            ArrayList ARlistOffer = or.GetPromotionOffers(Arpc, mCustomerVolClassId, mCustomerTypeId, dt, false);
        //            for (int i = 0; i < ARlistOffer.Count; i++)
        //            {
        //                PromoOffers_Collection pofferCol = (PromoOffers_Collection)ARlistOffer[i];
        //                if (pofferCol.Is_Claimable)
        //                {
        //                    if (pofferCol.SKU_ID > 0)
        //                    {
        //                        #region slab on SKU
        //                        foreach (DataRow dr in dt.Rows)
        //                        {
        //                            if (int.Parse(dr["SKU_ID"].ToString()) == pofferCol.SKU_ID)
        //                            {
        //                                if (pofferCol.Is_And == true)
        //                                {
        //                                    if (pofferCol.Offer_Value > 0)
        //                                    {
        //                                        dr["CLAIM_EXTRA_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString())) + pofferCol.Offer_Value;
        //                                    }
        //                                    if (pofferCol.Discount > 0)
        //                                    {
        //                                        dr["CLAIM_EXTRA_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100) * (decimal.Parse(dr["AMOUNT"].ToString()));
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (pofferCol.Offer_Value > 0)
        //                                    {
        //                                    }
        //                                    if (pofferCol.Discount > 0)
        //                                    {
        //                                        dr["CLAIM_PER"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_PER"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100);
        //                                    }
        //                                }
        //                                if (pofferCol.Free_SKU_ID > 0)
        //                                {
        //                                    #region Free sku
        //                                    DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        //                                    DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
        //                                    int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
        //                                    if (foundRows.Length > 0)
        //                                    {
        //                                        if (RowId < 0)
        //                                        {
        //                                            DataRow dr1 = dtFreeSKU.NewRow();
        //                                            dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
        //                                            dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
        //                                            dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
        //                                            if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                // dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                            }
        //                                            else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                //  dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                            }
        //                                            else
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                            }
        //                                            dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
        //                                            dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
        //                                            dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
        //                                            dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
        //                                            ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
        //                                            ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
        //                                            ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
        //                                            dtFreeSKU.Rows.Add(dr1);
        //                                        }
        //                                        else
        //                                        {
        //                                            DataRow dr1 = dtFreeSKU.Rows[RowId];
        //                                            ObjFreeGrossAmt = ObjFreeGrossAmt - decimal.Parse(dr1["AMOUNT"].ToString());
        //                                            ObjFreeGSTAmount = ObjFreeGSTAmount - decimal.Parse(dr1["GST_AMOUNT"].ToString());
        //                                            dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
        //                                            dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
        //                                            dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
        //                                            dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;
        //                                            if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                //    dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                            }
        //                                            else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                // dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                            }
        //                                            else
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                            }
        //                                            dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
        //                                            dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
        //                                            dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
        //                                            dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
        //                                            ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
        //                                            ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
        //                                            ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
        //                                        }
        //                                    }
        //                                    #endregion
        //                                }
        //                                this.Session.Add("dtFreeSKU", dtFreeSKU);
        //                                this.LoadFreeGrid();
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        #region slab on SKU Group
        //                        #region Free SKU
        //                        if (pofferCol.Free_SKU_ID > 0)
        //                        {
        //                            DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        //                            DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
        //                            int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
        //                            if (foundRows.Length > 0)
        //                            {
        //                                if (RowId < 0)
        //                                {
        //                                    DataRow dr1 = dtFreeSKU.NewRow();
        //                                    dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
        //                                    dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
        //                                    dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
        //                                    dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                    dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        //  dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                    }
        //                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        //    dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                    }
        //                                    else
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                    }
        //                                    dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
        //                                    dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
        //                                    dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
        //                                    dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
        //                                    ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
        //                                    ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
        //                                    ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
        //                                    dtFreeSKU.Rows.Add(dr1);
        //                                }
        //                                else
        //                                {
        //                                    DataRow dr1 = dtFreeSKU.Rows[RowId];
        //                                    ObjFreeGrossAmt = ObjFreeGrossAmt - decimal.Parse(dr1["AMOUNT"].ToString());
        //                                    ObjFreeGSTAmount = ObjFreeGSTAmount - decimal.Parse(dr1["GST_AMOUNT"].ToString());
        //                                    dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
        //                                    dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
        //                                    dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
        //                                    dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;
        //                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        //  dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                    }
        //                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        //   dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                    }
        //                                    else
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                    }
        //                                    dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
        //                                    dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
        //                                    dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
        //                                    dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
        //                                    ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
        //                                    ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
        //                                    ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
        //                                }
        //                                this.Session.Add("dtFreeSKU", dtFreeSKU);
        //                                this.LoadFreeGrid();
        //                            }
        //                        }
        //                        #endregion
        //                        foreach (DataRow dr in dt.Rows)
        //                        {
        //                            if (GroupCtl.ExistsInGroup(Constants.IntNullValue, pofferCol.Group_ID, int.Parse(dr["SKU_ID"].ToString())))
        //                            {
        //                                if (pofferCol.Is_And == true)
        //                                {
        //                                    if (pofferCol.Offer_Value > 0)
        //                                    {
        //                                        dr["CLAIM_EXTRA_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString())) + pofferCol.Offer_Value;
        //                                    }
        //                                    if (pofferCol.Discount > 0)
        //                                    {
        //                                        dr["CLAIM_EXTRA_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100) * (decimal.Parse(dr["AMOUNT"].ToString()));
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (pofferCol.Offer_Value > 0)
        //                                    {
        //                                    }
        //                                    if (pofferCol.Discount > 0)
        //                                    {
        //                                        dr["CLAIM_PER"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_PER"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                }
        //                else
        //                {
        //                    if (pofferCol.SKU_ID > 0)
        //                    {
        //                        #region slab on SKU
        //                        foreach (DataRow dr in dt.Rows)
        //                        {
        //                            if (int.Parse(dr["SKU_ID"].ToString()) == pofferCol.SKU_ID)
        //                            {
        //                                if (pofferCol.Is_And == true)
        //                                {
        //                                    if (pofferCol.Offer_Value > 0)
        //                                    {
        //                                    }
        //                                    if (pofferCol.Discount > 0)
        //                                    {
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (pofferCol.Offer_Value > 0)
        //                                    {
        //                                    }
        //                                    if (pofferCol.Discount > 0)
        //                                    {
        //                                    }
        //                                }
        //                                if (pofferCol.Free_SKU_ID > 0)
        //                                {
        //                                    DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        //                                    DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
        //                                    int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
        //                                    if (foundRows.Length > 0)
        //                                    {
        //                                        if (RowId < 0)
        //                                        {
        //                                            DataRow dr1 = dtFreeSKU.NewRow();
        //                                            dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
        //                                            dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
        //                                            dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
        //                                            if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                // dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                            }
        //                                            else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                //    dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                            }
        //                                            else
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                            }
        //                                            dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
        //                                            dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
        //                                            dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
        //                                            dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
        //                                            ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
        //                                            ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
        //                                            ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
        //                                            dtFreeSKU.Rows.Add(dr1);
        //                                        }
        //                                        else
        //                                        {
        //                                            DataRow dr1 = dtFreeSKU.Rows[RowId];
        //                                            ObjFreeGrossAmt = ObjFreeGrossAmt - decimal.Parse(dr1["AMOUNT"].ToString());
        //                                            ObjFreeGSTAmount = ObjFreeGSTAmount - decimal.Parse(dr1["GST_AMOUNT"].ToString());
        //                                            dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
        //                                            dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
        //                                            dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
        //                                            dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;
        //                                            if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                //    dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                            }
        //                                            else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                //   dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                            }
        //                                            else
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                            }
        //                                            dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
        //                                            dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
        //                                            dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
        //                                            dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
        //                                            ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
        //                                            ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
        //                                            ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
        //                                        }
        //                                    }
        //                                }
        //                                this.Session.Add("dtFreeSKU", dtFreeSKU);
        //                                this.LoadFreeGrid();
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        #region slab on SKU Group
        //                        #region Free SKU
        //                        if (pofferCol.Free_SKU_ID > 0)
        //                        {
        //                            DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        //                            DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
        //                            int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
        //                            if (foundRows.Length > 0)
        //                            {
        //                                if (RowId < 0)
        //                                {
        //                                    DataRow dr1 = dtFreeSKU.NewRow();
        //                                    dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
        //                                    dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
        //                                    dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
        //                                    dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                    dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        //  dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                    }
        //                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        // dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                    }
        //                                    else
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                    }
        //                                    dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
        //                                    dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
        //                                    dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
        //                                    dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
        //                                    ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
        //                                    ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
        //                                    ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
        //                                    dtFreeSKU.Rows.Add(dr1);
        //                                }
        //                                else
        //                                {
        //                                    DataRow dr1 = dtFreeSKU.Rows[RowId];
        //                                    ObjFreeGrossAmt = ObjFreeGrossAmt - decimal.Parse(dr1["AMOUNT"].ToString());
        //                                    ObjFreeGSTAmount = ObjFreeGSTAmount - decimal.Parse(dr1["GST_AMOUNT"].ToString());
        //                                    dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
        //                                    dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
        //                                    dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
        //                                    dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;
        //                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        //   dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                    }
        //                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        // dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                    }
        //                                    else
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                    }
        //                                    dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
        //                                    dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
        //                                    dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
        //                                    dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
        //                                    ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
        //                                    ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
        //                                    ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
        //                                }
        //                                this.Session.Add("dtFreeSKU", dtFreeSKU);
        //                                this.LoadFreeGrid();
        //                            }
        //                        }
        //                        #endregion
        //                        foreach (DataRow dr in dt.Rows)
        //                        {
        //                            if (GroupCtl.ExistsInGroup(Constants.IntNullValue, pofferCol.Group_ID, int.Parse(dr["SKU_ID"].ToString())))
        //                            {
        //                                if (pofferCol.Is_And == true)
        //                                {
        //                                    if (pofferCol.Offer_Value > 0)
        //                                    {
        //                                    }
        //                                    if (pofferCol.Discount > 0)
        //                                    {
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (pofferCol.Offer_Value > 0)
        //                                    {
        //                                    }
        //                                    if (pofferCol.Discount > 0)
        //                                    {
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                }
        //            }
        //            #region Recalulate Products
        //            foreach (DataRow dr in dt.Rows)
        //            {
        //                dr["CLAIM_STANDARD_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_PER"].ToString())) * (decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString())) - decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString())));
        //                dr["SED_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_STANDARD_DISCOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString()));
        //                ObjGrossSales += decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
        //                ObjTotalSED += decimal.Parse(dc.chkNull_0(dr["SED_AMOUNT"].ToString()));
        //                #region GST Calculation
        //                decimal TempAmount = decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
        //                dr["GST_AMOUNT"] = (TempAmount) * decimal.Parse(dc.chkNull_0(dr["GST_RATE"].ToString())) / 100;
        //                dr["NET_AMOUNT"] = (TempAmount - Convert.ToDecimal(dr["STANDARD_DISCOUNT"]) - Convert.ToDecimal(dr["EXTRA_DISCOUNT"])) + decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));
        //                ObjTotalGST += decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString()));
        //                ObjTotalTST += decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));
        //                #endregion
        //            }
        //            #endregion
        //            #endregion
        //        }
        //        else // Manual Discount
        //        {
        //            // safdar start
        //            #region 
        //            ArrayList ARlistOffer = or.GetPromotionOffers(Arpc, mCustomerVolClassId, mCustomerTypeId, dt, false);
        //            for (int i = 0; i < ARlistOffer.Count; i++)
        //            {
        //                PromoOffers_Collection pofferCol = (PromoOffers_Collection)ARlistOffer[i];
        //                if (pofferCol.Is_Claimable)
        //                {
        //                    if (pofferCol.SKU_ID > 0)
        //                    {
        //                        #region slab on SKU
        //                        foreach (DataRow dr in dt.Rows)
        //                        {
        //                            if (int.Parse(dr["SKU_ID"].ToString()) == pofferCol.SKU_ID)
        //                            {
        //                                if (pofferCol.Is_And == true)
        //                                {
        //                                }
        //                                else
        //                                {
        //                                }
        //                                if (pofferCol.Free_SKU_ID > 0)
        //                                {
        //                                    #region Free sku
        //                                    DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        //                                    DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
        //                                    int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
        //                                    if (foundRows.Length > 0)
        //                                    {
        //                                        if (RowId < 0)
        //                                        {
        //                                            DataRow dr1 = dtFreeSKU.NewRow();
        //                                            dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
        //                                            dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
        //                                            dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
        //                                            if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                // dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                            }
        //                                            else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                // dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                            }
        //                                            else
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                            }
        //                                            dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
        //                                            dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
        //                                            dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
        //                                            dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
        //                                            ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
        //                                            ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
        //                                            ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
        //                                            dtFreeSKU.Rows.Add(dr1);
        //                                        }
        //                                        else
        //                                        {
        //                                            DataRow dr1 = dtFreeSKU.Rows[RowId];
        //                                            ObjFreeGrossAmt = ObjFreeGrossAmt - decimal.Parse(dr1["AMOUNT"].ToString());
        //                                            ObjFreeGSTAmount = ObjFreeGSTAmount - decimal.Parse(dr1["GST_AMOUNT"].ToString());
        //                                            dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
        //                                            dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
        //                                            dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
        //                                            dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;
        //                                            if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                // dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                            }
        //                                            else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                // dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                            }
        //                                            else
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                            }
        //                                            dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
        //                                            dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
        //                                            dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
        //                                            dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
        //                                            ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
        //                                            ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
        //                                            ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
        //                                        }
        //                                    }
        //                                    #endregion
        //                                }
        //                                this.Session.Add("dtFreeSKU", dtFreeSKU);
        //                                this.LoadFreeGrid();
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        #region slab on SKU Group
        //                        #region Free SKU
        //                        if (pofferCol.Free_SKU_ID > 0)
        //                        {
        //                            DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        //                            DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
        //                            int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
        //                            if (foundRows.Length > 0)
        //                            {
        //                                if (RowId < 0)
        //                                {
        //                                    DataRow dr1 = dtFreeSKU.NewRow();
        //                                    dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
        //                                    dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
        //                                    dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
        //                                    dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                    dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        //  dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                    }
        //                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        // dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                    }
        //                                    else
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                    }
        //                                    dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
        //                                    dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
        //                                    dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
        //                                    dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
        //                                    ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
        //                                    ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
        //                                    ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
        //                                    dtFreeSKU.Rows.Add(dr1);
        //                                }
        //                                else
        //                                {
        //                                    DataRow dr1 = dtFreeSKU.Rows[RowId];
        //                                    ObjFreeGrossAmt = ObjFreeGrossAmt - decimal.Parse(dr1["AMOUNT"].ToString());
        //                                    ObjFreeGSTAmount = ObjFreeGSTAmount - decimal.Parse(dr1["GST_AMOUNT"].ToString());
        //                                    dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
        //                                    dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
        //                                    dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
        //                                    dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;
        //                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        // dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                    }
        //                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        // dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                    }
        //                                    else
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                    }
        //                                    dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
        //                                    dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
        //                                    dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
        //                                    dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
        //                                    ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
        //                                    ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
        //                                    ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
        //                                }
        //                                this.Session.Add("dtFreeSKU", dtFreeSKU);
        //                                this.LoadFreeGrid();
        //                            }
        //                        }
        //                        #endregion
        //                        #endregion
        //                    }
        //                }
        //                else
        //                {
        //                    if (pofferCol.SKU_ID > 0)
        //                    {
        //                        #region slab on SKU
        //                        foreach (DataRow dr in dt.Rows)
        //                        {
        //                            if (int.Parse(dr["SKU_ID"].ToString()) == pofferCol.SKU_ID)
        //                            {
        //                                if (pofferCol.Is_And == true)
        //                                {
        //                                }
        //                                else
        //                                {
        //                                }
        //                                if (pofferCol.Free_SKU_ID > 0)
        //                                {
        //                                    DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        //                                    DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
        //                                    int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
        //                                    if (foundRows.Length > 0)
        //                                    {
        //                                        if (RowId < 0)
        //                                        {
        //                                            DataRow dr1 = dtFreeSKU.NewRow();
        //                                            dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
        //                                            dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
        //                                            dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
        //                                            if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                //   dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                            }
        //                                            else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                //  dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                            }
        //                                            else
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                            }
        //                                            dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
        //                                            dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
        //                                            dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
        //                                            dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
        //                                            ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
        //                                            ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
        //                                            ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
        //                                            dtFreeSKU.Rows.Add(dr1);
        //                                        }
        //                                        else
        //                                        {
        //                                            DataRow dr1 = dtFreeSKU.Rows[RowId];
        //                                            ObjFreeGrossAmt = ObjFreeGrossAmt - decimal.Parse(dr1["AMOUNT"].ToString());
        //                                            ObjFreeGSTAmount = ObjFreeGSTAmount - decimal.Parse(dr1["GST_AMOUNT"].ToString());
        //                                            dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
        //                                            dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
        //                                            dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
        //                                            dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;
        //                                            if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                //    dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                            }
        //                                            else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                //    dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                                dr1["GST_AMOUNT"] = 0;
        //                                                dr1["TST_AMOUNT"] = 0;
        //                                            }
        //                                            else
        //                                            {
        //                                                dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                                dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                                dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                                dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                            }
        //                                            dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
        //                                            dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
        //                                            dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
        //                                            dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
        //                                            ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
        //                                            ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
        //                                            ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
        //                                        }
        //                                    }
        //                                }
        //                                this.Session.Add("dtFreeSKU", dtFreeSKU);
        //                                this.LoadFreeGrid();
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        #region slab on SKU Group
        //                        #region Free SKU
        //                        if (pofferCol.Free_SKU_ID > 0)
        //                        {
        //                            DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        //                            DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
        //                            int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
        //                            if (foundRows.Length > 0)
        //                            {
        //                                if (RowId < 0)
        //                                {
        //                                    DataRow dr1 = dtFreeSKU.NewRow();
        //                                    dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
        //                                    dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
        //                                    dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
        //                                    dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                    dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        //  dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                    }
        //                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        //    dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                    }
        //                                    else
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                    }
        //                                    dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
        //                                    dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
        //                                    dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
        //                                    dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
        //                                    ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
        //                                    ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
        //                                    ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
        //                                    dtFreeSKU.Rows.Add(dr1);
        //                                }
        //                                else
        //                                {
        //                                    DataRow dr1 = dtFreeSKU.Rows[RowId];
        //                                    ObjFreeGrossAmt = ObjFreeGrossAmt - decimal.Parse(dr1["AMOUNT"].ToString());
        //                                    ObjFreeGSTAmount = ObjFreeGSTAmount - decimal.Parse(dr1["GST_AMOUNT"].ToString());
        //                                    dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
        //                                    dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
        //                                    dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
        //                                    dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;
        //                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        //     dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                    }
        //                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        // dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                    }
        //                                    else
        //                                    {
        //                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
        //                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
        //                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
        //                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
        //                                        dr1["GST_AMOUNT"] = 0;
        //                                        dr1["TST_AMOUNT"] = 0;
        //                                    }
        //                                    dr1["PROMOTION_ID"] = pofferCol.Promotion_ID.ToString();
        //                                    dr1["BASKET_ID"] = pofferCol.Basket_ID.ToString();
        //                                    dr1["BASKET_DETAIL_ID"] = pofferCol.BasketDetail_ID.ToString();
        //                                    dr1["PROMOTION_OFFER_ID"] = pofferCol.PromoOffer_ID.ToString();
        //                                    ObjFreeGrossAmt += decimal.Parse(dr1["AMOUNT"].ToString());
        //                                    ObjFreeGSTAmount += decimal.Parse(dc.chkNull_0(dr1["GST_AMOUNT"].ToString()));
        //                                    ObjFreeTST += decimal.Parse(dc.chkNull_0(dr1["TST_AMOUNT"].ToString()));
        //                                }
        //                                this.Session.Add("dtFreeSKU", dtFreeSKU);
        //                                this.LoadFreeGrid();
        //                            }
        //                        }
        //                        #endregion
        //                        #endregion
        //                    }
        //                }
        //            }
        //            #endregion
        //            // SAFDAR END
        //            #region Reversecalulate Products
        //            decimal ObjClaimAblePer = 0;
        //            foreach (DataRow dr in dt.Rows)
        //            {
        //                ObjGrossSales += decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
        //            }
        //            if (decimal.Parse(dc.chkNull_0(numtxtUnClaimabledist.Text)) > 0)
        //            {
        //                ObjClaimAblePer = decimal.Parse(dc.chkNull_0(numtxtUnClaimabledist.Text)) / ObjGrossSales;
        //            }
        //            ObjGrossSales = 0;
        //            foreach (DataRow dr in dt.Rows)
        //            {
        //                decimal TempAmount = 0;
        //                decimal TempDiscount = 0;
        //                decimal TempExtraDisAmt = 0;
        //                decimal TempNetAmount = 0;
        //                dr["SED_AMOUNT"] = ObjClaimAblePer * decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
        //                #region GST Calculation
        //                TempAmount = decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
        //                TempDiscount = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString()));
        //                TempExtraDisAmt = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString()));
        //                TempNetAmount =
        //                      TempAmount
        //                    - TempDiscount
        //                    - TempExtraDisAmt
        //                    - decimal.Parse(dc.chkNull_0(txtINcentive.Text))
        //                    - decimal.Parse(dc.chkNull_0(txtBRDDiscount.Text))
        //                    - decimal.Parse(dc.chkNull_0(numTxtTotalStndrdDiscnt.Text))
        //                    - decimal.Parse(dc.chkNull_0(txtVBRDDiscount.Text))
        //                    - decimal.Parse(dc.chkNull_0(txtTradeOfferDisc.Text))
        //                    - decimal.Parse(dc.chkNull_0(txtDisplayDiscount.Text))
        //                    //- decimal.Parse(dc.chkNull_0(numtxtTotalExtraDiscnt.Text))
        //                    - decimal.Parse(dc.chkNull_0(txtRentalDiscount.Text));
        //                //dr["GST_AMOUNT"] = (TempAmount - TempDiscount - TempExtraDisAmt) * decimal.Parse(dc.chkNull_0(dr["GST_RATE"].ToString())) / 100;
        //                if (DistributorIsRegistered())
        //                {
        //                    if (!CustomerIsRegistered())
        //                    {
        //                        if (dr["BATCH_NO"].ToString() == "T")
        //                        {
        //                            decimal Discount = (TempNetAmount * 3) / 100;
        //                            dr["EXTRA_TAX"] = Discount; //Math.Round(Discount, 4);
        //                        }
        //                        else
        //                        {
        //                            dr["EXTRA_TAX"] = 0;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        dr["EXTRA_TAX"] = 0;
        //                    }
        //                }
        //                else
        //                {
        //                    dr["EXTRA_TAX"] = 0;
        //                }
        //                dr["GST_AMOUNT"] = (TempNetAmount * decimal.Parse(dc.chkNull_0(dr["GST_RATE"].ToString()))) / 100;
        //                dr["NET_AMOUNT"] = TempNetAmount + decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["EXTRA_TAX"].ToString()));
        //                ObjGrossSales += decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
        //                ObjTotalGST += decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString()));
        //                ObjTotalTST += decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));
        //                ObjTotalSED += decimal.Parse(dc.chkNull_0(dr["SED_AMOUNT"].ToString()));
        //                ObjTotalEXTRA_TAX += decimal.Parse(dc.chkNull_0(dr["EXTRA_TAX"].ToString()));
        //                ObjStandardDiscount += decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString()));
        //                ObjExtraDiscount += decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString()));
        //                ObjNetSales += decimal.Parse(dc.chkNull_0(dr["NET_AMOUNT"].ToString()));
        //                //TempAmount = decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
        //                //dr["GST_AMOUNT"] = (TempAmount) * decimal.Parse(dc.chkNull_0(dr["GST_RATE"].ToString())) / 100;
        //                //dr["NET_AMOUNT"] = (TempAmount - Convert.ToDecimal(dr["STANDARD_DISCOUNT"]) - Convert.ToDecimal(dr["EXTRA_DISCOUNT"])) + decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));
        //                //ObjGrossSales += decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
        //                //ObjTotalGST += decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString()));
        //                //ObjTotalTST += decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));
        //                //ObjTotalSED += decimal.Parse(dc.chkNull_0(dr["SED_AMOUNT"].ToString()));
        //                #endregion
        //            }
        //            #endregion
        //        }
        //        #region Set Total Values
        //        txtGrossAmount.Text = Convert.ToString(Math.Round(ObjGrossSales, 4));
        //        //numtxtUnClaimabledist.Text = Convert.ToString(Math.Round(ObjExtraDiscount, 4));
        //        numTxtTotalGST.Text = Convert.ToString(Math.Round(ObjTotalGST + ObjFreeGSTAmount, 4));
        //        numTxtTotalTST.Text = Convert.ToString(Math.Round(ObjTotalTST + ObjFreeTST, 4));
        //        numTxtTotalExtra_tax.Text = Convert.ToString(Math.Round((ObjTotalEXTRA_TAX), 4));
        //        numTxtTotlAmnt.Text = Convert.ToString(Math.Round(ObjNetSales, 4));
        //        //Convert.ToString(Math.Round(ObjGrossSales - decimal.Parse(dc.chkNull_0(numTxtTotalStndrdDiscnt.Text)) - decimal.Parse(dc.chkNull_0(numtxtTotalExtraDiscnt.Text)) - decimal.Parse(dc.chkNull_0(numtxtDiscount.Text)) - ObjExtraDiscount + ObjTotalGST + ObjFreeGSTAmount + ObjTotalTST + ObjTotalEXTRA_TAX + ObjFreeTST, 4));
        //        #endregion
        //        btnSaveOrder.Enabled = true;
        //        if (RblPayMode.SelectedIndex == 1)
        //        {
        //            txtCashReceived.Enabled = true;
        //            ScriptManager.GetCurrent(Page).SetFocus(txtCashReceived);
        //        }
        //        else
        //        {
        //            txtCashReceived.Enabled = false;
        //        }
        //    }
        //}
        #endregion
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        DataControl dc = new DataControl();
        DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
        DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + drpskuDetail.SelectedValue + "'");
        decimal mTradePrice = decimal.Parse(dc.chkNull_0(txtUnitRate.Text)); //decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString()));
        decimal PURCHASE_PRICE = decimal.Parse(dc.chkNull_0(foundRows[0]["PURCHASE_PRICE"].ToString()));
        int mPackSize = int.Parse(dc.chkNull_0(foundRows[0]["UNITS_IN_CASE"].ToString()));
        decimal DISTRIBUTOR_PRICE = decimal.Parse(dc.chkNull_0(foundRows[0]["DISTRIBUTOR_PRICE"].ToString()));
        int PRINCIPAL_ID = int.Parse(dc.chkNull_0(foundRows[0]["PRINCIPAL_ID"].ToString()));
        if (btnSave.Text == "Add Sku")
        {
            if (CheckDublicateSKU())
            {
                DataRow dr = PurchaseSKU.NewRow();
                dr["SKU_ID"] = foundRows[0]["SKU_ID"];
                dr["SKU_Code"] = foundRows[0]["SKU_CODE"];
                dr["SKU_Name"] = foundRows[0]["SKU_NAME"];
                dr["QUANTITY_UNIT"] = int.Parse(dc.chkNull_0(txtQuantity.Text));
                dr["QUANTITY_CTN"] = int.Parse(dc.chkNull_0(txtQuantityCtn.Text));
                dr["QUANTITY"] = (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
                dr["UNIT_PRICE"] = mTradePrice.ToString();
                dr["COST_OF_GOODS"] = (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize)) * PURCHASE_PRICE;
                dr["STANDARD_DISCOUNT_PER"] = Convert.ToDecimal(dc.chkNull_0(txtDiscount.Text));
                dr["STANDARD_DISCOUNT"] = (mTradePrice * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize))) * Convert.ToDecimal(dr["STANDARD_DISCOUNT_PER"]) / 100;
                dr["EXTRA_DISCOUNT"] = Math.Round(Convert.ToDecimal(dc.chkNull_0(txtTO.Text)), 4); //Math.Round((Convert.ToDecimal(dr["QUANTITY"]) / mPackSize) * Convert.ToDecimal(dc.chkNull_0(txtTO.Text)), 2);
                if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                {
                    dr["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                    dr["AMOUNT"] = mTradePrice * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
                    dr["TST_AMOUNT"] = 0;
                    dr["BATCH_NO"] = "T";
                }
                else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                {
                    dr["AMOUNT"] = mTradePrice * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
                    dr["TST_AMOUNT"] = decimal.Parse(foundRows[0]["GST_RATE_TP"].ToString()) * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
                    dr["GST_RATE"] = 0;
                    dr["BATCH_NO"] = "R";
                }
                else
                {
                    dr["AMOUNT"] = mTradePrice * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
                    dr["TST_AMOUNT"] = 0;
                    dr["GST_RATE"] = 0;
                    dr["BATCH_NO"] = "E";
                }
                dr["EXTRA_TAX"] = 0;
                dr["GST_AMOUNT"] = 0;
                dr["NET_AMOUNT"] = 0;
                dr["DISTRIBUTOR_PRICE"] = DISTRIBUTOR_PRICE * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
                dr["FREE_PRICE"] = DISTRIBUTOR_PRICE;
                dr["PRINCIPAL_ID"] = PRINCIPAL_ID;
                PurchaseSKU.Rows.Add(dr);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('SKU Already Exists')", true);
            }
        }
        else
        {
            DataRow dr = PurchaseSKU.Rows[RowId];
            dr["SKU_ID"] = foundRows[0]["SKU_ID"];
            dr["SKU_Code"] = foundRows[0]["SKU_CODE"];
            dr["SKU_Name"] = foundRows[0]["SKU_NAME"];
            dr["BATCH_NO"] = "N/A";
            dr["QUANTITY_UNIT"] = int.Parse(dc.chkNull_0(txtQuantity.Text));
            dr["QUANTITY_CTN"] = int.Parse(dc.chkNull_0(txtQuantityCtn.Text));
            dr["QUANTITY"] = (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
            dr["UNIT_PRICE"] = mTradePrice.ToString();
            dr["COST_OF_GOODS"] = (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize)) * PURCHASE_PRICE;
            dr["STANDARD_DISCOUNT_PER"] = Convert.ToDecimal(dc.chkNull_0(txtDiscount.Text));
            dr["STANDARD_DISCOUNT"] = (mTradePrice * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize))) * Convert.ToDecimal(dr["STANDARD_DISCOUNT_PER"]) / 100;
            dr["EXTRA_DISCOUNT"] = Math.Round(Convert.ToDecimal(dc.chkNull_0(txtTO.Text)), 4); //Math.Round((Convert.ToDecimal(dr["QUANTITY"]) / mPackSize) * Convert.ToDecimal(dc.chkNull_0(txtTO.Text)), 2);
            if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
            {
                dr["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                dr["AMOUNT"] = mTradePrice * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
                dr["TST_AMOUNT"] = 0;
                dr["BATCH_NO"] = "T";
            }
            else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
            {
                dr["AMOUNT"] = mTradePrice * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
                dr["TST_AMOUNT"] = decimal.Parse(foundRows[0]["GST_RATE_TP"].ToString()) * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
                dr["GST_RATE"] = 0;
                dr["BATCH_NO"] = "R";
            }
            else
            {
                dr["AMOUNT"] = mTradePrice * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
                dr["TST_AMOUNT"] = 0;
                dr["GST_RATE"] = 0;
                dr["BATCH_NO"] = "E";
            }
            dr["EXTRA_TAX"] = 0;
            dr["GST_AMOUNT"] = 0;
            dr["NET_AMOUNT"] = 0;
            dr["DISTRIBUTOR_PRICE"] = DISTRIBUTOR_PRICE * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
            dr["FREE_PRICE"] = DISTRIBUTOR_PRICE;
            dr["PRINCIPAL_ID"] = PRINCIPAL_ID;
        }
        this.Session.Add("PurchaseSKU", PurchaseSKU);
        this.EnableDisableController(false);
        this.LoadGrid();
        this.ClearAll();
        ScriptManager.GetCurrent(Page).SetFocus(drpskuDetail);
        if (ChbDiscount.Checked == false)
        {
            decimal ObjGrossSales = 0;
            decimal ObjTaxableAmount = 0;
            foreach (DataRow dr in PurchaseSKU.Rows)
            {
                ObjGrossSales += decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                if (dr["BATCH_NO"].ToString() == "T")
                {
                    ObjTaxableAmount = decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                }
            }
            txtGrossAmount.Text = Convert.ToString(Math.Round(ObjGrossSales, 4));
            numTxtTotalSED.Text = Convert.ToString(Math.Round(ObjTaxableAmount, 4));
        }
    }
    protected void btnSaveOrder_Click(object sender, EventArgs e)
    {
        if (FindCustomer())
        {
            if (IsDayClosed())
            {
                UserController UserCtl = new UserController();
                UserCtl.InsertUserLogoutTime(Convert.ToInt32(Session["User_Log_ID"]), Convert.ToInt32(Session["UserID"]));
                this.Session.Clear();
                System.Web.Security.FormsAuthentication.SignOut();
                Response.Redirect("../Login.aspx");
            }
            else
            {
                decimal WHTax = 0;
                DataTable dtCustomer = (DataTable)this.Session["dtCustomer"];
                DataRow[] foundRows = dtCustomer.Select("CUSTOMER_ID  = '" + drpcustomer.SelectedValue.Trim() + "'");
                if (foundRows.Length > 0)
                {
                    int CodeFileType = 0;
                    CustomerDataController cd = new CustomerDataController();
                    DataTable dtMappSetting = cd.SelectMappingSetting(Constants.IntNullValue, Constants.IntNullValue);
                    if (dtMappSetting != null || dtMappSetting.Rows.Count > 0)
                    {
                        CodeFileType = int.Parse(dtMappSetting.Rows[0]["CODE_TYPE"].ToString());
                    }
                    if (CodeFileType == 1)
                    {
                        WHTax = decimal.Parse(foundRows[0]["WHTAX"].ToString());
                    }
                    else
                    {
                        WHTax = 0;
                    }
                }
                string ManualID = "";
                if (txtBillBookNo.Text.Trim().Length > 0)
                {
                    ManualID = txtBillBookNo.Text.ToUpper();
                }
                DataControl DC = new DataControl();
                PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
                dtFreeSKU = (DataTable)this.Session["dtFreeSKU"];
                OrderEntryController mOrderController = new OrderEntryController();
                decimal unClaimAbleDisc = 0;
                if (btnSaveOrder.Text == "Save Order")
                {
                    decimal SumCostOfGoods = 0;
                    foreach (DataRow dr in PurchaseSKU.Rows)
                    {
                        SumCostOfGoods += Convert.ToInt32(dr["COST_OF_GOODS"]);
                    }
                    if (!IsBillBookNoExist())
                    {
                        if (rdbDiscountType2.SelectedValue == "1")
                        {
                            unClaimAbleDisc = decimal.Parse(DC.chkNull_0(numtxtUnClaimabledist.Text));
                        }
                        else
                        {
                            unClaimAbleDisc = decimal.Parse(DC.chkNull_0(numtxtDiscount.Text));
                        }
                        bool IsValidInsert = mOrderController.Add_OrderAE(int.Parse(this.Session["DistributorId"].ToString()), ManualID, mTownId, long.Parse(hfCustomerAreaID.Value), int.Parse(this.Session["PrincipalId"].ToString()), long.Parse(this.Session["CUSTOMER_ID"].ToString()), long.Parse(this.Session["CUSTOMER_ID"].ToString()), int.Parse(this.Session["OrderBookerId"].ToString()), int.Parse(this.Session["DeliveryManId"].ToString()), int.Parse(RblPayMode.SelectedValue.ToString()),
                        decimal.Parse(DC.chkNull_0(txtGrossAmount.Text)), unClaimAbleDisc + Convert.ToDecimal(DC.chkNull_0(txtTradeOfferDisc.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalExtraDiscnt.Text)) + Convert.ToDecimal(DC.chkNull_0(numTxtTotalStndrdDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalGST.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlAmnt.Text)), 0, Constants.Order_Pending_Id, PurchaseSKU, dtFreeSKU, int.Parse(this.Session["UserId"].ToString()),
                        DateTime.Parse(this.Session["OrderDate"].ToString()), decimal.Parse(DC.chkNull_0(txtVBRDDiscount.Text)),
                        decimal.Parse(DC.chkNull_0(numTxtTotalTST.Text)), decimal.Parse(DC.chkNull_0(txtBRDDiscount.Text)), decimal.Parse(DC.chkNull_0(txtINcentive.Text))
                        , decimal.Parse(DC.chkNull_0(txtTradeOfferDisc.Text)), decimal.Parse(DC.chkNull_0(txtDisplayDiscount.Text))
                        , decimal.Parse(DC.chkNull_0(txtRentalDiscount.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalStndrdDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalExtra_tax.Text)), WHTax, SumCostOfGoods);
                        if (IsValidInsert)
                        {
                            this.LoadPendingOrder();
                            this.ClearMasterALL();
                            this.Session.Remove("hfBillBookNo");
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('This Bill Book No already exist,Kindly enter different Bill Book No');", true);
                    }
                }
                else if (btnSaveOrder.Text == "Save Invoice")
                {
                    if (Convert.ToDateTime(this.Session["CurrentWorkDate"]) >= Convert.ToDateTime(Session["OrderDate"].ToString()))
                    {
                        if (!IsBillBookNoExist())
                        {
                            if (int.Parse(RblPayMode.SelectedValue.ToString()) == Constants.Credit_Order_Id)//|| int.Parse(RblPayMode.SelectedValue.ToString()) == Constants.Advance_PaymentOrder_id)
                            {
                                CustomerDataController CDC = new CustomerDataController();
                                DataTable dt = CDC.SelectCustomerCreditBalance(long.Parse(this.Session["CUSTOMER_ID"].ToString()), int.Parse(this.Session["PrincipalId"].ToString()), int.Parse(this.Session["DistributorId"].ToString()), int.Parse(RblPayMode.SelectedValue.ToString()));
                                decimal NetCashSale = decimal.Parse(numTxtTotlAmnt.Text) - decimal.Parse(DC.chkNull_0(txtCashReceived.Text));

                                if (decimal.Parse(DC.chkNull_0(dt.Rows[0][0].ToString())) <= NetCashSale)
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Customer Credit Limit/Advance Amount is " + DC.chkNull_0(dt.Rows[0][0].ToString()) + "');", true);
                                    return;
                                }
                            }
                            foreach (DataRow dr in PurchaseSKU.Rows)
                            {
                                PhaysicalStockController mController = new PhaysicalStockController();
                                DataTable dtstock = mController.SelectSKUClosingStock(int.Parse(this.Session["DistributorId"].ToString()), int.Parse(dr["SKU_ID"].ToString()), "N/A", DateTime.Parse(this.Session["OrderDate"].ToString()));
                                if (dtstock.Rows.Count > 0)
                                {
                                    if (int.Parse(dtstock.Rows[0][0].ToString()) < int.Parse(dr["QUANTITY"].ToString()))
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('" + dr["SKU_Code"].ToString() + " Current Stock is " + dtstock.Rows[0][0].ToString() + "');", true);
                                        return;
                                    }
                                    else if (Convert.ToDecimal(dtstock.Rows[0][1]) != Convert.ToDecimal(dr["UNIT_PRICE"]))
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('" + dr["SKU_Code"].ToString() + " Order Rate is " + dr["UNIT_PRICE"].ToString() + " and Current Rate is " + dtstock.Rows[0][1].ToString() + "');", true);
                                        return;
                                    }
                                }
                                else
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert(' + " + dr["SKU_Code"].ToString() + "No Stock Found');", true);
                                    return;
                                }
                            }
                            if (rdbDiscountType2.SelectedValue == "1")
                            {
                                unClaimAbleDisc = decimal.Parse(DC.chkNull_0(numtxtUnClaimabledist.Text));
                            }
                            else
                            {
                                unClaimAbleDisc = decimal.Parse(DC.chkNull_0(numtxtDiscount.Text));
                            }
                            bool IsValidInsert = mOrderController.Add_InvoiceAE(int.Parse(this.Session["DistributorId"].ToString()),
                            ManualID, mTownId, long.Parse(hfCustomerAreaID.Value), int.Parse(this.Session["PrincipalId"].ToString()),
                            long.Parse(this.Session["CUSTOMER_ID"].ToString()), long.Parse(this.Session["CUSTOMER_ID"].ToString()),
                            int.Parse(this.Session["OrderBookerId"].ToString()), int.Parse(this.Session["DeliveryManId"].ToString()),
                            long.Parse(drpDocumentNo.SelectedValue.ToString()), decimal.Parse(DC.chkNull_0(txtGrossAmount.Text)),
                            unClaimAbleDisc + Convert.ToDecimal(DC.chkNull_0(txtTradeOfferDisc.Text)),
                            decimal.Parse(DC.chkNull_0(numtxtTotalExtraDiscnt.Text)) + Convert.ToDecimal(DC.chkNull_0(numTxtTotalStndrdDiscnt.Text)),
                            decimal.Parse(DC.chkNull_0(numTxtTotalGST.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlAmnt.Text)),
                            decimal.Parse(DC.chkNull_0(txtVBRDDiscount.Text)), int.Parse(RblPayMode.SelectedValue.ToString()),
                            PurchaseSKU, dtFreeSKU, int.Parse(this.Session["UserId"].ToString()), decimal.Parse(DC.chkNull_0(txtCashReceived.Text)),
                            DateTime.Parse(this.Session["OrderDate"].ToString()), decimal.Parse(DC.chkNull_0(numTxtTotalTST.Text)),
                            decimal.Parse(DC.chkNull_0(txtTradeOfferDisc.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalStndrdDiscnt.Text)),
                            decimal.Parse(DC.chkNull_0(txtDisplayDiscount.Text)), decimal.Parse(DC.chkNull_0(txtINcentive.Text)),
                            decimal.Parse(DC.chkNull_0(txtBRDDiscount.Text)), decimal.Parse(DC.chkNull_0(txtRentalDiscount.Text)),
                            decimal.Parse(DC.chkNull_0(numTxtTotalExtra_tax.Text)), WHTax);
                            if (IsValidInsert)
                            {
                                if (drpDocumentNo.Enabled == true)
                                {
                                    btnSaveOrder.Text = "Save Order";
                                }
                                this.LoadPendingOrder();
                                this.ClearMasterALL();
                                this.Session.Remove("hfBillBookNo");
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('This Bill Book No already exist,Kindly enter different Bill Book No');", true);
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Order Date Can not be greater than current working date');", true);
                    }
                }
                else if (btnSaveOrder.Text == "Sale Return")
                {
                    if (rdbDiscountType2.SelectedValue == "1")
                    {
                        unClaimAbleDisc = decimal.Parse(DC.chkNull_0(numtxtUnClaimabledist.Text));
                    }
                    else
                    {
                        unClaimAbleDisc = decimal.Parse(DC.chkNull_0(numtxtDiscount.Text));
                    }
                    bool IsValidInsert = mOrderController.Add_SaleReturnAE(int.Parse(this.Session["DistributorId"].ToString()), mTownId, long.Parse(hfCustomerAreaID.Value), int.Parse(this.Session["PrincipalId"].ToString())
                    , long.Parse(this.Session["CUSTOMER_ID"].ToString()), long.Parse(this.Session["CUSTOMER_ID"].ToString()), int.Parse(this.Session["OrderBookerId"].ToString()), int.Parse(this.Session["DeliveryManId"].ToString()), long.Parse(drpDocumentNo.SelectedValue.ToString()),
                    decimal.Parse(DC.chkNull_0(txtGrossAmount.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalExtraDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalStndrdDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalGST.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlAmnt.Text))
                    , 0, int.Parse(RblPayMode.SelectedValue.ToString()), PurchaseSKU, dtFreeSKU, int.Parse(this.Session["UserId"].ToString()), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), decimal.Parse(DC.chkNull_0(numTxtTotalTST.Text))
                    , unClaimAbleDisc, decimal.Parse(DC.chkNull_0(numTxtTotalExtra_tax.Text)), WHTax);
                    if (IsValidInsert)
                    {
                        this.LoadPendingOrder();
                        this.ClearMasterALL();
                    }
                }
            }
        }
    }
    private void ClearAll()
    {
        drpskuDetail.SelectedIndex = 0;
        txtUnitRate.Text = "";
        txtQuantity.Text = "";
        txtQuantityCtn.Text = "";
        txtDiscount.Text = "";
        txtTO.Text = "";
        btnSave.Text = "Add Sku";
        btnSaveOrder.Enabled = false;
        btnCalculate.Enabled = true;
        drpskuDetail.Enabled = true;
    }
    private void ClearMasterALL()
    {
        this.EnableDisableController(true);
        this.Session.Remove("PurchaseSKU");
        this.Session.Remove("dtFreeSKU");
        btnSaveOrder.Enabled = false;
        this.CreatTable();
        this.CreateFreeSKU();
        drpcustomer.SelectedIndex = 0;
        this.LoadGrid();
        this.LoadFreeGrid();
        txtGrossAmount.Text = "";
        numtxtTotalExtraDiscnt.Text = "";
        numTxtTotalStndrdDiscnt.Text = "";
        numTxtTotalGST.Text = "";
        numTxtTotlAmnt.Text = "";
        numTxtTotalSED.Text = "";
        numTxtTotalTST.Text = "";
        numTxtTotalSED.Text = "";
        numtxtUnClaimabledist.Text = "";
        numtxtDiscount.Text = "";
        txtDisplayDiscount.Text = "";
        txtVBRDDiscount.Text = "";
        txtTradeOfferDisc.Text = "";
        txtRentalDiscount.Text = "";
        txtBRDDiscount.Text = "";
        txtINcentive.Text = "";
        numTxtTotalExtra_tax.Text = "";
        RowId = 0;
        mCustomerTypeId = 0;
        mCustomerVolClassId = 0;
        drpDocumentNo.SelectedIndex = 0;
        txtCashReceived.Text = "";
        txtBillBookNo.Text = string.Empty;
        if (btnSaveOrder.Text != "Sale Return")
        {
            ScriptManager.GetCurrent(Page).SetFocus(txtBillBookNo);
        }
        else
        {
            ScriptManager.GetCurrent(Page).SetFocus(drpcustomer);
        }
    }
    private bool IsDayClosed()
    {
        bool flag = false;
        DistributorController DistrCtl = new DistributorController();
        DataTable dtDayClose = DistrCtl.MaxDayClose(Convert.ToInt32(Session["DistributorId"]), 3);
        if (Convert.ToDateTime(Session["CurrentWorkDate"]) == Convert.ToDateTime(dtDayClose.Rows[0]["DayClose"]))
        {
            flag = false;
        }
        else
        {
            flag = true;
        }
        return flag;
    }
    protected void rdbDiscountType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rdbDiscountType2.SelectedValue == "1")
        {
            numtxtUnClaimabledist.Visible = true;
            numtxtUnClaimabledist.Text = "";
            numtxtDiscountage.Text = "";
            numtxtDiscount.Text = "";
            numtxtDiscountage.Visible = false;
            numtxtDiscount.Visible = false;
        }
        else
        {
            numtxtUnClaimabledist.Visible = false;
            numtxtDiscountage.Visible = true;
            numtxtDiscount.Visible = true;
            numtxtUnClaimabledist.Text = "";
            numtxtDiscountage.Text = "";
            numtxtDiscount.Text = "";
        }
    }
    protected void drpcustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
        hfCustomerAreaID.Value = "";
        try
        {
            DataTable dtCustomer = (DataTable)this.Session["dtCustomer"];
            DataRow[] foundRows = dtCustomer.Select("CUSTOMER_ID  = '" + drpcustomer.SelectedValue.Trim() + "'");
            if (foundRows.Length > 0)
            {
                hfCustomerAreaID.Value = foundRows[0]["AREA_ID"].ToString();
                int CodeFileType = 0;
                CustomerDataController cd = new CustomerDataController();
                DataTable dtMappSetting = cd.SelectMappingSetting(Constants.IntNullValue, Constants.IntNullValue);
                if (dtMappSetting != null || dtMappSetting.Rows.Count > 0)
                {
                    CodeFileType = int.Parse(dtMappSetting.Rows[0]["CODE_TYPE"].ToString());
                }
                if (CodeFileType == 1)
                {
                    hfWHTax.Value = foundRows[0]["WHTAX"].ToString();
                }
                else
                {
                    hfWHTax.Value = "0";
                }
            }
        }
        catch (Exception ex)
        {
            string ss = ex.StackTrace;
        }
    }
}