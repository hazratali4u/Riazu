using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Linq;

/// <summary>
/// From To Take Order, Invoice And Sale Return(Step1)
/// </summary>
public partial class Forms_frmOrderEntryStep1 : System.Web.UI.Page
{
    
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
            this.LoadDistributor();
            this.LoadOrderBooker();
            this.LoadArea();
            this.LoadDeliveryman();
            DateTime pOrderDate = DateTime.Parse(this.Session["CurrentWorkDate"].ToString());
            txtToDate.Text = pOrderDate.ToString("dd-MMM-yyyy");
            txtNewDeliveryDate.Text = pOrderDate.ToString("dd-MMM-yyyy");
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
    /// Enables/Disables Orderbooker Combo
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpDocumentType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DrpDocumentType.SelectedIndex == 0)
        {
            DrpOrderBooker.Enabled = true;
            DateTime pOrderDate = DateTime.Parse(this.Session["CurrentWorkDate"].ToString());
            txtToDate.Text = pOrderDate.ToString("dd-MMM-yyyy");
        }
        else
        {
            DrpOrderBooker.Enabled = false;
            DateTime pOrderDate = (DateTime)this.Session["CurrentWorkDate"];
            txtToDate.Text = pOrderDate.ToString("dd-MMM-yyyy");
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

    /// <summary>
    /// Loads Routes To Route Combo, Orderbookers To Orderbooker Combo And Deliverymen To Deliveryman Comob
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadArea();
        this.LoadOrderBooker();
        this.LoadDeliveryman();
    }

    private void LoadArea()
    {
        if (drpDistributor.Items.Count > 0)
        {
            DistributorAreaController mController = new DistributorAreaController();
            SaleForceController UController = new SaleForceController();
            //DataTable dt = mController.SelectDist_Area(Constants.LongNullValue, Constants.DateNullValue, Constants.DateNullValue, int.Parse(drpDistributor.SelectedValue.ToString()),Constants.IntNullValue, null, null);
            DataTable dt = UController.SelectSalesForceArea(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpOrderBooker.SelectedValue.ToString()), Constants.LongNullValue);
            clsWebFormUtil.FillListBox(this.LstRoutes, dt, 5, 6, true);
            for (int i = 0; i < LstRoutes.Items.Count; i++)
            {
                LstRoutes.Items[i].Selected = true;
            }
            ChbAllRoutes.Checked = true;
            //clsWebFormUtil.FillDropDownList(DrpRoute, dt, 0, 6, true);
        }
        else
        {
            LstRoutes.Items.Clear();
        }
    }

    /// <summary>
    /// Loads Orderbookers To Orderbooker Combo And Deliverymen To Deliveryman Comob
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpRoute_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadOrderBooker();
        this.LoadDeliveryman();
    }

    /// <summary>
    /// Loads Orderbookers To Orderbooker Combo
    /// </summary>
    private void LoadOrderBooker()
    {
        if (drpDistributor.Items.Count > 0)
        {
            //string Routes_IDs = null;
            //for (int i = 0; i < LstRoutes.Items.Count; i++)
            //{
            //    if (LstRoutes.Items[i].Selected == true)
            //    {
            //        Routes_IDs += LstRoutes.Items[i].Value.ToString() + ",";
            //    }
            //}
            //if (Routes_IDs == null)
            //{
            //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Route.');", true);
            //    return;
            //}
            //SaleForceController mDController = new SaleForceController();
            //DataTable m_dt = mDController.SelectSaleForceAssignedAreaMultiSelection(Constants.SALES_FORCE_ORDERBOOKER, int.Parse(drpDistributor.SelectedValue.ToString()), Routes_IDs, int.Parse(this.Session["CompanyId"].ToString()), Constants.IntNullValue);
            //clsWebFormUtil.FillDropDownList(this.DrpOrderBooker, m_dt, 0, 3, true);

            Distributor_UserController UCtl = new Distributor_UserController();
            DataTable dt = UCtl.SelectDistributorUser(Constants.SALES_FORCE_ORDERBOOKER, int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(this.Session["CompanyId"].ToString()));
            clsWebFormUtil.FillDropDownList(this.DrpOrderBooker, dt, 0, 6, true);
            DrpOrderBooker.SelectedIndex = 0;
        }
        else
        {
            DrpOrderBooker.Items.Clear();
        }
    }

    /// <summary>
    /// Loads Deliverymen To Deliveryman Combo
    /// </summary>
    private void LoadDeliveryman()
    {
        if (drpDistributor.Items.Count > 0 && LstRoutes.Items.Count > 0)
        {
            string Routes_IDs = null;
            for (int i = 0; i < LstRoutes.Items.Count; i++)
            {
                if (LstRoutes.Items[i].Selected == true)
                {
                    Routes_IDs += LstRoutes.Items[i].Value.ToString() + ",";
                }
            }
            if (Routes_IDs == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Route.');", true);
                return;
            }
            SaleForceController mDController = new SaleForceController();
            DataTable m_dt = mDController.SelectSaleForceAssignedAreaMultiSelection(int.Parse(drpDistributor.SelectedValue.ToString()), Routes_IDs, int.Parse(this.Session["CompanyId"].ToString()));
            clsWebFormUtil.FillDropDownList(this.DrpDeliveryMan, m_dt, 0, 3, true);
            if (m_dt.Rows.Count > 0)
            {
                DrpDeliveryMan.SelectedIndex = 0;
            }
            
        }
        else
        {
            DrpDeliveryMan.Items.Clear();
        }
    }


    protected void cbAll_CheckedChanged(object sender, EventArgs e)
    {
        if (cbAll.Checked == true)
        {
            foreach (GridViewRow dr in GrdOrder.Rows)
            {
                CheckBox ChbInvoice = (CheckBox)dr.FindControl("ChbInvoice");
                ChbInvoice.Checked = true;
            }
        }
        else
        {
            foreach (GridViewRow dr in GrdOrder.Rows)
            {
                CheckBox ChbInvoice = (CheckBox)dr.FindControl("ChbInvoice");
                ChbInvoice.Checked = false;
            }
        }
    }


    protected void btnGetOrder_Click(object sender, EventArgs e)
    {
        if (DrpOrderBooker.Items.Count > 0)
        {
            this.LoadPendingOrder();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Select Orderbooker');", true);
        }
    }

    
    private void LoadPendingOrder()
    {
        //string Routes_IDs = null;
        //for (int i = 0; i < LstRoutes.Items.Count; i++)
        //{
        //    if (LstRoutes.Items[i].Selected == true)
        //    {
        //        Routes_IDs += LstRoutes.Items[i].Value.ToString() + ",";
        //    }
        //}
        //if (Routes_IDs == null)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Route.');", true);
        //    return;
        //}
        OrderEntryController or = new OrderEntryController();
        DataTable dtOrder = or.SelectPendingOrderMultiSelection(int.Parse(drpDistributor.SelectedValue.ToString()), null,
            Constants.IntNullValue, int.Parse(DrpOrderBooker.SelectedValue.ToString()), int.Parse(DrpDeliveryMan.SelectedValue.ToString()),
            Constants.Order_Pending_Id, int.Parse(ddSearchType.SelectedValue.ToString()), int.Parse(this.Session["UserId"].ToString()), Convert.ToDateTime(txtToDate.Text), 0);
        GrdOrder.DataSource = dtOrder;
        GrdOrder.DataBind();
    }


    protected void btnNext_Click(object sender, EventArgs e)
    {
        DateTime Date = Convert.ToDateTime(Session["CurrentWorkDate"]);
        if (Convert.ToDateTime(txtToDate.Text) < Convert.ToDateTime(Session["CurrentWorkDate"]))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Invalid Working Date');", true);
            return;
        }
        if (DrpOrderBooker.Items.Count > 0)
        {
            if (drpDistributor.Items.Count > 0 && LstRoutes.Items.Count > 0 && DrpDeliveryMan.Items.Count > 0)
            {
                string Routes_IDs = null;
                string Routes_Names = null;
                for (int i = 0; i < LstRoutes.Items.Count; i++)
                {
                    if (LstRoutes.Items[i].Selected == true)
                    {
                        Routes_IDs += LstRoutes.Items[i].Value.ToString() + ",";
                        Routes_Names += LstRoutes.Items[i].Text.ToString() + ",";
                    }
                    
                }
                if (Routes_Names != null)
                {
                    Routes_Names = Routes_Names.Remove(Routes_Names.Length - 1);
                }
                if (Routes_IDs == null)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Route.');", true);
                    return;
                }
                this.Session.Add("DistributorId", int.Parse(drpDistributor.SelectedValue.ToString()));
               // this.Session.Add("PrincipalId", int.Parse(DrpPrincipal.SelectedValue.ToString()));
                this.Session.Add("AreaId", Routes_IDs);
                this.Session.Add("Route", Routes_Names);
                this.Session.Add("SaleMan", DrpDeliveryMan.SelectedItem.Text);
                this.Session.Add("OrderDate", Convert.ToDateTime(txtToDate.Text).ToString("dd-MMM-yyyy"));
              //  this.Session.Add("UnitType", RbUnitType.SelectedIndex);

                if (DrpDocumentType.SelectedIndex == 0)
                {
                    //string OrderBooker_IDs = null;
                    //for (int i = 0; i < LstOrderBooker.Items.Count; i++)
                    //{
                    //    if (LstOrderBooker.Items[i].Selected == true)
                    //    {
                    //        OrderBooker_IDs += LstOrderBooker.Items[i].Value.ToString() + ",";
                    //    }
                    //}
                    //if (OrderBooker_IDs == null)
                    //{
                    //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Order Booker.');", true);
                    //    return;
                    //}
                    this.Session.Add("OrderNo", 0);
                    this.Session.Add("OrderBookerId", int.Parse(DrpOrderBooker.SelectedValue.ToString()));
                    this.Session.Add("DeliveryManId", int.Parse(DrpDeliveryMan.SelectedValue.ToString()));
                }
                else if (DrpDocumentType.SelectedIndex == 1)
                {
                    this.Session.Add("OrderNo", -1);
                    this.Session.Add("OrderBookerId", int.Parse(DrpDeliveryMan.SelectedValue.ToString()));
                    this.Session.Add("DeliveryManId", int.Parse(DrpDeliveryMan.SelectedValue.ToString()));
                }
                else
                {
                    this.Session.Add("OrderNo", -2);
                    this.Session.Add("OrderBookerId", int.Parse(DrpDeliveryMan.SelectedValue.ToString()));
                    this.Session.Add("DeliveryManId", int.Parse(DrpDeliveryMan.SelectedValue.ToString()));
                }
                Response.Redirect("~/Forms/frmOrderEntry.aspx?Status=" + false + "&LevelType=3&LevelID=" + Request.QueryString["LevelID"].ToString());
            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Select Orderbooker');", true);
        }
    }

    
    protected void btnPost_Click(object sender, EventArgs e)
    {
        if (DrpOrderBooker.Items.Count > 0)
        {
            if (IsDayClosed())
            {
                UserController UserCtl = new UserController();

                UserCtl.InsertUserLogoutTime(Convert.ToInt32(Session["User_Log_ID"]), Convert.ToInt32(Session["UserID"]));
                this.Session.Clear();
                System.Web.Security.FormsAuthentication.SignOut();
                Response.Redirect("../Login.aspx");
            }
            else
            {
                CustomerDataController CDC = new CustomerDataController();
                OrderEntryController ORD = new OrderEntryController();
                DataControl dc = new DataControl();
                DataTable dtStock;
                GrdFreeSKU.Visible = false;
                gvRateDifference.Visible = false;
                int Count = 0;
                string OrderNumber = "";
                string Msg = "";
                foreach (GridViewRow dr in GrdOrder.Rows)
                {
                    CheckBox ChbInvoice = (CheckBox)dr.FindControl("ChbInvoice");
                    if (ChbInvoice.Checked == true)
                    {
                        if (Convert.ToDateTime(this.Session["CurrentWorkDate"]) >= Convert.ToDateTime(ConvertDate.British_To_American2(dr.Cells[5].Text)))
                        {
                            if (int.Parse(ddSearchType.SelectedValue.ToString()) == Constants.Cash_Order_Id)
                            {
                                dtStock = ORD.ConvertOrder_to_Invoice(int.Parse(drpDistributor.SelectedValue.ToString()), dr.Cells[12].Text, long.Parse(dr.Cells[0].Text), int.Parse(dr.Cells[13].Text),
                                long.Parse(dr.Cells[19].Text), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), decimal.Parse(dc.chkNull_0(dr.Cells[6].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[7].Text)),
                                decimal.Parse(dc.chkNull_0(dr.Cells[8].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[9].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[10].Text)), Constants.Order_Posted_Id, Constants.Cash_Order_Id, int.Parse(this.Session["UserId"].ToString()), dr.Cells[11].Text, dr.Cells[3].Text,"",true, decimal.Parse(dc.chkNull_0(dr.Cells[14].Text))
                                , decimal.Parse(dc.chkNull_0(dr.Cells[15].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[16].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[17].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[18].Text)));

                                if (dtStock.Columns.Count > 1)
                                {
                                    if (dtStock.Columns.Count == 7)
                                    {
                                        gvRateDifference.DataSource = dtStock;
                                        gvRateDifference.DataBind();
                                        gvRateDifference.Visible = true;
                                    }
                                    else
                                    {
                                        GrdFreeSKU.DataSource = dtStock;
                                        GrdFreeSKU.DataBind();
                                        GrdFreeSKU.Visible = true;
                                    }
                                    this.LoadPendingOrder();
                                    return;
                                }

                            }
                            else
                            {
                                if (ddSearchType.SelectedValue == "215") // For Credit Orders
                                {
                                    decimal CeditLimit = CustomerCreditLimt(int.Parse(dr.Cells[0].Text));
                                    if (CeditLimit > decimal.Parse(dc.chkNull_0(dr.Cells[10].Text)))
                                    {
                                        dtStock = ORD.ConvertOrder_to_Invoice(int.Parse(drpDistributor.SelectedValue.ToString()), dr.Cells[12].Text, long.Parse(dr.Cells[0].Text), int.Parse(dr.Cells[13].Text),
                                                                           long.Parse(dr.Cells[19].Text), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), decimal.Parse(dc.chkNull_0(dr.Cells[6].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[7].Text)),
                                                                           decimal.Parse(dc.chkNull_0(dr.Cells[8].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[9].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[10].Text)), Constants.Order_Posted_Id, int.Parse(ddSearchType.SelectedValue.ToString()), int.Parse(this.Session["UserId"].ToString()), dr.Cells[11].Text, dr.Cells[3].Text, "", true, decimal.Parse(dc.chkNull_0(dr.Cells[14].Text))
                                                                           , decimal.Parse(dc.chkNull_0(dr.Cells[15].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[16].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[17].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[18].Text)));
                                        if (dtStock.Columns.Count > 1)
                                        {
                                            if (dtStock.Columns.Count == 7)
                                            {
                                                gvRateDifference.DataSource = dtStock;
                                                gvRateDifference.DataBind();
                                                gvRateDifference.Visible = true;
                                            }
                                            else
                                            {
                                                GrdFreeSKU.DataSource = dtStock;
                                                GrdFreeSKU.DataBind();
                                                GrdFreeSKU.Visible = true;
                                            }
                                            this.LoadPendingOrder();
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        Count += 1;
                                        OrderNumber += dr.Cells[4].Text + ", ";
                                    }
                                }
                                else
                                {
                                    // For Other Orders
                                    dtStock = ORD.ConvertOrder_to_Invoice(int.Parse(drpDistributor.SelectedValue.ToString()), dr.Cells[12].Text, long.Parse(dr.Cells[0].Text), int.Parse(dr.Cells[13].Text),
                                                                           long.Parse(dr.Cells[19].Text), DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), decimal.Parse(dc.chkNull_0(dr.Cells[6].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[7].Text)),
                                                                           decimal.Parse(dc.chkNull_0(dr.Cells[8].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[9].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[10].Text)), Constants.Order_Posted_Id, int.Parse(ddSearchType.SelectedValue.ToString()), int.Parse(this.Session["UserId"].ToString()), dr.Cells[11].Text, dr.Cells[3].Text, "", true, decimal.Parse(dc.chkNull_0(dr.Cells[14].Text))
                                                                           , decimal.Parse(dc.chkNull_0(dr.Cells[15].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[16].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[17].Text)), decimal.Parse(dc.chkNull_0(dr.Cells[18].Text)));
                                    if (dtStock.Columns.Count > 1)
                                    {
                                        if (dtStock.Columns.Count == 7)
                                        {
                                            gvRateDifference.DataSource = dtStock;
                                            gvRateDifference.DataBind();
                                            gvRateDifference.Visible = true;
                                        }
                                        else
                                        {
                                            GrdFreeSKU.DataSource = dtStock;
                                            GrdFreeSKU.DataBind();
                                            GrdFreeSKU.Visible = true;
                                        }
                                        this.LoadPendingOrder();
                                        return;
                                    }
                                }
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Order Date Can not be greater than current working date');", true);
                            break;
                        }
                    }
                }
                this.LoadPendingOrder();
                if (Count > 1)
                {
                    Msg = "Order numbers " + OrderNumber.TrimEnd(", ".ToCharArray()) + " are not converted into invoices due to customer credit limit.";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('" + Msg + "');", true);
                }
                else if (Count > 0)
                {
                    Msg = "Order number " + OrderNumber.TrimEnd(", ".ToCharArray()) + " is not converted into invoice due to customer credit limit.";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('" + Msg + "');", true);
                }
            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Select Orderbooker');", true);
        }
    }
    public decimal CustomerCreditLimt(int p_CUSTOMER_ID)
    {
        decimal Limit = 0;
        CustomerDataController _customerCtrl = new CustomerDataController();
        DataTable dtCustomer = _customerCtrl.GetCustomerByID(int.Parse(drpDistributor.SelectedValue.ToString()), p_CUSTOMER_ID, 1);
        decimal CeditAmount = decimal.Parse(dtCustomer.Rows[0]["CREDIT_AMOUNT"].ToString());
        decimal CeditLimit = decimal.Parse(dtCustomer.Rows[0]["CREDITLIMIT"].ToString());
        return Limit = CeditLimit + CeditAmount;
    }
    private bool IsDayClosed()
    {
        bool flag = false;
        DistributorController DistrCtl = new DistributorController();
        DataTable dtDayClose = DistrCtl.MaxDayClose(Convert.ToInt32(Session["Distributor_Id"]), 3);
        if (Convert.ToDateTime(Session["CurrentWorkDate"]) == Convert.ToDateTime(dtDayClose.Rows[0]["DayClose"]))
        {
            flag = false;
        }
        else
        {
            flag = true;
        }

        return flag;
    }
    protected void DrpOrderBooker_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadArea();
        LoadDeliveryman();
    }
    protected void DrpDeliveryMan_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void btnUpdateOrder_Click(object sender, EventArgs e)
    {
        if (IsDayClosed())
        {
            UserController UserCtl = new UserController();

            UserCtl.InsertUserLogoutTime(Convert.ToInt32(Session["User_Log_ID"]), Convert.ToInt32(Session["UserID"]));
            this.Session.Clear();
            System.Web.Security.FormsAuthentication.SignOut();
            Response.Redirect("../Login.aspx");
        }
        else
        {
            DateTime dt = Convert.ToDateTime(Session["CurrentWorkDate"]);
            DateTime dt0 = Convert.ToDateTime(txtNewDeliveryDate.Text);

            if (dt0 < dt)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Can not select previous Delivery Date !');", true);
                return;
            }


            bool IsValidInsert = false;
            string OrderNumbers = "";
            foreach (GridViewRow dr in GrdOrder.Rows)
            {
                CheckBox ChbInvoice = (CheckBox)dr.FindControl("ChbInvoice");
                if (ChbInvoice.Checked == true)
                {
                    OrderEntryController mOrderController = new OrderEntryController();
                    IsValidInsert = mOrderController.Update_Order(long.Parse(dr.Cells[19].Text), int.Parse(this.Session["Distributor_Id"].ToString()), int.Parse(this.Session["UserId"].ToString()), DateTime.Parse(txtNewDeliveryDate.Text), "Order Update", int.Parse(DrpDateUpdateReason.SelectedValue));
           
                    if (!IsValidInsert)
                    {
                        OrderNumbers = OrderNumbers + "," + dr.Cells[19].Text;
                    }
                }
            }

            if (IsValidInsert)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Record Updated !');", true);
                DateTime pOrderDate = DateTime.Parse(this.Session["CurrentWorkDate"].ToString()).AddDays(1);
                txtToDate.Text = pOrderDate.ToString("dd-MMM-yyyy");

                this.LoadPendingOrder();

            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Error in Order Nos " + OrderNumbers + ".Please Try Again !');", true);
            }


        }

    }


    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (IsDayClosed())
        {
            UserController UserCtl = new UserController();
            UserCtl.InsertUserLogoutTime(Convert.ToInt32(Session["User_Log_ID"]), Convert.ToInt32(Session["UserID"]));
            this.Session.Clear();
            System.Web.Security.FormsAuthentication.SignOut();
            Response.Redirect("../Login.aspx");
        }
        else
        {
            bool IsValidInsert = false;
            string OrderNumbers = "";
            foreach (GridViewRow dr in GrdOrder.Rows)
            {
                CheckBox ChbInvoice = (CheckBox)dr.FindControl("ChbInvoice");
                if (ChbInvoice.Checked == true)
                {
                    OrderEntryController mOrderController = new OrderEntryController();
                    IsValidInsert = mOrderController.FreshReturnOrder(long.Parse(dr.Cells[19].Text), int.Parse(this.Session["Distributor_Id"].ToString()),-2,1);
                    if (!IsValidInsert)
                    {
                        OrderNumbers = OrderNumbers + "," + dr.Cells[19].Text;
                    }
                }
            }
            if (IsValidInsert)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Record Updated !');", true);
                DateTime pOrderDate = DateTime.Parse(this.Session["CurrentWorkDate"].ToString()).AddDays(1);
                txtToDate.Text = pOrderDate.ToString("dd-MMM-yyyy");
                this.LoadPendingOrder();

            }
        }
    }

    protected void ChbOrderBooker_CheckedChanged(object sender, EventArgs e)
    {

    }

    protected void LstRoutes_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadDeliveryman();
    }
}
