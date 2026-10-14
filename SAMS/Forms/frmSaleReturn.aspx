<%@ Page Title="CORN :: Sales Return" Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmSaleReturn.aspx.cs" Inherits="Forms_frmSaleReturn" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHeadPage" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphPage" runat="Server">
    <script type="text/javascript" language="JavaScript">


        //--------------------On Cash Recieved --------------------------------------------\\
        function CalculateBalance(e) {
            var key = e.charCode ? e.charCode : e.keyCode ? e.keyCode : 0;
            if (key == 13) {
                e.preventDefault();
                var disc2 = 0;

                var cashRcd = document.getElementById('<%=txtCashReturn.ClientID%>').value;
                var srFreght = 0;<%--document.getElementById('<%=txtSaleRetFreight.ClientID%>').value;--%>
                var netAmount = document.getElementById('<%=txtNetReturn.ClientID%>').value;
                var disc = document.getElementById('<%=txtDiscountReturn.ClientID%>').value;

                disc2 = document.getElementById('<%=txtExtraDiscountReturn.ClientID%>').value;
                if (disc2 == "") {
                    disc2 = 0;
                }
                if (cashRcd == "") {
                    cashRcd = 0;
                }
                if (srFreght == "") {
                    srFreght = 0;
                }
                srFreght = parseFloat(srFreght);
                cashRcd = parseFloat(cashRcd);
                netAmount = parseFloat(netAmount);
                var balce = 0;

                var radioButtonlist = 0;<%-- document.getElementById('<%=rbFreightType.ClientID%>');--%>
                var inputs = radioButtonlist.getElementsByTagName('input');
                var flag = false;
                var selected;

                for (var i = 0; i < inputs.length; i++) {
                    if (inputs[i].checked) {
                        selected = inputs[i];
                        flag = true;
                        break;
                    }
                }
                if (flag) {
                    if (selected.value == 0) {

                        document.getElementById('<%=txtBalanceReturn.ClientID%>').value = parseFloat(netAmount - srFreght - cashRcd).toFixed(2);
                    }
                    else {

                        document.getElementById('<%=txtBalanceReturn.ClientID%>').value = parseFloat(netAmount + srFreght - cashRcd).toFixed(2);

                    }
                }


                document.getElementById('<%=btnSaveOrder.ClientID%>').focus();
            }
        }
        function CalculateBalancee() {
            var disc2 = 0;
            var cashRcd = document.getElementById('<%=txtCashReturn.ClientID%>').value;
            var srFreght = 0; <%--document.getElementById('<%=txtSaleRetFreight.ClientID%>').value;--%>
            var netAmount = document.getElementById('<%=txtNetReturn.ClientID%>').value;
            var disc = document.getElementById('<%=txtDiscountReturn.ClientID%>').value;

            disc2 = document.getElementById('<%=txtExtraDiscountReturn.ClientID%>').value;
            if (disc2 == "") {
                disc2 = 0;
            }
            if (cashRcd == "") {
                cashRcd = 0;
            }
            if (srFreght == "") {
                srFreght = 0;
            }
            var balce = 0;
            srFreght = parseFloat(srFreght);
            cashRcd = parseFloat(cashRcd);

            netAmount = parseFloat(netAmount);

            var radioButtonlist = 0;<%-- document.getElementById('<%=rbFreightType.ClientID%>');--%>
            var inputs = radioButtonlist.getElementsByTagName('input');
            var flag = false;
            var selected;

            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].checked) {
                    selected = inputs[i];
                    flag = true;
                    break;
                }
            }
            if (flag) {
                if (selected.value == 0) {

                    document.getElementById('<%=txtBalanceReturn.ClientID%>').value = parseFloat(netAmount - srFreght - cashRcd).toFixed(2);
                }
                else {

                    document.getElementById('<%=txtBalanceReturn.ClientID%>').value = parseFloat(netAmount + srFreght - cashRcd).toFixed(2);

                }
            }

        }
        //---------------------------------------------------------------------------------\\

        //-------------On Sale Mode Change update Quantity and Discount---\\

        //-------------------------------------------------------------------\\

    </script>
    <link href="../css/bootstrap.css" rel="stylesheet" type="text/css" />
    <div id="right_data">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <%-----------------HeaderPortion--------------%>
                <table>
                    <tr>
                        <td align="left">
                            <strong>
                                <asp:Label ID="lblfromLocation" runat="server" Width="94px" Text="Distributor" CssClass="lblbox"></asp:Label></strong>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpDistributor" runat="server" Width="200px" CssClass="DropList">
                            </asp:DropDownList>
                        </td>
                        <td></td>
                        <td align="left">
                            <strong>
                                <asp:Label ID="Label8" runat="server" Width="89px" Text="Principal" CssClass="lblbox"></asp:Label></strong>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="DrpPrincipal" runat="server" Width="200px" CssClass="DropList"
                               >
                            </asp:DropDownList>
                        </td>
                          <td></td>
                    </tr>
                   
                    <tr>
                        <td style="height: 21px" align="left">
                            <strong>
                                <asp:Label ID="Label2" runat="server" Width="89px" Text="Sale Man" CssClass="lblbox"></asp:Label></strong>
                        </td>
                        <td style="height: 21px">
                            <asp:DropDownList ID="DrpDeliveryMan" runat="server" Width="200px"
                                CssClass="DropList" >
                            </asp:DropDownList>
                        </td>
                        <td></td>
                         <td style="width: 82px;">
                            <strong>
                                <asp:Label runat="server" ID="lblInvoices" Font-Size="10pt" Text="Invoice No" Visible="false"></asp:Label></strong>
                        </td>
                        <td style="width: 100px;">
                            <asp:TextBox ID="txtInvoiceNo" Width="196px" Font-Size="11pt" runat="server" Visible="false"></asp:TextBox>
                          
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                       <td></td>
                      <td>
                            <asp:Button runat="server"  ID="btnSearch" OnClick="btnSearch_Click" Visible="false" CssClass="Button"
                                Text="Search" Width="80px"></asp:Button>
                      </td>
                        <td style="width: 250px; text-align: left;" colspan="2">

                            <asp:Label ID="lblCustomerName" runat="server" Font-Size="Medium" Font-Bold="true"></asp:Label>
                        </td>
                        <td style="width: 250px; text-align: right;" colspan="2">
                            <asp:Label ID="lblinvoicedate" runat="server" Text="" Font-Size="Medium" Font-Bold="true"></asp:Label>
                        </td>

                        <%--<td style="width: 200px; text-align: right; visibility:hidden">
                            <asp:Label ID="lblinvoiceclosing" runat="server" Text="" Font-Size="Medium" Font-Bold="true"></asp:Label>
                        </td>
                        <td style="visibility :hidden">
                            <asp:Label ID="lblInvoiceBalance" runat="server" Font-Size="Medium" Font-Bold="true"></asp:Label>
                        </td>--%>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <asp:DropDownList ID="DrpPayMode" runat="server" TabIndex="2" Width="100px" Height="30px"
                                Visible="false">
                                <asp:ListItem Selected="True" Value="214">Cash</asp:ListItem>
                                <asp:ListItem Value="215">Credit Card</asp:ListItem>
                                <asp:ListItem Value="218">Customer Credit</asp:ListItem>
                                <asp:ListItem Value="219">Cheque Invoice</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td></td>
                        <td>
                            <asp:DropDownList ID="DrpDiscount" Height="30px" Width="90px" TabIndex="3" runat="server"
                                Visible="false">
                                <asp:ListItem Value="0">% age</asp:ListItem>
                                <asp:ListItem Value="1" Selected="True">Value</asp:ListItem>
                            </asp:DropDownList>

                        </td>


                    </tr>
                </table>
                <%-------------------------------------------%>
                <%-------- Hidden Fields--------%>
                <asp:HiddenField ID="hfSkuID" runat="server" />
                <asp:HiddenField ID="hfSaleForceId" runat="server" />
                <asp:HiddenField ID="hfsId" runat="server" Value="0" />
                <asp:HiddenField ID="hfCustomerId" runat="server" />
                <asp:HiddenField ID="hfInvoiceId" runat="server" />
                <asp:HiddenField ID="hfPrincipalId" runat="server" />
                <asp:HiddenField ID="hfAssignedDistributorId" runat="server" />
                <asp:HiddenField ID="hfAREA_ID" runat="server" />
                <asp:HiddenField ID="hfTOWN_ID" runat="server" />
                <%-------- Hidden Fields end--------%>
                <%--Wait Update Panel Start --%>
                <div style="z-index: 5000; left: 700px; width: 100px; position: absolute; top: 10px; height: 100px">
                    &nbsp;<asp:Panel ID="Panel21" runat="server">
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                            <ProgressTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" Height="28px" ImageUrl="~/App_Themes/Granite/Images/image003.gif"
                                    Width="31px" />
                                Wait....
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </asp:Panel>
                </div>
                <%--Wait Update Panel End --%>
                <asp:Panel ID="Panel2" runat="server" Height="220px" ScrollBars="Vertical" Width="99%"
                    BorderColor="Silver" BorderStyle="Groove" BorderWidth="2px">
                    <table style="background-color: SteelBlue; width: 100%; color: White;">
                        <tr>
                            <td style="width: 26%; color: White; padding: 2px; font-weight: bold">Item Name
                            </td>

                            <td style="width: 7%; color: White; padding: 2px; font-weight: bold; text-align: center">Sold Qty
                            </td>
                             <td style="width: 7%; color: White; padding: 2px; font-weight: bold; text-align: center">Free Qty
                            </td>
                           <%-- <td style="width: 8%; color: White; padding: 2px; font-weight: bold; text-align: center">Discount (%)
                            </td>
                            <td style="width: 8%; color: White; padding: 2px; font-weight: bold; text-align: center">Discount (Val)
                            </td>--%>
                            <td style="width: 8%; color: White; padding: 2px; font-weight: bold; text-align: center">Price
                            </td>
                            <td style="width: 8%; color: White; padding: 2px; font-weight: bold; text-align: center">Net Amount
                            </td>
                            <td style="width: 7%; color: White; padding: 2px; font-weight: bold">Return Qty
                            </td>
                            <td style="width: 9%; color: White; padding: 2px; font-weight: bold" align="right">Prev Return Qty
                            </td>
                        </tr>
                    </table>
                    <asp:GridView ID="GrdPurchase" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed"
                        AutoGenerateColumns="False" ShowHeader="false" Width="100%" EnableModelValidation="True"
                        OnRowDataBound="GrdPurchase_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="SKU_ID" HeaderText="SKU_ID">
                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="SKU_NAME" HeaderText="Item Name">
                                <ItemStyle HorizontalAlign="Left" CssClass="grdDetail"
                                    VerticalAlign="Middle" Font-Bold="true" Font-Size="13px" Width="28%"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="BATCH_NO" HeaderText="Rack No">
                                <ItemStyle CssClass="HidePanel" />
                            </asp:BoundField>
                            <asp:BoundField DataField="QUANTITY_UNIT" HeaderText="Quantity">
                                <ItemStyle CssClass="grdDetail" HorizontalAlign="Right"
                                    VerticalAlign="Middle" Font-Bold="true" Font-Size="12px" Width="7.6%" />
                            </asp:BoundField>
                             <asp:BoundField DataField="FREE_QUANTITY" HeaderText="Free Quantity">
                                <ItemStyle CssClass="grdDetail" HorizontalAlign="Right"
                                    VerticalAlign="Middle" Font-Bold="true" Font-Size="12px" Width="7.6%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="AgeDiscount" HeaderText="%Age Discount" DataFormatString="{0:F2}">
                                 <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="STANDARD_DISCOUNT" HeaderText="Discount" DataFormatString="{0:F2}">
                                 <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="UNIT_PRICE" HeaderText="Price" DataFormatString="{0:F2}">
                                <ItemStyle CssClass="grdDetail" HorizontalAlign="Right"
                                    VerticalAlign="Middle" Font-Bold="true" Font-Size="12px" Width="8%"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="NET_AMOUNT" HeaderText="Amount" DataFormatString="{0:F2}">
                                <ItemStyle CssClass="grdDetail" HorizontalAlign="Right"
                                    VerticalAlign="Middle" Font-Bold="true" Font-Size="12px" Width="10%"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="Amount" HeaderText="Amount">
                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="DistributorId" HeaderText="DistributorId">
                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="DISCOUNT_TYPE" HeaderText="DISCOUNT_TYPE">
                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="SALE_INVOICE_ID" HeaderText="SALE_INVOICE_ID">
                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="UOM">
                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                            </asp:BoundField>

                            <asp:TemplateField HeaderText="Return Qty">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtreturnQty" runat="server" Text='<%# Eval("returnQty") %>' Style="width: 100%;"
                                        OnTextChanged="txtreturnQty_TextChanged" AutoPostBack="True"></asp:TextBox>
                                </ItemTemplate>
                                <ItemStyle CssClass="grdDetail" HorizontalAlign="Right" Width="9%" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="returnQty">
                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="returnDiscount">
                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="returnAmount">
                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="UOM2">
                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="pcs">
                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="sqm">
                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="box">
                                <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                <ItemStyle CssClass="HidePanel"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="PrevReturnQty" HeaderText="Prev Return Qty">
                                <HeaderStyle CssClass="grdDetail"></HeaderStyle>
                                <ItemStyle CssClass="grdDetail" HorizontalAlign="Right" Width="9%"
                                    VerticalAlign="Middle" Font-Bold="true" Font-Size="13px"></ItemStyle>
                            </asp:BoundField>

                        </Columns>
                    </asp:GridView>
                </asp:Panel>
                <table>
                    <tr>
                        <td>
                            <fieldset>
                                <legend>Invoice</legend>
                                <table style="width: 461px; padding: 6px;">
                                    <tr>
                                        <td>
                                            <strong>
                                                <label style="font-size: 13px;">
                                                    Gross Sale</label></strong>
                                        </td>
                                        <td style="font-size: 13px;">
                                            <asp:TextBox ID="txtGrossAmount" Font-Size="11pt" runat="server" Width="150px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <strong>
                                                <label style="font-size: 13px;">
                                                    Standerd Discount
                                                </label>
                                            </strong>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="numtxtTotalExtraDiscnt"  Font-Size="11pt" runat="server" Width="150px"></asp:TextBox>
                                              
                                        </td>
                                    </tr>
                                   <tr>
                                            <td>
                                                <strong>
                                                    <label style="font-size: 13px;">
                                                        Extra Discount</label></strong>
                                            </td>
                                            <td>
                                                
                                                <asp:TextBox ID="numtxtDiscount" ReadOnly="true" runat="server" Font-Size="11pt" 
                                                    Width="150px"></asp:TextBox>
                                                
                                            </td>
                                        </tr>
                                         <tr>
                                            <td>
                                                <strong>
                                                    <label style="font-size: 13px;">
                                                        Distributor  Discount</label></strong>
                                            </td>
                                            <td>
                                                
                                                <asp:TextBox ID="txtdistrubtrdiscval" ReadOnly="true" runat="server" Font-Size="11pt" 
                                                    Width="150px"></asp:TextBox>
                                              
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <strong>
                                                    <label style="font-size: 13px;">
                                                        Claimable  Discount</label></strong>
                                            </td>
                                            <td>
                                                
                                                <asp:TextBox ID="txtclaimablediscountval" ReadOnly="true"  runat="server" Font-Size="11pt" placeholder="Value"
                                                    Width="150px"></asp:TextBox>
                                               
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <strong>
                                                    <label style="font-size: 13px;">
                                                        GST Amount</label></strong>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="numTxtTotalGST" Width="150px" Font-Size="11pt" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                          <tr>
                                            <td>
                                                <strong>
                                                    <label style="font-size: 13px;">
                                                        TST Amount</label></strong>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txttstamount" ReadOnly="true" Width="150px" Font-Size="11pt" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                          <tr>
                                            <td>
                                                <strong>
                                                    <label style="font-size: 13px;">
                                                        Additional Tax</label></strong>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtadditionaltax" ReadOnly="true" Width="150px" Font-Size="11pt" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                     
                                   
                                    <tr>
                                        <td>
                                            <strong>
                                                <label style="font-size: 13px;">
                                                    Net Amount</label></strong>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="numTxtTotlAmnt" Font-Size="11pt" Width="150px" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <strong>
                                                <label style="font-size: 13px;">
                                                    Cash Received</label></strong>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCashRecieved" Width="150px" Font-Size="11pt" runat="server" AutoCompleteType="None"
                                               ></asp:TextBox>
                                            
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <strong>
                                                <label style="font-size: 13px;">
                                                    Balance</label></strong>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtBalance" Width="150px" Font-Size="11pt" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </td>
                        <td>
                            <fieldset>
                                <legend>Sale Return</legend>
                                <table style="width: 461px;">
                                    <tr>
                                        <td>
                                            <strong>
                                                <label style="font-size: 13px;">
                                                    Gross Return
                                                </label>
                                            </strong>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtGrossReturn" Font-Size="11pt" Width="150px" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <strong>
                                                <label style="font-size: 13px;">
                                                    Standerd Discount
                                                </label>
                                            </strong>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtDiscountReturn" Font-Size="11pt" Width="150px" runat="server"></asp:TextBox>
                                         
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <strong>
                                                <label style="font-size: 13px;">
                                                    Extra Discount
                                                </label>
                                            </strong>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtExtraDiscountReturn" Font-Size="11pt" ReadOnly="true" Width="150px" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                     <tr>
                                            <td>
                                                <strong>
                                                    <label style="font-size: 13px;">
                                                        Distributor  Discount</label></strong>
                                            </td>
                                            <td>
                                                
                                                <asp:TextBox ID="txtRETdistributorDiscountval" ReadOnly="true" runat="server" Font-Size="11pt" placeholder="Value"
                                                    Width="150px"></asp:TextBox>
                                                
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <strong>
                                                    <label style="font-size: 13px;">
                                                        Claimable  Discount</label></strong>
                                            </td>
                                            <td>
                                             
                                                <asp:TextBox ID="txtRETclamablediscountval" ReadOnly="true" runat="server" Font-Size="11pt" placeholder="Value"
                                                    Width="150px"></asp:TextBox>
                                              
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <strong>
                                                    <label style="font-size: 13px;">
                                                        GST Amount</label></strong>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtRETGSTamount" ReadOnly="true" Width="150px" Font-Size="11pt" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                          <tr>
                                            <td>
                                                <strong>
                                                    <label style="font-size: 13px;">
                                                        TST Amount</label></strong>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtRETTSTamount" ReadOnly="true" Width="150px" Font-Size="11pt" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                          <tr>
                                            <td>
                                                <strong>
                                                    <label style="font-size: 13px;">
                                                        Additional Tax</label></strong>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtRETadditionlTax" ReadOnly="true" Width="150px" Font-Size="11pt" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                    <tr>
                                        <td>
                                            <strong>
                                                <label style="font-size: 13px;">
                                                    Net Return
                                                </label>
                                            </strong>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtNetReturn" Width="150px" Font-Size="11pt" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                  
                                    <tr>
                                        <td>
                                            <strong>
                                                <label style="font-size: 13px;">
                                                    Cash Return
                                                </label>
                                            </strong>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCashReturn" Width="150px" Font-Size="11pt" AutoCompleteType="None" onkeypress="CalculateBalance(event);"
                                                onkeyup="CalculateBalancee();" runat="server"></asp:TextBox><ajaxToolkit:FilteredTextBoxExtender
                                                    ID="FilteredTextBoxExtender1" runat="server" FilterType="Custom" ValidChars="0123456789."
                                                    TargetControlID="txtCashReturn" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <strong>
                                                <label style="font-size: 13px;">
                                                    Balance
                                                </label>
                                            </strong>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtBalanceReturn" Font-Size="11pt" Width="150px" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                   
                                </table>
                            </fieldset>
                        </td>
                    </tr>
                </table>
                <div>
                    <p>
                        <asp:Button runat="server" ID="btnSaveOrder" OnClick="btnSaveOrder_Click"
                            Text="Save" AccessKey="S" CssClass="Button" Width="60px"></asp:Button>
                        <asp:Button runat="server" ID="btnCancel" OnClick="btnCancel_Click" CssClass="Button"
                            Text="Cancel"></asp:Button>
                    </p>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnSaveOrder" />
                <asp:PostBackTrigger ControlID="btnCancel" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
</asp:Content>
