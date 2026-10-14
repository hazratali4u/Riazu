using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Linq;

public partial class Forms_frmDistributor : System.Web.UI.Page
{
    DistributorController mController = new DistributorController();
    private static int DistributorId;
    private static int CompanyId;

    /// <summary>
    /// Page_Load Function Populates All Combos and Grids On The Page
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
            this.GetDistributorType();
            this.LoadGird();
            this.LoadCompany();
            btnSave.Attributes.Add("onclick", "return ValidateForm()");
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
    /// Loads Companies To Comapny Combo
    /// </summary>
    private void LoadCompany()
    {
        CompanyController mCompany = new CompanyController();
        DataTable dt = mCompany.SelectCompany(Constants.IntNullValue, Constants.IntNullValue);
        clsWebFormUtil.FillDropDownList(DrpCompanyName, dt, 0, 1, true);
    }

    /// <summary>
    /// Loads Location Types To LocationType Combo
    /// </summary>
    private void GetDistributorType()
    {
        DataTable dt = mController.SelectDistributorTypeInfo(Constants.IntNullValue);
        clsWebFormUtil.FillDropDownList(ddDistributorType, dt, 0, 2);
    }

    /// <summary>
    /// Loads Locations To Location Grid
    /// </summary>
    private void LoadGird()
    {
        if (ddDistributorType.Items.Count > 0)
        {
            DataTable dtDistributor = mController.SelectAllDistributors(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue);

            switch (ddSearchType.SelectedIndex)
            {
                case 1:
                    dtDistributor.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                case 2:
                    dtDistributor.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                case 3:
                    dtDistributor.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                case 4:
                    dtDistributor.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                case 5:
                    dtDistributor.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                default:
                    dtDistributor.DefaultView.RowFilter = "Distributor_name" + " like '%" + "" + "%'";
                    break;
            }

            GridDistributor.DataSource = dtDistributor;
            GridDistributor.Columns[0].Visible = true;
            GridDistributor.Columns[1].Visible = true;
            GridDistributor.Columns[2].Visible = true;
            GridDistributor.Columns[3].Visible = true;
            GridDistributor.Columns[11].Visible = true;
            GridDistributor.DataBind();
            GridDistributor.Columns[0].Visible = false;
            GridDistributor.Columns[1].Visible = false;
            GridDistributor.Columns[2].Visible = false;
            GridDistributor.Columns[3].Visible = false;
            GridDistributor.Columns[11].Visible = false;
        }

    }

    /// <summary>
    /// Sets/UnSets Focus To GST No. TextBox
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void cbRegistered_CheckedChanged(object sender, EventArgs e)
    {
        if (cbRegistered.Checked == true)
        {
            txtgstno.Enabled = true;
            txtgstno.Focus();
        }
        else
        {
            txtgstno.Text = "";
            txtgstno.Enabled = false;
        }
    }

    /// <summary>
    /// Saves Or Updates A Location
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (btnSave.Text == "Save")
        {
            mController.InsertDistributor(int.Parse(DrpCompanyName.SelectedValue.ToString()), Constants.IntNullValue, !chkIsActive.Checked, System.DateTime.Now, System.DateTime.Now, Constants.IntNullValue, Constants.IntNullValue
                , Constants.IntNullValue, Constants.IntNullValue, int.Parse(ddDistributorType.SelectedValue.ToString()), txtcontactperson.Text, txtPhoneNo.Text, txtgstno.Text,
                txtpassword.Text, txtAddress1.Text, txtAddress2.Text, txtDistributorCode.Text, txtDistributorName.Text, null, cbRegistered.Checked, 1, int.Parse(this.Session["UserId"].ToString()));

        }
        else
        {
            mController.UpdateDistributor(int.Parse(DrpCompanyName.SelectedValue.ToString()), Constants.IntNullValue, !chkIsActive.Checked, Constants.DateNullValue, System.DateTime.Now, Constants.IntNullValue, Constants.IntNullValue
            , Constants.IntNullValue, Constants.IntNullValue, int.Parse(ddDistributorType.SelectedValue.ToString()), txtcontactperson.Text, txtPhoneNo.Text, txtgstno.Text,
            txtpassword.Text, txtAddress1.Text, txtAddress2.Text, DistributorId, txtDistributorCode.Text, txtDistributorName.Text, null, cbRegistered.Checked, 1, int.Parse(this.Session["UserId"].ToString()));
        }
        lblErrorMsg.Text = "";
        ClearAll();
        this.LoadGird();
    }

    /// <summary>
    /// Clears All The Fields Through ClearAll() Function.
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearAll();
    }

    /// <summary>
    /// Loads Locations To Location Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnFilter_Click(object sender, EventArgs e)
    {
        this.LoadGird();
    }

    /// <summary>
    /// Sets PageIndex Of Location Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewPageEventArgs</param>
    protected void GridDistributor_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.GridDistributor.PageIndex = e.NewPageIndex;
        this.LoadGird();
    }

    /// <summary>
    /// Actives/DeActives A Location.
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void GridDistributor_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        DistributorId = int.Parse(GridDistributor.Rows[e.RowIndex].Cells[0].Text);
        CompanyId = int.Parse(GridDistributor.Rows[e.RowIndex].Cells[11].Text);
        bool IsRegister = bool.Parse(GridDistributor.Rows[e.RowIndex].Cells[1].Text);
        mController.UpdateDistributor(CompanyId, Constants.IntNullValue, true, Constants.DateNullValue, System.DateTime.Now, Constants.IntNullValue, Constants.IntNullValue
               , Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, null, null, null, null, null, DistributorId, null,
               null, null, true, 1, int.Parse(this.Session["UserId"].ToString()));
        this.LoadGird();
    }

    private void ClearAll()
    {
        txtDistributorName.Text = "";
        txtDistributorCode.Text = "";
        txtAddress1.Text = "";
        txtAddress2.Text = "";
        txtcontactperson.Text = "";
        txtPhoneNo.Text = "";
        txtpassword.Text = "";
        txtgstno.Text = ""; 
        btnSave.Text = "Save";
        cbRegistered.Checked = false;  
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        GridView gv = sender as GridView;
        GridViewRow Rows = (GridViewRow)(sender as LinkButton).NamingContainer;
        DistributorId = int.Parse(Rows.Cells[0].Text);
        btnSave.Text = "Update";
        DrpCompanyName.SelectedValue = Rows.Cells[11].Text;
        cbRegistered.Checked = bool.Parse(Rows.Cells[1].Text);
        ddDistributorType.SelectedValue = Rows.Cells[2].Text;
        txtAddress2.Text = Rows.Cells[3].Text;
        txtDistributorCode.Text = Rows.Cells[4].Text;
        txtDistributorName.Text = Rows.Cells[5].Text;
        txtAddress1.Text = Rows.Cells[7].Text;
        txtcontactperson.Text = Rows.Cells[8].Text;
        txtPhoneNo.Text = Rows.Cells[9].Text;
        if (Rows.Cells[12].Text == "Active")
        {
            chkIsActive.Checked = true;
        }
        else
        {
            chkIsActive.Checked = false;
        }
        if (Rows.Cells[10].Text == "&nbsp;")
        {
            txtgstno.Text = "";
        }
        else
        {
            txtgstno.Text = Rows.Cells[10].Text;
        }

        try
        {
            txtpassword.Text = Rows.Cells[13].Text;
        }
        catch (Exception ex)
        { }
    }
}
