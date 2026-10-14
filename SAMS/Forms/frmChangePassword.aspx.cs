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

/// <summary>
/// Form To Change User Password
/// </summary>
public partial class Forms_fmChangePassword : System.Web.UI.Page
{
    UserController UserInfo = new UserController();
    DataTable dtLogin_ID = new DataTable();

    /// <summary>
    /// Page_Load Function
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
            btnSave.Attributes.Add("onclick", "return ValidatePassword();");   
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
    /// Updates User Password
    /// </summary>
    /// <remarks>
    /// Returns True on Success And False on Failure
    /// </remarks>
    /// <returns>True on Success And False on Failure</returns>
    protected bool UpdatePassword()
    {
        try
        {
            dtLogin_ID = UserInfo.SelectSlashUser(Convert.ToInt32(Session["UserID"].ToString()));
            if (this.txtCurrentPassword.Text == dtLogin_ID.Rows[0]["PASSWORD"].ToString())
            {
                UserInfo.UpdateUser(Convert.ToInt32(Session["UserID"].ToString()), Convert.ToInt32(Session["CompanyId"]), dtLogin_ID.Rows[0]["LOGIN_ID"].ToString(), this.txtConfirmNewPassword.Text, Constants.IntNullValue, true, Convert.ToInt32(Session["DISTRIBUTOR_ID"].ToString()));
                return true;
            }
            else 
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Current Password is wrong ');", true); 
                return false;
            }
        }
        catch(Exception ex)
        {
            ex.ToString();
            return false;
        }
    }

    /// <summary>
    /// Updates User Password Through UpdatePassword() Function
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        bool status = UpdatePassword();
        if (status == true)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Password has changed successfully ');", true);
        }
    }

    /// <summary>
    /// Clears Form Controls
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        this.txtCurrentPassword.Text = null;
        this.txtNewPassword.Text = null;
        this.txtConfirmNewPassword.Text = null;
    }
}
