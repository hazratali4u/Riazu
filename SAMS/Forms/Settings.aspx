<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="Settings.aspx.cs" Inherits="Forms_Settings" Title="SAMS: Application Settings" %>

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
                                            <td align="left">
                                                <strong>
                                                    <asp:Label ID="Label2" runat="server" Width="95px" Text="Setting Tab" CssClass="lblbox"></asp:Label>
                                                </strong>
                                            </td>
                                            <td align="left"></td>
                                            <td style="height: 25px" align="left">
                                                <asp:DropDownList ID="DrpTabType" runat="server" Width="200px" CssClass="DropList">
                                                    <asp:ListItem Value="1">General Setting</asp:ListItem>
                                                    <asp:ListItem Value="2">Sale Setting</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 98px">
                                                <strong>
                                                    <asp:Label ID="Label3" runat="server" Width="82px" Text="Description" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td align="left"></td>
                                            <td style="height: 25px" align="left">
                                                <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Width="400px" Height="30px" CssClass="txtBox "></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 98px">&nbsp;
                                            </td>
                                            <td align="left"></td>
                                            <td>
                                                <asp:CheckBox ID="chkIsActive" runat="server" Width="93px" Text="IsActive" Checked="True"></asp:CheckBox>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                                <asp:HiddenField ID="hfSettingID" runat="server" />
                                <asp:HiddenField ID="hfConfigrationDescID" runat="server" />
                                <asp:HiddenField ID="hfbtnStatus" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <asp:Button ID="btnSave" OnClick="btnSave_Click" runat="server" Style="margin-left: 108px;" Width="82px" Font-Size="8pt"
                                    Text="Save" ValidationGroup="vg" CssClass="Button" />
                                <asp:Button ID="btnCancel" runat="server" Width="73px" Font-Size="8pt" Text="Cancel"
                                    OnClick="btnCancel_Click" CssClass="Button" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
            &nbsp;
        </div>
        <div>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <div style="z-index: 101; left: 481px; width: 100px; position: absolute; top: 282px; height: 100px"
                        id="DIV1" onclick="return DIV1_onclick()">
                        <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                            <ProgressTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" Width="26px" Height="23px" ImageUrl="~/App_Themes/Granite/Images/image003.gif"></asp:ImageButton>&nbsp; Loading....
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </div>
                    <table class="tblhead">
                        <tbody>
                            <tr>
                                <td style="color: White; font-weight: bold;">
                                    <asp:Label ID="Label10" runat="server" Width="153px" Text="Select Searching Type"></asp:Label>
                                </td>
                                <td style="width: 170px; height: 21px" align="left">
                                    <asp:DropDownList ID="ddSearchType" runat="server" Width="200px" CssClass="DropList">
                                        <asp:ListItem Value="Description">All Records</asp:ListItem>
                                        <asp:ListItem Value="TabID">Tab Name</asp:ListItem>
                                        <asp:ListItem Value="Description">Description</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 224px; height: 21px" align="left">
                                    <asp:TextBox ID="txtSeach" runat="server" Width="200px" CssClass="txtBox "></asp:TextBox>
                                </td>
                                <td style="width: 250px; height: 21px" align="left">
                                    <asp:Button ID="btnFilter" runat="server" Width="85px" Font-Size="8pt" Text="Filter"
                                        OnClick="btnFilter_Click" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <asp:Panel ID="Panel1" runat="server" Width="100%" Height="250px" ScrollBars="Vertical" BorderWidth="1px">
                        <asp:GridView ID="Setting_grid" runat="server" Width="100%" ForeColor="SteelBlue" CssClass="gridRow2"
                            HorizontalAlign="Center" AutoGenerateColumns="False" BackColor="White" BorderColor="White"
                            OnPageIndexChanging="Setting_grid_PageIndexChanging" OnRowEditing="Setting_grid_RowEditing">
                            <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                PreviousPageText="Previous"></PagerSettings>
                            <Columns>
                                <asp:BoundField DataField="SettingDescID" HeaderText="SettingDescID">
                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                    <ItemStyle CssClass="HidePanel"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="TabID" HeaderText="TabID">
                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                    <ItemStyle CssClass="HidePanel "></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="TAB_NAME" HeaderText="Setting Tab">
                                    <HeaderStyle BorderColor="Silver" Width="200px" HorizontalAlign="Left" BorderWidth="1px" BorderStyle="Solid"></HeaderStyle>
                                    <ItemStyle BorderColor="Silver" Width="200px" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="Description" HeaderText="Description">
                                    <HeaderStyle BorderColor="Silver" BorderWidth="1px" HorizontalAlign="Left" BorderStyle="Solid"></HeaderStyle>
                                    <ItemStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="IsAtive" HeaderText="Status">
                                    <ItemStyle BorderColor="Silver" Width="80px" HorizontalAlign="Center" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="ConfigrationDescID" HeaderText="ConfigrationDescID">
                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                    <ItemStyle CssClass="HidePanel "></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="btnStatus" HeaderText="btnStatus">
                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                    <ItemStyle CssClass="HidePanel "></ItemStyle>
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
                            </Columns>
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" BackColor="#007395" ForeColor="White" />
                            <AlternatingRowStyle BackColor="#F2F2F2" CssClass="GridAlternateRowStyle" ForeColor="#333333" />
                        </asp:GridView>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
