using System;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web;
using System.Text.Json;

namespace SillasParser
{
    public class Chat
    {
        public string Pk { get; set; }
        public string Date { get; set; }
        public string From { get; set; }
        public string MsgContent { get; set; }
        public String MsgStatus { get; set; }
        public string MediaType { get; set; }
        public string MediaSize { get; set; }
    }

    class Program
    {
        public static string ScrubHtml(string value)
        {
            var step0 = Regex.Replace(value, @"<[^>]+>|&nbsp;", " ").Trim();
            //var step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;", " ").Trim();
            //var step2 = Regex.Replace(step1, @"\s{2,}", " ");
            return step0;
        }

        public static Chat GenerateChat(List<HtmlNode> s)
        {
            var newChat = new Chat
            {
                Pk = s[0].InnerText,
                Date = ScrubHtml(s[1].InnerText),
                From = s[2].InnerText,
                MsgContent = s[3].InnerText,
                MsgStatus = s[4].InnerText,
                MediaType = s[5].InnerText,
                MediaSize = s[6].InnerText
            };
            return newChat;
        }

        static void Main(string[] args)
        {
            var htmlFile = String.Concat(Directory.GetCurrentDirectory(), "/TMob.html");
            var resultFile = String.Concat(Directory.GetCurrentDirectory(), "/TMob.json");

            using (FileStream fs = File.Open(htmlFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                var document = new HtmlAgilityPack.HtmlDocument();
                document.Load(sr);

                var h = document.GetElementbyId("thead").Elements("tr").ToList();
                var x = document.GetElementbyId("tbody").Elements("tr").ToList();

                using (var outputFile = new StreamWriter(resultFile))
                {
                    outputFile.Write("[");

                    for (int i = 0; i < x.Count - 2; i++)
                    {
                        var s = x[i].Elements("td").ToList();
                        var newChat = GenerateChat(s);
                        outputFile.Write(JsonSerializer.Serialize(newChat));
                        outputFile.Write(", ");
                    }

                    var f = x[x.Count - 1].Elements("td").ToList();
                    var finalChat = GenerateChat(f);
                    outputFile.Write(JsonSerializer.Serialize(finalChat));

                    outputFile.Write("]");

                }
            }
        }
    }
}
