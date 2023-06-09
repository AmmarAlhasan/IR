using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace InfoRetSoulution
{
    public partial class Search : System.Web.UI.Page
    {
        Indexer i = new Indexer();
        protected void Page_Load(object sender, EventArgs e)
        { }

        protected void SearchOnClick(object sender, EventArgs e)
        {
           
            Label1.Text = "";
            char[] notAllowedBoolean = new char[] { '=', '+', '-', '/', ',', '.', '\'', '\"', '*', '~', '!', '@', '#', '$', '%', '^', '&', '(', ')', '{', '}', '[', ']', ':', ';', '<', '>', '?', '|', '\\' };
            char[] notAllowedBoolean2 = new char[] { '=', '+', '-', '/', ',', '.', '\'', '\"', '*', '~', '@', '#', '$', '%', '^', '{', '}', '[', ']', ':', ';', '<', '>', '?', '\\' };

            List<Article> searchResult = new List<Article>();

            string q = TextBox1.Text.Trim();

            for (int j = 0; j < notAllowedBoolean.Length; j++)
            {
                if (q.Contains(notAllowedBoolean[j]))
                {
                    Label1.Text = "Not allowed Query";
                }
            }
            Repeater1.Visible = false;
            if (Label1.Text != "Not allowed Query")
            {
                Label1.Text = "";
                i.InitializeQueryVector(q, 1);
                searchResult = i.VectorspaceModelQuery(q);
                Label1.Text = "";

                info1.Text = "tokenizer output: " + i.tokenizer_output;
                info2.Text = "remove stop word output: " + i.stopword_output;
                info3.Text = "stemmer output: " + i.stemmer_output;


            }

            if (searchResult == null)
            {
                Label1.Text = "No Matches Found";
            }

            else if (Label1.Text != "Not allowed Query")
            {
                Repeater1.Visible = true;
                Repeater1.DataSource = searchResult;
                Repeater1.DataBind();


                Label1.Text = "Number of result : " + searchResult.Count;
            }
        }// end SearchOnClick method.

                protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Article q = e.Item.DataItem as Article;

                Literal lit1 = e.Item.FindControl("Lit1") as Literal;

                string outputArts = q.arTxt;

                foreach (string term in i.QueryDistinctTerms)
                {
                    if (q.Words.Keys.Contains(term))
                    {
                        outputArts = HighlightSearchResults(q.Words[term], outputArts);
                    }
                }
                lit1.Text = outputArts;
            }
        }// end if Repeater1_ItemDataBound.




        public static string HighlightSearchResults(List<string> occurences, string input)
        {
            foreach (string occ in occurences)
            {
                input = input.Replace(occ, "<span style='background-color:yellow'>" + occ + "</span>");
            }

            return input;
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/FillDB.aspx");
        }
    }
}