using System.Linq;
using MyProject.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using MyProject.ViewModels;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MyProject.Controllers
{
    public class UserController : Controller
    {
        UserManager<User> _userManager;
        SignInManager<User> _signinManager;
        private ApplicationContext _context;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext context)
        {
            _userManager = userManager;
            _signinManager = signInManager;
            _context = context;
        }

        public IActionResult Index() => View(_userManager.Users.ToList());
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
            return View(model);

        }
        [HttpGet]
        public IActionResult CreatePost()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(CreatePostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                byte[] photo=null;
                if (model.Photo != null)
                {
                    using (var binaryReader = new BinaryReader(model.Photo.OpenReadStream()))
                    {
                        photo = binaryReader.ReadBytes((int) model.Photo.Length);
                    }
                }
                var post = new Post { User = user, RegisterTime = DateTime.Now, Description = model.Description, Photo = photo, Title = model.Title, UserId=user.Id};
                _context.Posts.Add(post);
                _context.SaveChanges();
                return RedirectToAction("Index","Home");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteUsers(IFormCollection form)
        {
            string[] ids = form["id"];
            if (ids != null && ids.Length > 0)
            {
                foreach (var id in ids)
                {
                    User user = await _userManager.FindByIdAsync(id);
                    if (user != null)
                    {
                        IdentityResult result = await _userManager.DeleteAsync(user);
                    }
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Block(IFormCollection form)
        {
            string[] ids = form["id"];
            foreach (var id in ids)
            {
                User user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    user.Status = false;
                    await _userManager.UpdateAsync(user);
                }
            }
            foreach (var id in ids)
            {
                var user = await _userManager.FindByIdAsync(id);
                var AutUser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (AutUser == user)
                {
                    await _userManager.UpdateAsync(user);
                    await _signinManager.SignOutAsync();
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Users");
        }
        [HttpPost]
        public async Task<IActionResult> UnBlock(IFormCollection form)
        {
            string[] ids = form["id"];
            if (ids != null && ids.Length > 0)
            {
                foreach (var id in ids)
                {
                    User user = await _userManager.FindByIdAsync(id);
                    if (user != null)
                    {
                        user.Status = true;
                        await _userManager.UpdateAsync(user);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var resultSignIn = await _signinManager.PasswordSignInAsync(model.Email,model.LastPassword,false,false);
                if (resultSignIn.Succeeded) {
                    User user = await _userManager.FindByIdAsync(model.Id);
                    if (user != null)
                    {
                        var _passwordValidator =
                            HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                        var _passwordHasher =
                            HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;

                        IdentityResult result =
                            await _passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
                        if (result.Succeeded)
                        {
                            user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                            await _userManager.UpdateAsync(user);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Пользователь не найден");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Старый пароль ввден не верно");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            EditUserViewModel model = new EditUserViewModel { Id = user.Id, Email = user.Email, Name = user.Name };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.Name = model.Name;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }
    }
}
