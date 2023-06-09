using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class ArtsTermFreq : IEquatable<ArtsTermFreq>
{
    long articleNo;

    public long ArticleNo
    {
        get { return articleNo; }
        set { articleNo = value; }
    }
    string term;

    public string Term
    {
        get { return term; }
        set { term = value; }
    }

    int termFreq;

    public int TermFreq
    {
        get { return termFreq; }
        set { termFreq = value; }
    }
    public ArtsTermFreq(long _articleNo, string _term, int _termFreq)
    {
        articleNo = _articleNo;
        term = _term;
        termFreq = _termFreq;
    }

    public bool Equals(ArtsTermFreq other)
    {
        if (this.ArticleNo == other.ArticleNo & this.Term == other.Term)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}