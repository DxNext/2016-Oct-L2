<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RecViewerApp._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Cognitive Services - Product Recommendations</h1>
        <p class="lead">Adds smarts to your e-commerce store by adding Product Recommendations</p>
        <p><a href="https://azure.microsoft.com/en-us/documentation/articles/machine-learning-recommendation-api-documentation/" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
    </div>
    <hr />
    <div class="modelInfoRow">
        <div class="modelInfoCol">
            <h2>Model Information</h2>
            <p class="lead">
                Please enter your Recommendations <strong>Account Key</strong>:&nbsp;&nbsp;&nbsp;
                <asp:TextBox ID="ModelKey" runat="server" OnTextChanged="ModelKey_TextChanged" AutoPostBack="true"></asp:TextBox>
            </p>
            <p class="lead">
                Select a Trained Model from the List:&nbsp;&nbsp;&nbsp;
                <asp:DropDownList ID="ModelSelect" runat="server" OnSelectedIndexChanged="ModelSelect_SelectedIndexChanged" Height="43px" Width="281px" >
                </asp:DropDownList>
                </p>
        </div>
        <hr />
        <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
            <script type="text/javascript" src="http://cdn.jsdelivr.net/jcarousel/0.2.8/jquery.jcarousel.min.js"></script>
            <script type="text/javascript">
                $(function () {
                    $('#mycarousel').jcarousel();
                });
                $(function () {
                    $('#reccarousel').jcarousel();
                });
            </script>
        <div class="row">
            <h2>Partsunlimited Products</h2>
            
            <ul id="mycarousel" class="jcarousel-skin-tango">
                <asp:Repeater ID="productdetails" runat="server" OnItemCommand="productdetails_ItemCommand" >
                    <ItemTemplate>
                        <li>
                            <asp:ImageButton runat="server" ToolTip="<%# ((RecViewerApp.ProductDetailsDisplay)Container.DataItem).title%>" CommandName="Click" AlternateText="<%# ((RecViewerApp.ProductDetailsDisplay)Container.DataItem).title%>" ImageUrl="<%# ((RecViewerApp.ProductDetailsDisplay)Container.DataItem).productArtUrl%>" CommandArgument="<%# ((RecViewerApp.ProductDetailsDisplay)Container.DataItem).productID%>" Height="200" Width="200" CausesValidation="false" />
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>             
        </div>
         <hr />
        <div class="row" id="recDisplay" runat="server" visible="false">
            <h2>Recommendations</h2>
            
            <ul id="reccarousel" class="jcarousel-skin-tango">
                <asp:Repeater ID="recommendations" runat="server" OnItemCommand="recdetails_ItemCommand" >
                    <ItemTemplate>
                        <li>
                            <asp:ImageButton runat="server" CommandName="Click" ToolTip="<%# ((RecViewerApp.RecDetailsDisplay)Container.DataItem).title%>" ImageUrl="<%# ((RecViewerApp.RecDetailsDisplay)Container.DataItem).productArtUrl%>" CommandArgument="<%# ((RecViewerApp.RecDetailsDisplay)Container.DataItem).productID%>" Height="200" Width="200" CausesValidation="false" />
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>           
            <hr />
            
        <p style="font-size: 16px"  visible="false" id="conf" runat="server"><em>Confidence %</em><strong>: </strong> <label id="confidenceLabel" runat="server"></label> </p>
        <p style="font-size: 16px" visible="false" id="reason" runat="server"><em>Reasoning</em><strong>: </strong> <label id="reasoningLabel" runat="server"></label></p>
        </div>
    </div>

</asp:Content>
