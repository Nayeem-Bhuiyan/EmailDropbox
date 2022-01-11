﻿using Dropbox.Api;
using DropBoxTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DropBoxTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
             string token = "sl.A_4xJY0xOokqLVoXEA7uz_KnliAjEXkTV8BhffcAxktHuRTSPyIGAzIvQpo35TiNDT21kmbgpUY8Ga4qkVklU7D4i34n85x-0P8Wm86tUiAYeqhi1fwZSV0AyH7hmtAEytTmr1w";
            UserDetails data = new UserDetails();
            using (var dbx = new DropboxClient(token))
            {
                var id = await dbx.Users.GetCurrentAccountAsync();
                 data = new UserDetails
                {
                    userName=id.Name.DisplayName,
                    email=id.Email,
                    country=id.Country,
                    profileImageUrl=id.ProfilePhotoUrl

                };


            }

            return View(data);
        }




        //public async Task<IEnumerable<UploadedImageDetails>> UploadedImageInfo(this DropboxClient client)
        //{
        //    string token = "sl.A_1H9p1YUdJlLv7Dxa17hs_AeuglCLDkI_h-7zzxbA8IS0KWTg5-yZjMzpxh1NWyzPAEFYFd8JFHkMI2bw2fgPERxlSFwQlbglzlRvmeCADlgS6_g12gF93nFd8gp5riR9OFg04";
        //    await GetArticleList(new DropboxClient(token));


        //    return View(listData);
        //}



























        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
