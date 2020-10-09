// Copyright 2020 Heath Stewart.
// Licensed under the MIT License.See LICENSE.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core.Serialization;
using Azure.Search.Documents;
using Newtonsoft.Json;

namespace Migration
{
    internal class Program
    {
        /// <summary>
        /// Queries an index of mountains.
        /// </summary>
        /// <param name="endpoint">The Search service URI.</param>
        /// <param name="key">The query key for the Search service.</param>
        /// <param name="query">Keywords to query. The default is "*".</param>
        /// <returns></returns>
        private static async Task Main(Uri endpoint, string key, string query = "*")
        {
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, args) =>
            {
                cts.Cancel();
                args.Cancel = true;
            };

            var credential = new AzureKeyCredential(key);
            var options = new SearchClientOptions
            {
                Serializer = new NewtonsoftJsonObjectSerializer(
                    new JsonSerializerSettings
                    {
                        Converters =
                        {
                            new NewtonsoftJsonMicrosoftSpatialGeoJsonConverter(),
                        },
                    }
                ),
            };

            var client = new SearchClient(endpoint, Mountain.IndexName, credential, options);
            var results = await client.SearchAsync<Mountain>(query);

            await foreach (var result in results.Value.GetResultsAsync())
            {
                var mountain = result.Document;
                Console.WriteLine("https://www.bing.com/maps?cp={0}~{1}&sp=point.{0}_{1}_{2}",
                    mountain.Summit.Latitude,
                    mountain.Summit.Longitude,
                    Uri.EscapeUriString(mountain.Name));
            }
        }
    }
}
