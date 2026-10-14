<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmSKUData.aspx.cs" Inherits="Forms_frmSKUData" Title="SAMS: SKU Information" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPage" runat="Server">
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
                                            <td align="left" colspan="4">
                                                <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Font-Bold="True"></asp:Label>
                                            </td>
                                            <asp:HiddenField ID="hdnSku_ID" runat="server" Value="0" />
                                        </tr>

                                        <tr>
                                            <td style="width: 100px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label1" runat="server" Width="100px" Text="SKU Principal"></asp:Label></strong>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddskuPrincipal" runat="server" Width="200px" AutoPostBack="True"
                                                    OnSelectedIndexChanged="ddskuPrincipal_SelectedIndexChanged" CssClass="DropList">
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                            <td align="left" rowspan="10"></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label2" runat="server" Width="100px" Text="SKU Division"></asp:Label></strong>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddskudivision" runat="server" Width="200px" AutoPostBack="True"
                                                    OnSelectedIndexChanged="ddskudivision_SelectedIndexChanged" CssClass="DropList">
                                                </asp:DropDownList>
                                            </td>
                                            <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label3" runat="server" Width="100px" Text="SKU Category"></asp:Label></strong>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddskucategory" runat="server" Width="200px" AutoPostBack="True"
                                                    OnSelectedIndexChanged="ddskucategory_SelectedIndexChanged" CssClass="DropList">
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label14" runat="server" Width="100px" Text="Sub Category"></asp:Label></strong>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="drpsubcategory" runat="server" Width="200px" AutoPostBack="True"
                                                    OnSelectedIndexChanged="drpsubcategory_SelectedIndexChanged" CssClass="DropList">
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label4" runat="server" Width="100px" Text="SKU Brand"></asp:Label></strong>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddskuBrand" runat="server" Width="200px" AutoPostBack="True"
                                                    CssClass="DropList">
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 100px">
                                                <strong>
                                                    <asp:Label ID="Label9" runat="server" Text="GST On"></asp:Label></strong>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="DrpSKUTaxType" runat="server" Width="200px" CssClass="DropList">
                                                    <asp:ListItem Value="T">Trade Price</asp:ListItem>
                                                    <asp:ListItem Value="R">Retail Price</asp:ListItem>
                                                    <asp:ListItem Value="E">Exempted</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label6" runat="server" Text="SKU Code"></asp:Label></strong>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtskucode" runat="server" Width="198px" CssClass="txtBox "></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px; height: 20px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label7" runat="server" Text="SKU Name"></asp:Label></strong>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtskuname" runat="server" Width="198px" CssClass="txtBox "></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px" align="left">
                                                <strong>
                                                    <asp:Label ID="lblPackSize" runat="server" Text="Shade Code"></asp:Label></strong>
                                            </td>
                                            <td style="width: 37px">
                                                <asp:TextBox ID="txtpacksize" runat="server" Width="198px" CssClass="txtBox "></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>

                                        <tr>
                                            <td style="width: 100px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label12" runat="server" Text="Unit In Case"></asp:Label></strong>
                                            </td>
                                            <td style="width: 37px">
                                                <asp:TextBox ID="txtunitincase" runat="server" Width="198px" CssClass="txtBox "></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px" align="left">
                                                <strong>
                                                    <asp:Label ID="lblweight" runat="server" Text="Weight"></asp:Label>
                                                </strong>
                                            </td>
                                            <td style="width: 37px">
                                                <asp:TextBox ID="txtweight" runat="server" CssClass="txtBox " Width="198px"></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px; height: 12px">
                                                <strong>
                                                    <asp:Label ID="lblUOM" runat="server" Text="UOM"></asp:Label>
                                                </strong>
                                            </td>
                                            <td style="width: 37px; height: 12px">
                                                <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="txtunitincase"
                                                    FilterType="Numbers">
                                                </cc1:FilteredTextBoxExtender>
                                                <asp:DropDownList ID="DrpUOM" runat="server" Width="200px" CssClass="DropList">
                                                    <asp:ListItem Value="K">Kg</asp:ListItem>
                                                    <asp:ListItem Value="L">Ltr</asp:ListItem>
                                                    <asp:ListItem Value="G">Grm</asp:ListItem>
                                                    <asp:ListItem Value="M">Ml</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                            <td style="width: 37px; height: 12px"></td>
                                        </tr>

                                        <tr>
                                            <td style="width: 100px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label5" runat="server" Text="FMR"></asp:Label>
                                                </strong>
                                            </td>
                                            <td style="width: 37px">
                                                <asp:TextBox ID="txtFMR" runat="server" CssClass="txtBox " Width="198px"></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label11" runat="server" Text="MR"></asp:Label>
                                                </strong>
                                            </td>
                                            <td style="width: 37px">
                                                <asp:TextBox ID="txtMR" runat="server" CssClass="txtBox " Width="198px"></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label13" runat="server" Text="Pur. Discount"></asp:Label>
                                                </strong>
                                            </td>
                                            <td style="width: 37px">
                                                <asp:TextBox ID="txtPurchaseDiscount" runat="server" CssClass="txtBox " Width="198px"></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px; height: 12px"></td>
                                            <td style="height: 12px" align="left" colspan="2">
                                                <asp:Button ID="btnSave" OnClick="btnSave_Click" runat="server" Width="86px" Font-Size="8pt"
                                                    Text="Save" CssClass="Button" />
                                            </td>
                                            <td style="width: 37px; height: 12px" align="left"></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                            <ProgressTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" Width="26px" Height="23px" ImageUrl="~/App_Themes/Granite/Images/image003.gif"></asp:ImageButton>&nbsp; Loading....
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </td>
                </tr>
            </table>
        </div>
        <div>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <table class="tblhead" style="width: 100% !important">
                        <tbody>
                            <tr>
                                <td style="color: White; font-weight: bold;">
                                    <asp:Label ID="Label10" runat="server" Width="153px" Text="Select Searching Type"></asp:Label>
                                </td>
                                <td style="width: 170px; height: 22px" align="left">
                                    <asp:DropDownList ID="ddSearchType" runat="server" Width="200px" CssClass="DropList">
                                        <asp:ListItem Value="SKU_code">All Records</asp:ListItem>
                                        <asp:ListItem Value="Principal">Principal</asp:ListItem>
                                        <asp:ListItem Value="Division">Division</asp:ListItem>
                                        <asp:ListItem Value="Category">Category</asp:ListItem>
                                        <asp:ListItem Value="SKU_code">Sku Code</asp:ListItem>
                                        <asp:ListItem Value="SKU_NAME">Sku Name</asp:ListItem>
                                        <asp:ListItem Value="Brand">Brand</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 224px; height: 22px" align="left">
                                    <asp:TextBox ID="txtSeach" runat="server" Width="200px" CssClass="txtBox "></asp:TextBox>
                                </td>
                                <td style="width: 250px; height: 22px" align="left">
                                    <asp:Button ID="btnFilter" runat="server" Width="85px" Font-Size="8pt" Text="Filter"
                                        OnClick="btnFilter_Click"></asp:Button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <asp:Panel ID="Panel1" runat="server" Width="100%" Height="200px" ScrollBars="Vertical" BorderWidth="1px">
                        <asp:GridView ID="grdSKUData" runat="server" Width="100%" ForeColor="SteelBlue" CssClass="gridRow2"
                            OnPageIndexChanging="grdSKUData_PageIndexChanging" HorizontalAlign="Center" BorderColor="White"
                            BackColor="White" AutoGenerateColumns="False">
                            <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                PreviousPageText="Previous"></PagerSettings>
                            <Columns>
                                <asp:BoundField DataField="Principal_Id">
                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                    <ItemStyle CssClass="HidePanel"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="Division_Id">
                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                    <ItemStyle CssClass="HidePanel"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="Category_Id" HeaderText="Category_Id">
                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                    <ItemStyle CssClass="HidePanel"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="sub_Category_Id" HeaderText="sub_Category_Id">
                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                    <ItemStyle CssClass="HidePanel"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="Brand_Id" HeaderText="Brand_Id">
                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                    <ItemStyle CssClass="HidePanel"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="SKU_ID" HeaderText="SKU_ID">
                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                    <ItemStyle CssClass="HidePanel"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="Principal" HeaderText="Principal">
                                    <ItemStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="Division" HeaderText="Division">
                                    <ItemStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="Category" HeaderText="Category">
                                    <ItemStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="sub_Category" HeaderText="Sub Category">
                                    <ItemStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="Brand" HeaderText="Brand">
                                    <ItemStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="SKU_CODE" HeaderText="Code">
                                    <ItemStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="SKU_NAME" HeaderText="Name">
                                    <ItemStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="PACKSIZE" HeaderText="Pack Size">
                                    <ItemStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="UNITS_IN_CASE" HeaderText="UIC">
                                    <ItemStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="GST_ON" HeaderText="GST">
                                    <ItemStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="SKU_Weight" HeaderText="Weight">
                                    <ItemStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="ISEXEMPTED" HeaderText="IsExemted">
                                    <ItemStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                    <ItemStyle CssClass="HidePanel"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="UOM" HeaderText="UOM">
                                    <ItemStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="FMR" HeaderText="FMR" DataFormatString="{0:n4}">
                                    <ItemStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="MR" HeaderText="MR" DataFormatString="{0:n4}">
                                    <ItemStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="Pur_discount" HeaderText="Pur. Dis" DataFormatString="{0:n4}">
                                    <ItemStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"></ItemStyle>
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Edit">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEdit" runat="server" CommandArgument='<%#Eval("SKU_ID")%>' OnClick="btnEdit_Click" ToolTip="Edit">
                                            <img id="imgEdit" alt="" src="~/images/edit.gif" runat="server" />
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" CssClass="grdDetail" Width="3%" Height="14px" BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid" />
                                    <HeaderStyle HorizontalAlign="Center" CssClass="grdHead" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDelete" runat="server" CommandArgument='<%#Eval("SKU_ID")%>' OnClick="btnDelete_Click" ToolTip="Delete" OnClientClick="javascript:return confirm('Are you sure you want to Delete?');return false;">
                                            <img id="imgDelete" alt="" src="~/images/delete.gif" runat="server" />
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle BorderStyle="Solid" BorderWidth="1px" Width="3%" BorderColor="Silver" HorizontalAlign="Center"></ItemStyle>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="tblhead"></HeaderStyle>
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" BackColor="#007395" ForeColor="White" />
                            <AlternatingRowStyle BackColor="#F2F2F2" CssClass="GridAlternateRowStyle" ForeColor="#333333" />
                        </asp:GridView>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
