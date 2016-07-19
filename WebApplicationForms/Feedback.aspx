<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Feedback.aspx.cs" Inherits="WebApplicationForms.Feedback" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:updatepanel runat="server">
        <ContentTemplate>
            <h2>Website Feedback</h2>
            <p></p>
            <hr style="border-width: 5px" />
            <div>
                <fieldset style="width: 40%;">
                    <table cellpadding="2" cellspacing="5" >
                        <tr>
                            <td width="80px">
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td width="80px">
                                Name: <span style="color: #CC3300"> *</span>
                            </td>
                            <td>
                                <asp:TextBox ID="txtName" runat="server" Width="200px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                    ErrorMessage="Name Required" ForeColor="#FF3300"
                                    ControlToValidate="txtName"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                E-mail: <span style="color: #CC3300"> *</span>
                            </td>
                            <td>
                                <asp:TextBox ID="txtEmail" runat="server" Width="300px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                                    ErrorMessage="Email Required" ForeColor="#FF3300"
                                    ControlToValidate="txtEmail"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                                    ErrorMessage="Not a vailed email" ControlToValidate="txtEmail"
                                    ForeColor="#FF3300"
                                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Comments: <span style="color: #CC3300"> </span>
                            </td>
                            <td>
                                <asp:TextBox ID="txtComments" runat="server" Height="100px" TextMode="MultiLine" Width="400px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                <asp:Button ID="btnSubmit" runat="server" Text="Send" Width="100px" OnClick="SubmitComments"/>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </div>
        </ContentTemplate>
    </asp:updatepanel>
</asp:Content>
