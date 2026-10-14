<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="RptCreditAging.aspx.cs" Inherits="Forms_RptCreditAging" Title="SAMS: Credit Aging Report" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script language="JavaScript" type="text/javascript">
        function ValidateForm() {

            return true;
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
                                        <td align="left">
                                        </td>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="Label2" runat="server" Width="94px" Text="Report Type" CssClass="lblbox"></asp:Label></strong>
                                        </td>
                                        <td valign="top" align="left" colspan="2">
                                            <asp:RadioButtonList ID="RbtReportType" runat="server" Width="200px" RepeatDirection="Horizontal" OnSelectedIndexChanged="RbtReportType_SelectedIndexChanged" AutoPostBack="True">
                                                <asp:ListItem Selected="True" Value="2">Summary</asp:ListItem>
                                                <asp:ListItem Value="1">Detail</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                    
                                    <tr>
                                        <td align="left">
                                        </td>
                                        <td valign="middle" align="left">
                                            <strong>
                                                <asp:Label ID="Label1" runat="server" Width="94px" Text="Location Type" CssClass="lblbox"></asp:Label></strong>
                                        </td>
                                        <td align="left">
                                        </td>
                                        <td style="height: 25px" valign="middle" align="left">
                                            <asp:DropDownList ID="ddDistributorType" runat="server" Width="200px" OnSelectedIndexChanged="ddDistributorType_SelectedIndexChanged"
                                                AutoPostBack="True" CssClass="DropList">
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
                                            <asp:DropDownList ID="drpDistributor" runat="server" Width="200px" CssClass="DropList">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <asp:Panel ID="PanelPrincipal" runat="server">
                                    <tr>
                                        <td align="left">
                                        </td>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="Label6" runat="server" Width="78px" Text="Principal" CssClass="lblbox"></asp:Label></strong>
                                        </td>
                                        <td align="left">
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:DropDownList ID="DrpPrincipal" runat="server" Width="200px" CssClass="DropList">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                        </asp:Panel>
                                    <tr>
                                        <td align="left">
                                        </td>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="lblNickName" runat="server" Width="79px" Text="Channel Type" CssClass="lblbox"
                                                    __designer:wfdid="w8"></asp:Label></strong>
                                        </td>
                                        <td align="left">
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:DropDownList ID="drpChannelType" runat="server" Width="200px" CssClass="DropList"
                                                __designer:wfdid="w7">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <asp:Panel ID="PanelOrderBooker" runat="server">
                                    <tr>
                                        <td align="left"></td>
                                        <td style="width: 82px" align="left">
                                            <strong>
                                                <asp:Label ID="Label5" runat="server" Width="79px" Text="Order Booker" CssClass="lblbox"></asp:Label></strong></td>
                                        <td align="left"></td>
                                        <td style="height: 25px" align="left">
                                            <asp:DropDownList ID="DrpOrderBooker" runat="server" Width="200px" CssClass="DropList"></asp:DropDownList></td>
                                    </tr>
                                        </asp:Panel>
                                    <asp:Panel ID="PenalReportType" runat="server">
                                    <tr>
                                        <td align="left">
                                        </td>
                                        <td align="left">
                                        </td>
                                        <td valign="top" align="left" colspan="2">
                                            <asp:RadioButtonList ID="rbtSotBy" runat="server" Width="301px" RepeatDirection="Horizontal"
                                                OnSelectedIndexChanged="rbtSotBy_SelectedIndexChanged" AutoPostBack="True">
                                                <%--<asp:ListItem Selected="True" Value="0">Principal Wise</asp:ListItem>
                                                <asp:ListItem Value="1">Customer Wise</asp:ListItem>
                                                <asp:ListItem>Date Wise</asp:ListItem>--%>
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                        </asp:Panel>
                                    <asp:Panel ID="PanelSortandOrder" runat="server">
                                        <tr>
                                    <td align="left"></td>
                                    <td align="left">
                                        <strong>
                                            <asp:Label ID="lblSortBy" runat="server" Text="Sort By"></asp:Label>
                                        </strong>
                                    </td>
                                    <td align="left"></td>
                                    <td align="left" style="height: 25px">
                                        <asp:DropDownList ID="ddlSortBy" runat="server" Width="200px">
                                            <asp:ListItem Value="1" Text="Customer"></asp:ListItem>
                                            <asp:ListItem Value="2" Text="Amount"></asp:ListItem>
                                            <asp:ListItem Value="3" Text="Days"></asp:ListItem>
                                            <asp:ListItem Value="4" Text="Invoice Date"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left"></td>
                                    <td align="left">
                                        <strong>
                                            <asp:Label ID="lblOrderBy" runat="server" Text="Order By"></asp:Label>
                                        </strong>
                                    </td>
                                    <td align="left"></td>
                                    <td align="left" style="height: 25px">
                                        <asp:DropDownList ID="ddlOrderBy" runat="server" Width="200px">
                                            <asp:ListItem Value="1" Text="Ascending"></asp:ListItem>
                                            <asp:ListItem Value="2" Text="Descending"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="PanelSubTotal" runat="server">
                                    <tr>
                                        <td align="left">
                                        </td>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="Label7" runat="server" Width="78px" Text="Report With" CssClass="lblbox"></asp:Label></strong>
                                        </td>
                                        <td align="left">
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:DropDownList ID="RbSubTotal" runat="server" Width="200px">
                                            <asp:ListItem Value="1" Text="With Out Sub Total"></asp:ListItem>
                                            <asp:ListItem Value="2" Text="With Sub Total" Selected="True"></asp:ListItem>
                                        </asp:DropDownList>
                                        </td>
                                    </tr>
                                        </asp:Panel>
                                    <asp:Panel ID="PanelStartDate" runat="server">
                                    <tr>
                                        <td align="left">
                                        </td>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="Label3" runat="server" Width="70px" Height="13px"
                                                    Text="Start Date"></asp:Label></strong>
                                        </td>
                                        <td align="left">
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:TextBox ID="txtDocmentDate" onkeyup="BlockStartDateKeyPress()" runat="server" Width="150px" CssClass="txtBox" MaxLength="10"></asp:TextBox><asp:ImageButton
                                                    ID="ibtnStartDate" runat="server" Width="16px" ImageUrl="~/App_Themes/Granite/Images/date.gif">
                                                </asp:ImageButton>
                                        </td>
                                    </tr>
                                        </asp:Panel>
                                    <tr>
                                        <td align="left">
                                        </td>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="Label4" runat="server" Width="80px" Height="13px" Text="End Date"></asp:Label></strong>
                                        </td>
                                        <td align="left">
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:TextBox ID="txtEndDate" runat="server" Width="149px" CssClass="txtBox " MaxLength="10"></asp:TextBox><asp:ImageButton
                                                ID="ibnEndDate" runat="server" Width="16px" ImageUrl="~/App_Themes/Granite/Images/date.gif">
                                            </asp:ImageButton>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            <cc1:CalendarExtender ID="CEStartDate" runat="server" TargetControlID="txtDocmentDate"
                                PopupButtonID="ibtnStartDate" Format="dd-MMM-yyyy">
                            </cc1:CalendarExtender>
                            <cc1:CalendarExtender ID="CEEndDate" runat="server" TargetControlID="txtEndDate"
                                PopupButtonID="ibnEndDate" Format="dd-MMM-yyyy">
                            </cc1:CalendarExtender>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <br />
                    <asp:Button ID="btnViewPDF" runat="server" CssClass="Button" Text="View PDF" Width="90" Style="margin-left:110px;"
                        OnClick="btnViewPDF_Click" />
                    <asp:Button ID="btnViewExcel" runat="server" CssClass="Button" Text="View Excel"
                        Width="90" OnClick="btnViewExcel_Click" />
                </td>
            </tr>
        </table>
        &nbsp;
    </div>
</asp:Content>
