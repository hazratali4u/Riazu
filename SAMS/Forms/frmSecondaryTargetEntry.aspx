<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmSecondaryTargetEntry.aspx.cs" Inherits="Forms_frmSecondaryTargetEntry" Title="SAMS: Target Entry" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script language="JavaScript" type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(startRequest);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequest);
        function startRequest(sender, e) {
            document.getElementById('<%=btnTarget.ClientID%>').disabled = true;
        }
        function endRequest(sender, e) {
            document.getElementById('<%=btnTarget.ClientID%>').disabled = false;
        }

        
    </script>
    <div id="right_data">
        <div>
            <table width="100%">
                <tr>
                    <td>
                        <asp:UpdatePanel ID="upSearchResults" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tbody>
                                                <tr>
                                    <td colspan ="4">
                                    
                                    <asp:Label ID="lblerror" runat="server" CssClass="lblbox" Height="14px" Text=""
                                                   Visible="false"  Width="98px"></asp:Label>
                                                   <asp:Label ID="lblSkiId" runat="server" CssClass="lblbox" Height="14px" Text=""
                                                   Visible="false"  Width="98px"></asp:Label>
                                    </td></tr>
                                        

                                        <tr >
                                            <td align="left" style="height: 25px">
                                                <strong>
                                                    <asp:Label ID="lblLocation" runat="server" CssClass="lblbox" Height="14px" Text="Location"
                                                        Width="98px"></asp:Label></strong>
                                            </td>
                                            <td style="height: 25px" align="left">
                                                <asp:DropDownList ID="drpDistributor" runat="server" CssClass="DropList" Width="212px"
                                                    OnSelectedIndexChanged="drpDistributor_SelectedIndexChanged" AutoPostBack="True">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="height: 25px">
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                        <tr >
                                            <td align="left" style="height: 25px">
                                                <strong>
                                                    <asp:Label ID="lblprincipal" runat="server" Width="71px" Text="Principal" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="height: 25px" align="left">
                                                <asp:DropDownList ID="drpPrincipal" runat="server" Width="211px" CssClass="DropList"
                                                    AutoPostBack="True" OnSelectedIndexChanged="drpPrincipal_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="height: 25px">
                                            </td>
                                            <td></td>
                                        </tr>
                                    
                                        <tr>
                                        
                                            <td align="left" style="height: 25px">
                                                <strong>
                                                    <asp:Label ID="Label1" runat="server" Width="72px" Text="For Month" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="height: 25px" align="left">
                                                <asp:TextBox ID="txtFromdate" runat="server" Width="204px" CssClass="txtBox" MaxLength="1"></asp:TextBox>
                                            </td>
                                            <td style="height: 25px">
                                                <asp:ImageButton ID="ibtnStartDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                                    Width="16px" />
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="height: 25px">
                                                <strong>
                                                    <asp:Label ID="lbltargetvalue" runat="server" CssClass="lblbox" Text="Growth %"
                                                        Width="97px"></asp:Label></strong>
                                            </td>
                                            <td align="left" style="height: 30px">
                                                <asp:TextBox ID="txtGrowthMaster" runat="server" AutoPostBack="true" OnTextChanged="txtGrowthMaster_TextChanged"
                                                     CssClass="txtBox" Width="204px">0</asp:TextBox>
                                            </td>
                                            <td style="height: 25px">
                                            </td>
                                            <td></td>
                                        </tr>


                                        <tr>
                                            <td align="left" style="height: 25px">
                                                <strong>
                                                    <asp:Label ID="Label2" runat="server" CssClass="lblbox" Text="Working Days"
                                                        Width="97px"></asp:Label></strong>
                                            </td>
                                            <td align="left" style="height: 30px">
                                                <asp:TextBox ID="txtWorkingDays" runat="server" CssClass="txtBox" Width="204px">0</asp:TextBox>
                                            </td>
                                            <td style="height: 25px">
                                            </td>
                                            <td style="width:100px;"></td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="height: 25px" valign="top">
                                            </td>
                                            <td colspan="3" style="width: 1px; height: 25px;" valign="top">
                                                <%@register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
                                                
                                                <cc1:CalendarExtender ID="CEStartDate" runat="server" Format="MMM-yyyy" PopupButtonID="ibtnStartDate"
                                                    TargetControlID="txtFromdate">
                                                </cc1:CalendarExtender>
                                                <asp:Button ID="BtnGetData" runat="server" Font-Size="8pt" Text="Get Data" 
                                                   Width="80px" OnClick="BtnGetData_Click" CssClass="Button" />
                                                
                                                <asp:Button ID="btnCalculate" runat="server" Font-Size="8pt" Text="Calculate" 
                                                   Width="80px" OnClick="btnCalculate_Click" CssClass="Button" />

                                                  <asp:Button ID="btnTarget" runat="server" Font-Size="8pt" Text="Save"
                                                    Width="80px" OnClick="btnTarget_Click" CssClass="Button" />
                                            </td>
                                            
                                            
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
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table width="100%">
                                    <tr>
                                        <td colspan="6">
                                            <asp:Panel ID="pnlgrdSkuDetail" runat="server" BorderColor="Silver" BorderStyle="Groove"
                                                BorderWidth="1px" Height="350px" ScrollBars="Vertical" Width="856px">
                                                <asp:GridView ID="GrdSkuDetail" runat="server" AutoGenerateColumns="False" BackColor="White"
                                                    BorderColor="White" CssClass="gridRow2" ForeColor="SteelBlue" HorizontalAlign="Center"
                                                    Width="100%" ShowHeader="true" >
                                                    
                                                    <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                                        PreviousPageText="Previous" />
                                                    <Columns>
                                                        <asp:BoundField DataField="SKU_ID" HeaderText="SKU_ID">
                                                            <HeaderStyle CssClass="HidePanel" />
                                                            <ItemStyle CssClass="HidePanel" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="SKU_weight" HeaderText="SKU_WEIGHT">
                                                            <HeaderStyle CssClass="HidePanel" />
                                                            <ItemStyle CssClass="HidePanel" />
                                                        </asp:BoundField>
                                                  <asp:BoundField DataField="Unit_Price" HeaderText="Unit_Price">
                                                            <HeaderStyle CssClass="HidePanel" />
                                                            <ItemStyle CssClass="HidePanel" />
                                                        </asp:BoundField>
                                                      
                                                          <asp:BoundField DataField="SKU_Code" HeaderText="Sku Code">
                                                            <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Left"
                                                                Width="105px" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="SKU_Name" HeaderText="Description">
                                                            <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Left"
                                                                Width="205px" />
                                                        </asp:BoundField>
                                                         <asp:TemplateField HeaderText="Sale">
                                                        <ItemTemplate>
                                                             <asp:TextBox ID="txtPurchaseQty" Width="80px" Text='<%# Eval("Purchase") %>' runat="server"></asp:TextBox>
                                                         </ItemTemplate>
                                                        </asp:TemplateField>
                                                        
                                                       <asp:TemplateField HeaderText="Growth %">
                                                        <ItemTemplate>
                                                             <asp:TextBox ID="txtGrowth" Width="80px" Text='<%# Eval("Growth") %>' runat="server"></asp:TextBox>
                                                         </ItemTemplate>
                                                        </asp:TemplateField>
                                                         <asp:TemplateField HeaderText="Target Qty">
                                                        <ItemTemplate>
                                                             <asp:TextBox ID="txtTargetQty" ReadOnly="true" Width="80px" Text='<%# Eval("targetqty") %>' runat="server"></asp:TextBox>
                                                         </ItemTemplate>
                                                        </asp:TemplateField>

                                                         <asp:TemplateField HeaderText="Weight">
                                                        <ItemTemplate>
                                                             <asp:TextBox ID="txtWeight" Width="100px" ReadOnly="true" Text='<%# Eval("weight") %>' runat="server"></asp:TextBox>
                                                         </ItemTemplate>
                                                        </asp:TemplateField>


                                                         <asp:TemplateField HeaderText="Value">
                                                        <ItemTemplate>
                                                             <asp:TextBox ID="txtValue" Width="100px" ReadOnly="true" Text='<%# Eval("value") %>'  runat="server"></asp:TextBox>
                                                         </ItemTemplate>
                                                        </asp:TemplateField>

                                                    </Columns>
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="tblhead" />
                                                </asp:GridView>
                                            </asp:Panel>
                                        </td>
                                      
                                    </tr>
                                   <tr><td colspan="6">
                                        <asp:Panel ID="pnlTargetview" runat="server" BorderColor="Silver" BorderStyle="Groove"
                                                BorderWidth="1px" Height="350px" ScrollBars="Vertical" Width="856px">
                                                <asp:GridView ID="GrdTargetView" runat="server" AutoGenerateColumns="False" BackColor="White"
                                                    BorderColor="White" CssClass="gridRow2" ForeColor="SteelBlue" HorizontalAlign="Center"
                                                    Width="100%" ShowHeader="true" >
                                                    
                                                    <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                                        PreviousPageText="Previous" />
                                                    <Columns>
                                                        <asp:BoundField DataField="TARGET_ID" HeaderText="TARGET_ID">
                                                            <HeaderStyle CssClass="HidePanel" />
                                                            <ItemStyle CssClass="HidePanel" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="SKU_ID" HeaderText="SKU_ID">
                                                            <HeaderStyle CssClass="HidePanel" />
                                                            <ItemStyle CssClass="HidePanel" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="SKU_weight" HeaderText="SKU_WEIGHT">
                                                            <HeaderStyle CssClass="HidePanel" />
                                                            <ItemStyle CssClass="HidePanel" />
                                                        </asp:BoundField>
                                                  <asp:BoundField DataField="Unit_Price" HeaderText="Unit_Price">
                                                            <HeaderStyle CssClass="HidePanel" />
                                                            <ItemStyle CssClass="HidePanel" />
                                                        </asp:BoundField>
                                                      
                                                          <asp:BoundField DataField="SKU_Code" HeaderText="Sku Code">
                                                            <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Left"
                                                                Width="105px" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="SKU_Name" HeaderText="Description">
                                                            <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Left"
                                                                Width="205px" />
                                                        </asp:BoundField>
                                                         <asp:TemplateField HeaderText="W.Days">
                                                        <ItemTemplate>
                                                             <asp:TextBox ID="txtWDays" Width="80px" Text='<%# Eval("WORKING_DAYS") %>' runat="server"></asp:TextBox>
                                                         </ItemTemplate>
                                                        </asp:TemplateField>
                                                       
                                                          <asp:TemplateField HeaderText="Sale">
                                                        <ItemTemplate>
                                                             <asp:TextBox ID="txtPurchaseQty" Width="80px" Text='<%# Eval("Purchase") %>' runat="server"></asp:TextBox>
                                                         </ItemTemplate>
                                                        </asp:TemplateField>
                                                        
                                                       <asp:TemplateField HeaderText="Growth %">
                                                        <ItemTemplate>
                                                             <asp:TextBox ID="txtGrowth" Width="80px" Text='<%# Eval("Growth") %>' runat="server"></asp:TextBox>
                                                         </ItemTemplate>
                                                        </asp:TemplateField>
                                                         <asp:TemplateField HeaderText="Target Qty">
                                                        <ItemTemplate>
                                                             <asp:TextBox ID="txtTargetQty" ReadOnly="true" Width="80px" Text='<%# Eval("targetqty") %>' runat="server"></asp:TextBox>
                                                         </ItemTemplate>
                                                        </asp:TemplateField>

                                                         <asp:TemplateField HeaderText="Weight">
                                                        <ItemTemplate>
                                                             <asp:TextBox ID="txtWeight" Width="100px" ReadOnly="true" Text='<%# Eval("weight") %>' runat="server"></asp:TextBox>
                                                         </ItemTemplate>
                                                        </asp:TemplateField>


                                                         <asp:TemplateField HeaderText="Value">
                                                        <ItemTemplate>
                                                             <asp:TextBox ID="txtValue" Width="100px" ReadOnly="true" Text='<%# Eval("value") %>'  runat="server"></asp:TextBox>
                                                         </ItemTemplate>
                                                        </asp:TemplateField>

                                                    </Columns>
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="tblhead" />
                                                </asp:GridView>
                                            </asp:Panel>



                                       </td></tr>
                                </table>
                               
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
            
        </div>
    </div>
</asp:Content>
