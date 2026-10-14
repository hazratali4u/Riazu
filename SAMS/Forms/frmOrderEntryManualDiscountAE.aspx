<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmOrderEntryManualDiscountAE.aspx.cs" Inherits="Forms_frmOrderEntryManualDiscountAE" Title="SAMS: Order/Invoice  Manual Discount Step 2" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script type="text/javascript" src="../AjaxLibrary/jquery.searchabledropdown-1.0.8.min.js"></script>
    <script language="JavaScript" type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(startRequest);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequest);
        function startRequest(sender, e) {
            document.getElementById('<%=btnSave.ClientID%>').disabled = true;
            document.getElementById('<%=btnCalculate.ClientID%>').disabled = true;
        }

        function endRequest(sender, e) {
            document.getElementById('<%=btnSave.ClientID%>').disabled = false;
            document.getElementById('<%=btnCalculate.ClientID%>').disabled = false;
        }

          function calpercntDiscount()
        {
            var total = document.getElementById("<%=  txtGrossAmount.ClientID  %>").value;
            var percent = document.getElementById("<%=  numtxtDiscountage.ClientID %>").value;
            var discount =Math.round (total * percent) / 100;
            document.getElementById("<%=  numtxtDiscount.ClientID %>").value = discount;
            return true;
        }
        function ValidateForm() {
            var str;
            str = document.getElementById('<%=txtQuantity.ClientID%>').value;
            str1 = document.getElementById('<%=txtQuantityCtn.ClientID%>').value;
            if (str1 == null || str1.length == 0) {
                if (str == null || str.length == 0) {
                    alert('Must Enter Order Quantity');
                    return false;
                }
            }

            return true;
        }

        function pageLoad() {
            $("select").searchable();
            var Str = $('#<%=drpskuDetail.ClientID %> option:selected').text();
            var Detail = Str.split(':');
            var unitprice = Detail[1];

            document.getElementById("<%= txtUnitRate.ClientID %>").value = unitprice;


            $('#<%=drpskuDetail.ClientID %>').change(function () {

                var Str = $('#<%=drpskuDetail.ClientID %> option:selected').text();
                var Detail = Str.split(':');
                var unitprice = Detail[1];

                document.getElementById("<%= txtUnitRate.ClientID %>").value = unitprice;

            });


        }
    </script>
    <div id="right_data">
        <div>
            <span class="heading">Order/Invoice Step 2</span>
        </div>
        <div>
            <table width="100%">
                <tr>
                    <td align="left">


                        <div style="z-index: 101; left: 612px; width: 100px; position: absolute; top: 369px; height: 100px">
                            &nbsp;<asp:Panel ID="Panel21" runat="server">
                                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel3">
                                    <ProgressTemplate>
                                        <asp:ImageButton ID="ImageButton1" runat="server" Height="28px" ImageUrl="~/App_Themes/Granite/Images/image003.gif"
                                            Width="31px" />
                                        Wait Update
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                            </asp:Panel>
                        </div>
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tbody>
                                        <tr>
                                            <td valign="top" align="left">
                                                <strong>
                                                    <asp:Label ID="lblDocumentNo" runat="server" Width="74px" Text="Order No" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td valign="top" align="left" colspan="1">
                                                <asp:DropDownList ID="drpDocumentNo" runat="server" Width="231px" CssClass="DropList"
                                                    OnSelectedIndexChanged="drpDocumentNo_SelectedIndexChanged" AutoPostBack="True">
                                                </asp:DropDownList>
                                                <strong></td>
                                            <td colspan="2">
                                                <asp:Label ID="lblBillBook" TabIndex="1" runat="server" Text="Bill Book No:" CssClass="lblbox"></asp:Label></strong>
                                                <asp:TextBox ID="txtBillBookNo" runat="server" CssClass="uppercase" Width="110px" MaxLength="10"></asp:TextBox>
                                                <asp:CheckBox ID="ChbDiscount" runat="server" Visible="False" Width="90px" Text="Promotion"
                                                    AutoPostBack="True" Checked="True"></asp:CheckBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="middle" align="left">
                                                <strong>
                                                    <asp:Label ID="Label11" runat="server" Width="77px" Text="Order Type" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td valign="top" align="left" colspan="2" style="width: 113px">
                                                <asp:RadioButtonList ID="RblPayMode" runat="server" Width="226px" Height="1px"
                                                    RepeatDirection="Horizontal">
                                                    <asp:ListItem Value="214">Cash</asp:ListItem>
                                                    <asp:ListItem Selected="True" Value="215">Credit</asp:ListItem>
                                                    <asp:ListItem Value="216">Advance</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                            <td valign="top" align="left" colspan="1">
                                                
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="middle" align="left">
                                                <strong>
                                                    <asp:Label ID="Label12" runat="server" Width="101px" Text="Route/Saleman" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td valign="top" align="left" colspan="2" style="width: 113px">
                                                <asp:TextBox ID="txtprincipal" runat="server" Width="231px"
                                                    CssClass="txtBox " ReadOnly="True"></asp:TextBox>
                                            </td>
                                            <td valign="top" align="left" colspan="1">
                                                <asp:TextBox ID="txtDeliveryMan" runat="server" Width="180px"
                                                    CssClass="txtBox " ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 23px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label1" runat="server" Width="74px" Text="Customer" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="height: 23px; width: 113px;" align="left" colspan="2">
                                                <asp:DropDownList ID="drpcustomer" OnSelectedIndexChanged="drpcustomer_SelectedIndexChanged" AutoPostBack="true" runat="server" Width="100%"></asp:DropDownList>
                                            </td>
                                            <td style="height: 23px" align="left">
                                                <asp:TextBox ID="txtDiscountType" runat="server" Width="180px"
                                                    CssClass="txtBox " ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                      <%--  <tr>
                                            <td align="left"></td>
                                            <td valign="top" align="left" colspan="2" style="width: 113px"></td>
                                            <td valign="top" align="left" colspan="1"></td>
                                        </tr>--%>
                                    </tbody>
                                </table>
                               
                            </ContentTemplate>
                        </asp:UpdatePanel>
                      
                    </td>
                    <td style="height: 126px"></td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Panel ID="Panel5" runat="server" DefaultButton="btnSave">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <table>
                                        <tr>
                                            <td style="height: 16px" colspan="2">
                                                <asp:Label ID="lblskuCode" runat="server" Width="100%" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="  SKU Detail" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>
                                            <td style="height: 16px">
                                                <asp:Label ID="Label3" runat="server" BackColor="#006699" CssClass="lblbox" Font-Bold="True"
                                                    ForeColor="White" Height="16px" Text="Qty Ctn" Width="61px"></asp:Label>
                                            </td>
                                            <td style="height: 16px">
                                                <asp:Label ID="Label6" runat="server" BackColor="#006699" CssClass="lblbox" Font-Bold="True"
                                                    ForeColor="White" Height="16px" Text="Qty Units" Width="61px"></asp:Label>
                                            </td>
                                            <td style="height: 16px">
                                                <asp:Label ID="lblquantity" runat="server" Width="62px" Height="16px" ForeColor="White"
                                                    Font-Bold="True" Text="Unit Price" CssClass="lblbox" BackColor="#006699"></asp:Label>
                                            </td>
                                            <td align="center" style="height: 16px">
                                                <asp:Label ID="Label7" runat="server" BackColor="#006699" CssClass="lblbox" Font-Bold="True"
                                                    ForeColor="White" Height="16px" Text="Discount(%)" Width="100%" Enabled="False"></asp:Label>
                                            </td>
                                            <td align="center" style="height: 16px">
                                                <asp:Label ID="Label20" runat="server" BackColor="#006699" Font-Bold="True"
                                                    ForeColor="White" Height="16px" Text="T.O" Width="100%" Enabled="False"></asp:Label>
                                            </td>
                                            <td style="height: 16px">
                                                <asp:Label ID="Label2" runat="server" BackColor="#006699" CssClass="lblbox" Font-Bold="True"
                                                    ForeColor="White" Height="16px" Text="Amount" Width="100%"></asp:Label>
                                            </td>
                                            <td style="height: 16px">
                                                <asp:Label ID="Label4" runat="server" BackColor="#006699" CssClass="lblbox" Font-Bold="True"
                                                    ForeColor="White" Height="16px" Text="Add SKU" Width="100%"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:DropDownList ID="drpskuDetail" runat="server" Width="100%"></asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtQuantityCtn" runat="server" CssClass="txtBox " onfocus="SearchSKUCode()" Width="55px"></asp:TextBox>
                                                <ajaxToolkit:FilteredTextBoxExtender runat="server" TargetControlID="txtQuantityCtn" ValidChars="0123456789"></ajaxToolkit:FilteredTextBoxExtender>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtQuantity" runat="server" CssClass="txtBox " onfocus="SearchSKUCode()" Width="55px"></asp:TextBox>
                                                <ajaxToolkit:FilteredTextBoxExtender runat="server" TargetControlID="txtQuantity" ValidChars="0123456789"></ajaxToolkit:FilteredTextBoxExtender>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtUnitRate" runat="server" Width="56px" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDiscount" runat="server" Width="62px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTO" runat="server" Width="62px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox1" runat="server" CssClass="txtBoxnNum" Enabled="False" onfocus="SearchSKUCode()"
                                                    Width="72px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnSave" runat="server" Font-Size="8pt" OnClick="btnSave_Click" Text="Add Sku"
                                                    ValidationGroup="vg" Width="100px" AccessKey="A" CssClass="Button" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" colspan="9">
                                                <asp:Panel ID="Panel2" runat="server" Height="130px" ScrollBars="Vertical" Width="906px"
                                                    BorderColor="Silver" BorderStyle="Groove" BorderWidth="1px">
                                                    <asp:GridView ID="GrdPurchase" runat="server" ForeColor="SteelBlue" CssClass="gridRow2" Width="100%"
                                                        BackColor="White" HorizontalAlign="Center" AutoGenerateColumns="False" BorderColor="White"
                                                        ShowHeader="False" OnRowDeleting="GrdPurchase_RowDeleting" OnRowEditing="GrdPurchase_RowEditing">
                                                        <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                                            PreviousPageText="Previous"></PagerSettings>
                                                        <Columns>
                                                            <asp:BoundField DataField="SKU_ID" ReadOnly="true" HeaderText="SKU_ID">
                                                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SKU_CODE" ReadOnly="true" HeaderText="SKU Code">
                                                                <ItemStyle HorizontalAlign="Left" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid"
                                                                    Width="75px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SKU_NAME" ReadOnly="true" HeaderText="SKU Name">
                                                                <ItemStyle HorizontalAlign="Left" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid"
                                                                    Width="213px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="QUANTITY_CTN" ReadOnly="true" HeaderText="QuantityCtn">
                                                                <ItemStyle HorizontalAlign="Right" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid"
                                                                    Width="65px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="QUANTITY_UNIT" ReadOnly="true" HeaderText="Quantity">
                                                                <ItemStyle HorizontalAlign="Right" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid"
                                                                    Width="65px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="UNIT_PRICE" ReadOnly="true" HeaderText="PRICE" DataFormatString="{0:F4}">
                                                                <ItemStyle HorizontalAlign="Right" BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid"
                                                                    Width="65px"></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="STANDARD_DISCOUNT_PER" ReadOnly="true" HeaderText="STANDARD_DISCOUNT_PER">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" Width="70px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="EXTRA_DISCOUNT" ReadOnly="true" HeaderText="STANDARD_DISCOUNT_PER">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" Width="70px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Amount" ReadOnly="true" HeaderText="Amount" DataFormatString="{0:F4}">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="2px" HorizontalAlign="Right"
                                                                    Width="80px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="QUANTITY_CTN" ReadOnly="true" HeaderText="QUANTITY_CTN">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:TemplateField HeaderText="Edit">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="btnEdit" runat="server" Text="Edit" CommandName="Edit"></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid" Width="45px"></ItemStyle>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Delete">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="btnDelete" runat="server" Text="Delete" OnClientClick="javascript:return confirm('Are you sure you want to Delete?');return false;"
                                                                        CommandName="Delete"></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid" Width="45px"></ItemStyle>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:Panel>
                    </td>
                    <td style="width: 100px">
                        <asp:HiddenField ID="hfBillBookNo" runat="server" />
                        <asp:HiddenField ID="hfCustomerAreaID" runat="server" />
                        <asp:HiddenField ID="hfWHTax" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
        <div>
            <table width="100%">
                <tr>
                    <td align="left">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tbody>
                                        <tr>
                                            <td valign="top" align="left" colspan="2" rowspan="7">
                                                <strong>
                                                    <asp:Label ID="Label10" runat="server" Width="60px" Text="Free SKU" CssClass="lblbox"></asp:Label></strong>
                                                <asp:Panel
                                                        ID="Panel4" runat="server" Width="350px" Height="90px" BorderColor="Silver" BorderStyle="Groove"
                                                        BorderWidth="1px">
                                                        <asp:GridView ID="GrdFreeSKU" runat="server" Width="100%" ForeColor="Silver" CssClass="gridRow2"
                                                            BorderColor="White" BackColor="White" AutoGenerateColumns="False" HorizontalAlign="Center">
                                                            <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                                                PreviousPageText="Previous"></PagerSettings>
                                                            <RowStyle BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid" ForeColor="Black"></RowStyle>
                                                            <Columns>
                                                                <asp:BoundField DataField="SKU_ID" HeaderText="SKU_ID">
                                                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                                    <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="SKU_Code" HeaderText="SKU Code">
                                                                    <ItemStyle Width="80px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"></ItemStyle>
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="SKU_Name" HeaderText="SKU Name">
                                                                    <ItemStyle Width="200px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"></ItemStyle>
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Quantity" HeaderText="Qty">
                                                                    <ItemStyle Width="50px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"></ItemStyle>
                                                                </asp:BoundField>
                                                            </Columns>
                                                            <HeaderStyle CssClass="tblhead"></HeaderStyle>
                                                        </asp:GridView>
                                                    </asp:Panel>
                                            </td>
                                            <td style="height: 20px" align="left"></td>
                                            <td style="height: 20px" align="left">
                                                <strong>
                                                    <asp:Label ID="lblDocumentNo1" runat="server" Width="94px" Text="Gross Sale" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px">
                                                <asp:TextBox ID="txtGrossAmount" runat="server" Width="150px" ForeColor="Black" Font-Bold="True"
                                                    CssClass="txtBoxnNum" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                    
                                        <tr>
                                            <td align="left"></td>
                                            <td align="left">
                                                <strong>
                                                    <asp:Label ID="Label16" runat="server" Width="80px" Text="Incentive" CssClass="lblbox"></asp:Label>
                                                </strong>


                                            </td>
                                            <td style="width: 7px">
                                                
                                                  <asp:TextBox ID="txtINcentive" runat="server" Width="150px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum" ></asp:TextBox>
                                            </td>
                                           
                                           <td>
                                              <strong>
                                                    <asp:Label ID="Label19" runat="server" Width="80px" Text="FOC Value" CssClass="lblbox"></asp:Label></strong>
                                             </td>
                                             <td style="width: 7px; height: 18px">
                                                <asp:TextBox ID="txtBRDDiscount" runat="server" Width="150px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum" ></asp:TextBox>
                                            </td>

                                        </tr>
                                        <tr>  <td style="height: 18px" align="left"></td>
                                            <td style="height: 18px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label51" runat="server" Width="130px" Text="Damage Value"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 18px">
                                                <asp:TextBox ID="numTxtTotalStndrdDiscnt" runat="server" Width="150px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum" ></asp:TextBox>
                                            </td>
                                            <td>
                                              <strong>
                                                    <asp:Label ID="Label13" runat="server" Width="80px" Text="VBRD" CssClass="lblbox"></asp:Label></strong>
                                             </td>
                                             <td style="width: 7px; height: 18px">
                                                <asp:TextBox ID="txtVBRDDiscount" runat="server" Width="150px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum" ></asp:TextBox>
                                            </td>

                                        </tr>
                                        <tr>
                                           <td style="height: 18px" align="left"></td>
                                            <td style="height: 18px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label14" runat="server" Width="80px" Text="Trade Offer" CssClass="lblbox"></asp:Label>
                                                </strong>
                                             </td>
                                            <td style="width: 7px; height: 18px">
                                                <asp:TextBox ID="txtTradeOfferDisc" runat="server" Width="150px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum" ></asp:TextBox>
                                            </td>
                                             <td>
                                              <strong>
                                                    <asp:Label ID="Label15" runat="server" Width="80px" Text="Display" CssClass="lblbox"></asp:Label></strong>
                                             </td>
                                             <td style="width: 7px; height: 18px">
                                                <asp:TextBox ID="txtDisplayDiscount" runat="server" Width="150px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum" ></asp:TextBox>
                                            </td>
                                        </tr>
                                         <tr>
                                           <td style="height: 18px" align="left"></td>
                                            <td style="height: 18px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label8" runat="server" Width="116px" Text="Claimable  Discount" CssClass="lblbox"></asp:Label></strong>
                                          
                                             </td>
                                            <td style="width: 7px; height: 18px">
                                                <asp:TextBox ID="numtxtTotalExtraDiscnt" runat="server" Width="150px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum" ReadOnly="True" Enabled="false"></asp:TextBox>
                                            </td>
                                             <td>
                                              <strong>
                                                    <asp:Label ID="Label17" runat="server" Width="80px" Text="Rental" CssClass="lblbox"></asp:Label></strong>
                                             </td>
                                             <td style="width: 7px; height: 18px">
                                                <asp:TextBox ID="txtRentalDiscount" runat="server" Width="150px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum" ></asp:TextBox>
                                            </td>
                                        </tr>
                                             <tr>
                                            <td style="width: 1px; height: 20px" valign="top"></td>
                                            <td style="width: 1px; height: 20px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label18" runat="server" CssClass="lblbox" Text="Discount in %/value"
                                                        Width="116px"></asp:Label>
                                                </strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="right">
                                                <asp:RadioButtonList ID="rdbDiscountType2"  OnSelectedIndexChanged="rdbDiscountType_SelectedIndexChanged"  runat="server"   AutoPostBack="true" RepeatDirection="Horizontal">
                                                    <asp:ListItem  Value="1" Selected="True" >Value</asp:ListItem>
                                                    <asp:ListItem  Value="2"  >%age</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                        </tr>
                                        <tr>
                                             <td style="width: 1px; height: 20px" valign="top"></td>
                                            <td style="width: 1px; height: 20px" align="left">
                                            <strong>
                                                    <asp:Label ID="lblfromLocation1" runat="server" Width="150px" Text="Unclaimable Discount"
                                                        CssClass="lblbox"></asp:Label></strong>
                                                </td>
                                               <td style="width: 7px">
                                             <asp:TextBox ID="numtxtUnClaimabledist" runat="server" Width="150px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum" ></asp:TextBox>
                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender6" runat="server"
                                                    FilterType="Custom" ValidChars="0123456789." TargetControlID="numtxtUnClaimabledist" />
                                                <asp:TextBox ID="numtxtDiscountage" runat="server" Font-Size="11pt" placeholder="%age" Width="50px" Visible="false" onkeyup="return calpercntDiscount()"></asp:TextBox>
                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender5" runat="server"
                                                    FilterType="Custom" ValidChars="0123456789." TargetControlID="numtxtDiscountage" />
                                                <asp:TextBox ID="numtxtDiscount" runat="server" Font-Size="11pt" placeholder="Value"
                                                    Width="85px" Visible="false" Enabled="false"></asp:TextBox>
                                                   </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 1px; height: 20px" valign="top" colspan="3"></td>
                                            <td style="width: 1px; height: 20px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label81" runat="server" Width="93px" Text="GST Amount" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="right">
                                                <asp:TextBox ID="numTxtTotalGST" runat="server" Width="150px" ForeColor="Black" Font-Bold="True"
                                                    CssClass="txtBoxnNum" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 1px; height: 20px" valign="top" colspan="3"></td>
                                            <td style="width: 1px; height: 20px" align="left" >
                                                <strong>
                                                    <asp:Label ID="Label9" runat="server" Width="93px" Text="TST Amount" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="right">
                                                <asp:TextBox ID="numTxtTotalTST" runat="server" Width="150px" ForeColor="Black" Font-Bold="True"
                                                    CssClass="txtBoxnNum" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 1px; height: 20px" valign="top" colspan="3"></td>
                                            <td style="width: 1px; height: 20px" align="left" >
                                                <strong>
                                                    <asp:Label ID="Label21" runat="server" Width="93px" Text="Extra Tax" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="right">
                                                <asp:TextBox ID="numTxtTotalExtra_tax" runat="server" Width="150px" ForeColor="Black" Font-Bold="True"
                                                    CssClass="txtBoxnNum" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 1px; height: 20px" valign="top" colspan="3"></td>
                                            <td style="width: 1px; height: 20px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label91" runat="server" Width="105px" Text="Net Amount" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px; height: 20px" align="right">
                                                <asp:TextBox ID="numTxtTotlAmnt" runat="server" Width="150px" ForeColor="Black" Font-Bold="True"
                                                    CssClass="txtBoxnNum" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" colspan="2">
                                                <table>
                                                    <tbody>
                                                        <tr>
                                                            <td style="width: 100px">
                                                                <asp:Button AccessKey="C" ID="btnCalculate" TabIndex="100" OnClick="btnCalculate_Click"
                                                                    runat="server" Width="100px" Font-Size="8pt" Text="Calculate" Enabled="False"
                                                                    CssClass="Button" />
                                                            </td>
                                                            <td style="width: 100px">
                                                                <asp:Button AccessKey="S" ID="btnSaveOrder" TabIndex="101" OnClick="btnSaveOrder_Click"
                                                                    runat="server" Width="110px" Font-Size="8pt" Text="Save Order" Enabled="False"
                                                                    CssClass="Button" />
                                                            </td>
                                                            <td style="width: 100px">
                                                                <asp:Button AccessKey="H" ID="btnCancel" TabIndex="102" runat="server" Width="110px"
                                                                    Font-Size="8pt" Text="Home" OnClick="btnCancel_Click" CssClass="Button" />
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </td>
                                            <td style="width: 1px" valign="top" align="left">&nbsp; &nbsp; &nbsp;&nbsp;
                                            </td>
                                            <td style="width: 1px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label5" runat="server" Width="105px" Text="Cash Received" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 7px" align="right">
                                                <asp:TextBox ID="txtCashReceived" runat="server" Width="150px" ForeColor="Black"
                                                    Font-Bold="True" CssClass="txtBoxnNum" Enabled="False"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        &nbsp;&nbsp;
                        <asp:TextBox ID="numTxtTotalSED" runat="server" CssClass="txtBox " Font-Bold="False"
                            ForeColor="Black" Width="139px" ReadOnly="True" Visible="False"></asp:TextBox>&nbsp;&nbsp;
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
