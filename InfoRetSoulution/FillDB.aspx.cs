using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace InfoRetSoulution
{
    public partial class FillDB : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Submitbtn_Click(object sender, EventArgs e)
        {

            if (TextBox2.Text =="")
            {
                Label1.Text = "please fill data.";
            }
            else
            {
                string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                SqlConnection conn = new SqlConnection(connectionString);

                string sql = "insert into ArticlesTBL (aContent) values (@aContent)";
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@aContent", TextBox2.Text);

                int a = 0;
                try
                {
                    conn.Open();
                    a = cmd.ExecuteNonQuery();
                }
                catch { }
                finally
                {
                    conn.Close();
                }

                if (a == 1)
                {
                    Label1.Text = "Success.";
                    TextBox2.Text = "";
                }
                else
                {
                    Label1.Text = "Faild!";
                }
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Search.aspx");

        }

        protected void LoadData(object sender, EventArgs e)
        {

            string filePath = Server.MapPath("~/englishdataset.csv");
            string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string tableName = "ArticlesTBL";

            ReadSVCFileAndInsertIntoTable(filePath, connectionString, tableName);
        }

        protected void LoadDataAr(object sender, EventArgs e)
        {

            string filePath = Server.MapPath("~/arabicdataset.csv");
            string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string tableName = "ArticlesTBL";

            ReadSVCFileAndInsertIntoTable(filePath, connectionString, tableName);
        }

        public void ReadSVCFileAndInsertIntoTable(string filePath, string connectionString, string tableName)
        {
            List<string> data = new List<string>();

            // Read the SVC file
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    data.Add(line);
                    //string[] parts = line.Split('\t');

                    //if (parts.Length == 2)
                    //{
                    //    string content = parts[1];
                    //    data.Add(content);
                    //}
                }
            }

            // Insert the data into the database table
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                
                int a = 0;
                try
                {
                    connection.Open();

                    foreach (string content in data)
                    {
                        string query = $"INSERT INTO {tableName} (aContent) VALUES (@content)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@content", content);
                            a = command.ExecuteNonQuery();
                        }
                    }
                }
                catch { }
                finally
                {
                    connection.Close();
                }

                if (a == 1)
                {
                    Label1.Text = "Success.";
                    TextBox2.Text = "";
                }
                else
                {
                    Label1.Text = "Faild!";
                }
            }
        }
    }
}