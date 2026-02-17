using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;
using System.Net.Http;

namespace SistemaVotacion.MVC.Controllers
{
    public class PadronesController : Controller
    {

        //private readonly IHttpClientFactory _httpClientFactory;

        //public PadronesController(IHttpClientFactory httpClientFactory)
        //{
        //    _httpClientFactory = httpClientFactory;
        //}
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ListUsuarios()
        {
            try
            {
                var usuarios = Crud<UsuarioListDto>.GetAll();
                return View(usuarios);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View(new List<UsuarioListDto>());
            }
        }


        public IActionResult EditUsuario(int id)
        {
            try
            {
                var usuario = Crud<UsuarioEditDto>.GetById(id);
                if (usuario == null)
                    return NotFound();

                CargarCombos();
                return View(usuario);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUsuario(int id, UsuarioEditDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                CargarCombos();
                return View(dto);
            }

            try
            {
                Crud<UsuarioEditDto>.Update(id, dto);
                return RedirectToAction(nameof(ListUsuarios));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                CargarCombos();
                return View(dto);
            }
        }

        public IActionResult DeleteUsuario(int id)
        {
            try
            {
                var usuario = Crud<UsuarioEditDto>.GetById(id);
                if (usuario == null)
                    return NotFound();

                return View(usuario);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUsuario(int id, UsuarioEditDto usuario)
        {
            try
            {
                Crud<UsuarioEditDto>.Delete(id);
                return RedirectToAction(nameof(ListUsuarios));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(usuario);
            }
        }


        private void CargarCombos()
        {
            ViewBag.Roles = GetRoles();
            ViewBag.Generos = GetGeneros();
            ViewBag.TiposIdentificacion = GetTiposIdentificacion();
        }

        private List<SelectListItem> GetRoles()
        {
            return Crud<Rol>.GetAll()
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.NombreRol
                })
                .ToList();
        }

        private List<SelectListItem> GetGeneros()
        {
            return Crud<Genero>.GetAll()
                .Select(g => new SelectListItem
                {
                    Value = g.IdGenero.ToString(),
                    Text = g.DetalleGenero
                })
                .ToList();
        }

        private List<SelectListItem> GetTiposIdentificacion()
        {
            return Crud<TipoIdentificacion>.GetAll()
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.DetalleTipIdentifiacion
                })
                .ToList();
        }
    

