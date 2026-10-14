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
public partial class frmCustomerOrderDelivery : System.Web.UI.Page
{
    #region Variables

    DataTable PurchaseSKU;
    DataTable dtFreeSKU;
    private static int mCustomerTypeId;
    private static int mCustomerVolClassId;
    private int mTownId;
    private long mAreaId;
    private int mRouteId;

    private static int RowId;
 //   private static int UnitType;
    private static int OrderNo;
    UserController mUserController = new UserController();
    public int PrincipalWiseSale = 0;
    private static int mPrincipalId;

    #endregion

    /// <summary>
    /// Page_Load Function Populates All Combos, Grids And ListBox On The Page
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
            try
            {
                
                LoadDistributor();
                LoadAssingned();
                LoadPrincipal();
                LoadDeliveryman();
                this.LoadCustomerData();
                // this.LoadSKUDetail();
                this.CreatTable();
                this.LoadPromotion();
                this.CreateFreeSKU();
                this.LoadFreeGrid();
                if (Session["orderId"] != null)
                {
                    var sale_order_id = long.Parse(this.Session["orderId"].ToString());
                    this.LoadPendingOrder(sale_order_id);
                }
                if (Session["invoiceId"] != null)
                {
                    var sale_invoice_id = long.Parse(this.Session["invoiceId"].ToString());
                    this.LoadPendingInvoice(sale_invoice_id);
                }

                btnSave.Attributes.Add("onclick", "return ValidateForm();");

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
    private void LoadDistributor()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDropDownList(this.drpDistributor, dt, 0, 2, true);
    }
    private void LoadAssingned()
    {
        drpAssignedDistributor.Items.Clear();
        if (drpDistributor.Items.Count > 0)
        {
            UserController mUserController = new UserController();
            DataTable dt = mUserController.SelectDisToDisAssignment(int.Parse(drpDistributor.SelectedValue), Constants.IntNullValue, 1, int.Parse(this.Session["CompanyId"].ToString()));
            clsWebFormUtil.FillDropDownList(drpAssignedDistributor, dt, 0, 2);
        }

    }

