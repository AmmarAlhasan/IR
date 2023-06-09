using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Data;
/// <summary>
/// Summary description for Indexer
/// </summary>
public class Indexer
{
    List<Article> articles;
    List<IndexRow> Index = new List<IndexRow>();
    public List<IndexRow> QueryDictionary;
    long articlesCount;
    Hashtable articlesLength;

    public string tokenizer_output = "";
    public string stopword_output = "";
    public string stemmer_output = "";

    public Indexer()
    {

        Hashtable h1 = new Hashtable();
        articles = new List<Article>();
        PorterStemmer porter = new PorterStemmer();
        ISRI stemmer = new ISRI();
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        SqlConnection conn2 = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        SqlConnection conn3 = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        SqlCommand dictionaryCommand = conn3.CreateCommand();
        SqlCommand postingCommand = conn2.CreateCommand();
        SqlCommand quesCommand = conn.CreateCommand();
        quesCommand.CommandText = "Select * from ArticlesTBL";

        try
        {
            conn.Open();
            SqlDataReader reader = quesCommand.ExecuteReader();
            while (reader.Read())
            {
                articles.Add(new Article(long.Parse(reader["aID"].ToString()),
                                           reader["aContent"].ToString()));
            }
        }
        catch (Exception e)
        {

        }
        finally
        {
            conn.Close();
        }

        articlesCount = articles.Count;
        dictionaryCommand.CommandText = "Delete from TermsIndex";
        postingCommand.CommandText = "Delete from InvertedIndex";
        try
        {
            conn3.Open();
            conn2.Open();

            postingCommand.ExecuteNonQuery();
            dictionaryCommand.ExecuteNonQuery();
        }
        catch (Exception e)
        {

            throw;
        }
        finally
        {
            conn3.Close();
            conn2.Close();
        }

        List<ArtsTermFreq> tempRows = new List<ArtsTermFreq>();

        foreach (Article q in articles)
        {
            List<string> stemmed = new List<string>();


            //tokenizing .....

            List<string> list = new List<string>();
            StringTokenizer tokenizer = new StringTokenizer(q.arTxt);
            Token token;
            StringBuilder sb = new StringBuilder();
            do
            {
                token = tokenizer.Next();
                if (token.Kind == TokenKind.Word || token.Kind == TokenKind.QuotedString)
                    list.Add(token.Value.ToLower());

            } while (token.Kind != TokenKind.EOF);


            RemoveStopWordsClass rsw = new RemoveStopWordsClass();
            List<string> list1 = new List<string>();

            foreach (string item in list)
            {
                if (rsw.removing(item) != "")
                    list1.Add(rsw.removing(item));
            }

            List<string> list2 = new List<string>();
            string xx = "";
            foreach (string str in list1)
            {

                if (str.Trim() != "")
                {



                    if (str.Trim().ToLower()[0] >= 'a' && str.Trim().ToLower()[0] <= 'z')
                        xx = porter.StemWord(str.Trim());
                    else
                        xx = stemmer.Stemming(str.Trim());

                }
                stemmed.Add(xx);
                if (!q.Words.Keys.Contains(xx))
                {
                    List<string> values = new List<string>();
                    values.Add(str);
                    q.Words.Add(xx, values);
                }
                else
                {
                    if (!q.Words[xx].Contains(str))
                        q.Words[xx].Add(str);
                }

            }


            foreach (string stemmedToken in stemmed)
            {
                tempRows.Add(new ArtsTermFreq(q.arID, stemmedToken, 1));
            }


        }

        tempRows.Sort(CompareQuesTermFreq);

        List<ArtsTermFreq> freqsList = new List<ArtsTermFreq>();

        foreach (ArtsTermFreq row in tempRows)
        {
            if (!freqsList.Contains(row))
                freqsList.Add(new ArtsTermFreq(row.ArticleNo, row.Term, 1));
            else
                freqsList.Find(x => x.ArticleNo == row.ArticleNo && x.Term == row.Term).TermFreq++;
        }


        List<string> distinct = new List<string>();
        foreach (ArtsTermFreq row in freqsList)
        {
            if (!distinct.Contains(row.Term))
            {
                distinct.Add(row.Term);
                List<ArtsTermFreq> temp = freqsList.FindAll(x => x.Term == row.Term).ToList<ArtsTermFreq>();
                int nArs = temp.Count;
                int collFreq = 0;
                LinkedList<PostingRow> posting = new LinkedList<PostingRow>();
                foreach (ArtsTermFreq row2 in temp)
                {
                    posting.AddLast(new PostingRow(row2.ArticleNo, row2.TermFreq));
                    collFreq += row2.TermFreq;
                }

                Index.Add(new IndexRow(row.Term, nArs, collFreq, posting));
            }
        }


        dictionaryCommand.CommandText = "Insert into TermsIndex values(@TermId, @term, @NumberOfArticles, @Frequency)";
        dictionaryCommand.Parameters.Add(new SqlParameter("@TermId", SqlDbType.BigInt));
        dictionaryCommand.Parameters.Add(new SqlParameter("@term", SqlDbType.NVarChar, 50));
        dictionaryCommand.Parameters.Add(new SqlParameter("@NumberOfArticles", SqlDbType.BigInt));
        dictionaryCommand.Parameters.Add(new SqlParameter("@Frequency", SqlDbType.BigInt));

        postingCommand.CommandText = "insert into InvertedIndex values(@TermId, @ArticleId, @TermFreq)";
        postingCommand.Parameters.Add(new SqlParameter("@TermId", SqlDbType.BigInt));
        postingCommand.Parameters.Add(new SqlParameter("@ArticleId", SqlDbType.BigInt));
        postingCommand.Parameters.Add(new SqlParameter("@TermFreq", SqlDbType.BigInt));

        int TermId = 0;

        try
        {
            conn3.Open();
            conn2.Open();
            foreach (IndexRow dicNode in Index)
            {
                TermId++;
                dictionaryCommand.Parameters["@TermId"].Value = TermId;
                dictionaryCommand.Parameters["@term"].Value = dicNode.Term;
                dictionaryCommand.Parameters["@NumberOfArticles"].Value = dicNode.NArs;
                dictionaryCommand.Parameters["@Frequency"].Value = dicNode.CollFreq;
                dictionaryCommand.ExecuteNonQuery();

                foreach (PostingRow postingNode in dicNode.PostingList)
                {
                    postingCommand.Parameters["@TermId"].Value = TermId;
                    postingCommand.Parameters["@ArticleId"].Value = postingNode.ArticleNo;
                    postingCommand.Parameters["@TermFreq"].Value = postingNode.TermFrequency;
                    postingCommand.ExecuteNonQuery();
                }

            }
        }
        catch (Exception e)
        {

            throw;
        }

        finally
        {
            conn3.Close();
            conn2.Close();
        }


        CalculateIDFI();
        CalculateTFI();
        CalculateWIJ();
        CalculateArticlesLength();
    }


