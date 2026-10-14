<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmOpeningStockNew.aspx.cs" Inherits="Forms_frmOpeningStockNew" Title="SAMS: Opening Stock" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script type="text/javascript" src="../AjaxLibrary/jquery.searchabledropdown-1.0.8.min.js"></script>
    <script language="JavaScript" type="text/javascript">
        function ConfirmDelete() {
            if (confirm("Do you want to Cancel this record?") == true)
                return true;

            else {
                return false;
            }
        }

        function ValidateForm() {
            var str;

          <%--  str = document.getElementById('<%=txtskuName.ClientID%>').value;
            if (str == null || str.length == 0) {
                alert('Must Select SKU Name');
                return false;
            }--%>
            str = document.getElementById('<%=txtQuantity.ClientID%>').value;
            if (str == null || str.length == 0) {
                alert('Must Enter Quantity');
                return false;
            }

            return true;
        }
        <%--function SearchList() {
            var l = document.getElementById('<%= lstCode.ClientID %>');
            var tb = document.getElementById('<%= txtskuCode.ClientID %>');

            if (tb.value == "") {
                ClearSelection(l);
            }
            else {
                for (var i = 0; i < l.options.length; i++) {
                    if (l.options[i].value.toLowerCase().match(tb.value.toLowerCase())) {
                        l.options[i].selected = true;
                        return false;
                    }
                    else {
                        ClearSelection(l);
                    }
                }
            }
        }--%>
        <%-- function SearchedCode() {
            var l = document.getElementById('<%= lstCode.ClientID %>');
            var str;
            for (var i = 0; i < l.options.length; i++) {
                if (l.options[i].selected) {
                    str = l.options[i].value;
                    ClearSelection(l);
                    break;
                }
                else {
                    str = "";
                }
            }--%>
        <%--   var stroption = document.getElementById("<%= txtskuCode.ClientID %>").value;
            if (str.length > 0) {
                document.getElementById("<%= txtskuCode.ClientID %>").value = str.substring(0, str.indexOf('-'));
                document.getElementById("<%= txtskuName.ClientID %>").value = str.substring(str.indexOf('-') + 1);
                document.getElementById("<%= Panel1.ClientID %>").className = "HidePanel";
            }
            else if (stroption.length == 0) {
                document.getElementById("<%= Panel1.ClientID %>").className = "ShowPanel";
                document.getElementById("<%= lstCode.ClientID %>").focus();

            }

        }--%>
        <%--function SelectCode(e) {
            var key = e.charCode ? e.charCode : e.keyCode ? e.keyCode : 0;
            if (key == 13) {
                e.preventDefault();
                var str = document.getElementById("<%= lstCode.ClientID %>").value;
                document.getElementById("<%= txtskuCode.ClientID %>").value = str.substring(0, str.indexOf('-'));
                document.getElementById("<%= txtskuName.ClientID %>").value = str.substring(str.indexOf('-') + 1);
                document.getElementById("<%= Panel1.ClientID %>").className = "HidePanel";
                document.getElementById("<%= txtQuantity.ClientID %>").focus();

            }
        }--%>
        function ClearSelection(lb) {
            lb.selectedIndex = -1;
        }
        function pageLoad() {

            $("select").searchable();
        }
        function isNumberKey(evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }
            return true;
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
                                            <td align="left" colspan="2">
                                                <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Font-Bold="True"></asp:Label>
                                                  <asp:DropDownList Visible="false" ID="DrpDocumentType" runat="server" Width="200px" CssClass="DropList"
                                                    AutoPostBack="True" OnSelectedIndexChanged="DrpDocumentType_SelectedIndexChanged">
                                                    <asp:ListItem Value="7">Opening Stock</asp:ListItem>
                                                   <%-- <asp:ListItem Value="8">Short</asp:ListItem>
                                                    <asp:ListItem Value="9">Excess</asp:ListItem>--%>
                                                </asp:DropDownList>
                                            </td>
                                            <td align="left" colspan="1">
                                            </td>
                                            
                                                <%--<strong>

                                                    <asp:Label ID="Label4" runat="server" Width="239px" Height="8px" CssClass="lblbox"></asp:Label></strong>
                                                &nbsp; &nbsp;&nbsp;
                                                <asp:Panel ID="Panel1" runat="server" Width="250px" Height="170px" CssClass="HidePanel"
                                                    BorderWidth="1px" BorderStyle="Groove" BorderColor="Silver" BackColor="Silver">
                                                    <strong>
                                                        <asp:Label ID="Label3" runat="server" CssClass="lblbox" Width="170px">Select from SKU List</asp:Label></strong><br />
                                                    <asp:ListBox ID="lstCode" runat="server" Height="154px" onkeydown="SelectCode(event)"
                                                        SelectionMode="Multiple" Width="245px"></asp:ListBox>
                                                </asp:Panel></td>--%>
                                            
                                        </tr>
                                       
                                        <tr>
                                            <td style="height: 25px" align="left">
                                                <strong>
                                                    <asp:Label ID="lblDocumentNo" runat="server" Width="94px" Text="Document No" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="height: 25px">
                                                <asp:DropDownList ID="drpDocumentNo" runat="server" Width="200px" CssClass="DropList"
                                                    AutoPostBack="True" OnSelectedIndexChanged="drpDocumentNo_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="height: 25px"></td>
                                        </tr>
                                        <tr>
                                            <td style="height: 25px" align="left">
                                                <strong>
                                                    <asp:Label ID="lbltoLocation" runat="server" Width="94px" Text="Principal" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="height: 25px">
                                                <asp:DropDownList ID="drpPrincipal" runat="server" Width="200px" CssClass="DropList"
                                                    AutoPostBack="True" OnSelectedIndexChanged="drpPrincipal_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="height: 25px"></td>
                                        </tr>
                                        <tr>
                                            <td style="height: 25px" align="left">
                                                <strong>
                                                    <asp:Label ID="lblfromLocation" runat="server" Width="94px" Text="Location" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="height: 25px">
                                                <asp:DropDownList ID="drpDistributor" runat="server" Width="200px" CssClass="DropList"
                                                    AutoPostBack="True" OnSelectedIndexChanged="drpDistributor_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="height: 25px"></td>
                                        </tr>
                                        <tr>
                                            <td style="height: 25px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label1" runat="server" Width="94px" Text="Remarks" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="height: 25px">
                                                <asp:TextBox ID="txtDocumentNo" runat="server" Width="195px" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                            <td style="height: 25px"></td>
                                        </tr>
                                        <tr style="visibility:hidden">
                                            <td >
                                                <asp:CheckBox ID="ChbBatchNo" runat="server"  Text="Batch No" AutoPostBack="True"
                                                    Visible="false"></asp:CheckBox>
                                            </td>
                                            <td >
                                                <asp:CheckBox ID="ChbFreeSKU" runat="server"  Text="Apply Free SKU"
                                                    AutoPostBack="True" Visible="false"></asp:CheckBox>
                                            </td>
                                            <td ></td>
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
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table border="1">
                                    <tbody>
                                        <tr>
                                            <%--<td>
                                                <asp:Label ID="lblskuCode" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="  SKU Code" BackColor="#006699"></asp:Label>
                                            </td>--%>
                                            <td colspan="2">
                                                <asp:Label ID="lblskuname" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="   SKU Name" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblquantity" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="Qty CTN" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>
                                            <td colspan="2">
                                                <asp:Label ID="lblquantityUnit" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="Qty Unit" CssClass="lblbox" BackColor="#006699" Enabled="False"></asp:Label>
                                                <asp:Label ID="lblFreeSKU" runat="server" Width="77px" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="Free SKU" CssClass="lblbox" BackColor="#006699" Enabled="False"
                                                    Visible="false"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label41" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="Add SKU" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <%--<td>
                                                <asp:TextBox ID="txtskuCode" onkeyup="SearchList()" runat="server" Width="82px" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtskuName" runat="server" Width="200px" Font-Bold="True" CssClass="txtBox"
                                                    Enabled="False"></asp:TextBox>
                                            </td>--%>
                                            <td colspan="2">
                                                <asp:DropDownList ID="ddlSKuCde" runat="server" Width="316px">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtQuantity" onfocus="SearchedCode()" onkeypress="return isNumberKey(event)" runat="server" Width="80px"
                                                    CssClass="txtBox "></asp:TextBox>
                                            </td>
                                            <td colspan="2">
                                                <asp:TextBox ID="txtUnit" runat="server" Width="80px" onkeypress="return isNumberKey(event)" CssClass="txtBox"></asp:TextBox>
                                                <asp:TextBox ID="txtBatchNo" runat="server" Visible="False" Width="51px"></asp:TextBox>
                                                <asp:TextBox ID="txtFreeSKU" runat="server" Width="70px" CssClass="txtBox" Enabled="False"
                                                    Visible="false">0</asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Button AccessKey="A" ID="btnSave" OnClick="btnSave_Click" runat="server" Width="95px"
                                                    Font-Size="8pt" Text="Add Sku" ValidationGroup="vg" CssClass="Button" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" colspan="6">
                                                <asp:Panel ID="Panel2" runat="server" Width="640px" Height="240px" ScrollBars="Vertical"
                                                    BorderWidth="1px" BorderStyle="Groove" BorderColor="Silver">
                                                    <asp:GridView ID="GrdPurchase" runat="server" Width="620px" ForeColor="SteelBlue"
                                                        CssClass="gridRow2" BorderColor="White" BackColor="White" ShowHeader="False"
                                                        OnRowDeleting="GrdPurchase_RowDeleting" OnRowEditing="GrdPurchase_RowEditing"
                                                        HorizontalAlign="Center" AutoGenerateColumns="False">
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
                                                                    Width="90px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SKU_NAME" HeaderText="SKU Name">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Left"
                                                                    Width="240px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Quantity" HeaderText="Quantity">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Right"
                                                                    Width="100px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="QuantityUnit" HeaderText="BatchNo">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Right" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="FREE_SKU" HeaderText="Free SKU">
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
                                <asp:Button AccessKey="S" ID="btnSaveDocument" OnClick="btnSaveDocument_Click" runat="server"
                                    Width="119px" Font-Size="8pt" Text="Save Document" UseSubmitBehavior="False"
                                    CssClass="Button" />
                                <asp:Button AccessKey="C" ID="btnCancel" OnClick="btnCancel_Click" runat="server"
                                    Width="120px" Font-Size="8pt" Text="Cancel" UseSubmitBehavior="False" CssClass="Button" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
            &nbsp;
        </div>
    </div>
</asp:Content>
