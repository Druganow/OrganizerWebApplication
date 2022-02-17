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
        Random Rand;
        public HomeController(ApplicationContext context)
        {
            db = context;
            Rand = new Random();
        }
        public async Task<IActionResult> Setting()
        {
            Competention competention;
            using (FileStream fs = new FileStream("competetion.json", FileMode.Open))
            {
                competention = await JsonSerializer.DeserializeAsync<Competention>(fs);
            }
            return View(competention);
        }
        public IActionResult SportsmanList()
        {
            FormCreateUser createUser = new FormCreateUser();
            createUser.userList = db.Sportsmans.ToList();
            return View(createUser);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSportsman(FormCreateUser UL)
        {
            UL.sportsman.RandNumber = Rand.Next(10000);
            db.Sportsmans.Add(UL.sportsman);
            await db.SaveChangesAsync();
            return RedirectToAction("SportsmanList");
        }

        public async Task<IActionResult> DeleteSportsman(int ID)
        {
            db.Sportsmans.Remove(db.Sportsmans.Where(t=> t.Id==ID).FirstOrDefault());
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
        public IActionResult ProtokolForm()
        {
            ProtokolForm pf = new ProtokolForm();
            pf.ageGroups = db.AgeGroups.ToList();
            pf.users = db.Sportsmans.ToList();
            return View(pf);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> FormCompetetion(Competention competention)
        {
            Competention c;
            using (FileStream fs = new("competetion.json", FileMode.Open))
            {
                c = await JsonSerializer.DeserializeAsync<Competention>(fs);
            }
            if (c.Start)
            {
                competention.Start = false;
                
            }
            else
            {
                competention.Start = true;
                List<AgeGroup> ageGroups = db.AgeGroups.ToList();
                foreach (var item in ageGroups)
                {
                    TimeSpan interval = new TimeSpan(0, 0, 0);
                    foreach (var us in db.Sportsmans.Where(t => (t.Sex == item.Sex && t.Age >= item.Yahr1 && t.Age <= item.Yahr2)).OrderBy(t => t.RandNumber))
                    {
                        us.StartTime = interval;
                        interval += competention.Interval;
                    }
                    db.SaveChanges();
                }
            }
            using (FileStream fs = new("competetion.json", FileMode.Create))
            {
                await JsonSerializer.SerializeAsync<Competention>(fs, competention);
            }
            return RedirectToAction("Setting");
        }

        public IActionResult InputResults()
        {
            ProtokolForm pf = new();
            pf.ageGroups = db.AgeGroups.ToList();
            pf.users = db.Sportsmans.ToList();
            return View(pf);
        }

        [HttpPost]
        public void FixInput(List<TimeSpan> times)
        {
            int i = 0;
            List<AgeGroup> ageGroups = db.AgeGroups.ToList();
            foreach (var item in ageGroups)
            {
                foreach (var us in db.Sportsmans.Where(t => (t.Sex == item.Sex && t.Age >= item.Yahr1 && t.Age <= item.Yahr2)).OrderBy(t => t.RandNumber))
                {
                    us.ResultTime = times[i];
                    us.FinishTime = us.ResultTime - us.StartTime;
                    i++;
                }
                db.SaveChanges();
            }
            RedirectToAction("ResultCompetetion");
        }

        public IActionResult ResultCompetetion()
        {
            ProtokolForm pf = new ProtokolForm();
            pf.ageGroups = db.AgeGroups.ToList();
            pf.users = db.Sportsmans.ToList();
            return View(pf);
        }
    }
}
