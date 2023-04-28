using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SistemaVentas.AppWeb.Models.ViewModels;
using SistemaVentas.AppWeb.Utilidades.Response;
using SistemaVentas.BLL.Interfaces;

namespace SistemaVentas.AppWeb.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        private readonly IDashBoardService _dashboardServicio;

        public DashBoardController(IDashBoardService dashboardServicio)
        {
            _dashboardServicio = dashboardServicio;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {
            GenericResponse<VMDashBoard> gResponse = new GenericResponse<VMDashBoard>();

            try
            {
                VMDashBoard vmDashBoard = new VMDashBoard();

                vmDashBoard.TotalVentas = await _dashboardServicio.TotalVentasultimaSemana();
                vmDashBoard.TotalIngresos = await _dashboardServicio.TotalIngresoUltimaSemana();
                vmDashBoard.TotalProductos = await _dashboardServicio.TotalProductos();
                vmDashBoard.TotalCategorias = await _dashboardServicio.TotalCategorias();

                List<VMVentasSemana> listaVentaSemana = new List<VMVentasSemana>();
                List<VMProductosSemana> listaProductoSemana = new List<VMProductosSemana>();

                foreach(KeyValuePair<string,int> item in await _dashboardServicio.VentasUltimaSemana())
                {
                    listaVentaSemana.Add(new VMVentasSemana()
                    {
                        Fecha = item.Key,
                        Total = item.Value
                    });
                } 

                foreach (KeyValuePair<string, int> item in await _dashboardServicio.ProductosTopUltimaSemana())
                {
                    listaProductoSemana.Add(new VMProductosSemana()
                    {
                        Producto = item.Key,
                        Cantidad = item.Value
                    });
                }

                vmDashBoard.VentasUltimaSemana = listaVentaSemana;
                vmDashBoard.ProductosTopUltimaSemana = listaProductoSemana;

                gResponse.Estado = true;
                gResponse.Objeto = vmDashBoard;
            }
            catch(Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
