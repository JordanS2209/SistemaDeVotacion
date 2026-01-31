using Newtonsoft.Json;
using SistemaVotacion.Modelos;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

namespace SistemaVotacion.ApiConsumer
{
    public static class Crud<T>
    {
        public static string EndPoint { get; set; }

        private static HttpClient CreateClient()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7202/")
            };


            return client;
        }

        public static List<T> GetAll()
        {
            if (string.IsNullOrWhiteSpace(EndPoint))
            {
                throw new InvalidOperationException($"El EndPoint de Crud<{typeof(T).Name}> no está configurado.");
            }

            using (var client = CreateClient())
            {
                try
                {
                    var response = client.GetAsync(EndPoint).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Error HTTP {(int)response.StatusCode} al llamar a {EndPoint}");
                    }

                    var json = response.Content.ReadAsStringAsync().Result;

                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new List<T>();
                    }

                    var result = JsonConvert.DeserializeObject<List<T>>(json);
                    return result ?? new List<T>();
                }
                catch (HttpRequestException ex)
                {

                    throw new Exception($"No se pudo conectar con la API ({EndPoint}). Detalle: {ex.Message}", ex);
                }
            }
        }

        public static T GetById(int id)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync($"{EndPoint}/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        public static T Create(T item)
        {
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(
                        EndPoint,
                        new StringContent(
                            JsonConvert.SerializeObject(item),
                            Encoding.UTF8,
                            "application/json"
                        )
                    ).Result;

                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }
        public static T CreateItem(T item)
        {
            if (string.IsNullOrWhiteSpace(EndPoint))
                throw new InvalidOperationException($"El EndPoint de Crud<{typeof(T).Name}> no está configurado.");

            using (var client = new HttpClient())
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                var json = JsonConvert.SerializeObject(item, settings);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = client.PostAsync(EndPoint, content).Result;
                var responseBody = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(responseBody);
                }
                else
                {
                    throw new Exception($"Error HTTP {(int)response.StatusCode}: {responseBody}");
                }
            }
        }

        public static bool Update(int id, T item)
        {
            using (var client = new HttpClient())
            {
                var response = client.PutAsync(
                        $"{EndPoint}/{id}",

                        new StringContent(
                            JsonConvert.SerializeObject(item),
                            Encoding.UTF8,
                            "application/json"
                        )
                    ).Result;

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        public static bool Delete(int id)
        {
            using (var client = new HttpClient())
            {
                var response = client.DeleteAsync($"{EndPoint}/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }
        public static List<T> GetCustom(string url)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<List<T>>(json);
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }


    }
}