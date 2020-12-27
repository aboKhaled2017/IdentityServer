using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.Data;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<IdentityServerUser> signInManager;
        private readonly UserManager<IdentityServerUser> userManager;

        public HomeController(
            SignInManager<IdentityServerUser> signInManager,
            UserManager<IdentityServerUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login(string returnUrl)
        {
            return View(model: new LoginViewModel {returnUrl=returnUrl});
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.username);
                if(user!=null)
                {
                    var loginRes = await signInManager.PasswordSignInAsync(user, model.password, false, false);
                    if(loginRes.Succeeded)
                    return Redirect(model.returnUrl);
                }
               
            }

                ViewBag.errors = "Login faild try again";
                return View(model);

            
        }

        public IActionResult Register(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
                return Redirect(returnUrl);
            return View(model: new RegisterViewModel { returnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
           
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.username);
                IdentityResult res=IdentityResult.Failed();
                if (user == null)
                {
                    user = new IdentityServerUser { UserName = model.username };
                     res =await userManager.CreateAsync(user, model.password);
                    var claims = model.Previliages
                        .Where(p => p.IsSelcted)
                        .Select(p => p.Name)
                        .Aggregate(new List<Claim>() { }, (prev, curr) =>
                        prev.Append(new Claim(curr, "true")).ToList());
                    await userManager.AddClaimsAsync(user, claims);
                }
                if(res.Succeeded)
                {
                    var loginRes = await signInManager.PasswordSignInAsync(user, model.password, false, false);
                    if (loginRes.Succeeded)
                        return Redirect(model.returnUrl);
                    else
                    {
                        ViewBag.errors = "Register faild try again";
                    }
                }
                else
                {
                    ViewBag.errors = "Register faild try again";
                }

            }
            else
            {
                ViewBag.errors = ModelState.Aggregate("", (prev, curr) =>
                $"{prev} [{curr.Key} : " +
                $"{curr.Value.Errors.Aggregate("",(p,c)=>$"{p} , {c.ErrorMessage}")}]");
            }
            return View(model);

        }

    }
}
