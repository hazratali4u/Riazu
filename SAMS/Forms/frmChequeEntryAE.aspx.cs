using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;

public partial class Forms_frmChequeEntryAE : System.Web.UI.Page
{
    readonly LedgerController _ledgerCtl = new LedgerController();
    readonly SKUPriceDetailController _skuPriceController = new SKUPriceDetailController();
    readonly ChequeEntryController _cEntryController = new ChequeEntryController();
    readonly CustomerDataController _customerDataController = new CustomerDataController();
    readonly DistributorController _dController = new DistributorController();
    readonly UserController _userController = new UserController();

    private void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            DrpStatus.Items.Add(new ListItem("Cheque Received", "527"));
            DrpStatus.Items.Add(new ListItem("Cheque Deposit", "528"));
            DrpStatus.Items.Add(new ListItem("Cheque Realize", "529"));
            DrpStatus.Items.Add(new ListItem("Cheque Bounce", "530"));
            DrpStatus.Items.Add(new ListItem("Cheque Cancel", "560"));
            // 
            LoadDropdownConvertto();
            lblStatusConvertTo.Visible = false;
            DrpStatusConvertTo.Visible = false;


            DateTime pOrderDate = DateTime.Parse(Session["CurrentWorkDate"].ToString());
            txtToDate.Text = pOrderDate.ToString("dd-MMM-yyyy");
            txtFromDate.Text = pOrderDate.ToString("dd-MMM-yyyy");

            //CalendarExtender2.SelectedDate = pOrderDate;

            LoadAccountHead();
            LoadPrincipal();
            LoadDistributor();
            LoadArea();
            LoadDeliveryman();
            LoadData();
            SelectCreditInvoice();
            LoadReceviedCheque();

            //   SetCustomerBank();
            UncheckSelectAll();
            btnSave.Attributes.Add("onclick", "return ValidateForm();");


