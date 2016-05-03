<%@ Page Title="Careers" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Careers.aspx.cs" Inherits="WebApplicationForms.Careers" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Drawing.Imaging" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:updatepanel runat="server">
        <ContentTemplate>
            <!-- Course Information dialog; when clicking a course button -->
            <div id="courseInfoModal" class="modal">
                <div class="modal-content" style="width:550px; height:400px; overflow-y: scroll">
                    <span class="close">×</span>
                    <table id="legend" 
                        class="legend-table">
                    </table>
                </div>
            </div>
            <table style="width:100%">
                <tr style="border: 1px solid black">
                    <td colspan="3" style="border: 1px solid black">
                        <div style="text-align: center">
                            <h2>Electrical and Computer Engineering <%: Title %></h2>
                        </div>
                    </td>
                </tr>
                <tr style="border: 1px solid black">
                    <td id="fieldlist-col-name" style="width:20%" class="col-name">
                        <div style="text-align: center">
                            <h3>Careers</h3>
                        </div>
                    </td>
                    <td id="pathcanvas-col-name" style="width:80%" class="col-name">
                        <div style="text-align: center">
                            <h3>Career Path</h3>
                        </div>
                    </td>
                </tr>
                <tr style="border: 1px solid black">
                    <td style="width:20%; vertical-align: top; border: 1px solid black">
                        <div id="fieldlist" style="background-color:transparent; overflow-y: scroll; height: 1024px">
                            <asp:TreeView ID="treeViewCareers" 
                                LeafNodeStyle-ImageUrl="~/Images/study_icon.png"
                                ShowLines="true"
                                ForeColor="Purple"
                                OnSelectedNodeChanged="TreeViewCareers_SelectedNodeChanged"
                                runat="server">
                                <NodeStyle Font-Size="10pt" ForeColor="Purple"/>
                                <RootNodeStyle Font-Bold="True" Font-Size="12pt" NodeSpacing="10"/>
                            </asp:TreeView>
                        </div>
                    </td>
                    <td style="width:80%; height:100%; vertical-align: top; border: 1px solid black">
                        <div id="coursepathpanel" style="height: 1024px">
                        </div>
                    </td>
                </tr>
                <!-- <tr style="border: 1px solid black">
                    <td style="vertical-align: top">
                        <div style="height:324px;overflow-y: scroll">
                            <table id="legend" 
                                class="legend-table">
                            </table>
                        </div>
                    </td>
                </tr> -->
            </table>
        </ContentTemplate>
    </asp:updatepanel>
<!-- <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:CareerPathConnectionString %>" ProviderName="<%$ ConnectionStrings:CareerPathConnectionString.ProviderName %>" SelectCommand="SELECT [NAME] FROM [Career]"></asp:SqlDataSource> -->
</asp:Content>