    private void CalculateArticlesLength()
    {
        articlesLength = new Hashtable();
        foreach (long artId in GetArticlesIDs())
        {
            float sumOfSquareRoots = 0;
            foreach (PostingRow postingNode in (from x in Index.SelectMany(k => k.PostingList)
                                                where x.ArticleNo == artId
                                                select x))
            {
                sumOfSquareRoots += postingNode.WIJ * postingNode.WIJ;
            }
            articlesLength.Add(artId, Math.Sqrt(sumOfSquareRoots));
        }
    }


    private void CalculateIDFI()
    {
        foreach (string term in GetDistinctTerms())
        {
            long dfi = getDFi(term);
            float idfi = getiDFi(dfi, articlesCount);
            ((from x in Index
              where x.Term == term
              select x).SingleOrDefault() as IndexRow).IDFI = idfi;
        }
    }


    private void CalculateTFI()
    {
        foreach (long docId in GetArticlesIDs())
        {
            List<string> _terms = GetArticleDistinctTerms(docId);
            long maxfij = getMaxFij(docId);


            foreach (string term in _terms)
            {
                long fij = getFij(docId, term);
                float tfij = getTFij(fij, maxfij);
                (from x in Index.FindAll(z => z.Term == term).SelectMany(k => k.PostingList)
                 where x.ArticleNo == docId
                 select x).SingleOrDefault().TFIJ = tfij;

            }

        }
    }


