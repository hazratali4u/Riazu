<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true" CodeFile="ConfigurationSetting.aspx.cs" Inherits="Forms_ConfigurationSetting" Title="SAMS: Configuration Setting" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphPage" runat="Server">
    <script language="JavaScript" type="text/javascript">
        
    </script>

    <div id="right_data">
        <table>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <cc1:TabContainer ID="TabContainer1" runat="server" Height="400px"
                                    Width="777px" ActiveTabIndex="0">
                                    <cc1:TabPanel ID="TabPanel1" runat="server">
                                        <HeaderTemplate>
                                            General Settings&nbsp;
                                        </HeaderTemplate>
                                        <ContentTemplate>

                                            <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                                                <ContentTemplate>
                                                    <table width="100%">
                                                        <tbody>
                                                            <tr>
                                                                <td colspan="2" style="width: 100%; height: 28px">
                                                                    <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Font-Bold="True"></asp:Label>
                                                                    <br />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="2">
                                                                   <%-- <asp:Panel ID="Panel2" runat="server" Height="359px" ScrollBars="Vertical" Width="750px"
                                                                        BorderColor="Silver" BorderStyle="Groove" BorderWidth="1px">--%>
                                                                        <asp:GridView ID="Setting_grid" runat="server" Width="100%" Font-Size="9pt"
                                                                            AutoGenerateColumns="False" BorderColor="SteelBlue" BorderStyle="None" OnRowDataBound="Setting_grid_RowDataBound"
                                                                             OnRowEditing="Setting_grid_RowEditing" OnRowDeleting="Setting_grid_RowDeleting">
                                                                            <Columns>
                                                                                <asp:BoundField DataField="Description" ReadOnly="true" HeaderText="">
                                                                                    <ItemStyle HorizontalAlign="Left" Height="30px" Font-Bold="true" CssClass="grdDetail mygrid-default"></ItemStyle>
                                                                                    <HeaderStyle HorizontalAlign="Left" CssClass="grdHead mygrid-default" />
                                                                                </asp:BoundField>
                                                                                <asp:TemplateField HeaderText="">
                                                                                    <ItemTemplate>
                                                                                        <asp:Button ID="btnEnable" runat="server"  CommandName="Edit" CssClass="mybtn mybtn-success" Style="font-weight: bold;" Width="55px" OnClick="btnEnable_Click" />
                                                                                        <asp:Button ID="btnDisable" runat="server"  CommandName="Delete" CssClass="mybtn mybtn-success" Style="font-weight: bold;" Width="55px" OnClick="btnDisable_Click" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;" />
                                                                                        <%--<asp:Button ID="btnEnable" runat="server" CommandArgument='<%#Eval("btnStatus")%>' CssClass="mybtn mybtn-success" Style="font-weight: bold;" Width="55px" OnClick="btnEnable_Click" />
                                                                                        <asp:Button ID="btnDisable" runat="server" CommandArgument='<%#Eval("btnStatus")%>' CssClass="mybtn mybtn-success" Style="font-weight: bold;" Width="55px" OnClick="btnDisable_Click" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;" />--%>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Center" CssClass="grdDetail mygrid-default" Width="20%" />
                                                                                    <HeaderStyle HorizontalAlign="Center" CssClass="grdHead mygrid-default" />
                                                                                </asp:TemplateField>
                                                                                <asp:BoundField DataField="btnStatus" HeaderText="btnStatus">
                                                                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                                                    <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                                                </asp:BoundField>
                                                                                <asp:BoundField DataField="SettingDescID" HeaderText="SettingDescID">
                                                                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                                                    <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                                                </asp:BoundField>
                                                                                <asp:BoundField DataField="ConfigrationDescID" HeaderText="ConfigrationDescID">
                                                                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                                                    <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                                                </asp:BoundField>
                                                                            </Columns>

                                                                            <HeaderStyle CssClass="grdHead"></HeaderStyle>
                                                                            <AlternatingRowStyle CssClass="GridAlternateRowStyle"></AlternatingRowStyle>
                                                                        </asp:GridView>
                                                                    <%--</asp:Panel>--%>
                                                                </td>
                                                                <%--<td style="width: 80%; height: 28px;">
                                                                   <strong> <asp:Label ID="Label2" runat="server" Text="Principal Wise Sale"></asp:Label></strong>
                                                                </td>
                                                                <td style="width: 20%; height: 28px">
                                                                    <asp:Button ID="btnPrincipalEnable" runat="server" CssClass="mybtn mybtn-success" Width="55px"  OnClick="btnPrincipalEnable_Click"/>
                                                                    <asp:Button ID="btnPrincipalDisable" runat="server" CssClass="mybtn mybtn-default" Width="55px"  OnClick="btnPrincipalDisable_Click"/>
                                                                </td>--%>
                                                            </tr>

                                                        </tbody>
                                                    </table>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>

                                        </ContentTemplate>
                                    </cc1:TabPanel>
                                    <cc1:TabPanel ID="TabPanel2" runat="server" Visible="false">
                                        <HeaderTemplate>
                                            Zone
                                        </HeaderTemplate>
                                        <ContentTemplate>
                                            <%--<table width="100%">
                                                <tr>
                                                    <td style="width: 100px"></td>
                                                    <td align="center" style="width: 100px">
                                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                            <ContentTemplate>
                                                                <table width="100%">
                                                                    <tbody>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 8px"></td>
                                                                            <td style="height: 8px" align="left" colspan="2">
                                                                                <asp:Label ID="lblErrorMsgDivsion" runat="server" ForeColor="Red" Font-Bold="True"></asp:Label>
                                                                                <br />
                                                                            </td>
                                                                            <td style="width: 100px; height: 8px"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 16px"></td>
                                                                            <td style="width: 49px; height: 28px" align="left">
                                                                                <strong>
                                                                                    <asp:Label ID="Label1" runat="server" Width="47px" Text="Region"></asp:Label></strong>
                                                                            </td>
                                                                            <td style="height: 16px" align="left">
                                                                                <asp:DropDownList ID="DrpRegion" runat="server" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="DrpRegion_SelectedIndexChanged" CssClass="DropList">
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                            <td style="width: 100px; height: 16px"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 8px"></td>
                                                                            <td style="width: 49px; height: 28px" align="left">
                                                                                <strong>
                                                                                    <asp:Label ID="Label21" runat="server" Width="43px" Text="Name"></asp:Label></strong>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:TextBox ID="txtZoneName" runat="server" Width="194px" CssClass="txtBox "></asp:TextBox>
                                                                            </td>
                                                                            <td style="width: 100px; height: 8px"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 37px"></td>
                                                                            <td align="right"></td>
                                                                            <td align="left">
                                                                                <asp:CheckBox ID="chbIsZoneActive" runat="server" Text="Is Active" Checked="True"></asp:CheckBox>
                                                                            </td>
                                                                            <td style="width: 100px; height: 37px"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 37px"></td>
                                                                            <td align="right">
                                                                                <asp:Label ID="Label4" runat="server" Text="Code" Width="43px" Visible="False"></asp:Label>
                                                                                &nbsp;</td>
                                                                            <td align="left">
                                                                                <asp:Button ID="btnSaveZone" OnClick="btnSaveZone_Click" runat="server" Width="85px" Font-Size="8pt" Text="Save" CssClass="Button" />
                                                                                <asp:TextBox ID="txtZoneCode" runat="server" CssClass="txtBox " Width="100px" Visible="False">N/A</asp:TextBox>
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
                                                
                                            </table>--%>
                                            <br />
                                            &nbsp;
                                        </ContentTemplate>
                                    </cc1:TabPanel>
                                    <cc1:TabPanel ID="TabPanel3" runat="server" Visible="false" HeaderText="TabPanel3">
                                        <HeaderTemplate>
                                            Territory
                                        </HeaderTemplate>
                                        <ContentTemplate>
                                            <%--<table width="100%">
                                                <tr>
                                                    <td style="width: 100px"></td>
                                                    <td align="center" style="width: 100px">
                                                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                                            <ContentTemplate>
                                                                <table width="100%">
                                                                    <tbody>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 8px"></td>
                                                                            <td style="height: 8px" align="left" colspan="2">
                                                                                <asp:Label ID="lblErrorZone" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                                                <br />
                                                                            </td>
                                                                            <td style="width: 100px; height: 8px"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 16px"></td>
                                                                            <td style="width: 49px; height: 28px" align="left">
                                                                                <strong>
                                                                                    <asp:Label ID="Label5" runat="server" Text="Region" Width="47px"></asp:Label>
                                                                                </strong>
                                                                            </td>
                                                                            <td style="height: 16px" align="left">
                                                                                <asp:DropDownList ID="drpRegionTerritory" runat="server" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="drpRegionTerritory_SelectedIndexChanged" CssClass="DropList">
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                            <td style="width: 100px; height: 16px"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 8px"></td>
                                                                            <td align="left" style="width: 49px; height: 28px">
                                                                                <strong>
                                                                                    <asp:Label ID="Label11" runat="server" Text="Zone" Width="47px"></asp:Label>
                                                                                </strong>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:DropDownList ID="DrpZone" runat="server" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="DrpZone_SelectedIndexChanged" CssClass="DropList">
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                            <td style="width: 100px; height: 8px"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 8px"></td>
                                                                            <td style="width: 49px; height: 28px" align="left">
                                                                                <strong>
                                                                                    <asp:Label ID="Label211" runat="server" Text="Name" Width="43px"></asp:Label>
                                                                                </strong>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:TextBox ID="txtTerritoryName" runat="server" CssClass="txtBox " Width="197px"></asp:TextBox>
                                                                            </td>
                                                                            <td style="width: 100px; height: 8px"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 37px"></td>
                                                                            <td align="right"></td>
                                                                            <td align="left">
                                                                                <asp:CheckBox ID="chbIsTerritoryActive" runat="server" Checked="True" Text="Is Active" />
                                                                            </td>
                                                                            <td style="width: 100px; height: 37px"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 37px"></td>
                                                                            <td align="right">
                                                                                <asp:Label ID="Label41" runat="server" Text="Code" Width="43px" Visible="False"></asp:Label>
                                                                                &nbsp;</td>
                                                                            <td align="left">
                                                                                <asp:Button ID="btnSaveTerritory" runat="server" Font-Size="8pt" OnClick="btnSaveTerritory_Click" CssClass="Button"
                                                                                    Text="Save" Width="85px" />
                                                                                <asp:TextBox ID="txtTerritoryCode" runat="server" CssClass="txtBox " Width="100px" Visible="False">N/A</asp:TextBox>
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
                                                
                                            </table>--%>
                                        </ContentTemplate>
                                    </cc1:TabPanel>
                                    <cc1:TabPanel ID="TabPanel4" runat="server" Visible="false" HeaderText="TabPanel4">
                                        <HeaderTemplate>
                                            Town
                                        </HeaderTemplate>
                                        <ContentTemplate>
                                            <%--<table width="100%">
                                                <tr>
                                                    <td style="width: 100px"></td>
                                                    <td align="center" style="width: 100px">
                                                        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                                            <ContentTemplate>
                                                                <table width="100%">
                                                                    <tbody>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 8px"></td>
                                                                            <td style="height: 8px" align="left" colspan="2">
                                                                                <asp:Label ID="lblErrorZone1" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                                                <br />
                                                                            </td>
                                                                            <td style="width: 100px; height: 8px"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 16px"></td>
                                                                            <td align="left" style="width: 49px; height: 28px">
                                                                                <strong>
                                                                                    <asp:Label ID="Label6" runat="server" Text="Region" Width="58px"></asp:Label></strong>
                                                                            </td>
                                                                            <td align="left" style="height: 16px">
                                                                                <asp:DropDownList ID="DrpTownRegion" runat="server" AutoPostBack="True" CssClass="DropList" OnSelectedIndexChanged="DrpTownRegion_SelectedIndexChanged" Width="200px">
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                            <td style="width: 100px; height: 16px"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 16px"></td>
                                                                            <td align="left" style="width: 49px; height: 28px">
                                                                                <strong>
                                                                                    <asp:Label ID="Label7" runat="server" Text="Zone" Width="58px"></asp:Label></strong>
                                                                            </td>
                                                                            <td align="left" style="height: 16px">
                                                                                <asp:DropDownList ID="DrpTownZone" runat="server" AutoPostBack="True" CssClass="DropList" OnSelectedIndexChanged="DrpTownZone_SelectedIndexChanged" Width="200px">
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                            <td style="width: 100px; height: 16px"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 16px"></td>
                                                                            <td style="width: 49px; height: 28px" align="left">
                                                                                <strong>
                                                                                    <asp:Label ID="Label33" runat="server" Text="Territory" Width="58px"></asp:Label></strong>
                                                                            </td>
                                                                            <td style="height: 16px" align="left">
                                                                                <asp:DropDownList ID="DrpTerritory" runat="server" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="DrpTerritory_SelectedIndexChanged" CssClass="DropList">
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                            <td style="width: 100px; height: 16px"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 8px"></td>
                                                                            <td style="width: 49px; height: 28px" align="left">
                                                                                <strong>
                                                                                    <asp:Label ID="Label22" runat="server" Text="Name" Width="43px"></asp:Label></strong>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:TextBox ID="txtTownName" runat="server" CssClass="txtBox " Width="197px"></asp:TextBox>
                                                                            </td>
                                                                            <td style="width: 100px; height: 8px"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 37px"></td>
                                                                            <td align="right"></td>
                                                                            <td align="left">
                                                                                <asp:CheckBox ID="chbIsTownActive" runat="server" Checked="True" Text="Is Active" />
                                                                            </td>
                                                                            <td style="width: 100px; height: 37px"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width: 100px; height: 37px"></td>
                                                                            <td align="right">
                                                                                <asp:Label ID="Label44" runat="server" Text="Code" Width="43px" Visible="False"></asp:Label>
                                                                                &nbsp;</td>
                                                                            <td align="left">
                                                                                <asp:Button ID="btnSaveTown" runat="server" Font-Size="8pt" OnClick="btnSaveTown_Click" CssClass="Button"
                                                                                    Text="Save" Width="85px" />
                                                                                <asp:TextBox ID="txtTownCode" runat="server" CssClass="txtBox " Width="100px" Visible="False">N/A</asp:TextBox>
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
                                                
                                            </table>--%>
                                        </ContentTemplate>
                                    </cc1:TabPanel>
                                </cc1:TabContainer>
                            </td>
                        </tr>
                    </table>
                </td>
                <td></td>
            </tr>
        </table>
    </div>
