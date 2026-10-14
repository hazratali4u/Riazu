<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmPurchaseEntry.aspx.cs" Inherits="Forms_frmPurchaseEntry" Title="SAMS: Stock Register" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script type="text/javascript" src="../AjaxLibrary/jquery.searchabledropdown-1.0.8.min.js"></script>
    <script language="JavaScript" type="text/javascript">

        function ValidateForm() {
            var str;
            var strunit;

            str = document.getElementById('<%=txtQuantity.ClientID%>').value;
            strunit = document.getElementById('<%=txtUnit.ClientID%>').value;
            if ((str == null || str.length == 0 || str < 0) && (strunit == null || strunit.length == 0 || strunit < 0)) {
                alert('Must Enter Quantity');
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
        function isNumberKey(evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
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
                                                        Width="110px"></asp:Label></strong>
                                            </td>
                                            <td style="height: 24px">
                                                <asp:DropDownList ID="DrpDocumentType" runat="server" AutoPostBack="True" CssClass="DropList"
                                                    OnSelectedIndexChanged="DrpDocumentType_SelectedIndexChanged" Width="200px">
                                                    <%--   <asp:ListItem Value="2">Purchase</asp:ListItem>--%>
                                                    <asp:ListItem Value="5">Transfer Out</asp:ListItem>
                                                    <asp:ListItem Value="3">Purchase Return</asp:ListItem>
                                                    <asp:ListItem Value="4">Transfer In</asp:ListItem>
                                                    <%-- <asp:ListItem Value="6">Damage(Replace)</asp:ListItem>--%>
                                                    <asp:ListItem Value="10">Damage</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                               <td width="20px"></td>
                                            <td style="height: 24px">
                                                <strong>
                                                    <asp:Label ID="Label5" runat="server" Width="5px"></asp:Label></strong>
                                            </td>
                                            <td></td>
                                          
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
                                              <td width="20px"></td>
                                            <td align="left" style="height: 25px">
                                                <strong>
                                                    <asp:Label ID="lbltoLocation" runat="server" CssClass="lblbox" Text="Principal" Width="94px"></asp:Label></strong>
                                            </td>
                                            <td style="height: 25px">
                                                <asp:DropDownList ID="drpPrincipal" runat="server" AutoPostBack="True" CssClass="DropList"
                                                    OnSelectedIndexChanged="drpPrincipal_SelectedIndexChanged" Width="200px">
                                                </asp:DropDownList>
                                            </td>
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
                                              <td width="20px"></td>
                                           <td style="height: 25px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label4" runat="server" CssClass="lblbox" Text="Transfer To" Visible="False"
                                                        Width="110px" Height="16px"></asp:Label></strong>
                                            </td>
                                            <td style="height: 25px">
                                                <asp:DropDownList ID="DrpTransferFor" runat="server" CssClass="DropList" Visible="False"
                                                    Width="200px">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                       
                                        <tr>
                                            <td style="height: 25px;" align="left">
                                                <strong>
                                                    <asp:Label ID="Label1" runat="server" CssClass="lblbox" Text="Invoice #" Width="94px"></asp:Label></strong>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDocumentNo" runat="server" CssClass="txtBox" Width="195px"></asp:TextBox>
                                            </td>
                                              <td width="20px"></td>
                                             <td style="height: 25px;" align="left">
                                                <strong>
                                                    <asp:Label ID="Label3" runat="server" CssClass="lblbox" Text="Dispatch Note #" Width="94px"></asp:Label></strong>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtBuiltyNo" runat="server" CssClass="txtBox" Width="195px"></asp:TextBox>
                                            </td>
                                        </tr>
                                       
                                        <tr style="display:none;">
                                            <td style="height: 25px" valign="top" align="left">
                                                <asp:CheckBox ID="ChbBatchNo" runat="server" AutoPostBack="True" OnCheckedChanged="ChbBatchNo_CheckedChanged"
                                                    Text="Batch No" Width="77px" Visible="false" />
                                            </td>
                                            <td style="height: 25px; width: 1px;" valign="top">
                                                <asp:CheckBox ID="ChbFreeSKU" runat="server" Width="121px" Text="Apply Free SKU"
                                                    AutoPostBack="True" OnCheckedChanged="ChbFreeSKU_CheckedChanged" Visible="false"></asp:CheckBox>
                                            </td>
                                            <td style="width: 1px; height: 25px" valign="top"></td>
                                            <td></td><td></td>
                                        </tr>
                                             <tr>
                                            <td style="height: 25px" valign="top" align="left">
                                                 <strong>
                                                    <asp:Label ID="Label6" runat="server" CssClass="lblbox" Text="Remarks" Width="94px"></asp:Label></strong>
                                            
                                            </td>
                                            <td colspan="4">
                                               <asp:TextBox ID="txtRemarks" runat="server" Width="540px" Height="30px" TextMode="MultiLine" ></asp:TextBox></td>
                                            <td style="width: 1px; height: 25px" valign="top"></td>
                                          
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
                                                    Font-Bold="True" Text="Quantity CTN" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblUnits" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="Quantity Unit" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>
                                            <asp:Panel ID="lblPriceLable" runat="server">
                                            <td>
                                                <asp:Label ID="Label9" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="Price" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>
                                                </asp:Panel>
                                            <td style="width: 100px">
                                                <asp:Label ID="Label41" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="Add SKU" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:DropDownList ID="ddlSKuCde" runat="server" Width="346px" OnSelectedIndexChanged="ddlSKuCde_SelectedIndexChanged" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtQuantity" onfocus="SearchedCode()" onkeypress="return isNumberKey(event)" runat="server" Width="75px" Style="text-align:right;"
                                                    CssClass="txtBox "></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtUnit" runat="server" onkeypress="return isNumberKey(event)" CssClass="txtBox" Width="75px" Style="text-align:right;"></asp:TextBox>
                                            </td>
                                            <asp:Panel ID="lblPriceTextbox" runat="server">
                                            <td>
                                                <asp:TextBox ID="txtPurchasePrice" runat="server" CssClass="txtBox" Style="text-align: right" Width="75px"></asp:TextBox>
                                            </td>
                                                </asp:Panel>
                                            <td style="width: 100px">
                                                <asp:Button AccessKey="A" ID="btnSave" OnClick="btnSave_Click" runat="server" Width="110px"
                                                    Font-Size="8pt" Text="Add Sku" ValidationGroup="vg" CssClass="Button" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="6">
                                                <asp:TextBox ID="txtBatchNo" runat="server" Width="80px" CssClass="txtBox" Enabled="False"
                                                    Visible="False">N/A</asp:TextBox>
                                                <asp:TextBox ID="txtFreeSKU" runat="server" CssClass="txtBox" Enabled="False" Width="70px"
                                                    Visible="false">0</asp:TextBox>
                                                <asp:TextBox ID="txtTransitDamage" runat="server" CssClass="txtBox" Visible="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" colspan="6">
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
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Left"
                                                                    Width="85px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SKU_NAME" HeaderText="SKU Name">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Left"
                                                                    Width="225px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Quantity" HeaderText="Quantity">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Right"
                                                                    Width="84px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="QuantityUnit" HeaderText="BatchNo">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Right"
                                                                    Width="80px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="FREE_SKU" HeaderText="Free SKU">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Right"
                                                                    Width="84px" />
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="TDAMAGE" HeaderText="T.DAMAGE">
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
                                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" BackColor="#007395" ForeColor="White" />
                                                        <AlternatingRowStyle BackColor="#F2F2F2" CssClass="GridAlternateRowStyle" ForeColor="#333333" />
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                                <table>
                                    <tr>
                                        <td style="visibility:hidden">
                                            <strong>
                                                <asp:Label ID="lblTotalCtn" runat="server" Width="103px" Height="16px" Text="Total Qty CTN"></asp:Label></strong>
                                        </td>
                                        <td>
                                            
                                        </td>
                                        <td align="center">
                                            <strong>
                                                <asp:Label ID="Label7" runat="server" Width="110px" Height="16px" Text="Total"></asp:Label></strong>
                                        </td>
                                        <td><asp:TextBox ID="txtTotalQuantityCtn" runat="server" Width="88px" CssClass="txtBox"
                                                ReadOnly="True"></asp:TextBox>
                                            <asp:TextBox ID="txtTotalQuantity" onkeyup="SearchList()" runat="server" Width="88px"
                                                CssClass="txtBox" ReadOnly="True"></asp:TextBox>
                                        </td>
                                        <td>
                                            <strong>
                                                <asp:Label ID="Label8" runat="server" Width="103px" Height="16px" Text="Total T.Damage" Visible="false"></asp:Label></strong>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtTotalTDamage" runat="server" Width="88px" CssClass="txtBox" ReadOnly="True" Visible="false"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Button AccessKey="S" ID="btnSaveDocument" runat="server" Width="119px" Font-Size="8pt"
                                                Text="Save Document" UseSubmitBehavior="False" OnClick="btnSaveDocument_Click"
                                                CssClass="Button" />
                                        </td>
                                        <td>
                                            <asp:Button AccessKey="C" ID="btnCancel" runat="server" Width="120px" Font-Size="8pt"
                                                Text="Cancel" UseSubmitBehavior="False" OnClick="btnCancel_Click" CssClass="Button" />
                                        </td>
                                        <td colspan="4"></td>
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
