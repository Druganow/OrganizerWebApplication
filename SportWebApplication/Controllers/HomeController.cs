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
            using (FileStream fs = new FileStream("competetion.json", FileMode.OpenOrCreate))
            {
                Competention competention;
                competention = await JsonSerializer.DeserializeAsync<Competention>(fs).ConfigureAwait(false);
                return View(competention);
            }
            
        }
        public IActionResult SportsmanList()
        {
            FormCreateUser createUser = new FormCreateUser();
            createUser.userList = db.Users.ToList();
            return View(createUser);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSportsman(FormCreateUser UL)
        {
            UL.user.RandNumber = Rand.Next(10000);
            db.Users.Add(UL.user);
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
            pf.users = db.Users.ToList();
            return View(pf);
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
            List<AgeGroup> ageGroups = db.AgeGroups.ToList();
            foreach(var item in ageGroups)
            {
                int interval = 0;
                //IEnumerable<User> users = db.Users.AsEnumerable()
                foreach (var us in db.Users.Where(t => (t.Sex==item.Sex&&t.Age >= item.Yahr1 && t.Age <=item.Yahr2 )).OrderBy(t => t.RandNumber))
                {
                    us.StartTime = interval;
                    interval += 15;
                   
                } 
                db.SaveChanges();
            }
            return RedirectToAction("Setting");
        }
    }
}
