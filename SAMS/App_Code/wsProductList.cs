using System.Collections.Generic;
using System.Data;
using System.Web.Script.Services;
using System.Web.Services;
using SAMSBusinessLayer.Classes;
using SAMSCommon.Classes;
using System;

/// <summary>
/// Summary description for wsProductList
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class wsProductList : System.Web.Services.WebService
{
    public wsProductList()
    {
    }
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string[] GetPosProducts(string prefixText,int count, string contextKey)
   {
        var products = new List<string>();
        var mSkuController = new SkuController();
        int distributorId = Constants.IntNullValue;
        if (contextKey != null)
        {
            distributorId = Convert.ToInt32(contextKey);
        }
        DataTable dtskuPrice = mSkuController.SearchProduct(prefixText, distributorId);
        for (int i = 0; i < dtskuPrice.Rows.Count; i++)
        {
            products.Add(AjaxControlToolkit.AutoCompleteExtender.CreateAutoCompleteItem(dtskuPrice.Rows[i]["SKUDETAIL2"].ToString(), dtskuPrice.Rows[i]["SKU_ID"].ToString()));
        }
        return products.ToArray();
    }
}