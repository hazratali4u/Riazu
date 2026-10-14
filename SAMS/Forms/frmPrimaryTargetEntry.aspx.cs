using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Linq;

/// <summary>
/// Form To Add Targets
/// </summary>
public partial class Forms_frmPrimaryTargetEntry : System.Web.UI.Page
{

    /// <summary>
    /// Page_Load Function Populates All Combos And Grid On The Page
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
        if (!Page.IsPostBack)
        {
            this.LoadDistributor();
            this.LoadPrincipal();
            this.LoadSKUDetail();
            
            btnTarget.Attributes.Add("onclick", "return ValidateForm();");
            SAMSCommon.Classes.Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtFromdate.Text = SAMSCommon.Classes.Configuration.SystemCurrentDateTime.ToString("MMM-yyyy");
            this.LoadGird();
            pnlTargetview.Visible = false;
            pnlgrdSkuDetail.Visible = true;

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
    /// Loads Sale Forces To TargetFor Combo And Targets To Target Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadSKUDetail();
        this.LoadGird();
        

    }

    /// <summary>
    /// Loads Principals To Prinicpal Combo
    /// </summary>
    private void LoadPrincipal()
    {
        SKUPriceDetailController PController = new SKUPriceDetailController();
        DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        clsWebFormUtil.FillDropDownList(this.drpPrincipal, m_dt, 0, 1, true);
    }

