﻿using CodeShareBackend.Data;
using CodeShareBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CodeShareBackend.Controllers
{
    [ApiController]
    [Route("account")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(ApplicationDbContext context, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost()]
        public async Task<IActionResult> SetOwner(string uniqueId, string ownerId)
        {
            var snippet = await _context.CodeSnippets.SingleOrDefaultAsync(s => s.UniqueId == uniqueId);
            if (snippet == null)
            {
                return NotFound("Snippet not found");
            }
            snippet.UserId = ownerId;
            await _context.SaveChangesAsync();
            return Ok("Owner set");
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountInfo()
        {
            User? user = await _userManager.GetUserAsync(User);
            Console.WriteLine(user?.Email + user?.Id);

            if (user == null)
            {
                return Unauthorized("User not found in token");
            }

            var accountInfo = await _context.Users
                .Where(u => u.Id == user.Id)
                .Include(u => u.CodeSnippets).Select(u =>
                new
                {
                    u.Id,
                    u.Email,
                    u.UserName,
                    CodeSnippets = u.CodeSnippets.Select(cs => cs.UniqueId).ToArray()
                }).FirstOrDefaultAsync();

            Console.WriteLine(accountInfo);
            return Ok(accountInfo);
        }

        [HttpPost("test")]
        public async Task<IActionResult> GetSnippets(string ownerId)
        {
            var snippets = await _context.CodeSnippets.Where(s => s.UserId == ownerId).ToListAsync();
            return Ok(snippets);
        }

    }
}
