// Copyright 2020 Heath Stewart.
// Licensed under the MIT License.See LICENSE.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Spatial;

#if TRACK1
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
#elif TRACK2
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
#endif

namespace Migration
{
    /// <summary>
    /// Model of a mountain.
    /// </summary>
    public class Mountain
    {
        internal const string IndexName = "mountains";

        /// <summary>
        /// Gets or sets the unique identifier of the mountain.
        /// </summary>
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString("n");

        /// <summary>
        /// Gets or sets the name of the mountain.
        /// </summary>
#if TRACK1
        [IsSearchable, IsSortable, Analyzer(AnalyzerName.AsString.EnLucene)]
#elif TRACK2
        [SearchableField(IsSortable = true, AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
#endif
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the coordinates of the summit.
        /// </summary>
#if TRACK1
        [IsFilterable]
#elif TRACK2
        [SimpleField(IsFilterable = true)]
#endif
        public GeographyPoint Summit { get; set; }
    }
}
