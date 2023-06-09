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
        .auto-style4 {
            height: 59px;
            width: 192px;
        }
        .auto-style5 {
            height: 59px;
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
                    <p>You Can Use This Page For Add The Questions To Database.</p>
                </div>
                <div class="col-12">
                        <div style="width:1000px">
    
                            <table style="border: 1px solid #DDD; padding: 1px; margin: 1px; width: 100%; background-color: #EEE; text-align: left; font-family: Arial, Helvetica, sans-serif; color: #000000; display: block;">
                                <tr>
                                    <td class="auto-style4">&nbsp;Enter The Question:&nbsp;</td>
                                    <td class="auto-style5" colspan="2">
                                        <asp:TextBox ID="TextBox1" runat="server" BackColor="Silver" BorderColor="#99CCFF" Height="59px" Rows="2" Width="774px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="auto-style6">Enter The Answer:</td>
                                    <td class="auto-style7" colspan="2">
                                        <asp:TextBox ID="TextBox2" runat="server" BackColor="Silver" BorderColor="#66CCFF" Height="200px" TextMode="MultiLine" Width="772px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="auto-style2"></td>
                                    <td class="auto-style8">
                                        <asp:Button ID="Button1" runat="server" Text="Submit" Width="99px" OnClick="Submitbtn_Click" />
                                        <asp:Button ID="Button2" runat="server" Text="Go To Search" OnClick="Button2_Click" />
                                        <asp:Label ID="Label1" runat="server" ForeColor="Lime"></asp:Label>
                                    </td>
                                    <td class="auto-style1"></td>
                                </tr>
                            </table>
                     </div>
                </div>
                <div class="col-12" style="min-height:100px">

                </div>
                
                    <div class="col-3">
                        <h4>علاء الحسن</h4>
                        <h5>alaa_122397</h5>
                    </div>
                    <div class="col-3">
                        <h4>علي ياسين</h4>
                        <h5>ali_122633</h5>
                   </div>
                   <div class="col-3">
                       <h4>ناجي خيزران</h4>
                        <h5>naji_94836</h5>
                    </div>
                    <div class="col-3">
                        <h4>باسل شقوف</h4>
                        <h5>bassel_117367</h5>
                    </div>
                
            </div>
        </div>
    </form>
</body>
</html>
