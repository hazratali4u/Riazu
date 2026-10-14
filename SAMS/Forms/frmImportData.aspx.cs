using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Form To Import Route, Market,  Customer, SKU And SKU Price Data From Text Files
/// </summary>
public partial class Forms_frmImportData : System.Web.UI.Page
{
    CustomerDataController mCustomer = new CustomerDataController();
    LedgerController LedgerCtl = new LedgerController();
    Distributor_UserController UController = new Distributor_UserController();
    GeoHierarchyController GControler = new GeoHierarchyController();
    DistributorController mController = new DistributorController();
    DistributorAreaController DistAreaCntl = new DistributorAreaController();
    DistributorRouteController DistRouteCtl = new DistributorRouteController();
    SKUPriceDetailController SKUPriceCtl = new SKUPriceDetailController();    

    /// <summary>
    /// Page_Load Function Populates All Combos On The Page
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
            this.LoadTown();
            this.LoadDistributor();
            this.LoadPrincipal();
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
    /// Loads Locations To Location Combo
    /// </summary>
    private void LoadDistributor()
    {
        DataTable dt = mController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDropDownList(DrpDistributor, dt, 0, 2, true);
    }

    /// <summary>
    /// Loads Towns To Town Combo
    /// </summary>
    private void LoadTown()
    {
        DataTable dt = GControler.SelectGeoHierarchy(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, null, true, Constants.IntNullValue, Constants.Town, Constants.IntNullValue, Constants.DateNullValue, Constants.DateNullValue, int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDropDownList(DrpTown, dt, 0, 4, true);
    }    
        
    /// <summary>
    /// Loads Principals To Principal Comb
    /// </summary>
    private void LoadPrincipal()
    {
        DataTable m_dt = SKUPriceCtl.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        clsWebFormUtil.FillDropDownList(this.DrpPrincipal, m_dt, 0, 1, true);
    }
        
    /// <summary>
    /// Imports File Data To Database
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        #region FileValidation
        if (txtFile.PostedFile.ContentLength == 0)
        {
            lblErrorMessage.Text = "Please select a file and then upload";
            return;
        }
        else if (txtFile.PostedFile.ContentType != "text/plain")
        {
            lblErrorMessage.Text = "Only text file are supported";
            return;
        }
        #endregion

        try
        {
            lblErrorMessage.ForeColor = System.Drawing.Color.Red;
            lblErrorMessage.Text = string.Empty;
            if (!Directory.Exists(Constants.fldOtherDataFolder))
            {
                Directory.CreateDirectory(Constants.fldOtherDataFolder);
            }

            string path = System.IO.Path.GetFullPath(txtFile.PostedFile.FileName);
            string filename = path.Substring(path.LastIndexOf('\\'), path.Length - path.LastIndexOf('\\'));
            if (File.Exists(Constants.fldOtherDataFolder + filename))
            {
                lblErrorMessage.Text = "File already Exist in folder. Save file with other name";
                cboFileTypes.SelectedIndex = 0;
                return;
            }
            else
            {
                txtFile.PostedFile.SaveAs(Constants.fldOtherDataFolder + filename);
                path = Constants.fldOtherDataFolder + filename;
                int index = cboFileTypes.SelectedIndex;
                this.lblErrorMessage.Text = "";
                if (cboFileTypes.SelectedIndex == 0) // Route
                {
                    DistAreaCntl.ImportRoute(int.Parse(DrpTown.SelectedValue.ToString()), int.Parse(DrpDistributor.SelectedValue.ToString()), path, int.Parse(this.Session["UserId"].ToString()));
                }
                else if (cboFileTypes.SelectedIndex == 1) // Market
                {
                    DistRouteCtl.ImportMarket(int.Parse(DrpTown.SelectedValue.ToString()), int.Parse(DrpDistributor.SelectedValue.ToString()), path, int.Parse(this.Session["UserId"].ToString()));
                }
                else if (cboFileTypes.SelectedIndex == 2) // Customer
                {
                    mCustomer.ImportCustomer(int.Parse(DrpTown.SelectedValue.ToString()), int.Parse(DrpDistributor.SelectedValue.ToString()), path);
                }
                else if (cboFileTypes.SelectedIndex == 3) // SKUS
                {
                    SKUPriceCtl.ImportSKU(int.Parse(DrpDistributor.SelectedValue.ToString()), path, int.Parse(DrpPrincipal.SelectedValue.ToString()), int.Parse(this.Session["UserID"].ToString()));
                }
                else if (cboFileTypes.SelectedIndex == 4) // SKU Price
                {
                    SKUPriceCtl.ImportSKUPrices(int.Parse(DrpDistributor.SelectedValue.ToString()), path, int.Parse(DrpPrincipal.SelectedValue.ToString()));
                }
                else if (cboFileTypes.SelectedIndex == 5) // Opening Stock
                {
                    SKUPriceCtl.ImportSKUOpeningStock(int.Parse(DrpDistributor.SelectedValue.ToString()), path, int.Parse(DrpPrincipal.SelectedValue.ToString()));
                }
                else if (cboFileTypes.SelectedIndex == 6) // Opening Credit
                {
                    DataTable dtEmployee = UController.SelectDistributorUser(Constants.IntNullValue, int.Parse(DrpDistributor.SelectedValue.ToString()), int.Parse(this.Session["CompanyId"].ToString()));
                    DataTable dtCustomer = mCustomer.SelectPrincipalCustomer(int.Parse(DrpDistributor.SelectedValue.ToString()), Constants.IntNullValue, Constants.IntNullValue, int.Parse(DrpPrincipal.SelectedValue.ToString()), 1);

                    if(LedgerCtl.ImportOpeningCredit(int.Parse(DrpDistributor.SelectedValue.ToString()),Convert.ToInt32(DrpPrincipal.SelectedValue),Convert.ToInt32(Session["UserID"]),Constants.CashSales, dtCustomer,dtEmployee, path))
                    {
                        lblErrorMessage.ForeColor = System.Drawing.Color.Green;
                        lblErrorMessage.Text = "Opening Credit Imported Successfully!";
                    }
                }
            }
        }
        catch (Exception excp)
        {
            lblErrorMessage.Text = excp.ToString();
            cboFileTypes.SelectedIndex = 0;
            return;
        }
    }
    
    private void DownLoadFile(string path)
    {
        FileInfo fi = new FileInfo(path);
        if (fi.Exists)
        {
            long sz = fi.Length;
            Response.ClearContent();
            Response.ContentType = MimeType(Path.GetExtension(path));
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename = {0}", System.IO.Path.GetFileName(path)));
            Response.AddHeader("Content-Length", sz.ToString("F0"));
            Response.TransmitFile(path);
            Response.End();
        }
    }
    public static string MimeType(string Extension)
    {
        string mime = "application/octetstream";
        if (string.IsNullOrEmpty(Extension))
            return mime;
        string ext = Extension.ToLower();
        Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
        if (rk != null && rk.GetValue("Content Type") != null)
            mime = rk.GetValue("Content Type").ToString();
        return mime;
    }

    protected void lnkFormat_Click(object sender, EventArgs e)
    {
        lblErrorMessage.ForeColor = System.Drawing.Color.Red;
        lblErrorMessage.Text = string.Empty;
        if (cboFileTypes.SelectedValue == "6")
        {
            DownLoadFile(Server.MapPath("~/Docs/ImportOpeningCreditFormat.xlsx"));
        }
        else
        {
            lblErrorMessage.Text = "Format not found";
        }
    }
}