﻿using Dropbox.Api;
using DropBoxTest.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using Dropbox.Api.Common;
using Dropbox.Api.Files;
using Dropbox.Api.Team;
using DropBoxTest.Areas.DropboxInfo.Models;
using System.Globalization;
using System.Text;

namespace DropBoxTest.Areas.DropboxInfo.Controllers
{
    [Area("DropboxInfo")]
    public class DropboxController : Controller
    {
        string token = "sl.BAI7o5q8qOiSBq2t3Tx3Ioazx-_og11AsG7HSCJW1sF41Lo2G2QsUXhVOKLiqDfMF9XXWyX9Or_BgoiqYEMGZqM16WofDpgc7-aeUBO8TaqyvwGy-RSBF6GpAaNuHBrEnQ4IHKQ";
        public async Task<IActionResult> FolderList()
        {
            List<FolderDetails> listFolder = new List<FolderDetails>();
            using (var client = new DropboxClient(token))
            {
                var list = await client.Files.ListFolderAsync(string.Empty, true);
                var folders = list.Entries.Where(x => x.IsFolder);
                var files = list.Entries.Where(x => x.IsFile);

                foreach (var folder in folders)
                {

                    FolderDetails data = new FolderDetails
                    {
                        folderName = folder.Name,
                        folderPath = folder.PathDisplay,
                        downloadLink = "https://www.dropbox.com/home" + folder.PathDisplay,
                        count = files.Count()

                    };
                    listFolder.Add(data);

                }
            }
            return View(listFolder);

        }

        public async Task<IActionResult> DropboxUserInfo()
        {

            UserDetails data = new UserDetails();

            using (var dbx = new DropboxClient(token))
            {

                var user = await dbx.Users.GetCurrentAccountAsync();
                data = new UserDetails
                {
                    userName = user.Name?.DisplayName,
                    email = user.Email,
                    country = user.Country,
                    profileImageUrl = user.ProfilePhotoUrl
                };
            }
            return View(data);
        }

        [HttpGet]
        public IActionResult CreateFolder()
        {
            CreateFolderViewModel model = new CreateFolderViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFolder(CreateFolderViewModel model)
        {

            var dropBoxclient = new DropboxClient(token);
            var list = await dropBoxclient.Files.ListFolderAsync(string.Empty);

            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                if (item.Name.Equals(model.folderName))
                {

                    model.errorResponse = "Sorry: folder with name " + model.folderName + " already exists!";

                }

            }
            if (model.errorResponse != "Sorry: folder with name " + model.folderName + " already exists!")
            {
                Dropbox.Api.Files.CreateFolderArg folderArg = new CreateFolderArg("/" + model.folderName);
                await dropBoxclient.Files.CreateFolderV2Async(folderArg);
                model.successResponse = "Successfully Created folder named  " + model.folderName;
                model.redirectFolder = "https://www.dropbox.com/home/" + model.folderName;
            }
            else
            {
                model.redirectFolder = "https://www.dropbox.com/home/" + model.folderName;
            }

            var user = await dropBoxclient.Users.GetCurrentAccountAsync();

            var filename = string.Format(
                   CultureInfo.InvariantCulture,
                   user.Name.DisplayName,
                   DateTime.Now);
            string targetFileName = user.Name.DisplayName + DateTime.Now.ToString("yymmssfff") + ".jpg";
            string srcFile = @"D:\DownloadImage\download.jpg";


                var targetFolder = "/" + model.folderName+"/";

       
                using (var fileToSave = new FileStream(srcFile, FileMode.Open))
                {
                  
                    var updated = await dropBoxclient.Files.UploadAsync(
                        targetFolder + targetFileName,
                        WriteMode.Overwrite.Instance,
                        body: fileToSave);
                }
            

         

            //using (var mem = new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(file)))
            //{
            //    await dropBoxclient.Files.UploadAsync(folder + fName, WriteMode.Overwrite.Instance, body: mem);
            //}
            //foreach (var item in System.IO.File.ReadAllText(file))
            //{

            //}

            return View(model);
        }


        public IActionResult ListTeamMembers()
        {
            
            return View();
        }

        private async Task<IActionResult> ListTeamMembers(DropboxTeamClient client)
        {
            var members = await client.Team.MembersListAsync();
            List<TeamMember> dataList = new List<TeamMember>();
            foreach (var member in members.Members)
            {
                TeamMember data = new TeamMember
                {
                    TeamMemberId = member.Profile.TeamMemberId,
                    Name = member.Profile.Name.ToString(),
                    Email = member.Profile.Email
                };
                dataList.Add(data);
            }

            return View(dataList);
        }





        public async Task Upload(string remoteFileName, string localFilePath)
            {

                using (var dbx = new DropboxClient(token))
                {
                    using (var fs = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
                    {
                        await dbx.Files.UploadAsync($"/{remoteFileName}", WriteMode.Overwrite.Instance, body: fs);
                    }
                }
            }

            //public IActionResult Upload()
            //{

            //    return View();
            //}
            //[HttpPost]
            //public async Task<ActionResult> Upload(string name, DateTime date, string content)
            //{
            //    var filename = string.Format(
            //        CultureInfo.InvariantCulture,
            //        "{0}.{1:yyyyMMdd}.md",
            //        name,
            //        date);

            //    var client = new DropboxClient(token);
            //    if (client == null)
            //    {
            //        return RedirectToAction("Index", "Home");
            //    }

            //    using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            //    {
            //        var upload = await client.Files.UploadAsync("/" + filename, body: mem);
            //    }

            //}





        }
    }
