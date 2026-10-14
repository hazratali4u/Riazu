<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmOrderEntryStep1.aspx.cs" Inherits="Forms_frmOrderEntryStep1" Title="SAMS: Order/Invoice Step 1" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script language="JavaScript" type="text/javascript">
                Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(startRequest);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequest);
        function startRequest(sender, e) {
            document.getElementById('<%=btnPost.ClientID%>').disabled = true;
            document.getElementById('<%=btnUpdateOrder.ClientID%>').disabled = true;
            document.getElementById('<%=btnPrint.ClientID%>').disabled = true;
        }
        function endRequest(sender, e) {
            document.getElementById('<%=btnPost.ClientID%>').disabled = false;
            document.getElementById('<%=btnUpdateOrder.ClientID%>').disabled = false;
            document.getElementById('<%=btnPrint.ClientID%>').disabled = false;
        }
        function SelectAllRoutes() {
            var chkBoxList = document.getElementById('<%= LstRoutes.ClientID %>');
            var chkBox = document.getElementById('<%= ChbAllRoutes.ClientID %>');
            if (chkBox.checked == true) {
                var chkBoxCount = chkBoxList.getElementsByTagName("input");
                for (var i = 0; i < chkBoxCount.length; i++) {
                    chkBoxCount[i].checked = true;
                }
            }
            else {
                var chkBoxCount = chkBoxList.getElementsByTagName("input");
                for (var i = 0; i < chkBoxCount.length; i++) {
                    chkBoxCount[i].checked = false;
                }
            }
        }
        function UnCheckSelectAllRoutes() {
            var chkBox = document.getElementById('<%= ChbAllRoutes.ClientID %>');
            var chkBoxList = document.getElementById('<%= LstRoutes.ClientID %>');
            var chkBoxCount = chkBoxList.getElementsByTagName("input");
            var count = 0;
            for (var i = 0; i < chkBoxCount.length; i++) {
                if (chkBoxCount[i].checked == false) {
                    count += 1;
                }
            }
            if (count > 0) {
                chkBox.checked = false;
            }
            else {
                chkBox.checked = true;
            }
        }
    </script>
    <div id="right_data">
        <div>
            <span class="heading">Order/Invoice Step 1</span>
        </div>
        <div>
            <table width="100%">
                <tr>
                    <td align="left">
                        <div style="z-index: 101; left: 397px; width: 100px; position: absolute; top: 109px; height: 100px">
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
                    <td align="left">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tbody>
                                        <tr>
                                            <td align="left">
                                                <strong>
                                                    <asp:Label ID="Label7" runat="server" Width="104px" Text="Transaction Type" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 238px; height: 25px" align="left">
                                                <asp:DropDownList ID="DrpDocumentType" runat="server" Width="200px" CssClass="DropList"
                                                    AutoPostBack="True" OnSelectedIndexChanged="DrpDocumentType_SelectedIndexChanged">
                                                    <asp:ListItem Value="0">New Order</asp:ListItem>
                                                    <asp:ListItem Value="1">Spot Sale</asp:ListItem>
                                                    <asp:ListItem Value="2">Sale Return</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <strong>
                                                    <asp:Label ID="lbltoLocation" runat="server" Width="94px" Text="Location" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 238px; height: 25px" align="left">
                                                <asp:DropDownList ID="drpDistributor" runat="server" Width="200px" CssClass="DropList"
                                                    AutoPostBack="True" OnSelectedIndexChanged="drpDistributor_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <strong>
                                                    <asp:Label ID="Label9" runat="server" Width="94px" Text="Order Bookers" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 238px; height: 25px" align="left">
                                                <asp:DropDownList ID="DrpOrderBooker" runat="server" Width="200px" CssClass="DropList"
                                                    OnSelectedIndexChanged="DrpOrderBooker_SelectedIndexChanged" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left"></td>
                                            <td style="width: 204px; height: 2px" align="left">
                                                <asp:CheckBox ID="ChbAllRoutes" onclick="SelectAllRoutes()" runat="server" Text=" Select All"></asp:CheckBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <strong>
                                                    <asp:Label ID="lblfromLocation" runat="server" Width="94px" Text="Customer Route"
                                                        CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="height: 2px" align="left">
                                                <asp:Panel ID="Panel3" runat="server" Width="200px" Height="140px" ScrollBars="Vertical"
                                                    BorderWidth="1px" BorderStyle="Groove">
                                                    <asp:CheckBoxList ID="LstRoutes" onclick="UnCheckSelectAllRoutes()" OnSelectedIndexChanged="LstRoutes_SelectedIndexChanged" AutoPostBack="true" runat="server">
                                                    </asp:CheckBoxList>
                                                </asp:Panel>
                                            </td>
                                            <%-- <td style="width: 238px; height: 25px" align="left">
                                                <asp:DropDownList ID="DrpRoute" runat="server" Width="200px" CssClass="DropList"
                                                    AutoPostBack="True" OnSelectedIndexChanged="DrpRoute_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>--%>
                                            <td rowspan="3">
                                                <fieldset style="border-width: 1px; width: auto; height: auto;">
                                                    <table>
                                                        <tr>
                                                            <td align="left">
                                                                <strong>
                                                                    <asp:Label ID="Label1" runat="server" Text="New Delivery Date" Width="124px" Height="16px"></asp:Label></strong>
                                                            </td>
                                                            <td align="left" style="width: 238px; height: 25px">
                                                                <asp:TextBox ID="txtNewDeliveryDate" runat="server" CssClass="txtBox" MaxLength="10"
                                                                    Width="150px"></asp:TextBox>
                                                                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif" />
                                                                <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" EnableViewState="False"
                                                                    Format="dd-MMM-yyyy" PopupButtonID="ImageButton2" TargetControlID="txtNewDeliveryDate">
                                                                </ajaxToolkit:CalendarExtender>
                                                            </td>
                                                            <td></td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left">
                                                                <strong>
                                                                    <asp:Label ID="Label8" runat="server" Text="Reason" Width="94px"></asp:Label></strong>
                                                            </td>
                                                            <td align="left" style="width: 238px; height: 25px">
                                                                <asp:DropDownList ID="DrpDateUpdateReason" runat="server" Width="156px" CssClass="DropList">
                                                                    <asp:ListItem Value="1">Strike</asp:ListItem>
                                                                    <asp:ListItem Value="2">Public Holiday</asp:ListItem>
                                                                    <asp:ListItem Value="3">Stock Shortage</asp:ListItem>
                                                                    <asp:ListItem Value="4">Petrol Shortage</asp:ListItem>
                                                                    <asp:ListItem Value="5">Natural Disaster</asp:ListItem>
                                                                    <asp:ListItem Value="6">Distributor Incident</asp:ListItem>
                                                                    <asp:ListItem Value="7">Heavy Rain</asp:ListItem>
                                                                    <asp:ListItem Value="8">Vehicle Not Available</asp:ListItem>
                                                                    <asp:ListItem Value="9">Delivery Man Not Available</asp:ListItem>
                                                                    <asp:ListItem Value="10">Sale Return complete order</asp:ListItem>
                                                                    <asp:ListItem Value="11">Taken part quantity item</asp:ListItem>
                                                                    <asp:ListItem Value="12">Cash or Cheque not available</asp:ListItem>
                                                                    <asp:ListItem Value="13">Shop Closed</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </fieldset>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <strong>
                                                    <asp:Label ID="Label5" runat="server" Width="94px" Text="Delivery Man" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 238px; height: 25px" align="left">
                                                <asp:DropDownList ID="DrpDeliveryMan" runat="server" Width="200px" CssClass="DropList"
                                                    OnSelectedIndexChanged="DrpDeliveryMan_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <strong>
                                                    <asp:Label ID="Label6" runat="server" Width="89px" Text="Working Date" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td>
                                                <asp:TextBox Style="text-align: justify" ID="txtToDate" onkeyup="BlockFromDateKeyPress()"
                                                    runat="server" Width="192px" CssClass="txtBox" MaxLength="1"></asp:TextBox>
                                                <asp:ImageButton ID="ImgBntFromCalc" runat="server" CausesValidation="False" Enabled="True"
                                                    ImageUrl="~/App_Themes/Granite/Images/date.gif" Width="17px"></asp:ImageButton>
                                                <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" CssClass="MYstyle"
                                    TargetControlID="txtToDate" Format="dd-MMM-yyyy" PopupButtonID="ImgBntFromCalc">
                                </ajaxToolkit:CalendarExtender>
                                            </td>
                                        </tr>
                                        <%--<tr>
                                            <td align="left">
                                                <strong>
                                                    <asp:Label ID="Label7" runat="server" Text="Working Date" Width="94px"></asp:Label></strong>
                                            </td>
                                            <td align="left" style="width: 238px; height: 25px">
                                                <asp:TextBox ID="txtToDate" runat="server" CssClass="txtBox" MaxLength="10" Width="150px"></asp:TextBox>
                                                <asp:ImageButton ID="ImgBntToDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif" />
                                                <cc1:CalendarExtender ID="CalendarExtender2" runat="server" EnableViewState="False" CssClass="MyClndr"
                                                    Format="dd-MMM-yyyy" PopupButtonID="ImgBntToDate" TargetControlID="txtToDate">
                                                </cc1:CalendarExtender>
                                            </td>
                                        </tr>--%>
                                        <tr>
                                            <td align="left">
                                                <strong>
                                                    <asp:Label ID="Label4" runat="server" Width="94px" Text="Quantity Type" CssClass="lblbox" Visible="false"></asp:Label></strong>
                                            </td>
                                            <td style="width: 238px; height: 25px" align="left">
                                                <asp:RadioButtonList ID="RbUnitType" runat="server" Width="225px" RepeatDirection="Horizontal" Visible="false">
                                                    <asp:ListItem Selected="True">Quantity In Unit</asp:ListItem>
                                                    <asp:ListItem>Quantity In Ctns</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <asp:Button ID="btnGetOrder" OnClick="btnGetOrder_Click" runat="server" Width="100px"
                                                    Font-Size="8pt" Text="Get Order" CssClass="Button" />
                                            </td>
                                            <td style="width: 238px; height: 25px" align="left">
                                                <asp:Button ID="btnNext" OnClick="btnNext_Click" runat="server" Width="100px" Font-Size="8pt"
                                                    Text="Next" CssClass="Button" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" colspan="2">
                                                <asp:GridView ID="GrdFreeSKU" runat="server" Visible="False" Width="100%" Height="57px"
                                                    ForeColor="Red" CssClass="gridRow2" AutoGenerateColumns="False" BackColor="White"
                                                    BorderColor="White" HorizontalAlign="Center">
                                                    <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next" PreviousPageText="Previous" />
                                                    <RowStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ForeColor="Black" />
                                                    <Columns>
                                                        <asp:BoundField DataField="SALE_ORDER_ID" HeaderText="Order Id">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                            <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="SKU_Code" HeaderText="SKU Code">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                            <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Left"
                                                                Width="80px" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="QUANTITY_UNIT" HeaderText="Order Qty">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="BALANCE" HeaderText="Stock">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center"
                                                                Width="50px" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                    <FooterStyle BackColor="White" />
                                                    <PagerStyle BackColor="Transparent" />
                                                    <HeaderStyle BackColor="Red" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"
                                                        ForeColor="Black" />
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" BackColor="#007395" ForeColor="White" />
                                                    <AlternatingRowStyle BackColor="#F2F2F2" CssClass="GridAlternateRowStyle" ForeColor="#333333" />
                                                </asp:GridView>
                                                <asp:GridView ID="gvRateDifference" runat="server" Visible="False" Width="100%" Height="57px"
                                                    ForeColor="Red" CssClass="gridRow2" AutoGenerateColumns="False" BackColor="White"
                                                    BorderColor="White" HorizontalAlign="Center">
                                                    <PagerSettings PreviousPageText="Previous" Mode="NextPrevious" LastPageText="" FirstPageText=""
                                                        NextPageText="Next"></PagerSettings>
                                                    <FooterStyle BackColor="White"></FooterStyle>
                                                    <Columns>
                                                        <asp:BoundField DataField="SALE_ORDER_ID" HeaderText="Order Id">
                                                            <ItemStyle BorderStyle="Solid" BorderWidth="1px" BorderColor="Silver" HorizontalAlign="Left"></ItemStyle>
                                                            <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="SKU_Code" HeaderText="SKU Code">
                                                            <ItemStyle BorderStyle="Solid" Width="80px" BorderWidth="1px" BorderColor="Silver"
                                                                HorizontalAlign="Left"></ItemStyle>
                                                            <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Order_Rate" HeaderText="Order Rate">
                                                            <ItemStyle BorderStyle="Solid" BorderWidth="1px" BorderColor="Silver" HorizontalAlign="Center"></ItemStyle>
                                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="SSR_Rate" HeaderText="SSR Rate">
                                                            <ItemStyle BorderStyle="Solid" Width="50px" BorderWidth="1px" BorderColor="Silver"
                                                                HorizontalAlign="Center"></ItemStyle>
                                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                        </asp:BoundField>
                                                    </Columns>
                                                    <RowStyle BorderStyle="Solid" ForeColor="Black" BorderWidth="1px" BorderColor="Silver"></RowStyle>
                                                    <PagerStyle BackColor="Transparent"></PagerStyle>
                                                    <HeaderStyle BackColor="Red" BorderStyle="Solid" ForeColor="Black" BorderWidth="1px"
                                                        BorderColor="Black"></HeaderStyle>
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" BackColor="#007395" ForeColor="White" />
                                                    <AlternatingRowStyle BackColor="#F2F2F2" CssClass="GridAlternateRowStyle" ForeColor="#333333" />
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td></td>
                </tr>
            </table>
        </div>
        <div>
            <table width="100%">
                <tr>
                    <td style="width: 100px; height: 173px;" align="left" valign="top">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table style="width: 700px;">
                                    <tbody>
                                        <tr>
                                            <td>
                                                <strong>
                                                    <asp:CheckBox ID="cbAll" runat="server" Width="103px" Text="Select All" AutoPostBack="True"
                                                        OnCheckedChanged="cbAll_CheckedChanged"></asp:CheckBox>
                                                </strong>
                                            </td>
                                            <td>
                                                <strong>
                                                    <asp:Label ID="Label3" runat="server" Width="100px" Text="Payment Mode"></asp:Label></strong>
                                            </td>
                                            <td style="z-index:-1 !important;">
                                                <asp:DropDownList ID="ddSearchType" runat="server" Width="176px" style="z-index:-1 !important;">
                                                    <asp:ListItem Value="214">Cash</asp:ListItem>
                                                    <asp:ListItem Value="215">Credit</asp:ListItem>
                                                    <asp:ListItem Value="216">Advance</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnPost" OnClick="btnPost_Click" runat="server" Width="128px" Font-Size="8pt"
                                                    Text="Convert to Invoice" CssClass="Button" />
                                            </td>
                                            <td>
                                                <asp:Button ID="btnUpdateOrder" runat="server" Width="94px" Font-Size="8pt" Text="Update Order"
                                                    CssClass="Button" OnClick="btnUpdateOrder_Click" />
                                            </td>
                                            <td>
                                                <asp:Button ID="btnPrint" runat="server" Width="104px" Font-Size="8pt" Text="Fresh Return"
                                                    CssClass="Button" OnClick="btnPrint_Click" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 500px; height: 21px" colspan="6">
                                                <asp:Panel ID="Panel1" runat="server" Width="100%" Height="180px" ScrollBars="Vertical" BorderWidth="1px">
                                                    <asp:GridView ID="GrdOrder" runat="server" AutoGenerateColumns="False" BackColor="White"
                                                        BorderColor="White" CssClass="gridRow2" ForeColor="SteelBlue" HorizontalAlign="Center"
                                                        Width="100%">
                                                        <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                                            PreviousPageText="Previous" />
                                                        <Columns>
                                                            <asp:BoundField DataField="CUSTOMER_ID" HeaderText="Customer Id">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:TemplateField HeaderText="Select">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="ChbInvoice" runat="server" />
                                                                </ItemTemplate>
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="CUSTOMER_CODE" HeaderText="Code">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Left" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="CUSTOMER_NAME" HeaderText="Name">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Left" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="DISTRIBUTOR_DOC_ID" HeaderText="Sale Order Id">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="DOCUMENT_DATE" HeaderText="Order Date">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="TOTAL_AMOUNT" DataFormatString="{0:F2}" HeaderText="Gross Amount">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="DISCOUNT_AMOUNT" DataFormatString="{0:F2}" HeaderText="Discount">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SCHEME_AMOUNT" DataFormatString="{0:F2}" HeaderText="Scheme">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="GST_AMOUNT" DataFormatString="{0:F2}" HeaderText="GST Amount">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="TOTAL_NET_AMOUNT" DataFormatString="{0:F2}" HeaderText="Net Amount">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="DELIVERYMAN_ID" HeaderText="DELIVERYMAN_ID">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="MANUAL_ORDER_ID" HeaderText="Bill Book No">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="PRINCIPAL_ID" HeaderText="PRINCIPAL_ID">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="COST_OF_GOODS" HeaderText="COST_OF_GOODS">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="YC_DISCOUNT" HeaderText="YcDiscount">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="FREE_SKU_AMOUNT" HeaderText="FREE_SKU_AMOUNT">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="EXTRA_TAX" HeaderText="EXTRA_TAX">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="TOTAL_WHTAX_AMOUNT" HeaderText="COST_OF_GOODS">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SALE_ORDER_ID" HeaderText="Sale Order Id">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                        </Columns>
                                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" BackColor="#007395" ForeColor="White" />
                                                        <AlternatingRowStyle BackColor="#F2F2F2" CssClass="GridAlternateRowStyle" ForeColor="#333333" />
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
