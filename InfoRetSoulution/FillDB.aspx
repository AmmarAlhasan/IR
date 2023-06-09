<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FillDB.aspx.cs" Inherits="InfoRetSoulution.FillDB" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <style type="text/css">
        .auto-style1 {
            height: 22px;
        }
        .auto-style2 {
            height: 22px;
            width: 192px;
        }
        .auto-style6 {
            height: 197px;
            width: 192px;
        }
        .auto-style7 {
            height: 197px;
        }
        .auto-style8 {
            height: 22px;
            width: 566px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <div class="container">
            <div class="row">
                <div class="col-12">
                    <p>you can past your text document here for submit to database</p>
                </div>
                <div class="col-12">
                        <div style="width:1000px">
    
                            <table style="border: 1px solid #DDD; padding: 1px; margin: 1px; width: 100%; background-color: #EEE; text-align: left; font-family: Arial, Helvetica, sans-serif; color: #000000; display: block;">
                               
                                <tr>
                                    <td class="auto-style6">Past your text here:</td>
                                    <td class="auto-style7" colspan="2">
                                        <asp:TextBox ID="TextBox2" runat="server" BackColor="Silver" BorderColor="#66CCFF" Height="200px" TextMode="MultiLine" Width="772px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="auto-style2"></td>
                                    <td class="auto-style8">
                                        <asp:Button ID="Button1" runat="server" Text="Submit" Width="99px" OnClick="Submitbtn_Click" />
                                        <asp:Button ID="Button2" runat="server" Text="Go To Search" OnClick="Button2_Click" />
                                        <asp:Button ID="Button3" runat="server" Text="Load En Data" OnClick="LoadData" />
                                        <asp:Button ID="Button4" runat="server" Text="Load Ar Data" OnClick="LoadDataAr" />
                                        <asp:Label ID="Label1" runat="server" ForeColor="Lime"></asp:Label>
                                    </td>
                                    <td class="auto-style1"></td>
                                </tr>
                            </table>
                     </div>
                </div>
                <div class="col-12" style="min-height:100px">

                </div>
                
                
            </div>
        </div>
    </form>
</body>
</html>
