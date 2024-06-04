using System;
using System.Collections.Generic;

namespace AsyncEnumerable
{
    public class SimpleParser
    {
        public void Parse()
        {
            var sites = new List<string> { "https://site1.ru", "https://site2.ru", "https://site3.ru" };

            foreach (var site in sites)
            {
                var siteMenuItems = GetMenuItems(site);
                foreach (var siteMenuItem in siteMenuItems)
                {
                    var itemsUrls = GetCategoryItems(siteMenuItem);
                    foreach (var itemsUrl in itemsUrls)
                    {
                        var item = ParseItem(itemsUrl);
                        Console.WriteLine(item);
                    }
                }
            }
        }

        private IEnumerable<string> GetMenuItems(string url)
        {
            Console.WriteLine($"Menu items parse: {url}");
            for (int i = 1; i < 4; i++)
            {
                yield return $"{url}/{i}";
            }
        }

        private IEnumerable<string> GetCategoryItems(string url)
        {
            Console.WriteLine($"Category items parse: {url}");
            for (int i = 1; i < 4; i++)
            {
                yield return $"{url}/{i}";
            }
        }

        private ParseResult ParseItem(string url)
        {
            Console.WriteLine($"Item parse: {url}");
            return new ParseResult { Url = url };
        }
    }
}