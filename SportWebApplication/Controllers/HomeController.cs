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

        [HttpPost]
        public async Task<IActionResult> CreateSportsman(User user)
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return RedirectToAction("SportsmanList");
        }
        public IActionResult AgeGroupList()
        {
            FormAgeGroup ageGroup = new FormAgeGroup();
            ageGroup.AG = db.AgeGroups.ToList<AgeGroup>();
            if (ageGroup.AG.Count != 0)
                ageGroup.Yahr1 = ageGroup.AG[ageGroup.AG.Count - 1].Yahr2 + 1;
            else
                ageGroup.Yahr1 = 0;
            return View(ageGroup);
        }

        [HttpPost]
        public async Task<IActionResult> AgeGroupCreate(FormAgeGroup AgeG)
        {
            AgeGroup ageGroupM = new AgeGroup();
            ageGroupM.Yahr1 = AgeG.Yahr1;
            ageGroupM.Yahr2 = AgeG.Yahr2;
            ageGroupM.Sex = 1;
            ageGroupM.Laps = AgeG.LapsM;
            db.AgeGroups.Add(ageGroupM);

            AgeGroup ageGroupF = new AgeGroup();
            ageGroupF.Yahr1 = AgeG.Yahr1;
            ageGroupF.Yahr2 = AgeG.Yahr2;
            ageGroupF.Sex = 0;
            ageGroupF.Laps = AgeG.LapsF;
            db.AgeGroups.Add(ageGroupF);
            await db.SaveChangesAsync();
            return RedirectToAction("AgeGroupList");
        }
        public IActionResult Index()
        {
            //db.Database.EnsureDeleted();
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
