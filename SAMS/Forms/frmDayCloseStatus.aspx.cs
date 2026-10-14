using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Linq;

/// <summary>
/// Form For Day Close
/// </summary>
public partial class Forms_frmDayCloseStatus : System.Web.UI.Page
{
    DistributorController mController = new DistributorController();

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
            LastClosedDay(int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["Distributor_Id"].ToString()));
            if (Grid_Hierarchy.Rows.Count > 1)
            {
                Grid_Hierarchy.UseAccessibleHeader = true;
                Grid_Hierarchy.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
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
    /// Gets Location(s) Last Day Close(s)
    /// </summary>
    /// <param name="UserId">User</param>
    /// <param name="p_Distributor">Location</param>
    private void LastClosedDay(int UserId, int p_Distributor)
    {
        DistributorController mDayClose = new DistributorController();
        DataTable dt = mDayClose.SelectMaxDayClose(UserId, p_Distributor);
        if (dt.Rows.Count > 0)
        {
            this.Session.Add("CurrentWorkDate", DateTime.Parse(dt.Rows[0]["CLOSING_DATE"].ToString()));
            btnDayClose.Visible = true;
        }
        else
        {
           this.Session.Add("CurrentWorkDate", DateTime.Now);
           rblDistributorTypes.Visible = true;
        }
        GetLastClosedDay(UserId, p_Distributor, 0);       
    }
    
    /// <summary>
    /// Loads Location(s) Last Day Close(s) To Grid
    /// </summary>
    /// <param name="UserId">User</param>
    /// <param name="p_Distributor">Location</param>
    /// <param name="p_Status">Status</param>
    private void GetLastClosedDay(int UserId, int p_Distributor, int p_Status)
    {
        DataTable dtable = mController.MaxDayClose(int.Parse(this.Session["UserId"].ToString()), p_Status);
        Grid_Hierarchy.DataSource = dtable;
        Grid_Hierarchy.DataBind();
    }
    
    /// <summary>
    /// Loads Locations(Active/InActive/All) Last Day Close(s) To Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void rblDistributorTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetLastClosedDay(int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["Distributor_Id"].ToString()), Convert.ToInt32(rblDistributorTypes.SelectedValue));
        if (Grid_Hierarchy.Rows.Count > 1)
        {
            Grid_Hierarchy.UseAccessibleHeader = true;
            Grid_Hierarchy.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    /// <summary>
    /// Performs Following Tasks And LogOuts
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>
    /// Closes Day Transactions
    /// </item>
    /// <item>
    /// Inserts Cash Data To GL
    /// </item>
    /// <item>
    /// Inserts Cheques Data To GL
    /// </item>
    /// <item>
    /// Inserts Expenses Data To GL
    /// </item>
    /// <item>
    /// Inserts Sales And Sales Return TO GL
    /// </item>
    /// <iterm>
    /// Inserts Purchase Data To GL
    /// </iterm>
    /// <item>
    /// Inserts Purchase Return To GL
    /// </item>
    /// <item>
    /// Inserts Rate Difference Data To GL
    /// </item>
    /// <item>
    /// Inserts LogOut Time
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnDayClose_Click(object sender, EventArgs e)
    {
        if (btnDayClose.Visible == true)
        {
            int  m_Distributor_id;
            int check = 0;
            bool PendingOrders = false;
            DistributorController mDayClose = new DistributorController();
            foreach (GridViewRow dr in Grid_Hierarchy.Rows)
            {
                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                if (chRelized.Checked == true)
                {
                    m_Distributor_id = int.Parse(dr.Cells[6].Text);
                    DataTable Orders = mDayClose.SelectPendingOrdrsforDayClose(Convert.ToDateTime(dr.Cells[7].Text), m_Distributor_id, Constants.IntNullValue);
                    if (Orders.Rows.Count > 0)
                    {
                        PendingOrders = true;
                        string Distributor_Name = dr.Cells[1].Text;
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Some Orders are pending for " + Distributor_Name + ".');", true);
                        lblMsg.Visible = true;
                        lblMsg.Text = "Some Orders are pending for " + Distributor_Name + ".";
                        btnOkay.Visible = true;
                        OrderPanel.Visible = true;
                        GrdPendingOrder.DataSource = Orders;
                        GrdPendingOrder.DataBind();
                        break;
                    }
                    else
                    {
                        PendingOrders = false;
                        OrderPanel.Visible = false;
                        lblMsg.Visible = false;
                        btnOkay.Visible = false;
                        bool dt = mDayClose.UspDayClose(Convert.ToDateTime(dr.Cells[7].Text), m_Distributor_id, int.Parse(this.Session["UserID"].ToString()));
                        if (dt == false)
                        {
                            check++;
                        }
                    }
                    
                }
            }
            if (!PendingOrders)
            {
                if (check > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert(' Some error in day Close Contact System Administrator');", true);

                }
                else
                {
                    UserController mController = new UserController();
                    if (mController.InsertUserLogoutTime(Convert.ToInt64(Session["User_Log_ID"]), Convert.ToInt32(Session["UserID"])) == "Logout Time Inserted")
                    {
                        this.Session.Clear();
                        Response.Redirect("~/Login.aspx");
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert(' Some error in day Close Contact System Administrator');", true);
                    }
                }
            }
            
        }
    }

    protected void btnOkay_Click(object sender, EventArgs e)
    {
        //PendingOrders = false;
        OrderPanel.Visible = false;
        lblMsg.Visible = false;
        btnOkay.Visible = false;
    }
}
