﻿<%@ Page Title="Programmes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Programmes.aspx.cs" Inherits="WebApplicationForms.Programmes" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Drawing.Imaging" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:updatepanel runat="server">
        <ContentTemplate>
            <!-- Course Information dialog; when clicking a course button -->
            <div id="courseInfoModal" class="modal" onmousedown="drag(this.parentNode, event);">
                <div class="modal-dialog" onmousedown="drag(this.parentNode, event);">
                    <div class="modal-content" onmousedown="drag(this.parentNode, event);">
                        <div class="modal-header" onmousedown="drag(this.parentNode, event);">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h3 class="modal-title"><b>Course Information</b></h3>
                        </div>
                        <div class="modal-body" onmousedown="drag(this.parentNode, event);">
                            <table id="legend" 
                                class="legend-table">
                            </table>
                        </div>
                        <div class="modal-footer" onmousedown="drag(this.parentNode, event);">
                            <p style="float: left">&copy; <%: DateTime.Now.Year %> - University of Auckland</p>
                            <button type="button" class="btn btn-default" data-dismiss="modal" style="float: right">Close</button>
                        </div>
                    </div>
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
                            <h3>Programmes</h3>
                        </div>
                    </td>
                    <td id="pathcanvas-col-name" style="width:80%" class="col-name">
                        <div style="text-align: center">
                            <h3>Path</h3>
                        </div>
                    </td>
                </tr>
                <tr style="border: 1px solid black">
                    <td style="width:20%; vertical-align: top; border: 1px solid black">
                        <div id="fieldlist" style="background-color:transparent; overflow-y: scroll; height: 1024px">
                            <asp:TreeView ID="treeViewProgrammes"
                                RootNodeStyle-ImageUrl="~/Images/graduate_icon.png"
                                LeafNodeStyle-ImageUrl="~/Images/study_icon.png"
                                ShowLines="true"
                                ForeColor="Purple"
                                OnSelectedNodeChanged="TreeViewProgrammes_SelectedNodeChanged" 
                                runat="server" >
                                <NodeStyle Font-Size="10pt" ForeColor="Purple"/>
                                <ParentNodeStyle Font-Bold="True" Font-Size="12pt"/>
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

</asp:Content>