    private void CalculateWIJ()
    {
        foreach (IndexRow dictionaryNode in Index)
        {
            foreach (PostingRow postingNode in dictionaryNode.PostingList)
            {
                postingNode.WIJ = postingNode.TFIJ * dictionaryNode.IDFI;
            }
        }
    }

    private float getTFij(long fij, long maxFij)
    {
        return (float)fij / maxFij;
    }
    private long getFij(long docId, string term)
    {
        return (from x in Index.FindAll(z => z.Term == term).SelectMany(k => k.PostingList)
                where x.ArticleNo == docId
                select x.TermFrequency).SingleOrDefault();
    }
    private long getMaxFij(long docId)
    {
        return (from x in Index.SelectMany(k => k.PostingList)
                where x.ArticleNo == docId
                select x.TermFrequency).Max();
    }

    private List<long> GetArticlesIDs()
    {
        List<long> list = (from x in Index.SelectMany(k => k.PostingList)
                           select x.ArticleNo).Distinct().ToList<long>();
        list.Sort();
        return list;
    }

    private long getDFi(string term)
    {
        return (from x in Index
                where x.Term == term
                select x.NArs).SingleOrDefault();
    }
    private float getiDFi(long dfi, long count)
    {
        return (float)Math.Log((float)count / dfi, 2);
    }

    //---------------------------------------------------------------search-------------------------------------------------------



    public List<Article> BooleanModelQuery(string query)
    {
        List<string> distinct = GetDistinctTerms();
        QueryDistinctTerms =
                 (from x in QueryDictionary
                  select x.Term).Distinct().ToList<string>();



        Dictionary<string, List<long>> termArticleIncidenceMatrix =
                new Dictionary<string, List<long>>();
        List<long> incidenceVector = new List<long>();
        foreach (string term in distinct)
        {
            //incidence vector for each terms
            incidenceVector = new List<long>();

            foreach (Article q in articles)
            {

                if (q.Words.ContainsKey(term))
                {
                    //document contains the term
                    incidenceVector.Add(1);

                }
                else
                {
                    //document do not contains the term
                    incidenceVector.Add(0);
                }
            }
            termArticleIncidenceMatrix.Add(term, incidenceVector);
        }

        string bitWiseOp = string.Empty;
        List<string> list = new List<string>();
        List<string> Querylist = new List<string>();
        StringTokenizer tokenizer = new StringTokenizer(query);
        Token token;
        StringBuilder sb = new StringBuilder();
        do
        {
            token = tokenizer.Next();
            if (token.Kind == TokenKind.Word || token.Kind == TokenKind.QuotedString)
                list.Add(token.Value.ToLower());

        } while (token.Kind != TokenKind.EOF);
        RemoveStopWordsClass2 rsw = new RemoveStopWordsClass2();
        List<string> list1 = new List<string>();
        foreach (string item in list)
        {
            if (rsw.removing(item) != "")
                list1.Add(rsw.removing(item));
        }

        string xx = "";
        PorterStemmer porter = new PorterStemmer();
        ISRI stemmer = new ISRI();
        foreach (string str in list1)
        {

            if (str.Trim() != "")
            {
                //if (str.Trim().ToLower()[0] >= 'a' && str.Trim().ToLower()[0] <= 'z')
                //xx = porter.StemWord(str.Trim());
                if (str.Trim().ToLower()[0] >= 'a' && str.Trim().ToLower()[0] <= 'z')
                    xx = porter.StemWord(str.Trim());
                else
                    xx = stemmer.Stemming(str.Trim());

            }
            Querylist.Add(xx);
        }//



        List<long> previousTermIncidenceV = null;
        List<long> nextTermsIncidenceV = null;
        //holds the bitwise operation result
        List<long> resultSet = null;
        //On query X AND Y, X is previousTerm term and Y is nextTerm
        Boolean hasPreviousTerm = false;
        Boolean hasNotOperation = false;
        foreach (string term in Querylist)
        {
            //is a term
            if (!term.Equals("but") && !term.Equals("not") && !term.Equals("or") && !term.Equals("and"))
            {
                //query case: structure AND NOT analysis
                if (hasNotOperation)
                {

                    if (hasPreviousTerm)
                    {
                        nextTermsIncidenceV = ProcessBooleanOperator("not",
                          termArticleIncidenceMatrix[term.ToLower()], nextTermsIncidenceV);
                    }
                    //query case: eg.NOT analysis
                    else
                    {
                        previousTermIncidenceV = ProcessBooleanOperator("not",
                          termArticleIncidenceMatrix[term.ToLower()], nextTermsIncidenceV);
                        resultSet = previousTermIncidenceV;
                    }
                    hasNotOperation = false;
                }
                else if (!hasPreviousTerm)
                {
                    if (!termArticleIncidenceMatrix.ContainsKey(term.ToLower())) return null;
                    previousTermIncidenceV = termArticleIncidenceMatrix[term.ToLower()];
                    resultSet = previousTermIncidenceV;
                    hasPreviousTerm = true; //
                }
                else
                {

                    nextTermsIncidenceV = termArticleIncidenceMatrix[term.ToLower()];
                }
            }
            else if (term.Equals("not"))
            {
                //indicates that the  term in the next iteration should be complemented.
                hasNotOperation = true;
            }
            else
            {
                //'BUT' also should be evaluated as AND eg. structure BUT
                //NOT semantic should be evaluated as structure AND NOT semantic
                if (term.Equals("but"))
                {
                    bitWiseOp = "and";
                }
                else
                    bitWiseOp = term;
            }

            if (nextTermsIncidenceV != null && !hasNotOperation)
            {
                resultSet = ProcessBooleanOperator(bitWiseOp,
                                 previousTermIncidenceV, nextTermsIncidenceV);
                previousTermIncidenceV = resultSet;
                hasPreviousTerm = true;
                nextTermsIncidenceV = null;
            }
        }
        List<Article> searchResult = new List<Article>();
        if (resultSet == null) return null;
        int cont = resultSet.Count();
        int vv = 1;

        do
        {
            if (resultSet[vv] == 1)
                searchResult.Add(articles[vv]);
            vv++;
        } while (vv < cont);

        return searchResult;

    }

