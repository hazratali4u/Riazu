<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true" CodeFile="frmPrintSaleDocuments.aspx.cs" Inherits="Forms_frmPrintSaleDocuments" Title="SAMS: Print Sale Document" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script language="JavaScript" type="text/javascript">
        function ValidateForm() {
            return true;
        }
    </script>
    <div id="right_data">
        <div>
            <table width="100%">
                <tr>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tbody>
                                        <tr>
                                            <td align="left" colspan="1"></td>
                                            <td align="left" colspan="4">
                                                <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Font-Bold="True"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 26px" align="left" colspan="1"></td>
                                            <td style="height: 26px" align="left" colspan="1"></td>
                                            <td style="height: 26px" align="left" colspan="3"></td>
                                        </tr>
                                        <tr>
                                            <td align="left"></td>
                                            <td align="left"></td>
                                            <td align="left">
                                                <strong>
                                                    <asp:Label ID="Label7" runat="server" Width="95px" Text="Paper Size" CssClass="lblbox"></asp:Label>
                                                </strong>
                                            </td>
                                            <td align="left"></td>
                                            <td style="height: 25px" align="left">
                                                <asp:DropDownList ID="DrpPrintPaperSize" runat="server" Width="200px" CssClass="DropList" OnSelectedIndexChanged="DrpPrintPaperSize_SelectedIndexChanged" AutoPostBack="true">
                                                    <asp:ListItem Value="0" Selected="True">A4 </asp:ListItem>
                                                    <asp:ListItem Value="1">A4 (With Summary)</asp:ListItem>
                                                    <asp:ListItem Value="2">A5</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <asp:Panel ID="PanelCompany" runat="server">
                                            <tr>
                                                <td align="left"></td>
                                                <td align="left"></td>
                                                <td align="left">
                                                    <strong>
                                                        <asp:Label ID="Label8" runat="server" Width="95px" Text="Company"></asp:Label>
                                                    </strong>
                                                </td>
                                                <td align="left"></td>
                                                <td style="height: 25px" align="left">
                                                    <asp:DropDownList ID="ddlCompany" runat="server" Width="200px">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </asp:Panel>
                                        <tr style="display: none;">
                                            <td style="height: 23px" align="left" colspan="1"></td>
                                            <td style="height: 23px" align="left" colspan="4">
                                                <div id="divFilter" class="containeRadioButtons">
                                                    <table width="100%">
                                                        <tbody>
                                                            <tr>
                                                                <td align="left">
                                                                    <asp:RadioButtonList ID="rblCustomerType" runat="server" Width="300px" RepeatDirection="Horizontal">
                                                                        <%-- <asp:ListItem Selected="True" Value="-1">All</asp:ListItem>--%>
                                                                        <asp:ListItem Value="1">With GST</asp:ListItem>
                                                                        <asp:ListItem Selected="True" Value="0">WithOut GST</asp:ListItem>
                                                                    </asp:RadioButtonList>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left">
                                                                    <asp:RadioButtonList ID="rbOrderstatus" runat="server" Width="300px" RepeatDirection="Horizontal">
                                                                        <asp:ListItem Selected="True" Value="0">All</asp:ListItem>
                                                                        <asp:ListItem Value="1">Stock Return</asp:ListItem>
                                                                    </asp:RadioButtonList>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left"></td>
                                            <td align="left"></td>
                                            <td align="left">
                                                <strong>
                                                    <asp:Label ID="Label2" runat="server" Width="95px" Text="Document Type" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td align="left"></td>
                                            <td style="height: 25px" align="left">
                                                <asp:DropDownList ID="DrpLedgerType" runat="server" Width="200px" CssClass="DropList"
                                                    OnSelectedIndexChanged="DrpLedgerType_SelectedIndexChanged" AutoPostBack="True">
                                                    <asp:ListItem Value="0">Order </asp:ListItem>
                                                    <asp:ListItem Value="1">Invoice</asp:ListItem>
                                                    <asp:ListItem Value="2">Sale Return</asp:ListItem>
                                                    <asp:ListItem Value="3">Delivery Challan</asp:ListItem>
                                                    <asp:ListItem Value="6">Fresh Return</asp:ListItem>
                                                    <%--<asp:ListItem Value="5">Non Executed Orders</asp:ListItem>--%>
                                                    <%--    <asp:ListItem Value="4">Invoice USD</asp:ListItem>--%>
                                                </asp:DropDownList>
                                            </td>
                                            <td align="left">
                                                <asp:CheckBox ID="ChkInvoiceNo" runat="server" Text="With Invoice No" OnCheckedChanged="ChkInvoiceNo_CheckedChanged" AutoPostBack="true" /></td>
                                        </tr>
                                        <asp:Panel ID="PanelReportType" runat="server">
                                            <tr>
                                                <td align="left"></td>
                                                <td align="left"></td>
                                                <td align="left">
                                                    <strong>
                                                        <asp:Label ID="Label10" runat="server" Width="95px" Text="Report Type"></asp:Label>
                                                    </strong>
                                                </td>
                                                <td align="left"></td>
                                                <td style="height: 25px" align="left">
                                                    <asp:DropDownList ID="DrpReportType" runat="server" Width="200px" CssClass="DropList">
                                                        <%--<asp:ListItem Value="0">Units</asp:ListItem>
                                                    <asp:ListItem Value="1">Carton Units</asp:ListItem>--%>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </asp:Panel>
                                        <asp:Panel ID="PanelInvoiceNo" runat="server" Visible="false">
                                            <tr>
                                                <td align="left"></td>
                                                <td align="left"></td>
                                                <td align="left">
                                                    <strong>
                                                        <asp:Label ID="Label9" runat="server" Width="95px" Text="Invoice No" CssClass="lblbox"></asp:Label></strong>
                                                </td>
                                                <td align="left"></td>
                                                <td style="height: 25px" align="left">
                                                    <asp:TextBox ID="txtInvoiceNo" runat="server" Width="96%"></asp:TextBox>
                                                    <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" FilterType="Custom" ValidChars="0123456789" TargetControlID="txtInvoiceNo"></cc1:FilteredTextBoxExtender>
                                                </td>
                                                <td align="left"></td>
                                            </tr>
                                        </asp:Panel>
                                        <asp:Panel ID="PanelOtherFields" runat="server" Visible="true">
                                            <tr>
                                                <td align="left"></td>
                                                <td align="left"></td>
                                                <td align="left">
                                                    <strong>
                                                        <asp:Label ID="lbltoLocation" runat="server" Width="94px" Text="Location" CssClass="lblbox"></asp:Label></strong>
                                                </td>
                                                <td align="left"></td>
                                                <td style="height: 25px" align="left">
                                                    <asp:DropDownList ID="drpDistributor" runat="server" Width="200px" CssClass="DropList"
                                                        OnSelectedIndexChanged="drpDistributor_SelectedIndexChanged" AutoPostBack="True">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <asp:Panel ID="PanelPrincipal" runat="server">
                                                <tr>
                                                    <td align="left"></td>
                                                    <td align="left"></td>
                                                    <td align="left">
                                                        <strong>
                                                            <asp:Label ID="Label6" runat="server" Width="78px" Text="Principal" CssClass="lblbox"></asp:Label></strong>
                                                    </td>
                                                    <td align="left"></td>
                                                    <td style="height: 25px" align="left">
                                                        <asp:DropDownList ID="DrpPrincipal" runat="server" Width="200px" CssClass="DropList"
                                                            OnSelectedIndexChanged="DrpPrincipal_SelectedIndexChanged" AutoPostBack="True">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </asp:Panel>
                                            <tr>
                                                <td align="left"></td>
                                                <td align="left"></td>
                                                <td align="left">
                                                    <strong>
                                                        <asp:Label ID="Label1" runat="server" Width="78px" Text="Sale Force" CssClass="lblbox"></asp:Label></strong>
                                                </td>
                                                <td align="left"></td>
                                                <td style="height: 25px" align="left">
                                                    <asp:DropDownList ID="DrpArea" runat="server" Width="200px" CssClass="DropList">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left"></td>
                                                <td align="left"></td>
                                                <td align="left">
                                                    <strong>
                                                        <asp:Label ID="lblfromLocation" runat="server" Width="94px" Text="Customer Route"
                                                            CssClass="lblbox"></asp:Label></strong>
                                                </td>
                                                <td align="left"></td>
                                                <td style="height: 25px" align="left">
                                                    <asp:DropDownList ID="DrpRoute" runat="server" Width="200px" CssClass="DropList"
                                                        OnSelectedIndexChanged="DrpRoute_SelectedIndexChanged" AutoPostBack="True">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left"></td>
                                                <td align="left"></td>
                                                <td align="left">
                                                    <strong>
                                                        <asp:Label ID="Label5" runat="server" Width="94px" Text="Customer" CssClass="lblbox"></asp:Label></strong>
                                                </td>
                                                <td align="left"></td>
                                                <td style="height: 25px" align="left">
                                                    <asp:DropDownList ID="DrpCustomer" runat="server" Width="200px" CssClass="DropList">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>

                                                <td align="left"></td>
                                                <td align="left"></td>
                                                <td>
                                                    <strong>
                                                        <asp:Label ID="lblSort" runat="server" CssClass="lblbox" Text="Sort By" Width="78px"></asp:Label>
                                                    </strong>
                                                </td>
                                                <td align="left"></td>
                                                <td align="right">
                                                    <asp:DropDownList ID="DrpSort" Style="z-index: -1 !important;" runat="server" Width="200px">
                                                        <asp:ListItem Value="0">Document No</asp:ListItem>
                                                        <asp:ListItem Value="1">Customer Code</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left"></td>
                                                <td align="left"></td>
                                                <td>
                                                    <strong>
                                                        <asp:Label ID="lblSortOrder" runat="server" CssClass="lblbox" Text="Sort Order" Width="78px"></asp:Label>
                                                    </strong>
                                                </td>
                                                <td align="left"></td>
                                                <td>
                                                    <asp:RadioButtonList ID="rbtSortOrder" runat="server" RepeatDirection="Horizontal"
                                                        Width="192px">
                                                        <asp:ListItem Selected="True" Text="Ascending"></asp:ListItem>
                                                        <asp:ListItem Text="Descending"></asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td align="left"></td>
                                                <td align="left"></td>
                                                <td align="left">
                                                    <strong>
                                                        <asp:Label ID="Label3" runat="server" Height="13px" Text="From Date" Width="90px"></asp:Label>
                                                    </strong>
                                                </td>
                                                <td align="left"></td>
                                                <td align="left" style="height: 25px">&nbsp;<asp:TextBox ID="txtStartDate" runat="server" CssClass="txtBox" MaxLength="10"
                                                    onkeyup="BlockStartDateKeyPress()" Width="150px" Style="z-index: 1 !important;"></asp:TextBox>
                                                    <asp:ImageButton ID="ibtnStartDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                                        Width="16px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left"></td>
                                                <td align="left"></td>
                                                <td align="left">
                                                    <strong>
                                                        <asp:Label ID="Label4" runat="server" Height="13px" Text="To Date" Width="80px"></asp:Label>
                                                    </strong>
                                                </td>
                                                <td align="left"></td>
                                                <td align="left" style="height: 25px">&nbsp;<asp:TextBox ID="txtEndDate" runat="server" CssClass="txtBox " MaxLength="10"
                                                    onkeyup="BlockEndDateKeyPress()" Width="150px" Style="z-index: 1 !important;"></asp:TextBox>
                                                    <asp:ImageButton ID="ibnEndDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                                        Width="16px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
                                                    <cc1:CalendarExtender ID="CEStartDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibtnStartDate"
                                                        TargetControlID="txtStartDate">
                                                    </cc1:CalendarExtender>
                                                    <cc1:CalendarExtender ID="CEEndDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibnEndDate"
                                                        TargetControlID="txtEndDate">
                                                    </cc1:CalendarExtender>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="height: 26px" align="left" colspan="1"></td>
                                                <td style="height: 26px" align="left" colspan="1"></td>
                                                <td style="height: 26px" align="left" colspan="3"></td>
                                            </tr>
                                        </asp:Panel>
                                    </tbody>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        &nbsp; &nbsp;
                        <asp:Button ID="btnViewPDF" Style="margin-left: 105px;" runat="server" Width="90" Text="View PDF" OnClick="btnViewPDF_Click"
                            CssClass="Button" />
                        <asp:Button ID="btnViewExcel" runat="server" Width="90" Text="View Excel" OnClick="btnViewExcel_Click"
                            CssClass="Button" />
                    </td>
                </tr>
            </table>
            &nbsp;
        </div>
    </div>
</asp:Content>
