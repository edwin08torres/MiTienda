using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using SistemaVentas.BLL.Interfaces;
using SistemaVentas.DAL.Interfaces;
using SistemaVentas.Entity;
using System.Globalization;

namespace SistemaVentas.BLL.Implementacion
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IVentaRepository                   _ventaRepository;
        private readonly IGenericRepository<DetalleVenta>   _repositorioDetalleVenta;
        private readonly IGenericRepository<Categoria>      _repositorioCategoria;
        private readonly IGenericRepository<Producto>       _repositorioProducto;
        private DateTime FechaInicio = DateTime.Now;

        public DashBoardService(
              IVentaRepository ventaRepository
            , IGenericRepository<DetalleVenta> repositorioDetalleVenta
            , IGenericRepository<Categoria> repositorioCategoria
            , IGenericRepository<Producto> repositorioProducto)
        {
            _ventaRepository = ventaRepository; ;
            _repositorioDetalleVenta = repositorioDetalleVenta;
            _repositorioCategoria = repositorioCategoria;
            _repositorioProducto = repositorioProducto;

            FechaInicio = FechaInicio.AddDays(-7);
        }
        public async Task<int> TotalVentasultimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _ventaRepository.Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date);
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> TotalIngresoUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _ventaRepository.Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date);

                decimal resultado = query
                    .Select(v => v.Total)
                    .Sum(v => v.Value);

                return Convert.ToString(resultado, new CultureInfo("es-NIC"));
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> TotalProductos()
        {
            try
            {
                IQueryable<Producto> query = await _repositorioProducto.Consultar();
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> TotalCategorias()
        {
            try
            {
                IQueryable<Categoria> query = await _repositorioCategoria.Consultar();
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Dictionary<string, int>> VentasUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _ventaRepository.Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date);

                Dictionary<string, int> resultado = query
                       .GroupBy(v => v.FechaRegistro.Value.Date).OrderByDescending(g => g.Key)//El key hace referencia al GroupBY
                        .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() }) // Acá creamos un nuevo objeto, con la propiedad fecha y la propiedad total
                        .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);

                return resultado;
            }
            catch
            {
                throw;
            }
        }
        public async Task<Dictionary<string, int>> ProductosTopUltimaSemana()
        {
            try
            {
                IQueryable<DetalleVenta> query = await _repositorioDetalleVenta.Consultar();

                Dictionary<string, int> resultado = query
                        .Include(v => v.IdVentaNavigation)
                        .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= FechaInicio.Date)
                       .GroupBy(dv => dv.DescripcionProducto).OrderByDescending(g => g.Count())
                        .Select(dv => new { producto = dv.Key, total = dv.Count() }).Take(4) 
                        .ToDictionary(keySelector: r => r.producto, elementSelector: r => r.total);

                return resultado;
            }
            catch
            {
                throw;
            }
        }
  
    }
}