    public static List<long> ProcessBooleanOperator(string op,
          List<long> previousTermV, List<long> nextTermV)
    {
        List<long> resultSet = new List<long>();
        if (op.Equals("not"))
        {
            foreach (int a in previousTermV)
            {
                if (a == 1)
                {
                    resultSet.Add(0);
                }
                else
                {
                    resultSet.Add(1);
                }
            }
        }
        else if (op.ToLower().Equals("and")) //bitwise AND operation
        {
            for (int a = 0; a < previousTermV.Count; a++)
            {
                if (previousTermV[a] == 1 && nextTermV[a] == 1)
                {
                    resultSet.Add(1);
                }
                else
                {
                    resultSet.Add(0);
                }
            }
        }
        else if (op.ToLower().Equals("or")) //bitwise OR operation
        {
            for (int a = 0; a < previousTermV.Count; a++)
            {
                if (previousTermV[a] == 0 && nextTermV[a] == 0)
                {
                    resultSet.Add(0);
                }
                else
                {
                    resultSet.Add(1);
                }
            }
        }
        return resultSet;
    }
    private List<string> GetDistinctTerms()
    {
        List<string> list = (from x in Index
                             select x.Term).Distinct().ToList<string>();
        list.Sort();
        return list;
    }

    private List<string> GetArticleDistinctTerms(long docId)
    {
        return (from x in Index
                where (x.PostingList.Any(k => k.ArticleNo == docId))
                select x.Term).Distinct().ToList<string>();
    }
    public static int CompareQuesTermFreq(ArtsTermFreq x, ArtsTermFreq y)
    {
        if (x.Term == y.Term)
            return (x.ArticleNo.CompareTo(y.ArticleNo));
        return x.Term.CompareTo(y.Term);
    }

    List<string> queryDistinctTerms;
    public List<string> QueryDistinctTerms
    {
        set { queryDistinctTerms = value; }
        get { return queryDistinctTerms; }
    }

    private List<string> GetQueryDistinctTerms()
    {
        List<string> list = (from x in QueryDictionary
                             select x.Term).Distinct().ToList<string>();
        list.Sort();
        return list;
    }

