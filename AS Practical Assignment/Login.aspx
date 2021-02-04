<%@ Page Title="Login Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AS_Practical_Assignment.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width: 60%; margin: 12% auto; padding: 3%; border: 1px solid; box-shadow: 20px 10px;">
        <h4 style="font-weight: bold;">Login Form</h4>
        <table style="width: 100%;">
            <tr>
                <td></td>
            </tr>
            <tr>
                <td>
        <asp:Label ID="lbl_email" runat="server" Text="Email Address"></asp:Label></td>
            </tr>
            <tr>
                <td>
        <asp:TextBox ID="tb_email" runat="server" TextMode="Email"></asp:TextBox></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
        <asp:Label ID="lbl_pwd" runat="server" Text="Password"></asp:Label></td>
            </tr>
            <tr>
                <td>
        <asp:TextBox ID="tb_pwd" runat="server" TextMode="Password"></asp:TextBox></td>
            </tr>
            <tr>
                <td>
        <asp:Label ID="lbl_pwdchecker" runat="server" Text="" ForeColor="Red"></asp:Label></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
        <asp:Button ID="btn_login" runat="server" Text="Login" OnClick="btn_login_Click" Width="100%"/>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td style="text-align: center;"><asp:Label ID="lbl_msg" runat="server" Text=""></asp:Label></td>
            </tr>
        </table>
    </div>
    
    <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
</asp:Content>
