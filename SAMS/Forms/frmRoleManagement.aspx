<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true" CodeFile="frmRoleManagement.aspx.cs" Inherits="frmRoleManagement" Title="SAMS: Role Management" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPage" runat="Server">
    <div id="right_data">
        <table width="100%">
            <tr>
                <td>

                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <table>
                                <tbody>
                                    <tr>
                                        <td style="width: 82px; height: 17px">
                                            <asp:Label ID="lblmsg" runat="server" Visible="False" Width="125px" ForeColor="Red"></asp:Label></td>
                                        <td>&nbsp; </td>
                                        <td style="height: 17px"></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <strong>
                                                <asp:Label ID="Label1" runat="server" Width="125px" Text="Role Description" CssClass="lblbox"></asp:Label></strong></td>
                                        <td style="width:250px;">
                                            <asp:DropDownList ID="ddRole" runat="server" Width="230px" CssClass="DropList" OnSelectedIndexChanged="ddRole_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                                            <asp:TextBox ID="TextBox1" runat="server" Visible="False" Width="195px" CssClass="txtBox " Enabled="False"></asp:TextBox></td>
                                        <td align="left">
                                            <asp:Button ID="btnNew" OnClick="btnNew_Click" runat="server" Width="75px" Font-Size="8pt" Text="New" CssClass="Button" />
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <strong>
                                                <asp:Label ID="Label11" runat="server" Width="125px" Text="Module 1st Layer" CssClass="lblbox"></asp:Label></strong></td>
                                        <td>
                                            <asp:DropDownList ID="DrpModule1stLayer" runat="server" Width="230px" CssClass="DropList" OnSelectedIndexChanged="DrpModule1stLayer_SelectedIndexChanged" AutoPostBack="True">
                                            </asp:DropDownList></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style="height: 24px">
                                            <strong>
                                                <asp:Label ID="Label2" runat="server" Width="125px" Text="Module 2nd Layer" CssClass="lblbox"></asp:Label></strong></td>
                                        <td>
                                            <asp:DropDownList ID="DrpModule2ndLayer" runat="server" Width="230px" CssClass="DropList" OnSelectedIndexChanged="DrpModule2ndLayer_SelectedIndexChanged" AutoPostBack="True">
                                            </asp:DropDownList></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style="height: 16px">
                                            <strong>
                                                <asp:Label ID="Label3" runat="server" Width="125px" Text="Module 3rd Layer" CssClass="lblbox"></asp:Label></strong></td>
                                        <td>
                                            <asp:DropDownList ID="DrpModule3rdLayer" runat="server" Width="230px" CssClass="DropList" OnSelectedIndexChanged="DrpModule3rdLayer_SelectedIndexChanged" AutoPostBack="True">
                                            </asp:DropDownList></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td style="height: 200px" colspan="4">
                                            <table>
                                                <tbody>
                                                    <tr>
                                                        <td style="width: 100px">
                                                            <strong>Unassigned Module</strong>
                                                        </td>
                                                        <td></td>
                                                        <td style="width: 102px">
                                                            <strong style="margin-left:20px;">Assigned Module</strong>
                                                        </td>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 100px" rowspan="5">
                                                            <asp:ListBox ID="lstUnAssignModule" runat="server" Width="250px" Height="200px" style="margin-right:20px;" CssClass="DropList"></asp:ListBox></td>
                                                        <td></td>
                                                        <td style="width: 102px" rowspan="5">
                                                            <asp:ListBox ID="lstAssignModule" runat="server" Width="250px" Height="200px" style="margin-left:20px;" CssClass="DropList"></asp:ListBox></td>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <asp:Button ID="btnAssign" runat="server" Width="40px" Font-Size="8pt" Text=">" OnClick="btnAssign_Click" CssClass="Button" />
                                                        </td>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <asp:Button ID="btnAssignAll" runat="server" Width="40px" Font-Size="8pt" Text=">>" OnClick="btnAssignAll_Click" CssClass="Button" />
                                                        </td>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <asp:Button ID="btnUnAssignAll" runat="server" Width="40px" Font-Size="8pt" Text="<<" OnClick="btnUnAssignAll_Click" CssClass="Button" />
                                                        </td>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <asp:Button ID="btnUnAssign" runat="server" Width="40px" Font-Size="8pt" Text="<" OnClick="btnUnAssign_Click" CssClass="Button" />
                                                        </td>
                                                        <td></td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>

            </tr>
            <tr>
                <td align="left" width="10%">
                    <asp:Button ID="btnReport" runat="server" Text="View Report" OnClick="btnReport_Click" Width="120px" CssClass="Button" />
                </td>
            </tr>

        </table>


    </div>
</asp:Content>