    private float CalculateCosineSimilarity(long docId)
    {
        float sum = 0;
        foreach (string term in GetArticleDistinctTerms(docId).Intersect(GetQueryDistinctTerms()))
        {
            float wij1 = (from x in Index
                          where x.Term == term
                          select x.PostingList.First(z => z.ArticleNo == docId)).Select(k => k.WIJ).First();
            float wij2 = (from x in QueryDictionary
                          where x.Term == term
                          select x.PostingList.First()).Select(k => k.WIJ).First();
            sum += wij1 * wij2;
        }
        float docLength = float.Parse(articlesLength[docId].ToString());
        float queryLength = float.Parse(articlesLength[-1].ToString());

        return sum / (docLength * queryLength);
    }

    public void InitializeQueryVector(string query, int ut)
    {
        articlesLength.Remove(-1);

        QueryDictionary = new List<IndexRow>();
        List<string> list = new List<string>();
        List<string> stemmed = new List<string>();
        List<string> Querylist = new List<string>();


        StringTokenizer tokenizer = new StringTokenizer(query);

        StringTokenizer1 tokenizer1 = new StringTokenizer1(query);

        Token token;
        StringBuilder sb = new StringBuilder();
        do
        {
            if (ut == 1)
            {
                token = tokenizer.Next();


                if (token.Kind == TokenKind.Word || token.Kind == TokenKind.QuotedString)
                    list.Add(token.Value.ToLower());
            }
            else
            {
                token = tokenizer1.Next();
                list.Add(token.Value.ToLower());
            }

        } while (token.Kind != TokenKind.EOF);

        // tokenizer output = list
        foreach (string toki in list)
        {
            tokenizer_output = tokenizer_output + " - " + toki;
        }

        RemoveStopWordsClass rsw = new RemoveStopWordsClass();
        List<string> list1 = new List<string>();
        foreach (string item in list)
        {
            if (rsw.removing(item) != "")
                list1.Add(rsw.removing(item));
        }

        // remove stop words output = xx
        foreach (string stopi in list1)
        {
            stopword_output = stopword_output + " - " + stopi;
        }

        string xx = "";
        PorterStemmer porter = new PorterStemmer();
        ISRI stemmer = new ISRI();
        foreach (string str in list1)
        {

            if (str.Trim() != "")
            {
                
                if (str.Trim().ToLower()[0] >= 'a' && str.Trim().ToLower()[0] <= 'z')
                    xx = porter.StemWord(str.Trim());
                else
                    xx = stemmer.Stemming(str.Trim());

            }
            stemmed.Add(xx);
        }
        if (stemmed.Count == 0)
        {
            return;
        }

        // stemmer output:
        foreach (string stemmi in stemmed)
        {
            stemmer_output = stemmer_output + " - " + stemmi;
        }

        List<ArtsTermFreq> tempRows = new List<ArtsTermFreq>();
        foreach (string stemmedToken in stemmed)
        {
            tempRows.Add(new ArtsTermFreq(-1, stemmedToken, 1));
        }
        if (ut == 1)
            tempRows.Sort(CompareQuesTermFreq);

        List<ArtsTermFreq> listWithFreqs = new List<ArtsTermFreq>();
        foreach (ArtsTermFreq row in tempRows)
        {
            if (!listWithFreqs.Contains(row))
                listWithFreqs.Add(new ArtsTermFreq(row.ArticleNo, row.Term, 1));
            else
                listWithFreqs.Find(x => x.ArticleNo == row.ArticleNo && x.Term == row.Term).TermFreq++;
        }
        List<string> distinctTerms = new List<string>();
        foreach (ArtsTermFreq row in listWithFreqs)

        {
            if (!distinctTerms.Contains(row.Term))
            {
                distinctTerms.Add(row.Term);
                List<ArtsTermFreq> temp = listWithFreqs.FindAll(x => x.Term == row.Term).ToList<ArtsTermFreq>();
                int nArs = temp.Count;
                int collFreq = 0;
                LinkedList<PostingRow> posting = new LinkedList<PostingRow>();
                foreach (ArtsTermFreq row2 in temp)
                {
                    posting.AddLast(new PostingRow(row2.ArticleNo, row2.TermFreq));
                    collFreq += row2.TermFreq;
                }
                IndexRow node = new IndexRow(row.Term, nArs, collFreq, posting);
                IndexRow dn = (from x in Index
                               where x.Term == row.Term
                               select x).SingleOrDefault();
                if (dn != null)
                {
                    node.IDFI = dn.IDFI;
                }
                else
                    node.IDFI = (float)0.0;
                QueryDictionary.Add(node);
            }
        }
        long maxFreq = (from x in QueryDictionary.SelectMany(k => k.PostingList)
                        select x.TermFrequency).Max();
        foreach (IndexRow node in QueryDictionary)
        {
            PostingRow postingNode = node.PostingList.Single();
            postingNode.TFIJ = (float)postingNode.TermFrequency / maxFreq;
            postingNode.WIJ = postingNode.TFIJ * node.IDFI;
        }

        float sumOfSquareRoots = 0;
        foreach (PostingRow postingNode in (from x in QueryDictionary.SelectMany(k => k.PostingList)
                                            select x))
        {
            sumOfSquareRoots += postingNode.WIJ * postingNode.WIJ;
        }
        articlesLength.Add(-1, Math.Sqrt(sumOfSquareRoots));
    } ///