</asp:Content>



<%--<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link/ href="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.0/css/bootstrap.min.css" rel="stylesheet" id="bootstrap-css" />
    <script src="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.0/js/bootstrap.min.js"></script>
    <script src="http://code.jquery.com/jquery-1.11.1.min.js"></script>
    <!------ Include the above in your HEAD tag ---------->
    <style>
        .tabs-left > .nav-tabs {
            border-bottom: 0;
        }

        .tab-content > .tab-pane,
        .pill-content > .pill-pane {
            display: none;
        }

        .tab-content > .active,
        .pill-content > .active {
            display: block;
        }

        .tabs-left > .nav-tabs > li {
            float: none;
        }

        .tabs-left > .nav-tabs > li > a {
            min-width: 74px;
            margin-right: 0;
            margin-bottom: 3px;
        }

        .tabs-left > .nav-tabs {
            float: left;
            margin-right: 19px;
            border-right: 1px solid #ddd;
        }

        .tabs-left > .nav-tabs > li > a {
            margin-right: -1px;
            -webkit-border-radius: 4px 0 0 4px;
            -moz-border-radius: 4px 0 0 4px;
            border-radius: 4px 0 0 4px;
        }

        .tabs-left > .nav-tabs > li > a:hover,
        .tabs-left > .nav-tabs > li > a:focus {
            border-color: #eeeeee #dddddd #eeeeee #eeeeee;
        }

        .tabs-left > .nav-tabs .active > a,
        .tabs-left > .nav-tabs .active > a:hover,
        .tabs-left > .nav-tabs .active > a:focus {
            border-color: #ddd transparent #ddd #ddd;
            *border-right-color: #ffffff;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="row">
                <div class="col-xs-12">
                    <h3>Configrations Settings</h3>
                    <!-- tabs -->
                    <div class="tabbable tabs-left">
                        <ul class="nav nav-tabs">
                            <li class="active"><a href="#home" data-toggle="tab">Home</a></li>
                            <li><a href="#about" data-toggle="tab">About</a></li>
                            <li><a href=".services" data-toggle="tab">Services</a></li>
                            <li><a href=".contact" data-toggle="tab">Contact</a></li>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane active" id="home" runat="server">
                                <div class="">
                                    <h1>Home Tab</h1>
                                    <p>These lists are meant to identify articles which deserve editor attention because they are the most important for an encyclopedia to have, as determined by the community of participating editors. They may also be of interest to readers as an alternative to lists of overview articles.</p>
                                </div>
                            </div>
                            <div class="tab-pane" id="about" runat="server">
                                <div class="">
                                    <h1>About Tab</h1>
                                    <p>because they are the most important for an encyclopedia to have, as determined by the community of participating editors. They may also be of interest to readers as an alternative to lists of overview articles.</p>
                                </div>
                            </div>

                            <div class="tab-pane" id="services" runat="server">
                                <div class="">
                                    <h1>Services Tab</h1>
                                    <p>meant to identify articles which deserve editor attention because they are the most important for an encyclopedia to have, as determined by the community of participating editors. They may also be of interest to readers as an alternative to lists of overview articles.</p>
                                </div>
                            </div>

                            <div class="tab-pane" id="contact" runat="server">
                                <div class="">
                                    <h1>Contact Tab</h1>
                                    <p>deserve editor attention because they are the most important for an encyclopedia to have, as determined by the community of participating editors. They may also be of interest to readers as an alternative to lists of overview articles.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- /tabs -->
                </div>
            </div>
        </div>
    </form>
</body>
</html>--%>
