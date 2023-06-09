using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


public class ISRI
{
    PrePost D, P3, P2, P1, S3, S2, S1;
    PrePost PR4, PR53, PR54, PR63, PR64;

    public ISRI()
    {
        //
        // TODO: Add constructor logic here
        //

        D = new PrePost(8);
        D.AddString("ْ"); D.AddString("ّ");
        D.AddString("َ"); D.AddString("ً");
        D.AddString("ُ"); D.AddString("ٌ");
        D.AddString("ِ"); D.AddString("ٍ");
        ////////////////////
        P3 = new PrePost(4);
        P3.AddString("ولل"); P3.AddString("وال");
        P3.AddString("كال"); P3.AddString("بال");
        P2 = new PrePost(2);
        P2.AddString("ال"); P2.AddString("لل");
        P1 = new PrePost(9);
        P1.AddString("ل"); P1.AddString("ب");
        P1.AddString("ف"); P1.AddString("س");
        P1.AddString("و"); P1.AddString("ي");
        P1.AddString("ت"); P1.AddString("ن");
        P1.AddString("ا");
        ////////////////////
        S3 = new PrePost(5);
        S3.AddString("تمل"); S3.AddString("همل");
        S3.AddString("تان"); S3.AddString("تين");
        S3.AddString("كمل");
        S2 = new PrePost(16);
        S2.AddString("ون"); S2.AddString("ات");
        S2.AddString("ان"); S2.AddString("ين");
        S2.AddString("تن"); S2.AddString("كم");
        S2.AddString("هن"); S2.AddString("نا");
        S2.AddString("يا"); S2.AddString("ها");
        S2.AddString("تم"); S2.AddString("كن");
        S2.AddString("ني"); S2.AddString("وا");
        S2.AddString("ما"); S2.AddString("هم");
        S1 = new PrePost(7);
        S1.AddString("ه"); S1.AddString("ة");
        S1.AddString("ي"); S1.AddString("ك");
        S1.AddString("ت"); S1.AddString("ا");
        S1.AddString("ن");
        /////////////////////
        PR4 = new PrePost(6);
        PR4.AddString("فاعل"); PR4.AddString("فعول");
        PR4.AddString("فعال"); PR4.AddString("فعيل");
        PR4.AddString("مفعل"); PR4.AddString("فعلة");
        PR53 = new PrePost(26);
        PR53.AddString("تفاعل"); PR53.AddString("افتعل");
        PR53.AddString("افعال"); PR53.AddString("افاعل");
        PR53.AddString("فعالة"); PR53.AddString("فعلان");
        PR53.AddString("فعولة"); PR53.AddString("تفعلة");
        PR53.AddString("تفعيل"); PR53.AddString("مفعلة");
        PR53.AddString("مفعول"); PR53.AddString("فاعول");
        PR53.AddString("فواعل"); PR53.AddString("مفعال");
        PR53.AddString("مفعيل"); PR53.AddString("افعلة");
        PR53.AddString("فعائل"); PR53.AddString("منفعل");
        PR53.AddString("مفتعل"); PR53.AddString("فاعلة");
        PR53.AddString("مفاعل"); PR53.AddString("فملاع");
        PR53.AddString("يفتعل"); PR53.AddString("تفتعل");
        PR53.AddString("انفعل"); PR53.AddString("فعالي");
        PR54 = new PrePost(6);
        PR54.AddString("تفعلل"); PR54.AddString("افعلل");
        PR54.AddString("فعلال"); PR54.AddString("فعالل");
        PR54.AddString("مفعلل"); PR54.AddString("فعللة");
        PR63 = new PrePost(6);
        PR63.AddString("استفعل"); PR63.AddString("مفعالة");
        PR63.AddString("افتعال"); PR63.AddString("انفعلل");
        PR63.AddString("افعوعل"); PR63.AddString("مستفعل");
        PR64 = new PrePost(3);
        PR64.AddString("افعلال"); PR64.AddString("متفعلل");
        PR64.AddString("افتعلل");
    }