    public Dictionary<int, float> ProssesExtended(string query)
    {
        Dictionary<int, float> simulatiry = new Dictionary<int, float>();
        QueryDistinctTerms =
                 (from x in QueryDictionary
                  select x.Term).Distinct().ToList<string>();
        List<long> articleIDsMatchOneQueryTerm =
                (from y in
                     (from x in Index
                      where QueryDistinctTerms.Contains(x.Term)
                      select x).SelectMany(z => z.PostingList)
                 select y.ArticleNo).Distinct().ToList<long>();
        if (articleIDsMatchOneQueryTerm.Count == 0)
            return null;
        string str = query;
        List<Article> Artssss = new List<Article>();
        string xx = "";
        PorterStemmer porter = new PorterStemmer();
        ISRI stemmer = new ISRI();
        foreach (string rr in QueryDistinctTerms)
        {

            if (rr.Trim() != "")
            {
                //if (str.Trim().ToLower()[0] >= 'a' && str.Trim().ToLower()[0] <= 'z')
                // xx = porter.StemWord(str.Trim());
                if (rr.Trim().ToLower()[0] >= 'a' && rr.Trim().ToLower()[0] <= 'z')
                    xx += " " + porter.StemWord(rr.Trim());
                else
                    xx += " " + stemmer.Stemming(rr.Trim());

            }

        }
        str = xx;
        foreach (long id in articleIDsMatchOneQueryTerm)
        {
            Artssss.Add((from x in articles where x.arID == id select x).Single());
        }
        Dictionary<string, float> tokens_wieght = new Dictionary<string, float>();
        foreach (Article q in Artssss)
        {
            float sum = 0;
            foreach (string term in GetArticleDistinctTerms(q.arID).Intersect(GetQueryDistinctTerms()))
            {
                float wij1 = (from x in Index
                              where x.Term == term
                              select x.PostingList.First(z => z.ArticleNo == q.arID)).Select(k => k.WIJ).First();
                tokens_wieght.Add(term, wij1);
            }

            /////////////////////////////////
            Stack<char> opera_and = new Stack<char>();
            Stack<char> opera_or = new Stack<char>();
            Stack<char> opera_not = new Stack<char>();
            Stack<token1> item = new Stack<token1>();
            Stack<token1> item_and = new Stack<token1>();
            Stack<token1> item_or = new Stack<token1>();
            Stack<token1> item_not = new Stack<token1>();
            Stack<char> open_arow = new Stack<char>();
            Stack<char> close_arow = new Stack<char>();
            Stack<float> value = new Stack<float>();
            List<char> stop = new List<char>();
            float sim = 0;
            stop.Add(' '); stop.Add('('); stop.Add(')'); stop.Add('|'); stop.Add('&'); stop.Add('!');
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '(') open_arow.Push(str[i]);
                else
                    if (str[i] == ')')
                {
                    close_arow.Push(str[i]);
                    if (opera_and.Count > 0 && item_and.Count > 1)
                    {
                        node n = new node();
                        float v = n.eval_and(item_and);
                        value.Push(v);
                        token1 t = new token1();
                        t.value = v;
                        t.x = "calculated_befor";
                        item.Push(t);
                        opera_and = new Stack<char>();
                        if (close_arow.Count > 0)
                        {
                            close_arow.Pop();
                            open_arow.Pop();
                        }
                    }
                }
                else

                        if (str[i] == '&' && opera_not.Count == 0 && opera_or.Count == 0 && item.Count > 0)
                {
                    opera_and.Push(str[i]); item_and.Push(item.Pop());
                }
                else
                            if (str[i] == '&' && opera_not.Count == 0 && opera_or.Count == 0)
                {
                    opera_and.Push(str[i]);
                    if (item.Count > 0)
                    {
                        item_and.Push(item.Pop());
                    }
                }
                else
                                if ((str[i] == '&' && opera_and.Count == 0) && (opera_or.Count != 0 || close_arow.Count > 0))
                {
                    node n = new node();
                    if (opera_or.Count != 0)
                    {
                        float v = n.eval_or(item_or);
                        value.Push(v);
                        token1 t = new token1();
                        t.value = v;
                        t.x = "calculated_befor";
                        item_and.Push(t);
                        opera_or = new Stack<char>();
                    }
                    opera_and.Push(str[i]);
                    if (close_arow.Count > 0)
                    {
                        close_arow.Pop();
                        open_arow.Pop();
                    }
                }
                else

                                    if ((str[i] == '|') && (opera_and.Count == 0) && (opera_not.Count == 0) && (item.Count > 0))
                {
                    opera_or.Push(str[i]); item_or.Push(item.Pop());
                }
                else
                                        if ((str[i] == '|') && (opera_and.Count == 0) && (opera_not.Count == 0))
                {
                    opera_or.Push(str[i]);
                    if (item.Count > 0)
                    {
                        item_or.Push(item.Pop());
                    }
                }
                else
                                            if ((str[i] == '|' && opera_or.Count != 0 && open_arow.Count == 0) && (opera_and.Count != 0 || close_arow.Count > 0))
                {
                    node n = new node();
                    if (opera_and.Count > 0)
                    {
                        float v = n.eval_and(item_and);
                        value.Push(v);
                        token1 t = new token1();
                        t.value = v;
                        t.x = "calculated_befor";
                        item_or.Push(t);
                        opera_and = new Stack<char>();
                    }
                    opera_or.Push(str[i]);
                    if (close_arow.Count > 0)
                    {
                        close_arow.Pop();
                        open_arow.Pop();
                    }
                }
                else
                                                if (str[i] == '|') opera_or.Push(str[i]);
                else

                                                    if (str[i] == '!')
                {
                    opera_not.Push(str[i]);
                    if (item.Count > 0)
                        item_not.Push(item.Pop());
                }

                else

                                                        if (!stop.Contains(str[i]))
                {
                    string word = "";
                    while (!stop.Contains(str[i]))
                    {
                        word += str[i]; i++; if (i == str.Length) break;
                    }
                    i--;
                    token1 t = new token1();
                    t.x = word;
                    foreach (KeyValuePair<string, float> p in tokens_wieght)
                        if (p.Key == word)
                            t.value = p.Value;
                    if (opera_and.Count > 0 && opera_not.Count == 0)
                    {
                        item_and.Push(t);

                    }
                    else
                        if (opera_and.Count > 0 && opera_not.Count != 0 && open_arow.Count == 0)
                    {

                        item_and.Push(t);
                        token1 tt = item_and.Pop();
                        tt.value = 1 - tt.value;
                        item_and.Push(tt);
                    }

                    else
                            if (opera_or.Count > 0 && opera_not.Count == 0)
                    {
                        item_or.Push(t);
                    }
                    else
                                if (opera_or.Count == 1 && opera_not.Count != 0)
                    {


                        token1 tt = item_not.Pop();
                        tt.value = 1 - tt.value;
                        item_or.Push(tt);
                        item_or.Push(t);

                    }
                    else
                                    if (opera_not.Count > 0)
                    {
                        item_not.Push(t);
                    }
                    else
                        item.Push(t);

                }
            }
            if (item_or.Count > 0 && item_and.Count > 0 && opera_and.Count > 0)
            {
                node n = new node();
                float v = n.eval_or(item_or);
                value.Push(v);

                token1 t = new token1();
                t.value = v;
                t.x = "calculated_befor";
                item_and.Push(t);
                opera_or = new Stack<char>();


                if (close_arow.Count > 0)
                {
                    close_arow.Pop();
                    open_arow.Pop();
                }
            }

