using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for BasePage
/// </summary>
public class BasePage : System.Web.UI.Page
{
    
    public bool ValidateScreen()
    {
        RoleManagementController mController = new RoleManagementController();
        int RoleID = (int)Session["RoleID"];
        DataTable dt = mController.GetAssignUnAssignModule(RoleID, Constants.IntNullValue, Constants.IntNullValue, 2);
        
        var req1 = this.Page.ToString().Substring(4, this.Page.ToString().Substring(4).Length - 5) + ".aspx";
        string req = Convert.ToString(req1.Replace("_", "/"));
        string[] reqArray = req.Split('/');
        req = reqArray[reqArray.Count() - 1];

        DataRow[] foundRows = dt.Select("MODULE_KEY  = '" + req.Trim() + "'");
        if (foundRows.Length > 0)
        {
            return true;
        }
        return false;
    }
   
}