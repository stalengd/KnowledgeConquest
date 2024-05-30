using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using KnowledgeConquest.Server.Models;

namespace KnowledgeConquest.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(AccountRegisterModel model)
        {
            var user = new User
            {
                UserName = model.Username,
                Firstname = model.Firstname,
                Surname = model.Surname,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return ValidationProblem();
            }

            await _signInManager.SignInAsync(user, false);
            return Ok();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(AccountLoginModel model)
        {
            var result =
                await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.Remember, false);
            if (!result.Succeeded)
                return BadRequest();

            return Ok();
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}
