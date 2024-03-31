using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EBBooksAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace EBBooksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly EbbooksContext _context;

        public UsersController(EbbooksContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, User user)
        {
            if (id != user.Email)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.Email))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUser", new { id = user.Email }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Email == id);
        }

        //[HttpPost("register")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> RegisterView([FromBody] LoginModel login, [FromBody] Models.User user)
        //{
        //    try
        //    {
        //        // Validate email format
        //        if (!login.Email.Contains("@"))
        //        {
        //            return BadRequest("Invalid email entered");
        //        }

        //        // Validate password length
        //        if (login.Password.Length < 8)
        //        {
        //            return BadRequest("Password must be at least 8 characters.");
        //        }

        //        // Validate password complexity (at least 1 special character and 1 number)
        //        if (!HasSpecialCharacter(login.Password) || !HasNumber(login.Password))
        //        {
        //            return BadRequest("Password must contain at least 1 special character (!@#$%^&*) and 1 number.");
        //        }



        //        // Assuming you have a method to hash the password before storing it
        //      user.PasswordHash = HashPassword(user, login.Password);

        //        user.Email = login.Email;
        //        user.UserRole = "Customer";

        //        _context.Add(user);

        //        await _context.SaveChangesAsync();
        //        return Ok("User registered successfully.");

        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception or handle it as needed
        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        //private bool UserExistsByEmail(string email)
        //{
        //    return _context.Users.Any(u => u.Email == email);
        //}

        //private bool HasSpecialCharacter(string password)
        //{
        //    return password.Any(c => char.IsSymbol(c));
        //}

        //private bool HasNumber(string password)
        //{
        //    return password.Any(char.IsDigit);
        //}

        //private string HashPassword(Models.User user, string password)
        //{
        //    // Implement your password hashing logic here
        //    // For example, using ASP.NET Core Identity's PasswordHasher
        //    var passwordHasher = new PasswordHasher<Models.User>();
        //    return passwordHasher.HashPassword(user, password);
        //}

    



    }


}
