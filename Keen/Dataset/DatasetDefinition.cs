﻿using System;
using System.Collections.Generic;
using Keen.Core.Query;

namespace Keen.Core.Dataset
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class DatasetDefinition
    {
        /// <summary>
        /// Name of the dataset, which is used as an identifier. Must be unique per project.
        /// </summary>
        public string DatasetName { get; set; }
        /// <summary>
        /// The human-readable string name for your Cached Dataset
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Holds information describing the query which is cached by this Cached Dataset.
        /// </summary>
        public QueryDefinition Query { get; set; }
        /// <summary>
        /// When the most recent computation was queued.
        /// </summary>
        public DateTime? LastScheduledDate { get; set; }
        /// <summary>
        /// The most recent interval that has been computed for the Cached Dataset.
        /// </summary>
        public DateTime? LatestSubtimeframeAvailable { get; set; }
        /// <summary>
        /// The difference between now and the most recent datapoint computed.
        /// </summary>
        public long MillisecondsBehind { get; set; }
        /// <summary>
        /// The event property name of string values results are retrieved by.
        /// </summary>
        public string IndexBy { get; set; }
    }

    internal static class DatasetDefinitionExtensions
    {
        public static void Validate(this DatasetDefinition dataset)
        {
            if (string.IsNullOrWhiteSpace(dataset.DatasetName))
            {
                throw new KeenException("DatasetDefinition must have a name.");
            }

            if (string.IsNullOrWhiteSpace(dataset.DisplayName))
            {
                throw new KeenException("DatasetDefinition must have a display name.");
            }

            if (string.IsNullOrWhiteSpace(dataset.IndexBy))
            {
                throw new KeenException("DatasetDefinition must specify a property to index by.");
            }

            if (dataset.Query == null)
            {
                throw new KeenException("DatasetDefinition must contain a query to be cached.");
            }

            dataset.Query.ValidateForCachedDataset();
        }
    }

    /*
     * This is used because the PUT endpoints take a string for index_by, but return an array of strings.
     */
    internal class DatasetDefinitionConverter : JsonConverter
    {
        /* This prevents JToken.ToObject form recursively calling ReadJson until the stack runs out */
        private bool _isNested;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            _isNested = true;
            serializer.Serialize(writer, value);
            _isNested = false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (token.Type != JTokenType.Object)
                return null;

            var indexByList = token["index_by"]?.ToArray();

            //Setting index_by to null so that Serializing without this converter doesnt run into issues.
            token["index_by"] = null;

            _isNested = true;
            var datasetDefinition = token.ToObject<DatasetDefinition>(serializer);
            _isNested = false;

            datasetDefinition.IndexBy = indexByList?.FirstOrDefault()?.ToString();
            return datasetDefinition;
        }

        public override bool CanConvert(Type objectType)
        {
            if (_isNested)
                return false;

            return objectType == typeof(DatasetDefinition);
        }
    }
}