using Microsoft.AspNetCore.Http;
using SistemaVotacion.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Servicios.Interfaces
{
    public interface IMultimediaService
    {
        Task<List<Multimedia>> GetAllAsync(); 
        Task<Multimedia> GetByIdAsync(int id); 
        Task<Multimedia> UploadAsync(IFormFile file, int idCandidato, int idLista, string? descripcion); 
        Task<bool> UpdateAsync(int id, Multimedia multimedia); 
        Task<bool> DeleteAsync(int id);
    }
}
