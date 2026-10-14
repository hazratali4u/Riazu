using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Forms_Settings : System.Web.UI.Page
{
    RoleManagementController mController = new RoleManagementController();
    UserController UController = new UserController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadGrid();
            ClearControls();
        }
    }
    public void LoadGrid()
    {
        //DataTable dt = UController.dtSetting(Constants.IntNullValue, Constants.IntNullValue, null, null, chkIsActive.Checked, 1);
        //if (dt.Rows.Count > 0)
        //{
        //    switch (ddSearchType.SelectedIndex)
        //    {
        //        case 1:
        //            dt.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
        //            break;
        //        case 2:
        //            dt.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
        //            break;
        //        case 3:
        //            dt.DefaultView.RowFilter = ddSearchType.SelectedValue.ToString() + " like '%" + txtSeach.Text + "%'";
        //            break;
        //        default:
        //            dt.DefaultView.RowFilter = "TAB_NAME" + " like '%" + "" + "%'";
        //            break;
        //    }
        //}
        //this.Setting_grid.DataSource = dt;
        //this.Setting_grid.DataBind();
    }

    public void ClearControls()
    {
        txtDescription.Text = "";
        btnSave.Text = "Save";
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtDescription.Equals(""))
            {
                lblErrorMsg.Text = "Description is Required.";
                return;
            }
            if (btnSave.Text == "Save")
            {
                //UController.InsertSetting(Constants.IntNullValue, int.Parse(DrpTabType.SelectedValue), txtDescription.Text, null, chkIsActive.Checked, 1, Constants.IntNullValue, 0);
            }
            else if (btnSave.Text == "Update")
            {
                
                //UController.UpdateSetting(int.Parse(hfSettingID.Value), int.Parse(DrpTabType.SelectedValue), txtDescription.Text, null, chkIsActive.Checked, 2, int.Parse(hfConfigrationDescID.Value), int.Parse(hfbtnStatus.Value));
            }
            LoadGrid();
            ClearControls();
            btnSave.Text = "Save";
        }

        catch (Exception ex)
        {

        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearControls();
        btnSave.Text = "Save";
    }

    protected void btnFilter_Click(object sender, EventArgs e)
    {

    }

    protected void Setting_grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.Setting_grid.PageIndex = e.NewPageIndex;
        LoadGrid();
    }

    protected void Setting_grid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        try
        {
            GridViewRow gvr = this.Setting_grid.Rows[e.NewEditIndex];
            hfSettingID.Value = gvr.Cells[0].Text;
            DrpTabType.SelectedValue = gvr.Cells[1].Text;
            txtDescription.Text = gvr.Cells[3].Text;
            this.chkIsActive.Checked = bool.Parse(gvr.Cells[4].Text);
            hfConfigrationDescID.Value = gvr.Cells[5].Text;
            hfbtnStatus.Value = gvr.Cells[6].Text;
            btnSave.Text = "Update";
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
    }
}