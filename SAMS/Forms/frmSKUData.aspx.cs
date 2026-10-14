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
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Linq;
using System.Data.SqlClient;

/// <summary>
/// From To Add, Edit, Delete SKU
/// </summary>
public partial class Forms_frmSKUData : System.Web.UI.Page
{

    SkuController mController = new SkuController();
    DataControl dc = new DataControl();
    SkuHierarchyController mHer_Controller = new SkuHierarchyController();
    private DataTable m_dt, m_SKUDt;
    private static int m_sku_id;


    bool ViewAccess;
    protected void Page_Load(object sender, EventArgs e)
    {
        ViewAccess = ValidateScreen();
        if (ViewAccess == false)
        {
            Response.Redirect("../Forms/AccessDenied.aspx");
        }
        if (!IsPostBack)
        {

            Populate_drpSKUCompany();
            Populate_drpSKUDivisions();
            Populate_drpSKUCategory();
            Populate_drpSKUSubCategory();
            Populate_drpSKUBrand();
            LoadData();
            LoadGrid();

            DataTable dtAppSetting = (DataTable)Session["dtAppSetting"];
            if (dtAppSetting != null)
            {
                DataRow[] drAppSetting = dtAppSetting.Select("strColumnName='PackSize'");
                if (drAppSetting.Length > 0)
                {
                    lblPackSize.Text = drAppSetting[0]["strColumnValue"].ToString();
                }
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
    private void Populate_drpSKUCompany()
    {
        m_dt = mHer_Controller.SelectSkuHierarchy(Constants.SKUPrincipal, Constants.IntNullValue, Constants.IntNullValue, null, null, true, int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDropDownList(this.ddskuPrincipal, m_dt, 0, 3, true);
    }


    protected void ddskuPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        Populate_drpSKUDivisions();
        Populate_drpSKUCategory();
        Populate_drpSKUSubCategory();
        Populate_drpSKUBrand();
        this.LoadData();
        this.LoadGrid();
    }

    private void Populate_drpSKUDivisions()
    {
        if (ddskuPrincipal.Items.Count > 0)
        {
            if (ddskuPrincipal.Items.Count > 0)
            {
                m_dt = mHer_Controller.SelectSkuHierarchy(Constants.SKUDivision, Constants.IntNullValue, int.Parse(ddskuPrincipal.SelectedValue.ToString()), null, null, true, int.Parse(this.Session["CompanyId"].ToString()));
                clsWebFormUtil.FillDropDownList(this.ddskudivision, m_dt, 0, 3, true);
            }
        }
        else
        {
            ddskudivision.Items.Clear();
        }
    }

    protected void ddskudivision_SelectedIndexChanged(object sender, EventArgs e)
    {
        Populate_drpSKUCategory();
        Populate_drpSKUSubCategory();
        Populate_drpSKUBrand();
        this.LoadData();
        this.LoadGrid();
    }

    private void Populate_drpSKUCategory()
    {
        if (ddskudivision.Items.Count > 0)
        {
            if (ddskudivision.Items.Count > 0)
            {
                m_dt = mHer_Controller.SelectSkuHierarchy(Constants.SKUCategory, Constants.IntNullValue, int.Parse(ddskudivision.SelectedValue.ToString()), null, null, true, int.Parse(this.Session["CompanyId"].ToString()));
                clsWebFormUtil.FillDropDownList(this.ddskucategory, m_dt, 0, 3, true);
            }
        }
        else
        {
            ddskucategory.Items.Clear();
        }
    }

    protected void ddskucategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        Populate_drpSKUSubCategory();
        Populate_drpSKUBrand();
        this.LoadData();
        this.LoadGrid();

    }

    private void Populate_drpSKUSubCategory()
    {
        if (ddskudivision.Items.Count > 0)
        {
            if (ddskucategory.Items.Count > 0)
            {
                m_dt = mHer_Controller.SelectSkuHierarchy(Constants.SKUSubCategory, Constants.IntNullValue, int.Parse(ddskucategory.SelectedValue.ToString()), null, null, true, int.Parse(this.Session["CompanyId"].ToString()));
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
    }

    private void Populate_drpSKUBrand()
    {
        if (drpsubcategory.Items.Count > 0)
        {
            string strSKUCategoryID = this.drpsubcategory.SelectedItem.Value;

            if (drpsubcategory.Items.Count > 0)
            {
                m_dt = mHer_Controller.SelectSkuHierarchy(Constants.SKUBrand, Constants.IntNullValue, int.Parse(drpsubcategory.SelectedValue.ToString()), null, null, true, int.Parse(this.Session["CompanyId"].ToString()));
                clsWebFormUtil.FillDropDownList(this.ddskuBrand, m_dt, 0, 3, true);
            }
        }
        else
        {
            ddskuBrand.Items.Clear();
        }
    }

    private void LoadData()
    {
        SkuController mSKUController = new SkuController();
        m_SKUDt = mSKUController.SelectSkuInfo(int.Parse(ddskuPrincipal.SelectedValue.ToString()), int.Parse(ddskudivision.SelectedValue.ToString()), int.Parse(ddskucategory.SelectedValue.ToString()), Constants.IntNullValue, int.Parse(this.Session["CompanyId"].ToString()));
        this.Session.Add("m_SKUDt", m_SKUDt);
    }

    private void LoadGrid()
    {
        m_SKUDt = (DataTable)this.Session["m_SKUDt"];

        switch (ddSearchType.SelectedIndex)
        {

            case 1:
                m_SKUDt.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                break;
            case 2:
                m_SKUDt.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                break;
            case 3:
                m_SKUDt.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                break;
            case 4:
                m_SKUDt.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                break;
            case 5:
                m_SKUDt.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                break;
            case 6:
                m_SKUDt.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                break;
            default:
                m_SKUDt.DefaultView.RowFilter = "SKU_CODE" + " like '%" + "" + "%'";
                break;
        }
        grdSKUData.DataSource = m_SKUDt.DefaultView;
        grdSKUData.DataBind();
    }

    protected void grdSKUData_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.grdSKUData.PageIndex = e.NewPageIndex;
        this.LoadGrid();

    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        lblErrorMsg.Text = "";
        bool IsExemted = true;
        char Gst_On = 'E';
        if (txtpacksize.Text.Length <= 0)
        {
            lblErrorMsg.Text = "Must Enter SKU Shade";
            return;
        }
        if (txtskucode.Text.Length <= 0)
        {
            lblErrorMsg.Text = "Must Enter SKU Code";
            return;
        }
        if (txtskuname.Text.Length <= 0)
        {
            lblErrorMsg.Text = "Must Enter SKU Name";
            return;
        }
        if (txtweight.Text.Length <= 0)
        {
            lblErrorMsg.Text = "Must Enter SKU Weight";
            return;
        }
        if (btnSave.Text == "Save")
        {
            if (checkDuplicate(txtskucode.Text.ToUpper()))
            {
                mController.InsertSKUS(IsExemted, true, char.Parse(DrpSKUTaxType.SelectedValue.ToString()), int.Parse(ddskuPrincipal.SelectedValue.ToString()), int.Parse(ddskudivision.SelectedValue.ToString()), int.Parse(ddskucategory.SelectedValue.ToString()),
                int.Parse(ddskuBrand.SelectedValue.ToString()), Constants.IntNullValue, 0, 0, short.Parse(dc.chkNull_1(txtunitincase.Text)),
                txtskucode.Text.ToUpper(), txtskuname.Text, null, txtpacksize.Text, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()), decimal.Parse(txtweight.Text), char.Parse(DrpUOM.SelectedValue.ToString()), decimal.Parse(dc.chkNull_0(txtFMR.Text)), decimal.Parse(dc.chkNull_0(txtMR.Text)), decimal.Parse(dc.chkNull_0(txtPurchaseDiscount.Text)), int.Parse(drpsubcategory.SelectedValue));
                this.CLearAll();
            }
            else
            {
                lblErrorMsg.Text = "SKU Code Already Exists";
                return;
            }
        }
        else if (btnSave.Text == "Update")
        {
            if (checkDuplicate(txtskucode.Text.ToUpper(), int.Parse(hdnSku_ID.Value)))
            {
                mController.UpdateSKUS(IsExemted, 1, char.Parse(DrpSKUTaxType.SelectedValue.ToString()), int.Parse(ddskuPrincipal.SelectedValue.ToString()), int.Parse(ddskudivision.SelectedValue.ToString()),
            int.Parse(ddskucategory.SelectedValue.ToString()), int.Parse(ddskuBrand.SelectedValue.ToString()), Constants.IntNullValue, 0
            , 0, short.Parse(dc.chkNull_1(txtunitincase.Text)), int.Parse(hdnSku_ID.Value), txtskucode.Text.ToUpper(), txtskuname.Text, null, txtpacksize.Text, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()), decimal.Parse(txtweight.Text), char.Parse(DrpUOM.SelectedValue.ToString()), decimal.Parse(dc.chkNull_0(txtFMR.Text)), decimal.Parse(dc.chkNull_0(txtMR.Text)), decimal.Parse(dc.chkNull_0(txtPurchaseDiscount.Text)), int.Parse(drpsubcategory.SelectedValue));
                this.CLearAll();
            }
            else
            {
                lblErrorMsg.Text = "SKU Code Already Exists";
                return;
            }
        }
        this.LoadData();

    }


    protected void btnFilter_Click(object sender, EventArgs e)
    {
        this.LoadGrid();
    }


    private void CLearAll()
    {
        txtpacksize.Text = "";
        txtskucode.Text = "";
        txtunitincase.Text = "";
        txtskuname.Text = "";
        txtweight.Text = "";
        txtFMR.Text = "";
        txtMR.Text = "";
        txtPurchaseDiscount.Text = "";
        btnSave.Text = "Save";
        DrpUOM.SelectedValue = "K";
        this.LoadData();
        this.LoadGrid();
    }

    public bool checkDuplicate(string p_sku_code)
    {
        DataTable dt = mController.SelectSkuInfoCode(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, p_sku_code);
        if (dt != null)
        {
            if (dt.Rows.Count != 0)
            {
                return false;
            }
        }
        return true;
    }


    public bool checkDuplicate(string p_sku_code, int sku_id)
    {
        DataTable dt = mController.SelectSkuInfoCode(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(hdnSku_ID.Value), p_sku_code);
        if (dt != null)
        {
            if (dt.Rows.Count != 0)
            {
                return false;
            }
        }
        return true;
    }


    protected void btnEdit_Click(object sender, EventArgs e)
    {
        GridView gv = sender as GridView;
        GridViewRow Rows = (GridViewRow)(sender as LinkButton).NamingContainer;
        btnSave.Text = "Update";
        ddskuPrincipal.SelectedValue = Rows.Cells[0].Text;
        this.Populate_drpSKUDivisions();
        ddskudivision.SelectedValue = Rows.Cells[1].Text;
        this.Populate_drpSKUCategory();
        ddskucategory.SelectedValue = Rows.Cells[2].Text;
        this.Populate_drpSKUSubCategory();
        drpsubcategory.SelectedValue = Rows.Cells[3].Text;
        this.Populate_drpSKUBrand();
        ddskuBrand.SelectedValue = Rows.Cells[4].Text;
        m_sku_id = int.Parse(Rows.Cells[5].Text);
        hdnSku_ID.Value = int.Parse(Rows.Cells[5].Text).ToString();
        txtskucode.Text = Rows.Cells[11].Text;
        txtskuname.Text = Rows.Cells[12].Text;
        txtpacksize.Text = Rows.Cells[13].Text;
        txtunitincase.Text = Rows.Cells[14].Text;
        //txtgstrate.Text = Rows.Cells[13].Text;
        txtweight.Text = Rows.Cells[16].Text.Replace("&nbsp;", "0");
        txtFMR.Text = Rows.Cells[19].Text.Replace("&nbsp;", "0");
        txtMR.Text = Rows.Cells[20].Text.Replace("&nbsp;", "0");
        txtPurchaseDiscount.Text = Rows.Cells[21].Text.Replace("&nbsp;", "0");
        string x = Rows.Cells[18].Text;
        string y = x.Substring(0, 1);
        DrpUOM.SelectedValue = y;
        DrpSKUTaxType.SelectedValue = Rows.Cells[15].Text.Trim();
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        GridView gv = sender as GridView;
        GridViewRow Rows = (GridViewRow)(sender as LinkButton).NamingContainer;
        bool IsExemted = bool.Parse(Rows.Cells[17].Text);
        //string result = mController.UpdateSKUS(IsExemted, Convert.ToInt32(0), char.Parse(Rows.Cells[15].Text), int.Parse(Rows.Cells[0].Text), int.Parse(Rows.Cells[1].Text), int.Parse(Rows.Cells[2].Text), int.Parse(Rows.Cells[4].Text), Constants.IntNullValue,
        //Constants.DecimalNullValue, 0, short.Parse(Rows.Cells[14].Text), int.Parse(Rows.Cells[5].Text), Rows.Cells[11].Text, Rows.Cells[12].Text, null, null, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()), Constants.DecimalNullValue, Constants.CharNullValue);

        using (SqlConnection con = new SqlConnection(SAMSCommon.Classes.Configuration.ConnectionString))
        {
            con.Open();
            using (SqlCommand command = new SqlCommand("UPDATE SKUS SET ISACTIVE = 0 WHERE SKU_ID = " + int.Parse(Rows.Cells[5].Text), con))
            {
                command.ExecuteNonQuery();
            }
            con.Close();
        }
        this.LoadData();
        this.LoadGrid();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Record updated successfully.');", true);
    }
}
