﻿using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Threading;
using System.Text.RegularExpressions;
using System.Configuration;

namespace A2TeamAssignment
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int pageSize = 100;
            int pageNumber = 0;
            int totalPages = 1;

            var appSettings = ConfigurationManager.AppSettings;
            var insertQuery = appSettings["insertQuery"];
            var queryWoodDealContent = appSettings["searchReportWoodDeal"];

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(10);

                while (pageNumber < totalPages)
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri("https://www.lesegais.ru/open-area/graphql"),
                        Headers =
                        {
                            { "Accept", "*/*" },
                            { "Accept-Language", "en-US,en;q=0.9,ru;q=0.8,ja;q=0.7,hu;q=0.6,uk;q=0.5,de;q=0.4" },
                            { "Connection", "keep-alive" },
                            { "Origin", "https://www.lesegais.ru" },
                            { "Referer", "https://www.lesegais.ru/open-area/deal" },
                            { "Sec-Fetch-Dest", "empty" },
                            { "Sec-Fetch-Mode", "cors" },
                            { "Sec-Fetch-Site", "same-origin" },
                            { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36" },
                            { "sec-ch-ua", "Chromium\";v=\"110\", \"Not A(Brand\";v=\"24\", \"Google Chrome\";v=\"110\"" },
                            { "sec-ch-ua-mobile", "?0" },
                            { "sec-ch-ua-platform", "\"Windows\"" },
                        },
                        Content = new StringContent(queryWoodDealContent, Encoding.UTF8, "application/json")
                    };


                    using (var dbManager = new DatabaseManager(appSettings["databaseConnectionString"]))
                    using (var response = await httpClient.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        var body = await response.Content.ReadAsStringAsync();

                        Console.WriteLine(body);

                        Root deals = JsonConvert.DeserializeObject<Root>(body);

                        if (pageNumber == 0)
                        {
                            var total = deals.data.searchReportWoodDeal.total;
                            totalPages = (int)Math.Ceiling((double)total / pageSize);
                        }
                    }

                    queryWoodDealContent = queryWoodDealContent.Replace($"\"number\":{pageNumber}", $"\"number\":{pageNumber + 1}");
                    pageNumber++;

                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            }
        }
    }
}