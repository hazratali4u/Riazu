<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmOrderEntry.aspx.cs" Inherits="Forms_frmOrderEntry" Title="SAMS: Order/Invoice Step 2" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script type="text/javascript" src="../AjaxLibrary/jquery.searchabledropdown-1.0.8.min.js"></script>
    <script language="JavaScript" type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(startRequest);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequest);
        function startRequest(sender, e) {

            document.getElementById('<%=btnSave.ClientID%>').disabled = true;
            document.getElementById('<%=btnCalculate.ClientID%>').disabled = true;

        }
        function endRequest(sender, e) {

            document.getElementById('<%=btnSave.ClientID%>').disabled = false;
            document.getElementById('<%=btnCalculate.ClientID%>').disabled = false;
        }
        function ValidateForm() {
            var str;
            str = document.getElementById('<%=txtQuantity.ClientID%>').value;
            str1 = document.getElementById('<%=txtQuantityCtn.ClientID%>').value;
            if (str1 == null || str1.length == 0) {
                if (str == null || str.length == 0) {
                    alert('Must Enter Order Quantity');
                    return false;
                }
            }
            return true;
        }




        function ClearSelection(lb) {
            lb.selectedIndex = -1;
        }
        function pageLoad() {
            $("select").searchable();
            if (document.getElementById('<%=btnSave.ClientID%>').value == "Add Sku") {
                document.getElementById("<%= txtStock.ClientID %>").value = '0-0';
                var SKUDetail = $("[id*='DrpSkuDetail'] :selected").text().split(':');
                document.getElementById("<%= txtStock.ClientID %>").value = SKUDetail[2];
                document.getElementById("<%= txtUnitRate.ClientID %>").value = SKUDetail[1];
                document.getElementById("<%= DrpSkuDetail.ClientID %>").focus();
            }

            $("#<%=DrpSkuDetail.ClientID %>").change(function () {
                document.getElementById("<%= txtStock.ClientID %>").value = '0-0';
                var SKUDetail = $("[id*='DrpSkuDetail'] :selected").text().split(':');
                document.getElementById("<%= txtStock.ClientID %>").value = SKUDetail[2];
                document.getElementById("<%= txtUnitRate.ClientID %>").value = SKUDetail[1];
                document.getElementById("<%= txtQuantityCtn.ClientID %>").focus();
            });
        }


        function RbtDistributorDiscount() {
            var selectedvalue = $('#<%= RbdiscountType.ClientID %> input:checked').val()
            if (selectedvalue == 0) {

                document.getElementById('<%=txtDDPer.ClientID%>').disabled = false;
                document.getElementById('<%=numtxtYcDiscount.ClientID%>').disabled = true;
                document.getElementById('<%=txtDDPer.ClientID%>').value = '';
                document.getElementById('<%=numtxtYcDiscount.ClientID%>').value = '';
            }
            else {
                document.getElementById('<%=txtDDPer.ClientID%>').disabled = true;
                document.getElementById('<%=numtxtYcDiscount.ClientID%>').disabled = false;
                document.getElementById('<%=txtDDPer.ClientID%>').value = '';
                document.getElementById('<%=numtxtYcDiscount.ClientID%>').value = '';
            }
        }
        function RbtStdDisc() {
            document.getElementById('<%=txtDistributorDiscount.ClientID%>').value = '0.00';
        }
        function RbtExtraDisc() {
            document.getElementById('<%=txtExtradiscount.ClientID%>').value = '0.00';
        }
    </script>
    <div id="right_data">
        <div>
            <span class="heading">Order/Invoice Step 2</span>
        </div>
        <div>
            <table width="100%">
                <tr>
                    <td align="left">
                        <div style="z-index: 101; left: 412px; width: 100px; position: absolute; top: 369px; height: 100px">
                            &nbsp;<asp:Panel ID="Panel21" runat="server">
                                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel3">
                                    <ProgressTemplate>
                                        <asp:ImageButton ID="ImageButton1" runat="server" Height="28px" ImageUrl="~/App_Themes/Granite/Images/image003.gif"
                                            Width="31px" />
                                        Wait Update
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                            </asp:Panel>
                        </div>
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tbody>
                                        <tr>
                                            <td valign="top" align="left">
                                                <strong>
                                                    <asp:Label ID="lblDocumentNo" runat="server" Width="74px" Text="Order No" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td colspan="2">
                                                <asp:DropDownList ID="drpDocumentNo" runat="server" Width="280px" CssClass="DropList"
                                                    OnSelectedIndexChanged="drpDocumentNo_SelectedIndexChanged" AutoPostBack="True">
                                                </asp:DropDownList>
                                                <td>
                                                    <strong>
                                                        <asp:Label ID="lblBillBook" runat="server" Text="Sales Tax Invoice No:" CssClass="lblbox"></asp:Label></strong>

                                                    <asp:TextBox ID="txtBillBookNo" runat="server" Width="100px" CssClass="uppercase" MaxLength="10"></asp:TextBox>
                                                </td>
                                            </td>
                                            <td rowspan="5">
                                                <asp:Panel ID="CreditPanel" runat="server" Visible="false">
                                                    <fieldset style="width: 300px;">
                                                        <table>
                                                            <tr>
                                                                <td style="text-align: left; width: 130px;">
                                                                    <strong>
                                                                        <asp:Label ID="Label22" runat="server" Text="Credit Days:" CssClass="lblbox"></asp:Label>
                                                                    </strong>
                                                                </td>
                                                                <td style="font-family: Verdana; width: 180px; text-align: right;">
                                                                    <asp:Label ID="lblCreditDays" runat="server"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="text-align: left; width: 130px;">
                                                                    <strong>
                                                                        <asp:Label ID="Label23" runat="server" Text="Credit Limit:" CssClass="lblbox"></asp:Label>
                                                                    </strong>
                                                                </td>
                                                                <td style="font-family: Verdana; width: 180px; text-align: right;">
                                                                    <asp:Label ID="lblSaleLimit" runat="server"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="text-align: left; width: 130px;">
                                                                    <strong>
                                                                        <asp:Label ID="Label24" runat="server" Text="Remaining Limit:" CssClass="lblbox"></asp:Label>
                                                                    </strong>
                                                                </td>
                                                                <td style="font-family: Verdana; width: 180px; text-align: right;">
                                                                    <asp:Label ID="lblRemainingSaleLimit" runat="server"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="2" style="display: none;">
                                                                    <strong>
                                                                        <asp:CheckBox ID="chkLimit" Text="Skip Limit" Font-Size="14px" runat="server" />
                                                                    </strong>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Literal runat="server" ID="ltrBusinessDealText"></asp:Literal>
                                                                </td>
                                                                <td>
                                                                    <asp:Literal runat="server" ID="ltrBusinessDeal"></asp:Literal>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </fieldset>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="ChbDiscount" runat="server" Visible="False" Width="90px" Text="Promotion"
                                                    AutoPostBack="True" Checked="True"></asp:CheckBox></td>
                                        </tr>
                                        <tr>
                                            <td valign="middle" align="left">
                                                <strong>
                                                    <asp:Label ID="Label11" runat="server" Width="77px" Text="Order Type" CssClass="lblbox"></asp:Label>
                                                </strong>
                                            </td>
                                            <td valign="top" align="left" colspan="2">
                                                <asp:RadioButtonList ID="RblPayMode" runat="server" Width="228px" Height="1px" RepeatDirection="Horizontal" OnSelectedIndexChanged="RblPayMode_SelectedIndexChanged" AutoPostBack="true">
                                                    <asp:ListItem Value="214">Cash</asp:ListItem>
                                                    <asp:ListItem Value="215">Credit</asp:ListItem>
                                                    <asp:ListItem Value="216">Advance</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                            <td valign="top" align="left" colspan="1">
                                                <asp:CheckBox ID="ChbBatchNo" runat="server" Visible="false" Width="94px" Text="Batch No" AutoPostBack="True"
                                                    OnCheckedChanged="ChbBatchNo_CheckedChanged"></asp:CheckBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="middle" align="left">
                                                <strong>
                                                    <asp:Label ID="Label12" runat="server" Width="101px" Text="Route/Saleman" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td valign="top" align="left" colspan="2">
                                                <asp:TextBox ID="txtprincipal" runat="server" Width="272px"
                                                    CssClass="txtBox " ReadOnly="True"></asp:TextBox>
                                            </td>
                                            <td valign="top" align="left" colspan="1">
                                                <asp:TextBox ID="txtDeliveryMan" runat="server" Width="180px"
                                                    CssClass="txtBox " ReadOnly="True"></asp:TextBox>
                                                <strong>
                                                    <asp:Label ID="Label21" runat="server" Width="200px" Text="Customer Ledger Balance" CssClass="lblbox"></asp:Label>
                                                </strong>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 23px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label1" runat="server" Width="74px" Text="Customer" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="height: 23px" align="left" colspan="2">
                                                <asp:DropDownList ID="drpcustomer" runat="server" Width="280px" AutoPostBack="true" OnSelectedIndexChanged="drpcustomer_SelectedIndexChanged"></asp:DropDownList>
                                            </td>
                                            <td style="height: 23px" align="left">
                                                <asp:TextBox ID="txtDiscountType" runat="server" Width="180px"
                                                    CssClass="txtBox " ReadOnly="True"></asp:TextBox>
                                                <asp:TextBox ID="txtCustomerLedgerBalance" runat="server" Width="180px" CssClass="txtBox " ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr style="display: none;">
                                            <td align="left"><strong>
                                                <asp:Label ID="lbltruck" runat="server" Width="74px" Text="Truck No" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td valign="top" align="left" colspan="2">
                                                <asp:TextBox ID="txtTruckNo" runat="server" Width="76px"
                                                    CssClass="txtBox "></asp:TextBox>
                                                <strong>
                                                    <asp:Label ID="Label16" runat="server" Width="74px" Text="Driver Name" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                        </tr>
                                        <asp:Panel ID="PanelPrincipal" runat="server">
                                            <tr>
                                                <td align="left">
                                                    <strong>
                                                        <asp:Label ID="Label17" runat="server" Width="94px" Text="Principal" CssClass="lblbox"
                                                            __designer:wfdid="w1"></asp:Label></strong>
                                                </td>
                                                <td style="width: 238px; height: 25px" colspan="2" align="left">
                                                    <asp:DropDownList ID="DrpPrincipal" runat="server" Width="280px" CssClass="DropList" TabIndex="2"
                                                        AutoPostBack="True" OnSelectedIndexChanged="DrpPrincipal_SelectedIndexChanged"
                                                        __designer:wfdid="w2">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </asp:Panel>
                                        <tr>
                                            <td>
                                                <strong>
                                                    <label>Stock Use By</label>
                                                </strong>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="drpAssignedDistributor" runat="server" CssClass="DropList" TabIndex="3"
                                                    Width="280px" OnSelectedIndexChanged="drpAssignedDistributor_SelectedIndexChanged" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>
                                            <td valign="top" align="left" colspan="2">
                                                <strong>
                                                    <asp:Label ID="lblAutoFreeSKU" runat="server" Width="100%" Text=" With Free SKU Auto Promotion." Style="color: red; margin-left: 5px;" CssClass="lblbox"></asp:Label></strong>
                                                <asp:CheckBox ID="isGSTCal" Checked="false" Visible="false" runat="server" Text="Calculate GST" />
                                                <asp:HiddenField ID="hdnCustomerType" Value="0" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <strong>
                                                    <label>Remarks</label>
                                                </strong>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDriverName" runat="server" Width="272px" TabIndex="4"
                                                    CssClass="txtBox" TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                            <td></td>
                                            <td colspan="2">
                                                <asp:Panel ID="pnlBlacklist" runat="server" Height="100%" Width="100%">
                                                    <table width="100%">
                                                        <tr>
                                                            <td>
                                                                <strong>
                                                                    <asp:Label ID="lblBlacklistedCustomer" runat="server" Width="280px" Text="" CssClass="lblbox" Visible="false" Font-Size="Medium" Font-Bold="true" ForeColor="Red"></asp:Label></strong>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                        &nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
                    </td>
                    <td style="height: 126px"></td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Panel ID="Panel5" runat="server" DefaultButton="btnSave">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <table>
                                        <tr>
                                            <td style="height: 16px" colspan="2">
                                                <strong>Available Stock:</strong>
                                                <asp:TextBox ID="txtStock" runat="server" Text="0-0" Enabled="false" Width="100px"></asp:TextBox>
                                            </td>
                                            <td style="height: 16px" colspan="3">
                                                <asp:Label ID="lblDiscountImplementation" runat="server" ForeColor="Red" Font-Bold="True" Text="Manual Discount Implementation : Per Ctn"></asp:Label>
                                            </td>
                                            <td style="height: 16px"></td>
                                            <td style="height: 16px">
                                                <asp:Panel ID="PanelRbtStdDisc" runat="server">
                                                    <asp:RadioButtonList ID="RbStdDiscType" runat="server" Width="90px" onclick="RbtStdDisc();" RepeatDirection="Horizontal">
                                                        <asp:ListItem Value="0" Selected="True">%</asp:ListItem>
                                                        <asp:ListItem Value="1">Value</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </asp:Panel>
                                            </td>
                                            <td style="height: 16px">
                                                <asp:Panel ID="PanelRbtExtraDisc" runat="server">
                                                    <asp:RadioButtonList ID="RbExtraDiscType" runat="server" Width="85px" onclick="RbtExtraDisc();" RepeatDirection="Horizontal">
                                                        <asp:ListItem Value="0" Selected="True">%</asp:ListItem>
                                                        <asp:ListItem Value="1">Value</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </asp:Panel>
                                            </td>
                                            <td style="height: 16px"></td>
                                            <td style="height: 16px"></td>

                                        </tr>
                                        <tr style="width: 100%">
                                            <td style="height: 16px" colspan="2">
                                                <asp:Label ID="lblskuname" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="   SKU Name" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>
                                            <td style="height: 16px">
                                                <asp:Label ID="Label3" runat="server" BackColor="#006699" CssClass="lblbox" Font-Bold="True"
                                                    ForeColor="White" Height="16px" Text="Qty Ctn" Width="61px"></asp:Label>
                                            </td>
                                            <td style="height: 16px">
                                                <asp:Label ID="Label6" runat="server" BackColor="#006699" CssClass="lblbox" Font-Bold="True"
                                                    ForeColor="White" Height="16px" Text="Qty Units" Width="61px"></asp:Label>
                                            </td>
                                            <td style="height: 16px">
                                                <asp:Label ID="lblquantity" runat="server" Width="62px" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="Unit Price" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>
                                            <td style="height: 16px">
                                                <asp:Label ID="Label7" runat="server" BackColor="#006699" CssClass="lblbox" Font-Bold="True"
                                                    ForeColor="White" Height="16px" Text="Free Sku" Width="62px" Enabled="False"></asp:Label>
                                            </td>

                                            <asp:Panel ID="PanelStdDiscLbl" runat="server">
                                                <td style="height: 16px">
                                                    <asp:Label ID="Label19" runat="server" BackColor="#006699" CssClass="lblbox" Font-Bold="True"
                                                        ForeColor="White" Height="16px" Text="Std. Dsct" Width="90px" Enabled="False"></asp:Label>
                                                </td>
                                            </asp:Panel>


                                            <asp:Panel ID="PanelExtraDsctLbl" runat="server">
                                                <td style="height: 16px">
                                                    <asp:Label ID="Label20" runat="server" BackColor="#006699" CssClass="lblbox" Font-Bold="True"
                                                        ForeColor="White" Height="16px" Text="Extra Dsct" Width="85px" Enabled="False"></asp:Label>
                                                </td>
                                            </asp:Panel>

                                            <td style="height: 16px">
                                                <asp:Label ID="Label2" runat="server" BackColor="#006699" CssClass="lblbox" Font-Bold="True"
                                                    ForeColor="White" Height="16px" Text="Amount" Width="74px"></asp:Label>
                                            </td>
                                            <td style="height: 16px">
                                                <asp:Label ID="Label4" runat="server" BackColor="#006699" CssClass="lblbox" Font-Bold="True"
                                                    ForeColor="White" Height="16px" Text="Add SKU" Width="85px"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:DropDownList ID="DrpSkuDetail" runat="server" Width="234px" TabIndex="5"></asp:DropDownList>
                                            </td>

                                            <td>
                                                <asp:TextBox ID="txtQuantityCtn" runat="server" CssClass="txtBox" Width="59px" Style="text-align: right" TabIndex="6"></asp:TextBox>
                                                <ajaxToolkit:FilteredTextBoxExtender runat="server" TargetControlID="txtQuantityCtn" ValidChars="0123456789"></ajaxToolkit:FilteredTextBoxExtender>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtQuantity" runat="server" CssClass="txtBox" Width="59px" Style="text-align: right" TabIndex="7"></asp:TextBox>
                                                <ajaxToolkit:FilteredTextBoxExtender runat="server" TargetControlID="txtQuantity" ValidChars="0123456789"></ajaxToolkit:FilteredTextBoxExtender>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtUnitRate" runat="server" Width="60px" CssClass="txtBox" Enabled="False" Style="text-align: right" TabIndex="8"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtFreeSku" runat="server" CssClass="txtBox" Width="60px" Style="text-align: right"></asp:TextBox>
                                                <ajaxToolkit:FilteredTextBoxExtender runat="server" TargetControlID="txtFreeSku" ValidChars="0123456789"></ajaxToolkit:FilteredTextBoxExtender>
                                            </td>
                                            <asp:Panel ID="PanelStdDisctTextbox" runat="server">
                                                <td>
                                                    <asp:TextBox ID="txtDistributorDiscount" runat="server" CssClass="txtBox" Width="88px" Style="text-align: right" TabIndex="9"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender runat="server" TargetControlID="txtDistributorDiscount" ValidChars=".0123456789"></ajaxToolkit:FilteredTextBoxExtender>
                                                </td>
                                            </asp:Panel>
                                            <asp:Panel ID="PanelExtraDisctTextbox" runat="server">
                                                <td>
                                                    <asp:TextBox ID="txtExtradiscount" runat="server" CssClass="txtBox" Width="83px" Style="text-align: right" TabIndex="10"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender runat="server" TargetControlID="txtExtradiscount" ValidChars=".0123456789"></ajaxToolkit:FilteredTextBoxExtender>
                                                </td>
                                            </asp:Panel>
                                            <td>
                                                <asp:TextBox ID="txtAmount" runat="server" CssClass="txtBoxnNum" Enabled="False" Width="72px" Style="text-align: right"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnSave" runat="server" Font-Size="8pt" OnClick="btnSave_Click" Text="Add Sku" TabIndex="11"
                                                    ValidationGroup="vg" Width="85px" AccessKey="A" CssClass="Button" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" colspan="10">
                                                <asp:Panel ID="Panel2" runat="server" Height="200px" ScrollBars="Vertical" Width="820px"
                                                    BorderColor="Silver" BorderStyle="Groove" BorderWidth="1px">
                                                    <asp:GridView ID="GrdPurchase" runat="server" ForeColor="SteelBlue" CssClass="gridRow2"
                                                        BackColor="White" HorizontalAlign="Center" AutoGenerateColumns="False" BorderColor="White"
                                                        ShowHeader="False" Width="100%" OnRowDataBound="GrdPurchase_RowDataBound">
                                                        <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                                            PreviousPageText="Previous"></PagerSettings>
                                                        <Columns>
                                                            <asp:BoundField DataField="SKU_ID" HeaderText="SKU_ID">
                                                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SKU_CODE" HeaderText="SKU Code">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SKU_NAME" HeaderText="SKU Name">
                                                                <ItemStyle HorizontalAlign="Left" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid"
                                                                    Width="207px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="QUANTITY_CTN" HeaderText="QuantityCtn">
                                                                <ItemStyle HorizontalAlign="Right" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid"
                                                                    Width="65px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="QUANTITY_UNIT" HeaderText="Quantity">
                                                                <ItemStyle HorizontalAlign="Right" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid"
                                                                    Width="65px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="UNIT_PRICE" HeaderText="PRICE" DataFormatString="{0:F4}">
                                                                <ItemStyle HorizontalAlign="Right" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid"
                                                                    Width="65px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="FREE_SKU" HeaderText="Free Sku">
                                                                <ItemStyle BorderColor="Silver" HorizontalAlign="Right" BorderStyle="Solid" BorderWidth="2px" Width="62px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="STANDARD_DISCOUNT" HeaderText="Std. Discount" DataFormatString="{0:F2}">
                                                                <ItemStyle HorizontalAlign="Right" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid" Width="8%"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="EXTRA_DISCOUNT" HeaderText="Ext. Discount" DataFormatString="{0:F2}">
                                                                <ItemStyle HorizontalAlign="Right" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid" Width="8%"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="AMOUNT" HeaderText="Amount" DataFormatString="{0:F4}">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Right"
                                                                    Width="80px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="QUANTITY_CTN" HeaderText="QUANTITY_CTN">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="BATCH_NO" HeaderText="Batch No">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="UNITS_IN_CASE" HeaderText="UNITS_IN_CASE">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="QUANTITY_CTN2" HeaderText="QUANTITY_CTN2">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="QUANTITY_UNIT2" HeaderText="QUANTITY_UNIT2">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:TemplateField HeaderText="Edit">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="btnEdit" runat="server" CommandArgument='<%#Eval("SKU_ID")%>' OnClick="btnEdit_Click" ToolTip="Edit">
                                                                        <img id="imgEdit" alt="" src="~/images/edit.gif" runat="server" />
                                                                    </asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" CssClass="grdDetail" Width="4%" Height="14px" BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid" />
                                                                <HeaderStyle HorizontalAlign="Center" CssClass="grdHead" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Delete">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="btnDelete" runat="server" CommandArgument='<%#Eval("SKU_ID")%>' OnClick="btnDelete_Click" OnClientClick="javascript:return confirm('Are you sure you want to Delete?');return false;">
                                                                        <img id="imgDelete" alt="" src="~/images/delete.gif" runat="server" />
                                                                    </asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle BorderStyle="Solid" BorderWidth="1px" Width="4%" BorderColor="Silver" HorizontalAlign="Center"></ItemStyle>
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" BackColor="#007395" ForeColor="White" />
                                                        <AlternatingRowStyle BackColor="#F2F2F2" CssClass="GridAlternateRowStyle" ForeColor="#333333" />
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td align="right">Total:</td>
                                            <td>
                                                <asp:TextBox ID="txttotalCtn" runat="server" Width="80px"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txttotalUnit" runat="server" Width="80px"></asp:TextBox></td>
                                            <td></td>
                                            <td>
                                                <asp:TextBox ID="txttotalFreeSku" runat="server" Width="80px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txttotalAmount" runat="server" Width="90px"></asp:TextBox>
                                                <asp:HiddenField ID="HdDistributorPrice" Value="0" runat="server" />
                                            </td>

                                        </tr>

                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:Panel>
                    </td>
                    <td style="width: 100px">
                        <asp:UpdatePanel ID="Panel" runat="server">
                            <ContentTemplate>
                                <asp:HiddenField ID="hfBillBookNo" runat="server" />
                                <asp:HiddenField ID="hfCustomerAreaID" runat="server" />
                                <asp:HiddenField ID="hfCostOfGoods" Value="0" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
        <div>
            <table width="100%">
                <tr>
                    <td align="left">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <table id="SummaryTable" runat="server">
                                    <tbody>
                                        <tr>
                                            <td valign="top" align="left" colspan="2" rowspan="8">
                                                <strong>
                                                    <asp:Label ID="Label10" runat="server" Width="60px" Text="Free SKU" CssClass="lblbox"></asp:Label></strong><asp:Panel
                                                        ID="Panel4" runat="server" Width="350px" Height="90px" BorderColor="Silver" BorderStyle="Groove"
                                                        BorderWidth="1px">
                                                        <asp:GridView ID="GrdFreeSKU" runat="server" Width="100%" ForeColor="Silver" CssClass="gridRow2"
                                                            BorderColor="White" BackColor="White" AutoGenerateColumns="False" HorizontalAlign="Center">
                                                            <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                                                PreviousPageText="Previous"></PagerSettings>
                                                            <RowStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid" ForeColor="Black"></RowStyle>
                                                            <Columns>
                                                                <asp:BoundField DataField="SKU_ID" HeaderText="SKU_ID">
                                                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                                    <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="SKU_Code" HeaderText="SKU Code">
                                                                    <ItemStyle Width="80px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"></ItemStyle>
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="SKU_Name" HeaderText="SKU Name">
                                                                    <ItemStyle Width="200px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"></ItemStyle>
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Quantity" HeaderText="Qty">
                                                                    <ItemStyle Width="50px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center"></ItemStyle>
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="AMOUNT" HeaderText="Amount">
                                                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                                    <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                                </asp:BoundField>
                                                            </Columns>
                                                            <HeaderStyle CssClass="tblhead"></HeaderStyle>
                                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" BackColor="#007395" ForeColor="White" />
                                                            <AlternatingRowStyle BackColor="#F2F2F2" CssClass="GridAlternateRowStyle" ForeColor="#333333" />
                                                        </asp:GridView>
                                                    </asp:Panel>
                                                <asp:HiddenField ID="HFreeSKUAmount" Value="0" runat="server" />
                                            </td>
                                            <td style="height: 20px" align="left"></td>
                                            <td style="height: 20px" align="left">
                                                <strong>
                                                    <asp:Label ID="lblDocumentNo1" runat="server" Width="94px" Text="Gross Sale" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="right">
                                                <asp:TextBox ID="txtGrossAmount" runat="server" Width="154px" ForeColor="Black" Font-Bold="True"
                                                    CssClass="txtBoxnNum" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 18px" align="left"></td>
                                            <td style="height: 18px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label51" runat="server" Width="116px" Text="Standard Discount" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 18px" align="right">
                                                <asp:TextBox ID="txtDisPer" runat="server"
                                                    Width="55px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum"></asp:TextBox>
                                                <asp:TextBox ID="numTxtTotalStndrdDiscnt" ReadOnly="True" runat="server"
                                                    Width="90px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left"></td>
                                            <td align="left">
                                                <strong>
                                                    <asp:Label ID="lblfromLocation1" runat="server" Width="116px" Text="Extra Discount/TO"
                                                        CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px" align="right">
                                                <asp:TextBox ID="txtEXDPer" runat="server"
                                                    Width="55px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum"></asp:TextBox>
                                                <asp:TextBox ID="numtxtTotalExtraDiscnt" ReadOnly="True" runat="server" Width="90px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td><strong>
                                                <asp:Label ID="Label18" runat="server" Width="110px" Text="Discount In" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="center">
                                                <%--AutoPostBack="true" OnSelectedIndexChanged="RbdiscountType_SelectedIndexChanged"--%>
                                                <asp:RadioButtonList ID="RbdiscountType" onclick="RbtDistributorDiscount();" runat="server" Width="100px" RepeatDirection="Horizontal">
                                                    <asp:ListItem Value="0" Selected="True">%</asp:ListItem>
                                                    <asp:ListItem Value="1">Value</asp:ListItem>
                                                </asp:RadioButtonList></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 1px; height: 20px" valign="top"></td>
                                            <td style="width: 1px; height: 20px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label13" runat="server" Width="116px" Text="Distributor Discount" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="right">
                                                <asp:TextBox ID="txtDDPer" runat="server"
                                                    Width="55px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum"></asp:TextBox>
                                                <asp:TextBox ID="numtxtYcDiscount" runat="server" Width="87px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr style="display: none;">
                                            <td style="width: 1px; height: 20px" valign="top"></td>
                                            <td style="width: 1px; height: 20px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label8" runat="server" Width="116px" Text="Claimable  Discount" CssClass="lblbox" Style="display: none;"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="right">
                                                <asp:TextBox ID="numtxtUnClaimabledist" runat="server" Width="150px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum" ReadOnly="True" Style="display: none;"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 1px; height: 20px" valign="top"></td>
                                            <td style="width: 1px; height: 20px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label81" runat="server" Width="93px" Text="GST Amount" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="right">
                                                <asp:TextBox ID="numTxtTotalGST" runat="server" Width="150px" ForeColor="Black" Font-Bold="True"
                                                    CssClass="txtBoxnNum" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 1px; height: 20px" valign="top"></td>
                                            <td style="width: 1px; height: 20px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label9" runat="server" Width="93px" Text="TST Amount" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="right">
                                                <asp:TextBox ID="numTxtTotalTST" runat="server" Width="150px" ForeColor="Black" Font-Bold="True"
                                                    CssClass="txtBoxnNum" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 1px; height: 20px" valign="top"></td>
                                            <td style="width: 1px; height: 20px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label14" runat="server" Width="105px" Text="Additional Tax" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="right">
                                                <asp:TextBox ID="numTxtTotalExtra_tax" runat="server" Width="150px" ForeColor="Black" Font-Bold="True"
                                                    CssClass="txtBoxnNum" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr style="display: none;">
                                            <td style="width: 1px; height: 20px" valign="top"></td>
                                            <td style="width: 1px; height: 20px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label15" runat="server" Width="105px" Text="MR" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="right">
                                                <asp:TextBox ID="numTxtTotlMR" runat="server" Width="50px" ForeColor="Black" Font-Bold="True"
                                                    CssClass="txtBoxnNum" ReadOnly="True"></asp:TextBox>
                                                <asp:TextBox ID="numTxtTotlFMR" runat="server" Width="50px" ForeColor="Black" Font-Bold="True"
                                                    CssClass="txtBoxnNum" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 1px; height: 20px" valign="top"></td>
                                            <td style="width: 1px; height: 20px" align="left"></td>
                                            <td></td>
                                            <td><strong>
                                                <asp:Label ID="Label91" runat="server" Width="105px" Text="Net Amount" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="right">
                                                <asp:TextBox ID="numTxtTotlAmnt" runat="server" Width="150px" ForeColor="Black" Font-Bold="True"
                                                    CssClass="txtBoxnNum" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" colspan="2">
                                                <table>
                                                    <tbody>
                                                        <tr>
                                                            <td style="width: 100px">
                                                                <asp:Button AccessKey="C" ID="btnCalculate" TabIndex="100" OnClick="btnCalculate_Click"
                                                                    runat="server" Width="100px" Font-Size="8pt" Text="Calculate" Enabled="False"
                                                                    CssClass="Button" />
                                                            </td>
                                                            <td style="width: 100px">
                                                                <asp:Button AccessKey="S" ID="btnSaveOrder" TabIndex="101" OnClick="btnSaveOrder_Click"
                                                                    runat="server" Width="110px" Font-Size="8pt" Text="Save Order" Enabled="False"
                                                                    CssClass="Button" />
                                                            </td>
                                                            <td style="width: 100px">
                                                                <asp:Button AccessKey="U" ID="btnUpdateOrder" TabIndex="101" OnClick="btnUpdateOrder_Click"
                                                                    runat="server" Width="110px" Font-Size="8pt" Text="Update Order" Enabled="False"
                                                                    CssClass="Button" />
                                                            </td>
                                                            <td style="width: 100px">
                                                                <asp:Button AccessKey="H" ID="btnCancel" TabIndex="102" runat="server" Width="110px"
                                                                    Font-Size="8pt" Text="Home" OnClick="btnCancel_Click" CssClass="Button" />
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </td>
                                            <td style="width: 1px" valign="top" align="left">&nbsp; &nbsp; &nbsp;&nbsp;
                                            </td>
                                            <td style="width: 1px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label5" runat="server" Width="105px" Text="Cash Received" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px" align="right">
                                                <asp:TextBox ID="txtCashReceived" runat="server" Width="151px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum" Enabled="False"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        &nbsp;&nbsp;
                        <asp:TextBox ID="numTxtTotalSED" runat="server" CssClass="txtBox " Font-Bold="False"
                            ForeColor="Black" Width="139px" ReadOnly="True" Visible="False"></asp:TextBox>&nbsp;&nbsp;
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
