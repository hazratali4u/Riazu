using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Linq;

/// <summary>
/// Form To Add, Edit, Delete Busines Type
/// </summary>
public partial class Forms_frmDesignation : System.Web.UI.Page
{
    private static int RefId;

    /// <summary>
    /// Page_Load Function Populates All Grids On The Page
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
            this.LoadChannelType();
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
    #region Designation

    /// <summary>
    /// Loads Channel Types To Channel Type Grid
    /// </summary>
    private void LoadChannelType()
    {
        SLASHCodesController mController = new SLASHCodesController();
        DataTable  dt = mController.SelectSlashCodes(Constants.IntNullValue, null, Constants.SaleForce, null, Constants.IntNullValue, bool.Parse("True"));
        gvDesignation.DataSource = dt.DefaultView;
        gvDesignation.DataBind();
     }

     /// <summary>
     /// Sets PageIndex Of Channel Type Grid
     /// </summary>
     /// <param name="sender">object</param>
     /// <param name="e">GridViewPageEventArgs</param>
    protected void gvDesignation_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvDesignation.PageIndex = e.NewPageIndex;
        this.LoadChannelType();
    }

    /// <summary>
    /// Sets Channel Type For Edit. This Function Runs When An Existing Channel Type Needs To Be Edited
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void gvDesignation_RowEditing(object sender, GridViewEditEventArgs e)
    {
        RefId = int.Parse(gvDesignation.Rows[e.NewEditIndex].Cells[0].Text);
        txtDesignation.Text = gvDesignation.Rows[e.NewEditIndex].Cells[2].Text;
        btnSaveDesignation.Text = "Update";
        txtDesignation.Enabled = true;
    }

    /// <summary>
    /// Deletes Channel Type
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void gvDesignation_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        SLASHCodesController mController = new SLASHCodesController();

        RefId = int.Parse(gvDesignation.Rows[e.RowIndex].Cells[0].Text);
        mController.UpdateSlashCodes(RefId, null, Constants.SaleForce, null, 1, false, Constants.DateNullValue);
        this.LoadChannelType();

    }

    /// <summary>
    /// Save Or Updates a Channel Type
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnSaveDesignation_Click(object sender, EventArgs e)
    {
        SLASHCodesController mController = new SLASHCodesController();

        if (btnSaveDesignation.Text == "New")
        {
            txtDesignation.Enabled = true;
            txtDesignation.Focus();
            btnSaveDesignation.Text = "Save";
            ScriptManager.GetCurrent(Page).SetFocus(txtDesignation);
        }
        else if (btnSaveDesignation.Text == "Save")
        {
            if (txtDesignation.Text.Length == 0)
            {
                lblErrorMsg.Text = "Must Entry Channel Name";
                return;
            }
            mController.InsertSlashCodes("", Constants.SaleForce, txtDesignation.Text, 1, true);
            this.LoadChannelType();
            txtDesignation.Text = "";
            txtDesignation.Enabled = false;
            btnSaveDesignation.Text = "New";
            lblErrorMsg.Text = "";
        }
        else if (btnSaveDesignation.Text == "Update")
        {
            mController.UpdateSlashCodes(RefId, null, Constants.SaleForce, txtDesignation.Text, 1, true, Constants.DateNullValue);
            this.LoadChannelType();
            txtDesignation.Text = "";
            txtDesignation.Enabled = false;
            btnSaveDesignation.Text = "New";
        }
    }

    #endregion    
}
