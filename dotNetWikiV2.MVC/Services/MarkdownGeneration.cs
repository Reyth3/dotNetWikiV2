using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace dotNetWikiV2.MVC.Services
{
    public class MarkdownGeneration
    {
        private readonly HttpClient _client;

        public MarkdownGeneration(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> GetMarkdownPage()
        {
            return await _client.GetStringAsync("https://jaspervdj.be/lorem-markdownum/markdown.txt");

        }
    }
}
