<%@ Page Language="C#" AutoEventWireup="true" CodeFile="rptLoadPassNew.aspx.cs" Inherits="Forms_rptLoadPassNew"
    MasterPageFile="~/Forms/PageMaster.master" Title="SAMS :: Load Pass Summary New" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="content1" runat="server" ContentPlaceHolderID="cphPage">   
    <div id="right_data">
        <table width="100%">
            <tr>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <table>
                                <tbody>
                                    <tr>
                                        <td style="height: 25px;" align="left">
                                            &nbsp;
                                        </td>
                                        <td style="height: 25px;" align="left">
                                            &nbsp;
                                        </td>
                                        <td style="height: 25px;" align="left">
                                            &nbsp;
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:RadioButtonList ID="rblReport" runat="server" RepeatDirection="Horizontal" Width="350px"
                                                OnSelectedIndexChanged="rblReport_SelectedIndexChanged">
                                                <asp:ListItem Selected="True" Value="0">Customer Wise</asp:ListItem>
                                                <asp:ListItem Value="1">Delivery Man Wise</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                        <td style="height: 25px" align="left">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 25px;" align="left">
                                            &nbsp;
                                        </td>
                                        <td style="height: 25px;" align="left">
                                            &nbsp;
                                        </td>
                                        <td style="height: 25px;" align="left">
                                            &nbsp;
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <asp:CheckBox ID="cbType" runat="server" Text="With Gross Amount & Discount" Checked="true" />
                                        </td>
                                        <td style="height: 25px" align="left">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="height: 25px;">
                                        </td>
                                        <td align="left" style="height: 25px">
                                            <strong>
                                                <asp:Label ID="Label2" runat="server" CssClass="lblbox" Text="Location" Width="48px"
                                                    Enabled="False"></asp:Label></strong>
                                        </td>
                                        <td align="left" style="height: 25px;">
                                        </td>
                                        <td align="left" style="height: 25px">
                                            <asp:DropDownList ID="DrpLocation" runat="server" Width="210px" AutoPostBack="True"
                                                OnSelectedIndexChanged="DrpLocation_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" style="height: 25px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="width: 1px; height: 25px">
                                        </td>
                                        <td align="left" style="height: 25px">
                                            <strong>
                                                <asp:Label ID="lbltoLocation" runat="server" Text="Principal" Width="95px"></asp:Label>
                                            </strong>
                                        </td>
                                        <td align="left" style="width: 1px; height: 25px">
                                        </td>
                                        <td align="left" style="width: 203px; height: 25px">
                                            <asp:DropDownList ID="drpPrincipal" runat="server" Width="210px">
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" style="width: 1px; height: 25px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 1px;" align="left">
                                        </td>
                                        <td style="height: 25px" align="left">
                                            <strong>
                                                <asp:Label ID="lblSalesMan" runat="server" Width="95px" Text="Deliveryman"></asp:Label></strong>
                                        </td>
                                        <td style="width: 1px" align="left">
                                        </td>
                                        <td style="width: 203px; height: 25px" align="left">
                                            <asp:DropDownList ID="DrpOrderBooker" runat="server" Width="210px">
                                            </asp:DropDownList>
                                        </td>
                                        <td style="width: 1px; height: 25px" align="left">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="width: 1px; height: 25px">
                                        </td>
                                        <td align="left" style="height: 25px">
                                            <strong>
                                                <asp:Label ID="Label3" runat="server" Text="Report By" Width="95px"></asp:Label>
                                            </strong>
                                        </td>
                                        <td align="left" style="width: 1px; height: 25px">
                                        </td>
                                        <td align="left" style="width: 203px; height: 25px">
                                            <asp:DropDownList ID="ddlReportBY" runat="server" Width="210px">
                                                <asp:ListItem Value="1" Text="SKU Code" Selected="True"></asp:ListItem>
                                                <asp:ListItem Value="2" Text="SKU Name"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" style="width: 1px; height: 25px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="width: 1px; height: 25px">
                                        </td>
                                        <td align="left" style="height: 25px">
                                            <strong>
                                                <asp:Label ID="Label1" runat="server" CssClass="lblbox" Text="Date" Width="59px"></asp:Label>
                                            </strong>
                                        </td>
                                        <td align="left" style="width: 1px; height: 25px">
                                        </td>
                                        <td align="left" style="width: 203px; height: 25px">
                                            <asp:TextBox ID="txtFromDate" runat="server" CssClass="txtBox" MaxLength="10" Width="153px"></asp:TextBox>
                                            <asp:ImageButton ID="ImgBntFromCalc" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif" />
                                        </td>
                                        <td align="left" style="width: 1px; height: 25px">
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            <cc1:CalendarExtender ID="CalendarExtender1" runat="server" EnableViewState="False"
                                Format="dd-MMM-yyyy" PopupButtonID="ImgBntFromCalc" TargetControlID="txtFromDate">
                            </cc1:CalendarExtender>
                            &nbsp;
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:Button ID="Button2" runat="server" Text="View PDF" CssClass="btn" OnClick="Button2_Click" Style="margin-left:119px;" />
                    <asp:Button ID="Button1" runat="server" Text="View Excel" CssClass="btn" OnClick="Button1_Click" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
