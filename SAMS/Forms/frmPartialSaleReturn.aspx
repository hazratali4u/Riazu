<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmPartialSaleReturn.aspx.cs" Inherits="Forms_frmPartialSaleReturn" Title="SAMS:Partial Sale Return" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script type="text/javascript" src="../AjaxLibrary/jquery.searchabledropdown-1.0.8.min.js"></script>
    <script language="JavaScript" type="text/javascript">

        function pageLoad() {

            $("select").searchable();
        }
    </script>
    <div id="right_data">
        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
            <ContentTemplate>
                <div>
                    <table width="100%">
                        <tr>
                            <td align="left">

                                <table>
                                    <tbody>
                                        <tr>
                                            <td valign="top" align="left">
                                                <strong>
                                                    <asp:Label ID="Label3" runat="server" Width="94px" Text="Location" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td valign="top" align="left" colspan="2">
                                                <asp:DropDownList ID="DrpDistributor" runat="server" Width="202px" CssClass="DropList">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:HiddenField ID="hdnSale_invoice_ID" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnCustomer_ID" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnTown_Id" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnArea_ID" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnPrincipal_Id" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnOrderBooker_ID" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnDeliveryMan_Id" runat="server" Value="0" />

                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" align="left">
                                                <strong>
                                                    <asp:Label ID="lblDocumentNo" runat="server" Width="74px" Text="Invoice No" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td valign="top" align="left" colspan="2">
                                                <asp:TextBox ID="txtInvoiceNo" runat="server" Width="198px" CssClass="txtBox"></asp:TextBox>


                                            </td>
                                            <td>
                                                <asp:Button ID="btnSearch" runat="server" CssClass="Button" OnClick="btnSearch_Click" Text="Search" />

                                            </td>
                                        </tr>
                                        <tr style="display: none;">
                                            <td valign="top" align="left">
                                                <strong>
                                                    <asp:Label ID="Label2" runat="server" Width="94px" Text="Sale Return No" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td valign="top" align="left" colspan="2">
                                                <asp:DropDownList ID="DrpSaleReturn" runat="server" Width="202px" CssClass="DropList">
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr style="display: none;">
                                            <td valign="middle" align="left">
                                                <strong>
                                                    <asp:Label ID="Label11" runat="server" Width="77px" Text="Doc. Type" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td valign="top" align="left" colspan="2">
                                                <asp:RadioButtonList ID="RblPayMode" runat="server" Width="200px" Height="1px" RepeatDirection="Horizontal">
                                                    <asp:ListItem Selected="True" Value="214">Cash</asp:ListItem>
                                                    <asp:ListItem Value="215">Credit</asp:ListItem>
                                                    <asp:ListItem Value="216">Advance</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                            <td valign="top" align="left" colspan="1"></td>
                                        </tr>
                                        <tr>
                                            <td valign="middle" align="left">
                                                <strong>
                                                    <asp:Label ID="Label12" runat="server" Width="101px" Text="Section/Saleman" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td valign="top" align="left" colspan="2">
                                                <asp:TextBox ID="txtArea" runat="server" Width="198px" CssClass="txtBox" ReadOnly="True"></asp:TextBox>
                                            </td>
                                            <td valign="top" align="left" colspan="1">
                                                <asp:TextBox ID="txtDeliveryMan" runat="server" Width="200px" CssClass="txtBox"
                                                    ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 23px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label1" runat="server" Width="74px" Text="Outlet Name"></asp:Label></strong>
                                            </td>

                                            <td style="height: 23px" align="left" colspan="2">
                                                <asp:TextBox ID="txtOutletName" runat="server" Width="198px"
                                                    CssClass="txtBox "></asp:TextBox>
                                            </td>
                                            <td style="height: 23px" align="left">
                                                <asp:TextBox ID="txtAddress" runat="server" CssClass="txtBox" Width="200px"></asp:TextBox>
                                            </td>
                                        </tr>


                                    </tbody>
                                </table>

                            </td>
                            <td style="height: 126px"></td>
                        </tr>
                        <tr>
                            <td align="left">
                                <asp:Panel ID="Panel5" runat="server">

                                    <table>

                                        <tr>
                                            <td align="left" colspan="8">
                                                <asp:Panel ID="Panel2" runat="server" Height="130px" ScrollBars="Vertical" Width="800px"
                                                    BorderColor="Silver" BorderStyle="Groove" BorderWidth="1px" CssClass="pnlCheckboxlist">
                                                    <asp:GridView ID="GrdInvoice" runat="server" ForeColor="SteelBlue" CssClass="gridRow2"
                                                        BackColor="White" HorizontalAlign="Center" AutoGenerateColumns="False" BorderColor="White"
                                                        ShowHeader="true" Width="100%">
                                                        <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                                            PreviousPageText="Previous"></PagerSettings>
                                                        <AlternatingRowStyle BackColor="#E0E0E0" />
                                                        <Columns>
                                                            <asp:BoundField DataField="SKU_ID" HeaderText="SKU_ID">
                                                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SKU_CODE" HeaderText="SKU Code">
                                                                <ItemStyle HorizontalAlign="Left" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid"
                                                                    Width="70px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SKU_NAME" HeaderText="SKU Name">
                                                                <ItemStyle HorizontalAlign="Left" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid"
                                                                    Width="248px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="QUANTITY_CTN" HeaderText="Invoice Ctn">
                                                                <ItemStyle HorizontalAlign="Right" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid"
                                                                    Width="61px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="QUANTITY_UNIT" HeaderText="Invoice Unit">
                                                                <ItemStyle HorizontalAlign="Right" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid"
                                                                    Width="63px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="UNIT_PRICE" HeaderText="PRICE" DataFormatString="{0:F4}">
                                                                <ItemStyle HorizontalAlign="Right" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid"
                                                                    Width="64px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:F4}">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Right"
                                                                    Width="75px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="PREV_RETURN_CTN" HeaderText="Prev R.Ctn">
                                                                <ItemStyle HorizontalAlign="Right" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid"
                                                                    Width="61px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="PREV_RETURN_UNIT" HeaderText="Prev R.Unit">
                                                                <ItemStyle HorizontalAlign="Right" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid"
                                                                    Width="63px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:TemplateField HeaderText="Return Ctn">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtreturnCtn" runat="server" Text='<%# Eval("RETURN_CTN","{0:f0}") %>' Style="width: 100%;"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="grdDetail" HorizontalAlign="Right" BorderWidth="2px" BorderStyle="Solid" Width="9%" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Return Unit">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtreturnUnit" runat="server" Text='<%# Eval("RETURN_UNIT","{0:f0}") %>' Style="width: 100%;"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="grdDetail" HorizontalAlign="Right" BorderStyle="Solid" BorderWidth="2px" Width="9%" />
                                                            </asp:TemplateField>

                                                            <asp:BoundField DataField="EXTRA_DISCOUNT" HeaderText="Ext. Discount" DataFormatString="{0:F2}">
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

                                                            <asp:BoundField DataField="UNITS_IN_CASE" HeaderText="UNITS_IN_CASE">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>


                                                        </Columns>
                                                        <HeaderStyle CssClass="tblhead" />
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>

                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </div>
                <div>
                    <table width="100%">
                        <tr>
                            <td align="left">

                                <table>
                                    <tbody>
                                        <tr>
                                            <td valign="top" align="left" colspan="2" rowspan="7">
                                                <strong>
                                                    <asp:Label ID="Label10" runat="server" Width="60px" Text="Free SKU"></asp:Label></strong>
                                                <asp:Panel ID="Panel4" runat="server" Width="450px" Height="90px" BorderColor="Silver"
                                                    BorderStyle="Groove" BorderWidth="1px" ScrollBars="Vertical" CssClass="pnlCheckboxlist">
                                                    <asp:GridView ID="GrdFreeSKU" runat="server" Width="100%" ForeColor="Silver" CssClass="gridRow2"
                                                        BorderColor="White" BackColor="White" AutoGenerateColumns="False" HorizontalAlign="Center">
                                                        <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                                            PreviousPageText="Previous"></PagerSettings>
                                                        <RowStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid" ForeColor="Black"></RowStyle>
                                                        <AlternatingRowStyle BackColor="#E0E0E0" />
                                                        <Columns>
                                                            <asp:BoundField DataField="SKU_ID" HeaderText="SKU_ID">
                                                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SKU_Code" HeaderText="SKU Code">
                                                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="UNIT_PRICE" HeaderText="UNIT_PRICE">
                                                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SKU_Name" HeaderText="SKU Name">
                                                                <ItemStyle Width="100px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Quantity_unit" HeaderText="Unit">
                                                                <ItemStyle Width="50px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="PREV_RETURN_UNIT" HeaderText="Prev R.Unit">
                                                                <ItemStyle Width="50px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:TemplateField HeaderText="R.Unit">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtFreeQty" runat="server" Text='<%# Eval("Return_unit","{0:f0}") %>' Style="width: 50px;"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="grdDetail" HorizontalAlign="Right" BorderStyle="Solid" BorderWidth="2px" Width="9%" />
                                                            </asp:TemplateField>

                                                        </Columns>
                                                        <HeaderStyle CssClass="tblhead"></HeaderStyle>
                                                    </asp:GridView>
                                                </asp:Panel>
                                                <br />
                                                <strong>
                                                    <asp:Label ID="lblRemarks" runat="server" Text="Remarks"></asp:Label>
                                                </strong>
                                                <asp:TextBox ID="txtRemarks" runat="server" Width="98%" Height="30px" TextMode="MultiLine" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                            <td style="height: 20px; width: 13px;" align="left"></td>
                                            <td style="height: 20px; width: 10px;" align="left">
                                                <strong>
                                                    <asp:Label ID="lblDocumentNo1" runat="server" Width="94px" Text="Gross Sale" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px">
                                                <asp:TextBox ID="txtGrossAmount" runat="server" Width="150px" ForeColor="Black" Font-Bold="True"
                                                    CssClass="txtBox" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 18px; width: 13px;" align="left"></td>
                                            <td style="height: 18px; width: 10px;" align="left">
                                                <strong>
                                                    <asp:Label ID="Label51" runat="server" Width="138px" Text="Distributor Discount" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 18px">
                                                <asp:TextBox ID="numtxtDisDiscount" Style="text-align: left" runat="server" Width="150px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBox" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 13px"></td>
                                            <td align="left" style="width: 10px" align="left">
                                                <strong>
                                                    <asp:Label ID="lblfromLocation1" runat="server" Width="144px" Text="Company Discount"
                                                        CssClass="lblbox" Height="16px"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px" align="left">
                                                <asp:TextBox ID="numtxtCompanyDiscount" runat="server" Width="150px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBox" Style="text-align: left" ReadOnly="True"></asp:TextBox>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 13px; height: 20px" valign="top"></td>
                                            <td style="width: 10px; height: 20px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label8" runat="server" Width="116px" Text="Trade Offer" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="left">
                                                <asp:TextBox ID="numtxtTradeOffer" runat="server" Width="150px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBox" Style="text-align: left" ReadOnly="True"></asp:TextBox>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 13px; height: 20px" valign="top"></td>
                                            <td style="width: 10px; height: 20px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label81" runat="server" Width="144px" Text="GST Normal Goods" CssClass="lblbox"
                                                        Height="16px"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="left">
                                                <asp:TextBox ID="numTxtTotalGST" runat="server" Width="150px" ForeColor="Black" Font-Bold="True"
                                                    CssClass="txtBox" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 13px; height: 20px" valign="top"></td>
                                            <td style="width: 10px; height: 20px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label9" runat="server" Width="142px" Text="GST 3rd Schedule" CssClass="lblbox"
                                                        Height="16px"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="left">
                                                <asp:TextBox ID="numTxtTotalTST" runat="server" Width="150px" ForeColor="Black" Font-Bold="True"
                                                    CssClass="txtBox" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>


                                            <td></td>
                                            <td>
                                                <strong>
                                                    <asp:Label ID="Label7" runat="server" Width="136px" Text="Less Addi. Discount"
                                                        CssClass="lblbox" Height="16px"></asp:Label></strong></td>
                                            <td align="left">
                                                <asp:TextBox ID="numtxtTotalExtraDiscnt" runat="server" CssClass="txtBox"
                                                    Width="150px" ForeColor="Black" AutoPostBack="true"
                                                    OnTextChanged="numtxtTotalExtraDiscnt_TextChanged">

                                                </asp:TextBox>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td></td>
                                            <td style="width: 13px; height: 20px" valign="top"></td>
                                            <td style="width: 10px; height: 20px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label91" runat="server" Width="105px" Text="Net Amount" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="right">
                                                <asp:TextBox ID="numTxtTotlAmnt" runat="server" Width="150px" ForeColor="Black" Font-Bold="True"
                                                    CssClass="txtBox" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" colspan="2">
                                                <table>
                                                    <tbody>
                                                        <tr>
                                                            <td style="width: 90px">
                                                                <asp:Button AccessKey="C" ID="btnCalculate" TabIndex="100" OnClick="btnCalculate_Click"
                                                                    runat="server" Width="90px" Font-Size="8pt" Text="Calculate"
                                                                    CssClass="Button" />
                                                            </td>
                                                            <td style="width: 90px">
                                                                <asp:Button AccessKey="S" ID="btnSaveOrder" TabIndex="101" OnClick="btnSaveOrder_Click"
                                                                    runat="server" Width="90px" Font-Size="8pt" Text="Save Return" Enabled="False"
                                                                    CssClass="Button" />
                                                            </td>
                                                            <td>
                                                                <asp:Button AccessKey="H" ID="btnCancel" TabIndex="103" runat="server" Width="90px"
                                                                    Font-Size="8pt" Text="Home" OnClick="btnCancel_Click" CssClass="Button" />
                                                            </td>
                                                            <td></td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </td>
                                            <td style="width: 13px" valign="top" align="left">&nbsp; &nbsp; &nbsp;&nbsp;
                                            </td>
                                            <td style="width: 10px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label5" runat="server" Width="105px" Text="Cash Received" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px" align="right">
                                                <asp:TextBox ID="txtCashReceived" runat="server" Width="150px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBox" Enabled="False"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td style="width: 10px"></td>
                                            <td>
                                                <asp:TextBox ID="txtExtra_tax" runat="server" Width="150px" ForeColor="Black" Font-Bold="True"
                                                    CssClass="txtBox" Visible="False"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>

                                &nbsp;&nbsp;
                        <asp:TextBox ID="numTxtTotalSED" runat="server" CssClass="txtBox " Font-Bold="False"
                            ForeColor="Black" Width="10px" ReadOnly="True" Visible="False"></asp:TextBox>
                                <asp:TextBox ID="numtxtTotalStandardDis_By_value" runat="server" CssClass="txtBox "
                                    Font-Bold="False" ForeColor="Black" Width="10px" ReadOnly="True" Visible="False"></asp:TextBox>
                                <asp:TextBox ID="numtxtTotalStandardDis_By_value_TO" runat="server" CssClass="txtBox "
                                    Font-Bold="False" ForeColor="Black" Width="10px" ReadOnly="True" Visible="False"></asp:TextBox>
                                <asp:TextBox ID="numtxtTotalStandardDis_By_value_AD" runat="server" CssClass="txtBox "
                                    Font-Bold="False" ForeColor="Black" Width="10px" ReadOnly="True" Visible="False"></asp:TextBox>
                                <asp:TextBox ID="numtxtTotalStandardDis_PER_value" runat="server" CssClass="txtBox "
                                    Font-Bold="False" ForeColor="Black" Width="10px" ReadOnly="True" Visible="False"></asp:TextBox>
                                <asp:TextBox ID="numtxtTotalStandardDis_PER_value_TO" runat="server" CssClass="txtBox "
                                    Font-Bold="False" ForeColor="Black" Width="10px" ReadOnly="True" Visible="False"></asp:TextBox>
                                <asp:TextBox ID="numtxtTotalStandardDis_PER_value_AD" runat="server" CssClass="txtBox "
                                    Font-Bold="False" ForeColor="Black" Width="10px" ReadOnly="True" Visible="False"></asp:TextBox>
                                <asp:TextBox ID="numtxtTotalSED_BY_value" runat="server" CssClass="txtBox " Font-Bold="False"
                                    ForeColor="Black" Width="10px" ReadOnly="True" Visible="False"></asp:TextBox>
                                <asp:TextBox ID="numtxtTotalSED_BY_value_TO" runat="server" CssClass="txtBox " Font-Bold="False"
                                    ForeColor="Black" Width="10px" ReadOnly="True" Visible="False"></asp:TextBox>
                                <asp:TextBox ID="numtxtTotalSED_BY_value_AD" runat="server" CssClass="txtBox " Font-Bold="False"
                                    ForeColor="Black" Width="10px" ReadOnly="True" Visible="False"></asp:TextBox>
                                <asp:TextBox ID="numtxtTotalSED_PER_value" runat="server" CssClass="txtBox " Font-Bold="False"
                                    ForeColor="Black" Width="10px" ReadOnly="True" Visible="False"></asp:TextBox>
                                <asp:TextBox ID="numtxtTotalSED_PER_value_TO" runat="server" CssClass="txtBox " Font-Bold="False"
                                    ForeColor="Black" Width="10px" ReadOnly="True" Visible="False"></asp:TextBox>
                                <asp:TextBox ID="numtxtTotalSED_PER_value_AD" runat="server" CssClass="txtBox " Font-Bold="False"
                                    ForeColor="Black" Width="10px" ReadOnly="True" Visible="False"></asp:TextBox>

                                <asp:TextBox ID="numTxtTotalStndrdDiscnt" runat="server" Width="10px" ForeColor="Black"
                                    Font-Bold="True" CssClass="txtBoxnNum" ReadOnly="True" Visible="false"></asp:TextBox>

                                <asp:TextBox ID="numtxtUnClaimabledist" runat="server" Width="10px" ForeColor="Black"
                                    Font-Bold="True" CssClass="txtBoxnNum" ReadOnly="True" Visible="false"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
