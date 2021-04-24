using HtmlAgilityPack;
using System.Linq;

namespace FS.Infrastructure.Data
{
    internal static class Utility
    {
        internal static HtmlNode CreateNode(string data)
        {
            var document = new HtmlDocument();
            document.LoadHtml(data);
            return document.DocumentNode;
        }

        internal static HtmlNode GetMainContent(HtmlNode node)
        {
            return node.Descendants().First(d => d.Id == "pagecontent");
        }

        internal static string GetNumberOfPages(HtmlNode node)
        {
            return node.Descendants()
                .First(d => d.HasClass("pages"))
                .Element("span")
                .InnerText
                .Split(" ")[1];
        }
    }
}