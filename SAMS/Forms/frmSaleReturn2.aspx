<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmSaleReturn2.aspx.cs" Inherits="Forms_frmSaleReturn2" Title="SAMS: Invoice Wise Sale Return" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
       <script src="../AjaxLibrary/jquery.searchabledropdown-1.0.8.min.js" type="text/javascript"></script>
<script type="text/javascript" language="javascript">

    function pageLoad() {

        $("select").searchable();
    }
   Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
   function BeginRequestHandler(sender, args) {
       var oControl = args.get_postBackElement();
       if (oControl.value == "Return Sale") {
           oControl.value = "Wait...";
           oControl.disabled = true;
       }
   }
</script>
    <div id="right_data">
        <div>
            <table width="100%">
                <tr>
                    <td>
                        <div style="z-index: 101; left: 534px; width: 100px; position: absolute; top: 256px;
                            height: 100px">
                            <asp:Panel ID="Panel2" runat="server">
                                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel2">
                                    <ProgressTemplate>
                                        <asp:ImageButton ID="ImageButton1" runat="server" Height="26px" ImageUrl="~/App_Themes/Granite/Images/image003.gif"
                                            Width="27px" />
                                        Wait Update.......
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                            </asp:Panel>
                        </div>
                    </td>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="lbltoLocation" runat="server" Width="94px" Text="Location" CssClass="lblbox"></asp:Label></strong>
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:DropDownList ID="drpDistributor" runat="server" Width="200px" CssClass="DropList"
                                                AutoPostBack="True" OnSelectedIndexChanged="drpDistributor_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>   
                                     <tr>
                                        <td align="left" style="height: 25px">
                                            <strong>
                                                <asp:Label ID="Label6" runat="server" Width="94px" Text="Order Booker"></asp:Label></strong>
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:DropDownList ID="DrpOrderBooker" runat="server" Width="200px" CssClass="DropList">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>                                 
                                    <tr>
                                        <td align="left" style="height: 25px">
                                            <strong>
                                                <asp:Label ID="lblFromDate" runat="server" Text="Date From"></asp:Label>
                                            </strong>
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:TextBox ID="txtFromDate" runat="server" Width="175px" CssClass="txtBox"></asp:TextBox>
                                            <asp:ImageButton
                                                ID="ibFromDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif" />
                                            <cc1:CalendarExtender ID="ceFromDate" runat="server" EnableViewState="False"
                                                Format="dd-MMM-yyyy" PopupButtonID="ibFromDate" TargetControlID="txtFromDate">
                                            </cc1:CalendarExtender>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="height: 25px">
                                            <strong>
                                                <asp:Label ID="lblToDate" runat="server" Text="Date To"></asp:Label>
                                            </strong>
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:TextBox ID="txtToDate" runat="server" Width="175px" CssClass="txtBox"></asp:TextBox>
                                            <asp:ImageButton
                                                ID="ibToDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif" />
                                            <cc1:CalendarExtender ID="ceToDate" runat="server" EnableViewState="False"
                                                Format="dd-MMM-yyyy" PopupButtonID="ibToDate" TargetControlID="txtToDate">
                                            </cc1:CalendarExtender>
                                        </td>
                                    </tr>
                                   
                                    <tr>
                                        <td align="right" colspan="2">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Button ID="btnGetOrder" OnClick="btnGetOrder_Click" runat="server" Width="100px"
                                                Font-Size="8pt" Text="Get Data" CssClass="Button" />
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:Button ID="btnReturn" runat="server" Font-Size="8pt" Text="Return Sale" Width="128px"
                                                OnClick="btnReturn_Click" CssClass="Button" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" colspan="2">
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
        </div>
        <div>
            <table width="100%">
                <tr>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table style="border-right: silver thin inset; border-top: silver thin inset; border-left: silver thin inset;
                                    width: 650px; border-bottom: silver thin inset">
                                    <tbody>
                                        <tr>
                                            <td style="height: 21px" align="left" colspan="5">
                                                <asp:Panel ID="Panel1" runat="server" Width="850px" Height="250px" ScrollBars="Vertical">
                                                    <asp:GridView ID="GrdOrder" runat="server" Width="830px" ForeColor="SteelBlue" CssClass="gridRow2"
                                                        DataKeyNames="SALE_INVOICE_ID" HorizontalAlign="Center" BorderColor="White" BackColor="White"
                                                        AutoGenerateColumns="False">
                                                        <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                                            PreviousPageText="Previous" />
                                                        <AlternatingRowStyle BackColor="#E0E0E0" />
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Select">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="ChbInvoice" runat="server" />
                                                                </ItemTemplate>
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" />
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="DOCUMENT_DATE" DataFormatString="{0:dd-MMM-yyyy}" HeaderText="Invoice Date">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="MANUAL_INVOICE_ID" HeaderText="Invoice No">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="CUSTOMER_CODE" HeaderText="Outlet Code">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Left" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="CUSTOMER_NAME" HeaderText="Outlet Name">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Left" />
                                                            </asp:BoundField>                                                                                                                        
                                                            <asp:BoundField DataField="TOTAL_AMOUNT" DataFormatString="{0:F2}" HeaderText="Gross Amount">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="DISCOUNT_AMOUNT" DataFormatString="{0:F2}" HeaderText="Standard Discount">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SCHEME_AMOUNT" DataFormatString="{0:F2}" HeaderText="Extra Discount">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="GST_AMOUNT" DataFormatString="{0:F2}" HeaderText="GST">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="TOTAL_NET_AMOUNT" DataFormatString="{0:F2}" HeaderText="Net Amount">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                        </Columns>
                                                        <HeaderStyle CssClass="tblhead" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:GridView>                                                    
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
