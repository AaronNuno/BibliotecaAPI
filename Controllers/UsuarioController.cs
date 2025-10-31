using BibliotecaAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
  

        public UsuarioController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }
        [HttpPost("registro")]
        [AllowAnonymous]
        public async Task<ActionResult<RespuestaAutentificacionDTO>> Registrar(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            var usuario = new IdentityUser
            {
                UserName = credencialesUsuarioDTO.Email,
                Email = credencialesUsuarioDTO.Email
            };

            var resultado = await userManager.CreateAsync(usuario, credencialesUsuarioDTO.Password!);

            if(resultado.Succeeded)
            {
                var respuestaAutenticacion = await ConstruirToken(credencialesUsuarioDTO);
                return respuestaAutenticacion;

            }
            else
            {
                foreach(var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return ValidationProblem();

            }

        }

        private async Task<RespuestaAutentificacionDTO> ConstruirToken(
            CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            var claims = new List<Claim>
            {
                new Claim ("email", credencialesUsuarioDTO.Email),
                new Claim ("lo que yo quiera", "cualquier valor")
            };

            var usuario = await userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario!);

            claims.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]!));
            var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            var tokenDeSeguridad = new JwtSecurityToken(issuer: null, audience: null,
                claims: claims, expires: expiracion, signingCredentials: credenciales);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDeSeguridad);

            return new RespuestaAutentificacionDTO
            { Token = token,
            Expiracion = expiracion };
        }
    }

}