    /// <summary>
    /// Loads Targets To Target Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void drpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadSKUDetail();
        this.LoadGird();
        

    }

    /// <summary>
    /// Loads Sale Forces To TargetFor 
    /// </summary>
    /// 

    private void LoadSKUDetail()
    {
        pnlTargetview.Visible = false;
        pnlgrdSkuDetail.Visible = true;
        SKUPriceDetailController PController = new SKUPriceDetailController();
        if (int.Parse(drpPrincipal.SelectedValue.ToString()) > 0)
        {
            DataTable Dtsku_Price = PController.SelectDataPrice(int.Parse(drpPrincipal.SelectedValue.ToString()), Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue,int.Parse (drpDistributor.SelectedValue), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 3, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
            this.Session.Add("Dtsku_Price", Dtsku_Price);
        }
    }



    /// <summary>
    /// Loads Targets To Target Grid
    /// </summary>
    private void LoadGird()
    {
        DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        GrdSkuDetail.DataSource = Dtsku_Price;
        GrdSkuDetail.DataBind();
    }

    private void LoadGird(DataTable Dtsku_Price)
    {
       // DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        GrdSkuDetail.DataSource = Dtsku_Price;
        GrdSkuDetail.DataBind();
    }

    protected void btnTarget_Click(object sender, EventArgs e)
    {
        btnCalculate_Click(null, null);
        TargetController TG = new TargetController();
        DataControl dc = new DataControl();
        if (btnTarget.Text == "Save")
        {
            bool Insert = false;
            DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
            if (pnlgrdSkuDetail.Visible == true)
            {
                Insert = TG.InsertTarget(int.Parse(drpDistributor.SelectedValue), DateTime.Parse(txtFromdate.Text), Constants.PrimarySale, Constants.LongNullValue, Constants.IntNullValue, Constants.DecimalNullValue, 0, int.Parse(drpPrincipal.SelectedValue), int.Parse(this.Session["UserID"].ToString()), Dtsku_Price, int.Parse(dc.chkNull_0(txtWorkingDays.Text)));
            }else
            {
                Insert = TG.updateTarget(int.Parse(drpDistributor.SelectedValue), DateTime.Parse(txtFromdate.Text), Constants.PrimarySale, Constants.LongNullValue, Constants.IntNullValue, Constants.DecimalNullValue, 0, int.Parse(drpPrincipal.SelectedValue), int.Parse(this.Session["UserID"].ToString()), Dtsku_Price, int.Parse(dc.chkNull_0(txtWorkingDays.Text)));
            }
            if (Insert)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert(' Target Updated');", true);
            }else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Some Error !');", true);
            }
            MasterClear();

        }


    }


    

    private void MasterClear()
    {
       txtGrowthMaster.Text = "";
        txtWorkingDays.Text = "";
        pnlgrdSkuDetail.Visible = true;
        pnlTargetview.Visible = false;
        
        //txtSkuName.Text = "";
        //txtSkuAmount.Text = "";
        //txtQuantity.Text = "";
        //txtTP.Text = "";
        //Session.Remove("PurchaseSKU");
        //GrdSkuDetail.DataSource = null;
        //GrdSkuDetail.DataBind();
        this.LoadSKUDetail();
        //this.CreatTable();
        this.LoadGird();

    }


    protected void btnCalculate_Click(object sender, EventArgs e)
    {
        DataControl dc = new DataControl();
        DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        if (pnlgrdSkuDetail.Visible == true)
        {
            foreach (GridViewRow dr in GrdSkuDetail.Rows)
            {
                foreach (DataRow Row in Dtsku_Price.Rows)
                {
                    var x = dr.Cells[0].ToString();
                    if (Row["SKU_ID"].ToString() == dr.Cells[0].Text)
                    {
                        TextBox txtPurchaseQty = (TextBox)dr.Cells[4].FindControl("txtPurchaseQty");
                        TextBox txtGrowth = (TextBox)dr.Cells[4].FindControl("txtGrowth");
                        TextBox txtTargetQty = (TextBox)dr.Cells[4].FindControl("txtTargetQty");
                        TextBox txtWeight = (TextBox)dr.Cells[4].FindControl("txtWeight");
                        TextBox txtValue = (TextBox)dr.Cells[4].FindControl("txtValue");

                        Row["Purchase"] = dc.chkNull_0(txtPurchaseQty.Text);
                        Row["Growth"] = dc.chkNull_0(txtGrowth.Text);
                        Row["targetqty"] = Math.Round(decimal.Parse(Row["Purchase"].ToString()) + (decimal.Parse(Row["Purchase"].ToString()) * decimal.Parse(Row["Growth"].ToString()) / 100));
                        Row["weight"] = Math.Round(decimal.Parse(Row["targetqty"].ToString()) * decimal.Parse(Row["Unit_Price"].ToString()) * decimal.Parse(Row["sku_weight"].ToString()) , 4);
                        Row["value"] = Math.Round(decimal.Parse(Row["targetqty"].ToString()) * decimal.Parse(Row["Unit_Price"].ToString()), 4);
                        break;
                    }
                }
            }
            this.Session.Add("Dtsku_Price", Dtsku_Price);
            LoadGird(Dtsku_Price);
        }
        else
        {

            foreach (GridViewRow dr in GrdTargetView.Rows)
            {
                foreach (DataRow Row in Dtsku_Price.Rows)
                {
                    var x = dr.Cells[0].ToString();
                    if (Row["SKU_ID"].ToString() == dr.Cells[1].Text)
                    {
                        TextBox txtPurchaseQty = (TextBox)dr.FindControl("txtPurchaseQty");
                        TextBox txtGrowth = (TextBox)dr.FindControl("txtGrowth");
                        TextBox txtTargetQty = (TextBox)dr.FindControl("txtTargetQty");
                        TextBox txtWeight = (TextBox)dr.FindControl("txtWeight");
                        TextBox txtValue = (TextBox)dr.FindControl("txtValue");
                        TextBox txtWDays = (TextBox)dr.FindControl("txtWDays");
                        Row["WORKING_DAYS"] = dc.chkNull_0(txtWDays.Text);
                        Row["Purchase"] = dc.chkNull_0(txtPurchaseQty.Text);
                        Row["Growth"] = dc.chkNull_0(txtGrowth.Text);
                        Row["targetqty"] = Math.Round(decimal.Parse(Row["Purchase"].ToString()) + (decimal.Parse(Row["Purchase"].ToString()) * decimal.Parse(Row["Growth"].ToString()) / 100));
                        Row["weight"] = Math.Round(decimal.Parse(Row["targetqty"].ToString()) * decimal.Parse(Row["Unit_Price"].ToString()) * decimal.Parse(Row["sku_weight"].ToString()), 4);
                        Row["value"] = Math.Round(decimal.Parse(Row["targetqty"].ToString()) * decimal.Parse(Row["Unit_Price"].ToString()), 4);
                        break;
                    }
                }
            }
            this.Session.Add("Dtsku_Price", Dtsku_Price);
            LoadTargetGird(Dtsku_Price);
        }
    }

    protected void txtGrowthMaster_TextChanged(object sender, EventArgs e)
    {
        DataControl dc = new DataControl();
        DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        foreach (DataRow Row in Dtsku_Price.Rows)
        {
            Row["Growth"] = dc.chkNull_0(txtGrowthMaster.Text);
        }
        this.Session.Add("Dtsku_Price", Dtsku_Price);
        if (pnlgrdSkuDetail.Visible == true)
        {
            LoadGird(Dtsku_Price);
        }
        else
        {
            LoadTargetGird(Dtsku_Price);
        }
        ScriptManager.GetCurrent(Page).SetFocus(txtWorkingDays);
    }


    protected void BtnGetData_Click(object sender, EventArgs e)
    {
        GetData();
    }
    private void GetData()
    {
        pnlgrdSkuDetail.Visible = false;
        pnlTargetview.Visible = true;
        TargetController TG = new TargetController();
        SKUPriceDetailController PController = new SKUPriceDetailController();
        if (int.Parse(drpPrincipal.SelectedValue.ToString()) > 0 && drpDistributor.Items .Count >0)
        {
            DataTable Dtsku_Price =  TG.SelectTarget(DateTime.Parse(txtFromdate.Text), Constants.PrimarySale, Constants.LongNullValue, int.Parse(drpDistributor.SelectedValue), int.Parse(drpPrincipal.SelectedValue), 2);
            // DataTable Dtsku_Price = PController.SelectDataPrice(int.Parse(drpPrincipal.SelectedValue.ToString()), Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(drpDistributor.SelectedValue), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 3, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
            var x = Dtsku_Price.Rows.Count;
            this.Session.Add("Dtsku_Price", Dtsku_Price);
            LoadTargetGird(Dtsku_Price);
        }
       

    }
    private void LoadTargetGird(DataTable Dtsku_Price)
    {
        // DataTable Dtsku_Price = (DataTable)this.Session["Dtsku_Price"];
        GrdTargetView.DataSource = Dtsku_Price;
        GrdTargetView.DataBind();
    }

}



