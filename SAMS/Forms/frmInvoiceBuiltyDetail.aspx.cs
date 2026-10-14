using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Linq;

/// <summary>
/// Form To Add, Edit Freight
/// </summary>
public partial class Forms_frmInvoiceBuiltyDetail : System.Web.UI.Page
{
    UserController mUserController = new UserController();
    public int PrincipalWiseSale = 0;
    private static int mPrincipalId;
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
        Configration();
        if (!Page.IsPostBack)
        {
            this.LoadDistributor();
            this.LoadPrincipal();
            this.LoadCustomer();
            this.LoadTransporter();
            this.LoadGrid();
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
    /// <summary>
    /// Loads Transporter To Transporter Combo
    /// </summary>
    private void LoadTransporter()
    {
        DataTable dt =  DistributorController.SelectTranspoter();
        clsWebFormUtil.FillDropDownList(DrpTransporter, dt, 0, 1);
          
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
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, m_dt, 0, 1);
    }
    
    /// <summary>
    /// Loads Customers To Customer Combo
    /// </summary>
    private void LoadCustomer()
    {
        if (PrincipalWiseSale == 0)
        {
            mPrincipalId = Constants.IntNullValue;
        }
        else
        {
            mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
        }
        if (drpDistributor.Items.Count > 0)
        {
            CustomerDataController mController = new CustomerDataController();
            DataTable dtCustomer = mController.SelectPrincipalCustomer(int.Parse(drpDistributor.SelectedValue.ToString()), mPrincipalId, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
            string[] colums = new string[] { "CUSTOMER_ID", "CUSTOMER_DETAIL"};
            DataTable dtCustomer2 = dtCustomer.DefaultView.ToTable("dtCustomer2", true, colums);
            clsWebFormUtil.FillDropDownList(this.DrpCustomer, dtCustomer2, 0, 1, true);
            if (dtCustomer.Rows.Count > 0)
            {
                DrpInvoiceNo.Items.Clear();   
                DataRow[] foundRows = dtCustomer.Select("Customer_id  = " + DrpCustomer.SelectedValue);
                for (int i = 0; i < foundRows.Length; i++)
                {
                    DrpInvoiceNo.Items.Add(new ListItem(foundRows[i]["INVOICE_NO"].ToString(), foundRows[i]["Sale_Invoice_Id"].ToString()));

                }
                this.Session.Add("dtCustomer", dtCustomer);
            }
        }
        else
        {
            DrpCustomer.Items.Clear();
        }
    }
    
    /// <summary>
    /// Loads Freight Data To Grid
    /// </summary>
    private void LoadGrid()
    {
        if (drpDistributor.Items.Count > 0)
        {
            LedgerController LController = new LedgerController();
            DataTable dt = LController.SelectBankCashTransction(int.Parse(drpDistributor.SelectedValue.ToString()), Constants.IntNullValue, Constants.Builty_Exp ,
                DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), PrincipalWiseSale);
            GrdOrder.DataSource = dt;
            GrdOrder.DataBind();
        }
    }

    /// <summary>
    /// Saves Frieght
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnAddNew_Click(object sender, EventArgs e)
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
            if (PrincipalWiseSale == 0)
            {
                mPrincipalId = Constants.IntNullValue;
            }
            else
            {
                mPrincipalId = int.Parse(DrpPrincipal.SelectedValue.ToString());
            }
            LedgerController LController = new LedgerController();
            SAMSCommon.Classes.Configuration.GetAccountHead();

            string MaxDocumentId = LController.SelectLedgerMaxDocumentId(Constants.Journal_Voucher, int.Parse(drpDistributor.SelectedValue.ToString()));

            LController.PostingCash_Bank_Account(Constants.Journal_Voucher, long.Parse(MaxDocumentId), long.Parse(SAMSCommon.Classes.Configuration.BuiltyExp.ToString()), int.Parse(drpDistributor.SelectedValue.ToString()), decimal.Parse(txtAmount.Text), 0,
                   DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), txtRemarks.Text, DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), mPrincipalId,
                    null, int.Parse(this.Session["UserId"].ToString()), int.Parse(DrpInvoiceNo.SelectedValue.ToString()), DrpInvoiceNo.SelectedValue.ToString(), Constants.Document_Invoice, txtBuiltyNo.Text, Constants.DateNullValue, Constants.Builty_Exp, DrpTransporter.SelectedValue.ToString());
            this.LoadGrid();
            this.LoadCustomer();
            DrpCustomer_SelectedIndexChanged(null, null);
            txtAmount.Text = "";
            txtBuiltyNo.Text = "";
            txtRemarks.Text = "";
        }
    }

    /// <summary>
    /// Loads Invoices To Invoice Combo
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpCustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataTable dtCustomer = (DataTable)this.Session["dtCustomer"];
        DrpInvoiceNo.Items.Clear();   
        DataRow[] foundRows = dtCustomer.Select("Customer_id  = " + DrpCustomer.SelectedValue);
        for (int i = 0; i < foundRows.Length; i++)
        {
            DrpInvoiceNo.Items.Add(new ListItem(foundRows[i]["INVOICE_NO"].ToString(), foundRows[i]["Sale_Invoice_Id"].ToString()));
        } 
    }
    /// <summary>
    /// Deletes Fright
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void GrdOrder_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        LedgerController LController = new LedgerController();
        DataControl dc = new DataControl();

        LController.DeleteWareHouseLedger(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(dc.chkNull_0(GrdOrder.Rows[e.RowIndex].Cells[2].Text)),
        int.Parse(dc.chkNull_0(GrdOrder.Rows[e.RowIndex].Cells[7].Text)), Constants.LongNullValue, decimal.Parse(dc.chkNull_0(GrdOrder.Rows[e.RowIndex].Cells[8].Text)));
        this.LoadGrid();
        
    }

    /// <summary>
    /// Loads Customers And Freight Data
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadCustomer(); 
        this.LoadGrid();
    }

    /// <summary>
    /// Loads Customers
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadCustomer(); 
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