    public string Stemming(string W)
    {
        string Stem = "";
        W = W.Trim();
        // 1 + 2
        for (int i = 0; i < W.Length; i++)
        {
            string S = W[i].ToString();
            if (!D.Search(S))
            {
                if ((W[i] == 'ئ') || (W[i] == 'ء')
                    || (W[i] == 'ؤ'))
                    Stem += "أ";
                else
                    Stem += S;
            }
        }
        // 3
        if (Stem.Length >= 6)
        {
            if (P3.Search2(Stem, true))
                Stem = Stem.Substring(3);
        }
        else
        {
            if (Stem.Length >= 5)
            {
                if (P2.Search2(Stem, true))
                    Stem = Stem.Substring(2);
            }
        }
        // 4
        if (Stem.Length >= 6)
        {
            if (S3.Search2(Stem, false))
                Stem = Stem.Substring(0, Stem.Length - 3);
        }
        else
        {
            if (Stem.Length >= 5)
            {
                if (S2.Search2(Stem, false))
                    Stem = Stem.Substring(0, Stem.Length - 2);
            }
        }
        // 5
        if ((Stem.Length >= 4) && (Stem[0] == 'و')
            && (Stem[1] == 'و'))
            Stem = Stem.Substring(1);
        // 6
        Stem = Stem.Replace('أ', 'ا');
        Stem = Stem.Replace('آ', 'ا');
        Stem = Stem.Replace('إ', 'ا');
        // 7
        if (Stem.Length <= 3)
            return Stem;
        // 8
        if (Stem.Length == 4)
            Stem = Word_4(Stem);
        else
        {
            if (Stem.Length == 5)
                Stem = Word_5(Stem);
            else
            {
                if (Stem.Length == 6)
                    Stem = Word_6(Stem);
                else
                {
                    if (Stem.Length == 7)
                    {
                        Stem = Short_Suffix(Stem);
                        if (Stem.Length == 6)
                            Stem = Word_6(Stem);
                        else
                        {
                            Stem = Short_Prefix(Stem);
                            if (Stem.Length == 6)
                                Stem = Word_6(Stem);
                        }
                    }
                }
            }
        }
        // 9
        return Stem;
    }

    string Short_Suffix(string W)
    {
        if (S1.Search2(W, false))
            W = W.Substring(0, W.Length - 1);
        return W;
    }

    string Short_Prefix(string W)
    {
        if (P1.Search2(W, true))
            W = W.Substring(1);
        return W;
    }

    string Word_4(string W)
    {
        string Stem = "";
        Stem = PR4.Apply(W);
        if (Stem.Length == 3)
            return Stem;
        else
        {
            Stem = Short_Suffix(Stem);
            if (Stem.Length == 4)
                Stem = Short_Prefix(Stem);
        }
        return Stem;
    }

    string Word_5(string W)
    {
        string Stem = "";
        Stem = PR53.Apply(W);
        if (Stem.Length == 3)
            return Stem;
        else
        {
            Stem = Short_Suffix(Stem);
            if (Stem.Length == 4)
                Stem = Word_4(Stem);
            else
            {
                Stem = Short_Prefix(Stem);
                if (Stem.Length == 4)
                    Stem = Word_4(Stem);
                else
                {
                    if (Stem.Length == 5)
                        Stem = PR54.Apply(Stem);
                }
            }
        }
        return Stem;
    }

    string Word_6(string W)
    {
        string Stem = "";
        Stem = PR63.Apply(W);
        if (Stem.Length == 3)
            return Stem;
        else
        {
            Stem = Short_Suffix(Stem);
            if (Stem.Length == 5)
                Stem = Word_5(Stem);
            else
            {
                Stem = Short_Prefix(Stem);
                if (Stem.Length == 5)
                    Stem = Word_5(Stem);
                else
                {
                    if (Stem.Length == 6)
                        Stem = PR64.Apply(Stem);
                }
            }
        }
        return Stem;
    }
}

class PrePost
{
    ArrayList L;
    public PrePost(int Num)
    {
        L = new ArrayList(Num);
    }

    public bool Search(string S)
    {
        return L.Contains(S);
    }

    public bool Search2(string S, bool F)
    {
        if (F)
        {
            for (int i = 0; i < L.Count; i++)
            {
                string Temp = (string)L[i];
                if (S.StartsWith(Temp))
                    return true;
            }
        }
        else
        {
            for (int i = 0; i < L.Count; i++)
            {
                string Temp = (string)L[i];
                if (S.EndsWith(Temp))
                    return true;
            }
        }
        return false;
    }

    public string Apply(string S)
    {
        for (int i = 0; i < L.Count; i++)
        {
            string Temp = (string)L[i];
            bool Flag = true;
            for (int j = 0; j < Temp.Length; j++)
            {
                if (!((Temp[j] == 'ف') || (Temp[j] == 'ع')
                    || (Temp[j] == 'ل')))
                    if (Temp[j] != S[j])
                    {
                        Flag = false;
                        break;
                    }
            }
            if (Flag)
            {
                string Stem = "";
                for (int j = 0; j < Temp.Length; j++)
                {
                    if ((Temp[j] == 'ف') || (Temp[j] == 'ع')
                        || (Temp[j] == 'ل'))
                        Stem += S[j].ToString();
                }
                return Stem;
            }
        }
        return S;
    }

    public void AddString(string S)
    {
        L.Add(S);
    }

}
