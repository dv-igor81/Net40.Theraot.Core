using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncEnumerable
{
    public class AdvansedParser
    {
        public async Task Parse()
        {
            var sites = new List<string> { "https://site1.ru", "https://site2.ru", "https://site3.ru" };
            
            var items = ParseItem(GetCategoryItems(GetMenuItems(sites)));
            
            await foreach (var item in items)
            {
                Console.WriteLine(item);
            }
        }

        private async IAsyncEnumerable<string> GetMenuItems(IEnumerable<string> sites)
        {
            foreach (var site in sites)
            {
                Console.WriteLine($"Menu items parse: {site}");
                for (int i = 1; i < 4; i++)
                {
                    await TaskEx.Delay(100);
                    yield return $"{site}/{i}";
                }
            }
        }

        private async IAsyncEnumerable<string> GetCategoryItems(IAsyncEnumerable<string> siteMenuItems)
        {
            await foreach (var siteMenuItem in siteMenuItems)
            {
                Console.WriteLine($"Category items parse: {siteMenuItem}");
                for (int i = 1; i < 4; i++)
                {
                    await TaskEx.Delay(100);
                    yield return $"{siteMenuItem}/{i}";
                }
            }
        }

        private async IAsyncEnumerable<ParseResult> ParseItem(IAsyncEnumerable<string> itemsUrls)
        {
            await foreach (var itemsUrl in itemsUrls)
            {
                Console.WriteLine($"Item parse: {itemsUrl}");
                yield return new ParseResult { Url = itemsUrl };
            }
        }
    }
}