using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Linq;

/// <summary>
/// Form To Add, Edit Branch Expense And Salary
/// </summary>
public partial class frmExpenseEntry : System.Web.UI.Page
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
            LoadPettyHead();
            this.LoadAccountHead();
            this.CreatTable();
            this.LoadVoucherNo();
            btnAddNew.Attributes.Add("onclick", "return ValidateForm();");
            ScriptManager.GetCurrent(Page).SetFocus(drpDistributor);
            lblRowId.Text = "-1";
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
    private void LoadPettyHead()
    {
        DrpPettyHead.Items.Clear();
        DrpPettyHead.Items.Add(new ListItem("Main Cash", "352"));
        DrpPettyHead.Items.Add(new ListItem("BAHL SA", "786"));
        DrpPettyHead.Items.Add(new ListItem("BAHL FA", "787"));
        DrpPettyHead.Items.Add(new ListItem("Shafiul Hassan Account", "937"));
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
    /// Loads Account Heads To AccountHead Combo
    /// </summary>
    private void LoadAccountHead()
    {
        if (DrpPrincipal.SelectedIndex > -1)
        {
            //AccountHeadController mAccountController = new AccountHeadController();
            //DataTable dt = mAccountController.GetAssignAccountHead(Convert.ToInt32(DrpPrincipal.SelectedValue), Convert.ToInt32(rblDetailAccountType.SelectedValue));
            //clsWebFormUtil.FillDropDownList(DrpAccountHead, dt, 0, 4, true);
            SAMSCommon.Classes.Configuration.GetAccountHead();
            AccountHeadController mAccountController = new AccountHeadController();
            //   DataTable dt = mAccountController.GetLocationPettyAccountHead(Convert.ToInt32(drpDistributor.SelectedValue), 1);
            DataTable dt = mAccountController.SelectAccountHeadManual(long.Parse(SAMSCommon.Classes.Configuration.ExpensDefaultType), Constants.AC_AccountHeadId, 1);
            clsWebFormUtil.FillDropDownList(DrpAccountHead, dt, 0, 1, true);
        }
    }

    /// <summary>
    /// Loads Principals To Principal Combo
    /// </summary>
    private void LoadPrincipal()
    {
        SKUPriceDetailController PController = new SKUPriceDetailController();
        DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        //DrpPrincipal.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
        DrpPrincipal.Items.Add(new ListItem("General Entry", "0"));
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, m_dt, 0, 1);
        //DataTable table = new DataTable();
        //table.Columns.Add("Principal_ID", typeof(int));
        //table.Columns.Add("Principal", typeof(string));
        //table.Rows.Add(0, "General Entry");
        //DrpPrincipal.Items.Clear();
        //clsWebFormUtil.FillDropDownList(this.DrpPrincipal, table, 0, 1);
    }
    
    /// <summary>
    /// Creates Datatable For Expense And Salary
    /// </summary>
    private void CreatTable()
    {
        DataTable  dtVoucher = new DataTable();
        dtVoucher.Columns.Add("Account_Head_Id", typeof(long));
        dtVoucher.Columns.Add("Account_Code", typeof(string));
        dtVoucher.Columns.Add("Account_Name", typeof(string));
        dtVoucher.Columns.Add("Debit", typeof(decimal));
        dtVoucher.Columns.Add("Credit", typeof(decimal));
        dtVoucher.Columns.Add("Remarks", typeof(string));
        dtVoucher.Columns.Add("Principal_Id", typeof(string));
        dtVoucher.Columns.Add("Principal", typeof(string));
        dtVoucher.Columns.Add("ACCOUNT_PARENT_ID", typeof(string));
        dtVoucher.Columns.Add("SLIP_NO", typeof(string));
        this.Session.Add("dtVoucher", dtVoucher);

    }

    /// <summary>
    /// Loads Voucher Nos To Voucher No Combo
    /// </summary>
    private void LoadVoucherNo()
    {
        LedgerController LController = new LedgerController();
        DataTable  dt  = LController.SelectVoucherNo(((DateTime)this.Session["CurrentWorkDate"]), int.Parse(drpDistributor.SelectedValue.ToString()), Constants.Journal_Voucher, int.Parse(this.Session["UserId"].ToString()),null,1);
        DrpVoucherNo.Items.Clear();
        DrpVoucherNo.Items.Add(new ListItem("New", "0"));
        clsWebFormUtil.FillDropDownList(DrpVoucherNo, dt, 3, 3);   
    }

    /// <summary>
    /// Deletes Expense And Salary
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void GrdOrder_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        DataTable  dtVoucher = (DataTable)this.Session["dtVoucher"];
        dtVoucher.Rows.RemoveAt(e.RowIndex);
        GrdOrder.DataSource = dtVoucher;
        GrdOrder.DataBind();
        lblRowId.Text = "-1";

        decimal TotalDebit = 0;

        foreach (DataRow dr in dtVoucher.Rows)
        {
            TotalDebit += decimal.Parse(dr["DEBIT"].ToString());
        }
        txtMainCash.Text = TotalDebit.ToString();
  
    }

    /// <summary>
    /// Sets Expense And Salary Data For Edit. This Function Runs When An Existing Expense And Salary Needs To Be Edited
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void GrdOrder_RowEditing(object sender, GridViewEditEventArgs e)
    {
        lblRowId.Text  = e.NewEditIndex.ToString();
        this.LoadAccountHead(); 
        DrpAccountHead.SelectedValue = GrdOrder.Rows[e.NewEditIndex].Cells[0].Text;
        txtRemarks.Text = GrdOrder.Rows[e.NewEditIndex].Cells[2].Text;
        txtslipNo.Text = GrdOrder.Rows[e.NewEditIndex].Cells[5].Text;
        txtAmount.Text = GrdOrder.Rows[e.NewEditIndex].Cells[3].Text;
    }

    /// <summary>
    /// Adds Record To Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        DataControl dc = new DataControl();
        DataTable  dtVoucher = (DataTable)this.Session["dtVoucher"];
        if (lblRowId.Text == "-1")
        {
            DataRow dr = dtVoucher.NewRow();
            dr["ACCOUNT_HEAD_ID"] = DrpAccountHead.SelectedValue.ToString();
            dr["ACCOUNT_NAME"] = DrpAccountHead.SelectedItem.Text;
            dr["DEBIT"] = decimal.Parse(dc.chkNull_0(txtAmount.Text));
            dr["Credit"] = "0";
            dr["REMARKS"] = txtRemarks.Text;
            dr["Principal"] = DrpPrincipal.SelectedItem.Text;
            dr["Principal_id"] = DrpPrincipal.SelectedItem.Value;
            dr["ACCOUNT_PARENT_ID"] = 61;//Branch Expenses  (FDM EXP)
            dr["SLIP_NO"] = txtslipNo.Text;
            dtVoucher.Rows.Add(dr);

        }
        else
        {
            DataRow dr = dtVoucher.Rows[int.Parse(lblRowId.Text)];
            dr["ACCOUNT_HEAD_ID"] = DrpAccountHead.SelectedValue.ToString();
            dr["ACCOUNT_NAME"] = DrpAccountHead.SelectedItem.Text;
            dr["DEBIT"] = decimal.Parse(dc.chkNull_0(txtAmount.Text));
            dr["Credit"] = "0";
            dr["REMARKS"] = txtRemarks.Text;
            dr["Principal"] = DrpPrincipal.SelectedItem.Text;
            dr["Principal_id"] = DrpPrincipal.SelectedItem.Value;
            dr["ACCOUNT_PARENT_ID"] = 61; //Branch Expenses  (FDM EXP)
            dr["SLIP_NO"] = txtslipNo.Text;
        }
        
        GrdOrder.DataSource = dtVoucher;
        GrdOrder.DataBind();
        //txtAmount.Text = "";
        //txtRemarks.Text = "";
        lblRowId.Text = "-1";
        ScriptManager.GetCurrent(Page).SetFocus(DrpAccountHead);
        
        decimal TotalDebit = 0;

        foreach (DataRow dr in dtVoucher.Rows)
        {
            TotalDebit += decimal.Parse(dr["DEBIT"].ToString());
        }
        txtMainCash.Text = TotalDebit.ToString();
    }

    /// <summary>
    /// Save Or Updates Expenses And Salary
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnSave_Click(object sender, EventArgs e)
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

            LedgerController mLController = new LedgerController();
            SAMSCommon.Classes.Configuration.GetAccountHead();
            DataControl dc = new DataControl();
            DataTable dtVoucher = (DataTable)this.Session["dtVoucher"];
            bool IsSaveVoucher;
            LedgerController LController = new LedgerController();
            string MaxDocumentId;

            if (dtVoucher.Rows.Count > 0)
            {
                string Head = DrpPettyHead.SelectedItem.Text;
                string[] words = Head.Split('-');
                DataRow dr = dtVoucher.NewRow();
                dr["ACCOUNT_HEAD_ID"] = int.Parse(DrpPettyHead.SelectedValue); //SAMSCommon.Classes.Configuration.PeatyCash;
                dr["ACCOUNT_NAME"] = words;
                dr["DEBIT"] = 0;
                dr["Credit"] = decimal.Parse(dc.chkNull_0(txtMainCash.Text));
                dr["REMARKS"] = dtVoucher.Rows[0]["REMARKS"].ToString(); //txtRemarks.Text;
                dr["Principal"] = DrpPrincipal.SelectedItem.Text;
                dr["Principal_id"] = DrpPrincipal.SelectedItem.Value;
                dtVoucher.Rows.Add(dr);
                if (DrpVoucherNo.SelectedValue.ToString() == "0")
                {
                    MaxDocumentId = mLController.SelectMaxVoucherId(Constants.Journal_Voucher, int.Parse(drpDistributor.SelectedValue.ToString()), ((DateTime)this.Session["CurrentWorkDate"]));
                    //IsSaveVoucher = mLController.Add_Voucher(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()), MaxDocumentId, Constants.Journal_Voucher,
                    //       ((DateTime)this.Session["CurrentWorkDate"]), Constants.Document_Petty_Cash, null, "Petty Voucher", Constants.DateNullValue, null, dtVoucher, int.Parse(this.Session["UserId"].ToString()), txtslipNo.Text);

                    IsSaveVoucher = LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToInt32(DrpPrincipal.SelectedValue), MaxDocumentId, Constants.Journal_Voucher, 
                        Convert.ToDateTime(Session["CurrentWorkDate"]), Constants.Document_Petty_Cash, "0", "Expense Voucher", Constants.DateNullValue, null, dtVoucher, Convert.ToInt32(Session["UserID"]), txtslipNo.Text, Constants.DateNullValue, true, Constants.Journal_Voucher);

                }
                else
                {
                    MaxDocumentId = DrpVoucherNo.SelectedValue.ToString();

                    //IsSaveVoucher = mLController.Add_Voucher(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()), MaxDocumentId, Constants.Journal_Voucher,
                    //       ((DateTime)this.Session["CurrentWorkDate"]), Constants.Document_Petty_Cash, null, "Petty Voucher", Constants.DateNullValue, null, dtVoucher, int.Parse(this.Session["UserId"].ToString()), txtslipNo.Text);

                    IsSaveVoucher = LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToInt32(DrpPrincipal.SelectedValue), MaxDocumentId, Constants.Journal_Voucher,
                            Convert.ToDateTime(Session["CurrentWorkDate"]), Constants.Document_Petty_Cash, "0", "Expense Voucher", Constants.DateNullValue, null, dtVoucher, Convert.ToInt32(Session["UserID"]), txtslipNo.Text, Constants.DateNullValue, true, Constants.Journal_Voucher);
                }

                if (IsSaveVoucher == true)
                {
                    dtVoucher.Rows.Clear();
                    GrdOrder.DataSource = null;
                    GrdOrder.DataBind();
                    txtAmount.Text = "";
                    txtRemarks.Text = "";
                    txtslipNo.Text = "";
                    lblRowId.Text = "-1";
                    ScriptManager.GetCurrent(Page).SetFocus(DrpAccountHead);

                    decimal TotalDebit = 0;

                    foreach (DataRow dr1 in dtVoucher.Rows)
                    {
                        TotalDebit += decimal.Parse(dr1["DEBIT"].ToString());
                    }
                    txtMainCash.Text = TotalDebit.ToString();
                    this.LoadVoucherNo();

                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Some Error in Voucher')", true);
                    return;
                }
            }
            else if (lblRowId.Text == "-1")
            {
                MaxDocumentId = DrpVoucherNo.SelectedValue.ToString();

                IsSaveVoucher = mLController.Delete_Voucher(int.Parse(drpDistributor.SelectedValue.ToString()), MaxDocumentId, Constants.Journal_Voucher);
                Session.Remove("dtVoucher");
                LoadVoucherNo();
            }
        }
    }

    /// <summary>
    /// Loads Expense And Salary Data To Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpVoucherNo_SelectedIndexChanged(object sender, EventArgs e)
    {       
        LedgerController LController = new LedgerController();
        DataTable dtVoucher = (DataTable)this.Session["dtVoucher"];

        if (DrpVoucherNo.SelectedValue.ToString() != "0")
        {
            dtVoucher = LController.SelectVoucherDetail(int.Parse(drpDistributor.SelectedValue.ToString()), DrpVoucherNo.SelectedValue.ToString(), Constants.Journal_Voucher);
            for (int i = 0; i < dtVoucher.Rows.Count; i++)
            {
                if (Decimal.Parse(dtVoucher.Rows[i]["CREDIT"].ToString()) > 0)
                {
                    dtVoucher.Rows.RemoveAt(i);
                }
            }
            this.Session.Add("dtVoucher", dtVoucher);
            GrdOrder.DataSource = dtVoucher;
            GrdOrder.DataBind();
        }
        else
        {
            dtVoucher.Rows.Clear();  
            GrdOrder.DataSource = null;
            GrdOrder.DataBind();

        }
        lblRowId.Text = "0";
        this.Session.Add("dtVoucher", dtVoucher);
       
    }

    /// <summary>
    /// Loads Account Heads
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadAccountHead();
    }

    /// <summary>
    /// Loads Principals And Account Heads
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void rblDetailAccountType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rblDetailAccountType.SelectedValue == "55")
        {
            DrpPrincipal.Enabled = false;
            this.LoadPrincipal();
            this.LoadAccountHead();
        }
        else
        {
            DrpPrincipal.Enabled = true;
            SKUPriceDetailController PController = new SKUPriceDetailController();
            DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
            DrpPrincipal.Items.Clear();            
            clsWebFormUtil.FillDropDownList(this.DrpPrincipal, m_dt, 0, 1);
            this.LoadAccountHead();
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
}
