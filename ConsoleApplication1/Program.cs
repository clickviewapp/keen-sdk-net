using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    using System.Diagnostics;
    using Keen.Core;
    using Keen.Core.Dataset;
    using Keen.Core.Query;

    class Program
    {
        private static readonly IDictionary<string, ProjectSettingsProvider> Configs = new Dictionary<string, ProjectSettingsProvider>
        {
            /* Configs excluded for security purposes */
        };

        private static IEnumerable<DatasetDefinition> GetDatasets()
        {
            return new List<DatasetDefinition>
            {
                new DatasetDefinition
                {
                    DatasetName = "videowatched-by-customer-id-grouped-by-category",
                    DisplayName = "Videos watched by customer id grouped by category",
                    IndexBy = "School.Id",
                    Query = new QueryDefinition
                    {
                        AnalysisType = "count",
                        EventCollection = "videoWatched",
                        GroupBy = new[] {"Data.video.category"},
                        Timeframe = "this_12_months",
                        Interval = "daily"
                    }
                },
                new DatasetDefinition
                {
                    DatasetName = "videowatched-by-customer-id-grouped-by-yeargroup",
                    DisplayName = "Videos watched by customer id grouped by yeargroup",
                    IndexBy = "School.Id",
                    Query = new QueryDefinition
                    {
                        AnalysisType = "count",
                        EventCollection = "videoWatched",
                        GroupBy = new[] {"User.YearGroup"},
                        Timeframe = "this_12_months",
                        Interval = "daily"
                    }
                },
                new DatasetDefinition
                {
                    DatasetName = "videowatched-by-customer-id-grouped-by-role",
                    DisplayName = "Videos watched by customer id grouped by role",
                    IndexBy = "School.Id",
                    Query = new QueryDefinition
                    {
                        AnalysisType = "count",
                        EventCollection = "videoWatched",
                        GroupBy = new[] {"User.Role"},
                        Timeframe = "this_12_months",
                        Interval = "daily"
                    }
                },
                new DatasetDefinition
                {
                    DatasetName = "videowatched-by-customer-id-grouped-by-service",
                    DisplayName = "Videos watched by customer id grouped by service",
                    IndexBy = "School.Id",
                    Query = new QueryDefinition
                    {
                        AnalysisType = "count",
                        EventCollection = "videoWatched",
                        GroupBy = new[] {"Data.service"},
                        Timeframe = "this_12_months",
                        Interval = "daily"
                    }
                },
                new DatasetDefinition
                {
                    DatasetName = "videowatched-by-customer-id-grouped-by-tokenid",
                    DisplayName = "Videos watched by customer id grouped by tokenid",
                    IndexBy = "School.Id",
                    Query = new QueryDefinition
                    {
                        AnalysisType = "count",
                        EventCollection = "videoWatched",
                        GroupBy = new[] {"Data.video.tokenId"},
                        Timeframe = "this_12_months",
                        Interval = "daily"
                    }
                },
                new DatasetDefinition
                {
                    DatasetName = "videowatched-by-tokenid-grouped-by-username",
                    DisplayName = "Videos watched by tokenid grouped by username",
                    IndexBy = "Data.video.tokenId",
                    Query = new QueryDefinition
                    {
                        AnalysisType = "count",
                        EventCollection = "videoWatched",
                        GroupBy = new[] {"User.Username"},
                        Timeframe = "this_12_months",
                        Interval = "daily"
                    }
                },
                new DatasetDefinition
                {
                    DatasetName = "login-by-customer-id-grouped-by-role",
                    DisplayName = "Logins by customer id grouped by role",
                    IndexBy = "School.Id",
                    Query = new QueryDefinition
                    {
                        AnalysisType = "count",
                        EventCollection = "login",
                        GroupBy = new[] {"User.Role"},
                        Timeframe = "this_12_months",
                        Interval = "daily"
                    }
                },
                new DatasetDefinition
                {
                    DatasetName = "login-by-customer-id-grouped-by-yeargroup",
                    DisplayName = "Logins by customer id grouped by yeargroup",
                    IndexBy = "School.Id",
                    Query = new QueryDefinition
                    {
                        AnalysisType = "count",
                        EventCollection = "login",
                        GroupBy = new[] {"User.YearGroup"},
                        Timeframe = "this_12_months",
                        Interval = "daily"
                    }
                }
            };
        }

        static void Main(string[] args)
        {
            //DeleteAll();
            CreateAll();
        }

        static bool HasWonkyProperties(string projectKey)
        {
            return projectKey.Contains("WINDOWS") || projectKey.Contains("ANDROID");
        }

        static void ReplaceProperties(string projectKey, DatasetDefinition definition)
        {
            if (!HasWonkyProperties(projectKey))
                return;

            if(definition.IndexBy == "Data.video.tokenId")
                definition.IndexBy = "Data.Video.tokenId";

            if (definition.Query.GroupBy.First() == "Data.video.tokenId")
                definition.Query.GroupBy = new[] {"Data.Video.tokenId"};

            if (definition.Query.GroupBy.First() == "Data.service")
                definition.Query.GroupBy = new[] { "Data.Service" };

            if (definition.Query.GroupBy.First() == "Data.video.category")
                definition.Query.GroupBy = new[] { "Data.Video.category" };
        }

        static void CreateAll()
        {
            foreach (var project in Configs)
            {
                var keenClient = new KeenClient(project.Value);

                var datasets = GetDatasets().ToList();

                for (var i = 0; i < datasets.Count; i++)
                {
                    try
                    {
                        ReplaceProperties(project.Key, datasets[i]);
                        var result = keenClient.CreateDataset(datasets[i]);

                        if (result == null)
                        {
                            
                        }
                    }
                    catch (Exception e)
                    {
                        
                    }

                }
            }
        }

        static void DeleteAll()
        {
            foreach (var project in Configs)
            {
                var keenClient = new KeenClient(project.Value);
                var datasets = keenClient.ListAllDatasetDefinitions();

                if (datasets != null)
                {
                    foreach (var dataset in datasets)
                    {
                        keenClient.DeleteDataset(dataset.DatasetName);
                    }
                }
            }
        }
    }
}
