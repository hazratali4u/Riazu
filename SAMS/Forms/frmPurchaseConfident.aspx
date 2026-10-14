
<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmPurchaseConfident.aspx.cs" Inherits="Forms_frmPurchaseConfident" Title="SAMS: Stock Register" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script type="text/javascript" src="../AjaxLibrary/jquery.searchabledropdown-1.0.8.min.js"></script>
    <script language="JavaScript" type="text/javascript">

        function ValidateForm() {
            var str;
            var str2;

            str = document.getElementById('<%=txtQuantity.ClientID%>').value;
            str2 = document.getElementById('<%=txtUnit.ClientID%>').value;
            if ((str == null || str.length == 0) && (str2 == null || str2.length == 0)) {
                alert('Must Enter Quantity');
                document.getElementById('<%=txtQuantity.ClientID%>').focus();
                return false;
            }

            str = document.getElementById('<%=txtDocumentNo.ClientID%>').value;
            if (str == null || str.length == 0) {
                alert('Must Enter Invoice/DC No');
                document.getElementById('<%=txtDocumentNo.ClientID%>').focus();
                return false;
            }

            str = document.getElementById('<%=txtBuiltyNo.ClientID%>').value;
            if (str == null || str.length == 0) {
                alert('Must Enter Builty No');
                document.getElementById('<%=txtBuiltyNo.ClientID%>').focus();
                return false;
            }

            return true;
        }

        function pageLoad() {

            $("select").searchable();
        }
    </script>
    <div id="right_data">
        <div>
            <table width="100%">
                <tr>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tbody>
                                        <tr>
                                            <td style="height: 24px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label2" runat="server" CssClass="lblbox" Height="14px" Text="Transaction Type"
                                                        Width="128px"></asp:Label></strong>
                                            </td>
                                            <td style="height: 24px">
                                                <asp:DropDownList ID="DrpDocumentType" runat="server" AutoPostBack="True" CssClass="DropList"
                                                    OnSelectedIndexChanged="DrpDocumentType_SelectedIndexChanged" Width="200px">
                                                    <asp:ListItem Value="2">Purchase</asp:ListItem>
                                                    <%-- <asp:ListItem Value="5">Transfer Out</asp:ListItem>
                                                    <asp:ListItem Value="3">Purchase Return</asp:ListItem>
                                                    <asp:ListItem Value="4">Transfer In</asp:ListItem>
                                                   <asp:ListItem Value="6">Damage(Replace)</asp:ListItem>
                                                    <asp:ListItem Value="10">Damage</asp:ListItem>--%>
                                                </asp:DropDownList>
                                            </td>
                                            <td style="height: 24px">
                                                <strong>
                                                    <asp:Label ID="Label5" runat="server" Width="5px"></asp:Label></strong>
                                            </td>
                                            <td style="width: 316px;" align="center" colspan="1" rowspan="8" valign="middle"></td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="height: 25px">
                                                <strong>
                                                    <asp:Label ID="lblDocumentNo" runat="server" Text="Document No" Width="94px"></asp:Label></strong>
                                            </td>
                                            <td style="height: 25px">
                                                <asp:DropDownList ID="drpDocumentNo" runat="server" AutoPostBack="True" CssClass="DropList"
                                                    OnSelectedIndexChanged="drpDocumentNo_SelectedIndexChanged" Width="200px">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="height: 25px"></td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="height: 25px">
                                                <strong>
                                                    <asp:Label ID="lbltoLocation" runat="server" CssClass="lblbox" Text="Principal" Width="94px"></asp:Label></strong>
                                            </td>
                                            <td style="height: 25px">
                                                <asp:DropDownList ID="drpPrincipal" runat="server" AutoPostBack="True" CssClass="DropList"
                                                    OnSelectedIndexChanged="drpPrincipal_SelectedIndexChanged" Width="200px">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="height: 25px"></td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="height: 25px">
                                                <strong>
                                                    <asp:Label ID="lblfromLocation" runat="server" CssClass="lblbox" Text="Purchase For"
                                                        Width="94px"></asp:Label></strong>
                                            </td>
                                            <td style="height: 25px">
                                                <asp:DropDownList ID="drpDistributor" runat="server" AutoPostBack="True" CssClass="DropList"
                                                    Width="200px">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="height: 25px"></td>
                                        </tr>
                                        <tr style="display: none;">
                                            <td>
                                                <strong>
                                                    <asp:Label ID="Label4" runat="server" CssClass="lblbox" Text="Transfer To" Visible="False"
                                                        Width="124px" Height="16px"></asp:Label></strong>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="DrpTransferFor" runat="server" CssClass="DropList" Visible="False"
                                                    Width="200px">
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style="height: 25px;" align="left">
                                                <strong>
                                                    <asp:Label ID="Label1" runat="server" CssClass="lblbox" Text="Invoice #" Width="94px"></asp:Label></strong>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDocumentNo" runat="server" CssClass="txtBox" Width="195px"></asp:TextBox>
                                            </td>
                                            <td style="width: 1px" valign="top"></td>
                                        </tr>
                                        <tr>
                                            <td style="height: 25px;" align="left">
                                                <strong>
                                                    <asp:Label ID="Label3" runat="server" CssClass="lblbox" Text="Dispatch Note #" Width="94px"></asp:Label></strong>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtBuiltyNo" runat="server" CssClass="txtBox" Width="195px"></asp:TextBox>
                                            </td>
                                            <td style="width: 1px" valign="top"></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="ChbBatchNo" runat="server" AutoPostBack="True" OnCheckedChanged="ChbBatchNo_CheckedChanged"
                                                    Text="Batch No" Width="77px" Visible="false" />
                                                <asp:CheckBox ID="chbCalculateGST" runat="server"
                                                    Text="Calculate GST" Width="100px" Visible="true" Checked="false" />
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="ChbFreeSKU" runat="server" Width="121px" Text="Apply Free SKU"
                                                    AutoPostBack="True" OnCheckedChanged="ChbFreeSKU_CheckedChanged"></asp:CheckBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
        <div>
            <table width="100%">
                <tr>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tbody>

                                        <tr>
                                            <td colspan="2">

                                                <asp:Label ID="lblskuname" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="   SKU Name" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblquantity" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="Qty CTN" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblUnits" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="Qty Unit" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label8" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="Purchase price" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label7" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="Gross" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label6" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="Free Sku" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>

                                            <td>
                                                <asp:Label ID="Label11" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="Net Amount" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>
                                            <td style="width: 100px">
                                                <asp:Label ID="Label41" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="Add SKU" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:DropDownList ID="ddlSKuCde" runat="server" Width="316px" OnSelectedIndexChanged="ddlSKuCde_SelectedIndexChanged" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtQuantity" onfocus="SearchedCode()" runat="server" Width="60px"
                                                    CssClass="txtBox "></asp:TextBox>
                                            </td>

                                            <td>
                                                <asp:TextBox ID="txtUnit" runat="server" CssClass="txtBox" Width="60px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPurchasePrice" runat="server" CssClass="txtBox" Style="text-align: right" Width="85px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtskuGross" Enabled="false" runat="server" CssClass="txtBox" Width="60px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtFreeSKU" Enabled="false" runat="server" CssClass="txtBox" Width="60px"></asp:TextBox>
                                            </td>

                                            <td>
                                                <asp:TextBox ID="txtskuAmount" runat="server" CssClass="txtBox" Width="70px" Enabled="false"></asp:TextBox>
                                            </td>
                                            <td style="width: 100px">
                                                <asp:Button AccessKey="A" ID="btnSave" OnClick="btnSave_Click" runat="server" Width="100px"
                                                    Font-Size="8pt" Text="Add Sku" ValidationGroup="vg" CssClass="Button" />
                                            </td>

                                        </tr>
                                        <tr>
                                            <td colspan="9">
                                                <asp:TextBox ID="txtBatchNo" runat="server" Width="80px" CssClass="txtBox" Enabled="False"
                                                    Visible="False">N/A</asp:TextBox>
                                                <asp:TextBox ID="txtTransitDamage" runat="server" CssClass="txtBox" Visible="false"></asp:TextBox>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td align="left" colspan="10">
                                                <asp:Panel ID="Panel2" runat="server" Width="100%" Height="130px" ScrollBars="Vertical"
                                                    BorderWidth="1px" BorderStyle="Groove" BorderColor="Silver">
                                                    <asp:GridView ID="GrdPurchase" runat="server" AutoGenerateColumns="False" BackColor="White"
                                                        BorderColor="White" CssClass="gridRow2" ForeColor="SteelBlue" HorizontalAlign="Center"
                                                        OnRowDeleting="GrdPurchase_RowDeleting" OnRowEditing="GrdPurchase_RowEditing"
                                                        ShowHeader="False" Width="100%">
                                                        <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                                            PreviousPageText="Previous" />
                                                        <RowStyle ForeColor="Black" />
                                                        <Columns>
                                                            <asp:BoundField DataField="SKU_ID" HeaderText="SKU_ID">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SKU_CODE" HeaderText="SKU Code">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Left"
                                                                    Width="85px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SKU_NAME" HeaderText="SKU Name">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Left"
                                                                    Width="215px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Quantity" HeaderText="Quantity">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Right"
                                                                    Width="60px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="QuantityUnit" HeaderText="Quantity">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Right"
                                                                    Width="60px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="PRICE" HeaderText="PRICE" DataFormatString="{0:f2}">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Right"
                                                                    Width="70px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Gross_AMOUNT" HeaderText="AMOUNT" DataFormatString="{0:f2}">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Right"
                                                                    Width="60px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="FREE_SKU" HeaderText="Free SKU">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Right"
                                                                    Width="60px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="TDAMAGE" HeaderText="T.DAMAGE">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="AMOUNT" HeaderText="AMOUNT" DataFormatString="{0:f2}">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Right"
                                                                    Width="70px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="UNITS_IN_CASE" HeaderText="UNITS_IN_CASE">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SKU_PRICE" HeaderText="SKU_PRICE">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:TemplateField HeaderText="Edit">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" ToolTip="Edit">
                                                                        <img id="imgEdit" alt="" src="~/images/edit.gif" runat="server" />
                                                                    </asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" CssClass="grdDetail" Width="3%" Height="14px" BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid" />
                                                                <HeaderStyle HorizontalAlign="Center" CssClass="grdHead" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Delete">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" OnClientClick="javascript:return confirm('Are you sure you want to Delete?');return false;">
                                                                        <img id="imgDelete" alt="" src="~/images/delete.gif" runat="server" />
                                                                    </asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle BorderStyle="Solid" BorderWidth="1px" Width="3%" BorderColor="Silver" HorizontalAlign="Center"></ItemStyle>
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <FooterStyle BackColor="White" />
                                                        <PagerStyle BackColor="Transparent" />
                                                        <HeaderStyle BackColor="#007395" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                                                            VerticalAlign="Middle" />
                                                        <AlternatingRowStyle BackColor="#F2F2F2" CssClass="GridAlternateRowStyle" ForeColor="#333333" />
                                                    </asp:GridView>
                                                    <asp:HiddenField ID="HdSKU_Price" runat="server" />
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                                <table>
                                    <tr>
                                        <td align="right">
                                            <strong>
                                                <asp:Label ID="lblTotalCtn" runat="server" Width="317px" Height="16px" Text="Total :"></asp:Label></strong>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtTotalQuantityCtn" runat="server" Width="60px" CssClass="txtBox"
                                                ReadOnly="True"></asp:TextBox>
                                        </td>

                                        <td>
                                            <asp:TextBox ID="txtTotalQuantity" runat="server" Width="60px"
                                                CssClass="txtBox" ReadOnly="True"></asp:TextBox>
                                        </td>

                                        <td>
                                            <asp:TextBox ID="txtTotalTDamage" runat="server" Width="60px" CssClass="txtBox" ReadOnly="True" Visible="false"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtGross" runat="server" Width="60px" CssClass="txtBox" ReadOnly="True"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtTotalFree" runat="server" Width="60px" CssClass="txtBox" ReadOnly="True"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtNetAmount" runat="server" Width="60px" CssClass="txtBox" ReadOnly="True"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4"></td>
                                        <td>Discount Type</td>
                                        <td colspan="2">
                                            <asp:RadioButtonList AutoPostBack="true" ID="rbDiscount"
                                                RepeatDirection="Horizontal" runat="server" OnSelectedIndexChanged="rbDiscount_SelectedIndexChanged">
                                                <asp:ListItem Value="0" Selected="True" Text="%"></asp:ListItem>
                                                <asp:ListItem Value="1" Text="Value"></asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4"></td>
                                        <td>Discount</td>
                                        <td colspan="2">
                                            <asp:TextBox ID="txtDisPer" AutoPostBack="true" runat="server" Width="50px" OnTextChanged="txtDisPer_TextChanged"></asp:TextBox>
                                            <asp:TextBox ID="txtDiscount" runat="server" Width="95px" Enabled="false" OnTextChanged="txtDiscount_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4"></td>
                                        <td>GST</td>
                                        <td colspan="2">

                                            <asp:TextBox ID="txtNumTotalGST" runat="server" Width="150px"></asp:TextBox>
                                        </td>
                                        <tr>
                                            <td colspan="4"></td>
                                            <td>Net Amount</td>
                                            <td colspan="2">

                                                <asp:TextBox ID="txtnumTotalnetAmount" runat="server" Width="150px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">

                                                <asp:Button AccessKey="C" ID="btnCalculate" runat="server" Width="119px" Font-Size="8pt" Enabled="false"
                                                    Text="Calculate" UseSubmitBehavior="False" OnClick="btnCalculate_Click"
                                                    CssClass="Button" />
                                                <asp:Button AccessKey="S" ID="btnSaveDocument" runat="server" Width="119px" Font-Size="8pt" Enabled="false"
                                                    Text="Save Document" UseSubmitBehavior="False" OnClick="btnSaveDocument_Click"
                                                    CssClass="Button" />


                                                <asp:Button AccessKey="C" ID="btnCancel" runat="server" Width="120px" Font-Size="8pt"
                                                    Text="Cancel" UseSubmitBehavior="False" OnClick="btnCancel_Click" CssClass="Button" />
                                            </td>
                                            <td colspan="5"></td>
                                        </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>

