using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SAMSCommon.Classes;
using SAMSBusinessLayer.Classes;
using System.Linq;

/// <summary>
/// From To Add, Edit SKU Price
/// </summary>
public partial class SKU_Price : System.Web.UI.Page
{
    /// <summary>
    /// Page_Load Function Populates All Combos And Grid On The Page
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    bool ViewAccess;
    protected void Page_Load(object sender, EventArgs e)
    {
        //ViewAccess = ValidateScreen();
        //if (ViewAccess == false)
        //{
        //    Response.Redirect("../Forms/AccessDenied.aspx");
        //}
        if (!Page.IsPostBack)
        {
            Populate_drpSKUCompany();
            Populate_drpSKUDivisions();
            Populate_drpSKUCategory();
            Populate_drpSKUSubCategory();
            Populate_drpSKUBrand();
            LoadSKUS();
            LoadGrid();
            LoadDistributor();
            btnSave.Attributes.Add("onclick", "return ValidateForm()");
        }
    }
    //public bool ValidateScreen()
    //{
    //    RoleManagementController mController = new RoleManagementController();
    //    int RoleID = (int)Session["RoleID"];
    //    DataTable dt = mController.GetAssignUnAssignModule(RoleID, Constants.IntNullValue, Constants.IntNullValue, 2);
    //    var PageURL = this.Page.ToString().Substring(4, this.Page.ToString().Substring(4).Length - 5) + ".aspx";
    //    string M_Key = Convert.ToString(PageURL.Replace("_", "/"));
    //    string[] reqArray = M_Key.Split('/');
    //    M_Key = reqArray[reqArray.Count() - 0];
    //    DataRow[] foundRows = dt.Select("MODULE_KEY  = '" + M_Key.Replace("/", "_").Trim() + "'");
    //    if (foundRows.Length > 0)
    //    {
    //        return true;
    //    }
    //    return false;
    //}
    /// <summary>
    /// Loads Principals To Principal Combo
    /// </summary>
    private void Populate_drpSKUCompany()
    {
        SkuHierarchyController mHer_Controller = new SkuHierarchyController();
        DataTable m_dt = mHer_Controller.SelectSkuHierarchy(Constants.SKUPrincipal, Constants.IntNullValue, Constants.IntNullValue, null, null, true, int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDropDownList(this.ddskuPrincipal, m_dt, 0, 3, true);
    }

    /// <summary>
    /// Loads Divisions To Division Combo, Categories To Category Combo, Brands To Brand Combo, SKUS To SKU Combo And Prices To Price Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddskuPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        Populate_drpSKUDivisions();
        Populate_drpSKUCategory();
        Populate_drpSKUSubCategory();
        Populate_drpSKUBrand();
        LoadSKUS();
        LoadGrid();
    }

    /// <summary>
    /// Loads Divisions To Division Combo
    /// </summary>
    private void Populate_drpSKUDivisions()
    {
        if (ddskuPrincipal.Items.Count > 0)
        {
            SkuHierarchyController mHer_Controller = new SkuHierarchyController();
            if (ddskuPrincipal.Items.Count   > 0)
            {
                DataTable m_dt = mHer_Controller.SelectSkuHierarchy(Constants.SKUDivision, Constants.IntNullValue, int.Parse(ddskuPrincipal.SelectedValue.ToString() ), null, null, true, int.Parse(this.Session["CompanyId"].ToString()));
                clsWebFormUtil.FillDropDownList(this.ddskuDivision, m_dt, 0, 3, true);
            }
        }
    }

    /// <summary>
    /// Loads Categories To Category Combo, Brands To Brand Combo, SKUS To SKU Combo And Prices To Price Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddskuDivision_SelectedIndexChanged(object sender, EventArgs e)
    {
        Populate_drpSKUCategory();
        Populate_drpSKUSubCategory();
        Populate_drpSKUBrand();
        LoadSKUS();
        LoadGrid();
    }
    
    /// <summary>
    /// Loads Categories To Category Combo
    /// </summary>
    private void Populate_drpSKUCategory()
    {
        if (ddskuDivision.Items.Count > 0)
        {
            SkuHierarchyController mHer_Controller = new SkuHierarchyController();
            if (ddskuDivision.Items.Count   > 0)
            {
                DataTable m_dt = mHer_Controller.SelectSkuHierarchy(Constants.SKUCategory, Constants.IntNullValue, int.Parse(ddskuDivision.SelectedValue.ToString()), null, null, true, int.Parse(this.Session["CompanyId"].ToString()));
                clsWebFormUtil.FillDropDownList(this.ddskuCategory, m_dt, 0, 3, true);
            }
        }
    }

