using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using letsdocoding_cognitologin.CognitoAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using letsdocoding_cognitologin.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using QRCoder;

namespace letsdocoding_cognitologin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserHelper _userHelper;

        public HomeController(ILogger<HomeController> logger, IUserHelper userHelper)
        {
            _logger = logger;
            _userHelper = userHelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> GenerateMfaCode()
        {
            await GenerateSecret();
            return View(new UserSecretModel());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> GenerateMfaCode(UserSecretModel model)
        {
            if (!int.TryParse(model.UserCode, out _) || model.UserCode.Length != 6)
            {
                ModelState.AddModelError(nameof(model.UserCode), "OTP should be numeric and of 6 digits.");
            }

            if (ModelState.IsValid)
            {
                model.IsVerified = await _userHelper.VerifyToken(model.UserCode, model.DeviceName);
            }


            await GenerateSecret();
            return View(model);
        }


        /// <summary>
        /// Generate the Secret
        /// </summary>
        /// <returns></returns>
        private async Task GenerateSecret()
        {
            var secret = await _userHelper.GenerateSecret();
            PayloadGenerator.OneTimePassword generator = new PayloadGenerator.OneTimePassword()
            {
                Secret = secret,
                Issuer = "AWS Cognito LDC 2021",
                Label = User.Identity.Name,
            };
            string payload = generator.ToString();

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            //var qrCodeAsBitmap = qrCode.GetGraphic(20);
            using (var ms = new MemoryStream())
            {
                using (var image = qrCode.GetGraphic(20))
                {
                    image.Save(ms, ImageFormat.Png);
                    ViewBag.image = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
        }


        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
