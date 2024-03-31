using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EBBooksAPI.Models;

namespace EBBooksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly EbbooksContext _context;

        public CartsController(EbbooksContext context)
        {
            _context = context;
        }

        // GET: api/Carts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCarts()
        {
            return await _context.Carts.ToListAsync();
        }

        // GET: api/Carts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(string id)
        {
            var cart = await _context.Carts.FindAsync(id);

            if (cart == null)
            {
                return NotFound();
            }

            return cart;
        }

        // PUT: api/Carts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCart(string id, Cart cart)
        {
            if (id != cart.Email)
            {
                return BadRequest();
            }

            _context.Entry(cart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
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

        // POST: api/Carts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cart>> PostCart(Cart cart)
        {
            _context.Carts.Add(cart);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CartExists(cart.Email))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCart", new { id = cart.Email }, cart);
        }

        // DELETE: api/Carts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(string id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartExists(string id)
        {
            return _context.Carts.Any(e => e.Email == id);
        }

        //code to add item to cart
        [HttpPost("addtocart/{itemcode}")]
        public async Task<IActionResult> AddToCart(string itemcode)
        {
            if (itemcode == null)
            {
                return NotFound("Item code cannot be null.");
            }

            var email = HttpContext.Session.GetString("LoggedInUser");
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("User is not logged in.");
            }

            var item = await _context.Items.FindAsync(itemcode);
            if (item == null)
            {
                return NotFound("Item not found.");
            }

            if (item.Stock <= 0)
            {
                return BadRequest("Item is out of stock.");
            }

            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.ItemCode == itemcode && c.Email == email);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += 1;
                _context.Carts.Update(existingCartItem);
            }
            else
            {
                var newCartItem = new Cart
                {
                    ItemCode = itemcode,
                    Email = email,
                    Quantity = 1,
                    ItemCodeNavigation = item,
                    EmailNavigation = await _context.Users.FindAsync(email)
                };
                await _context.Carts.AddAsync(newCartItem);
            }

            item.Stock -= 1;
            _context.Items.Update(item);

            await _context.SaveChangesAsync();
            return Ok("Item added to the cart.");
        }


        [HttpGet("viewcart")]
        public async Task<IActionResult> ViewCart()
        {
            var email = HttpContext.Session.GetString("LoggedInUser");
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("User is not logged in.");
            }

            var cartItems = await _context.Carts
                .Where(c => c.Email == email)
                .Include(c => c.ItemCodeNavigation)
                .ToListAsync();

            // Calculate the total price of the cart
            decimal totalPrice = cartItems.Sum(item => item.Quantity * item.ItemCodeNavigation.ItemPrice);

            // Create a model to return the cart items and total price
            var cartModel = new
            {
                Items = cartItems,
                TotalPrice = totalPrice
            };

            return Ok(cartModel);
        }
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] Cart cart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Add(cart);
            await _context.SaveChangesAsync();

            // Assuming you want to return the newly created cart item
            return CreatedAtAction(nameof(GetCart), new { id = cart.ItemCode }, cart);
        }

        // Example method to get a specific cart item by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCart(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            return Ok(cart);
        }


    }
}
