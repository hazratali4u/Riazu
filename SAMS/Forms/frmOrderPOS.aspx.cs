using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web;
using System.IO;
using System.Net;
using System.Text;

public partial class Forms_frmOrderPOS : System.Web.UI.Page
{
    #region Variables
    static Forms_frmOrderPOS  temp = new Forms_frmOrderPOS();
    readonly DistributorController _mDist = new DistributorController();
    readonly RptSaleController _rptSaleCtl = new RptSaleController();
    readonly OrderEntryController _or = new OrderEntryController();
    readonly SKUGroupController _groupCtl = new SKUGroupController();
    readonly RptCustomerController rcc = new RptCustomerController();
    readonly General general = new General();
    string ExpiryDate;
    DataTable PurchaseSKU;
    DataControl _dc = new DataControl();
    public int onhold = 0;
    public long newCustomerId;
    long saleInvoiceID = 0;
    public long IvoiceIdDistW;
    public int printType;
    public static string CompanyName;
    public static string CompanyPhonNmbr;
    private static string _hirarchyNameQuiz;
    
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LoadProduct();
            txtQuantity.Text = "1";
            txtDiscount.Text = "0";
            txtskuCode.Focus();
            LoadSaleForce();
            txtstartDate.Text = (DateTime.Parse(Session["CurrentWorkDate"].ToString())).ToString("dd-MMM-yyyy");
            txtEndDate.Text = (DateTime.Parse(Session["CurrentWorkDate"].ToString())).ToString("dd-MMM-yyyy");
            lblCurrentWorkingDate.Text = (DateTime.Parse(Session["CurrentWorkDate"].ToString())).ToString("dd-MMM-yyyy");
            hfCurrentTime.Value = DateTime.Now.ToString("hh:mm:ss");
            LoadloginDetail();
            LoadCustomerData();
            CreditLimit();
            txtGrossAmount.Attributes.Add("readonly", "readonly");
            numtxtTotalExtraDiscnt.Attributes.Add("readonly", "readonly");
            numTxtTotalGST.Attributes.Add("readonly", "readonly");
            numTxtTotlAmnt.Attributes.Add("readonly", "readonly");
            txtBalance.Attributes.Add("readonly", "readonly");
            bool disAll = false; ////bool.Parse(Session["DISCOUNT_ALLOWD"].ToString());
            if (disAll == true)
            {
                txtDiscount.Enabled = true;
            }
            else { txtDiscount.Enabled = false; }
            bool PromotionAllowed = false;//// bool.Parse(Session["PROMOTION_ON"].ToString());
            if (PromotionAllowed == true)
            {
                txtDiscount.Enabled = false;
                DrpDiscount.Disabled = true;
            }
            else
            {
                txtDiscount.Enabled = true;
                DrpDiscount.Disabled = false;
            }
            if (ddlCustomer.Items.Count > 0)
            {
                ddlCustomer.SelectedValue = "1";
            }
        }
        int distributerId = int.Parse(HttpContext.Current.Session["DISTRIBUTOR_ID"].ToString());
        hfMaxId.InnerText = OrderEntryController.GetMaxInvoiceId(distributerId);
    }
    
    /// <summary>
    /// Enables/Disables Controls
    /// </summary>
    /// <param name="cValue">Value</param>
    private void EnableDisableController(bool cValue)
    {
        try
        {
            if (cValue)
            {
                if (txtGrossAmount.Text != "")
                {
                    if (decimal.Parse(txtGrossAmount.Text) > 0)
                    {
                        ddlCustomer.Enabled = true;
                       
                    }
                }
                else
                {
                    ddlCustomer.Enabled = false;
                  
                }
            }
        }
        catch (Exception eee)
        {
            eee.Message.ToString();
        }
    }
    private void DisableControls()
    {
        ((LinkButton)Master.FindControl("btnEditinvoice")).Enabled = false;
        ((LinkButton)Master.FindControl("btnHold")).Enabled = false;
        ((LinkButton)Master.FindControl("btnUnhold")).Enabled = false;
        ((LinkButton)Master.FindControl("btnpricelookup")).Enabled = false;

    }
    private void EnableControls()
    {
    }
    #region Clear

    /// <summary>
    /// Clears Some Of Controls
    /// </summary>
    private void ClearDetail()
    {
        try
        {
            txtskuCode.Text = "";
            txtskuName.Text = "";
            txtUnitRate.Text = "";
            hfToggleMode.Value = "SALE MODE";
            if (hfToggleMode.Value != "SALE MODE")
            {
                txtQuantity.Text = "-1";
            }
            else
            {
                txtQuantity.Text = "1";
            }
            txtDiscount.Text = "0";
            txtcolor.Text = "";
            txtsize.Text = "";

            btnSaveOrder.Enabled = true;

            txtskuCode.Enabled = true;
            txtskuCode.Focus();

        }
        catch (Exception eee)
        {
            eee.Message.ToString();
        }

    }

    private void ClearAll()
    {
        try
        {
            txtskuCode.Text = "";
            txtskuName.Text = "";
            txtUnitRate.Text = "";
            hfToggleMode.Value = "SALE MODE";
            if (hfToggleMode.Value != "SALE MODE")
            {
                txtQuantity.Text = "-1";
            }
            else
            {
                txtQuantity.Text = "1";
            }
            txtDiscount.Text = "0";
            txtcolor.Text = "";
            txtsize.Text = "";
            txtCashRecieved2.Text = "";
            btnSaveOrder.Enabled = true;

            txtskuCode.Enabled = true;

            txtskuCode.Focus();
            txtAuthorisedBy.Text = "";
            txtBalance.Text = "";




        }
        catch (Exception eee)
        {
            eee.Message.ToString();
        }

    }

    /// <summary>
    /// Clears All Controls
    /// </summary>
    private void ClearMasterAll()
    {
        try
        {
            Session.Remove("CustName");
            Session.Remove("CustCode");
            txtGrossAmount.Text = "";
            numtxtTotalExtraDiscnt.Text = "";
            txtBalance.Text = "";
            numTxtTotalGST.Text = "";
            numTxtTotlAmnt.Text = "";
            txtCashRecieved2.Text = "";
            txtNewCustomer.Text = "";
            txtNewCustomerCOntactNumer.Text = "";
            txtAuthorisedBy.Text = "";
            hfToggleMode.Value = "SALE MODE";
            if (hfToggleMode.Value != "SALE MODE")
            {
                txtQuantity.Text = "-1";
            }
            else
            {
                txtQuantity.Text = "1";
            }
            if (DrpPayMode.SelectedValue == "215" || DrpPayMode.SelectedValue == "218")
            {
                txtCashRecieved2.Text = "";
                txtBalance.Text = numTxtTotlAmnt.Text;
                txtCashRecieved2.Attributes.Add("readonly", "readonly");
            }
            else {
               txtCashRecieved2.ReadOnly = false;
            }

      
        }
        catch (Exception eee)
        {
            eee.Message.ToString();
        }
    }

    #endregion
    #region Click OPerations
    

    [WebMethod]
    [ScriptMethod]
    public static string   InsertInvoice(string orderedProducts, string amountDue, string author, string discount, string netAmount, string paidIn,
        string payType, string Gst, string manualId,string customerId, string saleForce,string NewCustomerNam,string NewCustomerContactNumber)
    {
        bool DistWiseId = false;
        try
        {
            DataControl _dc = new DataControl();

            var manualId2 = manualId ==  "SALE MODE" ? "2" : "1";

            DateTime currentWorkDate = DateTime.Parse(HttpContext.Current.Session["CurrentWorkDate"].ToString());
            int userId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
            int distributerId = int.Parse(HttpContext.Current.Session["DISTRIBUTOR_ID"].ToString());


            var SaleSKU = (DataTable)JsonConvert.DeserializeObject(orderedProducts, (typeof(DataTable)));

            
            if (SaleSKU != null && SaleSKU.Rows.Count > 0)
            {
                DistWiseId = OrderEntryController.Add_Invoice2(distributerId, manualId2, 0,0,0,long.Parse(customerId), long.Parse(customerId), Convert.ToInt32(saleForce),0, -1,
                         decimal.Parse(_dc.chkNull_0(amountDue)), decimal.Parse(_dc.chkNull_0(discount)),0, decimal.Parse(_dc.chkNull_0(Gst)), Decimal.Parse(_dc.chkNull_0(netAmount)), 0, int.Parse(payType),SaleSKU,null, userId, decimal.Parse(_dc.chkNull_0(paidIn)), currentWorkDate,0,0,
                          0, 0, 0,"","",0,false,Constants.IntNullValue,"","","",0,0,0);
                if (NewCustomerNam != null && NewCustomerNam != "" && !NewCustomerNam.Equals(""))
                {
                    SaveNewCustomer(distributerId, currentWorkDate, NewCustomerNam, NewCustomerContactNumber);
                    sendSmsToCUstomer(distributerId, NewCustomerNam, NewCustomerContactNumber, netAmount);

                }

            }

            return DistWiseId.ToString();
        }
        catch(Exception ex)
        {
            return null;
        }
     
    }
    
    private static  void SaveNewCustomer(int distributerId,DateTime currentWorkDate,string custname,string custnumber)
    {
        CustomerDataController mController = new CustomerDataController();
        DataControl dc = new DataControl();
       {
            SETTINGS_TABLE_Controller mSettingsTableControl = new SETTINGS_TABLE_Controller();
            DataTable dtSettingsTable = mSettingsTableControl.Select_SETTINGS_TABLE("CUSTOMER", "CUSTOMER_ID", distributerId);

            if (dtSettingsTable.Rows.Count > 0)
            {
                long CustomerId = long.Parse(dtSettingsTable.Rows[0]["Value"].ToString()) + 1;
                string StrCode = "";


                if (CustomerId.ToString().Length == 1)
                {
                    StrCode = "OT0000" + CustomerId.ToString();
                }
                else if (CustomerId.ToString().Length == 2)
                {
                    StrCode = "OT000" + CustomerId.ToString();
                }
                else if (CustomerId.ToString().Length == 3)
                {
                    StrCode = "OT00" + CustomerId.ToString();
                }
                else if (CustomerId.ToString().Length == 4)
                {
                    StrCode = "OT0" + CustomerId.ToString();
                }
                else if (CustomerId.ToString().Length == 5)
                {
                    StrCode = "OT" + CustomerId.ToString();
                }

                ////mController.InsertCustomer(CustomerId, false, true, Constants.IntNullValue,0,
                ////    Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue,
                ////    distributerId, "", "", custnumber , "", StrCode, custname,
                ////    "", currentWorkDate, 1, 1, "", "", 0);
                
            }
        }
        
        
    }
    private static void sendSmsToCUstomer(int distributerId, string custname, string custnumber, string value)
    {
        try
        {
            SaleForceController UController = new SaleForceController();
            ////DataTable dtArea = UController.SelectSalesForceCustomers(distributerId, Constants.IntNullValue, Constants.LongNullValue, 5);
            ////if (dtArea.Rows.Count > 0)
            ////{
            ////    string smsMSg = dtArea.Rows[0]["MESSAGE"].ToString();
            ////    string UserId = dtArea.Rows[0]["USERID"].ToString();
            ////    string pass = dtArea.Rows[0]["PASSWORD"].ToString();
            ////    string MsgType = dtArea.Rows[0]["MASK"].ToString();
            ////    string url = dtArea.Rows[0]["URL"].ToString();
            ////    string Msg = smsMSg + value;
            ////    string Contact_No = CheckNumber(custnumber);

            ////    SendSMS(Contact_No, Msg, MsgType, url, UserId, pass);
            ////}
        }
        catch (Exception eeee)
        {
            eeee.ToString();
        }
    }
    public static string CheckNumber(string CNO)
    {
        string Customer_No = "";
        string CONTACT_NO = CNO;
        if (CONTACT_NO.Length == 11) // 0300xxxxxxx
        {
            string str = CNO.Substring(0, 2);
            if (str.ToString() == "03")
            {
                string str1 = CNO.Substring(1, 10);
                Customer_No = "92" + str1;
            }
            else
            {
                Customer_No = "0";
            }
        }
        else if (CONTACT_NO.Length == 12) // 92300xxxxxxx
        {
            string str = CNO.Substring(0, 3);
            if (str.ToString() == "923")
            {
                Customer_No = CNO;
            }
            else
            {
                Customer_No = "0";
            }
        }
        else if (CONTACT_NO.Length == 13) // 920300xxxxxxx
        {
            string str = CNO.Substring(0, 3);
            if (str.ToString() == "920")
            {
                string str1 = CNO.Substring(0, 2);
                string str2 = CNO.Substring(3, 10);
                Customer_No = str1 + str2;
            }
            else
            {
                Customer_No = "0";
            }

        }
        else if (CONTACT_NO.Length == 14) // 0092300xxxxxxx
        {
            string str = CNO.Substring(0, 5);
            if (str.ToString() == "00923")
            {
                string str1 = CNO.Substring(2, 2);
                string str2 = CNO.Substring(4, 10);
                Customer_No = str1 + str2;
            }
            else
            {
                Customer_No = "0";
            }
        }
        else if (CONTACT_NO.Length == 15) // 00920300xxxxxxx
        {
            string str = CNO.Substring(0, 5);
            if (str.ToString() == "00920")
            {
                string str1 = CNO.Substring(2, 2);
                string str2 = CNO.Substring(5, 10);
                Customer_No = str1 + str2;
            }
            else
            {
                Customer_No = "0";
            }
        }
        return Customer_No;
    }
    public static string SendSMS(string customerNo, string msg, string msgType, string Url,string userId,string password)
    {
        String result = "";
        String strPost = "id="+ userId + "&pass="+password+"&msg=" + msg + "&to=" + customerNo + "" + "&mask=" + msgType + "&type=xml&lang=English";
        StreamWriter myWriter = null;
        HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(Url);

        objRequest.Method = "POST";
        objRequest.ContentLength = Encoding.UTF8.GetByteCount(strPost);
        objRequest.ContentType = "application/x-www-form-urlencoded";
        try
        {
            myWriter = new StreamWriter(objRequest.GetRequestStream());
            myWriter.Write(strPost);
        }
        catch (Exception e)
        {
            return e.Message;
        }
        finally
        {
            myWriter.Close();
        }
        HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
        using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
        {
            result = sr.ReadToEnd();   // Close and clean up the StreamReader   
            sr.Close();
        }
        return result;
    }
   
    // Update Limit After Insertion
    protected void btnUpdateLimit_Click(object sender, EventArgs e)//cancel button click 
    {
            CreditLimit();        
    }

    protected void btnNewCustomer_Click(object sender, EventArgs e)
    {
        
        Server.Transfer("frmNewCustomerPOS.aspx");
    }
    protected void btnVoid_Click(object sender, EventArgs e)//cancel button click 
    {
        ClearAll();
        ClearMasterAll();
        btnToggleMode.Disabled = false;
        txtskuCode.Focus();
    }
    private void updateInvoiceNumber()
    {
    }
   
    /// <summary>
    /// OPerate on Exit Button ...Clear Sessions
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        try
        {
            if (this.Session["PurchaseSKU"] != null)
            {
                Session.Remove("PurchaseSKU");
            }

            if (this.Session["dtFreeSKU"] != null)
            {
                Session.Remove("dtFreeSKU");
            }
            if (this.Session["CustName"] != null)
            {
                Session.Remove("CustName");
            }
            if (this.Session["CustCode"] != null)
            {
                Session.Remove("CustCode");
            }
           
        }
        catch (Exception ex)
        {
            ExceptionPublisher.PublishException(ex);
        }
        finally
        {
            Response.Redirect("Home.aspx?");
        }
    }

    /// <summary>
    /// Saves/Updates Order, Invoice And Sale Return
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnSaveOrder_Click(object sender, EventArgs e)
    {

        ////try
        ////{
        string manualId = null;

        if (hfToggleMode.Value == "SALE MODE")
        {
            manualId = "2";//for order invoice
        }
        else
        {
            manualId = "1";//for sale return
        }

        ////        DataTable PurchaseSKU = (DataTable)JsonConvert.DeserializeObject(tab.Value, (typeof(DataTable)));

        ////        OrderEntryController mOrderController = new OrderEntryController();

        ////        if (PurchaseSKU.Rows.Count > 0)
        ////        {                    
        ////            if (btnSaveOrder.ToolTip == "Save")
        ////            {
        ////                saleInvoiceID = 0;
        ////                if (saleInvoiceID == -2 || saleInvoiceID == -1)
        ////                {
        ////                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Insertion Failed!!!.');", true);
        ////                }
        ////                else
        ////                {

        ////                    ClearMasterAll();

        ////                    btnToggleMode.Disabled = false;

        ////                    ScriptManager.GetCurrent(Page).SetFocus(txtskuCode);

        ////                    CORNBusinessLayer.Reports.CrpPrintInvoice crpReport = new CORNBusinessLayer.Reports.CrpPrintInvoice();

        ////                    DataSet ds = null;

        ////                    ds = rcc.PrintInvoice(int.Parse(Session["DISTRIBUTOR_ID"].ToString()), int.Parse(Session["PRINCIPAL_ID"].ToString()), 2, int.Parse(saleInvoiceID.ToString()), Constants.DateNullValue, Constants.DateNullValue);

        ////                DataTable dtNotes = rcc.GetNotes(int.Parse(Session["DISTRIBUTOR_ID"].ToString()));
        ////                string notes = "";
        ////                if (dtNotes.Rows.Count > 0)
        ////                {
        ////                    for (int i = 0; i < dtNotes.Rows.Count; i++)
        ////                    {
        ////                        notes = notes  +". "+ dtNotes.Rows[i]["SLIP_NOTE"].ToString()+". \r";
        ////                    }
        ////                }
        ////                    crpReport.SetDataSource(ds);
        ////                    crpReport.Refresh();
        ////                    if (string.IsNullOrEmpty(_hirarchyNameQuiz))
        ////                    {
        ////                        _hirarchyNameQuiz = "Need";
        ////                    }
        ////                    crpReport.SetParameterValue("COMPANY_NAME", _hirarchyNameQuiz);
        ////                    crpReport.SetParameterValue("INVOICENO", Convert.ToString(saleInvoiceID));
        ////                    crpReport.SetParameterValue("LOCATION", lblLoacation.Text);
        ////                    crpReport.SetParameterValue("PHONE_NUMBER", CompanyPhonNmbr);
        ////                    crpReport.SetParameterValue("CASHIER", lbluserlogin.Text);
        ////                crpReport.SetParameterValue("notes",notes);
        ////                    Session.Add("CrpReport", crpReport);
        ////                    Session.Add("ReportType", 0);


        ////                    const string url = "'Default.aspx'";
        ////                    const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=500,height=550,left=20,top=20\");</script>";
        ////                    Type cstype = GetType();
        ////                    var cs = Page.ClientScript;
        ////                    cs.RegisterStartupScript(cstype, "OpenWindow", script);
        ////                }
        ////            }

        ////        }
        ////        else
        ////        {
        ////            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('No product added');", true);
        ////        }
        ////    ScriptManager.GetCurrent(Page).SetFocus(txtskuCode);
        ////}
        ////catch (Exception eee)
        ////{
        ////    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Some error occurred');", true);
        ////}
    }
    protected void btnHold_Click(object sender, ImageClickEventArgs e)
    {
    }
  
    protected void btnRePrint_Click(object sender, EventArgs e)
    {
    }

    #endregion
    protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
        CreditLimit();
        txtskuCode.Text = "";
        txtskuName.Text = "";
        txtUnitRate.Text = "";
        txtDiscount.Text = "0";
        txtsize.Text = "";
        txtGrossAmount.Text = "";
        numTxtTotlAmnt.Text = "";
        numTxtTotalGST.Text = "";
        numtxtTotalExtraDiscnt.Text = "";
        txtBalance.Text = "";
        txtCashRecieved2.Text = "";
        txtskuCode.Focus();
        try
        {
            DataTable dtCustomer = (DataTable)HttpContext.Current.Session["dtCustomer"];
            DataRow[] foundRows = dtCustomer.Select("CUSTOMER_ID  = '" + ddlCustomer.SelectedValue.ToString() + "'");
            if (foundRows.Length > 0)
            {
                CustomerNamePrint.Value = (foundRows[0]["CUSTOMER_NAME"].ToString());
                CustomerAddressPrint.Value = (foundRows[0]["ADDRESS"].ToString());
                CustomerPhonPrint.Value = (foundRows[0]["CONTACT_NUMBER"].ToString());
            }
        }
        catch (Exception eee)
        { }
    }    
    private void CreditLimit()
    {
        CustomerDataController cdCtrl = new CustomerDataController();
        lblCreditLimit.Text = "0";
        lblLedgerBalance.Text = "0";
        lblAllowLimit.Text = "0";
        if (ddlCustomer.Items.Count > 0)
        {
            // --working on both options credit, credit and cash  
            DataTable dt = cdCtrl.SelectCustomerCreditBalance(long.Parse(ddlCustomer.SelectedValue), 1, Convert.ToInt32(Session["DISTRIBUTOR_ID"].ToString()), Constants.Credit_Order_Id);
            if (dt == null) return;
            if (dt.Rows.Count < 0) return;
            {
                //This Limit is AllowLimit + Ledger Balance
                lblAllowLimit.Text = Convert.ToString(decimal.Parse(_dc.chkNull_0(dt.Rows[0][0].ToString())));

                //This Limit is entered by user while adding customer
                lblCreditLimit.Text = Convert.ToString(decimal.Parse(_dc.chkNull_0(dt.Rows[0][1].ToString())));
                lblLedgerBalance.Text = Convert.ToString(decimal.Parse(_dc.chkNull_0(dt.Rows[0][2].ToString())));
            }
        }
    }
    #region Load Functions

    /// <summary>
    /// Checks SKU in Order Grid
    /// </summary>
    /// <returns>True On Success And False On Failure</returns>
    private bool CheckDublicateSku()
    {
        bool flag = true;

        PurchaseSKU = (DataTable)Session["PurchaseSKU"];

        DataRow[] foundRows2 = PurchaseSKU.Select("SKU_CODE = '" + txtskuCode.Text + "'");

        if (foundRows2.Length != 0)
        {
            foreach (GridViewRow dr2 in GrdPurchase.Rows)
            {
                string code = dr2.Cells[1].Text;
                int chkdel = Convert.ToInt32(dr2.Cells[10].Text);

                if ((txtskuCode.Text == code) && (chkdel == 1))
                {
                    int index = dr2.RowIndex;
                    PurchaseSKU.Rows.RemoveAt(index);
                    flag = true;
                    break;
                }
                else if ((txtskuCode.Text == code) && (chkdel == 0))
                {
                    flag = false;
                    break;
                }
            }
        }
        return flag;
    }
    private static bool FindCustomer(string customerCode)
    {

        DataTable dtCustomer = (DataTable)HttpContext.Current.Session["dtCustomer"];
        DataRow[] foundRows = dtCustomer.Select("CUSTOMER_CODE  = '" + customerCode.Trim() + "'");
        if (foundRows.Length > 0)
        {
            HttpContext.Current.Session.Add("CUSTOMER_ID", long.Parse(foundRows[0]["CUSTOMER_ID"].ToString()));
            
          
            return true;
        }
        return false;
    }
    /// <summary>
    /// Verifies Customer Code
    /// </summary>
    /// <returns>True On Success And False On Failure</returns>
    /// 
    public string GetJson(DataTable dt)
    {
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        serializer.MaxJsonLength = Int32.MaxValue;
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;

        foreach (DataRow dr in dt.Rows)
        {
            row = dt.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => dr[col]);
            rows.Add(row);
        }
        return serializer.Serialize(rows);
    }

    /// <summary>
    /// Loads Order To Order Grid
    /// </summary>
    private void LoadGird()
    {
        PurchaseSKU = (DataTable)Session["PurchaseSKU"];
        GrdPurchase.DataSource = PurchaseSKU;
        GrdPurchase.DataBind();
    }

   

    private void LoadDocumentDetail(long docId, int typeId)
    {
        try
        {
            OrderEntryController ord = new OrderEntryController();
            ////PurchaseSKU = ord.GetDocumentDetail(docId, typeId);
            Session.Add("PurchaseSKU", PurchaseSKU);
            LoadGird();
        }
        catch (Exception eee)
        {
            eee.Message.ToString();
        }
    }
    private void LoadloginDetail()
    {
        try
        {
            DataTable dtNotes = _mDist.GetNotes(int.Parse(Session["DISTRIBUTOR_ID"].ToString()));
            string notes = "";
            if (dtNotes.Rows.Count > 0)
            {
                for (int i = 0; i < dtNotes.Rows.Count; i++)
                {
                    notes = notes + ". " + dtNotes.Rows[i]["SLIP_NOTE"].ToString() + ". <br />";
                }
            }

            UserController userControl = new UserController();
            DataTable dt = userControl.SelectSlashUser2(int.Parse(Session["UserId"].ToString()));
            DateTime closingdate = Convert.ToDateTime(Session["CurrentWorkDate"].ToString());
            lbllogintimedate.Text = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
            lbluserlogin.Text = dt.Rows[0]["USER_NAME"].ToString();
            lblLoacation.Text = dt.Rows[0]["DISTRIBUTOR_NAME"].ToString();
            hfLocationName.Value = lblLoacation.Text;
            hfLocationPic.Value = "../Pics/" + (Session["DISTRIBUTOR_ID"].ToString()) + dt.Rows[0]["IMAGE_PATH"].ToString();
            CompanyName = dt.Rows[0]["COMPANY_NAME"].ToString();
            hfCompanyName.Value = CompanyName;
            CompanyPhonNmbr = dt.Rows[0]["CONTACT_NUMBER"].ToString();//Location Contact Number
            hfContactNo.Value = "PH: " + CompanyPhonNmbr;
            ltrnotes.Text = notes;
            hfPosReportType.Value = (dt.Rows[0]["pos_report"].ToString());
            AutoComplete.ContextKey = dt.Rows[0]["DISTRIBUTOR_ID"].ToString();
            hfAddess.Value = dt.Rows[0]["address1"].ToString();
            hfaddress2.Value = dt.Rows[0]["address2"].ToString();

            DataTable dt2 = userControl.SelectUserPrincipal(int.Parse(Session["UserId"].ToString()));
            if (dt2 != null)
            {
                _hirarchyNameQuiz = dt2.Rows[0]["SKU_HIE_NAME"].ToString();
            }
        }
        catch (Exception eee)
        {
            eee.Message.ToString();
        }
    }

    /// <summary>
    /// Loads Customers To Customer ListBox
    /// </summary>
    private void LoadCustomerData()
    {
        CustomerDataController mController = new CustomerDataController();
        DataTable dtCustomer = mController.SelectPrincipalCustomer(int.Parse(Session["DISTRIBUTOR_ID"].ToString()), Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue);
        clsWebFormUtil.FillDropDownList(ddlCustomer, dtCustomer, "CUSTOMER_ID", "CUSTOMER_DETAIL", true);        
        Session.Add("dtCustomer", dtCustomer);
    }

    /// <summary>
    /// Enables/Disables Discount Fields For Manual And Auto Promotion And Loads Promotion Controler For Auto Promotion
    /// </summary>
  
    private void LoadSaleForce()
    {
        Distributor_UserController UController = new Distributor_UserController();
        
        DataTable dt = UController.SelectDistributorUser(35, int.Parse(Session["DISTRIBUTOR_ID"].ToString()), int.Parse(Session["CompanyId"].ToString()));//37 is the ref id  for sale force 
        clsWebFormUtil.FillDropDownList(ddsalesForce, dt, "USER_ID", "USER_NAME");
        ddsalesForce.SelectedValue = Convert.ToString(Session["UserId"]);
        ddl_saleforce2.Items.Add(new ListItem("All", Constants.IntNullValue.ToString()));
        clsWebFormUtil.FillDropDownList(ddl_saleforce2, dt, "USER_ID", "USER_NAME", false);

    }
    public void LoadOnHoldInvoicNumber()
    {
        OrderEntryController or = new OrderEntryController();
        DataTable dtOrder = or.SelectPendingOrder(int.Parse(Session["DISTRIBUTOR_ID"].ToString()), 0, 0, 0, 0, Constants.Order_Pending_Id, Constants.IntNullValue, int.Parse(Session["UserId"].ToString()), Convert.ToDateTime(Session["CurrentWorkDate"]));
        ((TextBox)(Master.FindControl("lblonHold"))).Text = Convert.ToString(dtOrder.Rows.Count);

    }
    private void LoadProduct()
    {
        try
        {
            SKUPriceDetailController PController = new SKUPriceDetailController();
            DataTable dtProduct = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(Session["DISTRIBUTOR_ID"].ToString()), int.Parse(Session["UserId"].ToString()), Constants.IntNullValue, 1, DateTime.Parse(Session["CurrentWorkDate"].ToString()));
            if (dtProduct != null && dtProduct.Rows.Count > 0)
            {
                hfProduct.Value = GetJson(dtProduct);
                Session.Add("Dtsku_Price", dtProduct);
            }           
        }
        catch (Exception eee)
        {
            eee.Message.ToString();
        }
    }
    #endregion
    #region Sale Report

    protected void btnViewSalesReport_Click(object sender, EventArgs e)
    {

        ClearAll();
        ClearMasterAll();

        ////if (ddlReportType.SelectedValue == "2")
        ////{
        ////    try
        ////    {

        ////        CORNBusinessLayer.Reports.crpSalesReportPos CrpReport = new CORNBusinessLayer.Reports.crpSalesReportPos();

        ////        DataSet ds = null;
        ////        CORNBusinessLayer.Classes.DocumentPrintController DPrint = new CORNBusinessLayer.Classes.DocumentPrintController();
        ////        DataTable dt = DPrint.SelectReportTitle(int.Parse(Session["DISTRIBUTOR_ID"].ToString()));
        ////        ds = _rptSaleCtl.SelectSaleReport(int.Parse(Session["DISTRIBUTOR_ID"].ToString()), int.Parse(ddl_saleforce2.SelectedValue), DateTime.Parse(txtstartDate.Text), DateTime.Parse(txtEndDate.Text), Constants.LongNullValue);
               
               
        ////        CrpReport.SetDataSource(ds);
        ////        CrpReport.Refresh();
        ////        if (string.IsNullOrEmpty(_hirarchyNameQuiz))
        ////        {
        ////            _hirarchyNameQuiz = "Need";
        ////        }
        ////        CrpReport.SetParameterValue("COMPANY_NAME", dt.Rows[0]["COMPANY_NAME"].ToString());
        ////        CrpReport.SetParameterValue("FROM_DATE", DateTime.Parse(txtstartDate.Text));
        ////        CrpReport.SetParameterValue("TO_DATE", DateTime.Parse(txtEndDate.Text));
        ////        CrpReport.SetParameterValue("USER_NAME", Convert.ToString(ddl_saleforce2.SelectedItem.Text));
        ////        CrpReport.SetParameterValue("LOCATION", lblLoacation.Text);
        ////        CrpReport.SetParameterValue("PHONE_NUMBER", CompanyPhonNmbr);

        ////        Session.Add("CrpReport", CrpReport);
        ////        Session.Add("ReportType", 0);
        ////        const string url = "'Default.aspx'";
        ////        const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=450,height=450,left=40,top=40\");</script>";
        ////        Type cstype = GetType();
        ////        var cs = Page.ClientScript;
        ////        cs.RegisterStartupScript(cstype, "OpenWindow", script);
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        ex.ToString();
        ////    }
        ////}
        ////else
        ////{
        ////    try
        ////    {


        ////        CORNBusinessLayer.Reports.CRPSalesReportSummary CrpReport = new CORNBusinessLayer.Reports.CRPSalesReportSummary();
        ////        CORNBusinessLayer.Classes.DocumentPrintController DPrint = new CORNBusinessLayer.Classes.DocumentPrintController();
        ////        DataTable dt = DPrint.SelectReportTitle(int.Parse(Session["DISTRIBUTOR_ID"].ToString()));

        ////        DataSet ds = null;

        ////        ds = _rptSaleCtl.SelectSaleReport(int.Parse(Session["DISTRIBUTOR_ID"].ToString()), int.Parse(ddl_saleforce2.SelectedValue), DateTime.Parse(txtstartDate.Text), DateTime.Parse(txtEndDate.Text), -1);

        ////        CrpReport.SetDataSource(ds);
        ////        CrpReport.Refresh();
        ////        if (string.IsNullOrEmpty(_hirarchyNameQuiz))
        ////        {
        ////            _hirarchyNameQuiz = "Need";
        ////        }
        ////        CrpReport.SetParameterValue("COMPANY_NAME", dt.Rows[0]["COMPANY_NAME"].ToString());
        ////        CrpReport.SetParameterValue("FROM_DATE", DateTime.Parse(txtstartDate.Text));
        ////        CrpReport.SetParameterValue("TO_DATE", DateTime.Parse(txtEndDate.Text));
        ////        CrpReport.SetParameterValue("USER_NAME", Convert.ToString(ddl_saleforce2.SelectedItem.Text));
        ////        CrpReport.SetParameterValue("LOCATION", lblLoacation.Text);
        ////        CrpReport.SetParameterValue("PHONE_NUMBER", CompanyPhonNmbr);

        ////        Session.Add("CrpReport", CrpReport);
        ////        Session.Add("ReportType", 0);
        ////        const string url = "'Default.aspx'";
        ////        const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=450,height=450,left=40,top=40\");</script>";
        ////        Type cstype = GetType();
        ////        var cs = Page.ClientScript;
        ////        cs.RegisterStartupScript(cstype, "OpenWindow", script);
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        ex.ToString();
        ////    }

        ////}
    }
   
    #endregion
}