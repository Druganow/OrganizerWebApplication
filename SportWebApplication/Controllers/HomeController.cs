using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportWebApplication.Models;
using System;
using System.Collections.Generic;
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

        public async Task<IActionResult> DeleteSportsmans()
        {
            db.Sportsmans.RemoveRange(db.Sportsmans);
            await db.SaveChangesAsync();
            return RedirectToAction("Setting");
        }

        public async Task<IActionResult> DeleteAgeGroup()
        {
            db.AgeGroups.RemoveRange(db.AgeGroups);
            await db.SaveChangesAsync();
            return RedirectToAction("Setting");
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
            db.Sportsmans.Remove(db.Sportsmans.Where(t => t.Id == ID).FirstOrDefault());
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
        public async Task<IActionResult> ProtokolForm()
        {
            ProtokolForm pf = new();
            pf.ageGroups = db.AgeGroups.ToList();
            pf.users = db.Sportsmans.ToList();
            using (FileStream fs = new("competetion.json", FileMode.Open))
            {
                pf.CompetetionName = (await JsonSerializer.DeserializeAsync<Competention>(fs)).Name;
            }
            return View(pf);
        }

        public async Task<IActionResult> DiplomaForm()
        {
            ProtokolForm pf = new();
            pf.ageGroups = db.AgeGroups.ToList();
            pf.users = db.Sportsmans.ToList();
            using (FileStream fs = new("competetion.json", FileMode.Open))
            {
                pf.CompetetionName = (await JsonSerializer.DeserializeAsync<Competention>(fs)).Name;
            }
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
            return RedirectToAction("Setting", competention);
        }

        public IActionResult InputResults()
        {
            ProtokolForm pf = new();
            pf.ageGroups = db.AgeGroups.ToList();
            pf.users = db.Sportsmans.ToList();
            return View(pf);
        }

        [HttpPost]
        public IActionResult FixInput(List<TimeSpan> times)
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
            return RedirectToAction("ResultCompetetion");
        }

        public async Task<IActionResult> ResultCompetetion()
        {
            ProtokolForm pf = new ProtokolForm();
            pf.ageGroups = db.AgeGroups.ToList();
            pf.users = db.Sportsmans.ToList();
            using (FileStream fs = new("competetion.json", FileMode.Open))
            {
                pf.CompetetionName = (await JsonSerializer.DeserializeAsync<Competention>(fs)).Name;
            }
            return View(pf);
        }

        public async Task<IActionResult> DeleteLastAgeGroup()
        {
            db.AgeGroups.Remove(db.AgeGroups.OrderBy(t => t.Id).Last());
            await db.SaveChangesAsync();
            db.AgeGroups.Remove(db.AgeGroups.OrderBy(t => t.Id).Last());
            await db.SaveChangesAsync();
            return RedirectToAction("AgeGroupList");
        }
        public IActionResult Protokol()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile fileExcel)
        {
            if (fileExcel != null)
            {

                using (XLWorkbook workBook = new XLWorkbook(fileExcel.OpenReadStream(), XLEventTracking.Disabled))
                {
                    foreach (IXLWorksheet worksheet in workBook.Worksheets)
                    {
                        foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                        {
                            try
                            {
                                Sportsman SM = new Sportsman();
                                SM.Name = row.Cell(1).Value.ToString();
                                SM.Age = int.Parse(row.Cell(2).Value.ToString());
                                if (row.Cell(3).Value.ToString() == "м")
                                    SM.Sex = 1;
                                else SM.Sex = 0;
                                SM.RandNumber = Rand.Next(10000);
                                db.Sportsmans.Add(SM);
                                await db.SaveChangesAsync();
                            }
                            catch
                            {

                            }
                        }

                    }
                }

            }
            return RedirectToAction("SportsmanList");
        }

    }
}
