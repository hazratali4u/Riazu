<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmChequeEntryAE.aspx.cs" Inherits="Forms_frmChequeEntryAE" Title="SAMS: Cheque Entry" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
      <script type="text/javascript" src="../AjaxLibrary/jquery.searchabledropdown-1.0.8.min.js"></script>
    <script language="JavaScript" type="text/javascript">
        
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(startRequest);

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequest);

        function startRequest(sender, e) {

            document.getElementById('<%=btnSave.ClientID%>').disabled = true;
            document.getElementById('<%=btnCancel.ClientID%>').disabled = true;

        }
        function pageLoad() {
            $("select").searchable();
        }
        function endRequest(sender, e) {

            document.getElementById('<%=btnSave.ClientID%>').disabled = false;
            document.getElementById('<%=btnCancel.ClientID%>').disabled = false;

        }

        function ValidateForm() {
            var str;
            var btn;
            str = document.getElementById("<%= txtAmount.ClientID %>").value;
            btn = document.getElementById("<%= btnSave.ClientID %>").value;

            if (btn == "Save" && (str == null || str.length == 0)) {
                alert('Must enter Amount');
                return false;
            }

            str = document.getElementById("<%= txtSlipNo.ClientID %>").value;
            btn = document.getElementById("<%= btnSave.ClientID %>").value;
            var status = document.getElementById("<%= DrpStatus.ClientID %>").value;

            if (btn == "Update" && (str.length == 0 || str == null) && status == 527) {
                alert('Must enter Slip No');
                return false;
            }

            str = document.getElementById("<%= DrpCustomerBank.ClientID %>").value;
            if (str.length == 0 || str == null) {
                alert('Must Select Bank Name');
                return false;
            }


        }

        function checkDate(sender, args) {


        }
        function ChChequeListSelect() {

            var chkBoxList = document.getElementById('<%= GrdOrder.ClientID %>');
            var chkBox = document.getElementById('<%= ChbSelectAll.ClientID %>');
            var chkBoxCount;
            if (chkBox.checked == true) {
                chkBoxCount = chkBoxList.getElementsByTagName("input");
                for (var i = 0; i < chkBoxCount.length; i++) {
                    chkBoxCount[i].checked = true;
                }
            }
            else {
                chkBoxCount = chkBoxList.getElementsByTagName("input");
                for (var i = 0; i < chkBoxCount.length; i++) {
                    chkBoxCount[i].checked = false;
                }
            }

            return true;
        }
        function Check_Click(objRef) {
            var row = objRef.parentNode.parentNode;
            var GridView = row.parentNode;
            var inputList = GridView.getElementsByTagName("input");
            for (var i = 0; i < inputList.length; i++) {
                var headerCheckBox = inputList[0];
                var checked = true;
                if (inputList[i].type == "checkbox" && inputList[i] != headerCheckBox) {
                    if (!inputList[i].checked) {
                        checked = false;
                        break;
                    }
                }
            }

            headerCheckBox.checked = checked;
        }

        function checkAll(objRef) {
            var GridView = objRef.parentNode.parentNode.parentNode;
            var inputList = GridView.getElementsByTagName("input");
            for (var i = 0; i < inputList.length; i++) {
                var row = inputList[i].parentNode.parentNode;
                if (inputList[i].type == "checkbox" && objRef != inputList[i]) {
                    if (objRef.checked) {
                        inputList[i].checked = true;
                    }
                    else {
                        inputList[i].checked = false;
                    }
                }
            }
        }

    </script>
    
    <%-- <script type = "text/javascript">
        function Confirm() {
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";
            if (confirm("Advance Cheque Process.Do you want to save data?")) {
                confirm_value.value = "Yes";
            } else {
                confirm_value.value = "No";
            }
            document.forms[0].appendChild(confirm_value);
        }
    </script>--%>
    <div id="right_data">
        <div>
            <table width="100%">
                <tr>
                    <td>
                        <div style="z-index: 101; left: 400px; width: 100px; position: absolute; top: 350px; height: 100px">
                            <asp:Panel ID="Panel21" runat="server">
                                <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                                    <ProgressTemplate>
                                        <asp:ImageButton ID="ImageButton1" runat="server" Height="26px" ImageUrl="~/App_Themes/Granite/Images/image003.gif"
                                            Width="23px" />
                                        Wait Update..
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                            </asp:Panel>
                        </div>
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tbody>
                                        <tr>
                                            <td align="left" colspan="2">
                                                <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Font-Bold="True"></asp:Label>
                                            
                                                 </td>
                                            <td style="width: 50px" align="left" colspan="1"></td>
                                            <td align="left" colspan="1"></td>
                                            <td style="width: 50px" align="left" colspan="1"></td>
                                            <td align="left" colspan="1" style="width: 22px"></td>
                                        </tr>
                                        <tr>
                                            <td style="height: 17px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label12" runat="server" Width="84px" Text="Cheque Type" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 201px; height: 17px" align="left">
                                                <asp:DropDownList ID="DrpChequeType" runat="server" Width="226px" CssClass="DropList"
                                                    OnSelectedIndexChanged="DrpChequeType_SelectedIndexChanged" AutoPostBack="True">
                                                    <asp:ListItem>Cheque Realized</asp:ListItem>
                                                    <asp:ListItem>Cheque Advance</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 50px; height: 17px" align="left"></td>
                                            <td style="height: 17px" align="left">
                                                <strong>
                                                    <asp:Label ID="lblsaleforce" runat="server" Width="66px" Text="Sale Force" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 215px; height: 17px" align="left">
                                                <asp:DropDownList ID="DrpDeliveryMan" runat="server" Width="240px" CssClass="DropList">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="height: 17px; width: 22px;" align="left"></td>
                                        </tr>
                                        <tr>
                                            <td style="height: 17px" align="left">
                                                <strong>
                                                    <asp:Label ID="lblfromLocation" runat="server" Width="94px" Text="Location" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 201px; height: 17px" align="left">
                                                <asp:DropDownList ID="drpDistributor" runat="server" Width="226px" CssClass="DropList"
                                                    OnSelectedIndexChanged="drpDistributor_SelectedIndexChanged" AutoPostBack="True">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 50px; height: 17px" align="left"></td>
                                            <td style="height: 17px" align="left">
                                                <strong>
                                                    <asp:Label ID="lblroute" runat="server" Width="66px" Text="Route" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 215px; height: 17px" align="left">
                                                <asp:DropDownList ID="DrpRoute" runat="server" Width="240px" CssClass="DropList"
                                                    OnSelectedIndexChanged="DrpRoute_SelectedIndexChanged" AutoPostBack="True">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="height: 17px; width: 22px;" align="left"></td>
                                        </tr>
                                        <tr>
                                            <td style="height: 17px" align="left">
                                                <strong>
                                                    <asp:Label ID="Label8" runat="server" Width="89px" Text="Principal" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 201px; height: 17px" align="left">
                                                <asp:DropDownList ID="DrpPrincipal" runat="server" Width="226px" CssClass="DropList"
                                                    OnSelectedIndexChanged="DrpPrincipal_SelectedIndexChanged" AutoPostBack="True">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 50px; height: 17px" align="left"></td>
                                            <td style="height: 17px" align="left">
                                                <strong>
                                                    <asp:Label ID="lblcustomer" runat="server" Width="66px" Text="Customer" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 215px; height: 17px" align="left">
                                                <asp:DropDownList ID="DrpCustomer" runat="server" Width="240px" CssClass="DropList"
                                                    OnSelectedIndexChanged="DrpCustomer_SelectedIndexChanged" AutoPostBack="True">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="height: 17px; width: 22px;" align="left"></td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <strong>
                                                    <asp:Label ID="Label10" runat="server" Width="86px" Text="Status" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 201px" align="left">
                                                <asp:DropDownList ID="DrpStatus" runat="server" Width="226px" OnSelectedIndexChanged="DrpStatus_SelectedIndexChanged"
                                                    AutoPostBack="True">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 50px" align="left">
                                                <strong>
                                                    <asp:Label ID="lblDocumentNo" runat="server" Width="25px" Text="            " CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td align="left" colspan="2" rowspan="9">
                                                <asp:Panel ID="Panel1" runat="server" Height="155px" ScrollBars="Vertical" BorderColor="Silver"
                                                    BorderStyle="Groove" BorderWidth="1px">
                                                    <asp:GridView ID="GrdCredit" runat="server" AutoGenerateColumns="False" BackColor="White"
                                                        BorderColor="White" CssClass="gridRow2" ForeColor="SteelBlue" HorizontalAlign="Center"
                                                        Width="495px" DataKeyNames="SALE_INVOICE_ID">
                                                        <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                                            PreviousPageText="Previous" />
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <asp:CheckBox ID="checkAll" runat="server" onclick="checkAll(this);" />
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="ChbIsAssigned" runat="server" onclick="Check_Click(this)" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="3%" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"  HorizontalAlign="Center" />
                                                                <HeaderStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"  HorizontalAlign="Center" />
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="MANUAL_INVOICE_ID" HeaderText="Invoice No">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="DOCUMENT_DATE" HeaderText="Invoice Date">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="CURRENT_CREDIT_AMOUNT" HeaderText="Credit Amount" DataFormatString="{0:F2}">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="DELIVERYMAN_ID" HeaderText="DELIVERYMAN_ID">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="CREDIT_AMOUNT" HeaderText="Invoice Amount" DataFormatString="{0:F2}">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                             <asp:BoundField DataField="Cheque_count" HeaderText="Cheque received">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" Width="15px" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="TAX" HeaderText="WH Tax" DataFormatString="{0:F2}">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                        </Columns>
                                                        <HeaderStyle CssClass="tblhead" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:GridView>
                                                    <br />
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <strong>
                                                    <asp:Label ID="lblBankAccount" runat="server" Width="98px" Text="Deposit Account"
                                                        CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="DrpBankAccount" runat="server" Width="226px" CssClass="DropList"
                                                    OnSelectedIndexChanged="DrpStatus_SelectedIndexChanged" AutoPostBack="True">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 50px"></td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <strong>
                                                    <asp:Label ID="Label5" runat="server" Width="94px" Text="Chque No" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td align="left">
                                                <strong>
                                                    <asp:Label ID="Label3" runat="server" Width="94px" Text="Bank Name" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 50px"></td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <asp:TextBox ID="txtChequeNo" runat="server" Width="94px" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                            <td valign="top" align="left">
                                                <asp:TextBox ID="txtBankName" runat="server" Width="94px" CssClass="txtBox" Visible="False"></asp:TextBox>
                                                <asp:DropDownList ID="DrpCustomerBank" runat="server" AutoPostBack="True" CssClass="DropList"
                                                    OnSelectedIndexChanged="DrpStatus_SelectedIndexChanged" Width="226px" Visible="false">
                                                </asp:DropDownList>
                                                <asp:TextBox ID="txtAccountNo" runat="server" Width="223px" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                            <td valign="top" align="left" style="width: 50px">
                                                <asp:Label ID="lblstatus" runat="server" CssClass="lblbox" Height="16px" Visible="False"
                                                    Width="148px"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" align="left">
                                                <strong>
                                                    <asp:Label ID="Label13" runat="server" Width="94px" Text="Remarks" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 50px" valign="top" align="left"></td>
                                            <td style="width: 50px" valign="top" align="left"></td>
                                        </tr>
                                        <tr>
                                            <td valign="top" align="left" colspan="2">
                                                <asp:TextBox ID="txtRemarks" runat="server" Width="329px" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                            <td style="width: 50px" valign="top" align="left"></td>
                                        </tr>
                                        <tr>
                                            <td valign="top" align="left">
                                                <strong>
                                                    <asp:Label ID="Label6" runat="server" Width="74px" Text="Chq Amount" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 201px" valign="top" align="left">
                                                <strong>
                                                    <asp:Label ID="Label9" runat="server" Width="92px" Text="Cheque Date"
                                                        CssClass="lblbox" Height="16px"></asp:Label></strong>
                                            </td>
                                            <td style="width: 50px" valign="top" align="left">&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" align="left">
                                                <asp:TextBox ID="txtAmount" runat="server" Width="94px" CssClass="txtBox "></asp:TextBox>
                                            </td>
                                            <td style="width: 201px" valign="top" align="left">
                                                <asp:TextBox ID="txtStartDate" runat="server" Width="132px" CssClass="txtBox "></asp:TextBox>
                                            </td>
                                            <td style="width: 50px" valign="top" align="left"></td>
                                        </tr>
                                        <tr>
                                            <td valign="top" align="left">
                                                <strong>
                                                    <asp:Label ID="Label2" runat="server" Width="96px" Text="Slip No" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 201px" valign="top" align="left">
                                                <strong>
                                                    <asp:Label ID="Label1" runat="server" Width="100px" Text="Recevied Date" CssClass="lblbox"></asp:Label></strong>
                                            </td>
                                            <td style="width: 50px" valign="top" align="left"></td>
                                        </tr>
                                        <tr>
                                            <td valign="top" align="left">
                                                <asp:TextBox ID="txtSlipNo" runat="server" Width="94px" CssClass="txtBox "></asp:TextBox>
                                            </td>
                                            <td style="width: 201px" valign="top" align="left">
                                                <asp:TextBox ID="txtReceivedDate" runat="server" Width="132px" CssClass="txtBox "
                                                    ReadOnly="True"></asp:TextBox>
                                            </td>
                                            <td style="width: 50px" valign="top" align="left"></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <strong>                                                <asp:CheckBox ID="chbIsAdvance" Visible="false" runat="server" Checked="false" Text="Allow Advance Amount Entry"/>
                                           </strong> 
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" align="left">
                                                <asp:TextBox ID="TextBox1" runat="server" Width="94px" CssClass="txtBox "></asp:TextBox>
                                            </td>
                                            <td style="width: 201px" valign="top" align="left">
                                                <asp:TextBox ID="TextBox2" runat="server" Width="132px" CssClass="txtBox "
                                                    ReadOnly="True"></asp:TextBox>
                                            </td>
                                            <td valign="top" align="left"></td>
                                            <td valign="top" align="left"></td>
                                            <td align="right">
                                                <asp:TextBox ID="txtTotalCreditAmount" runat="server" Width="94px" ReadOnly="true" Style="margin-right:20px; text-align:right;" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <br />
                                                <strong>
                                                    <asp:Label ID="lblStatusConvertTo" runat="server" CssClass="lblbox" Text="Convert To"
                                                        Width="70px"></asp:Label>
                                                </strong>
                                                <td>
                                                    <br />
                                                    <asp:DropDownList ID="DrpStatusConvertTo" runat="server" AutoPostBack="True" CssClass="DropList"
                                                        Width="226px">
                                                    </asp:DropDownList>
                                                </td>
                                            </td>
                                            <td style="width: 50px"></td>
                                        </tr>
                                        <tr>
                                            <td valign="top" align="left">
                                                <asp:Button AccessKey="S" ID="btnSave" OnClick="btnSave_Click" runat="server" Width="102px"
                                                    Font-Size="8pt" Text="Save" CssClass="Button" />
                                            </td>
                                            <td style="width: 201px" valign="top" align="left">
                                                <asp:Button AccessKey="C" ID="btnCancel" runat="server" Width="120px" Font-Size="8pt"
                                                    Text="Cancel" OnClick="btnCancel_Click" CssClass="Button" />
                                            </td>
                                            <td style="width: 50px" valign="top" align="left"></td>
                                        </tr>
                                    </tbody>
                                </table>
                                <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" FilterType="Custom"
                                    ValidChars="0123456789." TargetControlID="txtAmount">
                                </cc1:FilteredTextBoxExtender>
                                <cc1:MaskedEditExtender ID="MaskedEditExtender2" runat="server" TargetControlID="txtStartDate"
                                    Mask="99/99/9999" MaskType="Date">
                                </cc1:MaskedEditExtender>
                                <asp:HiddenField ID="HFChqueProcessId" runat="server"></asp:HiddenField>
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
                                <table style="border-right: silver thin inset; border-top: silver thin inset; border-left: silver thin inset; width: 650px; border-bottom: silver thin inset">
                                    <tbody>
                                        <tr>
                                            <td style="height: 20px; width: 910px;" align="left" colspan="5">
                                                <asp:Panel ID="Panel12" runat="server" Width="910px" Height="200px" ScrollBars="Vertical">
                                                    <table style="border-right: silver thin inset; border-top: silver thin inset; border-left: silver thin inset; border-bottom: silver thin inset; background-color: silver"
                                                        width="100%">
                                                        <tbody>
                                                            <tr>
                                                                <td>
                                                                    <asp:CheckBox ID="ChbSelectAll" runat="server" Font-Size="8pt" onclick="ChChequeListSelect()"
                                                                        Text="Select All" Width="75px" OnCheckedChanged="ChbSelectAll_CheckedChanged1"
                                                                        AutoPostBack="true" />
                                                                </td>
                                                                <td style="height: 21px" align="left">
                                                                    <strong>
                                                                        <asp:Label ID="Label110" runat="server" Width="154px" Text="Select Searching Type"></asp:Label></strong>
                                                                </td>
                                                                <td style="width: 170px; height: 21px" align="left">
                                                                    <asp:DropDownList ID="ddSearchType" runat="server" Width="150px" CssClass="DropList">
                                                                        <asp:ListItem Value="CHEQUE_NO">All Records</asp:ListItem>
                                                                        <asp:ListItem Value="CUSTOMER_NAME">Customer</asp:ListItem>
                                                                        <asp:ListItem Value="BANK_NAME">Bank Name</asp:ListItem>
                                                                        <asp:ListItem Value="SlipNo">Slip No</asp:ListItem>
                                                                        <asp:ListItem Value="account_name">Deposit Account </asp:ListItem>
                                                                        <asp:ListItem Value="CHEQUE_DATE">Cheque Date</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td style="height: 21px" align="left">
                                                                    <asp:TextBox ID="txtSeach" runat="server" Width="150px" CssClass="txtBox "></asp:TextBox>
                                                                </td>
                                                                <td style="height: 21px" align="left">
                                                                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="txtBox" MaxLength="10" Width="100px"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:ImageButton ID="ImgBtnFromDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif" />
                                                                    <cc1:CalendarExtender ID="ceFromDate" runat="server" EnableViewState="False" Format="dd-MMM-yyyy"
                                                                        PopupButtonID="ImgBtnFromDate" TargetControlID="txtFromDate" OnClientDateSelectionChanged="checkDate">
                                                                    </cc1:CalendarExtender>
                                                                </td>
                                                                <td style="height: 21px" align="left">
                                                                    <asp:TextBox ID="txtToDate" runat="server" CssClass="txtBox" MaxLength="10" Width="100px"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:ImageButton ID="ImgBntToDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif" />
                                                                    <cc1:CalendarExtender ID="CalendarExtender2" runat="server" EnableViewState="False"
                                                                        Format="dd-MMM-yyyy" PopupButtonID="ImgBntToDate" TargetControlID="txtToDate"
                                                                        OnClientDateSelectionChanged="checkDate">
                                                                    </cc1:CalendarExtender>
                                                                </td>
                                                                <td style="width: 250px; height: 21px" align="left">
                                                                    <asp:Button ID="btnFilter" runat="server" Width="75px" Font-Size="8pt" Text="Filter"
                                                                        OnClick="btnFilter_Click"></asp:Button>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                    <asp:GridView ID="GrdOrder" runat="server" Width="100%" ForeColor="SteelBlue" CssClass="gridRow2"
                                                        BorderColor="White" HorizontalAlign="Center" BackColor="White" AutoGenerateColumns="False"
                                                        OnRowEditing="GrdOrder_RowEditing" OnRowDeleting="GrdOrder_RowDeleting" OnSelectedIndexChanged="GrdOrder_SelectedIndexChanged">
                                                        <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                                            PreviousPageText="Previous" />
                                                        <Columns>
                                                            <asp:BoundField DataField="CHEQUE_PROCESS_ID" ReadOnly="true" HeaderText="CHEQUE_PROCESS_ID">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="CUSTOMER_ID" ReadOnly="true" HeaderText="CUSTOMER_ID">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:TemplateField HeaderText="Select">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="ChbIsSelect" runat="server" Width="14px" OnCheckedChanged="ChbIsSelect_SelectedIndexChanged"
                                                                        AutoPostBack="True" />
                                                                </ItemTemplate>
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center"/>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="CUSTOMER_NAME" ReadOnly="true" HeaderText="Customer">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Left" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="CHEQUE_NO" ReadOnly="true" HeaderText="Chq. No">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Left" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="BANKID" ReadOnly="true" HeaderText="BANK_ID">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="BANKID" ReadOnly="true" HeaderText="Bank Name">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="CHEQUE_DATE" ReadOnly="true" HeaderText="Chq.Date">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="RECEIVED_DATE" ReadOnly="true" HeaderText="Received Date">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="DEPOSIT_DATE" ReadOnly="true" HeaderText="Deposit Date">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="CHEQUE_AMOUNT" ReadOnly="true" DataFormatString="{0:F2}" HeaderText="Chq.Amount">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Right" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SlipNo" ReadOnly="true" HeaderText="Slip No">
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Remarks" ReadOnly="true" HeaderText="Remarks">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="area_id" ReadOnly="true" HeaderText="area_id">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="account_name" ReadOnly="true" HeaderText="Deposit Account">
                                                                <ControlStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" />
                                                                <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="account_head_id" ReadOnly="true" HeaderText="account_head_id">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="DeliveryManID" ReadOnly="true" HeaderText="DeliveryManID">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Edit">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" Text="Edit" ToolTip="Edit">
                                                                    </asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle BorderColor="Silver" BorderWidth="2px" BorderStyle="Solid" Width="30px"></ItemStyle>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <HeaderStyle CssClass="tblhead" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:GridView>
                                                    <br />
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <br />
                        <br />
                        <br />
                        <br />
                        <br />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
