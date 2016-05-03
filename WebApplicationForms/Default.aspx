<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplicationForms._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h2>Kia Ora!</h2>
        <p class="lead">This application will guide you in identifying the courses to be taken in order to have a career in Electrical and Computer Engineering.</p>
        <p><a href="https://www.auckland.ac.nz" class="btn btn-primary btn-lg">University's Homepage &raquo;</a></p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Careers</h2>
            <p>
                What are the available careers that you can work on after graduating from the Electrical and Computer Engineering?
            </p>
            <p>
                <a class="btn btn-default" href="Careers">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Programmes</h2>
            <p>
                Want to know more on the different undergradudate programmes the Electrical and Computer Engineering department is offering?
            </p>
            <p>
                <a class="btn btn-default" href="Programmes">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Courses</h2>
            <p>
                Or you can directly view all the undergraduate courses being offered by the Electrical and Computer Engineering department.
            </p>
            <p>
                <a class="btn btn-default" href="Courses">Learn more &raquo;</a>
            </p>
        </div>
    </div>

</asp:Content>
