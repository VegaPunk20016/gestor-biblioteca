using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadHub.Libro.Application.Dtos;
using ReadHub.Libro.Application.Services;

namespace ReadHub.Libro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LibrosController : ControllerBase
    {
        private readonly BookService _bookService;

        public LibrosController(BookService bookService)
        {
            _bookService = bookService;
        }

        // Consultar todos los libros 
        [HttpGet]
        [Authorize] 
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var books = await _bookService.GetAllBooksAsync();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //Consultar libro por Id
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                if (book == null) return NotFound(new { message = "Libro no encontrado" });
                return Ok(book);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Crear libro 
        [HttpPost]
        [Authorize(Roles = "Bibliotecario")]
        public async Task<IActionResult> Create([FromBody] BookDto bookDto)
        {
            try
            {
                var createdBook = await _bookService.CreateBookAsync(bookDto);
                return CreatedAtAction(nameof(GetById), new { id = createdBook.Id }, createdBook);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Actualizar libro 
        [HttpPut("{id}")]
        [Authorize(Roles = "Bibliotecario")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BookDto bookDto)
        {
            try
            {
                var updatedBook = await _bookService.UpdateBookAsync(id, bookDto);
                if (updatedBook == null) return NotFound(new { message = "Libro no encontrado" });
                return Ok(updatedBook);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Eliminar libro 
        [HttpDelete("{id}")]
        [Authorize(Roles = "Bibliotecario")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var success = await _bookService.DeleteBookAsync(id);

                if (!success)
                    return NotFound(new { message = "Libro no encontrado" });

                return Ok(new { message = "Libro eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el libro", error = ex.Message });
            }
        }


        //Ver los libros que estan disponibles que su stock sea mayor a 0
        [HttpGet("LibrosDisponibles")]
        public async Task<IActionResult> GetAvailableBooks()
        {
            var books = await _bookService.GetAvailableBooksAsync();
            return Ok(books);
        }


    }
}
