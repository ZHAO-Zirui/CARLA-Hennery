using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Begin");
            
            var url = "https://docs.python.org/release/";

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "curl",
                    Arguments = url,
                    RedirectStandardOutput = true
                }
            };
            process.Start();
            process.WaitForExit();
            var html = process.StandardOutput.ReadToEnd();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            
            Console.WriteLine("HTML Document Get");

            var nodes = doc.DocumentNode.SelectNodes("//pre/a");
            foreach (var node in nodes)
            {
                Console.WriteLine(node.Attributes["href"].Value);
            }
        }
    }
}