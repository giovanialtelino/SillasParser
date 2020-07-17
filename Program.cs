using System;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SillasParser
{
    class Program
    {
        public static string ScrubHtml(string value)
        {
            var step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;", " ").Trim();
            var step2 = Regex.Replace(step1, @"\s{2,}", " ");
            return step2;
        }

        public void NotAsyncJapaDoesntLike()
        {
            var file = String.Concat(Directory.GetCurrentDirectory(), "/TMobM.html");
            var fullFileName = String.Concat(Directory.GetCurrentDirectory(), "/TMob.csv");

            var sr = new StreamReader(file);

            var document = new HtmlAgilityPack.HtmlDocument();
            document.Load(sr);

            var x = document.GetElementbyId("tbody").Elements("tr").ToList();

            using (var outputFile = new StreamWriter(fullFileName))
            {
                foreach (HtmlNode node in x)
                {
                    var s = node.Elements("td").ToList();
                    foreach (HtmlNode item in s)
                    {
                        outputFile.Write(String.Concat(ScrubHtml(item.InnerText), ","));
                    }
                    outputFile.WriteLine();
                }
            }
        }

        static void Main(string[] args)
        {
            var file = String.Concat(Directory.GetCurrentDirectory(), "/TMob.html");
            var fullFileName = String.Concat(Directory.GetCurrentDirectory(), "/TMob.csv");

            using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                var document = new HtmlAgilityPack.HtmlDocument();
                document.Load(sr);

                var h = document.GetElementbyId("thead").Elements("tr").ToList();
                var x = document.GetElementbyId("tbody").Elements("tr").ToList();

                using (var outputFile = new StreamWriter(fullFileName))
                {
                    foreach (HtmlNode node in h)
                    {
                        var s = node.Elements("th").ToList();
                        foreach (HtmlNode item in s)
                        {
                            outputFile.Write(String.Concat(ScrubHtml(item.InnerText), ","));
                        }
                        outputFile.WriteLine();
                    }

                    foreach (HtmlNode node in x)
                    {
                        var s = node.Elements("td").ToList();
                        foreach (HtmlNode item in s)
                        {
                            outputFile.Write(String.Concat(ScrubHtml(item.InnerText), ","));
                        }
                        outputFile.WriteLine();
                    }
                }
            }
        }
    }
}
