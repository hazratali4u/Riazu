<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmAcntHeadAssinmntToForms.aspx.cs" Inherits="Forms_frmAcntHeadAssinmntToForms" Title="SAMS: Account Head Assignment" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPage" runat="Server">
    <div id="right_data">
        <div>
            <table width="100%">
                <tr>

                    <td align="left">

                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tbody>
                                        <tr>
                                            <td colspan="2" align="left">
                                                <asp:Label ID="lblmsg" runat="server" Visible="False" Width="175px" ForeColor="Red"></asp:Label>&nbsp;
                                            </td>
                                            <td style="height: 17px"></td>
                                        </tr>
                                        <tr>
                                            <td style="height: 25px;" align="left">
                                                <strong>
                                                    <asp:Label ID="Label1" runat="server" Width="120px" Text="Locaton"></asp:Label></strong>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="drpDistributor" runat="server" Width="300px" CssClass="DropList" AutoPostBack="True" OnSelectedIndexChanged="drpDistributor_SelectedIndexChanged"></asp:DropDownList></td>
                                            <td style="height: 10px"></td>
                                        </tr>
                                        <tr>
                                            <td style="height: 25px;" align="left">
                                                <strong>
                                                    <asp:Label ID="Label2" runat="server" Width="120px" Text="Form Name"></asp:Label></strong>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="drpFormName" runat="server" Width="300px" CssClass="DropList" AutoPostBack="true" OnSelectedIndexChanged="drpFormName_SelectedIndexChanged">
                                                    <asp:ListItem Text="Customer And DM Cash Reciept" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Customer Bank Reciept" Value="2"></asp:ListItem>
                                                </asp:DropDownList></td>
                                            <td style="height: 10px"></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px">
                                                <strong>
                                                    <asp:Label ID="Label3" runat="server" Width="120px" Text="Account Category " __designer:wfdid="w123"
                                                        CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td colspan="2">
                                                <asp:DropDownList ID="DrpAccountCategory" runat="server" Width="300px" Height="18px" __designer:wfdid="w124"
                                                    CssClass="DropList" AutoPostBack="True" OnSelectedIndexChanged="DrpAccountCategory_SelectedIndexChanged">
                                                    <asp:ListItem>Balance Sheet Account</asp:ListItem>
                                                    <asp:ListItem>Income Statment Account</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 100px"></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px; height: 16px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label34" runat="server" Width="120px" Text="Main Account Type" CssClass="lblbox"
                                                        __designer:wfdid="w58"></asp:Label></strong>
                                            </td>
                                            <td style="height: 28px" align="left" colspan="2">
                                                <asp:DropDownList ID="ddAccountType3" runat="server" Height="18px" Width="300px" CssClass="DropList"
                                                    __designer:wfdid="w59" OnSelectedIndexChanged="ddAccountType3_SelectedIndexChanged"
                                                    AutoPostBack="True">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 100px; height: 16px"></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px; height: 16px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label62" runat="server" Width="120px" Text="Sub Account Type" CssClass="lblbox"
                                                        __designer:wfdid="w60"></asp:Label></strong>
                                            </td>
                                            <td style="height: 28px" align="left" colspan="2">
                                                <asp:DropDownList ID="ddAccountSubType2" runat="server" Height="18px" Width="300px" CssClass="DropList"
                                                    __designer:wfdid="w61" OnSelectedIndexChanged="ddAccountSubType2_SelectedIndexChanged"
                                                    AutoPostBack="True">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 100px; height: 16px"></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px; height: 16px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label71" runat="server" Width="120px" Text="Detail Account Type" CssClass="lblbox"
                                                        __designer:wfdid="w62"></asp:Label></strong>
                                            </td>
                                            <td style="height: 28px" align="left" colspan="2">
                                                <asp:DropDownList ID="drpAccountTypeDetail" runat="server" Height="18px" Width="300px" CssClass="DropList"
                                                    __designer:wfdid="w63" OnSelectedIndexChanged="drpAccountTypeDetail_SelectedIndexChanged"
                                                    AutoPostBack="True">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 100px; height: 16px"></td>
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

                    <td align="left">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tbody>
                                        <tr>
                                            <td style="width: 100px" rowspan="1">
                                                <strong>
                                                    <asp:Label ID="Label4" runat="server" Width="200px" Text="Unassigned Account Heads" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 102px" valign="bottom" align="center"></td>
                                            <td style="width: 102px" rowspan="1">
                                                <strong>
                                                    <asp:Label ID="Label6" runat="server" Text="Assigned Account Heads" Width="200px"></asp:Label></strong>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px" rowspan="4">
                                                <asp:ListBox ID="lstUnAssignTown" runat="server" Width="300px" Height="250px" CssClass="DropList"></asp:ListBox></td>
                                            <td valign="middle" align="center">
                                                <asp:Button ID="btnAssign" runat="server" Width="30px" Font-Size="8pt" Text=">" OnClick="btnAssign_Click" CssClass="Button" />
                                            </td>
                                            <td style="width: 102px" rowspan="4">
                                                <asp:ListBox ID="lstAssignTown" runat="server" Width="300px" Height="250px" CssClass="DropList"></asp:ListBox></td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:Button ID="btnAssignAll" runat="server" Width="30px" Font-Size="8pt" Text=">>" OnClick="btnAssignAll_Click" CssClass="Button" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:Button ID="btnUnAssign" runat="server" Width="30px" Font-Size="8pt" Text="<<" OnClick="btnUnAssign_Click" CssClass="Button" />

                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="middle" align="center">
                                                <asp:Button ID="btnUnAssignAll" runat="server" Width="30px" Font-Size="8pt" Text="<" OnClick="btnUnAssignAll_Click" CssClass="Button" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 24px" rowspan="1"></td>
                                            <td style="width: 102px; height: 24px"></td>
                                            <td style="height: 24px" rowspan="1"></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>

                </tr>

            </table>
        </div>
    </div>
</asp:Content>
