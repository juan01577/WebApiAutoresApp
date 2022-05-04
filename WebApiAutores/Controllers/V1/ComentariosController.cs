using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Utilidades;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/libros/{libroId:int}/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet(Name = "obtenerComentariosLibro")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId, [FromQuery] PaginacionDTO paginacionDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }

            var queryable = context.Comentarios.Where(comentarioDB => comentarioDB.Libroid == libroId).AsQueryable();

            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);

            var comentarios = await queryable.OrderBy(comentario => comentario.Id).Paginar(paginacionDTO).ToListAsync();
            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id:int}", Name = "obtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetPorId(int id)
        {
            var comentarios = await context.Comentarios.FirstOrDefaultAsync(comentarioDB => comentarioDB.Id == id);

            if (comentarios == null)
            { 
                return NotFound(); 
            } 

            return mapper.Map<ComentarioDTO>(comentarios);
        }

        [HttpPost(Name = "crearComentario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;

            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }   

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.Libroid = libroId;
            comentario.UsuarioId = usuarioId;
            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDTo = mapper.Map<ComentarioDTO>(comentario);
            return CreatedAtRoute("obtenerComentario", new { id = comentario.Id, libroId = libroId }, comentarioDTo);

        }

        [HttpPut("{id:int}", Name = "actualizarComentario")] // api/autores/1
        public async Task<ActionResult> Put(int libroId, int id, ComentarioCreacionDTO comentarioCreacionDTO)
        {

            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }

            var existeComentario = await context.Comentarios.AnyAsync(comentarioDB => comentarioDB.Id == id);
            if (!existeComentario)
            {
                return NotFound();
            }

            var cometario = mapper.Map<Comentario>(comentarioCreacionDTO);
            cometario.Id = id;
            cometario.Libroid = libroId;
            context.Update(cometario);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
