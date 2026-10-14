using System;
using System.Data;
using System.Web.UI;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System.Linq;

/// <summary>
/// From to Assign Towns To Locations.
/// </summary>
public partial class Forms_frmAcntHeadAssinmntToForms : System.Web.UI.Page
{
    /// <summary>
    /// Page_Load Function Populates All Combos and ListBox On The Page
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    AccountHeadController MController = new AccountHeadController();
    protected void Page_Load(object sender, EventArgs e)
    {
      
        if (!Page.IsPostBack)
        {
            this.LoadDistributor();
           
          
            this.GetAccountType();
           
            this.GetSubTypeForHead();
           
            this.GetDetailAccountTypeForHead();
            LoadAssignAccunt();
            LoadUnAssignAccunt();

        }
    }
    
    /// <summary>
    /// Loads Locations To Location Combo
    /// </summary>
    private void LoadDistributor()
    {
        DistributorController mController = new DistributorController();
        DataTable dt = mController.SelectDistributor(Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDropDownList(drpDistributor, dt, 1, 3, true);

    }

    /// <summary>
    /// Loads UnAssigned Towns To UnAssigned ListBox
    /// </summary>
    private void LoadUnAssignAccunt()
    {
        if (drpFormName.Items.Count > 0 && drpDistributor.Items.Count > 0)
        {
            DistributorTownController gController = new DistributorTownController();
            DataTable dt = MController.SelectAccountHeadAssignment(Constants.AC_AccountHeadId, int.Parse(drpAccountTypeDetail.SelectedValue.ToString()), DrpAccountCategory.SelectedIndex,int.Parse(drpFormName.SelectedValue), int.Parse(drpDistributor.SelectedValue.ToString()), 0,1);
            clsWebFormUtil.FillListBox(lstUnAssignTown, dt, 0, 1, true);
        }
        else
        {
            lstUnAssignTown.Items.Clear();
        }

    }
    
    /// <summary>
    /// Loads Assigned Towns To Assigned ListBox
    /// </summary>
    private void LoadAssignAccunt()
    {

        if (drpFormName.Items.Count > 0 && drpDistributor.Items.Count > 0)
        {
            DistributorTownController gController = new DistributorTownController();
            DataTable dt = MController.SelectAccountHeadAssignment(Constants.AC_AccountHeadId, int.Parse(drpAccountTypeDetail.SelectedValue.ToString()), DrpAccountCategory.SelectedIndex, int.Parse(drpFormName.SelectedValue), int.Parse(drpDistributor.SelectedValue.ToString()), 0, 2);
            clsWebFormUtil.FillListBox(lstAssignTown, dt, 0, 1, true);
        }
        else
        {
            lstAssignTown.Items.Clear();
        }
    }

    /// <summary>
    /// Loads Assigned Towns To Assigned ListBox And UnAssigned Towns To UnAssigned ListBox
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadAssignAccunt();
        LoadUnAssignAccunt();
    }

   
    /// <summary>
    /// Inserts Towns Assignment To Location
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnAssign_Click(object sender, EventArgs e)
    {
        DistributorTownController mTownController = new DistributorTownController();
        for (int i = 0; i < lstUnAssignTown.Items.Count; i++)
        {
            if (lstUnAssignTown.Items[i].Selected == true && drpDistributor.Items.Count > 0)
            {
                mTownController.InsertModuleAccountHeadassind(int.Parse(drpDistributor.SelectedValue.ToString()),int.Parse(drpFormName.SelectedValue), int.Parse(lstUnAssignTown.Items[i].Value)
                    , 1);
            }
        }
        LoadAssignAccunt();
        LoadUnAssignAccunt();
    }
    
    /// <summary>
    /// Inserts All Towns Assignment To Location
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnAssignAll_Click(object sender, EventArgs e)
    {
        DistributorTownController mTownController = new DistributorTownController();
        if (drpDistributor.Items.Count > 0)
        {
            for (int i = 0; i < lstUnAssignTown.Items.Count; i++)
            {

                mTownController.InsertModuleAccountHeadassind(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(drpFormName.SelectedValue), int.Parse(lstUnAssignTown.Items[i].Value)
                   , 1);

            }
        }
        LoadAssignAccunt();
        LoadUnAssignAccunt();
    }
    
