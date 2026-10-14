using System;
using System.Data;
using System.Collections;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using CrystalDecisions.Shared;
using System.Web;
using SAMSBusinessLayer.Reports;
using System.Globalization;

public partial class Forms_frmSaleReturn : System.Web.UI.Page
{
    #region Variables

    readonly DistributorController _mDist = new DistributorController();
    readonly OrderEntryController _or = new OrderEntryController();
    RptCustomerController rptCustomerController = new RptCustomerController();

    DataTable PurchaseSKU;
    readonly DataControl _dc = new DataControl();

    #endregion

    private void CreateTable()
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
        PurchaseSKU.Columns.Add("DISTRIBUTOR_PRICE", typeof(decimal));
        PurchaseSKU.Columns.Add("COST_AMOUNT", typeof(decimal));

        PurchaseSKU.Columns.Add("QUANTITY_UNIT", typeof(decimal));
        PurchaseSKU.Columns.Add("QUANTITY_UNIT_BOX", typeof(decimal));
        PurchaseSKU.Columns.Add("QUANTITY_UNIT_SQM", typeof(decimal));

        PurchaseSKU.Columns.Add("AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("STANDARD_DISCOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("STANDARD_DISCOUNT_TEMP", typeof(decimal));
        PurchaseSKU.Columns.Add("EXTRA_DISCOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("RETEXTRA_DISCOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("NET_AMOUNT", typeof(decimal)); 
PurchaseSKU.Columns.Add("DISTRIBUTOR_DISCOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("RETDISTRIBUTOR_DISCOUNT", typeof(decimal));

        PurchaseSKU.Columns.Add("CLAIMABLE_DISCOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("TST_AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("RETTST_AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("EXTRA_TAX", typeof(decimal));


        PurchaseSKU.Columns.Add("UOM", typeof(int));
       
        PurchaseSKU.Columns.Add("TRADE_PRICE", typeof(string));
        PurchaseSKU.Columns.Add("DISCOUNT_TYPE", typeof(int));
        PurchaseSKU.Columns.Add("SALE_INVOICE_ID", typeof(long));

        PurchaseSKU.Columns.Add("returnQty", typeof(decimal));
        PurchaseSKU.Columns.Add("returnDiscount", typeof(decimal));
        PurchaseSKU.Columns.Add("returnAmount", typeof(decimal));
        PurchaseSKU.Columns.Add("AgeDiscount", typeof(decimal));
        PurchaseSKU.Columns.Add("returnGST", typeof(decimal));
        PurchaseSKU.Columns.Add("retExtraTax", typeof(decimal));
        PurchaseSKU.Columns.Add("SED_AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("RETSED_AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("FREE_QUANTITY", typeof(decimal));
        PurchaseSKU.Columns.Add("RETFREE_QUANTITY", typeof(decimal));


        Session.Add("PurchaseSKU", PurchaseSKU);
    }
 
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {

            CreateTable();
            this.LoadPrincipal();
            this.LoadDistributor();
           
            this.LoadDeliveryman();

            ////////////////////Invoice\\\\\\\\\\\\\\\\\\\\\\\\\\
            txtGrossAmount.Attributes.Add("readonly", "readonly");
            numtxtTotalExtraDiscnt.Attributes.Add("readonly", "readonly");
          
            numtxtDiscount.Attributes.Add("readonly", "readonly");
            numTxtTotalGST.Attributes.Add("readonly", "readonly");
            numTxtTotlAmnt.Attributes.Add("readonly", "readonly");
            txtBalance.Attributes.Add("readonly", "readonly");
         
           
            txtCashRecieved.Attributes.Add("readonly", "readonly");
          
            txtGrossReturn.Attributes.Add("readonly", "readonly");
            txtDiscountReturn.Attributes.Add("readonly", "readonly");
            txtExtraDiscountReturn.Attributes.Add("readonly", "readonly");
            
            txtNetReturn.Attributes.Add("readonly", "readonly");
            txtBalanceReturn.Attributes.Add("readonly", "readonly");
         

            if (Session["srId"] != null)
            {
                
                lblInvoices.Text = "Invoice No" + " - " + Session["srId"].ToString();
                lblInvoices.Visible = true;
                
                DataTable piDt = rptCustomerController.SelectCustomerInvoicePrint(long.Parse(Session["srId"].ToString()),Constants.IntNullValue,3, Constants.DateNullValue);

                if (piDt == null) return;
                if (piDt.Rows.Count > 0)
                {
                    DataRow[] foundRows = piDt.Select("SALE_INVOICE_ID  = '" + Session["srId"] + "'");

                    if (foundRows.Length > 0)
                    {

                        hfCustomerId.Value = foundRows[0]["CUSTOMER_ID"].ToString();
                        hfSaleForceId.Value= foundRows[0]["CUSTOMER_ID"].ToString();
                        drpDistributor.SelectedValue = (foundRows[0]["DISTRIBUTOR_ID"].ToString());
                        DrpPrincipal.SelectedValue = (foundRows[0]["PRINCIPAL_ID"].ToString());
                        DrpPayMode.SelectedValue = foundRows[0]["PAYMENT_MODE"].ToString();
                        txtGrossAmount.Text = String.Format("{0:0.00}", foundRows[0]["TOTAL_AMOUNT"]);
                        numtxtDiscount.Text = String.Format("{0:0.00}", foundRows[0]["EXTRA_DISCOUNT"]);
                    
                        numtxtTotalExtraDiscnt.Text = String.Format("{0:0.00}", foundRows[0]["DISCOUNT"]);
                        numTxtTotalGST.Text = String.Format("{0:0.00}", foundRows[0]["GST"]);
                       
                        numTxtTotlAmnt.Text = String.Format("{0:0.00}", foundRows[0]["NET_AMOUNT"]);
                      
                     //-------------------------------------
                             txtGrossAmount.Text = String.Format("{0:0.00}", foundRows[0]["TOTAL_AMOUNT"]);
                        //numtxtDiscount.Text = String.Format("{0:0.00}", foundRows[0]["EXTRA_DISCOUNT"]);


                        numtxtTotalExtraDiscnt.Text = String.Format("{0:0.00}", foundRows[0]["CASH_RECIEVED"]); ;
                        numTxtTotalGST.Text = String.Format("{0:0.00}", foundRows[0]["GST"]);
                        txtdistrubtrdiscval.Text = String.Format("{0:0.00}", foundRows[0]["DISTRIBUTOR_DISCOUNT"]);
                        numTxtTotlAmnt.Text = String.Format("{0:0.00}", foundRows[0]["NET_AMOUNT"]);
                        txtCashRecieved.Text = "0";
                        txtclaimablediscountval.Text = String.Format("{0:0.00}", foundRows[0]["CLAIMABLE_DISCOUNT"]);
                        txttstamount.Text = String.Format("{0:0.00}", foundRows[0]["TST_AMOUNT"]);
                        txtadditionaltax.Text = String.Format("{0:0.00}", foundRows[0]["EXTRA_TAX"]);
                       // lblCustomerName.Text = foundRows[0]["CUSTOMER_NAME"].ToString();
                        hfSaleForceId.Value = foundRows[0]["SALE_FORCE"].ToString();
                        hfPrincipalId.Value = foundRows[0]["PRINCIPAL_ID"].ToString();
                        hfAREA_ID.Value = foundRows[0]["AREA_ID"].ToString();
                        hfTOWN_ID.Value = foundRows[0]["TOWN_ID"].ToString();

                        hfAssignedDistributorId.Value = foundRows[0]["ASSIGNED_DISTRIBUTOR_ID"].ToString();
                        // txtNarration.Text = foundRows[0]["REMARKS"].ToString();

                        txtExtraDiscountReturn.Text = String.Format("{0:0.00}", foundRows[0]["EXTRA_DISCOUNT"]);
                        //-----------------------------
                       
                        

                        if (int.Parse(DrpPayMode.SelectedValue) != 218)//customer credit id
                        {
                            txtBalance.Text = Convert.ToString(decimal.Parse(txtCashRecieved.Text) - (decimal.Parse(numTxtTotlAmnt.Text)));
                        }
                        try
                        {
                            ///lblInvoiceBalance.Text = String.Format("{0:0.00}", foundRows[0]["invoice_closing"]);
                            //lblinvoiceclosing.Text = "Invoice Balance :";
                            lblinvoicedate.Text = "Invoice Date :" + DateTime.Parse(foundRows[0]["document_date"].ToString()).ToString("dd-MMM-yyyy");
                        }
                        catch (Exception ex)
                        {
                           // lblInvoiceBalance.Text = "0";
                        }

                    }
                }
                
                DataTable dt = _or.GetDocumentDetail(long.Parse(Session["srId"].ToString()), 6, DateTime.Parse(Session["CurrentWorkDate"].ToString()));
                if (dt.Rows.Count > 0)
                {
                    DataRow[] foundRows2 = dt.Select("SALE_INVOICE_ID  = '" + Session["srId"] + "'");
                    if (foundRows2.Length > 0)
                    {
                        txtGrossReturn.Text = String.Format("{0:0.00}", foundRows2[0]["GrossReturn"]);
                        txtDiscountReturn.Text = String.Format("{0:0.00}", foundRows2[0]["ItemDiscountReturn"]);
                        txtExtraDiscountReturn.Text = String.Format("{0:0.00}", foundRows2[0]["ExtraDiscountReturn"]);
                        txtNetReturn.Text = String.Format("{0:0.00}", foundRows2[0]["NetReturn"]);
                        txtCashReturn.Text = String.Format("{0:0.00}", foundRows2[0]["cashRecieved"]);
                      //  txtSaleRetFreight.Text = String.Format("{0:0.00}", foundRows2[0]["FREIGHT_AMOUNT"]);
                        try
                        {
                           // rbFreightType.SelectedValue = foundRows2[0]["FREIGHT_TYPE"].ToString();
                        }
                        catch(Exception ex)
                        {

                        }
                       // txtNarration.Text = foundRows2[0]["remarks"].ToString();
                        
                        hfInvoiceId.Value = foundRows2[0]["SALES_RETURN_ID"].ToString();
                        hfSaleForceId.Value = foundRows2[0]["SALE_FORCE"].ToString();
                        lblCustomerName.Text = foundRows2[0]["CUSTOMER_NAME"].ToString() +" - "+ foundRows2[0]["ADDRESS"].ToString();
                        //if (rbFreightType.SelectedValue == "0")
                        //{
                        //    decimal balance = decimal.Parse(_dc.chkNull_0(txtNetReturn.Text)) - decimal.Parse(_dc.chkNull_0(txtSaleRetFreight.Text)) - decimal.Parse(_dc.chkNull_0(txtCashReturn.Text));
                        //    txtBalanceReturn.Text = balance.ToString();
                        //}
                        //else
                        {
                            decimal balance = decimal.Parse(_dc.chkNull_0(txtNetReturn.Text)) + 0 - decimal.Parse(_dc.chkNull_0(txtCashReturn.Text));
                            txtBalanceReturn.Text = balance.ToString();
                        }
                        btnSaveOrder.Text = "Update";
                    }
                    LoadDocumentDetail(long.Parse(Session["srId"].ToString()), 7);
                }
                
            }
            else
            {
                lblInvoices.Visible = true;
                txtInvoiceNo.Visible = true;
                btnSearch.Visible = true;
            }
        }
    }


    #region Sel/check Changed OPerations

    protected void txtreturnQty_TextChanged(object sender, EventArgs e)
    {
        TextBox txtreturnQty = sender as TextBox;
        if (txtreturnQty == null) return;
        GridViewRow item = (GridViewRow)txtreturnQty.NamingContainer;
        decimal FREESKU = 0;
        decimal returnQty = 0;
        if (!string.IsNullOrEmpty(txtreturnQty.Text))
        {
            returnQty =  decimal.Parse(txtreturnQty.Text);
        }

        DataTable dtOrderDetail = (DataTable)Session["PurchaseSKU"];
        if (returnQty >= 0)
        {
            decimal qty = decimal.Parse(dtOrderDetail.Rows[item.RowIndex]["QUANTITY_UNIT"].ToString());
            if (qty >= returnQty  + decimal.Parse(item.Cells[22].Text))
            {
                if (returnQty + decimal.Parse(item.Cells[22].Text) == qty)
                {
                 FREESKU= decimal.Parse(dtOrderDetail.Rows[item.RowIndex]["FREE_QUANTITY"].ToString());
                }
                decimal price = decimal.Parse(dtOrderDetail.Rows[item.RowIndex]["UNIT_PRICE"].ToString());
                decimal Discount = decimal.Parse(dtOrderDetail.Rows[item.RowIndex]["STANDARD_DISCOUNT"].ToString());
                decimal Distributorprice = decimal.Parse(dtOrderDetail.Rows[item.RowIndex]["DISTRIBUTOR_PRICE"].ToString());
              decimal gstret= decimal.Parse(dtOrderDetail.Rows[item.RowIndex]["GST_AMOUNT"].ToString());
                decimal extratax = decimal.Parse(dtOrderDetail.Rows[item.RowIndex]["EXTRA_TAX"].ToString());
                decimal EXTRA_DISCOUNT= decimal.Parse(dtOrderDetail.Rows[item.RowIndex]["EXTRA_DISCOUNT"].ToString()); 
                decimal DISTRIBUTOR_DISCOUNT = decimal.Parse(dtOrderDetail.Rows[item.RowIndex]["DISTRIBUTOR_DISCOUNT"].ToString());
                decimal SED_AMOUNT = decimal.Parse(dtOrderDetail.Rows[item.RowIndex]["SED_AMOUNT"].ToString());
                decimal TST_AMOUNT= decimal.Parse(dtOrderDetail.Rows[item.RowIndex]["TST_AMOUNT"].ToString());
                Discount = Discount / qty;
                Discount = Discount * returnQty;
                EXTRA_DISCOUNT = (EXTRA_DISCOUNT / qty) * returnQty;
                SED_AMOUNT= (SED_AMOUNT / qty) * returnQty;
                DISTRIBUTOR_DISCOUNT = (DISTRIBUTOR_DISCOUNT / qty) *returnQty;
                gstret = (gstret / qty) * returnQty;
                TST_AMOUNT = (TST_AMOUNT / qty) * returnQty;
                extratax = (extratax / qty) * returnQty;
                dtOrderDetail.Rows[item.RowIndex]["returnDiscount"] =string.Format("{0:0.00}",Discount);
                dtOrderDetail.Rows[item.RowIndex]["returnQty"] =  returnQty;
                dtOrderDetail.Rows[item.RowIndex]["returnAmount"] = price * returnQty;
                dtOrderDetail.Rows[item.RowIndex]["COST_AMOUNT"] = returnQty * Distributorprice; 
                dtOrderDetail.Rows[item.RowIndex]["returnGST"] = gstret;
                dtOrderDetail.Rows[item.RowIndex]["retExtraTax"] = extratax;
                dtOrderDetail.Rows[item.RowIndex]["RETEXTRA_DISCOUNT"] = EXTRA_DISCOUNT;
                dtOrderDetail.Rows[item.RowIndex]["RETDISTRIBUTOR_DISCOUNT"] = DISTRIBUTOR_DISCOUNT;
                dtOrderDetail.Rows[item.RowIndex]["RETSED_AMOUNT"] = string.Format("{0:0.00}", SED_AMOUNT);
                dtOrderDetail.Rows[item.RowIndex]["RETTST_AMOUNT"] = string.Format("{0:0.00}", TST_AMOUNT);
                dtOrderDetail.Rows[item.RowIndex]["RETFREE_QUANTITY"] = FREESKU;
                if (item.RowIndex + 1 < GrdPurchase.Rows.Count)
                {
                    GrdPurchase.Rows[item.RowIndex + 1].Cells[14].Focus();
                }
                else
                {
                  //  txtSaleRetFreight.Focus();
                }
            }
            else
            {

                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Return Qty should be less than Sale Qty!');", true);
                txtreturnQty.Text = "0.00";
                txtreturnQty.Focus();

            }
            Session["PurchaseSKU"] = dtOrderDetail;
            Calculate();
        }

    }
    protected void Calculate()
    {
        DataTable dt = new DataTable();

        decimal ObjGrossSales = 0;
        decimal ObjStandardDiscount = 0;
        decimal ObjTotalGST = 0;
        decimal ObjTotalTST = 0;
        decimal ObjFreeGSTAmount = 0;
        decimal objextratax = 0;
        decimal objEXTRA_DISCOUNT = 0;
        decimal objDISTRIBUTOR_DISCOUNT = 0;
        decimal ObjSED_AMOUNT = 0;
        decimal OBJTST_AMOUNT = 0;
        dt = (DataTable)Session["PurchaseSKU"];


        if (dt.Rows.Count > 0)
        {
            #region Reversecalulate Products


            ObjGrossSales = 0;

            foreach (DataRow dr in dt.Rows)
            {
                ObjGrossSales += decimal.Parse(_dc.chkNull_0(dr["returnAmount"].ToString()));
                ObjStandardDiscount += decimal.Parse(_dc.chkNull_0(dr["returnDiscount"].ToString()));
                ObjTotalGST += decimal.Parse(_dc.chkNull_0(dr["returnGST"].ToString()));
                objextratax += decimal.Parse(_dc.chkNull_0(dr["retExtraTax"].ToString())); 
                 objEXTRA_DISCOUNT += decimal.Parse(_dc.chkNull_0(dr["RETEXTRA_DISCOUNT"].ToString()));
                objDISTRIBUTOR_DISCOUNT += decimal.Parse(_dc.chkNull_0(dr["RETDISTRIBUTOR_DISCOUNT"].ToString()));
                ObjSED_AMOUNT += decimal.Parse(_dc.chkNull_0(dr["RETSED_AMOUNT"].ToString()));
                OBJTST_AMOUNT += decimal.Parse(_dc.chkNull_0(dr["RETTST_AMOUNT"].ToString()));

            }

            #endregion

        }

        #region Set Total Values

        txtGrossReturn.Text = Convert.ToString(Math.Round(ObjGrossSales, 2));
        
        //decimal ExtraDiscount = Math.Round((ObjGrossSales - ObjStandardDiscount) * (decimal.Parse(_dc.chkNull_0(txtExtraDiscountReturnAge.Text)) / 100), 2);
        decimal SrFreight = 0;
        if (ObjGrossSales != 0)
        {
            txtRETGSTamount.Text = Convert.ToString(Math.Round(ObjTotalGST, 2));
            txtRETadditionlTax.Text = Convert.ToString(Math.Round(objextratax, 2));

            txtDiscountReturn.Text = Convert.ToString(Math.Round(ObjStandardDiscount, 2));
            txtExtraDiscountReturn.Text = Convert.ToString(Math.Round(objEXTRA_DISCOUNT, 2));
            txtRETdistributorDiscountval.Text= Convert.ToString(Math.Round(objDISTRIBUTOR_DISCOUNT, 2));
            txtRETclamablediscountval.Text = Convert.ToString(Math.Round(ObjSED_AMOUNT, 2));
            txtRETTSTamount.Text = Convert.ToString(Math.Round(OBJTST_AMOUNT, 2));
            SrFreight = 0; //Convert.ToDecimal(_dc.chkNull_0(txtSaleRetFreight.Text));
            txtNetReturn.Text = Convert.ToString(Math.Round(ObjGrossSales - ObjStandardDiscount - objEXTRA_DISCOUNT - objDISTRIBUTOR_DISCOUNT- ObjSED_AMOUNT + ObjTotalGST + ObjFreeGSTAmount + ObjTotalTST + objextratax, 2));
            

            //if (rbFreightType.SelectedValue == "0")
            //{
            //    decimal balance = decimal.Parse(_dc.chkNull_0(txtNetReturn.Text)) - decimal.Parse(_dc.chkNull_0(txtSaleRetFreight.Text)) - decimal.Parse(_dc.chkNull_0(txtCashReturn.Text));
            //    txtBalanceReturn.Text = balance.ToString();
            //}
            //else
            {
                decimal balance = decimal.Parse(_dc.chkNull_0(txtNetReturn.Text)) + 0 - decimal.Parse(_dc.chkNull_0(txtCashReturn.Text));
                txtBalanceReturn.Text = balance.ToString();
            }
        }
        else
        {

            txtDiscountReturn.Text = Convert.ToString(Math.Round(ObjStandardDiscount, 2));
            txtExtraDiscountReturn.Text = Convert.ToString(Math.Round(objEXTRA_DISCOUNT, 2));
            SrFreight = 0;// Convert.ToDecimal(_dc.chkNull_0(txtSaleRetFreight.Text));
                          // numTxtTotalGST.Text = Convert.ToString(Math.Round(ObjTotalGST + ObjFreeGSTAmount, 2));

            txtRETTSTamount.Text = Convert.ToString(Math.Round(OBJTST_AMOUNT, 2)); txtRETdistributorDiscountval.Text = Convert.ToString(Math.Round(objDISTRIBUTOR_DISCOUNT, 2));
            txtNetReturn.Text = Convert.ToString(Math.Round(ObjGrossSales - ObjStandardDiscount - objEXTRA_DISCOUNT - objDISTRIBUTOR_DISCOUNT - ObjSED_AMOUNT + ObjTotalGST + ObjFreeGSTAmount + ObjTotalTST, 2));
            //if (rbFreightType.SelectedValue == "0")
            //{
            //    decimal balance = decimal.Parse(_dc.chkNull_0(txtNetReturn.Text)) - decimal.Parse(_dc.chkNull_0(txtSaleRetFreight.Text)) - decimal.Parse(_dc.chkNull_0(txtCashReturn.Text));
            //    txtBalanceReturn.Text = balance.ToString();
            //}
            //else
            {
                decimal balance = decimal.Parse(_dc.chkNull_0(txtNetReturn.Text)) + 0 - decimal.Parse(_dc.chkNull_0(txtCashReturn.Text));
                txtBalanceReturn.Text = balance.ToString();
            }
        }

        #endregion

    }
    #endregion

    #region Load Functions
    private void LoadDistributor()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDropDownList(this.drpDistributor, dt, 0, 2, true);
    }
    private void LoadPrincipal()
    {
        SKUPriceDetailController PController = new SKUPriceDetailController();
        DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, m_dt, 0, 1, true);
    }
    private void LoadDeliveryman()
    {
        if (drpDistributor.Items.Count > 0 )
        {
            SaleForceController mDController = new SaleForceController();
            DataTable m_dt = mDController.SelectSaleForceAssignedArea(int.Parse(drpDistributor.SelectedValue.ToString()), Constants.IntNullValue, int.Parse(this.Session["CompanyId"].ToString()));
            clsWebFormUtil.FillDropDownList(this.DrpDeliveryMan, m_dt, 0, 3, true);
        }
        else
        {
            DrpDeliveryMan.Items.Clear();
        }
    }
    private void LoadGird()
    {
        PurchaseSKU = (DataTable)Session["PurchaseSKU"];
        GrdPurchase.DataSource = PurchaseSKU;
        GrdPurchase.DataBind();
    }
    private void LoadDocumentDetail(long docId, int typeId)
    {
        try
        {
            PurchaseSKU = _or.GetDocumentDetail(docId, typeId, DateTime.Parse(Session["CurrentWorkDate"].ToString()));
            Session.Add("PurchaseSKU", PurchaseSKU);
            LoadGird();
        }
        catch (Exception eee)
        {
            eee.Message.ToString();
        }
    }
 #endregion



   protected void GrdPurchase_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtOrderDetail = (DataTable)Session["PurchaseSKU"];
        if (e.Row.RowIndex == 0)
        {   
            //Set the focus to control on the edited row
            e.Row.FindControl("txtreturnQty").Focus();
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TextBox txtreturnQty = (TextBox)e.Row.FindControl("txtreturnQty");
     
        }

    }

    #region Click OPerations

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearMasterAll();
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DataTable piDt = rptCustomerController.SelectCustomerInvoicePrintprincipal(long.Parse(txtInvoiceNo.Text.Trim()),int.Parse(drpDistributor.SelectedValue), 5, DateTime.Parse(Session["CurrentWorkDate"].ToString()), int.Parse(DrpPrincipal.SelectedValue), int.Parse(DrpDeliveryMan.SelectedValue));
        
        if (piDt == null) return;

        if (piDt.Rows.Count > 0)
        {

            DataRow[] foundRows = piDt.Select("SALE_INVOICE_ID  = '" + txtInvoiceNo.Text.Trim() + "'");

            if (foundRows.Length > 0)
            {

                hfCustomerId.Value = foundRows[0]["CUSTOMER_ID"].ToString();

                //DrpPayMode.SelectedValue = foundRows[0]["PAYMENT_MODE"].ToString();

                txtGrossAmount.Text = String.Format("{0:0.00}", foundRows[0]["TOTAL_AMOUNT"]);
                numtxtDiscount.Text = String.Format("{0:0.00}", foundRows[0]["EXTRA_DISCOUNT"]);
               

                numtxtTotalExtraDiscnt.Text = String.Format("{0:0.00}", foundRows[0]["DISCOUNT"]);
                numTxtTotalGST.Text = String.Format("{0:0.00}", foundRows[0]["GST"]);
                txtdistrubtrdiscval.Text = String.Format("{0:0.00}", foundRows[0]["DISTRIBUTOR_DISCOUNT"]);
                numTxtTotlAmnt.Text = String.Format("{0:0.00}", foundRows[0]["NET_AMOUNT"]);
                txtCashRecieved.Text = String.Format("{0:0.00}", foundRows[0]["CASH_RECIEVED"]);
                txtclaimablediscountval.Text= String.Format("{0:0.00}", foundRows[0]["CLAIMABLE_DISCOUNT"]);
                txttstamount.Text= String.Format("{0:0.00}", foundRows[0]["TST_AMOUNT"]); 
                txtadditionaltax.Text= String.Format("{0:0.00}", foundRows[0]["EXTRA_TAX"]); 
                lblCustomerName.Text=  foundRows[0]["CUSTOMER_NAME"].ToString(); 
                hfSaleForceId.Value = foundRows[0]["SALE_FORCE"].ToString();
                hfPrincipalId.Value= foundRows[0]["PRINCIPAL_ID"].ToString();
                hfAREA_ID.Value = foundRows[0]["AREA_ID"].ToString();
                hfTOWN_ID.Value = foundRows[0]["TOWN_ID"].ToString();

                hfAssignedDistributorId.Value= foundRows[0]["ASSIGNED_DISTRIBUTOR_ID"].ToString();
                // txtNarration.Text = foundRows[0]["REMARKS"].ToString();

                txtExtraDiscountReturn.Text = String.Format("{0:0.00}", foundRows[0]["EXTRA_DISCOUNT"]);
             
                try
                {
                    //lblInvoiceBalance.Text = String.Format("{0:0.00}", foundRows[0]["invoice_closing"]);
                   // lblinvoiceclosing.Text = "Invoice Balance :";
                    lblinvoicedate.Text = "Invoice Date :" + DateTime.Parse(foundRows[0]["document_date"].ToString()).ToString("dd-MMM-yyyy");
                }
                catch(Exception ex)
                {
                   // lblInvoiceBalance.Text = "0";
                }
                txtGrossReturn.Text = "";
                txtDiscountReturn.Text = "";
                txtExtraDiscountReturn.Text = "";
                txtNetReturn.Text = "";
                txtCashReturn.Text = "";
               // txtSaleRetFreight.Text = "";
                txtBalanceReturn.Text = "";
              //  txtNarration.Text = "";

                LoadDocumentDetail(Convert.ToInt64(txtInvoiceNo.Text.Trim()), 4);

                btnSaveOrder.ToolTip = "Update";



                if (int.Parse(DrpPayMode.SelectedValue) == 218)//Constants.CustomerCredit_Id=218
                {
                    
                }
                else
                {
                    txtBalance.Text = Convert.ToString(decimal.Parse(txtCashRecieved.Text) - (decimal.Parse(numTxtTotlAmnt.Text)));
                }
            }

            btnSaveOrder.Enabled = true;
        }
        else
        {

            GrdPurchase.DataSource = null;
            GrdPurchase.DataBind();

            btnSaveOrder.Enabled = false;

            ClearDetail();
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Invoice Not Found for Current Selections!');", true);
            
        }
        
    }
    private bool CheckPayType()
    {
        //if (DrpPayMode.SelectedValue == "214")
        //{
        //    if (Convert.ToDecimal(_dc.chkNull_0(txtBalance.Text)) < 0)
        //    {
        //        return false;
        //    }
        //}
        return true;
    }
    protected void btnSaveOrder_Click(object sender, EventArgs e)
    {
        try
        {
            if (CheckPayType())
            {

                PurchaseSKU = (DataTable)Session["PurchaseSKU"];

                if (PurchaseSKU.Rows.Count <= 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('No product added');", true);

                }
                //if (btnSaveOrder.Text == "Update")
                //{

                //    bool IsValidInsert = _or.Update_SaleReturn(long.Parse(hfInvoiceId.Value), int.Parse(Session["DISTRIBUTOR_ID"].ToString()), txtNarration.Text,
                //    long.Parse(hfCustomerId.Value), int.Parse(hfSaleForceId.Value), long.Parse(Session["srId"].ToString()),
                //     decimal.Parse(_dc.chkNull_0(txtGrossReturn.Text)), decimal.Parse(_dc.chkNull_0(txtExtraDiscountReturn.Text)),
                //     decimal.Parse(_dc.chkNull_0(txtDiscountReturn.Text)), decimal.Parse(_dc.chkNull_0(numTxtTotalGST.Text)),
                //     decimal.Parse(_dc.chkNull_0(txtNetReturn.Text)), decimal.Parse(_dc.chkNull_0(txtCashReturn.Text)), DrpPayMode.SelectedValue.ToString(), PurchaseSKU,
                //     int.Parse(Session["UserId"].ToString()), DateTime.Parse(Session["CurrentWorkDate"].ToString()), lblCustomerName.Text, decimal.Parse(_dc.chkNull_0(txtSaleRetFreight.Text)), int.Parse(rbFreightType.SelectedValue), decimal.Parse(_dc.chkNull_0(txtBalanceReturn.Text)));
                //    if (IsValidInsert)
                //    {
                //        GetInvoice(long.Parse(hfInvoiceId.Value));
                //        GetGatePass(long.Parse(hfInvoiceId.Value));
                //        ClearMasterAll();
                //    }
                //    else
                //    {
                //        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Insertion Failed!!!.');", true);
                //    }
                //}
                //else
                {


                    long IsValidInsert = _or.Add_SaleReturn(int.Parse(Session["DISTRIBUTOR_ID"].ToString()),"",
                        long.Parse(hfCustomerId.Value), int.Parse(hfSaleForceId.Value), long.Parse(txtInvoiceNo.Text.Trim()),
                    decimal.Parse(_dc.chkNull_0(txtGrossReturn.Text)), decimal.Parse(_dc.chkNull_0(txtExtraDiscountReturn.Text)),
                    decimal.Parse(_dc.chkNull_0(txtDiscountReturn.Text)), decimal.Parse(_dc.chkNull_0(txtRETGSTamount.Text)),
                    decimal.Parse(_dc.chkNull_0(txtNetReturn.Text)), decimal.Parse(_dc.chkNull_0(txtCashReturn.Text)), DrpPayMode.SelectedValue.ToString(), PurchaseSKU,
                    int.Parse(Session["UserId"].ToString()), DateTime.Parse(Session["CurrentWorkDate"].ToString()), lblCustomerName.Text,0,0,
                    decimal.Parse(_dc.chkNull_0(txtBalanceReturn.Text)),int.Parse(hfPrincipalId.Value),decimal.Parse(txtRETclamablediscountval.Text),decimal.Parse(txtRETdistributorDiscountval.Text),
                    decimal.Parse(txtRETadditionlTax.Text),int.Parse(hfAssignedDistributorId.Value),int.Parse(hfAREA_ID.Value),int.Parse(hfTOWN_ID.Value));
                    if (IsValidInsert > 0)
                    {

                      //  GetInvoice(IsValidInsert);
                        //GetGatePass(IsValidInsert);
                        ClearMasterAll();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Insertion Failed!!!.');", true);
                    }

                }


            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Balance should be zero');", true);
               
            }

        }
        catch (Exception eee)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('some error occurred');", true);
        }
    }

    #endregion
   
    #region PrintINvoice

    //protected void GetInvoice(long returnId)
    //{
    //    try
    //    {
    //        DocumentPrintController DPrint = new DocumentPrintController();

    //        DataTable dt = DPrint.SelectReportTitle(int.Parse(Session["DISTRIBUTOR_ID"].ToString()));

    //        RptSaleController rptSaleCtl = new RptSaleController();
    //        CrpSaleReturnDocument crpReport = new CrpSaleReturnDocument();
    //        DataSet ds = null;

    //        ds = rptSaleCtl.SelectSaleReport(returnId, 1);

    //        crpReport.SetDataSource(ds);
    //        crpReport.Refresh();

    //        crpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
    //        crpReport.SetParameterValue("CompanyAddress", dt.Rows[0]["ADDRESS1"].ToString());
    //        crpReport.SetParameterValue("ContactNo", dt.Rows[0]["CONTACT_NUMBER"].ToString());
    //        //

    //        CrpCustomerReturnDocument crpCustomer = new CrpCustomerReturnDocument();
    //        crpCustomer.SetDataSource(ds);
    //        crpCustomer.Refresh();

    //        crpCustomer.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
    //        crpCustomer.SetParameterValue("CompanyAddress", dt.Rows[0]["ADDRESS1"].ToString());
    //        crpCustomer.SetParameterValue("ContactNo", dt.Rows[0]["CONTACT_NUMBER"].ToString());
    //        //---------------------------------------------------------------------------------\\
    //        CrpFinanceReturnDocument crpFinance = new CrpFinanceReturnDocument();
    //        crpFinance.SetDataSource(ds);
    //        crpFinance.Refresh();

    //        crpFinance.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
    //        crpFinance.SetParameterValue("CompanyAddress", dt.Rows[0]["ADDRESS1"].ToString());
    //        crpFinance.SetParameterValue("ContactNo", dt.Rows[0]["CONTACT_NUMBER"].ToString());

    //        Session.Add("crpCustomer", crpCustomer);
    //        Session.Add("ReportTypeCustomer", 0);

    //        const string url2 = "'RptGatePassCustomer.aspx'";
    //        Response.Write("<script>window.open(" + url2 + ",'_blank');</script>");

    //        Session.Add("CrpReport", crpReport);
    //        Session.Add("ReportType", 0);

    //        const string url = "'MobileReports.aspx'";
    //        Response.Write("<script>window.open(" + url + ",'_blank');</script>");


    //        Session.Add("crpFinance", crpFinance);
    //        Session.Add("ReportTypeFinance", 0);

    //        const string url3 = "'RptGatePassFinance.aspx'";
    //        Response.Write("<script>window.open(" + url3 + ",'_blank');</script>");
    //    }
    //    catch (Exception ex)
    //    {
    //        ex.ToString();
    //    }
    //}
    //protected void GetGatePass(long returnId)
    //{
    //    try
    //    {
    //        DocumentPrintController DPrint = new DocumentPrintController();
    //        RptSaleController rptSaleCtl = new RptSaleController();

    //        DataTable dt = DPrint.SelectReportTitle(int.Parse(Session["DISTRIBUTOR_ID"].ToString()));


    //        DataSet ds = null;

    //        ds = rptSaleCtl.SelectSaleReport(returnId, 2);

    //        DataTable table = ds.Tables["spGetSaleReturnDocument"];
    //        if (table.Rows.Count <= 0) return;

    //        CrpGatePassReturn crpWarehouse = new CrpGatePassReturn();
    //        crpWarehouse.SetDataSource(ds);
    //        crpWarehouse.Refresh();

    //        crpWarehouse.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
    //        crpWarehouse.SetParameterValue("CompanyAddress", dt.Rows[0]["ADDRESS1"].ToString());
    //        crpWarehouse.SetParameterValue("ContactNo", dt.Rows[0]["CONTACT_NUMBER"].ToString());
    //        //---------------------------------------------------------------------------------\\

    //        //---------------------------------------------------------------------------------\\


    //        Session.Add("CrpReport2", crpWarehouse);
    //        Session.Add("ReportType2", 0);

    //        const string url = "'Default2.aspx'";
    //        Response.Write("<script>window.open(" + url + ",'_blank');</script>");



    //    }
    //    catch (Exception ex)
    //    {
    //        ex.ToString();
    //    }
    //}

   
    #endregion

    private void ClearMasterAll()
    {
        try
        {

            Session.Remove("PurchaseSKU");

            Server.Transfer("frmSaleReturnView.aspx?LevelType=3&LevelID=156");

        }
        catch (Exception eee)
        {
            eee.Message.ToString();
        }
    }
    private void ClearDetail()
    {
        txtGrossAmount.Text = "";
        numtxtTotalExtraDiscnt.Text = "";
      
        numtxtDiscount.Text = "";
        
        numTxtTotlAmnt.Text = "";
        txtCashRecieved.Text = "";
        txtBalance.Text = "";


        txtGrossReturn.Text = "";
        txtDiscountReturn.Text = "";
        txtExtraDiscountReturn.Text = "";
        txtNetReturn.Text = "";
        txtCashReturn.Text = "";
      //  txtSaleRetFreight.Text = "";
        txtBalanceReturn.Text = "";
      
    }

    protected void rbFreightType_SelectedIndexChanged(object sender, EventArgs e)
    {
        //if(rbFreightType.SelectedValue=="0")
        //{
        //    decimal balance = decimal.Parse(_dc.chkNull_0(txtNetReturn.Text)) - decimal.Parse(_dc.chkNull_0(txtSaleRetFreight.Text)) - decimal.Parse(_dc.chkNull_0(txtCashReturn.Text));
        //    txtBalanceReturn.Text = balance.ToString();
        //}
        //else
        {
            decimal balance = decimal.Parse(_dc.chkNull_0(txtNetReturn.Text)) +0- decimal.Parse(_dc.chkNull_0(txtCashReturn.Text));
            txtBalanceReturn.Text = balance.ToString();
        }
    }
}