using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using SAMSBusinessLayer.Reports;
using System.Linq;

/// <summary>
/// Form For Sale Person DSR Report
/// </summary>
public partial class Forms_frmOrderShifting : System.Web.UI.Page
{
    bool flag = false;
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
        if (!IsPostBack)
        {
            txtStartDate.Text = Convert.ToDateTime(Session["CurrentWorkDate"]).ToString("dd-MMM-yyyy");
            this.LoadDistributor();
            this.LoadDeliveryMan();
            this.LoadOrders();
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
    private void LoadDeliveryMan()
    {
        ddlDeliveryMan.Items.Clear();
        ddlDeliveryManTo.Items.Clear();
        if (drpDistributor.Items.Count > 0)
        {
            SaleForceController mDController = new SaleForceController();
            DataTable m_dt = mDController.SelectSaleForceAssignedArea(int.Parse(drpDistributor.SelectedValue.ToString()), Constants.IntNullValue, int.Parse(this.Session["CompanyId"].ToString()));
            clsWebFormUtil.FillDropDownList(this.ddlDeliveryMan, m_dt, 0, 3);
            clsWebFormUtil.FillDropDownList(this.ddlDeliveryManTo, m_dt, 0, 3);
        }
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

    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadDeliveryMan();
        this.LoadOrders();
    }

    protected void btnGetOrder_Click(object sender, EventArgs e)
    {
        if (Convert.ToDateTime(txtStartDate.Text) < Convert.ToDateTime(Session["CurrentWorkDate"]))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Date can not be less than Working Date.');", true);
            return;
        }
        this.LoadOrders();
    }

    protected void btnShift_Click(object sender, EventArgs e)
    {
        if (ddlDeliveryMan.SelectedValue == ddlDeliveryManTo.SelectedValue)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Orders can not be shifted to same Sale Force');", true);
            return;
        }
        else
        {
            flag = false;
            foreach (ListItem li in cblOrders.Items)
            {
                if (li.Selected)
                {
                    if (li.Value != Constants.LongNullValue.ToString())
                    {
                        OrderEntryController OrderCtl = new OrderEntryController();
                        flag = OrderCtl.ShiftOrder(Convert.ToInt64(li.Value), Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToInt32(ddlDeliveryManTo.SelectedValue));
                    }
                    else
                    {
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Order(s) shifted successfully.');", true);
                this.LoadOrders();
            }
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Forms/frmOrderShifting.aspx?LevelType=3&LevelID=" + Request.QueryString["LevelID"].ToString());
    }

    private void LoadOrders()
    {
        OrderEntryController or = new OrderEntryController();
        DataTable dtOrder = or.SelectPendingOrder(int.Parse(drpDistributor.SelectedValue.ToString()), Constants.IntNullValue,
            Constants.IntNullValue, Constants.IntNullValue, int.Parse(ddlDeliveryMan.SelectedValue.ToString()),
            Constants.Order_Pending_Id, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Convert.ToDateTime(txtStartDate.Text), 2);
        cblOrders.Items.Clear();
        if (dtOrder.Rows.Count > 0)
        {
            clsWebFormUtil.FillListBox(this.cblOrders, dtOrder, "SALE_ORDER_ID", "ORDERNO");
            cblOrders.Attributes.Add("onclick", string.Format("javascript:CheckBoxListSelect({0})", cblOrders.ClientID));
        }
        else
        {
            cblOrders.Items.Add(new ListItem("No Order found.", Constants.LongNullValue.ToString()));
        }
    }

    protected void ddlDeliveryMan_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadOrders();
    }
}