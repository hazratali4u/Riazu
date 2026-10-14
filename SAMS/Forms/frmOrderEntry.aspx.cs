using System;
using System.Data;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Linq;

/// <summary>
/// Form To Take Order, Invoice And Sale Return(Step2)
/// </summary>
public partial class Forms_frmOrderEntry : System.Web.UI.Page
{
    #region Variables
    DataTable PurchaseSKU;
    DataTable dtFreeSKU;
    private static int mCustomerTypeId;
    private static int mCustomerVolClassId;
    private int mTownId;
    private static int RowId;
    //   private static int UnitType;
    private static int OrderNo;
    readonly DataControl _dc = new DataControl();
    readonly UserController mUserController = new UserController();
    readonly SKUPriceDetailController PController = new SKUPriceDetailController();
    readonly OrderEntryController orderController = new OrderEntryController();
    readonly CustomerDataController _customerCtrl = new CustomerDataController();
    readonly DataControl dc = new DataControl();
    private static int DocumentType_ID = 2;
    public static int PrincipalWiseSale = 0;
    public int OrderType = 0;
    private static int mPrincipalId;
    #endregion
    bool ViewAccess;
    bool IsAuto;
    protected void Page_Load(object sender, EventArgs e)
    {
        DataTable dtAppSetting = (DataTable)Session["dtAppSetting"];
        if (dtAppSetting != null)
        {
            DataRow[] drAppSetting = dtAppSetting.Select("strColumnName='ManualPrice'");
            if (drAppSetting.Length > 0)
            {
                if (drAppSetting[0]["strColumnValue"].ToString() == "1")
                {
                    txtUnitRate.Enabled = true;
                }
            }

            DataRow[] drAppSetting2 = dtAppSetting.Select("strColumnName='ManualDiscounOn'");
            if (drAppSetting2.Length > 0)
            {
                Session.Add("ManualDiscounOn", drAppSetting2[0]["strColumnValue"].ToString());
                if (drAppSetting2[0]["strColumnValue"].ToString() == "1")
                {
                    lblDiscountImplementation.Text = "Manual Discount Implementation : Per Ctn";
                }
                else if (drAppSetting2[0]["strColumnValue"].ToString() == "2")
                {
                    lblDiscountImplementation.Text = "Manual Discount Implementation : Per Unit";
                }
                else
                {
                    lblDiscountImplementation.Text = "";
                }
            }
            // FREE SKU
            DataRow[] drAppSetting3 = dtAppSetting.Select("strColumnName='FreeSKUAutoPromotion'");
            if (drAppSetting3.Length > 0)
            {
                if (drAppSetting3[0]["strColumnValue"].ToString() == "0")
                {
                    IsAuto = false;
                }
                else
                {
                    IsAuto = true;
                }
            }
        }

        //ViewAccess = ValidateScreen();
        //if (ViewAccess == false)
        //{
        //    Response.Redirect("../Forms/AccessDenied.aspx");
        //}
        if (!Page.IsPostBack)
        {
            try
            {
                Configration();
                LoadAssingned();
                int OrderNo = (int)this.Session["OrderNo"];
                // UnitType = (int)this.Session["UnitType"];
                txtprincipal.Text = this.Session["Route"].ToString();
                txtDeliveryMan.Text = this.Session["SaleMan"].ToString();
                this.Session.Remove("Route");
                this.Session.Remove("SaleMan");
                ScriptManager.GetCurrent(Page).SetFocus(txtBillBookNo);
                this.LoadPrincipal();
                this.LoadCustomerData();
                if (OrderNo == -1)
                {
                    btnSaveOrder.Text = "Save Invoice";
                    drpDocumentNo.Enabled = false;
                    RblPayMode_SelectedIndexChanged(null, null);
                }
                else if (OrderNo == -2)
                {
                    drpDocumentNo.Enabled = false;
                    ChbDiscount.Visible = true;
                    btnSaveOrder.Text = "Sale Return";
                    lblBillBook.Visible = false;
                    txtBillBookNo.Visible = false;
                    RblPayMode_SelectedIndexChanged(null, null);
                    ScriptManager.GetCurrent(Page).SetFocus(drpcustomer);
                }
                else
                {
                    RblPayMode_SelectedIndexChanged(null, null);
                }
                this.LoadSKUDetail();
                this.CreatTable();
                this.LoadPromotion();
                this.CreateFreeSKU();
                this.LoadFreeGrid();
                this.LoadPendingOrder();
                btnSave.Attributes.Add("onclick", "return ValidateForm();");
                RbdiscountType_SelectedIndexChanged(null, null);
                ChkDiscountType();
            }
            catch (Exception ex)
            {
                ExceptionPublisher.PublishException(ex);
            }
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
        if (M_Key.Trim() == "frmorderentry.aspx")
        {
            return true;
        }
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
            DataRow[] drAppSetting2 = dtAppSetting.Select("strColumnName='OrderType'");
            if (drAppSetting.Length > 0)
            {
                PrincipalWiseSale = Convert.ToInt32(drAppSetting[0]["strColumnValue"].ToString());
                if (drAppSetting[0]["strColumnValue"].ToString() == "1")
                {
                    PanelPrincipal.Visible = true;
                }
                else
                {
                    PanelPrincipal.Visible = false;
                }
            }

            if (drAppSetting2.Length > 0)
            {
                RblPayMode.SelectedValue = drAppSetting2[0]["strColumnValue"].ToString();
            }
        }
    }
    private void ChkDiscountType()
    {
        if (txtDiscountType.Text != "Manual Discount")
        {
            PanelRbtStdDisc.Visible = false;
            PanelRbtExtraDisc.Visible = false;
            PanelStdDiscLbl.Visible = false;
            PanelExtraDsctLbl.Visible = false;
            PanelStdDisctTextbox.Visible = false;
            PanelExtraDisctTextbox.Visible = false;

            Panel2.Style.Add(HtmlTextWriterStyle.Width, "795px");
            SummaryTable.Style.Add(HtmlTextWriterStyle.Width, "795px");

            // Lables
            lblskuname.Style.Add(HtmlTextWriterStyle.Width, "252px");
            Label3.Style.Add(HtmlTextWriterStyle.Width, "82px");
            Label6.Style.Add(HtmlTextWriterStyle.Width, "82px");
            lblquantity.Style.Add(HtmlTextWriterStyle.Width, "82px");
            Label7.Style.Add(HtmlTextWriterStyle.Width, "82px");
            Label2.Style.Add(HtmlTextWriterStyle.Width, "94px");
            Label4.Style.Add(HtmlTextWriterStyle.Width, "85px");

            // Text Fields
            DrpSkuDetail.Style.Add(HtmlTextWriterStyle.Width, "254px");
            txtQuantityCtn.Style.Add(HtmlTextWriterStyle.Width, "79px");
            txtQuantity.Style.Add(HtmlTextWriterStyle.Width, "79px");
            txtUnitRate.Style.Add(HtmlTextWriterStyle.Width, "80px");
            txtFreeSku.Style.Add(HtmlTextWriterStyle.Width, "80px");
            txtAmount.Style.Add(HtmlTextWriterStyle.Width, "92px");
            btnSave.Style.Add(HtmlTextWriterStyle.Width, "85px");
        }
        else
        {
            PanelRbtStdDisc.Visible = true;
            PanelRbtExtraDisc.Visible = true;
            PanelStdDiscLbl.Visible = true;
            PanelExtraDsctLbl.Visible = true;
            PanelStdDisctTextbox.Visible = true;
            PanelExtraDisctTextbox.Visible = true;

            Panel2.Style.Add(HtmlTextWriterStyle.Width, "840px");
            SummaryTable.Style.Add(HtmlTextWriterStyle.Width, "840px");

            //Lables
            lblskuname.Style.Add(HtmlTextWriterStyle.Width, "234px");
            Label3.Style.Add(HtmlTextWriterStyle.Width, "82px");
            Label6.Style.Add(HtmlTextWriterStyle.Width, "82px");
            lblquantity.Style.Add(HtmlTextWriterStyle.Width, "82px");
            Label7.Style.Add(HtmlTextWriterStyle.Width, "82px");
            Label2.Style.Add(HtmlTextWriterStyle.Width, "74px");
            Label4.Style.Add(HtmlTextWriterStyle.Width, "85px");

            // Text Fields
            DrpSkuDetail.Style.Add(HtmlTextWriterStyle.Width, "234px");
            txtQuantityCtn.Style.Add(HtmlTextWriterStyle.Width, "79px");
            txtQuantity.Style.Add(HtmlTextWriterStyle.Width, "79px");
            txtUnitRate.Style.Add(HtmlTextWriterStyle.Width, "80px");
            txtFreeSku.Style.Add(HtmlTextWriterStyle.Width, "80px");
            txtAmount.Style.Add(HtmlTextWriterStyle.Width, "72px");
            btnSave.Style.Add(HtmlTextWriterStyle.Width, "85px");
        }
    }
    private void LoadAssingned()
    {
        drpAssignedDistributor.Items.Clear();
        if (this.Session["DistributorId"] != null)
        {

            DataTable dt = mUserController.SelectDisToDisAssignment(int.Parse(this.Session["DistributorId"].ToString()), Constants.IntNullValue, 8, int.Parse(this.Session["CompanyId"].ToString()), Convert.ToDateTime(Session["CurrentWorkDate"]));
            clsWebFormUtil.FillDropDownList(drpAssignedDistributor, dt, 0, 2);
        }

    }
    private void CreatTable()
    {
        PurchaseSKU = new DataTable();
        PurchaseSKU.Columns.Add("SALE_ORDER_DETAIL_ID", typeof(long));
        PurchaseSKU.Columns.Add("DistributorId", typeof(int));
        PurchaseSKU.Columns.Add("SALE_ORDER_ID", typeof(int));
        PurchaseSKU.Columns.Add("PRINCIPAL_ID", typeof(int));
        PurchaseSKU.Columns.Add("SKU_ID", typeof(int));
        PurchaseSKU.Columns.Add("SKU_Code", typeof(string));
        PurchaseSKU.Columns.Add("SKU_Name", typeof(string));
        PurchaseSKU.Columns.Add("BATCH_NO", typeof(string));
        PurchaseSKU.Columns.Add("UNIT_PRICE", typeof(decimal));
        PurchaseSKU.Columns.Add("QUANTITY_UNIT", typeof(int));
        PurchaseSKU.Columns.Add("QUANTITY_UNIT2", typeof(int));
        PurchaseSKU.Columns.Add("FREE_SKU", typeof(int));
        PurchaseSKU.Columns.Add("QUANTITY_CTN", typeof(decimal));
        PurchaseSKU.Columns.Add("QUANTITY_CTN2", typeof(decimal));
        PurchaseSKU.Columns.Add("PREV_QUANTITY", typeof(int));
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
        PurchaseSKU.Columns.Add("TST_AMOUNT1", typeof(decimal));
        PurchaseSKU.Columns.Add("CLAIM_EXTRA_AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("CLAIM_STANDARD_DISCOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("CLAIM_PER", typeof(decimal));
        PurchaseSKU.Columns.Add("CLAIM_AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("SED_AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("NET_AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("IS_DELETED", typeof(bool));
        PurchaseSKU.Columns.Add("FMR", typeof(decimal));
        PurchaseSKU.Columns.Add("MR", typeof(decimal));
        PurchaseSKU.Columns.Add("MR_RATE", typeof(decimal));
        PurchaseSKU.Columns.Add("YC_DISCOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("DISTRIBUTOR_PRICE", typeof(decimal));
        PurchaseSKU.Columns.Add("FREE_PRICE", typeof(decimal));
        PurchaseSKU.Columns.Add("UNITS_IN_CASE", typeof(int));
        PurchaseSKU.Columns.Add("COST_OF_GOODS", typeof(decimal));
        PurchaseSKU.Columns.Add("EXTRA_TAX_RATE", typeof(decimal));
        this.Session.Add("PurchaseSKU", PurchaseSKU);
    }
    private void loadCustomerLedgerBalance()
    {
        try
        {
            if (PrincipalWiseSale == 0)
            {
                mPrincipalId = Constants.IntNullValue;
            }
            else
            {
                mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
            }
            RptCustomerController RptCustomerCtl = new RptCustomerController();
            DataTable ds;
            ds = RptCustomerCtl.CustomerLedgerClosing2(mPrincipalId, int.Parse(this.Session["DistributorId"].ToString()), int.Parse(drpcustomer.SelectedValue.ToString()),
                      DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
            txtCustomerLedgerBalance.Text = ds.Rows[0]["Closing_Balance"].ToString();
        }
        catch (Exception e) { string s = e.StackTrace; }
    }
    private void CreateFreeSKU()
    {
        dtFreeSKU = new DataTable();
        dtFreeSKU.Columns.Add("SKU_ID", typeof(int));
        dtFreeSKU.Columns.Add("SKU_Code", typeof(string));
        dtFreeSKU.Columns.Add("SKU_Name", typeof(string));
        dtFreeSKU.Columns.Add("UNIT_PRICE", typeof(decimal));
        dtFreeSKU.Columns.Add("Quantity", typeof(int));
        dtFreeSKU.Columns.Add("ORG_QUANTITY", typeof(int));
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
        try
        {
            OrderEntryController or = new OrderEntryController();
            this.drpDocumentNo.Items.Clear();
            DataTable dtOrder = or.SelectPendingOrderMultiSelection(int.Parse(this.Session["DistributorId"].ToString()), null, Constants.IntNullValue, int.Parse(this.Session["OrderBookerId"].ToString()), int.Parse(this.Session["DeliveryManId"].ToString()), Constants.Order_Pending_Id, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Convert.ToDateTime(this.Session["OrderDate"]), 0);
            drpDocumentNo.Items.Add(new clsListItems("New", Constants.LongNullValue.ToString()));
            clsWebFormUtil.FillDropDownList(this.drpDocumentNo, dtOrder, "ORDER_NO", "ORDER_DETAIL");
            this.Session.Add("dtOrder", dtOrder);
        }
        catch (Exception ex)
        {
            ExceptionPublisher.PublishException(ex);
        }
    }
    protected void drpDocumentNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        RbdiscountType.SelectedValue = "1";
        RbdiscountType_SelectedIndexChanged(null, null);
        if (long.Parse(drpDocumentNo.SelectedValue.ToString()) != Constants.LongNullValue)
        {

            txtBillBookNo.Text = string.Empty;

            DataTable dt = (DataTable)this.Session["dtOrder"];
            DataRow[] foundRows = dt.Select("SALE_ORDER_ID  = '" + drpDocumentNo.SelectedValue.Trim() + "'");
            if (foundRows.Length > 0)
            {
                if (PrincipalWiseSale == 0)
                {
                    mPrincipalId = Constants.IntNullValue;
                }
                else
                {
                    DrpPrincipal.SelectedValue = foundRows[0]["PRINCIPAL_ID"].ToString();
                }
                //DrpPrincipal.SelectedValue = foundRows[0]["PRINCIPAL_ID"].ToString();
                DrpPrincipal.Enabled = false;
                DrpPrincipal_SelectedIndexChanged(null, null);
                drpAssignedDistributor.Enabled = false;
                LoadSKUDetail();
                LoadPromotion();

                txtGrossAmount.Text = foundRows[0]["TOTAL_AMOUNT"].ToString();
                numtxtTotalExtraDiscnt.Text = foundRows[0]["EXTRA_DISCOUNT_AMOUNT"].ToString();
                numTxtTotalStndrdDiscnt.Text = foundRows[0]["STANDARD_DISCOUNT_AMOUNT"].ToString();
                numTxtTotalGST.Text = foundRows[0]["GST_AMOUNT"].ToString();
                numTxtTotalExtra_tax.Text = foundRows[0]["EXTRA_TAX"].ToString();
                numTxtTotlAmnt.Text = foundRows[0]["TOTAL_NET_AMOUNT"].ToString();
                numTxtTotalTST.Text = foundRows[0]["TST_AMOUNT"].ToString();
                drpcustomer.SelectedValue = foundRows[0]["CUSTOMER_ID"].ToString();
                drpcustomer.Enabled = false;
                numtxtTotalExtraDiscnt.Text = foundRows[0]["EXTRA_DISCOUNT_AMOUNT"].ToString();
                numtxtUnClaimabledist.Text = foundRows[0]["SED_AMOUNT"].ToString();

                numtxtYcDiscount.Text = foundRows[0]["YC_DISCOUNT"].ToString();
                numTxtTotlFMR.Text = foundRows[0]["FMR"].ToString();
                numTxtTotlMR.Text = foundRows[0]["MR"].ToString();

                txtTruckNo.Text = foundRows[0]["TRUCK_NO"].ToString();
                txtDriverName.Text = foundRows[0]["DRIVER_NAME"].ToString();
                this.Session.Add("CUSTOMER_ID", long.Parse(foundRows[0]["CUSTOMER_ID"].ToString()));
                this.ExistenOrderDetail(long.Parse(drpDocumentNo.SelectedValue.ToString()));
                txtBillBookNo.Text = foundRows[0]["MANUAL_ORDER_ID"].ToString();
                hfCustomerAreaID.Value = foundRows[0]["AREA_ID"].ToString();
                this.Session.Add("hfBillBookNo", foundRows[0]["MANUAL_ORDER_ID"].ToString());
                EnableDisableController(false);
                btnSaveOrder.Text = "Save Invoice";
                btnSaveOrder.Enabled = false;
                btnUpdateOrder.Enabled = false;

                btnCalculate.Enabled = false;
                this.ClearAll();
                ScriptManager.GetCurrent(Page).SetFocus(DrpSkuDetail);
                RblPayMode.SelectedValue = foundRows[0]["ORDER_TYPE_ID"].ToString();
                RblPayMode_SelectedIndexChanged(null, null);
                try
                {
                    drpAssignedDistributor.SelectedValue = foundRows[0]["ASSIGNED_DISTRIBUTOR_ID"].ToString();
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Stock Used By Location not found');", true);
                    return;
                }
            }
        }
        else
        {
            this.CreateFreeSKU();
            this.CreatTable();
            this.LoadGird();
            this.LoadFreeGrid();
            ClearMasterALL();
            btnSaveOrder.Text = "Save Order";
            this.Session.Remove("hfBillBookNo");
        }
    }
    private void LoadPrincipal()
    {
        SKUPriceDetailController PController = new SKUPriceDetailController();
        DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, m_dt, 0, 1, true);
    }
    protected void DrpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadSKUDetail();
        this.LoadPromotion();
        this.ChkDiscountType();
    }
    private void LoadCustomerData()
    {
        try
        {
            if (int.Parse(this.Session["DistributorId"].ToString()) > 0 && this.Session["AreaId"].ToString() != "")
            {
                drpcustomer.Items.Clear();
                CustomerDataController mController = new CustomerDataController();
                DataTable dtCustomer = mController.SelectPrincipalCustomer4MultiSelection(int.Parse(this.Session["DistributorId"].ToString()), this.Session["AreaId"].ToString(), Constants.IntNullValue, Constants.IntNullValue, 5);
                clsWebFormUtil.FillDropDownList(this.drpcustomer, dtCustomer, "CUSTOMER_ID", "CUSTOMER_DETAIL", true);
                loadCustomerLedgerBalance();
                this.Session.Add("dtCustomer", dtCustomer);
                drpcustomer_SelectedIndexChanged(null, null);
            }
            else
            {
                drpcustomer.Items.Clear();
            }
        }
        catch (Exception ex)
        {
            ExceptionPublisher.PublishException(ex);
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
    private void LoadSKUDetail()
    {
        try
        {
            SKUPriceDetailController PController = new SKUPriceDetailController();
            if (DrpPrincipal.Items.Count > 0)
            {
                if (PrincipalWiseSale == 0)
                {
                    mPrincipalId = Constants.IntNullValue;
                }
                else
                {
                    mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
                }
                DataTable Dtsku_Price = PController.SelectDataPrice(mPrincipalId, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(drpAssignedDistributor.SelectedValue), int.Parse(this.Session["UserId"].ToString()), int.Parse(drpcustomer.SelectedValue), 1, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
                clsWebFormUtil.FillDropDownList(DrpSkuDetail, Dtsku_Price, 0, 20, true);
                this.Session.Add("Dtsku_Price", Dtsku_Price);
                //DrpSkuDetail_SelectedIndexChanged(null, null);
            }
        }
        catch (Exception ex)
        {
            ExceptionPublisher.PublishException(ex);

        }
    }
    private void LoadPromotion()
    {
        try
        {
            DataTable dt = PController.SelectDataPrice(int.Parse(DrpPrincipal.SelectedValue.ToString()), Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["OrderDate"].ToString()));
            if (bool.Parse(dt.Rows[0]["Is_ManualDiscount"].ToString()) == false)
            {
                PromotionCollections_Controller Arpc = orderController.LoadSchemes(int.Parse(this.Session["DistributorId"].ToString()), mPrincipalId, Convert.ToDateTime(this.Session["OrderDate"]));
                this.Session.Add("Arpc", Arpc);
                txtDiscountType.Text = "Auto Discount";
                numtxtUnClaimabledist.ReadOnly = true;
                numTxtTotalStndrdDiscnt.ReadOnly = true;
                numtxtTotalExtraDiscnt.ReadOnly = true;
                txtDistributorDiscount.ReadOnly = true;
                txtDisPer.ReadOnly = true;
                txtEXDPer.ReadOnly = true;
                txtExtradiscount.ReadOnly = true;
                ChbDiscount.Checked = true;
                txtFreeSku.Enabled = false;
                RbdiscountType.Enabled = true;
            }
            else
            {
                if (IsAuto)
                {
                    PromotionCollections_Controller Arpc = orderController.LoadSchemes(int.Parse(this.Session["DistributorId"].ToString()), mPrincipalId, Convert.ToDateTime(this.Session["OrderDate"]));
                    this.Session.Add("Arpc", Arpc);
                }
                txtDiscountType.Text = "Manual Discount";
                numTxtTotalStndrdDiscnt.ReadOnly = true;
                txtDisPer.ReadOnly = true;
                txtEXDPer.ReadOnly = true;
                numtxtTotalExtraDiscnt.ReadOnly = true;
                ChbDiscount.Checked = false;
                numtxtUnClaimabledist.ReadOnly = false;
                txtDistributorDiscount.ReadOnly = false;
                txtExtradiscount.ReadOnly = false;
                if (IsAuto)
                {
                    lblAutoFreeSKU.Visible = true;
                    txtFreeSku.Enabled = false;
                }
                else
                {
                    lblAutoFreeSKU.Visible = false;
                    txtFreeSku.Enabled = true;
                }
                RbdiscountType.Enabled = true;
            }
            //}
        }
        catch (Exception ex)
        {
            ExceptionPublisher.PublishException(ex);
        }
    }
    private void LoadGird()
    {
        try
        {
            PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
            GrdPurchase.DataSource = PurchaseSKU;
            GrdPurchase.DataBind();
            int ObjTotalCtn = 0;
            int ObjTotalfreesku = 0;
            int ObjTotalUnit = 0;
            decimal objtotalAmount = 0;
            decimal ObjDistributorPrice = 0;
            decimal ObjCostOfGoodsAmount = 0;
            decimal ObjStandardDiscount = 0;
            decimal ObjExtraDiscount = 0;
            foreach (DataRow dr in PurchaseSKU.Rows)
            {
                objtotalAmount += decimal.Parse(dr["AMOUNT"].ToString());
                ObjDistributorPrice += decimal.Parse(dr["DISTRIBUTOR_PRICE"].ToString());
                ObjTotalCtn += int.Parse(dr["QUANTITY_CTN"].ToString());
                ObjTotalUnit += int.Parse(dr["QUANTITY_UNIT"].ToString());
                ObjTotalfreesku += int.Parse(dr["FREE_SKU"].ToString());
                ObjStandardDiscount += Convert.ToDecimal(dr["STANDARD_DISCOUNT"]);
                ObjExtraDiscount += Convert.ToDecimal(dr["EXTRA_DISCOUNT"]);
                ObjCostOfGoodsAmount += decimal.Parse(dr["COST_OF_GOODS"].ToString());
            }
            HdDistributorPrice.Value = Convert.ToString(Math.Round(ObjDistributorPrice, 2));
            txttotalCtn.Text = ObjTotalCtn.ToString();
            txttotalUnit.Text = ObjTotalUnit.ToString();
            txttotalAmount.Text = Convert.ToString(Math.Round(objtotalAmount, 2));
            txttotalFreeSku.Text = ObjTotalfreesku.ToString();
            numtxtTotalExtraDiscnt.Text = Convert.ToString(Math.Round(ObjExtraDiscount, 2));
            numTxtTotalStndrdDiscnt.Text = Convert.ToString(Math.Round(ObjStandardDiscount, 2));
            hfCostOfGoods.Value = Convert.ToString(Math.Round(ObjCostOfGoodsAmount, 2));
        }
        catch (Exception ex)
        {
            ExceptionPublisher.PublishException(ex);
        }
    }
    private void LoadFreeGrid()
    {
        dtFreeSKU = (DataTable)this.Session["dtFreeSKU"];
        GrdFreeSKU.DataSource = dtFreeSKU;
        GrdFreeSKU.DataBind();
        decimal objtotalAmount = 0;
        //foreach (DataRow dr in GrdFreeSKU.Rows)
        //{
        //    objtotalAmount += decimal.Parse(dr["AMOUNT"].ToString());
        //}
        foreach (GridViewRow row in GrdFreeSKU.Rows)
        {
            decimal amount = decimal.Parse(row.Cells[4].Text);
            objtotalAmount += decimal.Parse(amount.ToString());
        }
        HFreeSKUAmount.Value = Convert.ToString(Math.Round(objtotalAmount, 4));
    }
    protected void ChbBatchNo_CheckedChanged(object sender, EventArgs e)
    {
        //if (ChbBatchNo.Checked == true)
        //{
        //    txtBatchNo.Text = "";
        //    txtBatchNo.Enabled = true;
        //}
        //else
        //{
        //    txtBatchNo.Enabled = false;
        //    txtBatchNo.Text = "N/A";
        //}
    }
    private bool FindCustomer()
    {
        DataTable dtCustomer = (DataTable)this.Session["dtCustomer"];
        DataRow[] foundRows = dtCustomer.Select("CUSTOMER_ID  = '" + drpcustomer.SelectedValue.Trim() + "'");
        if (foundRows.Length > 0)
        {
            this.Session.Add("CUSTOMER_ID", long.Parse(foundRows[0]["CUSTOMER_ID"].ToString()));
            mCustomerTypeId = int.Parse(foundRows[0]["CHANNEL_TYPE_ID"].ToString());
            mCustomerVolClassId = int.Parse(foundRows[0]["VOLUME_CLASS_ID"].ToString());
            mTownId = int.Parse(foundRows[0]["TOWN_ID"].ToString());
            if (bool.Parse(foundRows[0]["IS_GST_REGISTERED"].ToString()) == false)
            {
                hdnCustomerType.Value = "0";
            }
            else
            {
                hdnCustomerType.Value = "1";
            }

            return true;
        }
        return false;
    }
    private bool CheckDublicateSKU()
    {
        DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
        DataRow[] foundRows = PurchaseSKU.Select("SKU_ID  = '" + DrpSkuDetail.SelectedValue + "'");
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
        //dtFreeSKU = ord.SelectOrderPromotion(int.Parse(this.Session["DistributorId"].ToString()), OrderId);
        this.Session.Add("PurchaseSKU", PurchaseSKU);
        //this.Session.Add("dtFreeSKU", dtFreeSKU);
        this.LoadGird();
        //this.LoadFreeGrid();
    }
    private bool IsBillBookNoExist()
    {
        bool flag = false;
        //if ((long.Parse(drpDocumentNo.SelectedValue) == Constants.LongNullValue || this.Session["hfBillBookNo"].ToString() != txtBillBookNo.Text) && txtBillBookNo.Text.Trim().Length > 0)
        //{
        //    OrderEntryController OEC = new OrderEntryController();
        //    DataTable dtBillBookNo = OEC.SelectBillBookNo(Convert.ToInt32(this.Session["DistributorId"].ToString()), txtBillBookNo.Text, 0);
        //    if (dtBillBookNo.Rows.Count > 0)
        //    {
        //        flag = true;
        //    }
        //    DataTable dtBillBookNo2 = OEC.SelectBillBookNo(Convert.ToInt32(this.Session["DistributorId"].ToString()), txtBillBookNo.Text, 1);
        //    if (dtBillBookNo2.Rows.Count > 0)
        //    {
        //        flag = true;
        //    }
        //}
        return flag;
    }
    private void EnableDisableController(bool CValue)
    {

        if (CValue == true)
        {
            if (decimal.Parse(txtGrossAmount.Text) > 0)
            {
                drpcustomer.Enabled = true;
                DrpPrincipal.Enabled = true;
                drpAssignedDistributor.Enabled = true;
            }
        }
        else
        {
            drpcustomer.Enabled = false;
            DrpPrincipal.Enabled = false;
            drpAssignedDistributor.Enabled = false;
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        this.Session.Remove("CUSTOMER_ID");
        this.Session.Remove("AreaId");
        this.Session.Remove("OrderBookerId");
        this.Session.Remove("DeliveryManId");
        Response.Redirect("~/Forms/frmOrderEntryStep1.aspx?Status=" + false + "&LevelType=3&LevelID=" + Request.QueryString["LevelID"].ToString());
    }
    protected void btnCalculate_Click(object sender, EventArgs e)
    {
        CalculateDiscount();
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
        DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + DrpSkuDetail.SelectedValue + "'");
        decimal mTradePrice = decimal.Parse(dc.chkNull_0(txtUnitRate.Text.ToString()));
        decimal PURCHASE_PRICE = decimal.Parse(dc.chkNull_0(foundRows[0]["PURCHASE_PRICE"].ToString()));
        decimal DISTRIBUTOR_PRICE = decimal.Parse(dc.chkNull_0(foundRows[0]["DISTRIBUTOR_PRICE"].ToString()));
        int PRINCIPAL_ID = int.Parse(dc.chkNull_0(foundRows[0]["PRINCIPAL_ID"].ToString()));
        decimal SDiscount = decimal.Parse(dc.chkNull_0(txtDistributorDiscount.Text));
        decimal EDiscount = decimal.Parse(dc.chkNull_0(txtExtradiscount.Text));
        decimal mEXTRA_TAX_RATE = decimal.Parse(dc.chkNull_0(foundRows[0]["EXTRA_TAX_RATE"].ToString()));
        decimal mMR = 0;
        int mPackSize = int.Parse(dc.chkNull_0(foundRows[0]["UNITS_IN_CASE"].ToString()));
        decimal Extra_tax = 2;

        if (btnSave.Text == "Add Sku")
        {
            if (CheckDublicateSKU())
            {
                DataRow dr = PurchaseSKU.NewRow();
                dr["SKU_ID"] = foundRows[0]["SKU_ID"];
                dr["SKU_Code"] = foundRows[0]["SKU_CODE"];
                dr["SKU_Name"] = foundRows[0]["SKU_CODE"] + "-" + foundRows[0]["SKU_NAME"];
                dr["QUANTITY_UNIT"] = int.Parse(dc.chkNull_0(txtQuantity.Text));
                dr["QUANTITY_UNIT2"] = 0;
                dr["FREE_SKU"] = int.Parse(dc.chkNull_0(txtFreeSku.Text));
                dr["QUANTITY_CTN"] = int.Parse(dc.chkNull_0(txtQuantityCtn.Text));
                dr["QUANTITY_CTN2"] = 0;
                dr["QUANTITY"] = (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
                dr["UNIT_PRICE"] = mTradePrice.ToString();
                dr["COST_OF_GOODS"] = (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize)) * PURCHASE_PRICE;
                if (RbStdDiscType.SelectedValue == "0")
                {
                    SDiscount = (mTradePrice * decimal.Parse(dr["QUANTITY"].ToString()) * (SDiscount / 100));
                }
                else
                {
                    if (Session["ManualDiscounOn"].ToString() == "1")
                    {
                        SDiscount = (decimal.Parse(dr["QUANTITY"].ToString()) / mPackSize) * SDiscount;
                    }
                    else if (Session["ManualDiscounOn"].ToString() == "2")
                    {
                        SDiscount = decimal.Parse(dr["QUANTITY"].ToString()) * SDiscount;
                    }
                }
                if (RbExtraDiscType.SelectedValue == "0")
                {
                    EDiscount = (mTradePrice * decimal.Parse(dr["QUANTITY"].ToString()) * (EDiscount / 100));
                }
                else
                {
                    if (Session["ManualDiscounOn"].ToString() == "1")
                    {
                        EDiscount = (decimal.Parse(dr["QUANTITY"].ToString()) / mPackSize) * EDiscount;
                    }
                    else if (Session["ManualDiscounOn"].ToString() == "2")
                    {
                        EDiscount = decimal.Parse(dr["QUANTITY"].ToString()) * EDiscount;
                    }
                }
                if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                {
                    dr["AMOUNT"] = mTradePrice * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
                    dr["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
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
                if (hdnCustomerType.Value == "0" && isGSTCal.Checked == true)
                {
                    var gross = mTradePrice * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
                    dr["EXTRA_TAX"] = gross * Extra_tax / 100;
                }
                else
                {
                    dr["EXTRA_TAX"] = 0;
                }
                dr["TST_AMOUNT1"] = dr["TST_AMOUNT"];
                dr["MR_RATE"] = mMR;
                dr["MR"] = 0;
                dr["FMR"] = 0;
                dr["STANDARD_DISCOUNT"] = SDiscount;
                dr["EXTRA_DISCOUNT"] = EDiscount;
                dr["YC_DISCOUNT"] = 0;
                dr["GST_AMOUNT"] = 0;
                dr["NET_AMOUNT"] = 0;
                dr["PREV_QUANTITY"] = 0;
                dr["DISTRIBUTOR_PRICE"] = DISTRIBUTOR_PRICE * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
                dr["FREE_PRICE"] = mTradePrice; //DISTRIBUTOR_PRICE;
                dr["PRINCIPAL_ID"] = PRINCIPAL_ID;
                dr["UNITS_IN_CASE"] = mPackSize;
                dr["EXTRA_TAX_RATE"] = mEXTRA_TAX_RATE;
                if (btnSaveOrder.Text != "Sale Return")
                {
                    DataTable dtAppSetting = (DataTable)Session["dtAppSetting"];
                    if (dtAppSetting != null)
                    {
                        DataRow[] drAppSetting = dtAppSetting.Select("strColumnName='StockOnOrder'");
                        if (drAppSetting.Length > 0)
                        {
                            if (drAppSetting[0]["strColumnValue"].ToString() == "1")
                            {
                                int Stock = 0;
                                if (txtStock.Text != "0-0")
                                {
                                    string[] strstock = txtStock.Text.Split('-');
                                    Stock += Convert.ToInt32(strstock[0]) * Convert.ToInt32(dr["UNITS_IN_CASE"]);
                                    Stock += Convert.ToInt32(strstock[1]);
                                }

                                if ((int.Parse(dr["QUANTITY_CTN"].ToString()) * int.Parse(dr["UNITS_IN_CASE"].ToString())) + int.Parse(dr["QUANTITY_UNIT"].ToString()) > Stock)
                                {
                                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Available Stock is " + txtStock.Text + "')", true);
                                    return;
                                }
                            }
                        }
                    }
                }

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
            dr["SKU_Name"] = foundRows[0]["SKU_CODE"] + "-" + foundRows[0]["SKU_NAME"];
            dr["BATCH_NO"] = "N/A";
            dr["QUANTITY_UNIT"] = int.Parse(dc.chkNull_0(txtQuantity.Text));
            dr["QUANTITY_UNIT2"] = dr["QUANTITY_UNIT2"];
            dr["FREE_SKU"] = int.Parse(dc.chkNull_0(txtFreeSku.Text));
            dr["QUANTITY_CTN"] = int.Parse(dc.chkNull_0(txtQuantityCtn.Text));
            dr["QUANTITY_CTN2"] = dr["QUANTITY_CTN2"];
            dr["QUANTITY"] = (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
            dr["UNIT_PRICE"] = mTradePrice.ToString();
            dr["COST_OF_GOODS"] = (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize)) * PURCHASE_PRICE;
            if (RbStdDiscType.SelectedValue == "0")
            {
                SDiscount = (mTradePrice * decimal.Parse(dr["QUANTITY"].ToString()) * (SDiscount / 100));
            }
            else
            {
                if (Session["ManualDiscounOn"].ToString() == "1")
                {
                    SDiscount = (decimal.Parse(dr["QUANTITY"].ToString()) / mPackSize) * SDiscount;
                }
                else if (Session["ManualDiscounOn"].ToString() == "2")
                {
                    SDiscount = decimal.Parse(dr["QUANTITY"].ToString()) * SDiscount;
                }
            }
            if (RbExtraDiscType.SelectedValue == "0")
            {
                EDiscount = (mTradePrice * decimal.Parse(dr["QUANTITY"].ToString()) * (EDiscount / 100));
            }
            else
            {
                if (Session["ManualDiscounOn"].ToString() == "1")
                {
                    EDiscount = (decimal.Parse(dr["QUANTITY"].ToString()) / mPackSize) * EDiscount;
                }
                else if (Session["ManualDiscounOn"].ToString() == "2")
                {
                    EDiscount = decimal.Parse(dr["QUANTITY"].ToString()) * EDiscount;
                }
            }
            dr["AMOUNT"] = mTradePrice * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
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
            if (hdnCustomerType.Value == "0" && isGSTCal.Checked == true)
            {
                var gross = mTradePrice * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
                dr["EXTRA_TAX"] = gross * Extra_tax / 100;
            }
            else
            {
                dr["EXTRA_TAX"] = 0;
            }
            dr["TST_AMOUNT1"] = dr["TST_AMOUNT"];
            dr["MR_RATE"] = mMR;
            dr["MR"] = 0;
            dr["FMR"] = 0;
            dr["STANDARD_DISCOUNT"] = SDiscount;
            dr["EXTRA_DISCOUNT"] = EDiscount;
            dr["YC_DISCOUNT"] = 0;
            dr["GST_AMOUNT"] = 0;
            dr["NET_AMOUNT"] = 0;
            dr["STANDARD_DISCOUNT_PER"] = 0;
            dr["DISTRIBUTOR_PRICE"] = DISTRIBUTOR_PRICE * (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));
            dr["FREE_PRICE"] = mTradePrice; //DISTRIBUTOR_PRICE;
            dr["PRINCIPAL_ID"] = PRINCIPAL_ID;
            dr["UNITS_IN_CASE"] = mPackSize;
            dr["EXTRA_TAX_RATE"] = mEXTRA_TAX_RATE;
            if (btnSaveOrder.Text != "Sale Return")
            {
                if (btnSaveOrder.Text == "Save Order")
                {
                    DataTable dtAppSetting = (DataTable)Session["dtAppSetting"];
                    if (dtAppSetting != null)
                    {
                        DataRow[] drAppSetting = dtAppSetting.Select("strColumnName='StockOnOrder'");
                        if (drAppSetting.Length > 0)
                        {
                            if (drAppSetting[0]["strColumnValue"].ToString() == "1")
                            {
                                int Stock = 0;
                                if (txtStock.Text != "0-0")
                                {
                                    string[] strstock = txtStock.Text.Split('-');
                                    Stock += Convert.ToInt32(strstock[0]) * Convert.ToInt32(dr["UNITS_IN_CASE"]);
                                    Stock += Convert.ToInt32(strstock[1]);
                                }
                                if ((int.Parse(dr["QUANTITY_CTN"].ToString()) * int.Parse(dr["UNITS_IN_CASE"].ToString())) + int.Parse(dr["QUANTITY_UNIT"].ToString()) > Stock)
                                {
                                    string strStock = (Stock / int.Parse(dr["UNITS_IN_CASE"].ToString())).ToString() + "-" + (Stock % int.Parse(dr["UNITS_IN_CASE"].ToString())).ToString();
                                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Available Stock is " + strStock + "')", true);
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                {
                    int Stock = 0;
                    if (txtStock.Text != "0-0")
                    {
                        string[] strstock = txtStock.Text.Split('-');
                        Stock += Convert.ToInt32(strstock[0]) * Convert.ToInt32(dr["UNITS_IN_CASE"]);
                        Stock += Convert.ToInt32(strstock[1]);
                    }
                    if ((int.Parse(dr["QUANTITY_CTN"].ToString()) * int.Parse(dr["UNITS_IN_CASE"].ToString())) + int.Parse(dr["QUANTITY_UNIT"].ToString()) > Stock)
                    {
                        string strStock = (Stock / int.Parse(dr["UNITS_IN_CASE"].ToString())).ToString() + "-" + (Stock % int.Parse(dr["UNITS_IN_CASE"].ToString())).ToString();
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Available Stock is " + strStock + "')", true);
                        return;
                    }
                }
            }
        }

        this.Session.Add("PurchaseSKU", PurchaseSKU);
        this.EnableDisableController(false);
        //CalculateDiscount();
        this.LoadGird();
        this.ClearAll();
        ScriptManager.GetCurrent(Page).SetFocus(DrpSkuDetail);
        //DrpSkuDetail_SelectedIndexChanged(null, null);
        if (ChbDiscount.Checked == false)
        {
            decimal ObjGrossSales = 0;
            decimal ObjTaxableAmount = 0;
            decimal ObjFMR = 0;
            decimal ObjMR = 0;
            decimal ObjDistributorPrice = 0;

            foreach (DataRow dr in PurchaseSKU.Rows)
            {
                ObjGrossSales += decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                ObjDistributorPrice += decimal.Parse(dc.chkNull_0(dr["DISTRIBUTOR_PRICE"].ToString()));
                if (dr["BATCH_NO"].ToString() == "T")
                {
                    ObjTaxableAmount = decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                }
                ObjFMR += decimal.Parse(dc.chkNull_0(dr["FMR"].ToString()));
                ObjMR += decimal.Parse(dc.chkNull_0(dr["MR"].ToString()));
            }
            txtGrossAmount.Text = Convert.ToString(Math.Round(ObjGrossSales, 2));
            HdDistributorPrice.Value = Convert.ToString(Math.Round(ObjDistributorPrice, 2));
            numTxtTotalSED.Text = Convert.ToString(Math.Round(ObjTaxableAmount, 2));
            numTxtTotlFMR.Text = Convert.ToString(Math.Round(ObjFMR, 2));
            numTxtTotlMR.Text = Convert.ToString(Math.Round(ObjMR, 2));
        }
    }

    protected void btnSaveOrder_Click(object sender, EventArgs e)
    {
        if (drpAssignedDistributor.Items.Count > 0)
        { }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Assigned Location Not found !');", true);
            return;
        }
        string p_voucher_No;
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
                    WHTax = decimal.Parse(foundRows[0]["WHTAX"].ToString());
                }
                string ManualID = "";
                if (txtBillBookNo.Text.Trim().Length > 0)
                {
                    ManualID = txtBillBookNo.Text.ToUpper();
                }
                if (PrincipalWiseSale == 0)
                {
                    mPrincipalId = 0;
                }
                else
                {
                    mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
                }
                DataControl DC = new DataControl();
                PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
                dtFreeSKU = (DataTable)this.Session["dtFreeSKU"];
                OrderEntryController mOrderController = new OrderEntryController();

                if (btnSaveOrder.Text == "Save Order")
                {
                    if (!IsBillBookNoExist())
                    {
                        bool IsValidInsert = mOrderController.Add_OrderNew(int.Parse(this.Session["DistributorId"].ToString()), ManualID, mTownId, long.Parse(hfCustomerAreaID.Value), mPrincipalId, long.Parse(this.Session["CUSTOMER_ID"].ToString()), long.Parse(this.Session["CUSTOMER_ID"].ToString())
                            , int.Parse(this.Session["OrderBookerId"].ToString()), int.Parse(this.Session["DeliveryManId"].ToString()), int.Parse(RblPayMode.SelectedValue.ToString()),
                        decimal.Parse(DC.chkNull_0(txtGrossAmount.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalExtraDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalStndrdDiscnt.Text))
                        , decimal.Parse(DC.chkNull_0(numTxtTotalGST.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlAmnt.Text)), 0, Constants.Order_Pending_Id, PurchaseSKU, dtFreeSKU, int.Parse(this.Session["UserId"].ToString())
                        , DateTime.Parse(this.Session["OrderDate"].ToString()), decimal.Parse(DC.chkNull_0(numtxtUnClaimabledist.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalTST.Text)), decimal.Parse(DC.chkNull_0(numtxtYcDiscount.Text))
                        , decimal.Parse(DC.chkNull_0(numTxtTotlFMR.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlMR.Text)), txtTruckNo.Text, txtDriverName.Text, decimal.Parse(DC.chkNull_0(numTxtTotalExtra_tax.Text)), isGSTCal.Checked
                        , int.Parse(drpAssignedDistributor.SelectedValue), decimal.Parse(DC.chkNull_0(hfCostOfGoods.Value)), decimal.Parse(DC.chkNull_0(HFreeSKUAmount.Value)), WHTax);
                        if (IsValidInsert)
                        {
                            LoadSKUDetail();
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
                        if (CreditLimit())
                        {
                            if (!IsBillBookNoExist())
                            {
                                #region Check Available Stock
                                foreach (DataRow dr in PurchaseSKU.Rows)
                                {
                                    PhaysicalStockController PhyscalCtl = new PhaysicalStockController();
                                    DataTable dtstock = PhyscalCtl.GetOrderClosingStock(Convert.ToInt32(Session["DistributorId"]), Convert.ToInt32(dr["SKU_ID"]), Convert.ToDateTime(Session["OrderDate"]));
                                    if (dtstock.Rows.Count > 0)
                                    {
                                        if (int.Parse(dtstock.Rows[0][0].ToString()) < int.Parse(dr["QUANTITY"].ToString()))
                                        {
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('" + dr["SKU_Code"].ToString() + " Current Stock is " + dtstock.Rows[0][0].ToString() + "');", true);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert(' + " + dr["SKU_Code"].ToString() + "No Stock Found');", true);
                                        return;
                                    }
                                }
                                #endregion

                                bool IsValidInsert = mOrderController.Add_InvoiceNew(int.Parse(this.Session["DistributorId"].ToString()), ManualID, mTownId, long.Parse(hfCustomerAreaID.Value), mPrincipalId, long.Parse(this.Session["CUSTOMER_ID"].ToString()), long.Parse(this.Session["CUSTOMER_ID"].ToString()), int.Parse(this.Session["OrderBookerId"].ToString()), int.Parse(this.Session["DeliveryManId"].ToString()), long.Parse(drpDocumentNo.SelectedValue.ToString()),
                                decimal.Parse(DC.chkNull_0(txtGrossAmount.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalExtraDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalStndrdDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalGST.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlAmnt.Text)), 0, int.Parse(RblPayMode.SelectedValue.ToString()),
                                PurchaseSKU, dtFreeSKU, int.Parse(this.Session["UserId"].ToString()), decimal.Parse(DC.chkNull_0(txtCashReceived.Text)), DateTime.Parse(this.Session["OrderDate"].ToString()), decimal.Parse(DC.chkNull_0(numTxtTotalTST.Text)), decimal.Parse(DC.chkNull_0(numtxtUnClaimabledist.Text)), decimal.Parse(DC.chkNull_0(numtxtYcDiscount.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlFMR.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlMR.Text)), txtTruckNo.Text, txtDriverName.Text, decimal.Parse(DC.chkNull_0(numTxtTotalExtra_tax.Text)), isGSTCal.Checked, int.Parse(drpAssignedDistributor.SelectedValue)
                                , out p_voucher_No, "", drpcustomer.SelectedItem.Text, txtDriverName.Text, decimal.Parse(DC.chkNull_0(HdDistributorPrice.Value)), decimal.Parse(DC.chkNull_0(HFreeSKUAmount.Value)), WHTax);
                                if (IsValidInsert)
                                {
                                    if (drpDocumentNo.Enabled == true)
                                    {
                                        btnSaveOrder.Text = "Save Order";
                                    }
                                    LoadSKUDetail();
                                    this.LoadPendingOrder();
                                    this.ClearMasterALL();
                                    this.Session.Remove("hfBillBookNo");
                                    RblPayMode_SelectedIndexChanged(null, null);
                                }
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('This Bill Book No already exist,Kindly enter different Bill Book No');", true);
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Check Customer Business Limit');", true);
                        }   
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Order Date Can not be greater than current working date');", true);
                    }
                }
                else if (btnSaveOrder.Text == "Sale Return")
                {
                    bool IsValidInsert = mOrderController.Add_SaleReturnNew2(int.Parse(this.Session["DistributorId"].ToString()), mTownId, long.Parse(hfCustomerAreaID.Value), mPrincipalId, long.Parse(this.Session["CUSTOMER_ID"].ToString()), long.Parse(this.Session["CUSTOMER_ID"].ToString()), int.Parse(this.Session["OrderBookerId"].ToString()), int.Parse(this.Session["DeliveryManId"].ToString()), long.Parse(drpDocumentNo.SelectedValue.ToString()),
                    decimal.Parse(DC.chkNull_0(txtGrossAmount.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalExtraDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalStndrdDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalGST.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlAmnt.Text)), 0, int.Parse(RblPayMode.SelectedValue.ToString()), PurchaseSKU, dtFreeSKU, int.Parse(this.Session["UserId"].ToString()), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), decimal.Parse(DC.chkNull_0(numTxtTotalTST.Text)), decimal.Parse(DC.chkNull_0(numtxtUnClaimabledist.Text)),
                    decimal.Parse(DC.chkNull_0(numTxtTotalExtra_tax.Text)),
                    isGSTCal.Checked, int.Parse(drpAssignedDistributor.SelectedValue), decimal.Parse(DC.chkNull_0(numtxtYcDiscount.Text))
                    , out p_voucher_No, txtDriverName.Text, WHTax);
                    if (IsValidInsert)
                    {
                        LoadSKUDetail();
                        this.LoadPendingOrder();
                        this.ClearMasterALL();
                    }
                }
            }
        }
    }

    private void ClearAll()
    {
        txtUnitRate.Text = "";
        txtQuantity.Text = "";
        txtQuantityCtn.Text = "";
        txtFreeSku.Text = "";
        btnSave.Text = "Add Sku";
        btnSaveOrder.Enabled = false;
        btnUpdateOrder.Enabled = false;
        btnCalculate.Enabled = true;
        DrpSkuDetail.Enabled = true;
        txtExtradiscount.Text = "";
        txtDistributorDiscount.Text = "";
    }

    private void ClearMasterALL()
    {
        this.EnableDisableController(true);
        this.Session.Remove("PurchaseSKU");
        this.Session.Remove("dtFreeSKU");
        btnSaveOrder.Enabled = false;
        btnUpdateOrder.Enabled = false;
        this.CreatTable();
        this.CreateFreeSKU();

        this.LoadGird();
        this.LoadFreeGrid();
        txtGrossAmount.Text = "";
        numtxtTotalExtraDiscnt.Text = "";
        numTxtTotalStndrdDiscnt.Text = "";
        numTxtTotalGST.Text = "";

        numTxtTotlAmnt.Text = "";
        numTxtTotalSED.Text = "";
        numTxtTotalTST.Text = "";
        numTxtTotalSED.Text = "";
        numTxtTotalExtra_tax.Text = "";
        numtxtUnClaimabledist.Text = "";
        numtxtYcDiscount.Text = "";
        numTxtTotlFMR.Text = "";
        numTxtTotlMR.Text = "";
        txtTruckNo.Text = "";
        txtDriverName.Text = "";
        txtDisPer.Text = "";
        txtEXDPer.Text = "";
        txtDDPer.Text = "";
        txttotalFreeSku.Text = "";
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

    protected void btnUpdateOrder_Click(object sender, EventArgs e)
    {
        if (drpDocumentNo.SelectedIndex == 0)
        { return; }
        if (drpAssignedDistributor.Items.Count > 0)
        {
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Assigned Location Not found !');", true);
            return;
        }

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
                string ManualID = null;
                if (txtBillBookNo.Text.Trim().Length > 0)
                {
                    ManualID = txtBillBookNo.Text.ToUpper();
                }
                if (PrincipalWiseSale == 0)
                {
                    mPrincipalId = Constants.IntNullValue;
                }
                else
                {
                    mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
                }
                DataControl DC = new DataControl();
                PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
                dtFreeSKU = (DataTable)this.Session["dtFreeSKU"];
                OrderEntryController mOrderController = new OrderEntryController();

                if (!IsBillBookNoExist())
                {
                    bool stockavail = true;
                    string msg = "";
                    DataTable dtAppSetting = (DataTable)Session["dtAppSetting"];
                    if (dtAppSetting != null)
                    {
                        DataRow[] drAppSetting = dtAppSetting.Select("strColumnName='StockOnOrder'");
                        if (drAppSetting.Length > 0)
                        {
                            if (drAppSetting[0]["strColumnValue"].ToString() == "1")
                            {
                                foreach (DataRow dr in PurchaseSKU.Rows)
                                {
                                    PhaysicalStockController mController = new PhaysicalStockController();
                                    DataTable dtstock = mController.SelectSKUClosingStock(int.Parse(drpAssignedDistributor.SelectedValue), int.Parse(dr["SKU_ID"].ToString()), "N/A", DateTime.Parse(this.Session["OrderDate"].ToString()), 17);
                                    if (dtstock.Rows.Count > 0)
                                    {
                                        if (int.Parse(dtstock.Rows[0][0].ToString()) + int.Parse(dr["PREV_QUANTITY"].ToString()) < int.Parse(dr["QUANTITY"].ToString()))
                                        {
                                            msg = msg + dr["SKU_Code"].ToString() + " Current Stock is " + dtstock.Rows[0][0].ToString() + " \\n";
                                            stockavail = false;

                                        }
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert(' + " + dr["SKU_Code"].ToString() + "No Stock Found');", true);
                                        return;
                                    }
                                }
                                if (!stockavail)
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('" + msg.ToString() + "');", true);
                                    return;
                                }
                            }
                        }
                    }

                    bool IsValidInsert = mOrderController.Update_Order(int.Parse(drpDocumentNo.SelectedValue), int.Parse(this.Session["DistributorId"].ToString()), ManualID, mTownId, long.Parse(hfCustomerAreaID.Value), mPrincipalId
                        , long.Parse(this.Session["CUSTOMER_ID"].ToString()), long.Parse(this.Session["CUSTOMER_ID"].ToString()), int.Parse(this.Session["OrderBookerId"].ToString()), int.Parse(this.Session["DeliveryManId"].ToString())
                        , int.Parse(RblPayMode.SelectedValue.ToString()), decimal.Parse(DC.chkNull_0(txtGrossAmount.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalExtraDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalStndrdDiscnt.Text))
                        , decimal.Parse(DC.chkNull_0(numTxtTotalGST.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlAmnt.Text)), 0, Constants.Order_Pending_Id, PurchaseSKU, dtFreeSKU, int.Parse(this.Session["UserId"].ToString())
                        , DateTime.Parse(this.Session["OrderDate"].ToString()), decimal.Parse(DC.chkNull_0(numtxtUnClaimabledist.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalTST.Text)), decimal.Parse(DC.chkNull_0(numtxtYcDiscount.Text))
                        , decimal.Parse(DC.chkNull_0(numTxtTotlFMR.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlMR.Text)), txtTruckNo.Text, txtDriverName.Text, decimal.Parse(DC.chkNull_0(numTxtTotalExtra_tax.Text)), isGSTCal.Checked
                        , int.Parse(drpAssignedDistributor.SelectedValue), decimal.Parse(DC.chkNull_0(HdDistributorPrice.Value)), decimal.Parse(DC.chkNull_0(HFreeSKUAmount.Value)));
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
        }
    }

    protected void RbdiscountType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (RbdiscountType.SelectedValue == "0")
        {
            txtDisPer.Enabled = false;
            txtEXDPer.Enabled = false;
            txtDDPer.Enabled = true;
            numTxtTotalStndrdDiscnt.Enabled = false;
            numtxtTotalExtraDiscnt.Enabled = false;
            numtxtYcDiscount.Enabled = false;
            numtxtYcDiscount.Text = "";
        }
        else
        {
            txtDisPer.Enabled = false;
            txtEXDPer.Enabled = false;
            txtDDPer.Enabled = false;
            txtDDPer.Text = "";
            numTxtTotalStndrdDiscnt.Enabled = false;
            numtxtTotalExtraDiscnt.Enabled = false;
            numtxtYcDiscount.Enabled = true;
        }

    }

    private void CalculateDiscount()
    {
        if (FindCustomer())
        {
            decimal ObjGrossSales = 0;
            decimal ObjStandardDiscount = 0;
            decimal ObjExtraDiscount = 0;
            decimal ObjTotalGST = 0;

            decimal ObjTotalEXTRA_TAX = 0;
            decimal ObjTotalSED = 0;
            decimal ObjTotalTST = 0;
            decimal ObjFreeGSTAmount = 0;
            decimal ObjFreeTST = 0;
            decimal ObjFreeGrossAmt = 0;
            decimal ProductExtDist = 0;
            decimal ProductStdDist = 0;
            decimal ObjFMR = 0;
            decimal ObjMR = 0;
            decimal ObjNetSales = 0;
            decimal ObjYcDiscount = 0;
            decimal ProductYcDiscount = 0;
            DataControl dc = new DataControl();
            OrderEntryController or = new OrderEntryController();
            SKUGroupController GroupCtl = new SKUGroupController();
            DataTable dt = (DataTable)this.Session["PurchaseSKU"];
            foreach (DataRow dr in dt.Rows)
            {
                if (txtDiscountType.Text != "Manual Discount")
                {
                    dr["STANDARD_DISCOUNT"] = 0.00;
                    dr["YC_DISCOUNT"] = 0.00;
                    dr["STANDARD_DISCOUNT_PER"] = 0.00;
                    dr["EXTRA_DISCOUNT"] = 0.00;
                    dr["CLAIM_EXTRA_AMOUNT"] = 0.00;
                    dr["CLAIM_STANDARD_DISCOUNT"] = 0.00;
                    dr["CLAIM_PER"] = 0.00;
                    dr["CLAIM_AMOUNT"] = 0.00;
                    dr["SED_AMOUNT"] = 0.00;
                }
                else
                {
                    dr["YC_DISCOUNT"] = 0.00;
                    dr["STANDARD_DISCOUNT_PER"] = 0.00;
                    dr["CLAIM_EXTRA_AMOUNT"] = 0.00;
                    dr["CLAIM_STANDARD_DISCOUNT"] = 0.00;
                    dr["CLAIM_PER"] = 0.00;
                    dr["CLAIM_AMOUNT"] = 0.00;
                    dr["SED_AMOUNT"] = 0.00;
                }
            }

            PromotionCollections_Controller Arpc = (PromotionCollections_Controller)this.Session["Arpc"];
            this.CreateFreeSKU();
            this.LoadFreeGrid();

            if (dt.Rows.Count > 0 && drpcustomer.Items.Count > 0)
            {
                if (ChbDiscount.Checked == true)
                {
                    #region Auto Promotion
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
                                                dr["STANDARD_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString())) + pofferCol.Offer_Value;
                                                dr["CLAIM_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_AMOUNT"].ToString())) + pofferCol.Offer_Value;
                                            }
                                            if (pofferCol.Discount > 0)
                                            {
                                                dr["STANDARD_DISCOUNT_PER"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT_PER"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100);
                                                dr["CLAIM_PER"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_PER"].ToString())) + (decimal.Parse(pofferCol.Discount.ToString()) / 100);
                                            }
                                        }
                                        if (pofferCol.Free_SKU_ID > 0)
                                        {
                                            //Free SKU
                                            if (ChbDiscount.Checked == true || pofferCol.Is_And != true)
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
                                                        dr1["ORG_QUANTITY"] = 0;
                                                        if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                                        {
                                                            dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                            dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                            dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                            dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                            dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                            dr1["TST_AMOUNT"] = 0;
                                                        }
                                                        else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                        {
                                                            dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                            dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                            dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                            dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                            dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
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
                                                        //dr1["ORG_QUANTITY"] = 0;
                                                        dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;
                                                        if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                                        {

                                                            dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                            dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                            dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                            dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                            dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                            dr1["TST_AMOUNT"] = 0;

                                                        }
                                                        else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                        {
                                                            dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                            dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                            dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                            dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                            dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
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

                                //Free SKU
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
                                                dr["STANDARD_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString())) + pofferCol.Offer_Value;
                                                dr["CLAIM_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_AMOUNT"].ToString())) + pofferCol.Offer_Value;
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
                                            //Free SKU
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
                                                    //dr1["ORG_QUANTITY"] = 0;
                                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                                    {

                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                        dr1["TST_AMOUNT"] = 0;

                                                    }
                                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
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
                                                    //dr1["ORG_QUANTITY"] = 0;
                                                    dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;

                                                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                                    {

                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        //dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                        dr1["TST_AMOUNT"] = 0;

                                                    }
                                                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        //dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                        dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                        dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                        dr1["GST_AMOUNT"] = 0;
                                                    }
                                                    else
                                                    {
                                                        dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                        //dr1["Quantity"] = pofferCol.Quantity.ToString();
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
                                
                                foreach (DataRow dr in dt.Rows)
                                {
                                    if (GroupCtl.ExistsInGroup(Constants.IntNullValue, pofferCol.Group_ID, int.Parse(dr["SKU_ID"].ToString())))
                                    {
                                        if (pofferCol.Is_And == true)
                                        {
                                            if (pofferCol.Offer_Value > 0)
                                            {
                                                var Value = decimal.Parse(dc.chkNull_0(dr["QUANTITY"].ToString())) * pofferCol.Offer_Value;
                                                dr["EXTRA_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString())) + Value;
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
                                                var Value = decimal.Parse(dc.chkNull_0(dr["QUANTITY"].ToString())) * pofferCol.Offer_Value;
                                                dr["STANDARD_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString())) + Value;
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
                        dr["STANDARD_DISCOUNT"] = Convert.ToDecimal(dr["STANDARD_DISCOUNT"]) + (decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT_PER"].ToString())) * (decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()))));
                        dr["CLAIM_STANDARD_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_AMOUNT"].ToString())) + (decimal.Parse(dc.chkNull_0(dr["CLAIM_PER"].ToString())) * (decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()))));
                        dr["SED_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_STANDARD_DISCOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["CLAIM_EXTRA_AMOUNT"].ToString()));

                        ObjGrossSales += decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        ObjStandardDiscount += decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString()));
                        ObjExtraDiscount += decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString()));
                        ObjTotalSED += decimal.Parse(dc.chkNull_0(dr["SED_AMOUNT"].ToString()));

                        #region GST Calculation
                        decimal TempAmount = decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        decimal TempDiscount = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString()));
                        decimal TempExtraDisAmt = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString()));
                        decimal TempFMRAmt = decimal.Parse(dc.chkNull_0(dr["FMR"].ToString()));
                        decimal TempYCDisAmt = decimal.Parse(dc.chkNull_0(dr["YC_DISCOUNT"].ToString()));

                        dr["GST_AMOUNT"] = (TempAmount - TempDiscount - TempExtraDisAmt) * decimal.Parse(dc.chkNull_0(dr["GST_RATE"].ToString())) / 100;
                        dr["TST_AMOUNT"] = dr["TST_AMOUNT1"];

                        decimal TempGST_AMOUNT = decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString()));
                        decimal TempTST_AMOUNT = decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));
                        dr["MR"] = (TempAmount - TempDiscount - TempExtraDisAmt + TempGST_AMOUNT + TempTST_AMOUNT) * decimal.Parse(dc.chkNull_0(dr["MR_RATE"].ToString())) / 100;
                        decimal TempMRAmt = decimal.Parse(dc.chkNull_0(dr["MR"].ToString()));

                        // Calculate Extra Tax

                        #region EXTRA TAX CALCULATION
                        if (DistributorIsRegistered())
                        {
                            if (!CustomerIsRegistered())
                            {
                                if (dr["BATCH_NO"].ToString() == "T")
                                {
                                    dr["EXTRA_TAX"] = Math.Round(Convert.ToDecimal(((TempAmount - TempDiscount - TempExtraDisAmt - TempFMRAmt - TempMRAmt - TempYCDisAmt) * decimal.Parse(dr["EXTRA_TAX_RATE"].ToString())) / 100), 2);
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

                        ObjFMR += decimal.Parse(dc.chkNull_0(dr["FMR"].ToString()));
                        ObjMR += decimal.Parse(dc.chkNull_0(dr["MR"].ToString()));
                        dr["NET_AMOUNT"] = (TempAmount - TempDiscount - TempExtraDisAmt - TempFMRAmt - TempMRAmt - TempYCDisAmt) + decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["EXTRA_TAX"].ToString()));
                        ObjTotalGST += decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString()));
                        ObjTotalEXTRA_TAX += decimal.Parse(dc.chkNull_0(dr["EXTRA_TAX"].ToString()));
                        ObjTotalTST += decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));

                        #endregion
                    }

