<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="AccessDenied.aspx.cs" Inherits="Forms_AccessDenied" Title="SAMS: Access Denied" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
<h2>Access Denied</h2>
    <table>
        <tr>
            <td>
                &nbsp
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblmsg" runat="server" Text="" Style="color:red;"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
