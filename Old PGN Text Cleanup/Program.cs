using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Old_PGN_Text_Cleanup
{
    class Program
    {
        static void Main(string[] args)
        {
            string infilename = "complete.pgn";
            string outfilename = "clean.pgn";

            System.IO.StreamReader inFile = new System.IO.StreamReader(infilename);
            System.IO.StreamWriter outFile = new System.IO.StreamWriter(outfilename);

            string line;
            while( (line=inFile.ReadLine()) != null )
            {
                int round = -1;
                string outLine = line;

                if (line.IndexOf("[White ") == 0 || line.IndexOf("[Black ") == 0)
                {
                    string name = GetValueFromTag(line);
                    // reverse name
                    string updName = ReverseName( name );

                    outLine = line.Substring(0, 8) + updName + "\"]";
                }
               
                if (line.IndexOf("[Date") == 0)
                {
                    int year, month, day;
                    
                    // fix the date - should be YYYY.MM.DD
                    string possDate = GetValueFromTag(line);

                    ReadDateFromTag(possDate, out year, out month, out day, out round);

                    outLine = "[Date \"" +
                        (year >= 0 ? year.ToString() : "??") + "." +
                        (month >= 0 ? month.ToString("D2") : "??") + "." +
                        (day >= 0 ? day.ToString("D2") : "??") +
                        "\"]";
                }

                if (line.IndexOf("[Event") == 0)
                {
                    int lIndex = -1;

                    // ", Round 2"
                    // ", Rx"
                    // " Round x"
                    // " - Round x"
                    // "-Round x"
                    // " Rx"

                    // see if you can fix the round
                    string[] roundTags = new string[] { ", round x", " - round x", "-round x", " round x", ", rx", " rx" };

                    int rCloseTag = line.IndexOf("\"]");
                    char lastTagChar = line[rCloseTag - 1];
                    if (Char.IsDigit(lastTagChar))
                    {
                        bool rdBiggerThan10 = false;
                        if (line[rCloseTag - 2] == '1')
                        {
                            rdBiggerThan10 = true;
                        }
                        foreach (string rdTag in roundTags)
                        {
                            int startLoc = rCloseTag - rdTag.Length - (rdBiggerThan10 ? 1 : 0);
                            string testStr = line.Substring(startLoc, rdTag.Length + (rdBiggerThan10 ? 1 : 0)).ToLower();
                            string testTagString = rdTag.ToLower();
                            if (rdBiggerThan10)
                                testTagString = testTagString.Replace("x", "1x");
                            testTagString = testTagString.Replace('x', lastTagChar);
                            if (testTagString.CompareTo(testStr.ToLower()) == 0)
                            {
                                round = (rdBiggerThan10 ? 10 : 0) + lastTagChar - '0';
                                outLine = line.Substring(0, startLoc)+"\"]";
                                break;
                            }
                        }
                    }
                }

                outFile.WriteLine( outLine );
                if (round > 0)
                    outFile.WriteLine("[Round \""+round.ToString()+"\"]");
            }
            outFile.Flush();
            outFile.Close();
        
        }
        static string GetValueFromTag(string tag)
        {
            int lIndex = tag.IndexOf('\"');
            if (lIndex >= 0)
            {
                int rIndex = tag.IndexOf('\"', lIndex + 1);
                if (rIndex >= 0)
                    return tag.Substring(lIndex+1, rIndex - lIndex - 1);
            }
            return "";
        }
        static void ReadDateFromTag(string v, out int y, out int m, out int d, out int rd)
        {
            y = m = d = rd = -1;
            if( v == "" )
                return;
            string l, c, r;
            int li, ri, ei;
            li = v.IndexOfAny(new char[] { '.', '/', '-', ' ' });
            ri = v.IndexOfAny(new char[] { '.', '/', '-', ' ' }, li + 1);
            ei = v.IndexOfAny(new char[] { '.', '/', '-', ' ' }, ri + 1);
            if (ei == -1)
                ei = v.Length;
            l = v.Substring(0, li);
            c = v.Substring( li+1, ri-li-1 );
            r = v.Substring( ri+1, ei-ri-1);

            bool textmonth = false;

            if (!(l == "" || l == "??"))
                Int32.TryParse(l, out d);
            if (!(c == "" || c == "??") )
                if (!Int32.TryParse(c, out m))
                {
                    textmonth = true;
                    switch (c.ToLower())
                    {
                        case "jan": m = 1; break;
                        case "feb": m = 2; break;
                        case "mar": m = 3; break;
                        case "apr": m = 4; break;
                        case "may": m = 5; break;
                        case "jun": m = 6; break;
                        case "jul": m = 7; break;
                        case "aug": m = 8; break;
                        case "sep": m = 9; break;
                        case "oct": m = 10; break;
                        case "nov": m = 11; break;
                        case "dec": m = 12; break;
                        default: m = -1; break;
                    }
                }

            if (!(r == "" || r == "??"))
                Int32.TryParse(r, out y);

            // [Date "2002.05.??"]
            // [Date "30-May-93"]
            // [Date "6-12-97"]
            if (d > 50)                // ok - have a year... first case
            {
                int temp = y;
                y = d;
                if (y < 1900)
                    y += 1900;
                d = temp;
            }
            else if (textmonth)        // ok - second case
            {
                if (y > 0 && y > 50 && y < 100)
                    y += 1900;
                else if (y > 0 && y < 50 )
                    y += 2000;
            }
            else if (d > 0)              // gotta at least have a month
            {
                int temp = d;
                d = m;
                m = temp;
            }

            if (y < 0)
                return;

            if (y > 0 && y > 50 && y < 100)
                y += 1900;
            else if (y > 0 && y < 50)
                y += 2000;

            int rdi = v.IndexOf("Round");

            if (rdi > 0)
            {
                Int32.TryParse(v.Substring(rdi + 6), out rd);
            }
        }
        static string ReverseName(string inName)
        {
            int lastSp = inName.LastIndexOf(' ');
            if (lastSp < 0)
                return inName;
            return inName.Substring(lastSp + 1) + ", " + inName.Substring(0, lastSp);
        }
    }
}
