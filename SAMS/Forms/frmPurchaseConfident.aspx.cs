using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Linq;

public partial class Forms_frmPurchaseConfident : System.Web.UI.Page
{
    private SKUPriceDetailController PController = new SKUPriceDetailController();
    private DataControl dc = new DataControl();
    private static int RowNo;
    private static int PrivouseQty, FreePrivousQty;
    private DataTable PurchaseSKU;
    UserController mUserController = new UserController();

    bool ViewAccess;
    protected void Page_Load(object sender, EventArgs e)
    {
        ViewAccess = ValidateScreen();
        if (ViewAccess == false)
        {
            Response.Redirect("../Forms/AccessDenied.aspx");
        }
        if (!Page.IsPostBack)
        {
            this.LoadPrincipal();
            this.LoadDistributor();
            this.LoadSKUDetail();
            this.CreatTable();
            this.GetDocumentNo();
            btnSave.Attributes.Add("onclick", "return ValidateForm();");
            txtNumTotalGST.Attributes.Add("ReadOnly", "ReadOnly");
            txtnumTotalnetAmount.Attributes.Add("ReadOnly", "ReadOnly");
            SAMSCommon.Classes.Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
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
    private void CreatTable()
    {
        PurchaseSKU = new DataTable();
        PurchaseSKU.Columns.Add("PURCHASE_DETAIL_ID", typeof(long));
        PurchaseSKU.Columns.Add("SKU_ID", typeof(int));
        PurchaseSKU.Columns.Add("SKU_Code", typeof(string));
        PurchaseSKU.Columns.Add("SKU_Name", typeof(string));

        PurchaseSKU.Columns.Add("BATCH_NO", typeof(string));
        PurchaseSKU.Columns.Add("PRICE", typeof(decimal));
        PurchaseSKU.Columns.Add("Quantity", typeof(int));

        PurchaseSKU.Columns.Add("QuantityUnit", typeof(int));
        PurchaseSKU.Columns.Add("UNITS_IN_CASE", typeof(int));
        PurchaseSKU.Columns.Add("Total_Unit", typeof(int));
        PurchaseSKU.Columns.Add("GST_RATE", typeof(decimal));
        PurchaseSKU.Columns.Add("GST_AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("TST_AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("TST_AMOUNT1", typeof(decimal));
        PurchaseSKU.Columns.Add("FREE_SKU", typeof(int));
        PurchaseSKU.Columns.Add("TDAMAGE", typeof(int));
        PurchaseSKU.Columns.Add("AMOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("AVG_PRICE", typeof(decimal));
        PurchaseSKU.Columns.Add("SKU_PRICE", typeof(decimal));
        
        PurchaseSKU.Columns.Add("PUR_DISCOUNT", typeof(decimal));
        PurchaseSKU.Columns.Add("GROSS_AMOUNT", typeof(decimal));
        this.Session.Add("PurchaseSKU", PurchaseSKU);

    }

    protected void DrpDocumentType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DrpDocumentType.SelectedValue == "2") //0 purchase
        {
            lbltoLocation.Text = "Principal";
            lblfromLocation.Text = "Purchase For";
            drpDistributor.Enabled = true;
            drpPrincipal.Enabled = true;
            this.DrpTransferFor.Visible = false;
            Label4.Visible = false;
            this.GetDocumentNo();
            ddlSKuCde_SelectedIndexChanged(null, null);
        }
        //else if (DrpDocumentType.SelectedValue == "5") //1 Transfer Out
        //{
        //    lblfromLocation.Text = "Transfer From";
        //    drpDistributor.Enabled = true;
        //    drpPrincipal.Enabled = true;
        //    LoadToDistributor();
        //    this.DrpTransferFor.Visible = true;
        //    Label4.Visible = true;
        //    Label4.Text = "Transfer To";
        //    this.GetDocumentNo();
        //}
        //else if (DrpDocumentType.SelectedValue == "3") //2 Purchase REturn
        //{
        //    lbltoLocation.Text = "Principal";
        //    lblfromLocation.Text = "Return From";
        //    drpDistributor.Enabled = true;
        //    drpPrincipal.Enabled = true;
        //    this.DrpTransferFor.Visible = false;
        //    Label4.Visible = false;
        //    this.GetDocumentNo();
        //}
        //else if (DrpDocumentType.SelectedValue == "4") //3 Transfer In
        //{
        //    lblfromLocation.Text = "Transfer to";
        //    drpDistributor.Enabled = true;
        //    drpPrincipal.Enabled = true;
        //    LoadToDistributor();
        //    this.DrpTransferFor.Visible = true;
        //    Label4.Visible = true;
        //    Label4.Text = "Transfer From";
        //    this.GetDocumentNo();
        //}
        //else if (DrpDocumentType.SelectedValue == "6") //4 Damage(Replace)
        //{
        //    lbltoLocation.Text = "Principal";
        //    lblfromLocation.Text = "Location";
        //    drpDistributor.Enabled = true;
        //    drpPrincipal.Enabled = true;
        //    this.DrpTransferFor.Visible = false;
        //    Label4.Visible = false;
        //    this.GetDocumentNo();
        //}
        //else if (DrpDocumentType.SelectedValue == "10") // Damage 
        //{
        //    lbltoLocation.Text = "Principal";
        //    lblfromLocation.Text = "Location";
        //    LoadDamageDetail();
        //    this.DrpTransferFor.Visible = true;
        //    Label4.Visible = true;
        //    Label4.Text = "Damage Detail";
        //    this.GetDocumentNo();
        //    //txtTransitDamage.Enabled = false;
        //    ChbFreeSKU.Enabled = false;
        //}
        //else
        //{
        //    lbltoLocation.Text = "Principal";
        //    lblfromLocation.Text = "Location";
        //    drpDistributor.Enabled = true;
        //    drpPrincipal.Enabled = true;
        //    this.DrpTransferFor.Visible = false;
        //    Label4.Visible = false;
        //    this.GetDocumentNo();
        //}
    }

    private void LoadDamageDetail()
    {
        DrpTransferFor.Items.Clear();
        DrpTransferFor.Items.Add(new ListItem("Market Damage", "0"));
        DrpTransferFor.Items.Add(new ListItem("Floor Damage", "1"));
        DrpTransferFor.Items.Add(new ListItem("Transit Damage (Normal Loss)", "2"));
        DrpTransferFor.Items.Add(new ListItem("Transit Damage (Abnormal Loss)", "3"));


    }

    private void GetDocumentNo()
    {
        drpDocumentNo.Items.Clear();
        DateTime MWorkDate = System.DateTime.Now;
        PurchaseController mPurchase = new PurchaseController();
        DataTable dt = mPurchase.SelectPurchaseDocumentNo(int.Parse(DrpDocumentType.SelectedValue.ToString()),
                                                          Constants.IntNullValue, Constants.LongNullValue,
                                                          int.Parse(this.Session["UserId"].ToString()), 0);
        drpDocumentNo.Items.Add(new clsListItems("New", Constants.LongNullValue.ToString()));
        clsWebFormUtil.FillDropDownList(this.drpDocumentNo, dt, 0, 0);
    }

    protected void drpDocumentNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        rbDiscount.SelectedValue = "1";
        rbDiscount_SelectedIndexChanged(null, null);
        if (drpDocumentNo.SelectedValue.ToString() == Constants.LongNullValue.ToString())
        {
            this.CreatTable();

            this.Session.Add("PurchaseSKU", PurchaseSKU);
            LoadGird();
            this.ClearAll();
            drpPrincipal.Enabled = true;
            drpDistributor.Enabled = true;
            DrpDocumentType.Enabled = true;
        }
        else
        {
            txtBuiltyNo.Text = "";
            txtDocumentNo.Text = "";
            drpPrincipal.Enabled = false;
            drpDistributor.Enabled = false;
            DrpDocumentType.Enabled = false;

            this.LoadDocumentDetail();
            this.LoadSKUDetail();
        }
    }

    private void LoadPrincipal()
    {
        DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue,
                                                     Constants.IntNullValue, Constants.IntNullValue,
                                                     Constants.IntNullValue,
                                                     int.Parse(this.Session["UserId"].ToString()),
                                                     Constants.IntNullValue, 0,
                                                     DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        clsWebFormUtil.FillDropDownList(this.drpPrincipal, m_dt, 0, 1, true);
    }

    protected void drpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadSKUDetail();
    }

