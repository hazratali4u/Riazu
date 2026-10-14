<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmDesignation.aspx.cs" Inherits="Forms_frmDesignation" Title="SAMS: Designation" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPage" runat="Server">
    <div id="right_data">
        <table width="60%">
            <tr>
                <td style="width: 100px">
                    <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                        <ContentTemplate>
                            <table width="100%">
                                <tbody>
                                    <tr>
                                        <td colspan="2">
                                            <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Font-Bold="True" AccessKey="C"></asp:Label><br />
                                        </td>
                                        <td style="width: 100px"></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 49px; height: 29px">
                                            <strong>
                                                <asp:Label ID="Label2" runat="server" Width="65px" Text="Name"></asp:Label></strong>
                                        </td>
                                        <td style="width: 100px; height: 29px">
                                            <asp:TextBox ID="txtDesignation" runat="server" Width="200px" CssClass="txtBox "
                                                Enabled="False"></asp:TextBox>
                                        </td>
                                        <td style="width: 100px; height: 29px"></td>
                                    </tr>
                                    <tr>
                                        <td align="right">&nbsp;
                                        </td>
                                        <td>
                                            <asp:Button ID="btnSaveDesignation" OnClick="btnSaveDesignation_Click" runat="server"
                                                Width="85px" Font-Size="8pt" Text="New" CssClass="Button" />
                                        </td>
                                        <td style="width: 100px; height: 37px"></td>
                                    </tr>
                                </tbody>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td style="width: 100px"></td>
            </tr>
            <tr>
                <td align="center" colspan="2">
                    <asp:Panel ID="Panel1" runat="server" Height="300px" ScrollBars="Vertical" Width="100%" BorderWidth="1px">
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="gvDesignation" runat="server" Width="100%" ForeColor="SteelBlue"
                                    CssClass="gridRow2" BorderColor="White" BackColor="White" HorizontalAlign="Center"
                                    AutoGenerateColumns="False" OnRowEditing="gvDesignation_RowEditing" OnRowDeleting="gvDesignation_RowDeleting"
                                    OnPageIndexChanging="gvDesignation_PageIndexChanging">
                                    <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                        PreviousPageText="Previous"></PagerSettings>
                                    <Columns>
                                        <asp:BoundField DataField="REF_ID" HeaderText="Id">
                                            <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Left" BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"
                                                CssClass="HidePanel"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="SLASH_CODE" HeaderText="Code" Visible="false">
                                            <ItemStyle HorizontalAlign="Left" BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="SLASH_DESC" HeaderText="Name">
                                            <ItemStyle HorizontalAlign="Left" BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
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
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