    /// <summary>
    /// Deletes Towns Assignment To Location
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnUnAssign_Click(object sender, EventArgs e)
    {
        DistributorTownController mTownController = new DistributorTownController();
        if (drpDistributor.Items.Count > 0)
        {
            for (int i = 0; i < lstAssignTown.Items.Count; i++)
            {

                mTownController.InsertModuleAccountHeadassind(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(drpFormName.SelectedValue), int.Parse(lstAssignTown.Items[i].Value)
                   , 2);

            }
        }
        LoadAssignAccunt();
        LoadUnAssignAccunt();
    }
    
    /// <summary>
    /// Deletes All Towns Assignment To Location
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnUnAssignAll_Click(object sender, EventArgs e)
    {
        DistributorTownController mTownController = new DistributorTownController();
        for (int i = 0; i < lstAssignTown.Items.Count; i++)
        {
            if (lstAssignTown.Items[i].Selected == true && drpDistributor.Items.Count > 0)
            {
                mTownController.InsertModuleAccountHeadassind(int.Parse(drpDistributor.SelectedValue.ToString()), int.Parse(drpFormName.SelectedValue), int.Parse(lstAssignTown.Items[i].Value)
                   , 2);
            }
        }
        LoadAssignAccunt();
        LoadUnAssignAccunt();
    }

    private void GetAccountType()
    {
        DataTable dt = MController.SelectAccountHead(Constants.AC_MainTypeId, Constants.LongNullValue, DrpAccountCategory.SelectedIndex);
        
        clsWebFormUtil.FillDropDownList(ddAccountType3, dt, 0, 10, true);
    }
    protected void ddAccountType3_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.GetSubTypeForHead();
        this.GetDetailAccountTypeForHead();
        LoadAssignAccunt();
        LoadUnAssignAccunt();
    }
    private void GetSubTypeForHead()
    {
        if (ddAccountType3.Items.Count > 0)
        {
            DataTable dt = MController.SelectAccountHead(Constants.AC_SubTypeId, int.Parse(ddAccountType3.SelectedValue.ToString()), DrpAccountCategory.SelectedIndex);
            clsWebFormUtil.FillDropDownList(ddAccountSubType2, dt, 0, 10, true);
        }
    }

    /// <summary>
    /// Loads Account DetailTypes To DetailType Combo And AccountHeads To AccountHead Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void ddAccountSubType2_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.GetDetailAccountTypeForHead();
        LoadAssignAccunt();
        LoadUnAssignAccunt();
    }

    /// <summary>
    /// Loads Account DetailTypes To DetailType Combo
    /// </summary>
    private void GetDetailAccountTypeForHead()
    {
        if (ddAccountSubType2.Items.Count > 0)
        {
            DataTable dt = MController.SelectAccountHead(Constants.AC_DetailTypeId, int.Parse(ddAccountSubType2.SelectedValue.ToString()), DrpAccountCategory.SelectedIndex);
            clsWebFormUtil.FillDropDownList(drpAccountTypeDetail, dt, 0, 10, true);
        }
    }

    /// <summary>
    /// Loads Account Heads To AccountHead Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">GridViewEditEventArgs</param>
    protected void drpAccountTypeDetail_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadAssignAccunt();
        LoadUnAssignAccunt();
    }

    /// <summary>
    /// Loads Account Heads To AccountHead Grid
    /// </summary>
    //private void GetAccountHead()
    //{
    //    if (drpAccountTypeDetail.Items.Count > 0)
    //    {
    //        DataTable dt = MController.SelectAccountHead(Constants.AC_AccountHeadId, int.Parse(drpAccountTypeDetail.SelectedValue.ToString()), DrpAccountCategory.SelectedIndex);

    //    }
    //}
    protected void DrpAccountCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.GetAccountType();
       
        this.GetSubTypeForHead();
        
        this.GetDetailAccountTypeForHead();
        LoadAssignAccunt();
        LoadUnAssignAccunt();
    }

    protected void drpFormName_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadAssignAccunt();
        LoadUnAssignAccunt();
    }
}
