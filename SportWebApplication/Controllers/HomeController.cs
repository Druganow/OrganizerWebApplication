using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SportWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext db;
        public HomeController(ApplicationContext context)
        {
            db = context;
        }
        public async Task<IActionResult> Setting()
        {
            using (FileStream fs = new FileStream("competetion.json", FileMode.OpenOrCreate))
            {
                Competention competention;
                competention = await JsonSerializer.DeserializeAsync<Competention>(fs).ConfigureAwait(false);
                return View(competention);
            }
            
        }
        public async Task<IActionResult> SportsmanList()
        {
            return View(await db.Users.ToListAsync());
        }
        public IActionResult CreateSportsman()
        {
            return View();
        }
        public IActionResult FormCreateAgeGroup()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateSportsman(User user)
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return RedirectToAction("SportsmanList");
        }
        public async Task<IActionResult> AgeGroupList()
        {
            return View(await db.AgeGroups.ToListAsync());
        }
        public IActionResult CreateAgeGroup()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAgeGroup(AgeGroup AG)
        {
            db.AgeGroups.Add(AG);
            await db.SaveChangesAsync();
            return RedirectToAction("AgeGroupList");
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> FormCompetetion(Competention competention)
        {
            competention.Start = true;
            using (FileStream fs = new FileStream("competetion.json", FileMode.OpenOrCreate))
            {
                await JsonSerializer.SerializeAsync<Competention>(fs, competention);
            }
            return RedirectToAction("Setting");
        }
    }
}
