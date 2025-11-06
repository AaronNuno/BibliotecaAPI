using BibliotecaAPI.Servicios;
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
        private readonly IServicioHash servicioHash;

        public SeguridadController(IDataProtectionProvider protectionProvider,IServicioHash servicioHash)
        {
            protector = protectionProvider.CreateProtector("SeguridadController");
            protectorLimitadoPorTiempo = protector.ToTimeLimitedDataProtector();
            this.servicioHash = servicioHash;
        }

        [HttpGet("hash")]
        public ActionResult Hash(string textoPlano)
        {
            var hash1 = servicioHash.Hash(textoPlano);
            var hash2 = servicioHash.Hash(textoPlano);
            var hash3 = servicioHash.Hash(textoPlano, hash2.Sal);
            var resultado = new {textoPlano,hash1,hash2,hash3};
            return Ok(resultado);

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
