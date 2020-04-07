using System;
using System.Collections.Generic;
using System.Text;

namespace WebScraptingTest.Lib
{
    class HtmlNode
    {
        public int id { get; set; }
        //type could be: a, div, span, etc.
        public string type { get; set; }
        public string content { get; set; }
        public HtmlNode parent { get; set; }
        public HashSet<HtmlNode> child { get; set; }

        public HtmlNode() { }
        public HtmlNode(int _id, string _type) {
            id = _id;
            type = _type;
        }
    }
}