            if (item_or.Count > 0 && item_and.Count == 0)
            {

                node n = new node();
                float v = n.eval_or(item_or);
                value.Push(v);
                token1 t = new token1();
                t.value = v;
                t.x = "calculated_befor";
                sim = v;
                opera_or = new Stack<char>();


                if (close_arow.Count > 0)
                {
                    close_arow.Pop();
                    open_arow.Pop();
                }
            }

            if (item_and.Count > 0 && item_or.Count == 0 && opera_or.Count > 0)
            {
                node n = new node();
                float v = n.eval_and(item_and);
                value.Push(v);

                token1 t = new token1();
                t.value = v;
                t.x = "calculated_befor";
                item_or.Push(t);
                opera_and = new Stack<char>();


                if (close_arow.Count > 0)
                {
                    close_arow.Pop();
                    open_arow.Pop();
                }
            }
            if (item_and.Count > 0 && item_or.Count == 0)
            {

                node n = new node();
                float v = n.eval_and(item_and);
                value.Push(v);
                token1 t = new token1();
                t.value = v;
                t.x = "calculated_befor";
                sim = v;
                opera_and = new Stack<char>();


                if (close_arow.Count > 0)
                {
                    close_arow.Pop();
                    open_arow.Pop();
                }
            }

            sim = (float)value.Pop();

