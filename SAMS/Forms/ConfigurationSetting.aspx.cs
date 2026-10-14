using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Forms_ConfigurationSetting : System.Web.UI.Page
{
    UserController UController = new UserController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadSetting();
        }
    }
    public void LoadSetting()
    {
        //DataTable dt = UController.dtSetting(Constants.IntNullValue, Constants.IntNullValue, null, null, false, 2);
        //if (dt.Rows.Count > 0)
        //{
        //    this.Setting_grid.DataSource = dt;
        //    this.Setting_grid.DataBind();
        //}
        //else
        //{
        //    this.Setting_grid.DataSource = null;
        //    this.Setting_grid.DataBind();
        //}
    }
    protected void btnHome_Click(object sender, EventArgs e)
    {

        HtmlControl control = (HtmlControl)Page.FindControl("home");
        control.Style.Add("display", "none");
        HtmlControl control1 = (HtmlControl)Page.FindControl("about");
        control1.Style.Add("display", "block");


        //testdiv.Attributes.Add("style", "display:block");

        //Control myControl1 = FindControl("portlet_tab1");
        //Control Test = FindControl("portlet_tab1");
        //Test.("display", "none");
        //or
        //portlet_tab1.Style.Add("display", "none");
    }

    

    protected void btnEnable_Click(object sender, EventArgs e)
    {

    }

    protected void btnDisable_Click(object sender, EventArgs e)
    {

    }

    protected void Setting_grid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //Create an instance of the datarow
            DataRowView rowData = (DataRowView)e.Row.DataItem;
            //use this object to test data to make decisions
            int ColumnVal = int.Parse(rowData["btnStatus"].ToString());

            Button btnEnable = ((Button)e.Row.FindControl("btnEnable"));
            Button btnDisable = ((Button)e.Row.FindControl("btnDisable"));

            if (ColumnVal == 0)
            {
                btnEnable.Text = "Enable";
                btnDisable.Text = "Disable";
                btnEnable.CssClass = "mybtn mybtn-default";
                btnDisable.CssClass = "mybtn mybtn-success";
            }
            else
            {
                btnEnable.Text = "Enable";
                btnDisable.Text = "Disable";
                btnEnable.CssClass = "mybtn mybtn-success";
                btnDisable.CssClass = "mybtn mybtn-default";
            }
        }
    }

    protected void Setting_grid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        try
        {
            GridViewRow gvr = this.Setting_grid.Rows[e.NewEditIndex];
            int ConfigrationDescID = int.Parse(gvr.Cells[4].Text);
            //UController.UpdateConfigrationDesc(4, ConfigrationDescID, 1);
            LoadSetting();
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
    }

    protected void Setting_grid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            GridViewRow gvr = this.Setting_grid.Rows[e.RowIndex];
            int ConfigrationDescID = int.Parse(gvr.Cells[4].Text);
            //UController.UpdateConfigrationDesc(4, ConfigrationDescID, 0);
            LoadSetting();
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
    }
}