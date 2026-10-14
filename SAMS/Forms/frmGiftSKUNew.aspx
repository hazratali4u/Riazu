<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true" CodeFile="frmGiftSKUNew.aspx.cs" 
    Inherits="Forms_frmGiftSKUNew" Title="SAMS :: Gift SKU Entry New" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script language="JavaScript" type="text/javascript">
 
  function ValidateForm()
 {
		var str;
    
		str  = document.getElementById('<%=txtQuantity.ClientID%>').value; 
		if(str == null || str.length == 0)
		{
			alert('Must Enter Quantity');
			return false;
		}
		
        return true;
  }
    </script>
    <script type="text/javascript" language="javascript">
   Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
   function BeginRequestHandler(sender, args) {
       var oControl = args.get_postBackElement();
       if (oControl.value == "Add Sku") {
           oControl.value = "Wait...";
           oControl.disabled = true;
       }       
   }
    </script>
    <div id="right_data">
        <table width="100%">
            <tr>
                <td>
                    <div style="z-index: 101; left: 477px; width: 100px; position: absolute; top: 90px; height: 100px">
                        &nbsp;<asp:Panel ID="Panel21" runat="server">
                            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                                <ProgressTemplate>
                                    <asp:ImageButton ID="ImageButton1" runat="server" Height="26px" ImageUrl="~/App_Themes/Granite/Images/image003.gif"
                                        Width="23px" />
                                    Wait Update
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                        </asp:Panel>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
                            <table>
                                <tr>
                                    <td align="left" valign="top">
                                        <strong>
                                            <asp:Label ID="lbltoLocation" runat="server" CssClass="lblbox" Text="Location" Width="94px"></asp:Label></strong></td>
                                    <td align="left" valign="top" colspan="2">
                                        <asp:DropDownList ID="drpDistributor" runat="server" AutoPostBack="True" CssClass="DropList"
                                            OnSelectedIndexChanged="drpDistributor_SelectedIndexChanged" Width="280px">
                                        </asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td align="left" style="height: 17px" valign="top">
                                        <strong>
                                            <asp:Label ID="Label5" runat="server" CssClass="lblbox" Text="Principal" Width="94px"></asp:Label></strong></td>
                                    <td align="left" colspan="2" style="height: 17px" valign="top">
                                        <asp:DropDownList ID="DrpPrincipal" runat="server" CssClass="DropList" OnSelectedIndexChanged="DrpPrincipal_SelectedIndexChanged"
                                            Width="280px" AutoPostBack="True">
                                        </asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top" style="height: 17px">
                                        <strong>
                                            <asp:Label ID="lblDocumentNo" runat="server" CssClass="lblbox" Text="Invoice No" Width="79px"></asp:Label></strong></td>
                                    <td align="left" colspan="2" valign="top" style="height: 17px">
                                        <asp:DropDownList ID="drpDocumentNo" runat="server" Width="280px" CssClass="DropList" AutoPostBack="True" OnSelectedIndexChanged="drpDocumentNo_SelectedIndexChanged"></asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">
                                        <strong>
                                            <asp:Label ID="Label1" runat="server" CssClass="lblbox" Text="Customer Code" Width="110px"></asp:Label></strong></td>
                                    <td align="left" valign="top">
                                        <asp:TextBox ID="txtOutletCode" onkeyup="SearchList()" runat="server" Width="76px" CssClass="txtBox " ReadOnly="True"></asp:TextBox></td>
                                    <td align="left">
                                        <asp:TextBox ID="txtOutletName" runat="server" Width="188px" onfocus="SearchedCode()" CssClass="txtBox " ReadOnly="True"></asp:TextBox></td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:CheckBox ID="ChbAllTax" runat="server" Checked="True" Text="Is Apply All Tax" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <table>
                                <tr>
                                    <td style="height: 16px">
                                        <strong>
                                            <asp:Label ID="lblskuname" runat="server" CssClass="tblhead" Font-Bold="True"
                                                ForeColor="White" Height="16px" Text="   SKU Name" Width="250px"></asp:Label></strong></td>
                                    <td style="height: 16px">
                                        <strong>
                                            <asp:Label ID="Label3" runat="server" CssClass="tblhead" Font-Bold="True"
                                                ForeColor="White" Height="16px" Text="Quantity" Width="61px"></asp:Label></strong></td>
                                    <td style="height: 16px">
                                        <strong>
                                            <asp:Label ID="lblquantity" runat="server" CssClass="tblhead"
                                                Font-Bold="True" ForeColor="White" Height="16px" Text="Unit Price" Width="62px"></asp:Label></strong></td>
                                    <td align="center" style="height: 16px">
                                        <strong>
                                            <asp:Label ID="Label7" runat="server" CssClass="tblhead" Font-Bold="True"
                                                ForeColor="White" Height="16px" Text="GST Rate" Width="57px"></asp:Label></strong></td>
                                    <td style="height: 16px">
                                        <strong>
                                            <asp:Label ID="Label2" runat="server" CssClass="tblhead" Font-Bold="True"
                                                ForeColor="White" Height="16px" Text="Amount" Width="65px"></asp:Label></strong></td>
                                    <td style="height: 16px">
                                        <strong>
                                            <asp:Label ID="Label4" runat="server" CssClass="tblhead" Font-Bold="True"
                                                ForeColor="White" Height="16px" Text="Add SKU" Width="67px"></asp:Label></strong></td>
                                </tr>
                                <tr>
                                    <td style="height: 21px">
                                        <asp:DropDownList ID="ddlSKU" runat="server" Width="250px"></asp:DropDownList>
                                    </td>
                                    <td style="height: 21px">
                                        <asp:TextBox ID="txtQuantity" runat="server" CssClass="txtBox " onfocus="SearchSKUCode()"
                                            Width="55px"></asp:TextBox></td>
                                    <td style="height: 21px">
                                        <asp:TextBox ID="txtUnitRate" runat="server" CssClass="txtBox" Enabled="False" Width="56px"></asp:TextBox></td>
                                    <td style="height: 21px">
                                        <asp:TextBox ID="txtBatchNo" runat="server" CssClass="txtBoxnNum" onfocus="SearchSKUCode()"
                                            Width="62px"></asp:TextBox></td>
                                    <td style="height: 21px">
                                        <asp:TextBox ID="TextBox1" runat="server" CssClass="txtBoxnNum" Enabled="False" onfocus="SearchSKUCode()"
                                            Width="72px"></asp:TextBox></td>
                                    <td style="height: 21px">
                                        <asp:Button ID="btnSave" runat="server" AccessKey="A" OnClick="btnSave_Click"
                                            Text="Add Sku" ValidationGroup="vg" CssClass="btn" /></td>
                                </tr>
                                <tr>
                                    <td align="left" colspan="6">
                                        <asp:Panel ID="Panel2" runat="server" BorderColor="Silver" BorderStyle="Groove" BorderWidth="1px"
                                            Height="130px" ScrollBars="Vertical" Width="640px">
                                            <asp:GridView ID="GrdPurchase" runat="server" AutoGenerateColumns="False" BackColor="White"
                                                BorderColor="White" CssClass="gridRow2" ForeColor="SteelBlue" HorizontalAlign="Center"
                                                OnRowDeleting="GrdPurchase_RowDeleting" OnRowEditing="GrdPurchase_RowEditing"
                                                ShowHeader="False">
                                                <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                                    PreviousPageText="Previous" />
                                                <RowStyle ForeColor="Black" />
                                                <Columns>
                                                    <asp:BoundField DataField="SALE_INVOICE_PROMOTION_ID" HeaderText="SALE_INVOICE_PROMOTION_ID">
                                                        <HeaderStyle CssClass="hiddencol" />
                                                        <ItemStyle CssClass="hiddencol" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="SKU_ID" HeaderText="SKU_ID">
                                                        <HeaderStyle CssClass="hiddencol" />
                                                        <ItemStyle CssClass="hiddencol" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="SKU_CODE" HeaderText="SKU Code">
                                                        <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Left"
                                                            Width="75px" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="SKU_NAME" HeaderText="SKU Name">
                                                        <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Left"
                                                            Width="200px" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="QUANTITY_UNIT" HeaderText="Quantity">
                                                        <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Right"
                                                            Width="65px" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="UNIT_PRICE" DataFormatString="{0:F2}" HeaderText="PRICE">
                                                        <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Right"
                                                            Width="65px" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="GST_RATE" HeaderText="GST Rate">
                                                        <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" Width="70px" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Amount" DataFormatString="{0:F2}" HeaderText="Amount">
                                                        <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Right"
                                                            Width="80px" />
                                                    </asp:BoundField>
                                                    <asp:CommandField HeaderText="Edit" ShowEditButton="True">
                                                        <HeaderStyle CssClass="hiddencol" />
                                                        <ItemStyle CssClass="hiddencol" />
                                                    </asp:CommandField>
                                                    <asp:TemplateField HeaderText="Delete">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" OnClientClick="javascript:return confirm('Are you sure you want to Delete?');return false;"
                                                                Text="Delete"></asp:LinkButton>
                                                        </ItemTemplate>
                                                        <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" Width="45px" />
                                                    </asp:TemplateField>
                                                </Columns>
                                                <FooterStyle BackColor="White" />
                                                <PagerStyle BackColor="Transparent" />
                                                <HeaderStyle CssClass="btn" />
                                                <AlternatingRowStyle BackColor="#F2F2F2" CssClass="GridAlternateRowStyle" ForeColor="#333333" />
                                            </asp:GridView>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
