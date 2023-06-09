using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Document
/// </summary>
public class Article
{
    long aID;
    string aTxt;

        public long arID
        {
            get { return aID; }
            set { aID = value; }
        }
   
        public string arTxt
        {
            get { return aTxt; }
            set { aTxt = value; }
        }

    private Dictionary<string, List<string>> words;

    public Dictionary<string, List<string>> Words
    {
        get { return words; }
        set { words = value; }
    }
    public Article(long aID, string aTxt)
    {
        words = new Dictionary<string, List<string>>();
        this.aID = aID;
        this.aTxt = aTxt;
    }

}