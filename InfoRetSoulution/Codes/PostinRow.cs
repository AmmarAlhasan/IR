using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class PostingRow
{
    long aNo;

    public long ArticleNo
    {
        get { return aNo; }
        set { aNo = value; }
    }
    long termFrequency;

    public long TermFrequency
    {
        get { return termFrequency; }
        set { termFrequency = value; }
    }

    float tfij;

    public float TFIJ
    {
        get { return tfij; }
        set { tfij = value; }
    }

    float wij;

    public float WIJ
    {
        get { return wij; }
        set { wij = value; }
    }

    public PostingRow(long articleNo, long termFrequency)
    {
        this.aNo = articleNo;
        this.termFrequency = termFrequency;
    }
}