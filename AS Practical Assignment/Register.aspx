<%@ Page Title="Register Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="AS_Practical_Assignment.Register" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width: 60%; margin: 12% auto; padding: 3%; border: 1px solid; box-shadow: 20px 10px;">
        <h4 style="font-weight: bold;">Register Form</h4>
        <table style="width: 100%;">
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td><asp:Label ID="lbl_fname" runat="server" Text="First Name"></asp:Label></td>
                <td><asp:Label ID="lbl_lname" runat="server" Text="Last Name"></asp:Label></td>
            </tr>
            <tr>
                <td><asp:TextBox ID="tb_fname" runat="server"></asp:TextBox></td>
                <td><asp:TextBox ID="tb_lname" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td><asp:Label ID="lbl_email" runat="server" Text="Email Address"></asp:Label></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td><asp:TextBox ID="tb_email" runat="server" TextMode="Email"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td><asp:Label ID="lbl_dob" runat="server" Text="Date of Birth"></asp:Label></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td><asp:TextBox ID="tb_dob" runat="server" TextMode="Date"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td><asp:Label ID="lbl_pwd" runat="server" Text="Password"></asp:Label></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td><asp:TextBox ID="tb_pwd" runat="server" ClientIDMode="Static" onkeyup="javascript:validate()" TextMode="Password"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
                <tr>
                <td><asp:Label ID="lbl_pwdchecker" runat="server" ClientIDMode="Static" Text=""></asp:Label></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td><asp:Label ID="lbl_cardno" runat="server" Text="CardNo"></asp:Label></td>
                <td><asp:Label ID="lbl_cvv" runat="server" Text="CVV"></asp:Label></td>
            </tr>
            <tr>
                <td><asp:TextBox ID="tb_cardno" runat="server"></asp:TextBox></td>
                <td><asp:TextBox ID="tb_cvv" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td><asp:Label ID="lbl_expdate" runat="server" Text="Expiry Date"></asp:Label></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td><asp:TextBox ID="tb_expdate" runat="server"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td><asp:Label ID="lbl_ccichecker" runat="server" Text="" ForeColor="Red"></asp:Label></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2"><asp:Button ID="btn_register" runat="server" Text="Register" OnClick="btn_register_Click" Width="100%" /></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2" style="text-align: center;"><asp:Label ID="lbl_msg" runat="server" Text=""></asp:Label></td>
            </tr>
        </table>
    </div>
    
    <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
    </asp:Content>