    protected void ddskuCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        Populate_drpSKUSubCategory();
        Populate_drpSKUBrand();
        LoadSKUS();
        LoadGrid();
    }


    private void Populate_drpSKUSubCategory()
    {
        if (ddskuDivision.Items.Count > 0)
        {
            if (ddskuCategory.Items.Count > 0)
            {
                SkuHierarchyController mHer_Controller = new SkuHierarchyController();
                DataTable  m_dt = mHer_Controller.SelectSkuHierarchy(Constants.SKUSubCategory, Constants.IntNullValue, int.Parse(ddskuCategory.SelectedValue.ToString()), null, null, true, int.Parse(this.Session["CompanyId"].ToString()));
                clsWebFormUtil.FillDropDownList(this.drpsubcategory, m_dt, 0, 3, true);
            }
        }
        else
        {
            drpsubcategory.Items.Clear();
        }
    }
    protected void drpsubcategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        Populate_drpSKUBrand();
        LoadSKUS();
        LoadGrid();
    }



    /// <summary>
    /// Loads Brands To Brand Combo
    /// </summary>
    private void Populate_drpSKUBrand()
    {
        if (ddskuCategory.Items.Count > 0)
        {
            SkuHierarchyController mHer_Controller = new SkuHierarchyController();
            string strSKUCategoryID = this.ddskuCategory.SelectedItem.Value;

            if (int.Parse(strSKUCategoryID) > 0)
            {
                DataTable m_dt = mHer_Controller.SelectSkuHierarchy(Constants.SKUBrand, Constants.IntNullValue, int.Parse(drpsubcategory.SelectedValue.ToString()), null, null, true, int.Parse(this.Session["CompanyId"].ToString()));
                clsWebFormUtil.FillDropDownList(this.ddskuBrand, m_dt, 0, 3, true);
            }
        }
    }

    /// <summary>
    /// Loads SKUS To SKU Combo And Prices To Price Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddskuBrand_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSKUS();
        LoadGrid();
    }
    
    /// <summary>
    /// Loads SKUS To SKU Combo
    /// </summary>
    private void LoadSKUS()
    {
        ddskuName.Items.Clear();
        if (ddskuBrand.Items.Count > 0)
        {
            SkuController mSKUController = new SkuController();
            int compid = int.Parse(ddskuPrincipal.SelectedItem.Value);
            int divid = int.Parse(ddskuDivision.SelectedItem.Value);
            int catid = int.Parse(ddskuCategory.SelectedItem.Value);
            int subcatid = int.Parse(drpsubcategory.SelectedItem.Value);
            int barandid = int.Parse(ddskuBrand.SelectedItem.Value);
            int varid = Constants.IntNullValue;

            DataTable m_dt = mSKUController.SelectSkuInfo(compid, divid, catid, barandid, varid,subcatid);
            clsWebFormUtil.FillDropDownList(this.ddskuName, m_dt, "SKU_ID", "SKUDETAIL", true);
        }     
    }

    /// <summary>
    /// Loads Prices To Price Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddskuName_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadGrid();
    }
    
    /// <summary>
    /// Loads Prices To Price Grid
    /// </summary>
    private void LoadGrid()
    {
        SKUPriceDetailController mSKUController = new SKUPriceDetailController();
        if (ddskuName.Items.Count > 0)
        {
            DataTable dtsku = mSKUController.SelectSKuCurrentPrice(Constants.IntNullValue,int.Parse(this.Session["CompanyId"].ToString()), Constants.IntNullValue,
                Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue,
                int.Parse(ddskuName.SelectedValue.ToString()));
            Grid_pricedetails.DataSource = dtsku.DefaultView;
            Grid_pricedetails.DataBind();
        }
        
    }
    
    /// <summary>
    /// Loads Locations To Location ListBox
    /// </summary>
    private void LoadDistributor()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillListBox(ChbDistributorList, dt, 0, 2, true);
    }

    /// <summary>
    /// Save Or Updates an SKU Price
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (btnSave.Text == "Save")
        {
            bool isCheck = false;
            SKUPriceDetailController mPriceController = new SKUPriceDetailController();
            DataControl dc = new DataControl();
            for(int i  = 0;i<ChbDistributorList.Items.Count;i++)   
            {
                if(ChbDistributorList.Items[i].Selected == true)
                {
                    isCheck = true;
                    mPriceController.InsertSKU_PRICES(int.Parse(ChbDistributorList.Items[i].Value), int.Parse(ddskuName.SelectedValue.ToString()), 1, decimal.Parse(dc.chkNull_0(txtDistributorPrice.Text)), decimal.Parse(dc.chkNull_0(txtTaxPrices.Text)), decimal.Parse(dc.chkNull_0(txtDistributorPrice.Text)), decimal.Parse(dc.chkNull_0(txtTradePrice.Text)), decimal.Parse(dc.chkNull_0(txtRetailPrice.Text)), DateTime.Parse(txtFromdate.Text),decimal.Parse(dc.chkNull_0(txtSADTax.Text)));
                }
            }
            if (isCheck)
            {
                this.LoadGrid();
                txtRetailPrice.Text = "";
                txtTradePrice.Text = "";
                txtDistributorPrice.Text = "";
                txtTaxPrices.Text = "0.00";
                txtSADTax.Text = "0.00";
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Please Select Location')", true); 
            }
        }
    }
}
