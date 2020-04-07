using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using WebScraptingTest.Lib;
using System.Collections.Generic;
using System.Text;

namespace WebScraptingTest
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task Main()
        //static void Main()
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                HttpResponseMessage response = await client.GetAsync("https://www.evo.com/shop/sale/");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                ////Output to Console
                //Console.WriteLine(responseBody);

                string htmlSection = "head";
                string tagOpen = "<" + htmlSection + ">";
                string tagClosed = "</" + htmlSection + ">";

                int start = responseBody.IndexOf(tagOpen);
                int end = responseBody.IndexOf(tagClosed);

                string stringHtml = responseBody
                                    .Substring(start + tagOpen.Length
                                                , end - start - tagOpen.Length);

                stringHtml = FilterComments(stringHtml);

                GenerateHtmlTree(stringHtml);

                ////Output to File, varified.
                using (StreamWriter writer = new StreamWriter(@".\op.html"))
                {
                    writer.WriteLine(responseBody);
                }

                Console.WriteLine("Completed");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            Console.ReadKey();
        }

        static HtmlNode GenerateHtmlTree(string stringHtml)
        {
            string[] htmlElements = stringHtml.Split('<');

            if (htmlElements.Length <= 0)
            {
                return null;
            }

            int id = 0;

            string type = GetTypeForHtmlElement(htmlElements[0]);

            HtmlNode root = new HtmlNode(id, type);
            HtmlNode curr = root;

            Stack<HtmlNode> st = new Stack<HtmlNode>();

            st.Push(curr);

            for (int i = 1; i < htmlElements.Length; i++)
            {
                if(htmlElements[i][0] == '/')
                {
                    //this is a tagClosed.

                    continue;
                }
                else
                {
                    type = GetTypeForHtmlElement(htmlElements[i]);
                    HtmlNode newNode = new HtmlNode(++id, type);
                    newNode.parent = st.Peek();

                    if (st.Peek().child.Count == 0)
                        st.Peek().child = new HashSet<HtmlNode>();
                    st.Peek().child.Add(newNode);
                }
            }

            return root;
        }

        static string FilterComments(string stringHtml)
        {
            string commentsOpen = "<!--";
            string commentsClosed = "-->";

            StringBuilder sb = new StringBuilder();

            int indexCommentsOpen = stringHtml.IndexOf(commentsOpen);
            int indexCommentsClosed = 0;

            while (indexCommentsOpen > -1)
            {
                sb.Append(stringHtml.Substring(0, indexCommentsOpen));
                indexCommentsClosed = stringHtml.IndexOf(commentsClosed);
                stringHtml = stringHtml.Substring(indexCommentsClosed + commentsClosed.Length);
                indexCommentsOpen = stringHtml.IndexOf(commentsOpen);
            }

            return sb.Append(stringHtml).ToString();
        }

        static string GetTypeForHtmlElement(string stringHtml)
        {
            if (stringHtml[0] == '/')
                stringHtml = stringHtml.Substring(1);

            int indexEnd = Math.Min(stringHtml.IndexOf(' '), stringHtml.IndexOf('>'));
            
            return stringHtml.Substring(0, indexEnd);
        }
    }
}
