using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SistemaVotacion.Modelos;
using SistemaVotacion.Servicios.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SistemaVotacion.Servicios
{
    public class MultimediaService : IMultimediaService
    {
        private readonly HttpClient _httpClient;

        public MultimediaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Multimedia>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Multimedia>>(json) ?? new List<Multimedia>();
        }

        public async Task<Multimedia> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Multimedia>(json)!;
        }

        public async Task<Multimedia> UploadAsync(IFormFile file, int? idCandidato, int? idLista, string? descripcion)
        {
            var form = new MultipartFormDataContent();
            form.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);

            var qs = new List<string>();

            if (idCandidato.HasValue && idCandidato.Value > 0)
                qs.Add($"idCandidato={idCandidato.Value}");

            if (idLista.HasValue && idLista.Value > 0)
                qs.Add($"idLista={idLista.Value}");

            if (!string.IsNullOrWhiteSpace(descripcion))
                qs.Add($"descripcion={Uri.EscapeDataString(descripcion)}");

            var url = "upload";
            if (qs.Any()) url += "?" + string.Join("&", qs);

            var response = await _httpClient.PostAsync(url, form);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Multimedia>(json)!;
        }

        public async Task<bool> UpdateAsync(int id, Multimedia multimedia)
        {
            var content = new StringContent(JsonConvert.SerializeObject(multimedia), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
