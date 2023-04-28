using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using SistemaVentas.AppWeb.Models.ViewModels;
using SistemaVentas.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVentas.AppWeb.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {

        private readonly IMapper _mapper;
        private readonly IVentaService _ventaServicio;

        public ReporteController(IMapper mapper, IVentaService ventaServicio)
        {
            _mapper = mapper;
            _ventaServicio = ventaServicio;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ReporteVenta(string fechaInicio, string fechaFin)
        {
            try
            {
                List<VMReporteVenta> vmLista = _mapper.Map<List<VMReporteVenta>>(await _ventaServicio.Reporte(fechaInicio, fechaFin));
                return StatusCode(StatusCodes.Status200OK, new { data = vmLista });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
    }
}