            simulatiry.Add((int)q.arID, sim);
            tokens_wieght = new Dictionary<string, float>();
            ///////////////////////////////



        }





        if (simulatiry == null) return null;
        return simulatiry;
    }

    public Article getQuesById(int i)
    {
        return (from x in articles where x.arID == i select x).Single();
    }


    class token1
    {
        public string x;
        public float value;
        public token1()
        {
            value = 0;
            x = "";
        }
    }
    class node
    {
        public string op;
        public float left;
        public float right;
        public float value;
        public node()
        {
            op = "";
            left = 0;
            right = 0;
            value = 0;

        }
        public float eval(float x, float y, string op)
        {
            float result = 0;
            if (op == "&")
            {
                result = (float)(1 - Math.Sqrt((Math.Pow((1 - x), 2) + Math.Pow((1 - y), 2)) / 2));
            }
            if (op == "|")
            {
                result = (float)(Math.Sqrt((Math.Pow(x, 2) + Math.Pow(y, 2)) / 2));
            }
            if (op == "!")
            {
                result = 1 - x;
            }
            return result;
        }
        public float eval_and(Stack<token1> item)
        {
            float result = 0;
            float x = 0;
            int i = 0;
            while (item.Count > 0)
            {
                float y = 1 - item.Pop().value;
                x += (float)Math.Pow(y, 2);
                i++;
            }
            x = x / i;
            x = (float)Math.Sqrt(x);
            result = (float)(1 - x);
            return result;
        }
        public float eval_or(Stack<token1> item)
        {
            float result = 0;
            float x = 0;
            int i = 0;
            while (item.Count > 0)
            {
                float y = item.Pop().value;
                x += (float)Math.Pow(y, 2);
                i++;
            }
            x = x / i;
            x = (float)Math.Sqrt(x);

            result = (float)(x);

            return result;
        }
    } //
    public List<Article> VectorspaceModelQuery(string query)
    {
        QueryDistinctTerms =
            (from x in QueryDictionary
             select x.Term).Distinct().ToList<string>();
        List<long> articleIDsMatchOneQueryTerm =
            (from y in
                 (from x in Index
                  where QueryDistinctTerms.Contains(x.Term)
                  select x).SelectMany(z => z.PostingList)
             select y.ArticleNo).Distinct().ToList<long>();

        if (articleIDsMatchOneQueryTerm.Count == 0)
            return null;
        Dictionary<long, float> searchResults = new Dictionary<long, float>();
        foreach (long ques in articleIDsMatchOneQueryTerm)
        {
            searchResults.Add(ques, CalculateCosineSimilarity(ques));
        }
        List<long> articleIDs = (from entry in searchResults orderby entry.Value descending select entry.Key).ToList<long>();
        List<Article> Artssss = new List<Article>();
        foreach (long id in articleIDs)
        {
            Artssss.Add((from x in articles where x.arID == id select x).Single());
        }
        if (Artssss == null) return null;
        return Artssss;

    }
}

namespace InfoRetSoulution
{

}