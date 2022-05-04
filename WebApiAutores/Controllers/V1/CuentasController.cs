using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTOs;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/cuentas")]
    public class CuentasController : ControllerBase 
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IDataProtectionProvider dataProtectionProvider;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager, IDataProtectionProvider dataProtectionProvider, HashService hashService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.dataProtectionProvider = dataProtectionProvider;
            this.hashService = hashService;
            dataProtector = dataProtectionProvider.CreateProtector("valor_unico_y_quizas_secreto");
        }

        #region   <<< EJEMPLOS DE ENCRIPTACION AQUI >>>

        /*
        [HttpGet("hash/{textoPlano}", Name ="")]
        public ActionResult RealizarHash(string textoPlano)
        {
            var resultado1 = hashService.Hash(textoPlano);
            var resultado2 = hashService.Hash(textoPlano);
            return Ok(new
            {
                textoPlano = textoPlano,
                Hash1 = resultado1,
                Hash2 = resultado2
            });
        }

        [HttpGet("encriptar")]
        public ActionResult Encriptar()
        {
            var textoPlano = "Juan Miguel";
            var textoCifrado = dataProtector.Protect(textoPlano);
            var textoDesencriptado = dataProtector.Unprotect(textoCifrado);

            return Ok(new
            {
                textoPlano = textoPlano,
                textoCifrado = textoCifrado,
                textoDescriptado = textoDesencriptado
            });
        }

        [HttpGet("encriptarPorTiempo")]
        public ActionResult EncriptarPorTiempo()
        {
            var protectorLimitadoPorTiempo = dataProtector.ToTimeLimitedDataProtector();
            var textoPlano = "Juan Miguel";
            var textoCifrado = protectorLimitadoPorTiempo.Protect(textoPlano, lifetime:TimeSpan.FromSeconds(5));
            Thread.Sleep(6000);//Le pone una pausa al hilo que se esta ejecuentado por 6 segundos
            var textoDesencriptado = protectorLimitadoPorTiempo.Unprotect(textoCifrado);

            return Ok(new
            {
                textoPlano = textoPlano,
                textoCifrado = textoCifrado,
                textoDescriptado = textoDesencriptado
            });
        }
         */
        #endregion

       
        [HttpPost("registrar", Name ="registrarUsuario")] // api/cuentas/registrar
        public async Task<ActionResult<RepuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser { UserName = credencialesUsuario.Email, Email = credencialesUsuario.Email };

            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);

            if (resultado.Succeeded)
            {
                return await contruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }
       
        [HttpPost("login", Name ="loginUsuario")] // api/cuentas/login
        public async Task<ActionResult<RepuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email, credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return await contruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        [HttpGet("{RenovarToken}", Name ="renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RepuestaAutenticacion>> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;

            var credencialesUsuario = new CredencialesUsuario()
            {
                Email = email
            };

            return await contruirToken(credencialesUsuario);
        }
        private async Task< RepuestaAutenticacion> contruirToken(CredencialesUsuario credencialesUsuario)
        {
            var claims = new List<Claim>
            {
                new Claim("email", credencialesUsuario.Email),
            };

            var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario);
            claims.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials: creds);

            return new RepuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };

        }

        [HttpPost("HacerAdmin", Name ="hecerAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDTo editarAdminDTo)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTo.Email);
            await userManager.AddClaimAsync(usuario, new Claim("EsAdmin", "1"));
            return NoContent();
        }

        [HttpPost("RemoverAdmin", Name ="renovarAdmin")]
        public async Task<ActionResult> RemoverAdmin(EditarAdminDTo editarAdminDTo)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTo.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("EsAdmin", "1"));
            return NoContent();
        }

    }
}
