using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSCommon.Classes;
using SAMSBusinessLayer.Classes;
using System.Linq;

/// <summary>
/// Form to Add, Edit Users
/// </summary>
public partial class Forms_frmCreateUser : System.Web.UI.Page
{
    RoleManagementController mController = new RoleManagementController();
    UserController UController = new UserController();

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
            this.LoadDISTRIBUTOR();
            this.LoadUser();
            this.LoadRole();
            this.LoadGrid();
            btnSave.Attributes.Add("onclick", "return ValidateForm()");
        }
        
        Response.Expires = 0;
        Response.Cache.SetNoStore();

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
    private void LoadDISTRIBUTOR()
    {
        DistributorController mController = new DistributorController();
        DataTable dtDistributor = mController.SelectDistributor(Constants.IntNullValue,Constants.IntNullValue,Constants.IntNullValue);
        ddDistributorId.DataSource = dtDistributor;
        ddDistributorId.DataTextField = "DISTRIBUTOR_NAME";
        ddDistributorId.DataValueField = "DISTRIBUTOR_ID";
        ddDistributorId.DataBind();
    }

    /// <summary>
    /// Loads Users To User Combo
    /// </summary>
    private void LoadUser()
    {
        if (ddDistributorId.Items.Count > 0)
        {
            Distributor_UserController du = new Distributor_UserController();
            DataTable dt = du.SelectDistributorUser(Constants.IntNullValue, int.Parse(ddDistributorId.SelectedValue.ToString()), int.Parse(this.Session["CompanyId"].ToString()));
            clsWebFormUtil.FillDropDownList(this.DrpUser, dt, 0, 6, true);
        }
    }

    /// <summary>
    /// Loads Roles To Role Combo
    /// </summary>
    protected void LoadRole()
    {
        DataTable dt = mController.SelectRoleMaster(Constants.IntNullValue, null, Constants.DateNullValue, Constants.DateNullValue, true);
        ddRole.DataSource = dt;
        ddRole.DataTextField = "ROLE_NAME";
        ddRole.DataValueField = "ROLE_ID";
        ddRole.DataBind();
    }

    /// <summary>
    /// Loads Users To User Grid
    /// </summary>
    protected void LoadGrid()
    {
        if (ddDistributorId.Items.Count > 0)
        {
            DataTable dt = UController.SelectSlashUserBoth(null, null);
            switch (ddSearchType.SelectedIndex)
            {

                case 1:
                    dt.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                case 2:
                    dt.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                case 3:
                    dt.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                default:
                    dt.DefaultView.RowFilter = "USER_NAME" + " like '%" + "" + "%'";
                    break;
            }
            this.Grid_users.DataSource = dt;
            this.Grid_users.DataBind();
        }
    }

    /// <summary>
    /// Loads Users To User Combo
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void ddDistributorId_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadUser();
    }

    /// <summary>
    /// Sets User Data For Edit. This Function Runs When An Existing User Needs To Be Edited
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void Grid_users_RowEditing(object sender, GridViewEditEventArgs e)
    {
        try
        {
            GridViewRow gvr = this.Grid_users.Rows[e.NewEditIndex];
            ddDistributorId.SelectedValue = gvr.Cells[1].Text;
            this.LoadUser();
            DrpUser.SelectedValue = gvr.Cells[0].Text;
            this.txtLoginId.Text = gvr.Cells[4].Text;
            this.txtpassword.Text = gvr.Cells[10].Text;
            this.chkIsActive.Checked = bool.Parse(gvr.Cells[7].Text);
            ddRole.SelectedValue = gvr.Cells[8].Text;
            btnSave.Text = "Update";


        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
    }

    /// <summary>
    /// Sets PageIndex Of User Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewPageEventArgs</param>
    protected void Grid_users_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.Grid_users.PageIndex = e.NewPageIndex;
        LoadGrid();
    }

    /// <summary>
    /// Saves Or Updates A User.
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        
        try
        {
           
            if (btnSave.Text == "Save")
            {
                DataTable dt =  UController.SelectSlashUser(txtLoginId.Text, txtpassword.Text);
                if (dt.Rows.Count > 0)
                {
                    //lblErrorMsg.Text = "LoginId Already Exist";
                    //lblErrorMsg.Visible = true;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('LoginId Already Exist.');", true);
                    return; 
                }

                UController.InsertSlashUser(int.Parse(DrpUser.SelectedValue.ToString()),int.Parse(this.Session["CompanyId"].ToString()), int.Parse(ddDistributorId.SelectedValue.ToString()), txtLoginId.Text, txtpassword.Text, int.Parse(ddRole.SelectedValue.ToString()));                
                
            }
            else if (btnSave.Text == "Update")
            {
                
                UController.UpdateUser(int.Parse(DrpUser.SelectedValue.ToString()) ,int.Parse(this.Session["CompanyId"].ToString()),txtLoginId.Text,txtpassword.Text,int.Parse(ddRole.SelectedValue.ToString()),chkIsActive.Checked,int.Parse(ddDistributorId.SelectedValue.ToString()));

                                
            }
            LoadGrid();
            ClearControls();
            btnSave.Text = "Save";
        }

        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// Filters User From User Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnFilter_Click(object sender, EventArgs e)
    {
        LoadGrid();
    }

    /// <summary>
    /// Cancels Save Or Update Transaction
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearControls();
    }

    /// <summary>
    /// Clears Form Controls
    /// </summary>
    protected void ClearControls()
    {
        try
        {
            txtLoginId.Text = null;
            txtpassword.Text = null;
            lblErrorMsg.Text = null;
            btnSave.Text = "Save";

        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
    }
    
}
