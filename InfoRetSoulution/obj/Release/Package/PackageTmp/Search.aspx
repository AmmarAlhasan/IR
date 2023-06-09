<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="InfoRetSoulution.Search" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Search Page</title>
    <link href="css/index.css" rel="stylesheet" />
    <link href="css/colors.css" rel="stylesheet" />
    <link href="css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    
    <form id="form1" runat="server">
     <asp:Button ID="Button2" runat="server" Text="Fill DataBase" ForeColor="#000" CssClass="btn btn-success btn-fill-trans" OnClick="Button2_Click" />
               <div class="index">
                    <div class="index__logo"></div>


                        <asp:RadioButtonList  ID="RadioButtonList1" CssClass="index__nav" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Value="1">Boolean Model</asp:ListItem>
                        <asp:ListItem Value="2">Extended Boolean Model</asp:ListItem>
                        <asp:ListItem Value="3">Vector Space Model</asp:ListItem>
                        </asp:RadioButtonList>
              

                    <div class="index__search">
                        <div class="index__form">
                            <div>
                                <asp:TextBox ID="TextBox1" CssClass="index__query" runat="server" AutoPostBack="True"></asp:TextBox>
                            </div>
                                <asp:Button ID="Button1" CssClass="index__button" runat="server" OnClick="SearchOnClick"/>
                        </div>
                        
                    </div>
               

             </div>



             <div>
                 <div class="label-class">
                     <asp:Label ID="Label1" runat="server"></asp:Label>
                     <asp:Label ID="Label2" runat="server"></asp:Label>
                 </div>

                 <div class="label-class">
                     <asp:Label ID="info1" runat="server" ForeColor="#FF9900" ></asp:Label><br />
                     <asp:Label ID="info2" runat="server" ForeColor="#FF9900" ></asp:Label><br />
                     <asp:Label ID="info3" runat="server" ForeColor="#FF9900" ></asp:Label>
                 </div>
               

                 <div>
                       <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound" >
              
                        <ItemTemplate>

                            <div class="the-question-text">
                               <span style="color:black;font-size:large">
                                   <asp:Literal ID="Lit1" runat="server" />
                               </span>
                                <br />
                               <asp:Literal ID="Lit2" runat="server" /> 
                            </div>
                   
                         </ItemTemplate>
                     </asp:Repeater>
                 </div>
           </div>

    </form>
</body>
</html>
