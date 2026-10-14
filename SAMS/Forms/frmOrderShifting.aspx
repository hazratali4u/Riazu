<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmOrderShifting.aspx.cs" Inherits="Forms_frmOrderShifting" Title="SAMS: Order Shifting" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script type="text/javascript" language="javascript">
        function HeaderClick(CheckBox) {
            var TargetBaseControl = document.getElementById('<%= this.cblOrders.ClientID %>');
            var TargetChildControl = "cbAll";
            var Inputs = TargetBaseControl.getElementsByTagName("input");
            for (var n = 0; n < Inputs.length; ++n)
                if (Inputs[n].type == 'checkbox')
                    if (!Inputs[n].disabled) {
                        Inputs[n].checked = CheckBox.checked;
                    }
        }

        function CheckBoxListSelect(cbControl) {
            var chkBoxCount = cbControl.getElementsByTagName("input");
            var cbAll = document.getElementById('<%= this.cbAll.ClientID %>');
            var ListCount = 0;
            for (var i = 0; i < chkBoxCount.length; i++) {
                if (chkBoxCount[i].checked) {
                    ListCount++;
                }
            }
            if (ListCount > 0 && ListCount < chkBoxCount.length) {
                cbAll.checked = false;
            }
            else {
                cbAll.checked = true;
            }
        }   
</script>
    <div id="right_data">
        <table width="100%">
            <tr>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <table cellspacing="5px">
                                <tr>
                                    <td align="left">
                                    </td>
                                    <td align="left">
                                        <strong>
                                            <asp:Label ID="lbltoLocation" runat="server" CssClass="lblbox" Text="Location" Width="73px"></asp:Label></strong>
                                    </td>
                                    <td align="left">
                                    </td>
                                    <td style="height: 25px" align="left">
                                        <asp:DropDownList ID="drpDistributor" runat="server" Width="200px" AutoPostBack="true"
                                            OnSelectedIndexChanged="drpDistributor_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                    </td>
                                    <td align="left">
                                        <strong>
                                            <asp:Label ID="Label6" runat="server"  Text="Delivery Man (From)"></asp:Label></strong>
                                    </td>
                                    <td align="left">
                                    </td>
                                    <td align="left" style="height: 25px">
                                        <asp:DropDownList ID="ddlDeliveryMan" runat="server" Width="200px" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlDeliveryMan_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                    </td>
                                    <td align="left">
                                        <strong>
                                            <asp:Label ID="Label3" runat="server" Height="13px" Text="Working Date"></asp:Label></strong>
                                    </td>
                                    <td align="left">
                                    </td>
                                    <td align="left" style="height: 25px">
                                        &nbsp;<asp:TextBox ID="txtStartDate" runat="server" CssClass="txtBox" MaxLength="10"
                                            onkeyup="BlockStartDateKeyPress()" Width="150px"></asp:TextBox>
                                        <asp:ImageButton ID="ibtnStartDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                            Width="16px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                    </td>
                                    <td align="left">
                                    </td>
                                    <td align="left">
                                    </td>
                                    <td align="left" style="height: 25px">
                                        <asp:Button ID="btnGetOrder" runat="server" CssClass="Button" Width="90" Text="Get Orders"
                                        OnClick="btnGetOrder_Click" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                    </td>
                                    <td align="left">
                                    <asp:CheckBox ID="cbAll" runat="server" Text="All Orders" onclick="javascript:HeaderClick(this);"/>
                                    </td>
                                    <td align="left">
                                    </td>
                                    <td align="left" style="height: 25px">
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                    </td>
                                    <td align="left" colspan="3">
                                        <asp:Panel ID="pnlOrders" runat="server" Height="200" Width="350" ScrollBars="Vertical" style="border: thin solid #000000">
                                            <asp:CheckBoxList ID="cblOrders" runat="server" RepeatDirection="Vertical"></asp:CheckBoxList>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                    </td>
                                    <td align="left">
                                        <strong>
                                            <asp:Label ID="Label1" runat="server"  Text="Delivery Man (To)"></asp:Label></strong>
                                    </td>
                                    <td align="left">
                                    </td>
                                    <td align="left" style="height: 25px">
                                        <asp:DropDownList ID="ddlDeliveryManTo" runat="server" Width="200px">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                    </td>
                                    <td align="left">
                                    </td>
                                    <td align="left">
                                    </td>
                                    <td align="left" style="height: 25px">
                                        <asp:Button ID="btnShift" runat="server" CssClass="Button" Width="90" Text="Shift"
                                        OnClick="btnShift_Click" />
                                        <asp:Button ID="btnCancel" runat="server" CssClass="Button" Width="90" Text="Cancel"
                                        OnClick="btnCancel_Click" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                    </td>
                                    <td align="left">
                                    </td>
                                    <td align="left">
                                    </td>
                                    <td align="left" style="height: 25px">
                                        <%@ register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
                                        <cc1:CalendarExtender ID="CEStartDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibtnStartDate"
                                            TargetControlID="txtStartDate">
                                        </cc1:CalendarExtender>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
        &nbsp;
    </div>
</asp:Content>
