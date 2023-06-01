using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAppChatGPTPluginTEST.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    [Route("[controller]")]
    public class TodosController : ControllerBase
    {
        private const string AZURESEARCH_SERVICE = "****************";
        private const string AZURESEARCH_INDEX = "*********************";
        private const string AZURESEARCH_API_KEY = "*********************************";

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{searchText}")]
        public async Task<IActionResult> Search(string searchText)
        {
            Uri serviceEndpoint = new Uri($"https://{AZURESEARCH_SERVICE}.search.windows.net/");
            AzureKeyCredential credential = new AzureKeyCredential(AZURESEARCH_API_KEY);
            SearchClient client = new SearchClient(serviceEndpoint, AZURESEARCH_INDEX, credential);

            var searchOptions = new SearchOptions
            {
                Filter = null,
                Size = 5
            };

            // 検索を実行し、結果を取得します。
            SearchResults<SearchResult> response = await client.SearchAsync<SearchResult>(searchText, searchOptions);

            List<SearchResult> searchResults = new List<SearchResult>();

            // 応答を解析し、SearchResultオブジェクトのリストに変換します。
            foreach (var result in response.GetResults())
            {
                    SearchResult searchResult = new SearchResult
                    {
                        SearchScore = result.Score.HasValue ? result.Score.Value : 0.0,
                        ID = result.Document.ID,
                        SampleID = result.Document.SampleID,
                        SampleQuestion = result.Document.SampleQuestion,
                        SampleAnswer = result.Document.SampleAnswer
                    };

                    searchResults.Add(searchResult);
            }

            //return Ok(response.GetResults());
            return Ok(searchResults);
        }

    }
    public class SearchResult
    {
        public double SearchScore { get; set; }
        public string ID { get; set; }
        public string SampleID { get; set; }
        public string SampleQuestion { get; set; }
        public string SampleAnswer { get; set; }
    }

}