    private void LoadDistributor()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue,
                                                         int.Parse(this.Session["UserId"].ToString()),
                                                         int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDropDownList(this.drpDistributor, dt, 0, 2, true);
    }

    private void LoadToDistributor()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, Constants.IntNullValue,
                                                         int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDropDownList(this.DrpTransferFor, dt, 0, 2, true);
    }

    private void LoadDocumentDetail()
    {
        int Distributor_ID;
        DateTime MWorkDate = System.DateTime.Now;
        PurchaseController mPurchase = new PurchaseController();
        DataTable dt = mPurchase.SelectPurchaseDocumentNo(Constants.IntNullValue, Constants.IntNullValue,
                                                          long.Parse(drpDocumentNo.SelectedValue.ToString()),
                                                          Constants.IntNullValue, Constants.IntNullValue);
        if (dt.Rows.Count > 0)
        {
            if (DrpDocumentType.SelectedIndex == 0 || DrpDocumentType.SelectedIndex == 4)
            {
                drpDistributor.SelectedValue = dt.Rows[0]["SOLD_TO"].ToString();
                drpPrincipal.SelectedValue = dt.Rows[0]["SOLD_FROM"].ToString();
                txtDocumentNo.Text = dt.Rows[0][2].ToString();
                txtBuiltyNo.Text = dt.Rows[0]["BUILTY_NO"].ToString();
                Distributor_ID = int.Parse(dt.Rows[0]["SOLD_TO"].ToString());
            }
            else if (DrpDocumentType.SelectedIndex == 1)
            {
                drpDistributor.SelectedValue = dt.Rows[0]["SOLD_FROM"].ToString();
                DrpTransferFor.SelectedValue = dt.Rows[0]["SOLD_TO"].ToString();
                txtDocumentNo.Text = dt.Rows[0][2].ToString();
                txtBuiltyNo.Text = dt.Rows[0]["BUILTY_NO"].ToString();
                Distributor_ID = int.Parse(dt.Rows[0]["SOLD_FROM"].ToString());
            }
            else if (DrpDocumentType.SelectedIndex == 2)
            {
                drpPrincipal.SelectedValue = dt.Rows[0]["SOLD_TO"].ToString();
                drpDistributor.SelectedValue = dt.Rows[0]["SOLD_FROM"].ToString();
                txtDocumentNo.Text = dt.Rows[0][2].ToString();
                txtBuiltyNo.Text = dt.Rows[0]["BUILTY_NO"].ToString();
                Distributor_ID = int.Parse(dt.Rows[0]["SOLD_FROM"].ToString());
            }
            else
            {
                DrpTransferFor.SelectedValue = dt.Rows[0]["SOLD_FROM"].ToString();
                drpDistributor.SelectedValue = dt.Rows[0]["SOLD_TO"].ToString();
                txtDocumentNo.Text = dt.Rows[0][2].ToString();
                txtBuiltyNo.Text = dt.Rows[0]["BUILTY_NO"].ToString();
                Distributor_ID = int.Parse(dt.Rows[0]["SOLD_TO"].ToString());
            }
            PurchaseSKU = mPurchase.SelectPurchaseDetailstock(Distributor_ID, long.Parse(dt.Rows[0][0].ToString()));
            this.Session.Add("PurchaseSKU", PurchaseSKU);
            LoadGird();
            btnCalculate.Enabled = true;
        }
    }

    private void LoadGird()
    {
        int TotalValue = 0;
        int TotalCtn = 0;
        int TotalTDamage = 0;
        int TotalFree = 0;

        decimal TOTALPUR_DISCOUNT = 0;
        decimal TotalNET_AMOUNT = 0;
        decimal TotalGrossAMOUNT = 0;
        decimal TotalGST = 0;
        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
        GrdPurchase.DataSource = PurchaseSKU;
        GrdPurchase.DataBind();
        foreach (DataRow dr in PurchaseSKU.Rows)
        {
            TotalValue += int.Parse(dr["QuantityUnit"].ToString());
            TotalCtn += int.Parse(dr["Quantity"].ToString());
            TotalTDamage += int.Parse(dr["TDAMAGE"].ToString());
            TotalFree += int.Parse(dr["FREE_SKU"].ToString());

            TotalGST += decimal.Parse(dr["GST_AMOUNT"].ToString()) + decimal.Parse(dr["TST_AMOUNT"].ToString());


            TOTALPUR_DISCOUNT += decimal.Parse(dc.chkNull_0(dr["PUR_DISCOUNT"].ToString()));
            TotalGrossAMOUNT += decimal.Parse(dr["GROSS_AMOUNT"].ToString());
            TotalNET_AMOUNT += decimal.Parse(dr["AMOUNT"].ToString());
        }
        txtTotalFree.Text = TotalFree.ToString();
        txtTotalQuantity.Text = TotalValue.ToString();
        txtTotalQuantityCtn.Text = TotalCtn.ToString();
        txtTotalTDamage.Text = TotalTDamage.ToString();


        txtDiscount.Text = Math.Round(TOTALPUR_DISCOUNT, 4).ToString();
        txtNetAmount.Text = Math.Round(TotalNET_AMOUNT, 4).ToString();
        txtGross.Text = Math.Round(TotalGrossAMOUNT, 4).ToString();

        txtNumTotalGST.Text = Math.Round(TotalGST, 4).ToString();
        txtnumTotalnetAmount.Text = Math.Round(TotalNET_AMOUNT, 4).ToString();
    }

    protected void GrdPurchase_RowEditing(object sender, GridViewEditEventArgs e)
    {
        HdSKU_Price.Value = "";
        RowNo = e.NewEditIndex;
        ddlSKuCde.SelectedValue = GrdPurchase.Rows[e.NewEditIndex].Cells[0].Text;
        btnSave.Text = "Update Sku";
        txtQuantity.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[3].Text;
        txtUnit.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[4].Text;
        txtPurchasePrice.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[5].Text;
        PrivouseQty = int.Parse(GrdPurchase.Rows[e.NewEditIndex].Cells[4].Text) + (int.Parse(GrdPurchase.Rows[e.NewEditIndex].Cells[3].Text) * int.Parse(GrdPurchase.Rows[e.NewEditIndex].Cells[10].Text));
        txtskuGross.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[6].Text;
        txtFreeSKU.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[7].Text;
        txtskuAmount.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[9].Text;
        HdSKU_Price.Value = GrdPurchase.Rows[e.NewEditIndex].Cells[11].Text;
        ddlSKuCde.Enabled = false;
        txtQuantity.Focus();
        btnSaveDocument.Enabled = false;
    }


    protected void GrdPurchase_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
        if (PurchaseSKU.Rows.Count > 0)
        {
            PurchaseSKU.Rows.RemoveAt(e.RowIndex);
            this.Session.Add("PurchaseSKU", PurchaseSKU);
            this.LoadGird();
        }
        ddlSKuCde_SelectedIndexChanged(null, null);
        btnSaveDocument.Enabled = false;

    }

    private void LoadSKUDetail()
    {
        if (drpPrincipal.Items.Count > 0)
        {
            DataTable Dtsku_Price = PController.SelectDataPrice(int.Parse(drpPrincipal.SelectedValue.ToString()),
                                                                Constants.IntNullValue, Constants.IntNullValue,
                                                                Constants.IntNullValue,
                                                                int.Parse(drpDistributor.SelectedValue.ToString()),
                                                                int.Parse(this.Session["UserId"].ToString()),
                                                                Constants.IntNullValue, 1,
                                                                DateTime.Parse(
                                                                    this.Session["CurrentWorkDate"].ToString()));
            clsWebFormUtil.FillDropDownList(ddlSKuCde, Dtsku_Price, 0, 21, true);
            this.Session.Add("Dtsku_Price", Dtsku_Price);
            ddlSKuCde_SelectedIndexChanged(null, null);
        }
    }

    protected void ChbBatchNo_CheckedChanged(object sender, EventArgs e)
    {
        if (ChbBatchNo.Checked == true)
        {
            //lblFreeSku.Enabled = true;
            txtBatchNo.Enabled = true;
        }
        else
        {
            txtBatchNo.Text = "N/A";
            // lblFreeSku.Enabled = false;
            txtBatchNo.Enabled = false;
        }
    }

    protected void ChbFreeSKU_CheckedChanged(object sender, EventArgs e)
    {
        if (ChbFreeSKU.Checked == true)
        {

            txtFreeSKU.Enabled = true;
        }
        else
        {
            txtFreeSKU.Text = "0";

            txtFreeSKU.Enabled = false;
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        btnSaveDocument.Enabled = false;
        DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
        DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + ddlSKuCde.SelectedValue + "'");
        if (foundRows.Length > 0)
        {
            decimal mStdDiscount = decimal.Parse(dc.chkNull_0(foundRows[0]["DISTRIBUTOR_DISCOUNT"].ToString()));
            decimal mGSTRate = decimal.Parse(dc.chkNull_0(foundRows[0]["GST_RATE_TP"].ToString()));
            int CurrentStock = CheckStockStatus(int.Parse(dc.chkNull_0(foundRows[0]["SKU_ID"].ToString())));

            if (btnSave.Text == "Add Sku")
            {
                if (CheckDublicateSKU())
                {
                    if (CurrentStock == -1)
                    {
                        DataRow dr = PurchaseSKU.NewRow();
                        dr["SKU_ID"] = foundRows[0]["SKU_ID"];
                        dr["SKU_Code"] = foundRows[0]["SKU_CODE"];
                        dr["SKU_Name"] = foundRows[0]["SKU_NAME"];
                        dr["UNITS_IN_CASE"] = foundRows[0]["UNITS_IN_CASE"];
                        dr["FREE_SKU"] = int.Parse(dc.chkNull_0(txtFreeSKU.Text));
                        dr["PRICE"] = decimal.Parse(dc.chkNull_0(txtPurchasePrice.Text));
                        dr["Quantity"] = int.Parse(dc.chkNull_0(txtQuantity.Text));
                        dr["QuantityUnit"] = int.Parse(dc.chkNull_0(txtUnit.Text));
                        dr["TDAMAGE"] = int.Parse(dc.chkNull_0(txtTransitDamage.Text));
                        decimal TotalUnit = (decimal.Parse(dr["QuantityUnit"].ToString())) +
                                            ((decimal.Parse(dr["Quantity"].ToString())) *
                                             (decimal.Parse(dr["UNITS_IN_CASE"].ToString())));
                        dr["Total_Unit"] = TotalUnit;
                        dr["GROSS_AMOUNT"] = decimal.Parse(dc.chkNull_0(txtPurchasePrice.Text)) * TotalUnit;
                        #region GST 
                        if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                        {
                            var Amount = decimal.Parse(dr["GROSS_AMOUNT"].ToString());
                            dr["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                            dr["GST_AMOUNT"] = Amount * mGSTRate / 100;
                            dr["TST_AMOUNT"] = 0;
                            dr["BATCH_NO"] = "T";

                        }
                        else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                        {
                            dr["TST_AMOUNT"] = mGSTRate * TotalUnit;
                            dr["GST_AMOUNT"] = 0;
                            dr["GST_RATE"] = 0;
                            dr["BATCH_NO"] = "R";

                        }
                        else
                        {
                            dr["GST_AMOUNT"] = 0;
                            dr["TST_AMOUNT"] = 0;
                            dr["GST_RATE"] = 0;
                            dr["BATCH_NO"] = "E";
                        }
                        if (!chbCalculateGST.Checked)
                        {
                            dr["GST_AMOUNT"] = 0;
                            dr["TST_AMOUNT"] = 0;
                        }
                        #endregion

                        dr["AMOUNT"] = decimal.Parse(dr["GROSS_AMOUNT"].ToString())
                            + decimal.Parse(dr["GST_AMOUNT"].ToString())
                            + decimal.Parse(dr["TST_AMOUNT"].ToString());
                        dr["TST_AMOUNT1"] = dr["TST_AMOUNT"];
                        dr["SKU_PRICE"] = decimal.Parse(foundRows[0]["SKU_PRICE"].ToString());
                        decimal Purchaseamount = decimal.Parse(dc.chkNull_0(txtPurchasePrice.Text)) * TotalUnit;
                        decimal Stockamount = decimal.Parse(foundRows[0]["CLOSING_STOCK"].ToString()) * decimal.Parse(foundRows[0]["SKU_PRICE"].ToString());
                        decimal totalamount = Purchaseamount + Stockamount;
                        decimal totalstock = decimal.Parse(foundRows[0]["CLOSING_STOCK"].ToString()) + TotalUnit;
                        dr["AVG_PRICE"] = Math.Round((totalamount / totalstock),2);
                        PurchaseSKU.Rows.Add(dr);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg",
                                                            "alert('  " + ddlSKuCde.SelectedItem.Text +
                                                            " Current closing Stock is " + CurrentStock.ToString() +
                                                            "');", true);
                        ddlSKuCde.SelectedValue = foundRows[0]["SKU_ID"].ToString();
                        return;
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg",
                                                        "alert('  " + ddlSKuCde.SelectedItem.Text +
                                                        " Already Exists ');", true);
                    ddlSKuCde.SelectedValue = foundRows[0]["SKU_ID"].ToString();
                    return;
                }
            }
            else if (btnSave.Text == "Update Sku")
            {
                if (CurrentStock == -1)
                {
                    DataRow dr = PurchaseSKU.Rows[RowNo];
                    dr["SKU_ID"] = foundRows[0]["SKU_ID"];
                    dr["SKU_Code"] = foundRows[0]["SKU_CODE"];
                    dr["SKU_Name"] = foundRows[0]["SKU_NAME"];
                    dr["UNITS_IN_CASE"] = foundRows[0]["UNITS_IN_CASE"];
                    dr["PRICE"] = decimal.Parse(dc.chkNull_0(txtPurchasePrice.Text));
                    dr["Quantity"] = int.Parse(txtQuantity.Text);
                    dr["QuantityUnit"] = int.Parse(dc.chkNull_0(txtUnit.Text));
                    dr["FREE_SKU"] = int.Parse(dc.chkNull_0(txtFreeSKU.Text));
                    dr["TDAMAGE"] = int.Parse(dc.chkNull_0(txtTransitDamage.Text));
                    decimal TotalUnit = (decimal.Parse(dr["QuantityUnit"].ToString())) +
                                        ((decimal.Parse(dr["Quantity"].ToString())) *
                                         (decimal.Parse(dr["UNITS_IN_CASE"].ToString())));
                    dr["Total_Unit"] = TotalUnit;
                    dr["GROSS_AMOUNT"] = decimal.Parse(dc.chkNull_0(txtPurchasePrice.Text)) * TotalUnit;
                    #region GST
                    if (foundRows[0]["GST_ON"].ToString().Trim() == "T")
                    {
                        var Amount = decimal.Parse(dr["GROSS_AMOUNT"].ToString());
                        dr["GST_RATE"] = foundRows[0]["GST_RATE_TP"];
                        dr["GST_AMOUNT"] = Amount * mGSTRate / 100;
                        dr["TST_AMOUNT"] = 0;
                        dr["BATCH_NO"] = "T";

                    }
                    else if (foundRows[0]["GST_ON"].ToString().Trim() == "R")
                    {
                        dr["TST_AMOUNT"] = mGSTRate * TotalUnit;
                        dr["GST_AMOUNT"] = 0;
                        dr["GST_RATE"] = 0;
                        dr["BATCH_NO"] = "R";

                    }
                    else
                    {
                        dr["GST_AMOUNT"] = 0;
                        dr["TST_AMOUNT"] = 0;
                        dr["GST_RATE"] = 0;
                        dr["BATCH_NO"] = "E";
                    }
                    if (!chbCalculateGST.Checked)
                    {
                        dr["GST_AMOUNT"] = 0;
                        dr["TST_AMOUNT"] = 0;
                    }
                    #endregion
                    dr["AMOUNT"] = decimal.Parse(dr["GROSS_AMOUNT"].ToString())
                             + decimal.Parse(dr["GST_AMOUNT"].ToString())
                             + decimal.Parse(dr["TST_AMOUNT"].ToString());

                    dr["TST_AMOUNT1"] = dr["TST_AMOUNT"];
                    decimal Purchaseamount = decimal.Parse(dc.chkNull_0(txtPurchasePrice.Text)) * TotalUnit;
                    decimal CurruntStock = decimal.Parse(dr["CLOSING_STOCK"].ToString()) - decimal.Parse(dr["PRE_QUANTITY"].ToString());
                    decimal Stockamount = CurruntStock * decimal.Parse(dr["SKU_PRICE"].ToString());
                    decimal totalamount = Purchaseamount + Stockamount;
                    decimal totalstock = CurruntStock + TotalUnit;
                    dr["AVG_PRICE"] = Math.Round((totalamount / totalstock), 2);
                } 
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg",
                                                        "alert('  " + ddlSKuCde.SelectedValue +
                                                        "Current closing Stock is " + CurrentStock.ToString() + "');",
                                                        true);
                    ddlSKuCde.SelectedValue = foundRows[0]["SKU_ID"].ToString();
                    return;
                }
            }
            this.Session.Add("PurchaseSKU", PurchaseSKU);
            this.ClearAll();
            this.LoadGird();
            DisAbaleOption(true);
            ddlSKuCde_SelectedIndexChanged(null, null);
            btnCalculate.Enabled = true;
            ScriptManager.GetCurrent(Page).SetFocus(ddlSKuCde);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Wrong SKU Select');", true);

        }
    }


    protected void btnSaveDocument_Click(object sender, EventArgs e)
    {
        SAMSCommon.Classes.Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];

        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
        if (PurchaseSKU.Rows.Count > 0)
        {
            DistributorController mDayClose = new DistributorController();
            DataTable dt = mDayClose.SelectMaxDayClose(Constants.IntNullValue,
                                                       int.Parse(drpDistributor.SelectedValue.ToString()));
            if (dt.Rows.Count > 0)
            {
                if (IsDayClosed())
                {
                    UserController UserCtl = new UserController();

                    UserCtl.InsertUserLogoutTime(Convert.ToInt32(Session["User_Log_ID"]),
                                                 Convert.ToInt32(Session["UserID"]));
                    this.Session.Clear();
                    System.Web.Security.FormsAuthentication.SignOut();
                    Response.Redirect("../Login.aspx");
                }
                else
                {

                    if (CalculatePurchase(DateTime.Parse(dt.Rows[0]["CLOSING_DATE"].ToString())))
                    {
                        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
                        PurchaseSKU.Rows.Clear();
                        this.Session.Add("PurchaseSKU", PurchaseSKU);
                        this.LoadGird();
                        this.GetDocumentNo();
                        drpDistributor.Enabled = true;
                        drpPrincipal.Enabled = true;
                        DrpDocumentType.Enabled = true;
                        this.ClearAll();
                        txtBuiltyNo.Text = "";
                        txtDocumentNo.Text = "";
                        txtDocumentNo.Text = "";
                        DisAbaleOption(false);
                    }
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Wrong Location');", true);
            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert(' At least one SKU must enter');",
                                                true);

        }
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        DisAbaleOption(false);
        this.CreatTable();
        this.LoadGird();
        this.ClearAll();
        txtDocumentNo.Text = "";
        txtBuiltyNo.Text = "";
        txtDocumentNo.Text = "";
    }


    private bool CheckDublicateSKU()
    {
        DataControl dc = new DataControl();
        DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
        DataRow[] foundRows = PurchaseSKU.Select("SKU_ID  = '" + ddlSKuCde.SelectedValue + "'");
        if (foundRows.Length == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private bool CalculatePurchase(DateTime MWorkDate)
    {
        string Voucher_No;
        decimal mTotalAmount = 0;
        decimal mTotalPUR_DIS = 0;
        decimal mTotalGST_AMOUNT = 0;
        decimal mTotalTST_AMOUNT = 0;

        PurchaseController mController = new PurchaseController();
        DataTable dtPurchaseDetail = (DataTable)this.Session["PurchaseSKU"];
        foreach (DataRow dr in dtPurchaseDetail.Rows)
        {
            mTotalAmount += decimal.Parse(dr["AMOUNT"].ToString());

            mTotalPUR_DIS += decimal.Parse(dr["PUR_DISCOUNT"].ToString());
            mTotalGST_AMOUNT += decimal.Parse(dr["GST_AMOUNT"].ToString());
            mTotalTST_AMOUNT += decimal.Parse(dr["TST_AMOUNT"].ToString());


        }
        if (int.Parse(DrpDocumentType.SelectedValue) == 2)// Purchase
        {
            if (drpDocumentNo.SelectedValue.ToString() == Constants.LongNullValue.ToString())
            {
                bool mResult =
                    mController.InsertPurchaseforConfident(int.Parse(drpDistributor.SelectedValue.ToString()),
                                                            txtDocumentNo.Text,
                                                            int.Parse(DrpDocumentType.SelectedValue.ToString())
                                                            , MWorkDate,
                                                            int.Parse(drpDistributor.SelectedValue.ToString()),
                                                            int.Parse(drpPrincipal.SelectedValue.ToString()),
                                                            mTotalAmount, false, dtPurchaseDetail, 0, txtBuiltyNo.Text,
                                                            int.Parse(this.Session["UserId"].ToString()),
                                                            int.Parse(drpPrincipal.SelectedValue.ToString()), 0, 0, mTotalPUR_DIS, mTotalGST_AMOUNT, mTotalGST_AMOUNT, mTotalAmount, out Voucher_No, "", drpPrincipal.SelectedItem.Text);
                return mResult;
            }
            else
            {
                bool mResult = mController.UpdatePurchaseforConfodent(
                    int.Parse(drpDocumentNo.SelectedValue.ToString()),
                    int.Parse(drpDistributor.SelectedValue.ToString()), txtDocumentNo.Text,
                    int.Parse(DrpDocumentType.SelectedValue.ToString())
                    , MWorkDate, int.Parse(drpDistributor.SelectedValue.ToString()),
                    int.Parse(drpPrincipal.SelectedValue.ToString()), mTotalAmount, false, dtPurchaseDetail, 0,
                    txtBuiltyNo.Text, int.Parse(this.Session["UserId"].ToString()),
                    int.Parse(drpPrincipal.SelectedValue.ToString()), 0, 0, mTotalPUR_DIS, mTotalGST_AMOUNT, mTotalGST_AMOUNT, mTotalAmount, out Voucher_No, "", drpPrincipal.SelectedItem.Text);
                return mResult;
            }
        }
        return false;
        #region Old
        //else if (DrpDocumentType.SelectedIndex == 1)
        //{
        //    if (drpDocumentNo.SelectedValue.ToString() == Constants.LongNullValue.ToString())
        //    {
        //        bool mResult =
        //            mController.InsertPurchaseDocumentStock(int.Parse(drpDistributor.SelectedValue.ToString()),
        //                                                    txtDocumentNo.Text,
        //                                                    int.Parse(DrpDocumentType.SelectedValue.ToString())
        //                                                    , MWorkDate,
        //                                                    int.Parse(DrpTransferFor.SelectedValue.ToString()),
        //                                                    int.Parse(drpDistributor.SelectedValue.ToString())
        //                                                    , mTotalAmount, false, dtPurchaseDetail, 0, txtBuiltyNo.Text,
        //                                                    int.Parse(this.Session["UserId"].ToString()),
        //                                                    int.Parse(drpPrincipal.SelectedValue.ToString()));
        //        return mResult;
        //    }
        //    else
        //    {
        //        bool mResult = mController.UpdatePurchaseDocumentStock(
        //            int.Parse(drpDocumentNo.SelectedValue.ToString()),
        //            int.Parse(drpDistributor.SelectedValue.ToString()), txtDocumentNo.Text,
        //            int.Parse(DrpDocumentType.SelectedValue.ToString())
        //            , MWorkDate, int.Parse(DrpTransferFor.SelectedValue.ToString()),
        //            int.Parse(drpDistributor.SelectedValue.ToString())
        //            , mTotalAmount, false, dtPurchaseDetail, 0, txtBuiltyNo.Text,
        //            int.Parse(this.Session["UserId"].ToString()), int.Parse(drpPrincipal.SelectedValue.ToString()));
        //        return mResult;
        //    }
        //}
        //else if (DrpDocumentType.SelectedIndex == 2)
        //{
        //    if (drpDocumentNo.SelectedValue.ToString() == Constants.LongNullValue.ToString())
        //    {
        //        bool mResult =
        //            mController.InsertPurchaseDocumentStock(int.Parse(drpDistributor.SelectedValue.ToString()),
        //                                                    txtDocumentNo.Text,
        //                                                    int.Parse(DrpDocumentType.SelectedValue.ToString())
        //                                                    , MWorkDate,
        //                                                    int.Parse(drpPrincipal.SelectedValue.ToString()),
        //                                                    int.Parse(drpDistributor.SelectedValue.ToString())
        //                                                    , mTotalAmount, false, dtPurchaseDetail, 0, txtBuiltyNo.Text,
        //                                                    int.Parse(this.Session["UserId"].ToString()),
        //                                                    int.Parse(drpPrincipal.SelectedValue.ToString()));
        //        return mResult;
        //    }
        //    else
        //    {
        //        bool mResult = mController.UpdatePurchaseDocumentStock(
        //            int.Parse(drpDocumentNo.SelectedValue.ToString()),
        //            int.Parse(drpDistributor.SelectedValue.ToString()), txtDocumentNo.Text,
        //            int.Parse(DrpDocumentType.SelectedValue.ToString())
        //            , MWorkDate, int.Parse(drpPrincipal.SelectedValue.ToString()),
        //            int.Parse(drpDistributor.SelectedValue.ToString())
        //            , mTotalAmount, false, dtPurchaseDetail, 0, txtBuiltyNo.Text,
        //            int.Parse(this.Session["UserId"].ToString()), int.Parse(drpPrincipal.SelectedValue.ToString()));
        //        return mResult;
        //    }
        //}
        //else if (DrpDocumentType.SelectedIndex == 3)
        //{
        //    if (drpDocumentNo.SelectedValue.ToString() == Constants.LongNullValue.ToString())
        //    {
        //        bool mResult =
        //            mController.InsertPurchaseDocumentStock(int.Parse(drpDistributor.SelectedValue.ToString()),
        //                                                    txtDocumentNo.Text,
        //                                                    int.Parse(DrpDocumentType.SelectedValue.ToString())
        //                                                    , MWorkDate,
        //                                                    int.Parse(drpDistributor.SelectedValue.ToString()),
        //                                                    int.Parse(DrpTransferFor.SelectedValue.ToString())
        //                                                    , mTotalAmount, false, dtPurchaseDetail, 0, txtBuiltyNo.Text,
        //                                                    int.Parse(this.Session["UserId"].ToString()),
        //                                                    int.Parse(drpPrincipal.SelectedValue.ToString()));
        //        return mResult;
        //    }
        //    else
        //    {
        //        bool mResult = mController.UpdatePurchaseDocumentStock(
        //            int.Parse(drpDocumentNo.SelectedValue.ToString()),
        //            int.Parse(drpDistributor.SelectedValue.ToString()), txtDocumentNo.Text,
        //            int.Parse(DrpDocumentType.SelectedValue.ToString())
        //            , MWorkDate, int.Parse(drpDistributor.SelectedValue.ToString()),
        //            int.Parse(DrpTransferFor.SelectedValue.ToString())
        //            , mTotalAmount, false, dtPurchaseDetail, 0, txtBuiltyNo.Text,
        //            int.Parse(this.Session["UserId"].ToString()), int.Parse(drpPrincipal.SelectedValue.ToString()));
        //        return mResult;
        //    }
        //}
        //else if (DrpDocumentType.SelectedIndex == 4)
        //{
        //    if (drpDocumentNo.SelectedValue.ToString() == Constants.LongNullValue.ToString())
        //    {
        //        bool mResult =
        //            mController.InsertPurchaseDocumentStock(int.Parse(drpDistributor.SelectedValue.ToString()),
        //                                                    txtDocumentNo.Text,
        //                                                    int.Parse(DrpDocumentType.SelectedValue.ToString())
        //                                                    , MWorkDate,
        //                                                    int.Parse(drpDistributor.SelectedValue.ToString()),
        //                                                    int.Parse(drpPrincipal.SelectedValue.ToString())
        //                                                    , mTotalAmount, false, dtPurchaseDetail, 0, txtBuiltyNo.Text,
        //                                                    int.Parse(this.Session["UserId"].ToString()),
        //                                                    int.Parse(drpPrincipal.SelectedValue.ToString()));
        //        return mResult;
        //    }
        //    else
        //    {
        //        bool mResult = mController.UpdatePurchaseDocumentStock(
        //            int.Parse(drpDocumentNo.SelectedValue.ToString()),
        //            int.Parse(drpDistributor.SelectedValue.ToString()), txtDocumentNo.Text,
        //            int.Parse(DrpDocumentType.SelectedValue.ToString())
        //            , MWorkDate, int.Parse(drpDistributor.SelectedValue.ToString()),
        //            int.Parse(drpPrincipal.SelectedValue.ToString())
        //            , mTotalAmount, false, dtPurchaseDetail, 0, txtBuiltyNo.Text,
        //            int.Parse(this.Session["UserId"].ToString()), int.Parse(drpPrincipal.SelectedValue.ToString()));
        //        return mResult;
        //    }
        //}
        //else //For Damage Stock Return
        //{
        //    if (drpDocumentNo.SelectedValue.ToString() == Constants.LongNullValue.ToString())
        //    {
        //        bool mResult =
        //            mController.InsertPurchaseDocumentStock(int.Parse(drpDistributor.SelectedValue.ToString()),
        //                                                    txtDocumentNo.Text,
        //                                                    int.Parse(DrpDocumentType.SelectedValue.ToString())
        //                                                    , MWorkDate,
        //                                                    int.Parse(drpDistributor.SelectedValue.ToString()),
        //                                                    int.Parse(drpPrincipal.SelectedValue.ToString())
        //                                                    , mTotalAmount, false, dtPurchaseDetail, 0, txtBuiltyNo.Text,
        //                                                    int.Parse(this.Session["UserId"].ToString()),
        //                                                    int.Parse(drpPrincipal.SelectedValue.ToString()),
        //                                                    int.Parse(DrpTransferFor.SelectedValue));
        //        return mResult;
        //    }
        //    else
        //    {
        //        bool mResult = mController.UpdatePurchaseDocumentStock(
        //            int.Parse(drpDocumentNo.SelectedValue.ToString()),
        //            int.Parse(drpDistributor.SelectedValue.ToString()), txtDocumentNo.Text,
        //            int.Parse(DrpDocumentType.SelectedValue.ToString())
        //            , MWorkDate, int.Parse(drpDistributor.SelectedValue.ToString()),
        //            int.Parse(drpPrincipal.SelectedValue.ToString())
        //            , mTotalAmount, false, dtPurchaseDetail, 0, txtBuiltyNo.Text,
        //            int.Parse(this.Session["UserId"].ToString()), int.Parse(drpPrincipal.SelectedValue.ToString()),
        //            int.Parse(DrpTransferFor.SelectedValue));
        //        return mResult;
        //    }
        //}
        #endregion
    }


    private int CheckStockStatus(int SKU_ID)
    {
        if (DrpDocumentType.SelectedIndex == 0 || DrpDocumentType.SelectedIndex == 3)
        {
            return -1;
        }
        else
        {
            PhaysicalStockController mController = new PhaysicalStockController();
            DataTable dt = mController.SelectSKUClosingStock(int.Parse(drpDistributor.SelectedValue.ToString()), SKU_ID,
                                                             txtBatchNo.Text,
                                                             DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
            if (dt.Rows.Count > 0)
            {
                if (int.Parse(dt.Rows[0][0].ToString()) + PrivouseQty + FreePrivousQty >= int.Parse(txtQuantity.Text))
                {
                    return -1;
                }
                else
                {
                    return int.Parse(dt.Rows[0][0].ToString()) + PrivouseQty + FreePrivousQty;
                }
            }
        }

        return 0;
    }

    private void DisAbaleOption(bool IsDisable)
    {
        if (IsDisable == true)
        {
            DrpDocumentType.Enabled = false;
            drpPrincipal.Enabled = false;
            drpDistributor.Enabled = false;
            drpDocumentNo.Enabled = false;
        }
        else
        {
            DrpDocumentType.Enabled = true;
            drpPrincipal.Enabled = true;
            drpDistributor.Enabled = true;
            drpDocumentNo.Enabled = true;
            drpDocumentNo.SelectedIndex = 0;
        }
    }


    private void ClearAll()
    {
        ddlSKuCde.SelectedIndex = 0;
        txtQuantity.Text = "";
        txtFreeSKU.Text = "0";
        txtBatchNo.Text = "N/A";
        txtDisPer.Text = "";
        txtDiscount.Text = "";
        txtskuGross.Text = "";
        txtskuAmount.Text = "";

        txtUnit.Text = "";
        ddlSKuCde.Enabled = true;
        btnSave.Text = "Add Sku";
        PrivouseQty = 0;
        FreePrivousQty = 0;
        txtTransitDamage.Text = "";
        txtPurchasePrice.Text = "";
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

    protected void rbDiscount_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rbDiscount.SelectedValue == "0")
        {
            txtDisPer.Enabled = true;
            txtDiscount.Enabled = false;

        }
        else
        {
            txtDisPer.Enabled = false;
            txtDiscount.Enabled = true;
        }
    }

    protected void btnCalculate_Click(object sender, EventArgs e)
    {
        decimal ProductStdDist = 0;
        decimal ObjGrossSales = 0;
        DataTable dt = (DataTable)this.Session["PurchaseSKU"];
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                dr["PUR_DISCOUNT"] = 0;
                ObjGrossSales += decimal.Parse(dc.chkNull_0(dr["GROSS_AMOUNT"].ToString()));
            }


            if (rbDiscount.SelectedValue == "0")
            {
                if (decimal.Parse(dc.chkNull_0(txtDisPer.Text)) > 0)
                {
                    ProductStdDist = decimal.Parse(dc.chkNull_0(txtDisPer.Text)) / 100;
                }
            }
            else
            {
                if (decimal.Parse(dc.chkNull_0(txtDiscount.Text)) > 0)
                {
                    ProductStdDist = decimal.Parse(dc.chkNull_0(txtDiscount.Text)) / (ObjGrossSales);
                }
            }
            foreach (DataRow dr in dt.Rows)
            {
                dr["PUR_DISCOUNT"] = ProductStdDist * decimal.Parse(dc.chkNull_0(dr["GROSS_AMOUNT"].ToString()));


                #region GST
                if (chbCalculateGST.Checked)
                {
                    dr["GST_AMOUNT"] = (ObjGrossSales) * decimal.Parse(dc.chkNull_0(dr["GST_RATE"].ToString())) / 100;
                    dr["TST_AMOUNT"] = dr["TST_AMOUNT1"];
                }
                else
                {
                    dr["GST_AMOUNT"] = 0;
                    dr["TST_AMOUNT"] = 0;
                }
                #endregion


                dr["AMOUNT"] = decimal.Parse(dr["GROSS_AMOUNT"].ToString())
                           + decimal.Parse(dr["GST_AMOUNT"].ToString())
                           + decimal.Parse(dr["TST_AMOUNT"].ToString())
                           - decimal.Parse(dr["PUR_DISCOUNT"].ToString());

            }

            this.Session.Add("PurchaseSKU", dt);
            LoadGird();
            btnSaveDocument.Enabled = true;


        }
    }

    protected void txtDisPer_TextChanged(object sender, EventArgs e)
    {
        btnSaveDocument.Enabled = false;
        btnCalculate.Enabled = true;
    }

    protected void txtDiscount_TextChanged(object sender, EventArgs e)
    {
        btnSaveDocument.Enabled = false;
        btnCalculate.Enabled = true;
    }

    protected void ddlSKuCde_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataTable dtskuPrice = (DataTable)Session["Dtsku_Price"];
        DataRow[] foundRows = dtskuPrice.Select("SKU_ID  = '" + ddlSKuCde.SelectedValue + "'");
        if (foundRows.Length > 0)
        {
            decimal Price = decimal.Parse(foundRows[0]["SKU_PRICE"].ToString());
            txtPurchasePrice.Text = Math.Round(Price, 2).ToString();
        }
        else
        {
            txtPurchasePrice.Text = "0.00";
        }

        DataTable dtAppSetting = (DataTable)Session["dtAppSetting"];
        if (dtAppSetting != null)
        {
            DataRow[] drAppSetting = dtAppSetting.Select("strColumnName='ManualPurchasePrice'");
            if (drAppSetting.Length > 0)
            {
                if (drAppSetting[0]["strColumnValue"].ToString() == "1")
                {
                    txtPurchasePrice.Text = "";
                }
            }
        }
    }
}