using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Linq;

public partial class Forms_frmPysicalStockteken : System.Web.UI.Page
{
    DataTable PurchaseSKU;
    DataControl dc = new DataControl();
    private static int RowNo;

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
            this.LoadGird();
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
    /// Loads Principals To Principal Combo
    /// </summary>
    private void LoadPrincipal()
    {
        SKUPriceDetailController PController = new SKUPriceDetailController();
        DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        clsWebFormUtil.FillDropDownList(this.drpPrincipal, m_dt, 0, 1, true);
    }

    /// <summary>
    /// Loads Document Detail To Document Detail Grid And SKU Detail To ListBox
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void drpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadGird();
        this.LoadSKUDetail();
    }

    /// <summary>
    /// Loads SKU Detail To ListBox
    /// </summary>
    private void LoadSKUDetail()
    {
        if (drpPrincipal.Items.Count > 0)
        {
            SKUPriceDetailController PController = new SKUPriceDetailController();
            DataTable Dtsku_Price = PController.SelectDataPrice4(int.Parse(drpPrincipal.SelectedValue.ToString()), Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["DISTRIBUTOR_ID"].ToString()), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 1, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
            //  clsWebFormUtil.FillListBox(this.lstCode, Dtsku_Price, 10, 10, true);
            clsWebFormUtil.FillDropDownList(drpSKudetail,Dtsku_Price,0,10,true);
            this.Session.Add("Dtsku_Price", Dtsku_Price);
            
        }
    }

    /// <summary>
    ///  Loads Document Detail To Document Detail Grid
    /// </summary>
    private void LoadGird()
    {
        PhaysicalStockController MController = new PhaysicalStockController();
        DataTable dt = MController.SelectPysicalStock2(int.Parse(drpDistributor.SelectedValue.ToString()),0,int.Parse(drpPrincipal.SelectedValue.ToString()));
        GrdPurchase.DataSource = dt;
        GrdPurchase.DataBind();  
    }

    /// <summary>
    /// Deletes A Document Detail
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void GrdPurchase_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        PhaysicalStockController MController = new PhaysicalStockController();
        MController.DELETEPysicalStock(int.Parse(drpDistributor.SelectedValue.ToString()), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), int.Parse(GrdPurchase.Rows[e.RowIndex].Cells[0].Text));
        this.LoadGird();
    }

    /// <summary>
    /// Sets Document Detail Data For Edit. This Function Runs When An Existing Document Detail Needs To Be Edited
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void GrdPurchase_RowEditing(object sender, GridViewEditEventArgs e)
    {
        RowNo = e.NewEditIndex;
        //txtskuCode.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[1].Text;
       // txtskuName.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[2].Text;
       drpSKudetail.SelectedValue= GrdPurchase.Rows[e.NewEditIndex].Cells[0].Text;
        txtQuantityCTN .Text = GrdPurchase.Rows[e.NewEditIndex].Cells[3].Text;

        txtQuantityUnit.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[4].Text;
        txtQuantityUnit.Text = Convert .ToInt32 (decimal .Parse (txtQuantityUnit.Text)).ToString ();
       
        txtusaleableqtyCTN.Text = (GrdPurchase.Rows[e.NewEditIndex].Cells[5].Text);
       
        txtusaleableqtyUnit.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[6].Text;
        txtusaleableqtyUnit.Text =Convert .ToInt32  (decimal.Parse(txtusaleableqtyUnit.Text)).ToString();

        txtUnitRate.Text = GrdPurchase.Rows[e.NewEditIndex].Cells[7].Text;
        HdnUniInCase.Value = GrdPurchase.Rows[e.NewEditIndex].Cells[8].Text;
        txtQuantityCTN.Focus();
        btnSave.Text = "Update Sku";
    }
        
    /// <summary>
    /// Checks Duplicate SKU in Document Detail Grid
    /// </summary>
    /// <returns></returns>
    private bool CheckDublicateSKU()
    {
        DataControl dc = new DataControl();
        DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
        DataRow[] foundRows = PurchaseSKU.Select("SKU_ID  = '" + drpSKudetail.SelectedValue + "'");
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
    /// Saves/Updates Document
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EvemtArgs</param>
    protected void btnSave_Click(object sender, EventArgs e)
    
    {
        DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
        DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + drpSKudetail.SelectedValue + "'");
        
        DataControl dc = new DataControl();
 
        if (foundRows.Length > 0)
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


                PhaysicalStockController MController = new PhaysicalStockController();
                int TotalSaleQtyUnit = 0;
                int TotalUnsaleQtuUnit = 0;
                TotalSaleQtyUnit = (int.Parse(dc.chkNull_0(txtQuantityCTN.Text)) * int.Parse(HdnUniInCase.Value)) + int.Parse(dc.chkNull_0(txtQuantityUnit.Text));
                TotalUnsaleQtuUnit = (int.Parse(dc.chkNull_0(txtusaleableqtyCTN.Text)) * int.Parse(HdnUniInCase.Value)) + int.Parse(dc.chkNull_0(txtusaleableqtyUnit.Text));
                if (btnSave.Text == "Save")
                {
                    // by safdar  add CTN Qty

                    MController.InsertPysicalStock(int.Parse(drpDistributor.SelectedValue.ToString()), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), int.Parse(foundRows[0]["SKU_ID"].ToString()), TotalSaleQtyUnit, TotalUnsaleQtuUnit, decimal.Parse(dc.chkNull_0(txtUnitRate.Text)), 0, int.Parse(drpPrincipal.SelectedValue.ToString()));
                }
                else
                {
                    MController.UpdatePysicalStock(int.Parse(drpDistributor.SelectedValue.ToString()), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), int.Parse(foundRows[0]["SKU_ID"].ToString()), TotalSaleQtyUnit, TotalUnsaleQtuUnit, decimal.Parse(dc.chkNull_0(txtUnitRate.Text)), 0, int.Parse(drpPrincipal.SelectedValue.ToString()));

                }
                this.LoadGird();
                this.ClearAll();
                ScriptManager.GetCurrent(Page).SetFocus(drpSKudetail.SelectedValue);
            }
        }
    }

    /// <summary>
    /// Clears Form Controls
    /// </summary>
    private void ClearAll()
    {
       // txtskuCode.Text = "";
      //  txtskuName.Text = "";
        txtQuantityCTN.Text = "";
        txtQuantityUnit.Text = "";
        txtusaleableqtyCTN.Text = "";
        txtusaleableqtyUnit.Text = "";
        txtUnitRate.Text = "0";
       // txtskuCode.Enabled = true;
        btnSave.Text = "Save";
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

    protected void drpSKudetail_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        PurchaseSKU = (DataTable)this.Session["PurchaseSKU"];
        DataRow[] foundRows = Dtsku_Price.Select("SKU_ID  = '" + drpSKudetail.SelectedValue + "'");

        if (foundRows.Length > 0)
        {
            txtUnitRate.Text = (foundRows[0]["TRADE_PRICE"].ToString());
            HdnUniInCase.Value = (foundRows[0]["UNITS_IN_CASE"].ToString());
        }
    }
}
