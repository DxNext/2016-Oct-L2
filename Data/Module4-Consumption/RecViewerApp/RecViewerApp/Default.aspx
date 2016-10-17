<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RecViewerApp._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Cognitive Services - Product Recommendations</h1>
        <p class="lead">Adds smarts to your e-commerce store by adding Product Recommendations</p>
        <p><a href="https://azure.microsoft.com/en-us/documentation/articles/machine-learning-recommendation-api-documentation/" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Model Information</h2>
            <p>
                Please enter your Recommendations <strong>Account Key</strong>:&nbsp;&nbsp;&nbsp;
                <asp:TextBox ID="ModelKey" runat="server" OnTextChanged="ModelKey_TextChanged"></asp:TextBox>
            </p>
            <p>
                Select a Trained Model from the List:&nbsp;&nbsp;&nbsp;
                <asp:DropDownList ID="ModelSelect" runat="server" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                </asp:DropDownList>
                </p>
        </div>
        <div class="col-md-4">
            <h2>Partsunlimited Products</h2>
            <p>
                &nbsp;</p>
            <p>
                &nbsp;</p>
        </div>
        <div class="col-md-4">
            <h2>Recommendations</h2>
            <p>
                &nbsp;</p>
            <p>
                &nbsp;</p>
        </div>
    </div>

</asp:Content>
