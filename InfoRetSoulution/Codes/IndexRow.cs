using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class IndexRow
{
    string term;

    public string Term
    {
        get { return term; }
        set { term = value; }
    }
    int nArs;

    public int NArs
    {
        get { return nArs; }
        set { nArs = value; }
    }
    int collFreq;

    public int CollFreq
    {
        get { return collFreq; }
        set { collFreq = value; }
    }


    LinkedList<PostingRow> postingList;

    public LinkedList<PostingRow> PostingList
    {
        get { return postingList; }
        set { postingList = value; }
    }

    float idfi;

    public float IDFI
    {
        get { return idfi; }
        set { idfi = value; }
    }



    public IndexRow(string term, int nArs, int collFreq, LinkedList<PostingRow> postingList)
    {
        this.term = term;
        this.nArs = nArs;
        this.collFreq = collFreq;
        this.postingList = postingList;
    }
}