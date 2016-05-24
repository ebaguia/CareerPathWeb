<%@ Page Title="Help" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Help.aspx.cs" Inherits="WebApplicationForms.Help" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Drawing.Imaging" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:updatepanel runat="server">
        <ContentTemplate>
            <h2>CareerPath FAQ</h2>
            <p></p>
            <hr style="border-width: 5px" />
            <ol>
                <li>What are the things I can find in the Careers Page?</li>
                <ul style="list-style-type:circle">
                    <li><i>You can see careers which you will possibly be working on after finishing a Programme in Electrical and Computer Engineering.</i></li>
                </ul><br />
                <li>What are the things I can find in the Programmes Page?</li>
                <ul style="list-style-type:circle">
                    <li><i>You can view the different Programmes ECE Department is offering. Currently, there are Programmes namely, Electrical and Electronics Engineering (EEE), Computer Systems Engineering (CSE) and Software Engineering (SE). Courses are grouped into their respective years (II, III, IV) which they are to be taken.</i></li>
                </ul><br />
                <li>What are the things I can find in the Courses Page?</li>
                <ul style="list-style-type:circle">
                    <li><i>You can view the all the courses related to each Programme in the ECE Department.</i></li>
                </ul><br />
                <li>How can I identify which semester the course belongs to?</li>
                <ul style="list-style-type:circle">
                    <li><i>The first semester course is left indented.</i></li>
                    <li><i>The second semester course is right indented.</i></li>
                </ul><br />
                <li>How will I know whether the course is compulsory or elective?</li>
                <ul style="list-style-type:circle">
                    <li><i>The course is <b>elective</b> if the color of the button is <font color="green"><b>green</b></font> and <b>compulsory</b> if the button is <font color="red"><b>red</b></font>.</i></li>
                </ul><br />
            </ol>
        </ContentTemplate>
    </asp:updatepanel>

</asp:Content>
