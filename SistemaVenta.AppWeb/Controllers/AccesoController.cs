using Microsoft.AspNetCore.Mvc;

using SistemaVentas.AppWeb.Models.ViewModels;
using SistemaVentas.BLL.Interfaces;
using SistemaVentas.Entity;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Numerics;

namespace SistemaVentas.AppWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public AccesoController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public IActionResult login()
        {

            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", "Home");
            }

            return View();
        }

        public IActionResult RestablecerClave()
        {

            return View();
        }

        [HttpPost]
        public async  Task<IActionResult> login(VMUsuarioLogin modelo)
        {
            Usuario usuario_encontrado = await _usuarioService.ObtenerPorCredenciales(modelo.Correo, modelo.Clave);

            if(usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            ViewData["Mensaje"] = null;

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.Nombre),
                new Claim(ClaimTypes.NameIdentifier, usuario_encontrado.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, usuario_encontrado.IdRol.ToString()),
                new Claim("UrlFoto", usuario_encontrado.UrlFoto),
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = modelo.MantenerSesion
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                , new ClaimsPrincipal(claimsIdentity),
                properties);

            return RedirectToAction("index","Home");
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerClave(VMUsuarioLogin modelo)
        {
            try
            {
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/RestablecerClave?clave=[clave]";

                bool resultado = await _usuarioService.RestablecerClave(modelo.Correo, urlPlantillaCorreo);
            
                if(resultado)
                {
                    ViewData["Mensaje"] = "Listo, su contraseña fue restablecida. Revise su correo";
                    ViewData["MensajeError"] = null;
                }else
                {
                    ViewData["MensajeError"] = "Tenemos problemas. Porfavor intente de nuevo mas tarde";
                    ViewData["Mensaje"] = null;
                }
            
            }
            catch (Exception ex) 
            {
                ViewData["MensajeError"] = ex.Message;
                ViewData["Mensaje"] = null;
            }

            return View();
        }
    }
}
