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
            <p class="lead">
                Please enter your Recommendations <strong>Account Key</strong>:&nbsp;&nbsp;&nbsp;
                <asp:TextBox ID="ModelKey" runat="server" OnTextChanged="ModelKey_TextChanged" AutoPostBack="true"></asp:TextBox>
            </p>
            <p class="lead">
                Select a Trained Model from the List:&nbsp;&nbsp;&nbsp;
                <asp:DropDownList ID="ModelSelect" runat="server" OnSelectedIndexChanged="ModelSelect_SelectedIndexChanged" >
                </asp:DropDownList>
                </p>
        </div>
        <div class="row">
            <h2>Partsunlimited Products</h2>
            <table>
                <tr>
                    
                    <td>
                         <div class='col-md-8'>
                             <div class="carousel slide media-carousel" id="products">
                                 <div class="carousel-inner">
                                     <asp:Repeater ID="productdetails" runat="server" OnItemCommand="productdetails_ItemCommand" >
                                         <ItemTemplate>
                                             
                                             <div class="item <%# (Container.ItemIndex == 0 ? "active" : "") %>">
                                <div class="row">
                          <div class="col-md-4">
                              <asp:ImageButton runat="server" CommandName="Click" AlternateText="<%# ((RecViewerApp.ProductDetailsDisplay)Container.DataItem).title%>" ImageUrl="<%# ((RecViewerApp.ProductDetailsDisplay)Container.DataItem).productArtUrl%>" CommandArgument="<%# ((RecViewerApp.ProductDetailsDisplay)Container.DataItem).productID%>" Height="200" Width="400" CausesValidation="false" />
                            <!-- <a  class="thumbnail" href="#"><img alt="" src='<%# ((RecViewerApp.ProductDetailsDisplay)Container.DataItem).productArtUrl%>'></a>-->
                          </div>          
                            <!-- <asp:Button ID="btn" CommandName="Click" Text="myBtn" runat="server" 
                        CommandArgument='<%# ((RecViewerApp.ProductDetailsDisplay)Container.DataItem).productID%>' /> -->
                    </div>
                          </div>
                        

                                         </ItemTemplate>
                                      </asp:Repeater>  
                                  </div>
                                   <a data-slide="prev" href="#products" class="left carousel-control">‹</a>
                                    <a data-slide="next" href="#products" class="right carousel-control">›</a>

                              </div>
                          </div>
                    </td>
                    
                </tr>
            </table>
          
        </div>
        <div class="row">
            <h2>Recommendations</h2>
            <table>
                <tr>
                    
                    <td>
                         <div class='col-md-8'>
                             <div class="carousel slide media-carousel" id="recs">
                                 <div class="carousel-inner">
                                     <asp:Repeater ID="recommendations" runat="server" OnItemCommand="recdetails_ItemCommand" >
                                         <ItemTemplate>
                                             
                                             <div class="item <%# (Container.ItemIndex == 0 ? "active" : "") %>">
                                <div class="row">
                          <div class="col-md-4">
                              <asp:Image runat="server" CommandName="Click" AlternateText="<%# ((RecViewerApp.ProductDetailsDisplay)Container.DataItem).title%>" ImageUrl="<%# ((RecViewerApp.ProductDetailsDisplay)Container.DataItem).productArtUrl%>" CommandArgument="<%# ((RecViewerApp.ProductDetailsDisplay)Container.DataItem).productID%>" Height="200" Width="400" CausesValidation="false" />
                            <!-- <a  class="thumbnail" href="#"><img alt="" src='<%# ((RecViewerApp.ProductDetailsDisplay)Container.DataItem).productArtUrl%>'></a>-->
                          </div>          
                            <!-- <asp:Button ID="btn" CommandName="Click" Text="myBtn" runat="server" 
                        CommandArgument='<%# ((RecViewerApp.ProductDetailsDisplay)Container.DataItem).productID%>' /> -->
                    </div>
                          </div>
                        

                                         </ItemTemplate>
                                      </asp:Repeater>  
                                  </div>
                                   <a data-slide="prev" href="#products" class="left carousel-control">‹</a>
                                    <a data-slide="next" href="#products" class="right carousel-control">›</a>

                              </div>
                          </div>
                    </td>
                    
                </tr>
            </table>
        </div>
    </div>

</asp:Content>
