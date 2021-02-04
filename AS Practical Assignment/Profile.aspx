<%@ Page Title="Profile page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="AS_Practical_Assignment.Profile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width: 60%; margin: 12% auto; padding: 3%; border: 1px solid; box-shadow: 20px 10px;">
        <h4 style="font-weight: bold;">User Information</h4>
        <table style="width: 100%;">
            <tr>
                <td>Full Name:</td>
                <td><asp:Label ID="lbl_username" runat="server" Text=""></asp:Label></td>
            </tr>
            <tr>
                <td>Email Address:</td>
                <td><asp:Label ID="lbl_email" runat="server" Text=""></asp:Label></td>
            </tr>
            <tr>
                <td>Date of Birth</td>
                <td><asp:Label ID="lbl_dob" runat="server" Text=""></asp:Label></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2"><asp:Button ID="btn_logout" runat="server" Text="Logout" OnClick="btn_logout_Click" Width="100%" /></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2"><h4 style="font-weight: bold;">Change Password</h4></td>
            </tr>
            <tr>
                <td>New Password</td>
                <td>Confirm Password</td>
            </tr>
            <tr>
                <td><asp:TextBox ID="tb_newpwd" runat="server" TextMode="Password"></asp:TextBox></td>
                <td><asp:TextBox ID="tb_confirmpwd" runat="server" TextMode="Password"></asp:TextBox></td>
            </tr>
            <tr>
                <td colspan="2""><asp:Label ID="lbl_pwdchecker" runat="server" Text=""></asp:Label></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2"><asp:Button ID="btn_changepwd" runat="server" Text="Confirm" Width="100%" OnClick="btn_changepwd_Click" /></td>
            </tr>
        </table>
    </div>

    <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
</asp:Content>
