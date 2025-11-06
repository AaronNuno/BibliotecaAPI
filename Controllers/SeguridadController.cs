using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers
{
    [Route("api/seguridad")]
    [ApiController]
    public class SeguridadController : ControllerBase
    {
        private IDataProtector protector;
        private ITimeLimitedDataProtector protectorLimitadoPorTiempo;

        public SeguridadController(IDataProtectionProvider protectionProvider)
        {
            protector = protectionProvider.CreateProtector("SeguridadController");
            protectorLimitadoPorTiempo = protector.ToTimeLimitedDataProtector();

        }

        [HttpGet("encriptar-limitado-por-tiempo")]
        public ActionResult EncriptarLimitadoPorTiempo(string textoPlano)
        {
            string textoCrifrado = protectorLimitadoPorTiempo.Protect(textoPlano, lifetime : TimeSpan.FromSeconds(30));
            return Ok(new { textoCrifrado });
        }

        [HttpGet("desencriptar-limitado-por-tiempo")]
        public ActionResult DesencriptarLimitadoPorTiempo(string textoCifrado)
        {
            string textoPlano = protectorLimitadoPorTiempo.Unprotect(textoCifrado);
            return Ok(new { textoPlano });

        }



        [HttpGet("encriptar")]
        public ActionResult Encriptar(string textoPlano)
        {
            string textoCrifrado = protector.Protect(textoPlano);
            return Ok(new { textoCrifrado });
        }

        [HttpGet("desencriptar")]
        public ActionResult Desencriptar(string textoCifrado)
        {
            string textoPlano = protector.Unprotect(textoCifrado);
            return Ok(new {textoPlano});

        }
    }
}
