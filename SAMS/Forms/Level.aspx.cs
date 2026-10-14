using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;

public partial class Forms_Level : System.Web.UI.Page
{
    RoleManagementController ObjRole = new RoleManagementController();
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
            LoadMainGrid();
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
        if (M_Key.Trim() == "level.aspx")
        {
            return true;
        }
        DataRow[] foundRows = dt.Select("MODULE_KEY  = '" + M_Key.Trim() + "'");
        if (foundRows.Length > 0)
        {
            return true;
        }
        return false;
    }
    private void LoadMainGrid()
    {
        int RoleID = (int)Session["RoleID"];
        DataTable dtMenu = ObjRole.SelectRoleWiseModule(RoleID, 3, Convert.ToInt32(Request.QueryString["LevelID"]));
        gvMain.DataSource = dtMenu;
        gvMain.DataBind();
    }

    protected void gvMain_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView gvForms = (GridView)e.Row.FindControl("gvForms");
            {
                int RoleID = (int)Session["RoleID"];
                int a = int.Parse(((GridView)sender).DataKeys[e.Row.RowIndex].Value.ToString());
                DataTable dtFroms = ObjRole.SelectRoleWiseModule(RoleID, 4,a);
                if (dtFroms != null && dtFroms.Rows.Count > 0)
                {
                    gvForms.DataSource = dtFroms;
                    gvForms.DataBind();
                }
            }
        }
    }
}