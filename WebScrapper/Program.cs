﻿using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace WebScrapper
{
    internal class Program
    {
        private static string homePageUrl = "https://books.toscrape.com/";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Execution Started!");

            await DownloadWebSite();

            Console.WriteLine("Execution Completed!");
        }

        private static async Task DownloadWebSite()
        {
            var web = new HtmlWeb();
            var pageIndex = 0;
            HtmlDocument document;
            FileDownloader fileDownloader = new FileDownloader(homePageUrl);

            while (pageIndex < 50)
            {
                /// First download the html page
                /// 
                if (pageIndex == 0)
                {
                    await fileDownloader.Download("index.html", true);
                }
                else
                {
                    await fileDownloader.Download($"{homePageUrl}catalogue/page-{pageIndex}.html", false);
                }

                var currenPageUrl = pageIndex == 0 ? homePageUrl : $"{homePageUrl}catalogue/page-{pageIndex}.html";

                document = web.Load(currenPageUrl);

                var productHTMLElements = document.DocumentNode.QuerySelectorAll("li");


                foreach (var productHTMLElement in productHTMLElements)
                {
                    var url = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("a")?.Attributes["href"].Value);
                    var image = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("img")?.Attributes["src"].Value);

                    if (image != null)
                    {
                        await fileDownloader.Download(image);
                    }
                    if (url != null)
                    {
                        await fileDownloader.Download(url);
                    }
                }
                pageIndex++;
            }

        }
    }
}