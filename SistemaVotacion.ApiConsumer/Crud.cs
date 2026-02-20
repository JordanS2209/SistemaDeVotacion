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

        public static List<T> GetAll()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(EndPoint).Result;
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

        public static T GetSingle(string customUrl)
        {
            using (var client = new HttpClient())
            {
                // Usamos la URL completa que le pasemos
                var response = client.GetAsync(customUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    // Deserializa como un objeto único T, no como List<T>
                    return JsonConvert.DeserializeObject<T>(json);
                }
                return default;
            }
        }
        public static List<T> GetCustom(string customUrl)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(customUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }



        public static bool PutCustom(string customUrl)
        {
            using (var client = new HttpClient())
            {
                var response = client.PutAsync(customUrl, null).Result;
                return response.IsSuccessStatusCode;
            }
        }

    }
}