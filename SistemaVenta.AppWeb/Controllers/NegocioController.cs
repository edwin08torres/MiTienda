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
    public class NegocioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioService;

        public NegocioController(IMapper mapper, INegocioService negocioService)
        {
            _mapper = mapper;
            _negocioService = negocioService;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Devolvera toda la info del negocio
        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();

            try
            {
                VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());

                gResponse.Estado = true;
                gResponse.Objeto = vmNegocio;
            }
            catch(Exception ex) 
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarCambios([FromForm] IFormFile logo, [FromForm] string modelo)
        {
            GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();

            try
            {
                VMNegocio vmNegocio = JsonConvert.DeserializeObject<VMNegocio>(modelo);

                string nombre_logo = "";
                Stream logoStream = null;

                if(logo != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(logo.FileName);
                    nombre_logo = string.Concat(nombre_en_codigo, extension);
                    logoStream = logo.OpenReadStream();
                }

                Negocio negocio_editado = await _negocioService.GuardarCambios(_mapper.Map<Negocio>(vmNegocio), logoStream, nombre_logo);

                vmNegocio = _mapper.Map<VMNegocio>(negocio_editado);

                gResponse.Estado = true;
                gResponse.Objeto = vmNegocio;
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
