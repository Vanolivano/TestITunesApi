using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestITunesApi
{
    public class Response
    {
        public int resultCount { get; set; }
        public List<Result> results { get; set; }
    }
    public class Result
    {
        public string wrapperType { get; set; }
        public string collectionType { get; set; }
        public int artistId { get; set; }
        public int collectionId { get; set; }
        public int amgArtistId { get; set; }
        public string artistName { get; set; }
        public string collectionName { get; set; }
        public string collectionCensoredName { get; set; }
        public string artistViewUrl { get; set; }
        public string collectionViewUrl { get; set; }
        public string artworkUrl60 { get; set; }
        public string artworkUrl100 { get; set; }
        public double collectionPrice { get; set; }
        public string collectionExplicitness { get; set; }
        public int trackCount { get; set; }
        public string copyright { get; set; }
        public string country { get; set; }
        public string currency { get; set; }
        public string releaseDate { get; set; }
        public string primaryGenreName { get; set; }
    }

    class Program
    {
        static async Task<Response> GetAsync(string path)
        {
            HttpClient client = new HttpClient();
            Response response = null;
            HttpResponseMessage httpResponse = await client.GetAsync(path);
            if (httpResponse.IsSuccessStatusCode)
            {
                var result = await httpResponse.Content.ReadAsStringAsync();
                response = JsonSerializer.Deserialize<Response>(result);
                if (response.resultCount > 0)
                {
                    // запись в файл
                    using (FileStream fstream = new FileStream($"response.txt", FileMode.Create))
                    {
                        // преобразуем строку в байты
                        byte[] array = System.Text.Encoding.Default.GetBytes(result);
                        // запись массива байтов в файл
                        fstream.Write(array, 0, array.Length);
                    }
                }
            }
            else
            {
                using (FileStream fstream = File.OpenRead($"response.txt"))
                {
                    // преобразуем строку в байты
                    byte[] array = new byte[fstream.Length];
                    // считываем данные
                    fstream.Read(array, 0, array.Length);
                    // декодируем байты в строку
                    string textFromFile = System.Text.Encoding.Default.GetString(array);
                    response = JsonSerializer.Deserialize<Response>(textFromFile);
                }
            }
            return response;
        }

        

        static void Main()
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            while (true)
            {
                Console.WriteLine("Enter name artist");
                string nameArtist = Console.ReadLine();
                if (nameArtist != null)
                {
                    try
                    {
                        var url = $"http://itunes.apple.com/search?term={nameArtist}&entity=album";
                        var responce = await GetAsync(url);
                        foreach (var item in responce.results)
                        {
                            Console.WriteLine(item.collectionName);
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