    private void LoadPrincipal()
    {
        SKUPriceDetailController PController = new SKUPriceDetailController();
        DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, m_dt, 0, 1, true);
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
        this.Session.Add("PurchaseSKU", PurchaseSKU);
    }

    /// <summary>
    /// Creates Datatable For Free SKU
    /// </summary>
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

    /// <summary>
    /// Loads Pending Order Nos To Docuemnt No Combo
    /// </summary>
    private void LoadPendingOrder( long p_Sale_order_ID)
    {
        try
        {
            btnSaveOrder.Text = "Save Invoice";
            OrderEntryController or = new OrderEntryController();
            this.drpDocumentNo.Items.Clear();
            DataTable dtOrder = or.SelectPendingOrder(Constants.IntNullValue, Constants.IntNullValue, Constants .IntNullValue, Constants.IntNullValue, Constants .IntNullValue, Constants.Order_Pending_Id, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Convert.ToDateTime(this.Session["CurrentWorkDate"]),0,p_Sale_order_ID);
           // drpDocumentNo.Items.Add(new clsListItems("New", Constants.LongNullValue.ToString()));
            clsWebFormUtil.FillDropDownList(this.drpDocumentNo, dtOrder, 0, 0,false);
            this.Session.Add("dtOrder", dtOrder);

            DataTable dt = (DataTable)this.Session["dtOrder"];
            DataRow[] foundRows = dt.Select("SALE_ORDER_ID  = '" + p_Sale_order_ID.ToString().Trim() + "'");
            if (foundRows.Length > 0)
            {
                txtGrossAmount.Text = foundRows[0]["TOTAL_AMOUNT"].ToString();
                numtxtTotalExtraDiscnt.Text = foundRows[0]["EXTRA_DISCOUNT_AMOUNT"].ToString();
                numTxtTotalStndrdDiscnt.Text = foundRows[0]["STANDARD_DISCOUNT_AMOUNT"].ToString();
                numTxtTotalGST.Text = foundRows[0]["GST_AMOUNT"].ToString();
                numTxtTotlAmnt.Text = foundRows[0]["TOTAL_NET_AMOUNT"].ToString();
                numTxtTotalTST.Text = foundRows[0]["TST_AMOUNT"].ToString();
                DrpCustomer.SelectedValue = foundRows[0]["CUSTOMER_ID"].ToString();
                if (PrincipalWiseSale == 0)
                {
                    mPrincipalId = Constants.IntNullValue;
                }
                else
                {
                    DrpPrincipal.SelectedValue = foundRows[0]["PRINCIPAL_ID"].ToString();
                }
                
                txtBillBookNo.Text = foundRows[0]["MANUAL_ORDER_ID"].ToString();

                LoadPromotion();
                LoadSKUDetail();
                LoadCustomerOpBalance();
                numtxtTotalExtraDiscnt.Text = foundRows[0]["EXTRA_DISCOUNT_AMOUNT"].ToString();
                numtxtUnClaimabledist.Text = foundRows[0]["SED_AMOUNT"].ToString();
                txtMKSDiscount.Text = foundRows[0]["YC_DISCOUNT"].ToString();

                try
                {
                    drpAssignedDistributor.SelectedValue = foundRows[0]["ASSIGNED_DISTRIBUTOR_ID"].ToString();
                }
                catch (Exception ex)
                { }

                txtRemarks.Text = foundRows[0]["REMARKS"].ToString();
                txtDriver.Text = foundRows[0]["DRIVER_NAME"].ToString();
                txtTruckno.Text = foundRows[0]["TRUCK_NO"].ToString();
                this.Session.Add("CUSTOMER_ID", long.Parse(foundRows[0]["CUSTOMER_ID"].ToString()));
                this.ExistenOrderDetail(long.Parse(drpDocumentNo.SelectedValue.ToString()));

                this.Session.Add("hfBillBookNo", foundRows[0]["MANUAL_ORDER_ID"].ToString());
                EnableDisableController(false);
                
                btnSaveOrder.Enabled = false;
                btnCalculate.Enabled = false;
                this.ClearAll();
                ScriptManager.GetCurrent(Page).SetFocus(DrpSkuDetail);
                // no Order Type saved on order.. 
                // RblPayMode.SelectedValue = foundRows[0]["ORDER_TYPE_ID"].ToString();
            }


        }
        catch(Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Some Error in Data ! Please Contact Admin')", true);

            ExceptionPublisher.PublishException(ex);
        }
    }

    private void LoadPendingInvoice(long p_Sale_Invoice_ID)
    {
        try
        {
            btnSaveOrder.Text = "Update Invoice";
            OrderEntryController or = new OrderEntryController();
            this.drpDocumentNo.Items.Clear();
            DataTable dtOrder = or.SelectPendingOrder(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.Order_Pending_Id, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Convert.ToDateTime(this.Session["CurrentWorkDate"]), 1, p_Sale_Invoice_ID);
            // drpDocumentNo.Items.Add(new clsListItems("New", Constants.LongNullValue.ToString()));
            clsWebFormUtil.FillDropDownList(this.drpDocumentNo, dtOrder, 0, 0, false);
            this.Session.Add("dtOrder", dtOrder);

            DataTable dt = (DataTable)this.Session["dtOrder"];
            DataRow[] foundRows = dt.Select("SALE_INVOICE_ID  = '" + p_Sale_Invoice_ID.ToString().Trim() + "'");
            if (foundRows.Length > 0)
            {
                txtGrossAmount.Text = foundRows[0]["TOTAL_AMOUNT"].ToString();
                numtxtTotalExtraDiscnt.Text = foundRows[0]["EXTRA_DISCOUNT_AMOUNT"].ToString();
                numTxtTotalStndrdDiscnt.Text = foundRows[0]["STANDARD_DISCOUNT_AMOUNT"].ToString();
                numTxtTotalGST.Text = foundRows[0]["GST_AMOUNT"].ToString();
                numTxtTotlAmnt.Text = foundRows[0]["TOTAL_NET_AMOUNT"].ToString();
                numTxtTotalTST.Text = foundRows[0]["TST_AMOUNT"].ToString();
                DrpCustomer.SelectedValue = foundRows[0]["CUSTOMER_ID"].ToString();
                if (PrincipalWiseSale == 0)
                {
                    mPrincipalId = Constants.IntNullValue;
                }
                else
                {
                    DrpPrincipal.SelectedValue = foundRows[0]["PRINCIPAL_ID"].ToString();
                }
                
                try
                {
                    drpDeliveryman.SelectedValue = foundRows[0]["DELIVERYMAN_ID"].ToString();
                    drpAssignedDistributor.SelectedValue = foundRows[0]["ASSIGNED_DISTRIBUTOR_ID"].ToString();
                    txtBillBookNo.Text = foundRows[0]["MANUAL_ORDER_ID"].ToString();
                }
                catch (Exception ex)
                { }
               
                LoadPromotion();
                LoadSKUDetail();
                LoadCustomerOpBalance();
                numtxtTotalExtraDiscnt.Text = foundRows[0]["EXTRA_DISCOUNT_AMOUNT"].ToString();
                numtxtUnClaimabledist.Text = foundRows[0]["SED_AMOUNT"].ToString();
                txtMKSDiscount.Text = foundRows[0]["YC_DISCOUNT"].ToString();



                txtRemarks.Text = foundRows[0]["REMARKS"].ToString();
                txtDriver.Text = foundRows[0]["DRIVER_NAME"].ToString();
                txtTruckno.Text = foundRows[0]["TRUCK_NO"].ToString();
                this.Session.Add("CUSTOMER_ID", long.Parse(foundRows[0]["CUSTOMER_ID"].ToString()));
                this.ExistenInvoiceDetail(long.Parse(drpDocumentNo.SelectedValue.ToString()));

                this.Session.Add("hfBillBookNo", foundRows[0]["MANUAL_ORDER_ID"].ToString());
                EnableDisableController(false);

                btnSaveOrder.Enabled = false;
                btnCalculate.Enabled = false;
                this.ClearAll();
                ScriptManager.GetCurrent(Page).SetFocus(DrpSkuDetail);
                RblPayMode.SelectedValue = foundRows[0]["ORDER_TYPE_ID"].ToString();
            }


        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Some Error in Data ! Please Contact Admin')", true);

            ExceptionPublisher.PublishException(ex);
        }
    }

   
    private void LoadCustomerData()
    {
        try
        {
            if (drpDistributor.Items.Count > 0 )
            {
                ListCustomer.Items.Clear();
                CustomerDataController mController = new CustomerDataController();
                DataTable dtCustomer = mController.SelectPrincipalCustomer(int.Parse(drpDistributor.SelectedValue), int.Parse(this.Session["UserID"].ToString()), Constants.IntNullValue, Constants.IntNullValue,3);
                clsWebFormUtil.FillDropDownList(this.DrpCustomer, dtCustomer, "CUSTOMER_ID", "CUSTOMER_DETAIL", true);
                this.Session.Add("dtCustomer", dtCustomer);
            }
            else
            {
                ListCustomer.Items.Clear();
            }
        }
        catch (Exception ex)
        {
            ExceptionPublisher.PublishException(ex);
        }
    }

    /// <summary>
    /// Loads SKU Data To SKU ListBox
    /// </summary>
    private void LoadSKUDetail()
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
            SKUPriceDetailController PController = new SKUPriceDetailController();
            //if (DrpPrincipal.Items .Count  > 0)
            //{
                DataTable Dtsku_Price = PController.SelectDataPrice(mPrincipalId, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(drpDistributor.SelectedValue), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 1, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
                clsWebFormUtil.FillDropDownList(DrpSkuDetail, Dtsku_Price, 0, 20, true);
                this.Session.Add("Dtsku_Price", Dtsku_Price);
            //}
        }
        catch (Exception ex)
        {
            ExceptionPublisher.PublishException(ex);

        }
    }

    /// <summary>
    /// Enables/Disables Discount Fields For Manual And Auto Promotion And Loads Promotion Controler For Auto Promotion
    /// </summary>
    private void LoadPromotion()
    {try
        {
            if (PrincipalWiseSale == 0)
            {
                mPrincipalId = Constants.IntNullValue;
            }
            else
            {
                mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
            }
            //if (DrpPrincipal.Items.Count >0)
            //{
            SKUPriceDetailController PController = new SKUPriceDetailController();
                DataTable dt = PController.SelectDataPrice(mPrincipalId, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
                if (bool.Parse(dt.Rows[0]["Is_ManualDiscount"].ToString()) == false)
                {
                    OrderEntryController orderController = new OrderEntryController();
                    PromotionCollections_Controller Arpc = orderController.LoadSchemes(int.Parse(drpDistributor.SelectedValue), mPrincipalId, Convert.ToDateTime(this.Session["CurrentWorkDate"]));
                    this.Session.Add("Arpc", Arpc);
                    txtDiscountType.Text = "Auto Discount";
                    numtxtUnClaimabledist.ReadOnly = true;
                    ChbDiscount.Checked = true;
                }
                else
                {
                    txtDiscountType.Text = "Manual Discount";
                    numTxtTotalStndrdDiscnt.ReadOnly = false;
                    numtxtTotalExtraDiscnt.ReadOnly = false;
                    numtxtUnClaimabledist.ReadOnly = false;
                    ChbDiscount.Checked = false;
                }
            //}
        }
        catch(Exception ex)
        {
            ExceptionPublisher.PublishException(ex);
        }
    }

    /// <summary>
    /// Loads Order To Order Grid
    /// </summary>
    private void LoadGird()
    {
        try
        {
            PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
            GrdPurchase.DataSource = PurchaseSKU;
            GrdPurchase.DataBind();
        }
        catch(Exception ex)
        {
            ExceptionPublisher.PublishException(ex);
        }
    }

    /// <summary>
    /// Sets Order SKU For Edit. This Function Runs When An Existing Order SKU Needs To Be Edited
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void GrdPurchase_RowEditing(object sender, GridViewEditEventArgs e)
    {
        RowId = e.NewEditIndex;

        DrpSkuDetail.SelectedValue = GrdPurchase.Rows[e.NewEditIndex].Cells[0].Text;
       // txtskuName.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[2].Text;
        txtQuantityCtn.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[3].Text;
        txtQuantity.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[4].Text;
        txtUnitRate.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[5].Text;
        //txtBatchNo.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[6].Text;
      
        
        DrpSkuDetail.Enabled = false;
        btnSave.Text = "Update SKU";

    }

    /// <summary>
    /// Deletes An SKU From Order Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void GrdPurchase_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
        PurchaseSKU.Rows.RemoveAt(e.RowIndex);
        this.Session.Add("PurchaseSKU", PurchaseSKU);
        this.LoadGird();
        btnCalculate.Enabled = true;
        btnSaveOrder.Enabled = false;
    }
    
    /// <summary>
    /// Loads Free SKUS To SKU Grid
    /// </summary>
    private void LoadFreeGrid()
    {
        dtFreeSKU = (DataTable)this.Session["dtFreeSKU"];
        GrdFreeSKU.DataSource = dtFreeSKU;
        GrdFreeSKU.DataBind();
    }

    /// <summary>
    /// Enables/Disables Batch No TextBox
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
   

    /// <summary>
    /// Verifies Customer Code
    /// </summary>
    /// <returns>True On Success And False On Failure</returns>
    private bool FindCustomer()
    {
        DataTable dtCustomer = (DataTable)this.Session["dtCustomer"];
        DataRow[] foundRows = dtCustomer.Select("CUSTOMER_ID = '" + DrpCustomer.SelectedValue.Trim() + "'");
        if (foundRows.Length > 0)
        {
            this.Session.Add("CUSTOMER_ID", long.Parse(foundRows[0]["CUSTOMER_ID"].ToString()));
            mCustomerTypeId = int.Parse(foundRows[0]["CHANNEL_TYPE_ID"].ToString());
            mCustomerVolClassId = int.Parse(foundRows[0]["VOLUME_CLASS_ID"].ToString());
            mTownId = int.Parse(foundRows[0]["TOWN_ID"].ToString());
            mAreaId = long.Parse(foundRows[0]["AREA_ID"].ToString());
            mRouteId = int.Parse(foundRows[0]["ROUTE_ID"].ToString());
            return true;
        }
        return false;
    }

    private void LoadDeliveryman()
    {
        if (drpDistributor.Items.Count > 0)
        {
            SaleForceController mDController = new SaleForceController();
            DataTable m_dt = mDController.SelectSaleForceAssignedArea(int.Parse(drpDistributor.SelectedValue.ToString()), Constants.IntNullValue, int.Parse(this.Session["CompanyId"].ToString()));
            clsWebFormUtil.FillDropDownList(this.drpDeliveryman, m_dt, 0, 3, true);
        }
        else
        {
            drpDeliveryman.Items.Clear();
        }
    }

    /// <summary>
    /// Checks SKU in Order Grid
    /// </summary>
    /// <returns>True On Success And False On Failure</returns>
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

    /// <summary>
    /// Checks Free SKU in Free SKU Grid
    /// </summary>
    /// <returns>True On Success And False On Failure</returns>
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

    /// <summary>
    /// Loads Order And Free SKUS Grids
    /// </summary>
    /// <param name="OrderId">Order</param>
    private void ExistenOrderDetail(long OrderId)
    {
        OrderEntryController ord = new OrderEntryController();
        PurchaseSKU = ord.SelectOrderDetail(int.Parse(drpDistributor.SelectedValue), OrderId);
        dtFreeSKU = ord.SelectOrderPromotion(int.Parse(drpDistributor.SelectedValue), OrderId);
        this.Session.Add("PurchaseSKU", PurchaseSKU);
        this.Session.Add("dtFreeSKU", dtFreeSKU);
        this.LoadGird();
        this.LoadFreeGrid();
    }

    private void ExistenInvoiceDetail(long sale_invoice_Id)
    {
        OrderEntryController ord = new OrderEntryController();
        PurchaseSKU = ord.SelectInvoiceDetail(int.Parse(drpDistributor.SelectedValue), sale_invoice_Id);
        dtFreeSKU = ord.SelectInvoicePromotion(int.Parse(drpDistributor.SelectedValue), sale_invoice_Id);
        this.Session.Add("PurchaseSKU", PurchaseSKU);
        this.Session.Add("dtFreeSKU", dtFreeSKU);
        this.LoadGird();
        this.LoadFreeGrid();
    }

    /// <summary>
    /// Checks Bill Book No in System
    /// </summary>
    /// <returns>True On Success And False On Failure</returns>
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
    
    /// <summary>
    /// Enables/Disables Controls
    /// </summary>
    /// <param name="CValue">Value</param>
    private void EnableDisableController(bool CValue)
    {

        if (CValue == true)
        {
            if (decimal.Parse(txtGrossAmount.Text) > 0)
            {
                DrpCustomer.Enabled = true;
                drpDistributor.Enabled = true;
                DrpPrincipal.Enabled = true;

            }
        }
        else
        {
            DrpCustomer.Enabled = false;
            drpDistributor.Enabled = false;
            DrpPrincipal.Enabled = false;

        }
    }

    /// <summary>
    /// Clears Session Variables And Redircts To Step1
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
       

    }

    /// <summary>
    /// Calcualtes Discount And Free SKU For Order
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnCalculate_Click(object sender, EventArgs e)
    {
        if (FindCustomer())
        {
            decimal ObjGrossSales = 0;
            decimal ObjStandardDiscount = 0;
            decimal ObjExtraDiscount = 0;
            decimal ObjTotalGST = 0;
            decimal ObjTotalSED = 0;
            decimal ObjTotalTST = 0;
            decimal ObjFreeGSTAmount = 0;
            decimal ObjFreeGrossAmt = 0;
            decimal ObjFreeTST = 0;
            decimal ProductExtDist = 0;
            decimal ProductStdDist = 0;
            decimal ObjFMR = 0;
            decimal ObjMR = 0;

            decimal ObjYcDiscount = 0;
            decimal ProductYcDiscount = 0;
            DataControl dc = new DataControl();
            OrderEntryController or = new OrderEntryController();
            SKUGroupController GroupCtl = new SKUGroupController();
            DataTable dt = (DataTable)this.Session["PurchaseSKU"];
            foreach (DataRow dr in dt.Rows)
            {
                dr["STANDARD_DISCOUNT"] = 0;
                dr["YC_DISCOUNT"] = 0;
                dr["STANDARD_DISCOUNT_PER"] = 0;
                dr["EXTRA_DISCOUNT"] = 0;
                dr["CLAIM_EXTRA_AMOUNT"] = 0;
                dr["CLAIM_STANDARD_DISCOUNT"] = 0;
                dr["CLAIM_PER"] = 0;
                dr["CLAIM_AMOUNT"] = 0;

            }
            PromotionCollections_Controller Arpc = (PromotionCollections_Controller)this.Session["Arpc"];
            this.CreateFreeSKU();
            this.LoadFreeGrid();

            if (dt.Rows.Count > 0 && DrpCustomer.Items .Count > 0)
            {
                if (ChbDiscount.Checked == true)
                {
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

                        dr["GST_AMOUNT"] = (TempAmount - TempDiscount ) * decimal.Parse(dc.chkNull_0(dr["GST_RATE"].ToString())) / 100;
                        //
                        decimal TempGST_AMOUNT = decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString()));
                        decimal TempTST_AMOUNT = decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));
                        dr["MR"] = (TempAmount - TempDiscount + TempGST_AMOUNT + TempTST_AMOUNT) * decimal.Parse(dc.chkNull_0(dr["MR_RATE"].ToString())) / 100;
                        decimal TempMRAmt = decimal.Parse(dc.chkNull_0(dr["MR"].ToString()));

                        //
                        ObjFMR += decimal.Parse(dc.chkNull_0(dr["FMR"].ToString()));
                        ObjMR += decimal.Parse(dc.chkNull_0(dr["MR"].ToString()));
                        dr["NET_AMOUNT"] = (TempAmount - TempDiscount - TempExtraDisAmt - TempFMRAmt - TempMRAmt - TempYCDisAmt) + decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));
                        ObjTotalGST += decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString()));
                        ObjTotalTST += decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));
                        #endregion
                    }
                    //Reverse Calculation for Extra Discount
                    //if (decimal.Parse(dc.chkNull_0(numtxtYcDiscount.Text)) > 0)
                    //{
                    //    ProductYcDiscount = decimal.Parse(dc.chkNull_0(numtxtYcDiscount.Text)) / (ObjGrossSales - ObjStandardDiscount);
                    //}
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["YC_DISCOUNT"] = ProductYcDiscount * (decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString())) - decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString())));
                        ObjYcDiscount += decimal.Parse(dc.chkNull_0(dr["YC_DISCOUNT"].ToString()));
                    }
                    #endregion
                }
                else
                {
                    #region Reversecalulate Products

                    //decimal ObjGrossSales = 0;
                    decimal ObjTaxableAmount = 0;
                    decimal ObjClaimAblePer = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        ObjGrossSales += decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));

                    }
                    if (decimal.Parse(dc.chkNull_0(numtxtUnClaimabledist.Text)) > 0)
                    {
                        ObjClaimAblePer = decimal.Parse(dc.chkNull_0(numtxtUnClaimabledist.Text)) / ObjGrossSales;
                    }
                    if (decimal.Parse(dc.chkNull_0(numTxtTotalStndrdDiscnt.Text)) > 0)
                    {
                        ProductStdDist = decimal.Parse(dc.chkNull_0(numTxtTotalStndrdDiscnt.Text)) / (ObjGrossSales - ObjTaxableAmount);
                    }
                    if (decimal.Parse(dc.chkNull_0(numtxtTotalExtraDiscnt.Text)) > 0)
                    {
                        ProductExtDist = decimal.Parse(dc.chkNull_0(numtxtTotalExtraDiscnt.Text)) / (ObjGrossSales - ObjTaxableAmount);
                    }


                    //if (decimal.Parse(dc.chkNull_0(numtxtYcDiscount.Text)) > 0)
                    //{
                    //    ProductYcDiscount = decimal.Parse(dc.chkNull_0(numtxtYcDiscount.Text)) / (ObjGrossSales - ObjTaxableAmount);
                    //}

                    ObjGrossSales = 0;

                    foreach (DataRow dr in dt.Rows)
                    {

                        decimal TempAmount = 0;
                        decimal TempDiscount = 0;
                        decimal TempExtraDisAmt = 0;
                        decimal TempYcDisAmt = 0;
                        decimal TempFMrAmt = 0;
                        decimal TempMRAmt = 0;


                        dr["EXTRA_DISCOUNT"] = ProductExtDist * decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        dr["STANDARD_DISCOUNT"] = ProductStdDist * decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));

                        dr["SED_AMOUNT"] = ObjClaimAblePer * decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        dr["YC_DISCOUNT"] = ProductYcDiscount * decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));

                        #region GST Calculation

                        TempAmount = decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        TempDiscount = decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString()));
                        TempExtraDisAmt = decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString()));
                        TempYcDisAmt = decimal.Parse(dc.chkNull_0(dr["YC_DISCOUNT"].ToString()));

                        TempFMrAmt = decimal.Parse(dc.chkNull_0(dr["FMR"].ToString()));
                        
                        dr["GST_AMOUNT"] = (TempAmount - TempDiscount) * decimal.Parse(dc.chkNull_0(dr["GST_RATE"].ToString())) / 100;
                        //
                        decimal TempGST_AMOUNT = decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString()));
                        decimal TempTST_AMOUNT = decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));
                        dr["MR"] = (TempAmount - TempDiscount + TempGST_AMOUNT + TempTST_AMOUNT) * decimal.Parse(dc.chkNull_0(dr["MR_RATE"].ToString())) / 100;
                         TempMRAmt = decimal.Parse(dc.chkNull_0(dr["MR"].ToString()));


                        //
                        dr["NET_AMOUNT"] = (TempAmount - TempDiscount - TempExtraDisAmt-TempYcDisAmt-TempFMrAmt-TempMRAmt) + decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString())) + decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));

                        ObjGrossSales += decimal.Parse(dc.chkNull_0(dr["AMOUNT"].ToString()));
                        ObjTotalGST += decimal.Parse(dc.chkNull_0(dr["GST_AMOUNT"].ToString()));
                        ObjTotalTST += decimal.Parse(dc.chkNull_0(dr["TST_AMOUNT"].ToString()));
                        ObjTotalSED += decimal.Parse(dc.chkNull_0(dr["SED_AMOUNT"].ToString()));

                        ObjFMR += decimal.Parse(dc.chkNull_0(dr["FMR"].ToString()));
                        ObjMR += decimal.Parse(dc.chkNull_0(dr["MR"].ToString()));
                        ObjStandardDiscount += decimal.Parse(dc.chkNull_0(dr["STANDARD_DISCOUNT"].ToString()));
                        ObjExtraDiscount += decimal.Parse(dc.chkNull_0(dr["EXTRA_DISCOUNT"].ToString()));
                        ObjYcDiscount += decimal.Parse(dc.chkNull_0(dr["YC_DISCOUNT"].ToString()));

                        #endregion
                    }
                    #endregion
                }

                #region Set Total Values

                txtGrossAmount.Text = Convert.ToString(Math.Round(ObjGrossSales, 4));
                numtxtTotalExtraDiscnt.Text = Convert.ToString(Math.Round(ObjExtraDiscount, 4));
                numTxtTotalStndrdDiscnt.Text = Convert.ToString(Math.Round(ObjStandardDiscount, 4));
                numTxtTotalGST.Text = Convert.ToString(Math.Round((ObjTotalGST + ObjFreeGSTAmount), 4));
                numTxtTotalTST.Text = Convert.ToString(Math.Round((ObjTotalTST + ObjFreeTST), 4));
                //numTxtTotlAmnt.Text = Convert.ToString(Math.Round(ObjGrossSales - ObjExtraDiscount - ObjStandardDiscount + ObjTotalGST + ObjFreeGSTAmount + ObjTotalTST, 2));
                numTxtTotlAmnt.Text = Convert.ToString(Math.Round(ObjGrossSales - ObjExtraDiscount - ObjStandardDiscount + ObjTotalGST + ObjFreeGSTAmount + ObjTotalTST + ObjFreeTST - ObjFMR - ObjMR - ObjYcDiscount, 4));
                numtxtUnClaimabledist.Text = Convert.ToString(Math.Round(ObjTotalSED, 4));

                
                #endregion

                btnSaveOrder.Enabled = true;
             
            }
        }
       
    }

    /// <summary>
    /// Adds/Updates Row To Order Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnSave_Click(object sender, EventArgs e)
    {

        DataControl dc = new DataControl();
        DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
        DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" +DrpSkuDetail.SelectedValue + "'");
        decimal mTradePrice = decimal.Parse(dc.chkNull_0(foundRows[0]["TRADE_PRICE"].ToString()));

        int mPackSize = int.Parse(dc.chkNull_0(foundRows[0]["UNITS_IN_CASE"].ToString()));

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

                dr["MR_RATE"] = 0;
                dr["MR"] = 0;
                dr["FMR"] = 0;
                dr["STANDARD_DISCOUNT"] = 0;
                dr["EXTRA_DISCOUNT"] = 0;
                dr["YC_DISCOUNT"] = 0;
                dr["GST_AMOUNT"] = 0;
                dr["NET_AMOUNT"] = 0;
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
           

            dr["QUANTITY_UNIT"] = int.Parse(dc.chkNull_0(txtQuantity.Text));

            dr["QUANTITY_CTN"] = int.Parse(dc.chkNull_0(txtQuantityCtn.Text));


            dr["QUANTITY"] = (decimal.Parse(dr["QUANTITY_UNIT"].ToString()) + (decimal.Parse(dr["QUANTITY_CTN"].ToString()) * mPackSize));

            dr["UNIT_PRICE"] = mTradePrice.ToString();
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
            
            dr["MR_RATE"] = 0;
            dr["MR"] = 0;
            dr["FMR"] = 0;
            dr["STANDARD_DISCOUNT"] = 0;
            dr["EXTRA_DISCOUNT"] = 0;
            dr["YC_DISCOUNT"] = 0;
            dr["GST_AMOUNT"] = 0;
            dr["NET_AMOUNT"] = 0;
            dr["STANDARD_DISCOUNT_PER"] = 0;
        }

        this.Session.Add("PurchaseSKU", PurchaseSKU);
        this.EnableDisableController(false);
        this.LoadGird();
        this.ClearAll();
        ScriptManager.GetCurrent(Page).SetFocus(DrpSkuDetail);
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
                string ManualID = null;
                if (txtBillBookNo.Text.Trim().Length > 0)
                {
                    ManualID = txtBillBookNo.Text.ToUpper();
                }
                DataControl DC = new DataControl();
                PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
                dtFreeSKU = (DataTable)this.Session["dtFreeSKU"];
                OrderEntryController mOrderController = new OrderEntryController();

                if (PrincipalWiseSale == 0)
                {
                    mPrincipalId = 0;
                }
                else
                {
                    mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
                }


                if (btnSaveOrder.Text == "Save Invoice")
                {
                    //if (int.Parse(RblPayMode.SelectedValue.ToString()) == Constants.Credit_Order_Id || int.Parse(RblPayMode.SelectedValue.ToString()) == Constants.Advance_PaymentOrder_id)
                    //{
                    //    CustomerDataController CDC = new CustomerDataController();
                    //    DataTable dt = CDC.SelectCustomerCreditBalance(long.Parse(DrpCustomer.SelectedValue), int.Parse(DrpPrincipal.SelectedValue), int.Parse(drpDistributor.SelectedValue), int.Parse(RblPayMode.SelectedValue.ToString()));
                    //    decimal NetCashSale = decimal.Parse(numTxtTotlAmnt.Text);

                    //    if (decimal.Parse(DC.chkNull_0(dt.Rows[0][0].ToString())) <= NetCashSale)
                    //    {
                    //        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Customer Credit Limit/Advance Amount is " + DC.chkNull_0(dt.Rows[0][0].ToString()) + "');", true);
                    //        return;
                    //    }

                    //}

                    foreach (DataRow dr in PurchaseSKU.Rows)
                    {
                        PhaysicalStockController mController = new PhaysicalStockController();
                        DataTable dtstock = mController.SelectSKUClosingStock(int.Parse(drpAssignedDistributor.SelectedValue), int.Parse(dr["SKU_ID"].ToString()), "N/A", DateTime.Parse(this.Session["CurrentWorkDate"].ToString()),16);
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
                    bool IsValidInsert = mOrderController.Add_Invoice(int.Parse(drpDistributor.SelectedValue), ManualID, mTownId, mAreaId, mPrincipalId, long.Parse(DrpCustomer.SelectedValue), long.Parse(DrpCustomer.SelectedValue), -1, int.Parse(drpDeliveryman.SelectedValue), long.Parse(drpDocumentNo.SelectedValue.ToString()),
                    decimal.Parse(DC.chkNull_0(txtGrossAmount.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalExtraDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalStndrdDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalGST.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlAmnt.Text)), 0, int.Parse(RblPayMode.SelectedValue.ToString()),
                    PurchaseSKU, dtFreeSKU, int.Parse(this.Session["UserId"].ToString()),0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), decimal.Parse(DC.chkNull_0(numTxtTotalTST.Text)), decimal.Parse(DC.chkNull_0(numtxtUnClaimabledist.Text)), decimal.Parse(DC.chkNull_0(txtMKSDiscount.Text)), 0, 0, txtTruckno.Text, txtDriver.Text,txtRemarks.Text, int.Parse(drpAssignedDistributor.SelectedValue));
                    if (IsValidInsert)
                    {
                        this.ClearMasterALL();
                        Response.Redirect("frmSaleOrderView.aspx?LevelID=156&LevelType=3");

                    }
                }

                else if (btnSaveOrder.Text == "Update Invoice")
                {

                    //if (int.Parse(RblPayMode.SelectedValue.ToString()) == Constants.Credit_Order_Id || int.Parse(RblPayMode.SelectedValue.ToString()) == Constants.Advance_PaymentOrder_id)
                    //{
                    //    CustomerDataController CDC = new CustomerDataController();
                    //    DataTable dt = CDC.SelectCustomerCreditBalance(long.Parse(DrpCustomer.SelectedValue), int.Parse(DrpPrincipal.SelectedValue), int.Parse(drpDistributor.SelectedValue), int.Parse(RblPayMode.SelectedValue.ToString()));
                    //    decimal NetCashSale = decimal.Parse(numTxtTotlAmnt.Text);

                    //    if (decimal.Parse(DC.chkNull_0(dt.Rows[0][0].ToString())) <= NetCashSale)
                    //    {
                    //        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Customer Credit Limit/Advance Amount is " + DC.chkNull_0(dt.Rows[0][0].ToString()) + "');", true);
                    //        return;
                    //    }

                    //}

                    foreach (DataRow dr in PurchaseSKU.Rows)
                    {
                        PhaysicalStockController mController = new PhaysicalStockController();
                        DataTable dtstock = mController.SelectSKUClosingStock(int.Parse(drpAssignedDistributor.SelectedValue), int.Parse(dr["SKU_ID"].ToString()), "N/A", DateTime.Parse(this.Session["CurrentWorkDate"].ToString()),16);
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
                    bool IsValidInsert = mOrderController.Update_Invoice(long.Parse(drpDocumentNo.SelectedValue), int.Parse(drpDistributor.SelectedValue), ManualID, mTownId, mAreaId, mPrincipalId, long.Parse(DrpCustomer.SelectedValue), long.Parse(DrpCustomer.SelectedValue), -1, int.Parse(drpDeliveryman.SelectedValue), long.Parse(drpDocumentNo.SelectedValue.ToString()),
                    decimal.Parse(DC.chkNull_0(txtGrossAmount.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalExtraDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalStndrdDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalGST.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlAmnt.Text)), 0, int.Parse(RblPayMode.SelectedValue.ToString()),
                    PurchaseSKU, dtFreeSKU, int.Parse(this.Session["UserId"].ToString()), 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), decimal.Parse(DC.chkNull_0(numTxtTotalTST.Text)), decimal.Parse(DC.chkNull_0(numtxtUnClaimabledist.Text)), decimal.Parse(DC.chkNull_0(txtMKSDiscount.Text)), 0, 0, txtTruckno.Text, txtDriver.Text, txtRemarks.Text, int.Parse(drpAssignedDistributor.SelectedValue));
                    if (IsValidInsert)
                    {
                        this.ClearMasterALL();
                        Response.Redirect("frmSaleOrderView.aspx?LevelID=156&LevelType=3");

                    }
                }


                #region old
                //if (btnSaveOrder.Text == "Save Order")
                //{
                //    if (!IsBillBookNoExist())
                //    {
                //        bool IsValidInsert = mOrderController.Add_Order(int.Parse(drpDistributor.SelectedValue), null, mTownId, mAreaId, int.Parse(DrpPrincipal.SelectedValue), long.Parse(DrpCustomer.SelectedValue), long.Parse(DrpCustomer.SelectedValue), -1, -1, int.Parse(RblPayMode.SelectedValue.ToString()),
                //        decimal.Parse(DC.chkNull_0(txtGrossAmount.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalExtraDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalStndrdDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalGST.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlAmnt.Text)), 0, Constants.Order_Pending_Id, PurchaseSKU, dtFreeSKU, int.Parse(this.Session["UserId"].ToString()), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), decimal.Parse(DC.chkNull_0(numtxtUnClaimabledist.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalTST.Text)), Constants.DecimalNullValue, Constants.DecimalNullValue, Constants.DecimalNullValue, null, txtRemarks.Text);
                //        if (IsValidInsert)
                //        {
                //            this.LoadPendingOrder();
                //            this.ClearMasterALL();
                //            this.Session.Remove("hfBillBookNo");
                //        }
                //    }
                //    else
                //    {
                //        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('This Bill Book No already exist,Kindly enter different Bill Book No');", true);
                //    }
                //}
                //else if (btnSaveOrder.Text == "Update Order")
                //{
                //    bool IsValidInsert = mOrderController.Update_Order(long.Parse(drpDocumentNo.SelectedValue),int.Parse(drpDistributor.SelectedValue), null, mTownId, mAreaId, int.Parse(DrpPrincipal.SelectedValue), long.Parse(DrpCustomer.SelectedValue), long.Parse(DrpCustomer.SelectedValue), -1, -1, int.Parse(RblPayMode.SelectedValue.ToString()),
                //       decimal.Parse(DC.chkNull_0(txtGrossAmount.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalExtraDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalStndrdDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalGST.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlAmnt.Text)), 0, Constants.Order_Pending_Id, PurchaseSKU, dtFreeSKU, int.Parse(this.Session["UserId"].ToString()), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), decimal.Parse(DC.chkNull_0(numtxtUnClaimabledist.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalTST.Text)), Constants.DecimalNullValue, Constants.DecimalNullValue, Constants.DecimalNullValue, null, txtRemarks.Text);
                //    if (IsValidInsert)
                //    {
                //        this.LoadPendingOrder();
                //        this.ClearMasterALL();
                //        this.Session.Remove("hfBillBookNo");
                //    }
                //}
                //else if (btnSaveOrder.Text == "Save Invoice")
                //{
                //    if (Convert.ToDateTime(this.Session["CurrentWorkDate"]) >= Convert.ToDateTime(Session["OrderDate"].ToString()))
                //    {
                //        if (!IsBillBookNoExist())
                //        {
                //            if (int.Parse(RblPayMode.SelectedValue.ToString()) == Constants.Credit_Order_Id || int.Parse(RblPayMode.SelectedValue.ToString()) == Constants.Advance_PaymentOrder_id)
                //            {
                //                CustomerDataController CDC = new CustomerDataController();
                //                DataTable dt = CDC.SelectCustomerCreditBalance(long.Parse(this.Session["CUSTOMER_ID"].ToString()), int.Parse(this.Session["PrincipalId"].ToString()), int.Parse(this.Session["DistributorId"].ToString()), int.Parse(RblPayMode.SelectedValue.ToString()));
                //                decimal NetCashSale = decimal.Parse(numTxtTotlAmnt.Text) - decimal.Parse(DC.chkNull_0(txtCashReceived.Text));

                //                if (decimal.Parse(DC.chkNull_0(dt.Rows[0][0].ToString())) <= NetCashSale)
                //                {
                //                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Customer Credit Limit/Advance Amount is " + DC.chkNull_0(dt.Rows[0][0].ToString()) + "');", true);
                //                    return;
                //                }

                //            }

                //            foreach (DataRow dr in PurchaseSKU.Rows)
                //            {
                //                PhaysicalStockController mController = new PhaysicalStockController();
                //                DataTable dtstock = mController.SelectSKUClosingStock(int.Parse(this.Session["DistributorId"].ToString()), int.Parse(dr["SKU_ID"].ToString()), txtBatchNo.Text, DateTime.Parse(this.Session["OrderDate"].ToString()));
                //                if (dtstock.Rows.Count > 0)
                //                {
                //                    if (int.Parse(dtstock.Rows[0][0].ToString()) < int.Parse(dr["QUANTITY"].ToString()))
                //                    {
                //                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('" + dr["SKU_Code"].ToString() + " Current Stock is " + dtstock.Rows[0][0].ToString() + "');", true);
                //                        return;
                //                    }
                //                    else if (Convert.ToDecimal(dtstock.Rows[0][1]) != Convert.ToDecimal(dr["UNIT_PRICE"]))
                //                    {
                //                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('" + dr["SKU_Code"].ToString() + " Order Rate is " + dr["UNIT_PRICE"].ToString() + " and Current Rate is " + dtstock.Rows[0][1].ToString() + "');", true);
                //                        return;
                //                    }
                //                }
                //                else
                //                {
                //                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert(' + " + dr["SKU_Code"].ToString() + "No Stock Found');", true);
                //                    return;
                //                }
                //            }
                //            bool IsValidInsert = mOrderController.Add_Invoice(int.Parse(this.Session["DistributorId"].ToString()), ManualID, mTownId, long.Parse(this.Session["AreaId"].ToString()), int.Parse(this.Session["PrincipalId"].ToString()), long.Parse(this.Session["CUSTOMER_ID"].ToString()), long.Parse(this.Session["CUSTOMER_ID"].ToString()), int.Parse(this.Session["OrderBookerId"].ToString()), int.Parse(this.Session["DeliveryManId"].ToString()), long.Parse(drpDocumentNo.SelectedValue.ToString()),
                //            decimal.Parse(DC.chkNull_0(txtGrossAmount.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalExtraDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalStndrdDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalGST.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlAmnt.Text)), 0, int.Parse(RblPayMode.SelectedValue.ToString()),
                //            PurchaseSKU, dtFreeSKU, int.Parse(this.Session["UserId"].ToString()), decimal.Parse(DC.chkNull_0(txtCashReceived.Text)), DateTime.Parse(this.Session["OrderDate"].ToString()), decimal.Parse(DC.chkNull_0(numTxtTotalTST.Text)), decimal.Parse(DC.chkNull_0(numtxtUnClaimabledist.Text)), decimal.Parse(DC.chkNull_0(numtxtYcDiscount.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlFMR.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlMR.Text)),txtTruckNo .Text ,txtDriverName .Text );
                //            if (IsValidInsert)
                //            {
                //                if (drpDocumentNo.Enabled == true)
                //                {
                //                    btnSaveOrder.Text = "Save Order";
                //                }
                //                this.LoadPendingOrder();
                //                this.ClearMasterALL();
                //                this.Session.Remove("hfBillBookNo");
                //            }
                //        }
                //        else
                //        {
                //            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('This Bill Book No already exist,Kindly enter different Bill Book No');", true);
                //        }
                //    }
                //    else
                //    {
                //        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Order Date Can not be greater than current working date');", true);
                //    }
                //}
                //else if (btnSaveOrder.Text == "Sale Return")
                //{
                //    bool IsValidInsert = mOrderController.Add_SaleReturn(int.Parse(this.Session["DistributorId"].ToString()), mTownId, (long)this.Session["AreaId"], int.Parse(this.Session["PrincipalId"].ToString()), long.Parse(this.Session["CUSTOMER_ID"].ToString()), long.Parse(this.Session["CUSTOMER_ID"].ToString()), int.Parse(this.Session["OrderBookerId"].ToString()), int.Parse(this.Session["DeliveryManId"].ToString()), long.Parse(drpDocumentNo.SelectedValue.ToString()),
                //    decimal.Parse(DC.chkNull_0(txtGrossAmount.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalExtraDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalStndrdDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalGST.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlAmnt.Text)), 0, int.Parse(RblPayMode.SelectedValue.ToString()), PurchaseSKU, dtFreeSKU, int.Parse(this.Session["UserId"].ToString()), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), decimal.Parse(DC.chkNull_0(numTxtTotalTST.Text)), decimal.Parse(DC.chkNull_0(numtxtUnClaimabledist.Text)));
                //    if (IsValidInsert)
                //    {
                //        this.LoadPendingOrder();
                //        this.ClearMasterALL();
                //    }
                //}
                #endregion
            }
        }
    }

    /// <summary>
    /// Clears Some Of Controls
    /// </summary>
    private void ClearAll()
    {
       
        txtUnitRate.Text = "";
        txtQuantity.Text = "";
        txtQuantityCtn.Text = "";
        btnSave.Text = "Add Sku";
        btnSaveOrder.Enabled = false;
        btnCalculate.Enabled = true;
        DrpSkuDetail.Enabled = true;

    }

    /// <summary>
    /// Clears All Controls
    /// </summary>
    private void ClearMasterALL()
    {   
        this.Session.Remove("PurchaseSKU");
        this.Session.Remove("dtFreeSKU");
        if (Session["orderId"] != null)
        {
            this.Session.Remove("orderId");
        }
        if (Session["invoiceId"] != null)
        {
            this.Session.Remove("invoiceId");
        }


    }

    private bool IsDayClosed()
    {
        bool flag = false;
        DistributorController DistrCtl = new DistributorController();
        DataTable dtDayClose = DistrCtl.MaxDayClose(Convert.ToInt32(drpDistributor.SelectedValue), 3);
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



    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        //LoadPendingOrder();
        LoadCustomerData();
        LoadSKUDetail();
        LoadCustomerOpBalance();
        LoadAssingned();

    }

    protected void DrpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadPromotion();
        LoadSKUDetail();
    }
    private void LoadCustomerOpBalance()
    {
        if (drpDistributor.Items.Count > 0 && DrpCustomer.Items.Count > 0)
        {
            CustomerDataController mController = new CustomerDataController();
            DataTable dt = mController.SelectPrincipalCustomerOpBalance(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpCustomer.SelectedValue.ToString()), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()).AddDays(1), Constants.IntNullValue);
            lblCustomerclosing.Text = (decimal.Parse(dt.Rows[0][0].ToString()) * -1).ToString();
        }
        else
        { lblCustomerclosing.Text = "0.00"; }
    }
}
