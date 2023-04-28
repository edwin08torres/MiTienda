using SistemaVentas.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.BLL.Interfaces
{
    public interface IUsuarioService
    {
        Task<List<Usuario>> Lista();
        Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string NombreFoto = "", string urlPlantillaCorreo = "");
        Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string NombreFoto = "");
        Task<bool> Eliminar(int idUsuario);
        Task<Usuario> ObtenerPorCredenciales(string correo, string clave);
        Task<Usuario> ObtenerPorID(int idUsuario);
        Task<bool> GuardarPeril(Usuario entidad);
        Task<bool> CambiarClave(int idUsuario, string claveActual, string claveNueva);
        Task<bool> RestablecerClave(string correo, string urlPlantillaCorreo);
    }
}
