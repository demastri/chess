using Microsoft.Web.WebView2.Core;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessableToPGN
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void GenPGN_Click(object sender, EventArgs e)
        {
            if (inputChessableHTML.Text == "")
                return;

            HtmlAgilityPack.HtmlDocument analysis = new HtmlAgilityPack.HtmlDocument();

            analysis.LoadHtml(inputChessableHTML.Text);

            HtmlAgilityPack.HtmlNode titlespan = analysis.DocumentNode.SelectSingleNode("//div[@id=\"theOpeningTitle\"]/child::span");
            VariationHeader.Text = titlespan.InnerText;
            VariationHeader.Text = titlespan.InnerText;
            VariationHeader.Text = titlespan.InnerText;
            VariationHeader.Text = titlespan.InnerText;

            HtmlAgilityPack.HtmlNodeCollection chapterDetails = analysis.DocumentNode.SelectNodes("//div[@class=\"allOpeningDetails\"]/child::ol/child::li");
            ChapterHeader.Text = chapterDetails[2].InnerText;

            HtmlAgilityPack.HtmlNodeCollection movesDiv = analysis.DocumentNode.SelectNodes("//div[@id=\"theOpeningMoves\"]/child::span");

            string outText = GenerateStartText();
            outputNodeCount = 0;
            foreach (HtmlAgilityPack.HtmlNode n in movesDiv)
            {
                outText += GetTextFromNode(n);
            }
            outText += GenerateEndText();
            outputPGN.Text = outText;

            GameTag.Text = ChapterHeader.Text + " - " + VariationHeader.Text;
        }
        private string GenerateStartText()
        {
            return "[Event \"LinePGNImport\"]" + Environment.NewLine
                + "[Site \"?\"]" + Environment.NewLine
                + "[Date \"????.??.??\"]" + Environment.NewLine
                + "[Round \"\"]" + Environment.NewLine
                + "[White \"\"]" + Environment.NewLine
                + "[Black \"\"]" + Environment.NewLine
                + "[Result \"\"]" + Environment.NewLine
                + Environment.NewLine;
        }
        private string GenerateEndText()
        {
            return Environment.NewLine + "{" + VariationHeader.Text + Environment.NewLine + ".}" + Environment.NewLine + Environment.NewLine + "*" + Environment.NewLine;
        }
        static int outputNodeCount = 0;
        private string GetTextFromNode(HtmlAgilityPack.HtmlNode n)
        {
            return GetTextFromNode(n, false);

        }
        private string GetTextFromNode(HtmlAgilityPack.HtmlNode n, bool newSubVar)
        {

            outputNodeCount++; // where are we?

            string outText = "";

            foreach (HtmlAgilityPack.HtmlNode child in n.ChildNodes)
            {
                HtmlAgilityPack.HtmlNode moveNode;
                bool isWhite;
                switch (GetNodeType(child))
                {
                    case "moveNbr":
                        outText += "";   //don't do anything for now;
                        break;
                    case "comment":
                        foreach (HtmlAgilityPack.HtmlNode c in child.ChildNodes)
                        {
                            if (c.Attributes["class"].Value == "commentInVariation")
                            {
                                outText += Environment.NewLine + "{" + c.InnerText + "}" + Environment.NewLine;
                            }
                            if (c.Attributes["class"].Value.Contains("commentMoveSmall"))
                            {
                                if (newSubVar)
                                {
                                    string moveID = c.Id;
                                    int thisPly = Convert.ToInt32(moveID.Substring(0, moveID.IndexOf('.')));
                                    outText += (thisPly / 2 + 1).ToString() + ".";
                                    if (child.Attributes["class"].Value.Contains("blackMoveC"))
                                        outText += " ... ";
                                    newSubVar = false;
                                }
                                outText += BuildSANText(c, false) + " ";
                            }
                            if (c.Attributes["class"].Value == "commentSubvar" || c.Attributes["class"].Value == "commentTopvar")
                            {
                                outText += Environment.NewLine + "(" + GetTextFromNode(c, true) + ")" + Environment.NewLine;
                            }
                        }
                        break;
                    case "move":
                        moveNode = n.SelectSingleNode("./*[contains(@class, 'whiteMove') and not(contains(@class, 'commentInMove')) ]");
                        isWhite = moveNode != null;
                        if (!isWhite)
                            moveNode = n.SelectSingleNode("./*[contains(@class, 'blackMove') and not(contains(@class, 'commentInMove')) ]");
                        outText += (isWhite ? moveNode.Attributes["data-move"].Value : "") + BuildSANText(moveNode, isWhite) + " ";
                        break;
                    case "commentMove":
                        moveNode = n.SelectSingleNode("./*[contains(@class, 'whiteMove') and not(contains(@class, 'commentInMove')) ]");
                        isWhite = moveNode != null;
                        outText += BuildSANText(child, isWhite) + " ";
                        break;
                    case "unknown":
                        outText += "Unknown tag at ply " + outputNodeCount.ToString();
                        break;
                }
            }
            // first, just extract the basic pgn moves from the core line here.  move nbr, piece designator and move text
            // only need to pull the move number if it's white's move:
            // then get any text / commentary for this move
            // then get any sub variation(s) for this move and add them to the text
            return outText;

        }
        private string BuildSANText(HtmlAgilityPack.HtmlNode n, bool isWhite)
        {

            HtmlAgilityPack.HtmlNode annotationNode = n.SelectSingleNode("./child::span[@class='annotation']");
            if (n.Attributes["data-san"] == null)
                return "";
            // if this is a sub move, may need to extract the NAG from the comment text !!
            if (annotationNode != null && annotationNode.Attributes["data-original-title"] != null)
            {
                return n.Attributes["data-san"].Value + GetAnnotationNAG(annotationNode.Attributes["data-original-title"].Value, isWhite);
            }
            return n.Attributes["data-san"].Value + GetAnnotationNAGFromMoveText(n.InnerText, isWhite, containsPromotion(n.Attributes["data-san"].Value));
        }

        bool containsPromotion( String s )
        {
            int e = s.IndexOf("=");
            int pp = (e >= 0 && e+1 < s.Length ? s.ToLower()[e+1] : -1);
            return pp == 'q' || pp == 'n' || pp == 'r' || pp == 'b';
        }

        private string GetNodeType(HtmlAgilityPack.HtmlNode n)
        {
            if (n.Name == "div" && n.Attributes["class"].Value.Contains("openingNum"))
            {
                return "moveNbr";
            }
            if (n.Name == "span" && n.Attributes["class"].Value.Contains("commentInMove"))
            {
                return "comment";
            }
            if (n.Name == "span" && n.Attributes["class"].Value.Contains("commentMove"))
            {
                return "commentMove";
            }
            if (n.Name == "div" &&
                (n.Attributes["class"].Value.Contains("whiteMove") || n.Attributes["class"].Value.Contains("blackMove")))
            {
                return "move";
            }
            return "unknown";
        }

        String[] diacritics = { "??", "!?", "?!", "!!", "!", "?" };
        int[] diacriticNumber = { 4, 5, 6, 3, 1, 2 };
        String[] evals = { "-+", "+-", "+=", "=+", "±", "∓", "⩲", "⩱", "=" };
        int[] evalNumber = { 19, 18, 14, 15, 16, 17, 14, 15, 10 };

        private string GetAnnotationNAGFromMoveText(string moveText, bool isWhite, bool ispromo)
        {
            // want to strip off text that simply gets appended:
            // checks, promotions, mates
            // and then take any NAG symbols and translat them to their text counterparts
            // ### promotion should not result in '=' eval
            bool hasCheck = moveText.IndexOf("+") >= 0;
            bool hasMate = moveText.IndexOf("#") >= 0;
            String outDiacritic = "";
            String outEval = "";
            int index = 0;
            foreach( String d in diacritics)
            {
                if (moveText.IndexOf(d) >= 0)
                {
                    outDiacritic = " $"+diacriticNumber[index]+" ";
                    break;
                }
                index++;
            }
            index = 0;
            foreach (String d in evals)
            {
                if (moveText.IndexOf(d) >= 0)
                {
                    if( evalNumber[index] == 10 && ispromo)
                    {
                        continue;
                    }
                    outEval = " $" + evalNumber[index] + " ";
                    break;
                }
                index++;
            }
            

            return outDiacritic+outEval;
        }
        private string GetAnnotationNAG(string s, bool isWhite)
        {
            switch (s.ToLower())
            {
                case "": return "";
                case "good move": return " $1 ";
                case "poor move": return " $2 ";
                case "very good move": return " $3 ";
                case "excellent move": return " $3 ";
                case "blunder": return " $4 ";
                case "interesting move": return " $5 ";
                case "dubious move": return " $6 ";
                case "forced move": return " $7 ";
                case "the only move. no reasonable alternatives": return " $8 ";
                case "worst move": return " $9 ";
                case "drawish position": return " $10 ";
                case "equal chances, quiet position": return " $11 ";
                case "equal chances, active position": return " $12 ";
                case "unclear position": return " $13 ";
                case "white has a slight advantage": return " $14 ";
                case "black has a slight advantage": return " $15 ";
                case "white has a moderate advantage": return " $16 ";
                case "black has a moderate advantage": return " $17 ";
                case "white has a decisive advantage": return " $18 ";
                case "black has a decisive advantage": return " $19 ";
                case "white has a crushing advantage": return " $20 ";
                case "black has a crushing advantage": return " $21 ";
                case "white is in zugzwang": return " $22 ";
                case "black is in zugzwang": return " $23 ";
                case "white has a slight space advantage": return " $24 ";
                case "black has a slight space advantage": return " $25 ";
                case "white has a moderate space advantage": return " $26 ";
                case "black has a moderate space advantage": return " $27 ";
                case "white has a decisive space advantage": return " $28 ";
                case "black has a decisive space advantage": return " $29 ";
                case "white has a slight time (development) advantage": return " $30 ";
                case "black has a slight time (development) advantage": return " $31 ";
                case "white has a moderate time (development) advantage": return " $32 ";
                case "black has a moderate time (development) advantage": return " $33 ";
                case "white has a decisive time (development) advantage": return " $34 ";
                case "black has a decisive time (development) advantage": return " $35 ";
                case "white has the initiative": return " $36 ";
                case "black has the initiative": return " $37 ";
                case "white has a lasting initiative": return " $38 ";
                case "black has a lasting initiative": return " $39 ";
                case "white has the attack": return " $40 ";
                case "black has the attack": return " $41 ";
                case "with compensation":
                    if (isWhite)
                        return " $44 ";
                    return " $45 ";
                case "white has insufficient compensation for material deficit": return " $42 ";
                case "black has insufficient compensation for material deficit": return " $43 ";
                case "white has sufficient compensation for material deficit": return " $44 ";
                case "black has sufficient compensation for material deficit": return " $45 ";
                case "white has more than adequate compensation for material deficit": return " $46 ";
                case "black has more than adequate compensation for material deficit": return " $47 ";
                case "white has a slight center control advantage": return " $48 ";
                case "black has a slight center control advantage": return " $49 ";
                case "white has a moderate center control advantage": return " $50 ";
                case "black has a moderate center control advantage": return " $51 ";
                case "white has a decisive center control advantage": return " $52 ";
                case "black has a decisive center control advantage": return " $53 ";
                case "white has a slight kingside control advantage": return " $54 ";
                case "black has a slight kingside control advantage": return " $55 ";
                case "white has a moderate kingside control advantage": return " $56 ";
                case "black has a moderate kingside control advantage": return " $57 ";
                case "white has a decisive kingside control advantage": return " $58 ";
                case "black has a decisive kingside control advantage": return " $59 ";
                case "white has a slight queenside control advantage": return " $60 ";
                case "black has a slight queenside control advantage": return " $61 ";
                case "white has a moderate queenside control advantage": return " $62 ";
                case "black has a moderate queenside control advantage": return " $63 ";
                case "white has a decisive queenside control advantage": return " $64 ";
                case "black has a decisive queenside control advantage": return " $65 ";
                case "white has a vulnerable first rank": return " $66 ";
                case "black has a vulnerable first rank": return " $67 ";
                case "white has a well protected first rank": return " $68 ";
                case "black has a well protected first rank": return " $69 ";
                case "white has a poorly protected king": return " $70 ";
                case "black has a poorly protected king": return " $71 ";
                case "white has a well protected king": return " $72 ";
                case "black has a well protected king": return " $73 ";
                case "white has a poorly placed king": return " $74 ";
                case "black has a poorly placed king": return " $75 ";
                case "white has a well placed king": return " $76 ";
                case "black has a well placed king": return " $77 ";
                case "white has a very weak pawn structure": return " $78 ";
                case "black has a very weak pawn structure": return " $79 ";
                case "white has a moderately weak pawn structure": return " $80 ";
                case "black has a moderately weak pawn structure": return " $81 ";
                case "white has a moderately strong pawn structure": return " $82 ";
                case "black has a moderately strong pawn structure": return " $83 ";
                case "white has a very strong pawn structure": return " $84 ";
                case "black has a very strong pawn structure": return " $85 ";
                case "white has poor knight placement": return " $86 ";
                case "black has poor knight placement": return " $87 ";
                case "white has good knight placement": return " $88 ";
                case "black has good knight placement": return " $89 ";
                case "white has poor bishop placement": return " $90 ";
                case "black has poor bishop placement": return " $91 ";
                case "white has good bishop placement": return " $92 ";
                case "black has good bishop placement": return " $93 ";
                case "white has poor rook placement": return " $94 ";
                case "black has poor rook placement": return " $95 ";
                case "white has good rook placement": return " $96 ";
                case "black has good rook placement": return " $97 ";
                case "white has poor queen placement": return " $98 ";
                case "black has poor queen placement": return " $99 ";
                case "white has good queen placement": return " $100 ";
                case "black has good queen placement": return " $101 ";
                case "white has poor piece coordination": return " $102 ";
                case "black has poor piece coordination": return " $103 ";
                case "white has good piece coordination": return " $104 ";
                case "black has good piece coordination": return " $105 ";
                case "white has played the opening very poorly": return " $106 ";
                case "black has played the opening very poorly": return " $107 ";
                case "white has played the opening poorly": return " $108 ";
                case "black has played the opening poorly": return " $109 ";
                case "white has played the opening well": return " $110 ";
                case "black has played the opening well": return " $111 ";
                case "white has played the opening very well": return " $112 ";
                case "black has played the opening very well": return " $113 ";
                case "white has played the middlegame very poorly": return " $114 ";
                case "black has played the middlegame very poorly": return " $115 ";
                case "white has played the middlegame poorly": return " $116 ";
                case "black has played the middlegame poorly": return " $117 ";
                case "white has played the middlegame well": return " $118 ";
                case "black has played the middlegame well": return " $119 ";
                case "white has played the middlegame very well": return " $120 ";
                case "black has played the middlegame very well": return " $121 ";
                case "white has played the ending very poorly": return " $122 ";
                case "black has played the ending very poorly": return " $123 ";
                case "white has played the ending poorly": return " $124 ";
                case "black has played the ending poorly": return " $125 ";
                case "white has played the ending well": return " $126 ";
                case "black has played the ending well": return " $127 ";
                case "white has played the ending very well": return " $128 ";
                case "black has played the ending very well": return " $129 ";
                case "counterplay": 
                    if( isWhite )
                        return " $132 ";
                    return " $133 ";
                case "white has slight counterplay": return " $130 ";
                case "black has slight counterplay": return " $131 ";
                case "white has moderate counterplay": return " $132 ";
                case "black has moderate counterplay": return " $133 ";
                case "white has decisive counterplay": return " $134 ";
                case "black has decisive counterplay": return " $135 ";
                case "white has moderate time control pressure": return " $136 ";
                case "black has moderate time control pressure": return " $137 ";
                case "white has severe time control pressure": return " $138 ";
                case "black has severe time control pressure": return " $139 ";
                case "novelty": return " $146 ";
                default: return " I have no idea...";
            }
        }


        //        private async void Wv_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs args)
        //        {
        //            WebView2 o = (WebView2)sender;
        //            string wvresult = await o.ExecuteScriptAsync("document.documentElement.outerHTML;");
        //        }
        System.Windows.Forms.WebBrowser browser;
        WebBrowserDocumentCompletedEventHandler onDocumentCompleted;
            private async void getHTML_Click(object sender, EventArgs e)
        {
            browser = new System.Windows.Forms.WebBrowser() { ScriptErrorsSuppressed = true };

            string link = "yourLinkHere";

            //This will be called when the web page loads, it better be a class member since this is just a simple demonstration
            onDocumentCompleted = new WebBrowserDocumentCompletedEventHandler((s, evt) => {
                //Do your HtmlParsingHere
                Task.Delay(2000).Wait();
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(browser.DocumentText);
                var html = doc.Text;
                //var someNode = doc.DocumentNode.SelectNodes("yourxpathHere");
                inputChessableHTML.Text = html;
            });

            //subscribe to the DocumentCompleted event using our above handler before navigating
            //browser.DocumentCompleted += onDocumentCompleted;
            browser.Navigate(variationURL.Text);

            while (true)
            {
                Task.Delay(2000).Wait();
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(browser.DocumentText);
                var html = doc.Text;

                HtmlAgilityPack.HtmlNode titlespan = doc.DocumentNode.SelectSingleNode("//div[@id=\"theOpeningTitle\"]/child::span");
                if (titlespan != null)
                    break;
            }



#if false

            HtmlAgilityPack.HtmlWeb w = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument d = w.Load(variationURL.Text);
            inputChessableHTML.Text = d.ParsedText;
            //GenPGN_Click(sender, e);
            var webView2Environment = await CoreWebView2Environment.CreateAsync();
            await localWV.EnsureCoreWebView2Async(webView2Environment);

            string urlAddress = variationURL.Text;
            localWV.NavigateToString(urlAddress);
            var html = await localWV.ExecuteScriptAsync("document.body.outerHTML");
            inputChessableHTML.Text = html;

//            WebView2 wv = new WebView2();
//            wv.NavigationCompleted += Wv_NavigationCompleted;
//            wv.NavigateToString(urlAddress);
#endif
            return;

//            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
  //          HttpWebResponse response = (HttpWebResponse)request.GetResponse();
    //        if (response.StatusCode == HttpStatusCode.OK)
      //      {
        //        Stream receiveStream = response.GetResponseStream();
          //      StreamReader readStream = null;
            //    if (response.CharacterSet == null)
              //      readStream = new StreamReader(receiveStream);
                //else
 //                   readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
   //             string data = readStream.ReadToEnd();
     //           response.Close();
       //         readStream.Close();
         //       inputChessableHTML.Text = data;
                //GenPGN_Click(sender, e);
//            }
        }

        private void inputChessableHTML_TextChanged(object sender, EventArgs e)
        {
            try
            {
                GenPGN_Click(sender, e);
            } catch( Exception )
            {
                MessageBox.Show(
                    "Appears the text was not HTML (did you paste PGN inadvertently??)\nPlease Try Again...",
                    "PGN Parse Error",
                    MessageBoxButtons.OK
                    );
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.Clipboard.SetText(outputPGN.Text);
            }catch( Exception ex ) 
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.Clipboard.SetText(GameTag.Text);
            }
            catch (Exception ex)
            {

            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            inputChessableHTML.Text =  System.Windows.Forms.Clipboard.GetText();
        }
    }
}
