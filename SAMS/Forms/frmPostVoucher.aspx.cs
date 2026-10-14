using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Linq;

/// <summary>
/// Form To Post Vouchers
/// </summary>
public partial class Forms_frmPostVoucher : System.Web.UI.Page
{
    /// <summary>
    /// Page_Load Function Populates All Combos And Grids On The Page
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
            SAMSCommon.Classes.Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtStartDate.Text = SAMSCommon.Classes.Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = SAMSCommon.Classes.Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            LoadSubVoucher();
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
    /// Loads Locations To Location Comob
    /// </summary>
    private void LoadDistributor()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        this.drpDistributor.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
        clsWebFormUtil.FillDropDownList(this.drpDistributor, dt, 0, 2);
    }

    /// <summary>
    /// Loads Principals To Principal Combo
    /// </summary>
    private void LoadPrincipal()
    {
        SKUPriceDetailController PController = new SKUPriceDetailController();
        DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        this.DrpPrincipal.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
        this.DrpPrincipal.Items.Add(new ListItem("GENERAL ENTRY", "0"));
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, m_dt, 0, 1);
    }

    /// <summary>
    /// Loads Vouchers To Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnView_Click(object sender, EventArgs e)
    {
        LedgerController mController = new LedgerController();
        int voucherType, voucherSubType;
        voucherType = int.Parse(DrpVoucherType.SelectedValue.ToString());
        voucherSubType = int.Parse(DrpSubVoucherType.SelectedValue.ToString());
        if (voucherSubType == Constants.IntNullValue && voucherType==14)
        {
            voucherSubType = 24;
        }
       else  if (voucherSubType == Constants.IntNullValue && voucherType == 15)
        {
            voucherSubType = 17;
        }
        else if (voucherSubType == Constants.IntNullValue && voucherType == 16)
        {
            voucherSubType = 16;
        }
        else if (voucherSubType != Constants.IntNullValue && voucherType != voucherSubType)
        {
            voucherType= voucherSubType ;
        }
        DataTable dt = mController.SelectUnPostVoucherNoNew(voucherType, int.Parse(drpDistributor.SelectedValue.ToString()),int.Parse(DrpPrincipal.SelectedValue.ToString()),false, DateTime.Parse(txtStartDate.Text + " 00:00:00"), 
            DateTime.Parse(txtEndDate.Text + " 23:59:59"),5, voucherSubType);
        GrdLedger.DataSource = dt;
        GrdLedger.DataBind(); 
    }

    /// <summary>
    /// Sets Voucher Data For Edit. This Function Runs When An Existing Voucher Needs To Be Edited
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void GrdLedger_RowEditing(object sender, GridViewEditEventArgs e)
    {
        LedgerController mController = new LedgerController();
        Decimal mDEBITAMT = 0;
        Decimal mCREDITAMT = 0;
        DataTable dtVoucher = mController.SelectVoucherDetail(int.Parse(GrdLedger.Rows[e.NewEditIndex].Cells[6].Text), GrdLedger.Rows[e.NewEditIndex].Cells[1].Text, int.Parse(DrpVoucherType.SelectedValue.ToString()));
        foreach (DataRow dr in dtVoucher.Rows)
        {
            mDEBITAMT  += decimal.Parse(dr["Debit"].ToString());
            mCREDITAMT  += decimal.Parse(dr["CREDIT"].ToString());  
        }

        HF1.Value = Convert.ToString(Math.Round(mDEBITAMT, 2));
        HF2.Value = Convert.ToString(Math.Round(mCREDITAMT, 2));
   
        GrdOrder.DataSource = dtVoucher;
        GrdOrder.DataBind();
        
    }

    /// <summary>
    /// Post Vouchers
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnPost_Click(object sender, EventArgs e)
    {
        int count = 0;
        bool Check = false;
        LedgerController lController = new LedgerController();
        foreach (GridViewRow dr in GrdLedger.Rows)
        {
            CheckBox ChbSelect = (CheckBox)dr.Cells[0].FindControl("ChbSelect");
            if (ChbSelect.Checked == true)
            {
               Check = lController.PostSelectVoucher(int.Parse(dr.Cells[6].Text), dr.Cells[1].Text, int.Parse(dr.Cells[4].Text), 0, DateTime.Parse(dr.Cells[5].Text));
                if (Check)
                {
                    count += count + 1;
                }
            }
        }
        int voucherType, voucherSubType;
        voucherType = int.Parse(DrpVoucherType.SelectedValue.ToString());
        voucherSubType = int.Parse(DrpSubVoucherType.SelectedValue.ToString());
        if (voucherSubType == Constants.IntNullValue && voucherType == 14)
        {
            voucherSubType = 24;
        }
        else if (voucherSubType == Constants.IntNullValue && voucherType == 15)
        {
            voucherSubType = 17;
        }
        else if (voucherSubType == Constants.IntNullValue && voucherType == 16)
        {
            voucherSubType = 16;
        }
        DataTable dt = lController.SelectUnPostVoucherNoNew(voucherType, int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()), false, DateTime.Parse(txtStartDate.Text + " 00:00:00"),
            DateTime.Parse(txtEndDate.Text + " 23:59:59"), 5, voucherSubType);
        GrdLedger.DataSource = dt;
        GrdLedger.DataBind();
        if (count > 0)
        {
            if (count > 1)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Selected vouchers are posted successfully.');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Selected voucher is posted successfully.');", true);
            }

        }
    }

    /// <summary>
    /// Checks/UnChecks All Vouchers in Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void ChbSelect_CheckedChanged(object sender, EventArgs e)
    {
        if (ChbSelect.Checked == true)
        {
            foreach (GridViewRow dr in GrdLedger.Rows)
            {
                CheckBox ChbSelect1 = (CheckBox)dr.Cells[0].FindControl("ChbSelect");
                ChbSelect1.Checked = true;
            }
        }
        else
        {
            foreach (GridViewRow dr in GrdLedger.Rows)
            {
                CheckBox ChbSelect1 = (CheckBox)dr.Cells[0].FindControl("ChbSelect");
                ChbSelect1.Checked = false;
            }
        }
    }

    /// <summary>
    /// Sets Voucher Grid Footer Contorls
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void GrdOrder_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[3].Text = "Total";  
            e.Row.Cells[4].Text = HF1.Value;
            e.Row.Cells[5].Text = HF2.Value;
        }
    }


    protected void DrpVoucherType_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadSubVoucher();
    }
    private void LoadSubVoucher()
    {
        if (int.Parse(DrpVoucherType.SelectedValue.ToString()) == 14)// Cash Voucher
        {
            DrpSubVoucherType.Items.Clear();
            this.DrpSubVoucherType.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
            DrpSubVoucherType.Items.Add(new ListItem("Cash Receipt Voucher", 14.ToString()));
            DrpSubVoucherType.Items.Add(new ListItem("Cash Payment Voucher", 24.ToString()));
        }
        else if (int.Parse(DrpVoucherType.SelectedValue.ToString()) == 15)//Bank Voucher
        {
            DrpSubVoucherType.Items.Clear();
            this.DrpSubVoucherType.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
            DrpSubVoucherType.Items.Add(new ListItem("Bank Receipt Voucher", 15.ToString()));
            DrpSubVoucherType.Items.Add(new ListItem("Bank Payment Voucher", 17.ToString()));
        }
        else
        {
            DrpSubVoucherType.Items.Clear();
            DrpSubVoucherType.Items.Add(new ListItem("N/A", Constants.IntNullValue.ToString()));
        }
    }
}