        private List<SelectListItem> GetUsuarios()
        {
            return Crud<Usuario>.GetAll()
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Nombres + " " + u.Apellidos
                })
                .ToList();
        }

        private List<SelectListItem> GetJuntas()
        {
            return Crud<JuntaReceptora>.GetAll()
                .Select(j => new SelectListItem
                {
                    Value = j.Id.ToString(),
                    Text = $"Junta {j.NumeroJunta}"
                })
                .ToList();
        }

        public IActionResult ListVotantes()
        {
            try
            {
                var votantes = Crud<Votante>.GetAll();
                return View(votantes);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View(new List<Votante>());
            }
        }
        public IActionResult DetailsVotante(int id)
        {
            try
            {
                var votante = Crud<Votante>.GetById(id);
                if (votante == null)
                    return NotFound();

                return View(votante);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }
        public IActionResult CreateVotante()
        {
            ViewBag.Usuarios = GetUsuarios();
            ViewBag.Juntas = GetJuntas();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateVotante(Votante nuevoVotante)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Crud<Votante>.Create(nuevoVotante);
                    return RedirectToAction(nameof(ListVotantes));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear: " + ex.Message);
                }
            }
            ViewBag.Usuarios = GetUsuarios();
            ViewBag.Juntas = GetJuntas();
            return View(nuevoVotante);
        }
        public IActionResult EditVotante(int id)
        {
            try
            {
                var votante = Crud<Votante>.GetById(id);
                if (votante == null)
                    return NotFound();

                ViewBag.Usuarios = GetUsuarios();
                ViewBag.Juntas = GetJuntas();
                return View(votante);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditVotante(int id, Votante votante)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Crud<Votante>.Update(id, votante);
                    return RedirectToAction(nameof(ListVotantes));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                }
            }

            ViewBag.Usuarios = GetUsuarios();
            ViewBag.Juntas = GetJuntas();
            return View(votante);
        }
        public IActionResult DeleteVotante(int id)
        {
            try
            {
                var votante = Crud<Votante>.GetById(id);
                if (votante == null)
                    return NotFound();

                return View(votante);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteVotante(int id, Votante votante)
        {
            try
            {
                Crud<Votante>.Delete(id);
                return RedirectToAction(nameof(ListVotantes));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(votante);
            }
        }

        //// Listas para los select items
        //private async Task<List<SelectListItem>> GetVotantesAsync()
        //{
        //    var client = _httpClientFactory.CreateClient();
        //    var votantes = await client.GetFromJsonAsync<List<Votante>>("api/votantes");

        //    return votantes
        //        .Select(v => new SelectListItem
        //        {
        //            Value = v.Id.ToString(),
        //            Text = v.Usuario != null
        //                ? v.Usuario.Nombres + " " + v.Usuario.Apellidos
        //                : "Votante sin usuario"
        //        })
        //        .ToList();
        //}

        //private async Task<List<SelectListItem>> GetProcesosAsync()
        //{
        //    var client = _httpClientFactory.CreateClient();
        //    var procesos = await client.GetFromJsonAsync<List<ProcesoElectoral>>("api/procesosElectorales");

        //    return procesos
        //        .Select(p => new SelectListItem
        //        {
        //            Value = p.Id.ToString(),
        //            Text = !string.IsNullOrWhiteSpace(p.NombreProceso) ? p.NombreProceso : $"Proceso {p.Id}"
        //        })
        //        .ToList();
        //}

        //// GET: List
        //public async Task<IActionResult> ListPadron()
        //{
        //    try
        //    {
        //        var client = _httpClientFactory.CreateClient();
        //        var padrones = await client.GetFromJsonAsync<List<Padron>>("api/padrones");
        //        return View(padrones);
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Error = "Error al obtener padrones: " + ex.Message;
        //        return View(new List<Padron>());
        //    }
        //}


        //// GET: Details
        //// DETALLE PADRON
        //public async Task<IActionResult> DetailsPadron(int id)
        //{
        //    try
        //    {
        //        var client = _httpClientFactory.CreateClient();
        //        var padron = await client.GetFromJsonAsync<Padron>($"api/padrones/{id}");
        //        if (padron == null) return NotFound();
        //        return View(padron);
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Error = "Error al obtener detalle: " + ex.Message;
        //        return View();
        //    }
        //}

        //[HttpGet]
        //public async Task<IActionResult> CreatePadron()
        //{
        //    try
        //    {
        //        ViewBag.Votantes = await GetVotantesAsync();
        //        ViewBag.Procesos = await GetProcesosAsync();
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Error = "Error al cargar formulario: " + ex.Message;
        //        return View();
        //    }
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreatePadron(int votanteId)
        //{
        //    try
        //    {
        //        var client = _httpClientFactory.CreateClient();
        //        // Llamada a API para crear padrón + generar código automáticamente
        //        var response = await client.PostAsync($"api/padrones/crear-o-generar-codigo/{votanteId}", null);

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var msg = await response.Content.ReadAsStringAsync();
        //            ModelState.AddModelError("", string.IsNullOrWhiteSpace(msg) ? "Error al crear padrón." : msg);
        //            ViewBag.Votantes = await GetVotantesAsync();
        //            ViewBag.Procesos = await GetProcesosAsync();
        //            return View();
        //        }

        //        return RedirectToAction(nameof(ListPadron));
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", "Error al crear padrón: " + ex.Message);
        //        ViewBag.Votantes = await GetVotantesAsync();
        //        ViewBag.Procesos = await GetProcesosAsync();
        //        return View();
        //    }
        //}



        //// EDITAR PADRON
        //[HttpGet]
        //public async Task<IActionResult> EditPadron(int id)
        //{
        //    try
        //    {
        //        var client = _httpClientFactory.CreateClient();
        //        var padron = await client.GetFromJsonAsync<Padron>($"api/padrones/{id}");
        //        if (padron == null) return NotFound();

        //        ViewBag.Votantes = await GetVotantesAsync();
        //        ViewBag.Procesos = await GetProcesosAsync();
        //        return View(padron);
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Error = "Error al cargar padrón: " + ex.Message;
        //        return View();
        //    }
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditPadron(int id, Padron padron)
        //{
        //    if (id != padron.Id) return BadRequest();

        //    try
        //    {
        //        var client = _httpClientFactory.CreateClient();
        //        var response = await client.PutAsJsonAsync($"api/padrones/{id}", padron);

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var msg = await response.Content.ReadAsStringAsync();
        //            ModelState.AddModelError("", string.IsNullOrWhiteSpace(msg) ? "Error al actualizar padrón." : msg);
        //            ViewBag.Votantes = await GetVotantesAsync();
        //            ViewBag.Procesos = await GetProcesosAsync();
        //            return View(padron);
        //        }

        //        return RedirectToAction(nameof(ListPadron));
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", "Error al actualizar padrón: " + ex.Message);
        //        ViewBag.Votantes = await GetVotantesAsync();
        //        ViewBag.Procesos = await GetProcesosAsync();
        //        return View(padron);
        //    }
        //}


        //[HttpGet]
        //public async Task<IActionResult> DeletePadron(int id)
        //{
        //    try
        //    {
        //        var client = _httpClientFactory.CreateClient();
        //        var padron = await client.GetFromJsonAsync<Padron>($"api/padrones/{id}");
        //        if (padron == null) return NotFound();

        //        return View(padron);
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Error = "Error al obtener padrón: " + ex.Message;
        //        return View();
        //    }
        //}


        //[HttpPost, ActionName("DeletePadron")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeletePadronConfirmed(int id)
        //{
        //    try
        //    {
        //        var client = _httpClientFactory.CreateClient();
        //        var response = await client.DeleteAsync($"api/padrones/{id}");

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var msg = await response.Content.ReadAsStringAsync();
        //            ModelState.AddModelError("", string.IsNullOrWhiteSpace(msg) ? "Error al eliminar padrón." : msg);
        //            return View();
        //        }

        //        return RedirectToAction(nameof(ListPadron));
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", "Error al eliminar padrón: " + ex.Message);
        //        return View();
        //    }
        //}
    }
}