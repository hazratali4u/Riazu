<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master"
    AutoEventWireup="true" CodeFile="RptCustomerAnexusC.aspx.cs"
    Inherits="Forms_RptCustomerAnexusC" Title="SAMS: Annexure C Report" %>

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
                                    <tr>
                                        <td align="left"></td>
                                        <td align="left" colspan="3"></td>
                                    </tr>

                                    <tr>
                                        <td align="left"></td>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="lbltoLocation" runat="server" CssClass="lblbox" Text="Location" Width="94px"></asp:Label></strong></td>
                                        <td align="left"></td>
                                        <td align="left" style="height: 25px">
                                            <asp:DropDownList ID="drpDistributor" runat="server" CssClass="DropList" Width="200px">
                                            </asp:DropDownList></td>
                                    </tr>
                                    <asp:Panel ID="PanelPrincipal" runat="server">
                                    <tr>
                                        <td align="left"></td>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="Label6" runat="server" CssClass="lblbox" Text="Principal" Width="70px"></asp:Label></strong></td>
                                        <td align="left"></td>
                                        <td align="left" style="height: 25px">
                                            <asp:DropDownList ID="DrpPrincipal" runat="server" Width="200px" CssClass="DropList">
                                            </asp:DropDownList></td>
                                    </tr>
                                        </asp:Panel>
                                    <tr>
                                        <td align="left"></td>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="Label1" runat="server" CssClass="lblbox" Text="Customer Type" Width="90px"></asp:Label></strong></td>
                                        <td align="left"></td>
                                        <td align="left" style="height: 25px">
                                            <asp:DropDownList ID="DrpCUstomerType" runat="server" Width="200px" CssClass="DropList">
                                                <asp:ListItem Text="All" Value="1" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Registered" Value="2">
                                                </asp:ListItem>
                                                <asp:ListItem Text="Unregistered" Value="3"></asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td align="left"></td>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="Label3" runat="server" Height="13px" Text="From Date" Width="77px"></asp:Label></strong></td>
                                        <td align="left"></td>
                                        <td align="left" style="height: 25px">
                                            <asp:TextBox ID="txtStartDate" runat="server" CssClass="txtBox" MaxLength="10"
                                                onkeyup="BlockStartDateKeyPress()" Width="150px"></asp:TextBox>
                                            <asp:ImageButton ID="ibtnStartDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                                Width="16px" /></td>
                                    </tr>
                                    <tr>
                                        <td align="left"></td>
                                        <td align="left">
                                            <strong>
                                                <asp:Label ID="Label4" runat="server" Height="13px" Text="To Date" Width="76px"></asp:Label></strong></td>
                                        <td align="left"></td>
                                        <td align="left" style="height: 25px">
                                            <asp:TextBox ID="txtEndDate" runat="server" CssClass="txtBox " MaxLength="10"
                                                onkeyup="BlockEndDateKeyPress()" Width="150px"></asp:TextBox>
                                            <asp:ImageButton ID="ibnEndDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                                Width="16px" /></td>
                                    </tr>

                                    <tr>
                                        <td align="left"></td>
                                        <td align="left"></td>
                                        <td align="left"></td>
                                        <td align="left" style="height: 25px">
                                            <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
                                            <script runat="server">
                                            </script>

                                            <cc1:CalendarExtender ID="CEStartDate" runat="server" Format="dd-MMM-yyyy"
                                                PopupButtonID="ibtnStartDate" TargetControlID="txtStartDate">
                                            </cc1:CalendarExtender>
                                            <cc1:CalendarExtender ID="CEEndDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibnEndDate"
                                                TargetControlID="txtEndDate">
                                            </cc1:CalendarExtender>
                                        </td>
                                    </tr>
                                </table>
                                &nbsp;
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
            &nbsp;
        
        </div>
        <div>
            <table width="100%">
                <tr>
                    <td>

                        <asp:Button ID="BtnViewPdf" runat="server" CssClass="Button" OnClick="BtnViewPdf_Click" Style="margin-left: 110px;"
                            Text="View PDF" Width="90" />
                        <asp:Button ID="btnViewExcel" runat="server" CssClass="Button"
                            Text="View Excel" Width="90" OnClick="btnViewExcel_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </div>

</asp:Content>
