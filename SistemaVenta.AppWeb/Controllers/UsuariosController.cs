using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using Newtonsoft.Json;
using SistemaVentas.AppWeb.Models.ViewModels;
using SistemaVentas.AppWeb.Utilidades.Response;
using SistemaVentas.BLL.Interfaces;
using SistemaVentas.Entity;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVentas.AppWeb.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioServicio;
        private readonly IRolService _rolServicio;

        public UsuariosController(IMapper mapper, IUsuarioService usuarioServicio, IRolService rolServicio)
        {
            _mapper = mapper;
            _usuarioServicio = usuarioServicio;
            _rolServicio = rolServicio;

        }

        public IActionResult Index()
        {
            return View();
        }

        //PETICIONES HTPP

        [HttpGet]
        public async Task<IActionResult> ListaRoles()
        {
            //var lista = await _rolServicio.Lista();
            List<VMRol> vmListaRoles = _mapper.Map<List<VMRol>>(await _rolServicio.Lista());
            return StatusCode(StatusCodes.Status200OK, vmListaRoles);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            //Lista de usuarios
            List<VMUsuarios> vmUsuarioLista = _mapper.Map<List<VMUsuarios>>(await _usuarioServicio.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmUsuarioLista }); // 'data', trabajaremos con DataTable de JQuery.
        }

        //[HttpGet]
        //public async Task<IActionResult> Lista()
        //{
        //    List<VMUsuarios> vmUsuarioLista = _mapper.Map<List<VMUsuarios>>(await _usuarioServicio.Lista());

        //    var resultado = new
        //    {
        //        draw = 1, // Esto es requerido por DataTables
        //        recordsTotal = vmUsuarioLista.Count(),
        //        recordsFiltered = vmUsuarioLista.Count(),
        //        data = vmUsuarioLista
        //    };

        //    return Json(resultado);
        //}

        [HttpPost]                                                    // [FROMFORM] Desde el formulario Get it.
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuarios> gResponse = new GenericResponse<VMUsuarios>();

            try
            {
                VMUsuarios vmUsuario = JsonConvert.DeserializeObject<VMUsuarios>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if(foto != null)
                {                                                    //Solo numero y letras
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);  // Obtenemos la extension de esa foto.
                    nombreFoto = string.Concat(nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }
                //.Host para local, acá debera ir el host del sitio alojado
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";
                //string urlPlantillaCorreo = $"{this.Request.Scheme}://eatv21-001-site1.ftempurl.com/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";

                Usuario usuario_creado = await _usuarioServicio.Crear(_mapper.Map<Usuario>(vmUsuario),fotoStream,nombreFoto,urlPlantillaCorreo);

                vmUsuario = _mapper.Map<VMUsuarios>(usuario_creado);

                gResponse.Estado = true;
                gResponse.Objeto = vmUsuario;
            }
            catch(Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        //UPDATE
        [HttpPut]                                                 
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuarios> gResponse = new GenericResponse<VMUsuarios>();

            try
            {
                VMUsuarios vmUsuario = JsonConvert.DeserializeObject<VMUsuarios>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {                                                    //Solo numero y letras
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);  // Obtenemos la extension de esa foto.
                    nombreFoto = string.Concat(nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                Usuario usuario_editado = await _usuarioServicio.Editar(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto);

                vmUsuario = _mapper.Map<VMUsuarios>(usuario_editado);

                gResponse.Estado = true;
                gResponse.Objeto = vmUsuario;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        //REMOVE
        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idUsuario)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _usuarioServicio.Eliminar(idUsuario);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
