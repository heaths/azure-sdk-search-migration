// Copyright 2020 Heath Stewart.
// Licensed under the MIT License.See LICENSE.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using Index = Microsoft.Azure.Search.Models.Index;

namespace Migration
{
    internal class Program
    {
        /// <summary>
        /// Creates, populates, and queries an index of mountains.
        /// </summary>
        /// <param name="endpoint">The Search service URI.</param>
        /// <param name="key">The admin key for the Search service.</param>
        /// <param name="query">Keywords to query. The default is "*".</param>
        /// <param name="wait">The number of seconds to wait for indexing, if created. The default is 2 seconds.</param>
        /// <returns></returns>
        private static async Task Main(Uri endpoint, string key, string query = "*", int wait = 2)
        {
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, args) =>
            {
                cts.Cancel();
                args.Cancel = true;
            };

            var serviceName = endpoint.Host.Substring(0, endpoint.Host.IndexOf('.'));

            var credentials = new SearchCredentials(key);
            var serviceClient = new SearchServiceClient(serviceName, credentials);
            var indexClient = new SearchIndexClient(serviceName, Mountain.IndexName, credentials);

            if (!await serviceClient.Indexes.ExistsAsync(Mountain.IndexName, cancellationToken: cts.Token))
            {
                var fields = FieldBuilder.BuildForType<Mountain>();
                var index = new Index(Mountain.IndexName, fields);

                await serviceClient.Indexes.CreateOrUpdateAsync(index, cancellationToken: cts.Token);

                var batch = new IndexBatch<Mountain>(new []
                {
                    new IndexAction<Mountain>(new Mountain
                    {
                        Name = "Mount Rainier",
                        Summit = GeographyPoint.Create(46.85287, -121.76044),
                    }),
                });

                await indexClient.Documents.IndexAsync(batch);
                await Task.Delay(TimeSpan.FromSeconds(wait));
            }

            var results = await indexClient.Documents.SearchAsync<Mountain>(query, cancellationToken: cts.Token);
            foreach (var result in results.Results)
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
