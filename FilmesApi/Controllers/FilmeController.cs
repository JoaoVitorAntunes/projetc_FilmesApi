﻿using AutoMapper;
using FilmesApi.Data;
using FilmesApi.Data.Dtos;
using FilmesApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class FilmeController : ControllerBase
    {

        private FilmeContext _context;
        private IMapper _mapper;

        public FilmeController(FilmeContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /* --- [HttpPost] = Metodo que faz a inserção de recursos no sistema --- */
        [HttpPost]
        public IActionResult AdicionaFilme([FromBody] CreateFilmeDtos filmeDto) 
        {

            Filme filme = _mapper.Map<Filme>(filmeDto);
            _context.Filmes.Add(filme);
            _context.SaveChanges();
            return CreatedAtAction(nameof(RecuperaFilmePorId), new {id = filme.Id}, filme);
        }

        /* --- [HttpGet] = Metodo que ira fazer a leitura dos recursos do sistema --- */
        [HttpGet]
        public IEnumerable<Filme> RecuperaFilmes([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            return _context.Filmes.Skip(skip).Take(take);
        }

        [HttpGet("{id}")]
        public IActionResult RecuperaFilmePorId(int id)
        {
            var filme = _context.Filmes.FirstOrDefault(filmes => filmes.Id == id);
            if(filme == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(filme);
            }
        }

        /* --- [HttpPut] = Precisa atualizar todos os campos --- */
        [HttpPut("{id}")]
        public IActionResult AtualizaFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
        {
            var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
            if(filme == null) return NotFound();
            _mapper.Map(filmeDto, filme);
            _context.SaveChanges();
            return NoContent();
        }

        /* --- [HttpPatch] = Atualiza uma unica informação --- */
        [HttpPatch("{id}")]
        public IActionResult AtualizaFilmeParcial(int id, JsonPatchDocument<UpdateFilmeDto> patch)
        {
            var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
            if (filme == null) return NotFound();

            var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);

            patch.ApplyTo(filmeParaAtualizar, ModelState);
            if (!TryValidateModel(filmeParaAtualizar))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(filmeParaAtualizar, filme);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
