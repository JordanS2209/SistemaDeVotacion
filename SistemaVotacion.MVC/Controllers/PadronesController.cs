using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVotacion.MVC.Controllers
{
    [Authorize]
    public class PadronesController : Controller
    {

      
        [Authorize(Roles = "Admin,SuperAdmin")]
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
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
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
                TempData["Error"] = ex.Message;
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
                TempData["Success"] = "Usuario actualizado correctamente.";
                return RedirectToAction(nameof(ListUsuarios));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
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
                TempData["Error"] = ex.Message;
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
                TempData["Success"] = "Usuario eliminado correctamente.";
                return RedirectToAction(nameof(ListUsuarios));
            }
            catch (Exception ex)
            {
                 TempData["Error"] = "Error al eliminar: " + ex.Message;
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
            return Crud<UsuarioListDto>.GetAll()
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
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
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
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
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
                    TempData["Success"] = "Votante creado correctamente.";
                    return RedirectToAction(nameof(ListVotantes));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear: " + ex.Message;
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
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
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
                    TempData["Success"] = "Votante actualizado correctamente.";
                    return RedirectToAction(nameof(ListVotantes));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al actualizar: " + ex.Message;
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
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
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
                TempData["Success"] = "Votante eliminado correctamente.";
                return RedirectToAction(nameof(ListVotantes));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar: " + ex.Message;
                return View(votante);
            }
        }


        // ==========================================
        // GESTIÓN DE PADRÓN ELECTORAL (Habilitación)
        // ==========================================

        public IActionResult ListPadron()
        {
            try
            {
                var padrones = Crud<Padron>.GetAll();
                return View(padrones);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al obtener padrón: " + ex.Message;
                return View(new List<Padron>());
            }
        }

        public IActionResult CreatePadron()
        {
            CargarCombosPadron();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePadron(Padron padron)
        {
            // Validar que se haya seleccionado votante
            if (padron.IdVotante == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un votante.");
                CargarCombosPadron();
                return View(padron);
            }

            try
            {
                // Validación básica de votante
                var votante = Crud<Votante>.GetById(padron.IdVotante);
                 // Validar estado (Opcional aquí, pero bueno para consistencia)
                if (votante != null && !votante.Estado)
                {
                     TempData["Error"] = "El votante seleccionado está INACTIVO.";
                     CargarCombosPadron();
                     return View(padron);
                }

                // Usamos el Create estándar que NO genera código (CodigoAcceso será null)
                // El código se generará SOLO cuando el delegado habilite al votante.
                Crud<Padron>.Create(padron);
                
                TempData["Success"] = "Votante asignado al padrón correctamente (Código pendiente de habilitación).";
                return RedirectToAction(nameof(ListPadron));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al habilitar votante: " + ex.Message;
            }

            CargarCombosPadron();
            return View(padron);
        }

        public IActionResult EditPadron(int id)
        {
            try
            {
                var padron = Crud<Padron>.GetById(id);
                if (padron == null) return NotFound();

                CargarCombosPadron();
                return View(padron);
            }
            catch (Exception ex)
            {
                 TempData["Error"] = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPadron(int id, Padron padron)
        {
            if (id != padron.Id) return BadRequest();

            try
            {
                // Mantenemos el código original si no se envía uno nuevo (o lógica de negocio que decidas)
                // Aquí asumimos que el admin edita estados o asignaciones
                Crud<Padron>.Update(id, padron);
                TempData["Success"] = "Padrón actualizado correctamente.";
                return RedirectToAction(nameof(ListPadron));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar: " + ex.Message;
                CargarCombosPadron();
                return View(padron);
            }
        }

        public IActionResult DeletePadron(int id)
        {
            try
            {
                var padron = Crud<Padron>.GetById(id);
                if (padron == null) return NotFound();
                return View(padron);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View();
            }
        }

        [HttpPost, ActionName("DeletePadron")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePadronConfirmed(int id)
        {
            try
            {
                Crud<Padron>.Delete(id);
                TempData["Success"] = "Padrón eliminado correctamente.";
                return RedirectToAction(nameof(ListPadron));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar: " + ex.Message;
                return View(); 
            }
        }

        // Helpers para Padron
        private void CargarCombosPadron()
        {
            ViewBag.Votantes = GetVotantesList();
            ViewBag.Procesos = GetProcesosList();
        }

        private List<SelectListItem> GetVotantesList()
        {
            var votantes = Crud<Votante>.GetAll() ?? new List<Votante>();
            return votantes.Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = v.Usuario != null ? $"{v.Usuario.Nombres} {v.Usuario.Apellidos} (CI: {v.Usuario.NumeroIdentificacion})" : $"Votante #{v.Id}"
            }).ToList();
        }

        private List<SelectListItem> GetProcesosList()
        {
            var procesos = Crud<ProcesoElectoral>.GetAll() ?? new List<ProcesoElectoral>();
            // Filtramos solo procesos activos si se requiere, por ahora todos
            return procesos.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.NombreProceso
            }).ToList();
        }


        // ==========================================
        // DELEGADO DE MESA - HABILITAR VOTO
        // ==========================================

        [Authorize(Roles = "DelegadoMesa, SuperAdmin")]
        [HttpGet]
        public IActionResult Habilitar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Habilitar(string numeroIdentificacion)
        {
            if (string.IsNullOrWhiteSpace(numeroIdentificacion))
            {
                // Usamos TempData para feedback visual consistente
                TempData["Error"] = "Por favor, ingrese un número de identificación.";
                return View();
            }

            try
            {
                // Llamada a la API: api/Padrones/crear-o-generar-codigo/{cedula}
                var url = $"{Crud<Padron>.EndPoint}/crear-o-generar-codigo/{numeroIdentificacion}";

                using (var client = new HttpClient())
                {
                    // POST vacío a la URL específica
                    var content = new StringContent("", Encoding.UTF8, "application/json");
                    var response = client.PostAsync(url, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var json = response.Content.ReadAsStringAsync().Result;
                        
                        // Deserializamos la respuesta anónima de la API
                        dynamic resultado = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                        string codigo = resultado.codigoAcceso;
                        bool haVotado = resultado.haVotado;
                        bool procesoActivo = resultado.procesoActivo;

                        if (haVotado)
                        {
                            TempData["Error"] = "Este votante YA ha sufragado.";
                            return View();
                        }

                        if (!procesoActivo)
                        {
                            TempData["Error"] = "No hay un proceso electoral activo o el votante no está asignado al proceso actual.";
                            return View();
                        }

                        // Éxito: Mostrar Código
                        TempData["Success"] = "Votante habilitado correctamente.";
                        ViewBag.CodigoGenerado = codigo;
                        ViewBag.Cedula = numeroIdentificacion;
                        return View("CodigoGenerado");
                    }
                    else
                    {
                        var msg = response.Content.ReadAsStringAsync().Result;
                        TempData["Error"] = msg; 
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error de comunicación: " + ex.Message;
                return View();
            }
        }
    }
}