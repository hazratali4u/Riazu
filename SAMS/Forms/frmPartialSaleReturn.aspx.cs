using System;
using System.Data;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
public partial class Forms_frmPartialSaleReturn : System.Web.UI.Page
{
    SKUPriceDetailController PController = new SKUPriceDetailController();

    #region Variables

    DataTable PurchaseSKU;
    DataTable dtFreeSKU;
    DataTable dtSaleReturn;


    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            DrpSaleReturn.Items.Add(new clsListItems("New", Constants.LongNullValue.ToString()));
            this.LoadDistributor();
            txtAddress.Attributes.Add("Readonly", "Readonly");
            txtArea.Attributes.Add("Readonly", "Readonly");
            txtDeliveryMan.Attributes.Add("Readonly", "Readonly");
            txtOutletName.Attributes.Add("Readonly", "Readonly");
            DrpDistributor.Focus();
        }
    }
    private void LoadDistributor()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDropDownList(this.DrpDistributor, dt, 0, 2, true);
    }

    //private void LoadSaleReturn()
    //{
    //    DrpSaleReturn.Items.Clear();
    //       dtSaleReturn = (DataTable)this.Session["dtSaleReturn"];
    //    DrpSaleReturn.Items.Add(new ListItem("New", Constants.LongNullValue.ToString()));
    //    clsWebFormUtil.FillDropDownList(this.DrpSaleReturn, dtSaleReturn, 0, 0, false);
    //}

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            OrderEntryController or = new OrderEntryController();
            DataControl dc = new DataControl();

            DataTable dtOrder = or.SelectPendingInvoice(int.Parse(DrpDistributor.SelectedValue), Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(dc.chkNull_0(txtInvoiceNo.Text)), Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.DateNullValue, 0);
            if (dtOrder != null && dtOrder.Rows.Count > 0)
            {
                hdnSale_invoice_ID.Value = dtOrder.Rows[0]["SALE_INVOICE_ID"].ToString();
                hdnCustomer_ID.Value = dtOrder.Rows[0]["CUSTOMER_ID"].ToString();
                hdnArea_ID.Value = dtOrder.Rows[0]["AREA_ID"].ToString();
                hdnTown_Id.Value = dtOrder.Rows[0]["TOWN_ID"].ToString();
                hdnOrderBooker_ID.Value = dtOrder.Rows[0]["ORDERBOOKER_ID"].ToString();
                hdnDeliveryMan_Id.Value = dtOrder.Rows[0]["DELIVERYMAN_ID"].ToString();
                hdnPrincipal_Id.Value = dtOrder.Rows[0]["PRINCIPAL_ID"].ToString();


                txtArea.Text = dtOrder.Rows[0]["AREA_NAME"].ToString();
                txtDeliveryMan.Text = dtOrder.Rows[0]["DELIVERYMAN"].ToString();
                txtOutletName.Text = dtOrder.Rows[0]["CUSTOMER_NAME"].ToString();
                txtAddress.Text = dtOrder.Rows[0]["ADDRESS"].ToString();
                ExistenInvoiceDetail(long.Parse(hdnSale_invoice_ID.Value));
                EnableDisableController(false);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('No Record Found !')", true);
            }

        }
        catch (Exception ex)
        {
            ExceptionPublisher.PublishException(ex);
        }

    }
    private void ExistenInvoiceDetail(long Sale_invoice_id)
    {
        OrderEntryController ord = new OrderEntryController();
        PurchaseSKU = ord.SelectInvoiceDetail(int.Parse(DrpDistributor.SelectedValue), Sale_invoice_id);
        dtFreeSKU = ord.SelectInvoicePromotion(int.Parse(DrpDistributor.SelectedValue), Sale_invoice_id);
         dtSaleReturn = ord.SelectSaleReturnDocument(int.Parse(DrpDistributor.SelectedValue), Sale_invoice_id);
        this.Session.Add("PurchaseSKU", PurchaseSKU);
        this.Session.Add("dtFreeSKU", dtFreeSKU);
        this.Session.Add("dtSaleReturn", dtSaleReturn);
        this.LoadGird();
        this.LoadFreeGrid();
        this.SetTotal();
       
    }
    private void LoadGird()
    {
        try
        {
            PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
           // GrdInvoice.EditIndex = -1;
            GrdInvoice.DataSource = PurchaseSKU;
            GrdInvoice.DataBind();
        }
        catch (Exception ex)
        {
            ExceptionPublisher.PublishException(ex);
        }
    }
    private void LoadFreeGrid()
    {
        try
        {
            dtFreeSKU = (DataTable)this.Session["dtFreeSKU"];
            GrdFreeSKU.DataSource = dtFreeSKU;
            GrdFreeSKU.DataBind();
        }
        catch (Exception ex)
        {
            ExceptionPublisher.PublishException(ex);
        }
    }

    private void SetTotal()
    {
        try
        {
            PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
            dtFreeSKU = (DataTable)this.Session["dtFreeSKU"];

            decimal GrossAmount = 0;
            decimal StandardDiscount = 0;
            decimal ExtraDiscount = 0;
            decimal SedAmount = 0;
            decimal GstAmount = 0;
            decimal TstAmount = 0;
            decimal ExtraTax = 0;
           

            decimal FreeGstAmount = 0;
            decimal FreeTstAmount = 0;
            decimal FreeExtraTax = 0;


            decimal ObjStandardDiscount_By_VALUE = 0;
            decimal ObjStandardDiscount_By_VALUE_TO = 0;
            decimal ObjStandardDiscount_By_VALUE_AD = 0;

            decimal ObjStandardDiscount_PER_VALUE = 0;
            decimal ObjStandardDiscount_PER_VALUE_TO = 0;
            decimal ObjStandardDiscount_PER_VALUE_AD = 0;

            decimal ObjTotalSED_BY_VALUE = 0;
            decimal ObjTotalSED_BY_VALUE_TO = 0;
            decimal ObjTotalSED_BY_VALUE_AD = 0;

            decimal ObjTotalSED_PER_VALUE = 0;
            decimal ObjTotalSED_PER_VALUE_TO = 0;
            decimal ObjTotalSED_PER_VALUE_AD = 0;


            foreach (DataRow dr in PurchaseSKU.Rows)
            {
                GrossAmount += decimal.Parse(dr["SR_GROSS_AMOUNT"].ToString());
                StandardDiscount += decimal.Parse(dr["SR_STANDARD_DISCOUNT"].ToString());
                ExtraDiscount += decimal.Parse(dr["SR_EXTRA_DISCOUNT"].ToString());
                SedAmount += decimal.Parse(dr["SR_SED_AMOUNT"].ToString());
                GstAmount += decimal.Parse(dr["SR_GST_AMOUNT"].ToString());
                TstAmount += decimal.Parse(dr["SR_TST_AMOUNT"].ToString());
                ExtraTax += decimal.Parse(dr["SR_EXTRA_TAX"].ToString());

                ObjTotalSED_BY_VALUE += decimal.Parse(dr["SR_SED_AMOUNT_BY_VALUE"].ToString());
                ObjTotalSED_BY_VALUE_AD += decimal.Parse(dr["SR_SED_AMOUNT_BY_VALUE_AD"].ToString());
                ObjTotalSED_BY_VALUE_TO += decimal.Parse(dr["SR_SED_AMOUNT_BY_VALUE_TO"].ToString());

                ObjTotalSED_PER_VALUE += decimal.Parse(dr["SR_SED_AMOUNT_PER_VALUE"].ToString());
                ObjTotalSED_PER_VALUE_AD += decimal.Parse(dr["SR_SED_AMOUNT_PER_VALUE_AD"].ToString());
                ObjTotalSED_PER_VALUE_TO += decimal.Parse(dr["SR_SED_AMOUNT_PER_VALUE_TO"].ToString());

                        
                ObjStandardDiscount_By_VALUE += decimal.Parse(dr["SR_STANDARD_DISCOUNT_BY_VALUE"].ToString());
                ObjStandardDiscount_By_VALUE_TO += decimal.Parse(dr["SR_STANDARD_DISCOUNT_BY_VALUE_TO"].ToString());
                ObjStandardDiscount_By_VALUE_AD += decimal.Parse(dr["SR_STANDARD_DISCOUNT_BY_VALUE_AD"].ToString());

                ObjStandardDiscount_PER_VALUE += decimal.Parse(dr["SR_STANDARD_DISCOUNT_PER_VALUE"].ToString());
                ObjStandardDiscount_PER_VALUE_TO += decimal.Parse(dr["SR_STANDARD_DISCOUNT_PER_VALUE_TO"].ToString());
                ObjStandardDiscount_PER_VALUE_AD += decimal.Parse(dr["SR_STANDARD_DISCOUNT_PER_VALUE_AD"].ToString());

                }

            foreach (DataRow dr in dtFreeSKU.Rows)
            {

                FreeGstAmount += decimal.Parse(dr["SR_GST_AMOUNT"].ToString());
                FreeTstAmount += decimal.Parse(dr["SR_TST_AMOUNT"].ToString());
                FreeExtraTax += decimal.Parse(dr["SR_EXTRA_TAX"].ToString());
            }

            txtGrossAmount.Text = Math.Round(GrossAmount, 4).ToString();
            numtxtDisDiscount.Text = Math.Round(StandardDiscount - SedAmount, 4).ToString();
            numtxtCompanyDiscount.Text = Math.Round(ObjTotalSED_BY_VALUE_AD + ObjTotalSED_PER_VALUE_AD, 4).ToString();
            numtxtTradeOffer.Text = Math.Round(ObjTotalSED_BY_VALUE_TO + ObjTotalSED_PER_VALUE_TO, 4).ToString();

            numTxtTotalStndrdDiscnt.Text = Convert.ToString(Math.Round(StandardDiscount, 4));

            numTxtTotalGST.Text = Math.Round(GstAmount + FreeGstAmount + ExtraTax + FreeExtraTax, 4).ToString();
            numTxtTotalTST.Text = Math.Round(TstAmount + FreeTstAmount, 4).ToString();
            txtExtra_tax.Text = Convert.ToString(Math.Round(ExtraTax + FreeExtraTax, 4));
            DataControl dc = new DataControl();
            numTxtTotlAmnt.Text = Convert.ToString(Math.Round(GrossAmount 
                - Convert.ToDecimal(dc.chkNull_0(numtxtTotalExtraDiscnt.Text))
                  - StandardDiscount + GstAmount
                  + FreeGstAmount
                  + TstAmount
                  + FreeTstAmount
                  + ExtraTax
                  + FreeExtraTax
                  , 4));


            numtxtTotalStandardDis_By_value.Text = Convert.ToString(Math.Round(ObjStandardDiscount_By_VALUE, 4));
            numtxtTotalStandardDis_By_value_TO.Text = Convert.ToString(Math.Round(ObjStandardDiscount_By_VALUE_TO, 4));
            numtxtTotalStandardDis_By_value_AD.Text = Convert.ToString(Math.Round(ObjStandardDiscount_By_VALUE_AD, 4));

            numtxtTotalStandardDis_PER_value.Text = Convert.ToString(Math.Round(ObjStandardDiscount_PER_VALUE, 4));
            numtxtTotalStandardDis_PER_value_TO.Text = Convert.ToString(Math.Round(ObjStandardDiscount_PER_VALUE_TO, 4));
            numtxtTotalStandardDis_PER_value_AD.Text = Convert.ToString(Math.Round(ObjStandardDiscount_PER_VALUE_AD, 4));
            numtxtUnClaimabledist.Text = Convert.ToString(Math.Round(SedAmount, 4));

            numtxtTotalSED_BY_value.Text = Convert.ToString(Math.Round(ObjTotalSED_BY_VALUE, 4));
            numtxtTotalSED_BY_value_TO.Text = Convert.ToString(Math.Round(ObjTotalSED_BY_VALUE_TO, 4));
            numtxtTotalSED_BY_value_AD.Text = Convert.ToString(Math.Round(ObjTotalSED_BY_VALUE_AD, 4));

            numtxtTotalSED_PER_value.Text = Convert.ToString(Math.Round(ObjTotalSED_PER_VALUE, 4));
            numtxtTotalSED_PER_value_TO.Text = Convert.ToString(Math.Round(ObjTotalSED_PER_VALUE_TO, 4));
            numtxtTotalSED_PER_value_AD.Text = Convert.ToString(Math.Round(ObjTotalSED_PER_VALUE_AD, 4));


        }
        catch (Exception ex)
        {
            ExceptionPublisher.PublishException(ex);
        }
    }

    protected void btnCalculate_Click(object sender, EventArgs e)
    {  
        if (FindCustomer())
        {
            DataTable PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
            DataTable dtFreeSKU = (DataTable)this.Session["dtFreeSKU"];
            DataControl _dc = new DataControl();


            #region master grid
            int TotalReturnQty = 0;

            foreach (GridViewRow gvr in GrdInvoice.Rows)
            {
                TextBox txtreturnCtn = gvr.FindControl("txtreturnCtn") as TextBox;
                TextBox txtreturnUnit = gvr.FindControl("txtreturnUnit") as TextBox;

                //if (Convert.ToInt32(_dc.chkNull_0(txtreturnCtn.Text)) > 0 || Convert.ToInt32(_dc.chkNull_0(txtreturnUnit.Text)) > 0)
                //{
                int UIC = Convert.ToInt32(_dc.chkNull_0(gvr.Cells[14].Text));
                decimal UnitPrice = Convert.ToDecimal(_dc.chkNull_0(gvr.Cells[5].Text));

                TotalReturnQty = (int.Parse(_dc.chkNull_0(txtreturnCtn.Text)) * UIC) + (int.Parse(_dc.chkNull_0(txtreturnUnit.Text)));

                foreach (DataRow dr in PurchaseSKU.Rows)
                {
                    if (dr["SKU_ID"].ToString() == gvr.Cells[0].Text)
                    {

                        int totalPrevReturnQty = int.Parse(dr["TOTAL_PREV_RETURN_QTY"].ToString());
                        int totalSaleQty = int.Parse(dr["TOTAL_SALE_QTY"].ToString());
                        if (TotalReturnQty + totalPrevReturnQty > totalSaleQty)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('"+dr["SKU_NAME"].ToString()+" Qty Should not Greater then Balance Qty!')", true);
                            return;
                        }
                        dr["RETURN_CTN"] = int.Parse(_dc.chkNull_0(txtreturnCtn.Text)).ToString();
                        dr["RETURN_UNIT"] = int.Parse(_dc.chkNull_0(txtreturnUnit.Text)).ToString();
                        dr["SR_GROSS_AMOUNT"] = TotalReturnQty * UnitPrice;


                        decimal standard_discount = decimal.Parse(dr["STANDARD_DISCOUNT"].ToString());
                        decimal std_dis_per = standard_discount / totalSaleQty;
                        dr["SR_STANDARD_DISCOUNT"] = TotalReturnQty * std_dis_per;

                        decimal extra_discount = decimal.Parse(dr["EXTRA_DISCOUNT"].ToString());
                        decimal ext_dis_per = extra_discount / totalSaleQty;
                        dr["SR_EXTRA_DISCOUNT"] = TotalReturnQty * ext_dis_per;

                        decimal sed_amount = decimal.Parse(dr["SED_AMOUNT"].ToString());
                        decimal sed_amount_per = sed_amount / totalSaleQty;
                        dr["SR_SED_AMOUNT"] = TotalReturnQty * sed_amount_per;

                        decimal gst_amount = decimal.Parse(dr["GST_AMOUNT"].ToString());
                        decimal GST_AMOUNT_PER = gst_amount / totalSaleQty;
                        dr["SR_GST_AMOUNT"] = TotalReturnQty * GST_AMOUNT_PER;

                        decimal tst_amount = decimal.Parse(dr["TST_AMOUNT"].ToString());
                        decimal TST_AMOUNT_PER = tst_amount / totalSaleQty;
                        dr["SR_TST_AMOUNT"] = TotalReturnQty * TST_AMOUNT_PER;

                        decimal extra_tax = decimal.Parse(dr["EXTRA_TAX"].ToString());
                        decimal extra_tax_per = extra_tax / totalSaleQty;
                        dr["SR_EXTRA_TAX"] = TotalReturnQty * extra_tax_per;

                        decimal STANDARD_DISCOUNT_BY_VALUE = decimal.Parse(dr["STANDARD_DISCOUNT_BY_VALUE"].ToString());
                        decimal STANDARD_DISCOUNT_BY_VALUE_per = STANDARD_DISCOUNT_BY_VALUE / totalSaleQty;
                        dr["SR_STANDARD_DISCOUNT_BY_VALUE"] = TotalReturnQty * STANDARD_DISCOUNT_BY_VALUE_per;


                        decimal STANDARD_DISCOUNT_PER_VALUE = decimal.Parse(dr["STANDARD_DISCOUNT_PER_VALUE"].ToString());
                        decimal STANDARD_DISCOUNT_PER_VALUE_PER = STANDARD_DISCOUNT_PER_VALUE / totalSaleQty;
                        dr["SR_STANDARD_DISCOUNT_PER_VALUE"] = TotalReturnQty * STANDARD_DISCOUNT_PER_VALUE_PER;

                        decimal SED_AMOUNT_BY_VALUE = decimal.Parse(dr["SED_AMOUNT_BY_VALUE"].ToString());
                        decimal SED_AMOUNT_BY_VALUE_PER = SED_AMOUNT_BY_VALUE / totalSaleQty;
                        dr["SR_SED_AMOUNT_BY_VALUE"] = TotalReturnQty * SED_AMOUNT_BY_VALUE_PER;


                        decimal SED_AMOUNT_PER_VALUE = decimal.Parse(dr["SED_AMOUNT_PER_VALUE"].ToString());
                        decimal SED_AMOUNT_PER_VALUE_PER = SED_AMOUNT_PER_VALUE / totalSaleQty;
                        dr["SR_SED_AMOUNT_PER_VALUE"] = TotalReturnQty * SED_AMOUNT_PER_VALUE_PER;


                        decimal STANDARD_DISCOUNT_BY_VALUE_TO = decimal.Parse(dr["STANDARD_DISCOUNT_BY_VALUE_TO"].ToString());
                        decimal STANDARD_DISCOUNT_BY_VALUE_TO_PER = STANDARD_DISCOUNT_BY_VALUE_TO / totalSaleQty;
                        dr["SR_STANDARD_DISCOUNT_BY_VALUE_TO"] = TotalReturnQty * STANDARD_DISCOUNT_BY_VALUE_TO_PER;

                        decimal STANDARD_DISCOUNT_BY_VALUE_AD = decimal.Parse(dr["STANDARD_DISCOUNT_BY_VALUE_AD"].ToString());
                        decimal STANDARD_DISCOUNT_BY_VALUE_AD_PER = STANDARD_DISCOUNT_BY_VALUE_AD / totalSaleQty;
                        dr["SR_STANDARD_DISCOUNT_BY_VALUE_AD"] = TotalReturnQty * STANDARD_DISCOUNT_BY_VALUE_AD_PER;


                        decimal STANDARD_DISCOUNT_PER_VALUE_TO = decimal.Parse(dr["STANDARD_DISCOUNT_PER_VALUE_TO"].ToString());
                        decimal STANDARD_DISCOUNT_PER_VALUE_TO_PER = STANDARD_DISCOUNT_PER_VALUE_TO / totalSaleQty;
                        dr["SR_STANDARD_DISCOUNT_PER_VALUE_TO"] = TotalReturnQty * STANDARD_DISCOUNT_PER_VALUE_TO_PER;

                        decimal STANDARD_DISCOUNT_PER_VALUE_AD = decimal.Parse(dr["STANDARD_DISCOUNT_PER_VALUE_AD"].ToString());
                        decimal STANDARD_DISCOUNT_PER_VALUE_AD_PER = STANDARD_DISCOUNT_PER_VALUE_AD / totalSaleQty;
                        dr["SR_STANDARD_DISCOUNT_PER_VALUE_AD"] = TotalReturnQty * STANDARD_DISCOUNT_PER_VALUE_AD_PER;


                        decimal SED_AMOUNT_BY_VALUE_TO = decimal.Parse(dr["SED_AMOUNT_BY_VALUE_TO"].ToString());
                        decimal SED_AMOUNT_BY_VALUE_TO_PER = SED_AMOUNT_BY_VALUE_TO / totalSaleQty;
                        dr["SR_SED_AMOUNT_BY_VALUE_TO"] = TotalReturnQty * SED_AMOUNT_BY_VALUE_TO_PER;

                        decimal SED_AMOUNT_BY_VALUE_AD = decimal.Parse(dr["SED_AMOUNT_BY_VALUE_AD"].ToString());
                        decimal SED_AMOUNT_BY_VALUE_AD_PER = SED_AMOUNT_BY_VALUE_AD / totalSaleQty;
                        dr["SR_SED_AMOUNT_BY_VALUE_AD"] = TotalReturnQty * SED_AMOUNT_BY_VALUE_AD_PER;

                        decimal SED_AMOUNT_PER_VALUE_TO = decimal.Parse(dr["SED_AMOUNT_PER_VALUE_TO"].ToString());
                        decimal SED_AMOUNT_PER_VALUE_TO_PER = SED_AMOUNT_PER_VALUE_TO / totalSaleQty;
                        dr["SR_SED_AMOUNT_PER_VALUE_TO"] = TotalReturnQty * SED_AMOUNT_PER_VALUE_TO_PER;

                        decimal SED_AMOUNT_PER_VALUE_AD = decimal.Parse(dr["SED_AMOUNT_PER_VALUE_AD"].ToString());
                        decimal SED_AMOUNT_PER_VALUE_AD_PER = SED_AMOUNT_PER_VALUE_AD / totalSaleQty;
                        dr["SR_SED_AMOUNT_PER_VALUE_AD"] = TotalReturnQty * SED_AMOUNT_PER_VALUE_AD_PER;

                        dr["SR_NET_AMOUNT"] = decimal.Parse(dr["SR_GROSS_AMOUNT"].ToString())
                           - decimal.Parse(dr["SR_STANDARD_DISCOUNT"].ToString())
                            - decimal.Parse(dr["SR_EXTRA_DISCOUNT"].ToString())
                            + decimal.Parse(dr["SR_GST_AMOUNT"].ToString())
                            + decimal.Parse(dr["SR_TST_AMOUNT"].ToString())
                            + decimal.Parse(dr["SR_EXTRA_TAX"].ToString());
                        break;
                    }
                }

                // }
            }
            this.Session.Add("PurchaseSKU", PurchaseSKU);
            LoadGird();
            
            #endregion

            #region Free sku grid
            foreach (GridViewRow gvr in GrdFreeSKU.Rows)
            {
                TextBox txtFreeQty = gvr.FindControl("txtFreeQty") as TextBox;
                
                    decimal UnitPrice = Convert.ToDecimal(_dc.chkNull_0(gvr.Cells[2].Text));
                foreach (DataRow dr in dtFreeSKU.Rows)
                {
                    if (dr["SKU_ID"].ToString() == gvr.Cells[0].Text)
                    {
                        int totalInvoiceFreeQty = int.Parse(dr["QUANTITY_UNIT"].ToString());
                        int totalPrevFreeQty = int.Parse(dr["PREV_RETURN_UNIT"].ToString());
                        int FreeReturnQty = int.Parse(_dc.chkNull_0(txtFreeQty.Text));

                        if (FreeReturnQty + totalPrevFreeQty > totalInvoiceFreeQty)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert(' "+dr["SKU_NAME"].ToString()+" Free Qty Should not Greater then Balance Qty!')", true);
                            return;
                        }


                        dr["Return_unit"] = int.Parse(_dc.chkNull_0(txtFreeQty.Text)).ToString();

                        dr["SR_AMOUNT"] = FreeReturnQty * UnitPrice;

                       

                        decimal gst_amount = decimal.Parse(dr["GST_AMOUNT"].ToString());
                        decimal gst_amount_per = gst_amount / totalInvoiceFreeQty;
                        dr["SR_GST_AMOUNT"] = FreeReturnQty * gst_amount_per;

                        decimal tst_amount = decimal.Parse(dr["TST_AMOUNT"].ToString());
                        decimal tst_amount_per = tst_amount / totalInvoiceFreeQty;
                        dr["SR_TST_AMOUNT"] = FreeReturnQty * tst_amount_per;


                        decimal extra_tax = decimal.Parse(dr["EXTRA_TAX"].ToString());
                        decimal extra_tax_per = extra_tax / totalInvoiceFreeQty;
                        dr["SR_EXTRA_TAX"] = FreeReturnQty * extra_tax_per;

                        decimal sed_amount = decimal.Parse(dr["SED_AMOUNT"].ToString());
                        decimal sed_amount_per = sed_amount / totalInvoiceFreeQty;
                        dr["SR_SED_AMOUNT"] = FreeReturnQty * sed_amount_per;


                        break;
                    }
                    //}

                }
            }
            this.Session.Add("dtFreeSKU", dtFreeSKU);
            LoadFreeGrid();
            SetTotal();
            #endregion
            btnSaveOrder.Enabled = true;

        }
    }

    private bool FindCustomer()
    {
        if (int.Parse(hdnCustomer_ID.Value) > 0)
        {
            return true;
        }

        return false;
    }
    private void EnableDisableController(bool CValue)
    {

        if (CValue == true)
        {

            DrpDistributor.Enabled = true;
            txtInvoiceNo.Enabled = true;
            btnSearch.Enabled = true;

        }
        else
        {
            DrpDistributor.Enabled = false;
            txtInvoiceNo.Enabled = false;
            btnSearch.Enabled = false;
        }
    }
    
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Forms/Home.aspx");
    }
    protected void btnSaveOrder_Click(object sender, EventArgs e)
    {
        btnCalculate_Click(null, null);
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

                DataControl DC = new DataControl();
                PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
                dtFreeSKU = (DataTable)this.Session["dtFreeSKU"];
                OrderEntryController mOrderController = new OrderEntryController();
                // sale return case
                bool IsValidInsert = mOrderController.Add_PartialSaleReturn(int.Parse(DrpDistributor.SelectedValue), int.Parse(hdnTown_Id.Value), long.Parse(hdnArea_ID.Value), int.Parse(hdnPrincipal_Id.Value), long.Parse(hdnCustomer_ID.Value), long.Parse(hdnCustomer_ID.Value), int.Parse(hdnOrderBooker_ID.Value), int.Parse(hdnDeliveryMan_Id.Value), long.Parse(hdnSale_invoice_ID.Value),
                decimal.Parse(DC.chkNull_0(txtGrossAmount.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalExtraDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalStndrdDiscnt.Text)), decimal.Parse(DC.chkNull_0(numTxtTotalGST.Text)), decimal.Parse(DC.chkNull_0(numTxtTotlAmnt.Text)), 0, int.Parse(RblPayMode.SelectedValue.ToString()), PurchaseSKU, dtFreeSKU, int.Parse(this.Session["UserId"].ToString()), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), decimal.Parse(DC.chkNull_0(numTxtTotalTST.Text)), decimal.Parse(DC.chkNull_0(numtxtUnClaimabledist.Text)), decimal.Parse(DC.chkNull_0(txtExtra_tax.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalStandardDis_By_value.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalStandardDis_PER_value.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalSED_BY_value.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalSED_PER_value.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalStandardDis_By_value_TO.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalStandardDis_By_value_AD.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalStandardDis_PER_value_TO.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalStandardDis_PER_value_AD.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalSED_BY_value_TO.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalSED_BY_value_AD.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalSED_PER_value_TO.Text)), decimal.Parse(DC.chkNull_0(numtxtTotalSED_PER_value_AD.Text))
                    , DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), 1);//Constants.InvoiceWise_sale_return=1
                if (IsValidInsert)
                {

                    this.ClearMasterALL();
                }

            }
        }
    }

    private void ClearAll()
    {
       
        numtxtDisDiscount.Text = "";
        numtxtCompanyDiscount.Text = "";
        numtxtTradeOffer.Text = "";
        btnSaveOrder.Enabled = false;
        btnCalculate.Enabled = true;
    }
    private void Clear()
    {
        btnSaveOrder.Enabled = false;
        btnCalculate.Enabled = true;
    }

    private void ClearMasterALL()
    {
        this.EnableDisableController(true);
        this.Session.Remove("PurchaseSKU");
        this.Session.Remove("dtFreeSKU");
        this.Session.Remove("dtSaleReturn");

        btnSaveOrder.Enabled = false;

        txtOutletName.Text = "";
        this.LoadGird();
        this.LoadFreeGrid();
        this.SetTotal();
        txtGrossAmount.Text = "";
        numtxtTotalExtraDiscnt.Text = "";
        numTxtTotalStndrdDiscnt.Text = "";
        numTxtTotalGST.Text = "";
        numTxtTotlAmnt.Text = "";
        numTxtTotalSED.Text = "";
        numTxtTotalTST.Text = "";
        numTxtTotalSED.Text = "";
        numtxtUnClaimabledist.Text = "";
        txtRemarks.Text = "";

        txtCashReceived.Text = "";
        hdnCustomer_ID.Value = "0";
        hdnSale_invoice_ID.Value = "0";
        hdnArea_ID.Value = "0";
        hdnTown_Id.Value = "0";
        hdnPrincipal_Id.Value = "0";
        hdnOrderBooker_ID.Value = "0";
        hdnDeliveryMan_Id.Value = "0";
       
    }

    private bool IsDayClosed()
    {
        bool flag = false;
        DistributorController DistrCtl = new DistributorController();
        DataTable dtDayClose = DistrCtl.MaxDayClose(Convert.ToInt32(Session["DistributorId"]), 3);
        if (Convert.ToDateTime(Session["CurrentWorkDate"]) <= Convert.ToDateTime(dtDayClose.Rows[0]["DayClose"]))
        {
            flag = false;
        }
        else
        {
            flag = true;
        }

        return flag;
    }

    protected void numtxtTotalExtraDiscnt_TextChanged(object sender, EventArgs e)
    {
        btnSaveOrder.Enabled = false;
       
    }

    protected void drpDocumentNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        //var dtOrder = (DataTable)this.Session["dtOrder"];
        //DataRow[] foundRows = dtOrder.Select("SALE_INVOICE_ID = '" + drpDocumentNo.SelectedValue + "'");
        //if (foundRows.Length > 0)
        //{
        //    txtOutletCode.Text = foundRows[0]["CUSTOMER_CODE"].ToString();
        //    txtOutletName.Text = foundRows[0]["CUSTOMER_NAME"].ToString();
        //    txtPriceGroup.Text = foundRows[0]["ADDRESS"].ToString();
        //    ExistenInvoiceDetail(long.Parse(drpDocumentNo.SelectedValue));
        //}
    }
    
}