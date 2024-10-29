using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebApi.Filters;
using WebApi.Models;
using WebApi.Models.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<UserController> logger
            )
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._logger = logger;
        }

        // GET: api/<UserController>/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> Get([FromServices]IMemoryCache memoryCache)
        {       
            if (!memoryCache.TryGetValue("userList", out List<User> users))
            {
                var usersList = await _userManager.Users.ToListAsync();
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };
                _logger.LogInformation("The user list was cached.");
                memoryCache.Set("userList", usersList, cacheOptions);
            }
            _logger.LogInformation("The list of users is returned.");
            return Ok(memoryCache.Get("userList"));
        }

        // Post api/<UserController>/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new User();
                user.UserName = model.UserName;
                user.Name = model.Name;

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogTrace("User {Username} was created successfully.", model.UserName);
                    return Ok("User was created successfully.");
                }
                _logger.LogWarning("User {Username} creation was not successful", model.UserName);
                return BadRequest(result.Errors.ToList());
            }
            _logger.LogError("Invalid model state during login attempt.");
            return BadRequest("Model is not valid !");
        }

        // Post api/<UserController>/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Username} logged in successfully.", model.UserName);
                    return Ok("Login was successful.");
                }
                _logger.LogWarning("Failed login attempt for user {Username}.", model.UserName);
                return BadRequest("User Name or Password is not valid !!");
            }
            _logger.LogError("Invalid model state during login attempt.");
            return BadRequest("Model is not valid !");
        }

        // Put api/<UserController>/Edit
        [HttpPut("Edit")]
        public async Task<IActionResult> Edit([FromBody]EditUserVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                
                if (user != null)
                {
                    user.Name = model.Name;
                    user.UserName = model.UserName;                
                    
                    var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        var updateResult = await _userManager.UpdateAsync(user);
                        if (updateResult.Succeeded)
                        {
                            _logger.LogInformation("User {Username} was edited .", model.UserName);
                            return Ok("User was edited .");
                        }
                        else
                        {
                            _logger.LogWarning("The user {Username} has not been edited", model.UserName);
                            return BadRequest(updateResult.Errors.ToList());
                        }
                    }
                    _logger.LogWarning("ChangePasswordAsync was failed for user {Username}", model.UserName);
                    return BadRequest(result.Errors.ToList());
                }
                else
                {
                    _logger.LogWarning("User {Username} not found", model.UserName);
                    return NotFound();
                }                         
            }
            _logger.LogError("Invalid model state during login attempt.");
            return BadRequest("Model is not valid !");
        }

    }
}