                    #region Distributor Discount
                    if (RbdiscountType.SelectedValue == "0")
                    {
                        if (decimal.Parse(dc.chkNull_0(txtDDPer.Text)) > 0)
                        {
                            ProductYcDiscount = decimal.Parse(dc.chkNull_0(txtDDPer.Text)) / 100;
                        }
                    }
                    else
                    {
                        if (decimal.Parse(dc.chkNull_0(numtxtYcDiscount.Text)) > 0)
                        {
                            ProductYcDiscount = decimal.Parse(dc.chkNull_0(numtxtYcDiscount.Text)) / (ObjGrossSales - ObjStandardDiscount);
                        }
                    }
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["YC_DISCOUNT"] = ProductYcDiscount * (decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString())) - decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString())));
                        ObjYcDiscount += decimal.Parse(dc.chkNull_0(dr["YC_DISCOUNT"].ToString()));
                    }
                    #endregion


                    #endregion
                    #endregion
                }
                else
                {
                    // BY M. IJAZ JAMIL AKHTAR

                    #region Free SKU Auto Promotion
                    if (IsAuto)
                    {
                        ArrayList ARlistOffer = or.GetPromotionOffers(Arpc, mCustomerVolClassId, mCustomerTypeId, dt, false);
                        for (int i = 0; i < ARlistOffer.Count; i++)
                        {
                            PromoOffers_Collection pofferCol = (PromoOffers_Collection)ARlistOffer[i];
                            if (pofferCol.Is_Claimable)
                            {
                                // Claimable
                                if (pofferCol.SKU_ID > 0)
                                {
                                    #region Slab on SKU
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        if (int.Parse(dr["SKU_ID"].ToString()) == pofferCol.SKU_ID)
                                        {
                                            if (pofferCol.Free_SKU_ID > 0)
                                            {
                                                ////Free SKU
                                                //if (ChbDiscount.Checked == true || pofferCol.Is_And != true)
                                                //{
                                                DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
                                                DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
                                                int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
                                                if (foundRows.Length > 0)
                                                {
                                                    #region Promotion
                                                    if (RowId < 0)
                                                    {
                                                        DataRow dr1 = dtFreeSKU.NewRow();
                                                        dr1["SKU_ID"] = foundRows[0]["SKU_ID"];
                                                        dr1["SKU_Code"] = foundRows[0]["SKU_CODE"];
                                                        dr1["SKU_Name"] = foundRows[0]["SKU_NAME"];
                                                        dr1["ORG_QUANTITY"] = 0;
                                                        if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                                        {
                                                            dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                            dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                            dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                            dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                            dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                            dr1["TST_AMOUNT"] = 0;
                                                        }
                                                        else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                        {
                                                            dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                            dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                            dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                            dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                            dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
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
                                                        //dr1["ORG_QUANTITY"] = 0;
                                                        dr1["Quantity"] = int.Parse(dr1["Quantity"].ToString()) + pofferCol.Quantity;
                                                        if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                                                        {

                                                            dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                            dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                            dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                            dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                            dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                            dr1["TST_AMOUNT"] = 0;

                                                        }
                                                        else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                        {
                                                            dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                            dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                            dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                            dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                            dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
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
                                                    #endregion
                                                }
                                                //}
                                            }
                                            this.Session.Add("dtFreeSKU", dtFreeSKU);
                                            this.LoadFreeGrid();
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region Slab on SKU Group
                                    //Free SKU
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        if (int.Parse(dr["SKU_ID"].ToString()) == pofferCol.SKU_ID)
                                        {
                                            if (pofferCol.Is_And != true)
                                            {
                                                if (pofferCol.Offer_Value > 0)
                                                {
                                                    dr["STANDARD_DISCOUNT"] = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString())) + pofferCol.Offer_Value;
                                                    dr["CLAIM_AMOUNT"] = decimal.Parse(dc.chkNull_0(dr["CLAIM_AMOUNT"].ToString())) + pofferCol.Offer_Value;
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
                                // Unclaimable
                                if (pofferCol.SKU_ID > 0)
                                {
                                    #region Slab on SKU
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        if (int.Parse(dr["SKU_ID"].ToString()) == pofferCol.SKU_ID)
                                        {
                                            if (pofferCol.Free_SKU_ID > 0)
                                            {
                                                //Free SKU
                                                DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
                                                DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
                                                int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
                                                if (foundRows.Length > 0)
                                                {
                                                    #region Promotion
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
                                                            dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                            dr1["TST_AMOUNT"] = 0;

                                                        }
                                                        else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                        {
                                                            dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                            dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                            dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                            dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                            dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
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
                                                            dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                            dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                            dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                            dr1["TST_AMOUNT"] = 0;
                                                        }
                                                        else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                        {
                                                            dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                            dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                            dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                            dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                            dr1["GST_AMOUNT"] = 0;
                                                        }
                                                        else
                                                        {
                                                            dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
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
                                                    #endregion
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
                                    #region Slab on SKU Group
                                    #region Free SKU
                                    if (pofferCol.Free_SKU_ID > 0)
                                    {
                                        DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
                                        DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + pofferCol.Free_SKU_ID.ToString() + "'");
                                        int RowId = FreeSKUExist(dtFreeSKU, pofferCol.Free_SKU_ID);
                                        if (foundRows.Length > 0)
                                        {
                                            #region Promotion
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
                                                    dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                    dr1["TST_AMOUNT"] = 0;

                                                }
                                                else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                {
                                                    dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                    dr1["Quantity"] = pofferCol.Quantity.ToString();
                                                    dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                    dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                    dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
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
                                                    dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                    dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                    dr1["GST_AMOUNT"] = (decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) / 100) * decimal.Parse(dr1["AMOUNT"].ToString());
                                                    dr1["TST_AMOUNT"] = 0;
                                                }
                                                else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                                                {
                                                    dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
                                                    dr1["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                                                    dr1["AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                    dr1["TST_AMOUNT"] = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString())) * decimal.Parse(pofferCol.Quantity.ToString());
                                                    dr1["GST_AMOUNT"] = 0;
                                                }
                                                else
                                                {
                                                    dr1["UNIT_PRICE"] = foundRows[0]["TRADE_PRICE"];
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
                                            #endregion
                                        }
                                        this.Session.Add("dtFreeSKU", dtFreeSKU);
                                        this.LoadFreeGrid();
                                    }
                                        #endregion
                                        foreach (DataRow dr in dt.Rows)
                                    {
                                        if (int.Parse(dr["SKU_ID"].ToString()) == pofferCol.SKU_ID)
                                        {
                                            if (pofferCol.Is_And != true)
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
                    }
                    #endregion

                    #region Reversecalulate Products
                    decimal ObjClaimAblePer = 0;
                    decimal ObjTaxableAmount = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        ObjGrossSales += decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                    }
                    if (decimal.Parse(dc.chkNull_0(numtxtUnClaimabledist.Text)) > 0)
                    {
                        ObjClaimAblePer = decimal.Parse(dc.chkNull_0(numtxtUnClaimabledist.Text)) / ObjGrossSales;
                    }
                    if (RbdiscountType.SelectedValue == "0")
                    {
                        if (decimal.Parse(dc.chkNull_0(txtDDPer.Text)) > 0)
                        {
                            ProductYcDiscount = decimal.Parse(dc.chkNull_0(txtDDPer.Text)) / 100;
                        }
                    }
                    else
                    {
                        if (decimal.Parse(dc.chkNull_0(numtxtYcDiscount.Text)) > 0)
                        {
                            ProductYcDiscount = decimal.Parse(dc.chkNull_0(numtxtYcDiscount.Text)) / (ObjGrossSales - ObjTaxableAmount);
                        }
                    }
                    ObjGrossSales = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        decimal TempAmount = 0;
                        decimal TempDiscount = 0;
                        decimal TempExtraDisAmt = 0;
                        decimal TempYcDiscount = 0;
                        decimal TempNetAmount = 0;

                        // Calculate Extra Tax By M. IJAZ JAMIL AKHTAR

                        #region GST Calculation
                        TempAmount = decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        TempDiscount = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString()));
                        TempExtraDisAmt = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString()));
                        TempYcDiscount = ProductYcDiscount * decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        TempNetAmount =
                                          TempAmount
                                        - TempDiscount
                                        - TempExtraDisAmt
                                        - TempYcDiscount;
                        //- decimal.Parse(dc.chkNull_0(numTxtTotalStndrdDiscnt.Text))
                        //- decimal.Parse(dc.chkNull_0(numtxtTotalExtraDiscnt.Text))
                        //- decimal.Parse(dc.chkNull_0(numtxtUnClaimabledist.Text))


                        #region EXTRA TAX
                        if (DistributorIsRegistered())
                        {
                            if (!CustomerIsRegistered())
                            {
                                if (dr["BATCH_NO"].ToString() == "T")
                                {
                                    decimal Discount = (TempNetAmount * decimal.Parse(dr["EXTRA_TAX_RATE"].ToString())) / 100;
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
                        dr["YC_DISCOUNT"] = ProductYcDiscount * decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        dr["SED_AMOUNT"] = ObjClaimAblePer * decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        dr["GST_AMOUNT"] = (TempNetAmount * decimal.Parse(dc.chkNull_0(dr["GST_RATE"].ToString()))) / 100;
                        dr["NET_AMOUNT"] = TempNetAmount + decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["EXTRA_TAX"].ToString()));
                        #endregion
                        ObjGrossSales += decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        ObjTotalGST += decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString()));
                        ObjTotalTST += decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));
                        ObjTotalSED += decimal.Parse(dc.chkNull_0(dr["SED_AMOUNT"].ToString()));
                        ObjTotalEXTRA_TAX += decimal.Parse(dc.chkNull_0(dr["EXTRA_TAX"].ToString()));
                        ObjStandardDiscount += decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString()));
                        ObjExtraDiscount += decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString()));
                    }
                    ObjYcDiscount = ProductYcDiscount * ObjGrossSales;
                    #endregion

                }
                #region Set Total Values
                txtGrossAmount.Text = Convert.ToString(Math.Round(ObjGrossSales, 2));
                numTxtTotalStndrdDiscnt.Text = Convert.ToString(Math.Round(ObjStandardDiscount, 2));
                //decimal StandardPer = (Math.Round(ObjStandardDiscount, 2) / Math.Round(ObjGrossSales, 2)) * 100;
                //txtDisPer.Text = Convert.ToString(Math.Round(StandardPer, 2));
                numtxtTotalExtraDiscnt.Text = Convert.ToString(Math.Round(ObjExtraDiscount, 2));
                //decimal ExtraPer = (Math.Round(ObjExtraDiscount, 2) / Math.Round(ObjGrossSales, 2)) * 100;
                //txtEXDPer.Text = Convert.ToString(Math.Round(ExtraPer, 2));
                numTxtTotalGST.Text = Convert.ToString(Math.Round((ObjTotalGST + ObjFreeGSTAmount), 2));
                //decimal DDPer = (Math.Round(ObjYcDiscount, 2) / Math.Round(ObjGrossSales, 2)) * 100;
                //txtDDPer.Text = Convert.ToString(Math.Round(DDPer, 2));
                numTxtTotalExtra_tax.Text = Convert.ToString(Math.Round((ObjTotalEXTRA_TAX), 2));
                numTxtTotalTST.Text = Convert.ToString(Math.Round((ObjTotalTST + ObjFreeTST), 2));

                //numTxtTotlAmnt.Text = Convert.ToString(Math.Round(ObjGrossSales - ObjExtraDiscount - ObjStandardDiscount + ObjTotalGST + ObjTotalEXTRA_TAX + ObjFreeGSTAmount + ObjTotalTST + ObjFreeTST - ObjFMR - ObjMR - ObjYcDiscount, 2));
                ObjNetSales = Math.Round(ObjGrossSales - ObjStandardDiscount - ObjExtraDiscount + ObjTotalGST + ObjFreeGSTAmount + ObjTotalTST + ObjTotalEXTRA_TAX + ObjFreeTST - ObjYcDiscount, 2);
                //-decimal.Parse(dc.chkNull_0(numTxtTotalStndrdDiscnt.Text)) - decimal.Parse(dc.chkNull_0(numtxtTotalExtraDiscnt.Text))
                numTxtTotlAmnt.Text = Convert.ToString(Math.Round(ObjNetSales, 2));

                numtxtUnClaimabledist.Text = Convert.ToString(Math.Round(ObjTotalSED, 2));
                numtxtYcDiscount.Text = Convert.ToString(Math.Round(ObjYcDiscount, 2));
                numTxtTotlFMR.Text = Convert.ToString(Math.Round(ObjFMR, 2));
                numTxtTotlMR.Text = Convert.ToString(Math.Round(ObjMR, 2));
                #endregion

                btnSaveOrder.Enabled = true;
                btnUpdateOrder.Enabled = true;
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
    }
    protected void drpcustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
        hfCustomerAreaID.Value = "";
        loadCustomerLedgerBalance();
        LoadSKUDetail();
        try
        {
            DataTable dtCustomer = (DataTable)this.Session["dtCustomer"];
            DataRow[] foundRows = dtCustomer.Select("CUSTOMER_ID  = '" + drpcustomer.SelectedValue.Trim() + "'");
            if (foundRows.Length > 0)
            {
                if ((int.Parse(foundRows[0]["IS_BLACKLISTED"].ToString())) == 1)
                {
                    lblBlacklistedCustomer.Text = "Black Listed Customer :" + foundRows[0]["BLAKLISTED_REMARKS"].ToString();
                    lblBlacklistedCustomer.Visible = true;
                    pnlBlacklist.BorderWidth = 1;
                }
                else
                {
                    lblBlacklistedCustomer.Text = "";
                    lblBlacklistedCustomer.Visible = false;
                    pnlBlacklist.BorderWidth = 0;
                }
                hfCustomerAreaID.Value = foundRows[0]["AREA_ID"].ToString();

            }
            RblPayMode_SelectedIndexChanged(null, null);
            DrpSkuDetail.Focus();
        }
        catch (Exception ex)
        {
            string ss = ex.StackTrace;
            lblBlacklistedCustomer.Text = "";
            lblBlacklistedCustomer.Visible = false;
            pnlBlacklist.BorderWidth = 0;
        }
    }

    protected void drpAssignedDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSKUDetail();
    }

    protected void GrdPurchase_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (txtDiscountType.Text != "Manual Discount")
        {
            e.Row.Cells[7].Visible = false;
            e.Row.Cells[8].Visible = false;
        }
        else
        {
            e.Row.Cells[7].Visible = true;
            e.Row.Cells[8].Visible = true;
        }
    }

    public void CustomerCreditLimt(decimal p_CurrentOrderAmount)
    {
        decimal Limit = 0;
        decimal SaleLimit = 0;
        decimal RemainingSaleLimit = 0;
        DataTable dtCustomer = _customerCtrl.GetCustomerByID(int.Parse(this.Session["DistributorId"].ToString()), Convert.ToInt64(drpcustomer.SelectedValue), 1);
        decimal CeditAmount = decimal.Parse(dtCustomer.Rows[0]["CREDIT_AMOUNT"].ToString());
        decimal CeditLimit = decimal.Parse(dtCustomer.Rows[0]["CREDITLIMIT"].ToString());
        lblCreditDays.Text = dtCustomer.Rows[0]["CREDIT_DAYS"].ToString() + " Days";
        if (CeditAmount >= 0)
        {
            Limit = CeditLimit + CeditAmount + p_CurrentOrderAmount;
            if (Limit > CeditLimit)
            {
                SaleLimit = Math.Round(Limit, 2);
                lblSaleLimit.Text = string.Format("{0:0,0.00}", SaleLimit);
            }
            else
            {
                SaleLimit = Math.Round(CeditLimit, 2);
                lblSaleLimit.Text = string.Format("{0:0,0.00}", SaleLimit);
            }
            RemainingSaleLimit = Math.Round(Limit, 2);
            lblRemainingSaleLimit.Text = string.Format("{0:0,0.00}", RemainingSaleLimit);
        }
        else
        {
            Limit = CeditLimit + CeditAmount + p_CurrentOrderAmount;
            if (Limit > CeditLimit)
            {
                SaleLimit = Math.Round(Limit, 2);
                lblSaleLimit.Text = string.Format("{0:0,0.00}", SaleLimit);
            }
            else
            {
                SaleLimit = Math.Round(CeditLimit, 2);
                lblSaleLimit.Text = string.Format("{0:0,0.00}", SaleLimit);
            }
            RemainingSaleLimit = Math.Round(Limit, 2);
            lblRemainingSaleLimit.Text = string.Format("{0:0,0.00}", RemainingSaleLimit);
            if (Limit > 0)
            {
                RemainingSaleLimit = Math.Round(Limit, 2);
                lblRemainingSaleLimit.Text = string.Format("{0:0,0.00}", RemainingSaleLimit);
            }
            else
            {
                lblRemainingSaleLimit.Text = "0.00";
            }
        }
    }
    private bool CreditLimit()
    {
        if (RblPayMode.SelectedValue == "215") // For Credit Invoices
        {
            if (!chkLimit.Checked)
            {
                decimal RemainingSaleLimit = decimal.Parse(_dc.chkNull_0(lblRemainingSaleLimit.Text));
                if (decimal.Parse(_dc.chkNull_0(numTxtTotlAmnt.Text)) > RemainingSaleLimit)
                {
                    return false;
                }
                return true;
            }
        }
        else
        {
            return true;
        }
        return true;
    }
    protected void RblPayMode_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (RblPayMode.SelectedValue == "215") // For Credit Invoices
        {
            if (btnSaveOrder.Text == "Save Invoice")
            {
                CreditPanel.Visible = true;
                CustomerCreditLimt(0);
            }
            else
            {
                CreditPanel.Visible = false;
            }
            //int OrderNumber = (int)this.Session["OrderNo"];
            //if (OrderNumber == -1)
            //{
            //    CreditPanel.Visible = true;
            //    CustomerCreditLimt(0);
            //}
            //else
            //{
            //    CreditPanel.Visible = false;
            //}
        }
        else
        {
            CreditPanel.Visible = false;
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        GridView gv = sender as GridView;
        GridViewRow Rows = (GridViewRow)(sender as LinkButton).NamingContainer;
        RowId = Convert.ToInt32(Rows.DataItemIndex.ToString());
        int stock = 0;
        stock += Convert.ToInt32(Rows.Cells[13].Text) * Convert.ToInt32(Rows.Cells[12].Text);
        stock += Convert.ToInt32(Rows.Cells[14].Text);
        DrpSkuDetail.SelectedValue = Rows.Cells[0].Text;
        DrpSkuDetail.Enabled = false;
        btnSave.Text = "Update SKU";
        txtQuantityCtn.Text = Rows.Cells[3].Text;
        txtQuantity.Text = Rows.Cells[4].Text;
        txtUnitRate.Text = Rows.Cells[5].Text;
        txtFreeSku.Text = Rows.Cells[6].Text;
        if (txtDiscountType.Text == "Auto Discount")
        {

        }
        else
        {
            int PackSize = int.Parse(Rows.Cells[12].Text);
            decimal StdDisc = decimal.Parse(Rows.Cells[7].Text);
            decimal ExtraDisc = decimal.Parse(Rows.Cells[8].Text);
            int TotalQty = int.Parse(Rows.Cells[4].Text) + (int.Parse(Rows.Cells[3].Text) * PackSize);
            int Qty = (Convert.ToInt32(Rows.Cells[3].Text) * Convert.ToInt32(Rows.Cells[12].Text)) + Convert.ToInt32(Rows.Cells[4].Text);
            int Ctn = Qty / Convert.ToInt32(Rows.Cells[12].Text);
            if (RbStdDiscType.SelectedValue == "0")
            {
                decimal Ddiscount = (StdDisc / (decimal.Parse(txtUnitRate.Text) * decimal.Parse(TotalQty.ToString()))) * 100;
                txtDistributorDiscount.Text = String.Format("{0:0.00}", Ddiscount);
            }
            else
            {
                decimal Ddiscount = decimal.Parse(Rows.Cells[7].Text);
                if (Session["ManualDiscounOn"].ToString() == "1")
                {
                    txtDistributorDiscount.Text = String.Format("{0:0.00}", Ddiscount / Ctn);
                }
                else if (Session["ManualDiscounOn"].ToString() == "2")
                {
                    txtDistributorDiscount.Text = String.Format("{0:0.00}", Ddiscount / Qty);
                }
                else
                {
                    txtDistributorDiscount.Text = String.Format("{0:0.00}", Ddiscount);
                }
            }
            if (RbExtraDiscType.SelectedValue == "0")
            {
                decimal Ediscount = (ExtraDisc / (decimal.Parse(txtUnitRate.Text) * decimal.Parse(TotalQty.ToString()))) * 100;
                txtExtradiscount.Text = String.Format("{0:0.00}", Ediscount);
            }
            else
            {
                decimal Ediscount = decimal.Parse(Rows.Cells[8].Text);
                if (Session["ManualDiscounOn"].ToString() == "1")
                {
                    txtExtradiscount.Text = String.Format("{0:0.00}", Ediscount / Ctn);
                }
                else if (Session["ManualDiscounOn"].ToString() == "2")
                {
                    txtExtradiscount.Text = String.Format("{0:0.00}", Ediscount / Qty);
                }
                else
                {
                    txtExtradiscount.Text = String.Format("{0:0.00}", Ediscount);
                }
            }
        }

        if (drpDocumentNo.SelectedValue == Constants.LongNullValue.ToString())
        {
        }
        else
        {
            try
            {
                int Stock = 0;
                txtStock.Text = "0-0";
                DataTable dtskuPrice = (DataTable)Session["Dtsku_Price"];
                DataRow[] foundRows = dtskuPrice.Select("SKU_ID  = '" + DrpSkuDetail.SelectedValue + "'");

                string[] strstock = foundRows[0]["Stock"].ToString().Split('-');
                Stock += Convert.ToInt32(strstock[0]) * Convert.ToInt32(Rows.Cells[12].Text);
                Stock += Convert.ToInt32(strstock[1]);
                Stock = Stock + stock;
                txtStock.Text = (Stock / Convert.ToInt32(Rows.Cells[12].Text)).ToString() + "-" + (Stock % Convert.ToInt32(Rows.Cells[12].Text)).ToString();
            }
            catch
            {
                txtStock.Text = "0-0";
            }
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        GridView gv = sender as GridView;
        GridViewRow Rows = (GridViewRow)(sender as LinkButton).NamingContainer;
        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
        PurchaseSKU.Rows.RemoveAt(Rows.DataItemIndex);
        this.Session.Add("PurchaseSKU", PurchaseSKU);
        this.LoadGird();
        btnCalculate.Enabled = true;
        btnSaveOrder.Enabled = false;
        btnUpdateOrder.Enabled = false;
    }
}