            //
            lblBankAccount.Visible = false;
            DrpBankAccount.Visible = false;

        }
    }

    #region Load

    /// <summary>
    /// Loads Locations To Location Combo
    /// </summary>
    /// 
    private void LoadDropdownConvertto()
    {
        DrpStatusConvertTo.Items.Clear();
        // DrpStatusConvertTo.Items.Add(new ListItem("Cheque Received", "527"));
        DrpStatusConvertTo.Items.Add(new ListItem("Cheque Deposit", "528"));
        DrpStatusConvertTo.Items.Add(new ListItem("Cheque Realize", "529"));
        DrpStatusConvertTo.Items.Add(new ListItem("Cheque Bounce", "530"));
        DrpStatusConvertTo.Items.Add(new ListItem("Cheque Cancel", "560"));
    }

    private void LoadDistributor()
    {

        DataTable dt = _dController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDropDownList(drpDistributor, dt, 0, 2, true);
    }

    private void LoadArea()
    {

        DistributorAreaController mController = new DistributorAreaController();
        DataTable dt = mController.SelectDist_Area(Constants.LongNullValue, Constants.DateNullValue, Constants.DateNullValue, int.Parse(drpDistributor.SelectedValue.ToString()), Constants.IntNullValue, null, null);
        clsWebFormUtil.FillDropDownList(DrpRoute, dt, 0, 6, true);

    }

    private void LoadData()
    {
        GrdCredit.DataSource = null;
        GrdCredit.DataBind();


        if (DrpChequeType.SelectedIndex == 1)
        {
            DataTable dtCustomer =
                _customerDataController.SelectPrincipalCustomer(int.Parse(drpDistributor.SelectedValue),
                    int.Parse(DrpRoute.SelectedValue), Constants.IntNullValue, int.Parse(DrpPrincipal.SelectedValue));
            clsWebFormUtil.FillDropDownList(this.DrpCustomer, dtCustomer, 0, 4, true);
            DrpRoute.Enabled = true;
        }
        else
        {
            if (drpDistributor.Items.Count > 0)
            {

                DataTable dtCredit = _ledgerCtl.SelectCreditPendingInvoice2(int.Parse(drpDistributor.SelectedValue),
                    int.Parse(DrpPrincipal.SelectedValue), Constants.LongNullValue, Constants.IntNullValue,
                    Constants.IntNullValue);
                clsWebFormUtil.FillDropDownList(DrpCustomer, dtCredit, 0, 1, true);

                DrpRoute.Enabled = false;

            }
        }

    }

    private void LoadPrincipal()
    {

        var mDt = _skuPriceController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, mDt, 0, 1, true);
    }

    private void LoadReceviedCheque()
    {
        if (DrpStatus.SelectedValue != Constants.Cheque_Clear.ToString())
        {

            DataTable dt = _cEntryController.SelectChequeEntry2(int.Parse(DrpStatus.SelectedValue), Constants.DateNullValue, Constants.DateNullValue, int.Parse(drpDistributor.SelectedValue), int.Parse(DrpPrincipal.SelectedValue), DrpChequeType.SelectedIndex);
            Session.Add("dt", dt);

            GrdOrder.DataSource = dt;
            GrdOrder.DataBind();
        }
        else
        {
            GrdOrder.DataSource = null;
            GrdOrder.DataBind();
        }
    }

    /// <summary>
    /// Loads Crdit Invoices To Grid
    /// </summary>
    private void SelectCreditInvoice()
    {

        GrdCredit.DataSource = null;
        GrdCredit.DataBind();
        if (DrpCustomer.Items.Count > 0 && DrpChequeType.SelectedIndex != 1)
        {
            DataTable dtCredit = _ledgerCtl.SelectCreditPendingInvoice(int.Parse(drpDistributor.SelectedValue), int.Parse(DrpPrincipal.SelectedValue), long.Parse(DrpCustomer.SelectedValue), 0);
            GrdCredit.DataSource = dtCredit;
            GrdCredit.DataBind();
            decimal objtotalAmount = 0;
            if (DrpStatus.SelectedIndex == 0)
            {   
                foreach (GridViewRow dr in GrdCredit.Rows)
                {
                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                    chRelized.Checked = true;
                    //chRelized.Enabled = false;
                }
            }
            foreach (DataRow row in dtCredit.Rows)
            {
                objtotalAmount += decimal.Parse(row["CURRENT_CREDIT_AMOUNT"].ToString());
            }
            txtTotalCreditAmount.Text = Convert.ToString(Math.Round(objtotalAmount, 2));
        }
    }

    /// <summary>
    /// Saves Cheque Realization
    /// </summary>
    private void ChequeRealization()
    {

        string maxDocumentId = _ledgerCtl.SelectLedgerMaxDocumentId(Constants.Bank_Voucher, int.Parse(drpDistributor.SelectedValue));
        Session.Add("VoucherNo", maxDocumentId);
        decimal offerAmount = decimal.Parse(txtAmount.Text);
        decimal realizeAmount = 0;
        if (DrpChequeType.SelectedIndex == 0)
        {
            foreach (GridViewRow dr in GrdCredit.Rows)
            {
                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                if (chRelized.Checked == true)
                {
                    if (decimal.Parse(dr.Cells[3].Text) >= offerAmount)
                    {
                        realizeAmount += offerAmount;
                        _ledgerCtl.PostingCash_Bank_Account(Constants.Bank_Voucher, long.Parse(maxDocumentId), 107, int.Parse(drpDistributor.SelectedValue.ToString()), 0, offerAmount,
                        DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), "Cheque Relization", DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()),
                        txtChequeNo.Text, int.Parse(this.Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), dr.Cells[1].Text, Constants.Document_Invoice, txtSlipNo.Text, Constants.DateNullValue, 18, DrpDeliveryMan.SelectedValue.ToString());

                        _ledgerCtl.PostingCash_Bank_Account(Constants.Bank_Voucher, long.Parse(maxDocumentId), long.Parse(DrpBankAccount.SelectedValue.ToString()), int.Parse(drpDistributor.SelectedValue.ToString()), offerAmount, 0,
                        DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), "Cheque Relization", DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()),
                        txtChequeNo.Text, int.Parse(this.Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), dr.Cells[1].Text, Constants.Document_Invoice, txtSlipNo.Text, Constants.DateNullValue, 18, DrpDeliveryMan.SelectedValue.ToString());

                        offerAmount = decimal.Parse(dr.Cells[3].Text) - offerAmount;
                        _ledgerCtl.UpdateSaleInvoice(Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), int.Parse(drpDistributor.SelectedValue.ToString()), offerAmount);
                        break;
                    }
                    else if (decimal.Parse(dr.Cells[3].Text) <= offerAmount)
                    {
                        realizeAmount += Convert.ToDecimal(dr.Cells[3].Text);
                        _ledgerCtl.PostingCash_Bank_Account(Constants.Bank_Voucher, long.Parse(maxDocumentId), 107, int.Parse(drpDistributor.SelectedValue.ToString()), 0, decimal.Parse(dr.Cells[3].Text),
                        DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), "Cheque Relization", DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()),
                        txtChequeNo.Text, int.Parse(this.Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), dr.Cells[1].Text, Constants.Document_Invoice, txtSlipNo.Text, Constants.DateNullValue, 18, DrpDeliveryMan.SelectedValue.ToString());

                        _ledgerCtl.PostingCash_Bank_Account(Constants.Bank_Voucher, long.Parse(maxDocumentId), long.Parse(DrpBankAccount.SelectedValue.ToString()), int.Parse(drpDistributor.SelectedValue.ToString()), decimal.Parse(dr.Cells[3].Text), 0,
                        DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), "Cheque Relization", DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()),
                        txtChequeNo.Text, int.Parse(this.Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), dr.Cells[1].Text, Constants.Document_Invoice, txtSlipNo.Text, Constants.DateNullValue, 18, DrpDeliveryMan.SelectedValue.ToString());

                        offerAmount = offerAmount - decimal.Parse(dr.Cells[3].Text);
                        _ledgerCtl.UpdateSaleInvoice(Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), int.Parse(drpDistributor.SelectedValue.ToString()), 0);
                    }
                }
            }


            if (realizeAmount < decimal.Parse(txtAmount.Text))// remaining amount as advance
            {

                _ledgerCtl.PostingCash_Bank_Account(Constants.Bank_Voucher, long.Parse(maxDocumentId), 107, int.Parse(drpDistributor.SelectedValue.ToString()), 0, offerAmount,
                               DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), "Cheque Advance", DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()),
                               txtChequeNo.Text, int.Parse(this.Session["UserId"].ToString()), Constants.LongNullValue, null, Constants.IntNullValue, txtSlipNo.Text, Constants.DateNullValue, 20, "");

                _ledgerCtl.PostingCash_Bank_Account(Constants.Bank_Voucher, long.Parse(maxDocumentId), long.Parse(DrpBankAccount.SelectedValue.ToString()), int.Parse(drpDistributor.SelectedValue.ToString()), offerAmount, 0,
                    DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), "Cheque Advance", DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()),
                    txtChequeNo.Text, int.Parse(this.Session["UserId"].ToString()), Constants.LongNullValue, null, Constants.IntNullValue, txtSlipNo.Text, Constants.DateNullValue, 20, "");

            }
        }
        else
        {
            _ledgerCtl.PostingCash_Bank_Account(Constants.Bank_Voucher, long.Parse(maxDocumentId), 107, int.Parse(drpDistributor.SelectedValue.ToString()), 0, decimal.Parse(txtAmount.Text),
                DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), "Cheque Advance", DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()),
                txtChequeNo.Text, int.Parse(this.Session["UserId"].ToString()), Constants.LongNullValue, null, Constants.IntNullValue, txtSlipNo.Text, Constants.DateNullValue, 20, "");

            _ledgerCtl.PostingCash_Bank_Account(Constants.Bank_Voucher, long.Parse(maxDocumentId), long.Parse(DrpBankAccount.SelectedValue.ToString()), int.Parse(drpDistributor.SelectedValue.ToString()), decimal.Parse(txtAmount.Text), 0,
                DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), "Cheque Advance", DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()),
                txtChequeNo.Text, int.Parse(this.Session["UserId"].ToString()), Constants.LongNullValue, null, Constants.IntNullValue, txtSlipNo.Text, Constants.DateNullValue, 20, "");
        }

    }



    private void LoadDeliveryman()
    {
        if (drpDistributor.Items.Count > 0)
        {
            SaleForceController mDController = new SaleForceController();
            DataTable m_dt = mDController.SelectSaleForceAssignedArea(int.Parse(drpDistributor.SelectedValue.ToString()), Constants.IntNullValue, int.Parse(this.Session["CompanyId"].ToString()));
            clsWebFormUtil.FillDropDownList(this.DrpDeliveryMan, m_dt, 0, 3, true);
        }

    }

    private void LoadAccountHead()
    {
        Configuration.GetAccountHead();
        AccountHeadController mAccountController = new AccountHeadController();
        DataTable dt = mAccountController.SelectAccountHead(Constants.AC_AccountHeadId, long.Parse(Configuration.BankDefaultType));
        clsWebFormUtil.FillDropDownList(DrpBankAccount, dt, 0, 4, true);
        try
        {
            string temp_name = this.Session["UserName"].ToString();
            var arr = temp_name.Split('-');
            var Compny_name = arr[0].ToString();
            if (Compny_name.Trim() == "FC")
            {
                DrpBankAccount.SelectedValue = "599";//sonery bank for food corp
            }
        }
        catch (Exception ex)
        {

        }

    }

    #endregion

    #region Click Operations
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (IsDayClosed())
        {
            _userController.InsertUserLogoutTime(Convert.ToInt32(Session["User_Log_ID"]), Convert.ToInt32(Session["UserID"]));
            Session.Clear();
            System.Web.Security.FormsAuthentication.SignOut();
            Response.Redirect("../Login.aspx");
        }

        else
        {
            DateTime chequeDate;

            if (txtStartDate.Text.Length == 10)
            {
                chequeDate = DateTime.Parse(ConvertDate.British_To_American(txtStartDate.Text));

            }
            else
            {
                chequeDate = DateTime.Now;
            }

            int invoiceCount = Constants.IntNullValue;

            decimal invoicesTotalAmount = 0;

            if (btnSave.Text == "Save")
            {
                foreach (GridViewRow dr in GrdCredit.Rows)
                {
                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");

                    if (chRelized.Checked == true)
                    {
                        invoiceCount++;
                        invoicesTotalAmount = invoicesTotalAmount + decimal.Parse(dr.Cells[3].Text);
                        // break;
                    }
                }

                if (invoiceCount == Constants.IntNullValue && DrpChequeType.SelectedIndex == 0)
                {

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Must Select Invoice');", true);
                    return;
                }
                if (decimal.Parse(txtAmount.Text) > invoicesTotalAmount && DrpChequeType.SelectedIndex == 0)
                {
                    if (!chbIsAdvance.Checked)
                    {
                        chbIsAdvance.Visible = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Advance Amount Alert, Must Allow Advance Amount Entry !');", true);
                        return;
                    }

                }

                if (DrpStatus.SelectedIndex == 0)
                {
                    HFChqueProcessId.Value = _cEntryController.InsertChequeEntry(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()), long.Parse(DrpCustomer.SelectedValue.ToString()), txtChequeNo.Text, txtAccountNo.Text, chequeDate,
                       DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), Constants.DateNullValue, Constants.DateNullValue, decimal.Parse(txtAmount.Text), int.Parse(DrpStatus.SelectedValue.ToString()), DateTime.Now, DrpChequeType.SelectedIndex, txtSlipNo.Text, txtRemarks.Text, long.Parse(DrpBankAccount.SelectedValue.ToString()));


                    foreach (GridViewRow dr in GrdCredit.Rows)
                    {
                        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                        if (chRelized.Checked == true)
                        {
                            _cEntryController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]));
                        }
                    }
                    SelectCreditInvoice();
                }
            }
            else if (btnSave.Text == "Edit")
            {
                #region cheque pending
                if (int.Parse(DrpStatus.SelectedValue.ToString()) == Constants.Cheque_Pending &&
                    txtReceivedDate.Text ==
                    DateTime.Parse(this.Session["CurrentWorkDate"].ToString()).ToString("dd/MM/yyyy"))
                {
                    //cEntryController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()), long.Parse(DrpCustomer.SelectedValue.ToString()), txtChequeNo.Text, txtBankName.Text, ChequeDate, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), Constants.DateNullValue, Constants.DateNullValue,
                    //   decimal.Parse(txtAmount.Text), int.Parse(DrpStatus.SelectedValue.ToString()), Constants.DateNullValue, txtSlipNo.Text, DrpChequeType.SelectedIndex, txtRemarks.Text, int.Parse(DrpBankAccount.SelectedValue.ToString()));
                    // by safdar .. bank ID 
                    _cEntryController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value),
                        int.Parse(drpDistributor.SelectedValue.ToString()),
                        int.Parse(DrpPrincipal.SelectedValue.ToString()),
                        long.Parse(DrpCustomer.SelectedValue.ToString()), txtChequeNo.Text,
                        txtAccountNo.Text, chequeDate,
                        DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), Constants.DateNullValue,
                        Constants.DateNullValue,
                        decimal.Parse(txtAmount.Text), int.Parse(DrpStatus.SelectedValue.ToString()),
                        Constants.DateNullValue, txtSlipNo.Text, DrpChequeType.SelectedIndex, txtRemarks.Text,
                        int.Parse(DrpBankAccount.SelectedValue.ToString()));
                    _cEntryController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
                    foreach (GridViewRow dr in GrdCredit.Rows)
                    {
                        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                        if (chRelized.Checked == true)
                        {
                            _cEntryController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value),
                                Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]));
                        }
                    }
                }
                #endregion
                #region cheque deposit
                else if (int.Parse(DrpStatus.SelectedValue.ToString()) == Constants.Cheque_Deposit)
                {
                    _cEntryController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue,
                        Constants.IntNullValue, Constants.LongNullValue, txtChequeNo.Text, txtAccountNo.Text, Constants.DateNullValue,
                        Constants.DateNullValue, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()),
                        Constants.DateNullValue,
                        Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedValue.ToString()),
                        Constants.DateNullValue, txtSlipNo.Text, DrpChequeType.SelectedIndex, txtRemarks.Text,
                        int.Parse(DrpBankAccount.SelectedValue.ToString()));
                }
                #endregion

                #region cheque bons or cancel

                //else if (int.Parse(DrpStatus.SelectedValue.ToString()) == Constants.Cheque_Bons ||
                //         int.Parse(DrpStatus.SelectedValue.ToString()) == Constants.Cheque_Cancel)
                //{
                //    _cEntryController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue,
                //        Constants.IntNullValue, Constants.LongNullValue, null, null, Constants.DateNullValue,
                //        Constants.DateNullValue, Constants.DateNullValue,
                //        DateTime.Parse(this.Session["CurrentWorkDate"].ToString()),
                //        Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedValue.ToString()),
                //        Constants.DateNullValue, txtSlipNo.Text, DrpChequeType.SelectedIndex, txtRemarks.Text,
                //        long.Parse(DrpBankAccount.SelectedValue.ToString()));

                //}
                #endregion

                #region cheque clear

                //else if (int.Parse(DrpStatus.SelectedValue.ToString()) == Constants.Cheque_Clear)
                //{

                //    _cEntryController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue,
                //        Constants.IntNullValue, Constants.LongNullValue, null, null, Constants.DateNullValue,
                //        Constants.DateNullValue, Constants.DateNullValue,
                //        DateTime.Parse(this.Session["CurrentWorkDate"].ToString()),
                //        Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedValue.ToString()),
                //        Constants.DateNullValue, null, DrpChequeType.SelectedIndex, txtRemarks.Text,
                //        long.Parse(DrpBankAccount.SelectedValue.ToString()));

                //    _cEntryController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);

                //    foreach (GridViewRow dr in GrdCredit.Rows)
                //    {
                //        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                //        if (chRelized.Checked == true)
                //        {
                //            _cEntryController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value),
                //                Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]));
                //        }
                //    }
                //    this.ChequeRealization();

                //    this.IncomeTax3();
                //    this.SelectCreditInvoice();
                //    this.LoadData();
                //}
                #endregion
                ClearAll();
                LoadReceviedCheque();
            }
            else if (btnSave.Text == "Update")
            {
                foreach (GridViewRow drow in GrdOrder.Rows)
                {
                    CheckBox chSelect = (CheckBox)drow.Cells[0].FindControl("ChbIsSelect");
                    if (chSelect.Checked == true)
                    {
                        HFChqueProcessId.Value = drow.Cells[0].Text;

                        #region cheque pending

                        //if (int.Parse(DrpStatusConvertTo.SelectedValue.ToString()) == Constants.Cheque_Pending &&
                        //    txtReceivedDate.Text ==
                        //    DateTime.Parse(this.Session["CurrentWorkDate"].ToString()).ToString("dd/MM/yyyy"))
                        //{
                        //    //cEntryController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()), long.Parse(DrpCustomer.SelectedValue.ToString()), txtChequeNo.Text, txtBankName.Text, ChequeDate, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), Constants.DateNullValue, Constants.DateNullValue,
                        //    //   decimal.Parse(txtAmount.Text), int.Parse(DrpStatusConvertTo.SelectedValue.ToString()), Constants.DateNullValue, txtSlipNo.Text, DrpChequeType.SelectedIndex, txtRemarks.Text, int.Parse(DrpBankAccount.SelectedValue.ToString()));
                        //    //  .. drpcustomerbank instead of bank text box
                        //    _cEntryController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value),
                        //        int.Parse(drpDistributor.SelectedValue.ToString()),
                        //        int.Parse(DrpPrincipal.SelectedValue.ToString()),
                        //        long.Parse(DrpCustomer.SelectedValue.ToString()), txtChequeNo.Text,
                        //        txtBankName.Text, chequeDate,
                        //        DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), Constants.DateNullValue,
                        //        Constants.DateNullValue,
                        //        decimal.Parse(txtAmount.Text), int.Parse(DrpStatusConvertTo.SelectedValue.ToString()),
                        //        Constants.DateNullValue, txtSlipNo.Text, DrpChequeType.SelectedIndex, txtRemarks.Text,
                        //        int.Parse(DrpBankAccount.SelectedValue.ToString()));

                        //    _cEntryController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);

                        //    foreach (GridViewRow dr in GrdCredit.Rows)
                        //    {
                        //        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                        //        if (chRelized.Checked == true)
                        //        {
                        //            _cEntryController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value),
                        //                Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]));
                        //        }
                        //    }
                        //}
                        #endregion
                        #region cheque deposit

                        if (int.Parse(DrpStatusConvertTo.SelectedValue.ToString()) == Constants.Cheque_Deposit)
                        {
                            long x = long.Parse(HFChqueProcessId.Value);
                            _cEntryController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value),
                                Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, txtChequeNo.Text, txtAccountNo.Text,
                                Constants.DateNullValue, Constants.DateNullValue,
                                DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), Constants.DateNullValue,
                                Constants.DecimalNullValue, int.Parse(DrpStatusConvertTo.SelectedValue.ToString()),
                                Constants.DateNullValue, txtSlipNo.Text, DrpChequeType.SelectedIndex, txtRemarks.Text,
                                int.Parse(DrpBankAccount.SelectedValue.ToString()));
                        }
                        #endregion
                        #region cheque bons or cancel

                        else if (int.Parse(DrpStatusConvertTo.SelectedValue.ToString()) == Constants.Cheque_Bons ||
                                 int.Parse(DrpStatusConvertTo.SelectedValue.ToString()) == Constants.Cheque_Cancel)
                        {
                            _cEntryController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value),
                                Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, txtChequeNo.Text, txtAccountNo.Text,
                                Constants.DateNullValue, Constants.DateNullValue, Constants.DateNullValue,
                                DateTime.Parse(this.Session["CurrentWorkDate"].ToString()),
                                Constants.DecimalNullValue, int.Parse(DrpStatusConvertTo.SelectedValue.ToString()),
                                Constants.DateNullValue, txtSlipNo.Text, DrpChequeType.SelectedIndex,
                                txtRemarks.Text, long.Parse(DrpBankAccount.SelectedValue.ToString()));

                        }
                        #endregion
                        #region cheque clear

                        else if (int.Parse(DrpStatusConvertTo.SelectedValue.ToString()) == Constants.Cheque_Clear)
                        {

                            try
                            {
                                _cEntryController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value),
                                                                    Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, txtChequeNo.Text, txtAccountNo.Text,
                                                                    Constants.DateNullValue, Constants.DateNullValue, Constants.DateNullValue,
                                                                    DateTime.Parse(this.Session["CurrentWorkDate"].ToString()),
                                                                    Constants.DecimalNullValue, int.Parse(DrpStatusConvertTo.SelectedValue.ToString()),
                                                                    Constants.DateNullValue, txtSlipNo.Text, DrpChequeType.SelectedIndex,
                                                                    txtRemarks.Text, long.Parse(DrpBankAccount.SelectedValue.ToString()));

                                foreach (GridViewRow dr in GrdCredit.Rows)
                                {
                                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    if (chRelized.Checked == true)
                                    {
                                        _cEntryController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value),
                                            Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]));
                                    }
                                }

                                #region Load data on edit button

                                HFChqueProcessId.Value = drow.Cells[0].Text;
                                DrpRoute.SelectedValue = drow.Cells[13].Text;
                                LoadData();
                                DrpCustomer.SelectedValue = drow.Cells[1].Text;
                                txtChequeNo.Text = drow.Cells[4].Text;
                                //  txtBankName.Text = drow.Cells[5].Text;
                                SelectCustomerBank(drow.Cells[5].Text);
                                txtStartDate.Text = drow.Cells[7].Text;
                                txtReceivedDate.Text = drow.Cells[8].Text;
                                txtAmount.Text = drow.Cells[10].Text;
                                txtSlipNo.Text = drow.Cells[11].Text;
                                txtRemarks.Text = drow.Cells[12].Text;
                                DrpBankAccount.SelectedValue = drow.Cells[15].Text;
                                try
                                {
                                    DrpDeliveryMan.SelectedValue = drow.Cells[16].Text;
                                }
                                catch (Exception ex)
                                {
                                    DrpDeliveryMan.SelectedIndex = 0;
                                }
                                SelectCreditInvoice();
                                DataTable dt =
                                    _cEntryController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 0);

                                foreach (GridViewRow dr in GrdCredit.Rows)
                                {
                                    foreach (DataRow dbr in dt.Rows)
                                    {
                                        if (Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]) == Convert.ToInt64(dbr["SALE_INVOICE_ID"]))
                                        {
                                            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                            chRelized.Checked = true;
                                        }
                                    }
                                }


                                #endregion

                                this.ChequeRealization();
                                InsertGL();
                                // this.IncomeTax3();
                                this.LoadData();
                                this.SelectCreditInvoice();
                            }
                            catch (Exception ex)
                            {
                                ExceptionPublisher.PublishException(ex);
                            }
                        }

                        #endregion
                    }

                }

            }

            ClearAll();
            LoadReceviedCheque();
        }
    }
    private void InsertGL()
    {
        DataTable dtVoucher = new DataTable();
        dtVoucher.Columns.Add("LEDGER_ID", typeof(long));
        dtVoucher.Columns.Add("ACCOUNT_HEAD_ID", typeof(long));
        dtVoucher.Columns.Add("DEBIT", typeof(decimal));
        dtVoucher.Columns.Add("CREDIT", typeof(decimal));
        dtVoucher.Columns.Add("REMARKS", typeof(string));
        dtVoucher.Columns.Add("Principal_Id", typeof(string));

        ///Debit Side Entry
        DataRow dr = dtVoucher.NewRow();
        dr["LEDGER_ID"] = Constants.LongNullValue.ToString();
        dr["ACCOUNT_HEAD_ID"] = Convert.ToInt64(DrpBankAccount.SelectedValue);
        dr["REMARKS"] = DrpBankAccount.SelectedItem.Text + " received from " + DrpCustomer.SelectedItem.Text;
        dr["DEBIT"] = decimal.Parse(txtAmount.Text);
        dr["CREDIT"] = 0;
        dr["Principal_Id"] = 0;
        dtVoucher.Rows.Add(dr);

        ///Credit Side Entry
        DataRow dr1 = dtVoucher.NewRow();
        dr1["LEDGER_ID"] = Constants.LongNullValue.ToString();
        dr1["DEBIT"] = 0;
        dr1["CREDIT"] = decimal.Parse(txtAmount.Text);
        dr1["Principal_Id"] = 0;
        dr1["ACCOUNT_HEAD_ID"] = 107; // 3002020001 Account Receivable (Party Wise)
        dr1["REMARKS"] = DrpBankAccount.SelectedItem.Text + " received from " + DrpCustomer.SelectedItem.Text;
        dtVoucher.Rows.Add(dr1);

        LedgerController LController = new LedgerController();
        string MaxDocumentId = "";
        MaxDocumentId = LController.SelectMaxVoucherId(Constants.Bank_Voucher, Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToDateTime(Session["CurrentWorkDate"]));
        LController.Add_Voucher_AE(Convert.ToInt32(drpDistributor.SelectedValue), Convert.ToInt32(DrpPrincipal.SelectedValue), MaxDocumentId, Constants.Bank_Voucher, Convert.ToDateTime(Session["CurrentWorkDate"]), Constants.Bank_Deposit, Session["VoucherNo"].ToString(), "Default " + DrpBankAccount.SelectedItem.Text + " Voucher" + "-" + txtRemarks.Text, Convert.ToDateTime(Session["CurrentWorkDate"]), txtChequeNo.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), txtSlipNo.Text, Constants.DateNullValue);
        //PrintReport(MaxDocumentId);
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearAll();
    }

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        DataTable dt = (DataTable)Session["dt"];

        switch (ddSearchType.SelectedIndex)
        {
            case 1:
                dt.DefaultView.RowFilter = ddSearchType.SelectedValue + " like '%" + txtSeach.Text + "%'";//+ "AND " + "CHEQUE_DATE " + " like '%" + x + "%'";
                break;
            case 2:
                dt.DefaultView.RowFilter = ddSearchType.SelectedValue + " like '%" + txtSeach.Text + "%'";// + "AND " + "CHEQUE_DATE " + " like '%" + x + "%'";
                break;
            case 3:
                dt.DefaultView.RowFilter = ddSearchType.SelectedValue + " like '%" + txtSeach.Text + "%'";//+ "AND " + "CHEQUE_DATE " + " like '%" + x + "%'";
                break;
            case 4:
                dt.DefaultView.RowFilter = ddSearchType.SelectedValue + " like '%" + txtSeach.Text + "%'";//+ "AND " + "CHEQUE_DATE " + " like '%" + x + "%'";
                break;
            case 5:

                dt = _cEntryController.SelectChequeEntry3(int.Parse(DrpStatus.SelectedValue), DateTime.Parse(txtFromDate.Text + " 00:00:00"), DateTime.Parse(txtToDate.Text + " 23:59:59"), int.Parse(drpDistributor.SelectedValue), int.Parse(DrpPrincipal.SelectedValue), DrpChequeType.SelectedIndex);
                GrdOrder.DataSource = dt;
                GrdOrder.DataBind();

                break;
            //  dt.DefaultView.RowFilter = "CHEQUE_DATE >= '" + txtFromDate.Text + " 00:00:00" + "'" + " AND " + "CHEQUE_DATE <= '" + txtToDate.Text + " 23:59:59" + "' ";
            //  break;
            default:
                dt.DefaultView.RowFilter = "CHEQUE_NO" + " like '%" + "" + "%'";// + "AND " + "CHEQUE_DATE" + " like '%" + x + "%'";
                break;
        }
        GrdOrder.DataSource = dt.DefaultView;
        GrdOrder.DataBind();
    }
    #endregion
    #region Grid Operations

    /// <summary>
    /// Deletes Cheque
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void GrdOrder_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {

        _cEntryController.DeleteChequeEntry(long.Parse(GrdOrder.Rows[e.RowIndex].Cells[0].Text));
        LoadReceviedCheque();
    }
    /// <summary>
    /// Sets Cheque Data For Edit. This Function Runs When An Existing Cheque Needs To Be Edited
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void GrdOrder_RowEditing(object sender, GridViewEditEventArgs e)
    {

        try
        {
            // 
            DrpStatus.Enabled = false;


            HFChqueProcessId.Value = GrdOrder.Rows[e.NewEditIndex].Cells[0].Text;


            LoadData();

            DrpCustomer.SelectedValue = GrdOrder.Rows[e.NewEditIndex].Cells[1].Text;
            // by safdar .. drpbank 
            SelectCustomerBank(GrdOrder.Rows[e.NewEditIndex].Cells[5].Text);
            // DrpCustomerBank.SelectedValue = GrdOrder.Rows[e.NewEditIndex].Cells[5].Text;
            // DrpCustomerBank.Enabled = false;
            txtChequeNo.Text = GrdOrder.Rows[e.NewEditIndex].Cells[4].Text;

            txtStartDate.Text = GrdOrder.Rows[e.NewEditIndex].Cells[7].Text;
            txtReceivedDate.Text = GrdOrder.Rows[e.NewEditIndex].Cells[8].Text;
            txtAmount.Text = GrdOrder.Rows[e.NewEditIndex].Cells[10].Text;
            txtSlipNo.Text = GrdOrder.Rows[e.NewEditIndex].Cells[11].Text.Replace("&nbsp;", "");
            txtRemarks.Text = GrdOrder.Rows[e.NewEditIndex].Cells[12].Text.Replace("&nbsp;", "");
            txtAccountNo.Text = GrdOrder.Rows[e.NewEditIndex].Cells[6].Text.Replace("&nbsp;", "");
            lblBankAccount.Visible = true;
            DrpBankAccount.Visible = true;
            DrpBankAccount.SelectedValue = GrdOrder.Rows[e.NewEditIndex].Cells[15].Text;
            try
            {
                LoadDeliveryman();
                DrpDeliveryMan.SelectedValue = GrdOrder.Rows[e.NewEditIndex].Cells[16].Text;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Sale Force not found.');", true);
            }

            btnSave.Text = "Edit";
            SelectCreditInvoice();

            DataTable dt = _cEntryController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 0);

            foreach (GridViewRow dr in GrdCredit.Rows)
            {
                foreach (DataRow dbr in dt.Rows)
                {

                    if (Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]) == Convert.ToInt64(dbr["SALE_INVOICE_ID"]))
                    {
                        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                        chRelized.Checked = true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Invoice not found for selected cheque');", true);
        }
    }

    #endregion
    #region Sel/Index Change

    /// <summary>
    /// Loads Credit Invoices
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void DrpCustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
        // LoadData();
        SelectCreditInvoice();
        //   SetCustomerBank();
    }


    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadArea();
        LoadData();
        SelectCreditInvoice();
        LoadReceviedCheque();
    }
    protected void DrpDeliveryMan_SelectedIndexChanged(object sender, EventArgs e)
    {
        //  LoadData();
        //SelectCreditInvoice();
        // SetCustomerBank();
    }

    protected void DrpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadData();
        this.SelectCreditInvoice();
        this.LoadReceviedCheque();
    }

    protected void DrpStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (int.Parse(DrpStatus.SelectedValue) == Constants.Cheque_Pending)
        {
            DrpStatusConvertTo.Items.Clear();
            DrpStatusConvertTo.Items.Add(new ListItem("Cheque Deposit", "528"));
            DrpStatusConvertTo.Items.Add(new ListItem("Cheque Realize", "529"));
            DrpStatusConvertTo.Items.Add(new ListItem("Cheque Bounce", "530"));
            DrpStatusConvertTo.Items.Add(new ListItem("Cheque Cancel", "560"));
        }
        else if (int.Parse(DrpStatus.SelectedValue) == Constants.Cheque_Deposit)
        {
            DrpStatusConvertTo.Items.Clear();

            DrpStatusConvertTo.Items.Add(new ListItem("Cheque Realize", "529"));
            DrpStatusConvertTo.Items.Add(new ListItem("Cheque Bounce", "530"));
            DrpStatusConvertTo.Items.Add(new ListItem("Cheque Cancel", "560"));
        }
        else
        {
            LoadDropdownConvertto();
        }



        if (btnSave.Text == "Save")
        {
            LoadReceviedCheque();
        }
    }


    protected void DrpChequeType_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadData();
        this.LoadReceviedCheque();
    }


    protected void DrpRoute_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadData();
        this.SelectCreditInvoice();
    }

    protected void GrdOrder_SelectedIndexChanged(object sender, EventArgs e)
    {


    }

    protected void ChbIsSelect_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridViewRow row = ((GridViewRow)((CheckBox)sender).NamingContainer);
        int index = row.RowIndex;
        CheckBox cb1 = (CheckBox)GrdOrder.Rows[index].FindControl("ChbIsSelect");

        if (cb1.Checked == true)
        {
            #region check
            if (int.Parse(DrpStatus.SelectedValue) == Constants.Cheque_Deposit)
            {
                if (DrpChequeType.SelectedIndex == 0)
                {
                    lblsaleforce.Visible = false;
                    DrpDeliveryMan.Visible = false;
                }
                lblcustomer.Visible = false;
                DrpCustomer.Visible = false;
                lblroute.Visible = false;
                DrpRoute.Visible = false;
                GrdCredit.Visible = false;
                Panel1.Visible = false;
                //DrpPrincipal.Enabled = false;
                DrpBankAccount.Enabled = false;
                //DrpChequeType.Enabled = false;
                //DrpRoute.Enabled = false;

                txtAccountNo.Enabled = false;
                lblBankAccount.Visible = true;
                DrpBankAccount.Visible = true;


            }
            DrpStatus.Enabled = false;
            txtChequeNo.ReadOnly = true;
            //  txtBankName.ReadOnly = true;
            //   DrpCustomerBank.Enabled = false;
            txtAmount.ReadOnly = true;
            txtStartDate.ReadOnly = true;
            lblStatusConvertTo.Visible = true;
            DrpStatusConvertTo.Visible = true;
            btnSave.Text = "Update";
            lblBankAccount.Visible = true;
            DrpBankAccount.Visible = true;
            txtAccountNo.Enabled = false;

            for (int i = 0; i < GrdOrder.Columns.Count; i++)
            {
                if (GrdOrder.Columns[i] is CommandField)
                {
                    GrdOrder.Columns[i].Visible = false;
                }
            }

            #endregion
        }

        else if (cb1.Checked == false)
        {

            #region Unchecked
            if (int.Parse(DrpStatus.SelectedValue) == Constants.Cheque_Deposit)
            {
                lblsaleforce.Visible = true;
                DrpDeliveryMan.Visible = true;
                lblcustomer.Visible = true;
                DrpCustomer.Visible = true;
                lblroute.Visible = true;
                DrpRoute.Visible = true;
                GrdCredit.Visible = true;
                Panel1.Visible = true;
                //DrpPrincipal.Enabled = false;
                //DrpBankAccount.Enabled = false;
                //DrpChequeType.Enabled = false;
                //DrpRoute.Enabled = false;

                txtAccountNo.Enabled = true;
                lblBankAccount.Visible = false;
                DrpBankAccount.Visible = false;

            }
            DrpStatus.Enabled = true;
            txtChequeNo.ReadOnly = false;
            //  txtBankName.ReadOnly = true;
            DrpCustomerBank.Enabled = true;
            txtAmount.ReadOnly = false;
            txtStartDate.ReadOnly = false;
            lblStatusConvertTo.Visible = false;
            DrpStatusConvertTo.Visible = false;
            btnSave.Text = "Save";
            lblBankAccount.Visible = false;
            DrpBankAccount.Visible = false;
            txtAccountNo.Enabled = true;


            for (int i = 0; i < GrdOrder.Columns.Count; i++)
            {
                if (GrdOrder.Columns[i] is CommandField)
                {
                    GrdOrder.Columns[i].Visible = true;
                }
            }



            #endregion
        }



    }

    protected void txtAccountNo_TextChanged(object sender, EventArgs e)
    {
        //lblstatus.ForeColor = System.Drawing.Color.Black;
        //lblstatus.Visible = false;
        //lblErrorMsg.Visible = false;
        //string AccountNo = txtAccountNo.Text;
        //DataTable dtBankInfo = (DataTable)this.Session["dtBankInfo"];
        //DataRow[] foundRows = dtBankInfo.Select("Account_No = '" + AccountNo + "'");

        //if (foundRows.Length > 0)
        //{
        //    string Bank = foundRows[0]["Bank_Name"].ToString();
        //    this.Session.Add("BankID", foundRows[0]["Bank_ID"]);
        //      lblstatus .Visible = true;
        //      lblstatus.Text = Bank;
        //}
        //else{
        //    lblstatus.ForeColor   = System.Drawing.Color.Red;
        //    lblstatus.Visible = true;
        //    lblstatus.Text = "Account Mismatch";
        //    this.Session.Add("BankID",0);
        //}


    }

    protected void ChbSelectAll_CheckedChanged1(object sender, EventArgs e)
    {
        if (ChbSelectAll.Checked == true)
        {
            #region checked
            if (int.Parse(DrpStatus.SelectedValue) == Constants.Cheque_Deposit)
            {
                lblsaleforce.Visible = false;
                DrpDeliveryMan.Visible = false;
                lblcustomer.Visible = false;
                DrpCustomer.Visible = false;
                lblroute.Visible = false;
                DrpRoute.Visible = false;
                GrdCredit.Visible = false;
                Panel1.Visible = false;
                //DrpPrincipal.Enabled = false;
                //DrpBankAccount.Enabled = false;
                //DrpChequeType.Enabled = false;
                //DrpRoute.Enabled = false;

                txtAccountNo.Enabled = false;
                lblBankAccount.Visible = true;
                DrpBankAccount.Visible = true;


            }
            DrpStatus.Enabled = false;
            txtChequeNo.ReadOnly = true;
            //  txtBankName.ReadOnly = true;
            DrpCustomerBank.Enabled = false;
            txtAmount.ReadOnly = true;
            txtStartDate.ReadOnly = true;
            lblStatusConvertTo.Visible = true;
            DrpStatusConvertTo.Visible = true;
            btnSave.Text = "Update";
            lblBankAccount.Visible = true;
            DrpBankAccount.Visible = true;
            txtAccountNo.Enabled = false;

            for (int i = 0; i < GrdOrder.Columns.Count; i++)
            {
                if (GrdOrder.Columns[i] is CommandField)
                {
                    GrdOrder.Columns[i].Visible = false;
                }
            }


            #endregion
        }
        else
        {
            #region Unchecked
            if (int.Parse(DrpStatus.SelectedValue) == Constants.Cheque_Deposit)
            {
                lblsaleforce.Visible = true;
                DrpDeliveryMan.Visible = true;
                lblcustomer.Visible = true;
                DrpCustomer.Visible = true;
                lblroute.Visible = true;
                DrpRoute.Visible = true;
                GrdCredit.Visible = true;
                Panel1.Visible = true;
                //DrpPrincipal.Enabled = false;
                //DrpBankAccount.Enabled = false;
                //DrpChequeType.Enabled = false;
                //DrpRoute.Enabled = false;

                txtAccountNo.Enabled = true;
                lblBankAccount.Visible = false;
                DrpBankAccount.Visible = false;



            }
            DrpStatus.Enabled = true;
            txtChequeNo.ReadOnly = false;
            //  txtBankName.ReadOnly = true;
            DrpCustomerBank.Enabled = true;
            txtAmount.ReadOnly = false;
            txtStartDate.ReadOnly = false;
            lblStatusConvertTo.Visible = false;
            DrpStatusConvertTo.Visible = false;
            btnSave.Text = "Save";
            lblBankAccount.Visible = false;
            DrpBankAccount.Visible = false;
            txtAccountNo.Enabled = true;


            for (int i = 0; i < GrdOrder.Columns.Count; i++)
            {
                if (GrdOrder.Columns[i] is CommandField)
                {
                    GrdOrder.Columns[i].Visible = true;
                }
            }



            #endregion
        }
    }

    #endregion
    private bool IsDayClosed()
    {
        bool flag = false;
        DistributorController DistrCtl = new DistributorController();
        DataTable dtDayClose = DistrCtl.MaxDayClose(Convert.ToInt32(drpDistributor.SelectedValue), 3);
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
    private void SelectCustomerBank(string bankId)
    {
        try
        {
            DataTable dtBankInfo = _customerDataController.UspSelectCustomerBankID(long.Parse(DrpCustomer.SelectedValue));
            clsWebFormUtil.FillDropDownList(DrpCustomerBank, dtBankInfo, 0, 1, true);
            DataRow[] foundRows = dtBankInfo.Select("Bank_ID = '" + bankId + "'");
            txtAccountNo.Text = foundRows[0]["Account_No"].ToString();
            //  DrpCustomerBank.SelectedValue = foundRows[0]["Bank_ID"].ToString();
            txtAccountNo.Enabled = false;
            // DrpCustomerBank.Enabled = false;
            Session.Add("BankID", foundRows[0]["Bank_ID"]);
        }
        catch (Exception ex)
        {
        }
    }
    private void IncomeTax3()
    {

        string maxDocumentId = _ledgerCtl.SelectLedgerMaxDocumentId(Constants.Bank_Voucher, int.Parse(drpDistributor.SelectedValue));

        DataTable dtwhtax = _ledgerCtl.SelectWHTAX(long.Parse(DrpCustomer.SelectedValue));

        decimal whtax = decimal.Parse(dtwhtax.Rows[0][0].ToString());

        //to use in future
        //LedgerController CDC = new LedgerController();
        //DataTable dtCredit = CDC.SelectCreditPendingInvoice(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()), long.Parse(DrpCustomer.SelectedValue.ToString()), 0);

        //
        decimal offerAmount = decimal.Parse((whtax * (decimal.Parse(txtAmount.Text)) / 100).ToString());


        if (DrpChequeType.SelectedIndex == 0)
        {
            foreach (GridViewRow dr in GrdCredit.Rows)
            {
                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                if (chRelized.Checked == true)
                {
                    if (decimal.Parse(dr.Cells[3].Text) >= offerAmount)
                    {
                        _ledgerCtl.PostingCash_Bank_Account(Constants.Bank_Voucher, long.Parse(maxDocumentId), 107, int.Parse(drpDistributor.SelectedValue.ToString()), 0, offerAmount,
                        DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), "Income Tax", DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()),
                        txtChequeNo.Text, int.Parse(this.Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), dr.Cells[1].Text, Constants.Document_Invoice, txtSlipNo.Text, Constants.DateNullValue, 18, DrpDeliveryMan.SelectedValue.ToString());

                        _ledgerCtl.PostingCash_Bank_Account(Constants.Bank_Voucher, long.Parse(maxDocumentId), long.Parse(DrpBankAccount.SelectedValue.ToString()), int.Parse(drpDistributor.SelectedValue.ToString()), offerAmount, 0,
                        DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), "Income Tax", DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()),
                        txtChequeNo.Text, int.Parse(this.Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), dr.Cells[1].Text, Constants.Document_Invoice, txtSlipNo.Text, Constants.DateNullValue, 18, DrpDeliveryMan.SelectedValue.ToString());

                        offerAmount = decimal.Parse(dr.Cells[3].Text) - offerAmount;
                        _ledgerCtl.UpdateSaleInvoice(Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), int.Parse(drpDistributor.SelectedValue.ToString()), offerAmount);
                        break;
                    }
                    else if (decimal.Parse(dr.Cells[3].Text) <= offerAmount)
                    {
                        _ledgerCtl.PostingCash_Bank_Account(Constants.Bank_Voucher, long.Parse(maxDocumentId), 107, int.Parse(drpDistributor.SelectedValue.ToString()), 0, decimal.Parse(dr.Cells[3].Text),
                        DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), "Income Tax", DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()),
                        txtChequeNo.Text, int.Parse(this.Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), dr.Cells[1].Text, Constants.Document_Invoice, txtSlipNo.Text, Constants.DateNullValue, 18, DrpDeliveryMan.SelectedValue.ToString());

                        _ledgerCtl.PostingCash_Bank_Account(Constants.Bank_Voucher, long.Parse(maxDocumentId), long.Parse(DrpBankAccount.SelectedValue.ToString()), int.Parse(drpDistributor.SelectedValue.ToString()), decimal.Parse(dr.Cells[3].Text), 0,
                        DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), "Income Tax", DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()),
                        txtChequeNo.Text, int.Parse(this.Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), dr.Cells[1].Text, Constants.Document_Invoice, txtSlipNo.Text, Constants.DateNullValue, 18, DrpDeliveryMan.SelectedValue.ToString());

                        offerAmount = offerAmount - decimal.Parse(dr.Cells[3].Text);
                        _ledgerCtl.UpdateSaleInvoice(Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_INVOICE_ID"]), int.Parse(drpDistributor.SelectedValue.ToString()), 0);
                    }
                }
            }
            if (offerAmount > 0)// as advance
            {
                string maxDocumentId2 = _ledgerCtl.SelectLedgerMaxDocumentId(Constants.Bank_Voucher, int.Parse(drpDistributor.SelectedValue));

                _ledgerCtl.PostingCash_Bank_Account(Constants.Bank_Voucher, long.Parse(maxDocumentId2), 107, int.Parse(drpDistributor.SelectedValue.ToString()), 0, offerAmount,
                             DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), "Cheque Advance", DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()),
                             txtChequeNo.Text, int.Parse(this.Session["UserId"].ToString()), Constants.LongNullValue, null, Constants.IntNullValue, txtSlipNo.Text, Constants.DateNullValue, 20, "");

                _ledgerCtl.PostingCash_Bank_Account(Constants.Bank_Voucher, long.Parse(maxDocumentId2), long.Parse(DrpBankAccount.SelectedValue.ToString()), int.Parse(drpDistributor.SelectedValue.ToString()), offerAmount, 0,
                    DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), "Cheque Advance", DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()),
                    txtChequeNo.Text, int.Parse(this.Session["UserId"].ToString()), Constants.LongNullValue, null, Constants.IntNullValue, txtSlipNo.Text, Constants.DateNullValue, 20, "");


            }
        }
        else
        {
            _ledgerCtl.PostingCash_Bank_Account(Constants.Bank_Voucher, long.Parse(maxDocumentId), 107, int.Parse(drpDistributor.SelectedValue.ToString()), 0, decimal.Parse(txtAmount.Text),
                DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), "Cheque Advance", DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()),
                txtChequeNo.Text, int.Parse(this.Session["UserId"].ToString()), Constants.LongNullValue, null, Constants.IntNullValue, txtSlipNo.Text, Constants.DateNullValue, 20, "");

            _ledgerCtl.PostingCash_Bank_Account(Constants.Bank_Voucher, long.Parse(maxDocumentId), long.Parse(DrpBankAccount.SelectedValue.ToString()), int.Parse(drpDistributor.SelectedValue.ToString()), decimal.Parse(txtAmount.Text), 0,
                DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), "Cheque Advance", DateTime.Now, int.Parse(DrpCustomer.SelectedValue.ToString()), int.Parse(DrpPrincipal.SelectedValue.ToString()),
                txtChequeNo.Text, int.Parse(this.Session["UserId"].ToString()), Constants.LongNullValue, null, Constants.IntNullValue, txtSlipNo.Text, Constants.DateNullValue, 20, "");
        }

    }

    private void UncheckSelectAll()
    {
        ChbSelectAll.Checked = false;

    }

    private void ClearAll()
    {
        txtChequeNo.Text = "";
        txtAmount.Text = "";

        DrpCustomerBank.Items.Clear();
        DrpCustomerBank.Enabled = true;
        txtAccountNo.Enabled = true;
        txtAccountNo.Text = "";
        lblstatus.Visible = false;
        txtStartDate.Text = "";
        btnSave.Text = "Save";
        chbIsAdvance.Checked = false;
        chbIsAdvance.Visible = false;

        txtReceivedDate.Text = "";
        txtSlipNo.Text = "";
        txtRemarks.Text = "";
        lblStatusConvertTo.Visible = false;
        DrpStatusConvertTo.Visible = false;
        DrpStatus.Enabled = true;
        lblBankAccount.Visible = false;
        DrpBankAccount.Visible = false;

        lblsaleforce.Visible = true;
        DrpDeliveryMan.Visible = true;
        lblcustomer.Visible = true;
        DrpCustomer.Visible = true;
        lblroute.Visible = true;
        DrpRoute.Visible = true;
        GrdCredit.Visible = true;
        Panel1.Visible = true;


        this.Session.Remove("Bank_ID");

    }
}