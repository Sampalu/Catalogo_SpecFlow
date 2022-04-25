using Catalogo.API.Hubs;
using Catalogo.Application.DTOs;
using Catalogo.Application.Interfaces;
using Catalogo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalogo.API.Controllers
{
    [Route("api/v1/[Controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;
        private readonly IHubContext<StreamingHub> _streaming;
        public CategoriasController(ICategoriaService categoriaService, IHubContext<StreamingHub> streaming)
        {
            _categoriaService = categoriaService;
            _streaming = streaming;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
        {
            var categorias = await _categoriaService.GetCategorias();
            return Ok(categorias);
        }

        [HttpGet("{id}", Name = "GetCategoria")]
        public async Task<ActionResult<Categoria>> Get(int id)
        {
            var categoria = await _categoriaService.GetById(id);

            if (categoria == null)
            {
                return NotFound();
            }
            return Ok(categoria);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CategoriaDTO categoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _categoriaService.Add(categoriaDto);
            await WriteOnStream(categoriaDto.CategoriaId.ToString() + "|" + categoriaDto.Nome);

            return new CreatedAtRouteResult("GetCategoria",
                new { id = categoriaDto.CategoriaId }, categoriaDto);
        }

        private async Task WriteOnStream(string endpointDataId)
        {
            await _streaming.Clients.All.SendAsync("addMessage", endpointDataId);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CategoriaDTO categoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != categoriaDto.CategoriaId)
            {
                return BadRequest();
            }
            await _categoriaService.Update(categoriaDto);
            await WriteOnStream(categoriaDto.CategoriaId.ToString() + "|" + categoriaDto.Nome);
            return Ok(categoriaDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Categoria>> Delete(int id)
        {
            var categoriaDto = await _categoriaService.GetById(id);
            if (categoriaDto == null)
            {
                return NotFound();
            }
            await _categoriaService.Remove(id);
            await WriteOnStream(categoriaDto.CategoriaId.ToString() + "|" + categoriaDto.Nome);
            return Ok(categoriaDto);
        }
    }
}
