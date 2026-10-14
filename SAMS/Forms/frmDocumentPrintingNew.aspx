<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true" CodeFile="frmDocumentPrintingNew.aspx.cs" Inherits="Forms_frmDocumentPrintingNew" Title="SAMS :: Print Sale Document New" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script language="JavaScript" type="text/javascript">
        function ValidateForm() {

            return true;
        }
        function SelectAllPrincipals() {
            var chkBoxList = document.getElementById('<%= cblPrincipal.ClientID %>');
            var chkBox = document.getElementById('<%= ChbAllPrincipals.ClientID %>');
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

        function UnCheckSelectAllPrincipals() {
            var chkBox = document.getElementById('<%= ChbAllPrincipals.ClientID %>');
            var chkBoxList = document.getElementById('<%= cblPrincipal.ClientID %>');
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
        <table width="100%">
            <tr>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <table>
                                <tbody>
                                    <tr>
                                        <td align="left" colspan="4">
                                            <strong>
                                                <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Font-Bold="True"></asp:Label></strong>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                        </td>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="Label5" runat="server" Width="95px" Text="Company"></asp:Label></strong>
                                        </td>
                                        <td align="left">
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:DropDownList ID="ddlCompany" runat="server" Width="200px">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>                                    
                                    <tr>
                                        <td align="left">
                                        </td>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="Label2" runat="server" Width="120px" Text="Document Type" CssClass="lblbox"></asp:Label></strong>
                                        </td>
                                        <td align="left">
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:DropDownList ID="DrpLedgerType" runat="server" Width="200px" CssClass="DropList"
                                                OnSelectedIndexChanged="DrpLedgerType_SelectedIndexChanged" AutoPostBack="True">
                                                <asp:ListItem Value="0">Order </asp:ListItem>
                                                <asp:ListItem Value="1">Invoice</asp:ListItem>
                                                <asp:ListItem Value="2">Sale Return</asp:ListItem>
                                                <asp:ListItem Value="3">Delivery Challan</asp:ListItem>
                                                <asp:ListItem Value="4">Invoice USD</asp:ListItem>
                                                <asp:ListItem Value="5">GST Invoice</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                        </td>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="lbltoLocation" runat="server" Width="94px" Text="Location" CssClass="lblbox"></asp:Label></strong>
                                        </td>
                                        <td align="left">
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:DropDownList ID="drpDistributor" runat="server" Width="200px" CssClass="DropList"
                                                OnSelectedIndexChanged="drpDistributor_SelectedIndexChanged" AutoPostBack="True">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <asp:Panel ID="PanelPrincipal" runat="server">
                                    <tr>
                                        <td align="left">
                                        </td>
                                        <td align="left" valign="top">
                                            &nbsp;
                                        </td>
                                        <td align="left">
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:CheckBox ID="ChbAllPrincipals" onclick="SelectAllPrincipals()" runat="server" Text="All Principals" Font-Bold="true" ForeColor="Black"></asp:CheckBox>
                                        </td>
                                    </tr>
                                    <tr> 
                                        <td align="left">
                                        </td>
                                        <td align="left" valign="top">
                                            <strong>
                                                <asp:Label ID="Label6" runat="server" Width="78px" Text="Principal" CssClass="lblbox"></asp:Label></strong>
                                        </td>
                                        <td align="left">
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:CheckBoxList ID="cblPrincipal" runat="server" BorderColor="#333333" BorderStyle="Solid" BorderWidth="1px" onclick="UnCheckSelectAllPrincipals()">
                                            </asp:CheckBoxList>
                                        </td>
                                    </tr>
                                        </asp:Panel>
                                    <tr>
                                        <td align="left">
                                        </td>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="Label1" runat="server" Width="78px" Text="Sale Force" CssClass="lblbox"></asp:Label></strong>
                                        </td>
                                        <td align="left">
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:DropDownList ID="DrpArea" runat="server" Width="200px" CssClass="DropList">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                        </td>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="Label3" runat="server" Width="90px" Height="13px" Text="From Date"></asp:Label></strong>
                                        </td>
                                        <td align="left">
                                        </td>
                                        <td style="height: 25px" align="left">
                                            &nbsp;<asp:TextBox ID="txtStartDate" onkeyup="BlockStartDateKeyPress()" runat="server"
                                                Width="150px" CssClass="txtBox" MaxLength="10"></asp:TextBox>
                                            <asp:ImageButton ID="ibtnStartDate" runat="server" Width="16px" ImageUrl="~/App_Themes/Granite/Images/date.gif">
                                            </asp:ImageButton>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                        </td>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="Label4" runat="server" Width="80px" Height="13px" Text="To Date"></asp:Label></strong>
                                        </td>
                                        <td align="left">
                                        </td>
                                        <td style="height: 25px" align="left">
                                            &nbsp;<asp:TextBox ID="txtEndDate" onkeyup="BlockEndDateKeyPress()" runat="server"
                                                Width="150px" CssClass="txtBox " MaxLength="10"></asp:TextBox>
                                            <asp:ImageButton ID="ibnEndDate" runat="server" Width="16px" ImageUrl="~/App_Themes/Granite/Images/date.gif">
                                            </asp:ImageButton>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                        </td>
                                        <td align="left">
                                        </td>
                                        <td align="left">
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <%@ register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
                                            <cc1:CalendarExtender ID="CEStartDate" runat="server" TargetControlID="txtStartDate"
                                                PopupButtonID="ibtnStartDate" Format="dd-MMM-yyyy">
                                            </cc1:CalendarExtender>
                                            <cc1:CalendarExtender ID="CEEndDate" runat="server" TargetControlID="txtEndDate"
                                                PopupButtonID="ibnEndDate" Format="dd-MMM-yyyy">
                                            </cc1:CalendarExtender>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    &nbsp; &nbsp;
                    <asp:Button  CssClass="Button" ID="Button2" runat="server" Text="Print Document" OnClick="Button2_Click"
                        Width="110px" />
                    <asp:Button  CssClass="Button" ID="Button1" runat="server" Text="View Document" OnClick="Button1_Click"
                        Width="110px" />
                    <asp:Button  CssClass="Button" ID="btnViewExcel" runat="server" Text="View Excel" OnClick="btnViewExcel_Click"
                        Width="100px" />
                </td>
            </tr>
        </table>
        <asp:GridView ID="GrdOrder" runat="server" AutoGenerateColumns="False" BackColor="White"
            BorderColor="White" CssClass="gridRow2" ForeColor="SteelBlue" HorizontalAlign="Center"
            OnRowEditing="GrdOrder_RowEditing" Width="99%">
            <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                PreviousPageText="Previous" />
            <RowStyle ForeColor="Black" />
            <Columns>
                <asp:BoundField DataField="AREA_NAME" HeaderText="Route Name">
                    <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="SOLD_TO" HeaderText="Customer">
                    <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="CUSTOMERADDRESS" HeaderText="Address">
                    <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" />
                </asp:BoundField>
                <asp:BoundField DataField="DocumentNo" HeaderText="Document Id">
                    <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="DOCUMENT_DATE" HeaderText="Document Date">
                    <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="TOTAL_NET_AMOUNT" DataFormatString="{0:F2}" HeaderText="Net Amount">
                    <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:CommandField EditText="Select" HeaderText="Print" ShowEditButton="True">
                    <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" />
                </asp:CommandField>
            </Columns>
            <FooterStyle BackColor="White" />
            <PagerStyle BackColor="Transparent" />
            <HeaderStyle CssClass="tblhead" />
            <AlternatingRowStyle BackColor="#F2F2F2" CssClass="GridAlternateRowStyle" ForeColor="#333333" />
        </asp:GridView>
    </div>
</asp:Content>
