using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Testv1.Models;

namespace Testv1.Controllers
{
    public class SyllabusController : Controller
    {

        LABTestDBEntities db = new LABTestDBEntities();
        // GET: Syllabus
        [HttpGet]
        public ActionResult SyllabusCreate()
        {
            var TradeList = db.tblTrades.ToList();
            ViewBag.Trade = TradeList;

            var LevelList = db.tblLevels .ToList();
            ViewBag.Level = LevelList;

            var LangList = db.tblLanguages.ToList();
            ViewBag.Language = LangList;

            return View();
        }
        [HttpPost]
        public ActionResult SyllabusCreate(tblSyllabu syllobj, HttpPostedFileBase uploadSyllabus, HttpPostedFileBase uploadTestPlan,int[] colLanguageId)
        {
            if (uploadSyllabus.ContentLength > 0)
            {
                var FileName = Path.GetFileName(uploadSyllabus.FileName);
                var GlobalUnique = Guid.NewGuid().ToString();
                var Destination = Path.Combine(Server.MapPath("~/SyllabusUpload"), GlobalUnique + FileName);
                uploadSyllabus.SaveAs(Destination);
                string sub = Destination.Substring(Destination.LastIndexOf("\\"));
                string[] split = sub.Split('\\');
                string newpath = split[1];
                string syllabuspath = "~/SyllabusUpload" + newpath;
                syllobj.colSyllabusDocUrl = syllabuspath;

            }
            if (uploadTestPlan.ContentLength > 0)
            {
                var FileName = Path.GetFileName(uploadTestPlan.FileName);
                var GlobalUnique = Guid.NewGuid().ToString();
                var Destination = Path.Combine(Server.MapPath("~/TestPlanUpload"), GlobalUnique + FileName);
                uploadTestPlan.SaveAs(Destination);
                string sub = Destination.Substring(Destination.LastIndexOf("\\"));
                string[] split = sub.Split('\\');
                string newpath = split[1];
                string testpath = "~/TestPlanUpload" + newpath;
                syllobj.colSyllabusDocUrl = testpath;

            }

            try
            {
                List<tblSyllabu> ListOfSyllabus = db.tblSyllabus.ToList();
                int SyllabusID = ListOfSyllabus.Count + 1;
                syllobj.colSyllabusId = SyllabusID;

                int[] langId = colLanguageId;
                List<tblSyllabusLanguage> syllabusLanguagesList = new List<tblSyllabusLanguage>();
                foreach (var id in langId)
                {
                    tblSyllabusLanguage syllabusLanguage = new tblSyllabusLanguage();
                    syllabusLanguage.colSyllabusId = SyllabusID;
                    syllabusLanguage.colLanguageId = id;

                    syllabusLanguagesList.Add(syllabusLanguage);

                }
                db.tblSyllabus.Add(syllobj);
                var message = db.SaveChanges();

                foreach (var lang in syllabusLanguagesList)
                {
                    db.tblSyllabusLanguages.Add(lang);
                    var message_2 = db.SaveChanges();
                }

            }
            catch (Exception ex)
            {

                Response.Write(ex);
            }

            var TradeList = db.tblTrades.ToList();
            ViewBag.Trade = TradeList;

            var LevelList = db.tblTrades.ToList();
            ViewBag.Level = LevelList;

            var LangList = db.tblTrades.ToList();
            ViewBag.Language = LangList;
            return View();
        }
        [HttpGet]
        public ActionResult SyllabusList()
        {
            Session["SyllabusId"] = null;
            List<tblSyllabu> syllabusList = db.tblSyllabus.ToList();
            List<tblTrade> tradeList = db.tblTrades.ToList();
            List<tblLevel> levelList = db.tblLevels.ToList();
            List<tblSyllabusLanguage> syllabusLanguage = db.tblSyllabusLanguages.ToList();
            List<tblLanguage> totalLanguage = db.tblLanguages.ToList();
            List<string> everySyllLang = new List<string>();
            foreach (var syllabus in syllabusList)
            {
                string s = "";
                foreach (var language in syllabusLanguage)
                {
                    if (syllabus.colSyllabusId == language.colSyllabusId)
                    {
                        s = s + (from p in totalLanguage
                                 where p.colLanguageId == language.colLanguageId
                                 select p.colLanguageShortName).SingleOrDefault() + " ";
                    }
                }
                everySyllLang.Add(s);
            }


            List<string>syllabusName = new List<string>();

            foreach (var syllabus in syllabusList)
            {
                string name = syllabus.colSyllabusName;
                syllabusName.Add(name);
                
            }

            List<string> tradeName = new List<string>();

            foreach (var syllabus in syllabusList)
            {
                string allTradeName = (from p in tradeList
                                       where p.colTradeId == syllabus.colTradeId
                                       select p.colTradeName).SingleOrDefault();

                tradeName.Add(allTradeName);
            }

            List<string> levelName = new List<string>();
            foreach (var syllabus in syllabusList)
            {
                string allLevelName = (from p in levelList
                                       where p.colLevelId == syllabus.colTradeId
                                       select p.colLevelName).SingleOrDefault();

                levelName.Add(allLevelName);
            }

            ViewBag.SyllabusList = syllabusList;
            ViewBag.Trade = tradeName;
            ViewBag.Level = levelName;
            ViewBag.Language = everySyllLang;
            ViewBag.SyllabusNameList = syllabusName;
            ViewBag.TotalTradeList = tradeList;
            ViewBag.TotalLevelList = levelName;

            return View(syllabusList);
        }

        [HttpPost]
        public ActionResult SyllabusList(string colTradeId, string colLevelId)
        {
            // If both are empty Rederect to same page
            if (colTradeId == "" && colLevelId == "" )
            {
                return RedirectToAction("SyllabusList", "Syllabus");
            }

            // If trade Id not empty but level Id empty 
            else if (colTradeId != "" && colLevelId == "")
            {
                int cTradeId = Convert.ToInt32(colLevelId);
                List<tblTrade> totalTrades = db.tblTrades.ToList();
                List<tblLevel> totalLevels = db.tblLevels.ToList();
                ViewBag.TotalTradeList = totalTrades;
                ViewBag.TotalLevelList = totalLevels;
                List<tblSyllabu> syllabusList = db.tblSyllabus.Where(x => x.colTradeId == cTradeId).ToList();

                List<tblSyllabusLanguage> syllabusLanguages = db.tblSyllabusLanguages.ToList();
                List<tblLanguage> totalLanguage = db.tblLanguages.ToList();
                List<string> eachSyllabusLanguage = new List<string>();

                foreach (var syllabus in syllabusList)
                {
                    string s = "";
                    foreach (var language in syllabusLanguages)
                    {
                        if (syllabus.colSyllabusId == language.colSyllabusId)
                        {
                            s = s + (from p in totalLanguage
                                     where p.colLanguageId == language.colLanguageId
                                     select p.colLanguageShortName).SingleOrDefault() + " ";
                        }
                    }

                    eachSyllabusLanguage.Add(s);
                }
                List<string> syllabusName = new List<string>();

                foreach (var syllabus in syllabusList)
                {
                    string name = syllabus.colSyllabusName;
                    syllabusName.Add(name);


                }
                List<string> tradeName = new List<string>();

                foreach (var syllabus in syllabusList)
                {
                    string eachTradeName =(from p in totalTrades
                                           where p.colTradeId == syllabus.colTradeId
                                           select p.colTradeName).SingleOrDefault();

                    tradeName.Add(eachTradeName);

                }
                List<string> levelName = new List<string>();


                foreach (var syllabus in syllabusList)
                {
                    string eachLevelName =(from p in totalLevels
                                           where p.colLevelId == syllabus.colLevelId
                                           select p.colLevelName).SingleOrDefault();

                    levelName.Add(eachLevelName);

                }

                ViewBag.SyllabusList = syllabusList;
                ViewBag.Language = eachSyllabusLanguage;
                ViewBag.Trade = tradeName;
                ViewBag.Level = levelName;
                ViewBag.SyllabusNameList = syllabusName;


                return View(syllabusList);



            }
            else if (colTradeId == "" && colLevelId != "")
            {
                int cTradeId = Convert.ToInt32(colLevelId);
                List<tblTrade> totalTrades = db.tblTrades.ToList();
                List<tblLevel> totalLevels = db.tblLevels.ToList();
                ViewBag.TotalTradeList = totalTrades;
                ViewBag.TotalLevelList = totalLevels;
                List<tblSyllabu> syllabusList = db.tblSyllabus.Where(x => x.colTradeId == cTradeId).ToList();

                List<tblSyllabusLanguage> syllabusLanguages = db.tblSyllabusLanguages.ToList();
                List<tblLanguage> totalLanguage = db.tblLanguages.ToList();
                List<string> eachSyllabusLanguage = new List<string>();

                foreach (var syllabus in syllabusList)
                {
                    string s = "";
                    foreach (var language in syllabusLanguages)
                    {
                        if (syllabus.colSyllabusId == language.colSyllabusId)
                        {
                            s = s + (from p in totalLanguage
                                     where p.colLanguageId == language.colLanguageId
                                     select p.colLanguageShortName).SingleOrDefault() + " ";
                        }
                    }

                    eachSyllabusLanguage.Add(s);
                }
                List<string> syllabusName = new List<string>();

                foreach (var syllabus in syllabusList)
                {
                    string name = syllabus.colSyllabusName;
                    syllabusName.Add(name);


                }
                List<string> tradeName = new List<string>();

                foreach (var syllabus in syllabusList)
                {
                    string eachTradeName = (from p in totalTrades
                                            where p.colTradeId == syllabus.colTradeId
                                            select p.colTradeName).SingleOrDefault();

                    tradeName.Add(eachTradeName);

                }
                List<string> levelName = new List<string>();


                foreach (var syllabus in syllabusList)
                {
                    string eachLevelName = (from p in totalLevels
                                            where p.colLevelId == syllabus.colLevelId
                                            select p.colLevelName).SingleOrDefault();

                    levelName.Add(eachLevelName);

                }

                ViewBag.SyllabusList = syllabusList;
                ViewBag.Language = eachSyllabusLanguage;
                ViewBag.Trade = tradeName;
                ViewBag.Level = levelName;
                ViewBag.SyllabusNameList = syllabusName;


                return View(syllabusList);



            }
            else
            {
                int cTradeId = Convert.ToInt32(colLevelId);
                List<tblTrade> totalTrades = db.tblTrades.ToList();
                List<tblLevel> totalLevels = db.tblLevels.ToList();
                ViewBag.TotalTradeList = totalTrades;
                ViewBag.TotalLevelList = totalLevels;
                List<tblSyllabu> syllabusList = db.tblSyllabus.Where(x => x.colTradeId == cTradeId).ToList();

                List<tblSyllabusLanguage> syllabusLanguages = db.tblSyllabusLanguages.ToList();
                List<tblLanguage> totalLanguage = db.tblLanguages.ToList();
                List<string> eachSyllabusLanguage = new List<string>();

                foreach (var syllabus in syllabusList)
                {
                    string s = "";
                    foreach (var language in syllabusLanguages)
                    {
                        if (syllabus.colSyllabusId == language.colSyllabusId)
                        {
                            s = s + (from p in totalLanguage
                                     where p.colLanguageId == language.colLanguageId
                                     select p.colLanguageShortName).SingleOrDefault() + " ";
                        }
                    }

                    eachSyllabusLanguage.Add(s);
                }
                List<string> syllabusName = new List<string>();

                foreach (var syllabus in syllabusList)
                {
                    string name = syllabus.colSyllabusName;
                    syllabusName.Add(name);


                }
                List<string> tradeName = new List<string>();

                foreach (var syllabus in syllabusList)
                {
                    string eachTradeName = (from p in totalTrades
                                            where p.colTradeId == syllabus.colTradeId
                                            select p.colTradeName).SingleOrDefault();

                    tradeName.Add(eachTradeName);

                }
                List<string> levelName = new List<string>();


                foreach (var syllabus in syllabusList)
                {
                    string eachLevelName = (from p in totalLevels
                                            where p.colLevelId == syllabus.colLevelId
                                            select p.colLevelName).SingleOrDefault();

                    levelName.Add(eachLevelName);

                }

                ViewBag.SyllabusList = syllabusList;
                ViewBag.Language = eachSyllabusLanguage;
                ViewBag.Trade = tradeName;
                ViewBag.Level = levelName;
                ViewBag.SyllabusNameList = syllabusName;


                return View(syllabusList);
            }



            
        }

        public FileResult FileDownload(string filedirectory)
        {
            var vpath = filedirectory;
            return File(vpath, "application/force-download", Path.GetFileName(vpath));
        }

        [HttpGet]

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("SyllabusList");
            }

            else
            {
                Session["SyllabusId"] = id;

                tblSyllabu syllabusDetails =(from p in db.tblSyllabus
                                             where p.colSyllabusId == id select p).
                                             SingleOrDefault();
                var languageList = db.tblSyllabusLanguages.Where(x => x.colSyllabusId == id).Select(x => x.colLanguageId).ToList();

                ViewBag.SyllabusLanguageId = languageList;

                var tradeList = db.tblTrades.ToList();
                ViewBag.Trade = tradeList;
                var levelList = db.tblLevels.ToList();
                ViewBag.Level = levelList;

                var totalLanguageList = db.tblLanguages.ToList();
                ViewBag.Language = totalLanguageList;

                ViewBag.Syllabus = syllabusDetails;

                tblSyllabu testSyl = syllabusDetails;
                return View();
            }

        }

        [HttpPost]

        public ActionResult Edit(tblSyllabu syllabusObject, int[] colLanguageId, HttpPostedFileBase syllabusfile, HttpPostedFileBase testPlanFile)
        {
            if (syllabusfile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(syllabusfile.FileName);
                var guid = Guid.NewGuid().ToString();
                var path = Path.Combine(Server.MapPath("~/uploadSyllabus"), guid + fileName);
                syllabusfile.SaveAs(path);
                string fl = path.Substring(path.LastIndexOf("\\"));
                string[] split = fl.Split('\\');
                string newpath = split[1];
                string sylpath = "~/uploadSyllabus/" + newpath;
                syllabusObject.colSyllabusDocUrl = sylpath;

            }

            if (testPlanFile.ContentLength > 0)
            {

                var fileName = Path.GetFileName(testPlanFile.FileName);
                var guid = Guid.NewGuid().ToString();
                var path = Path.Combine(Server.MapPath("~/uploadTestPlan"), guid + fileName);
                syllabusfile.SaveAs(path);
                string fl = path.Substring(path.LastIndexOf("\\"));
                string[] split = fl.Split('\\');
                string newpath = split[1];
                string testpath = "~/uploadTestPlan/" + newpath;
                syllabusObject.colTestPlanUrl = testpath;


            }


            syllabusObject.colSyllabusId = Convert.ToInt32(Session["SyllabusId"]);
            tblSyllabu syllabusFromDB = (from p in db.tblSyllabus
                                         where p.colSyllabusId == syllabusObject.colSyllabusId
                                         select p).SingleOrDefault();


            syllabusFromDB.colTradeId = syllabusObject.colTradeId;
            syllabusFromDB.colLevelId = syllabusObject.colLevelId;
            syllabusFromDB.colSyllabusName = syllabusObject.colSyllabusName;
            syllabusFromDB.colSyllabusDocUrl = syllabusObject.colSyllabusDocUrl;
            syllabusFromDB.colTestPlanUrl = syllabusObject.colTestPlanUrl;
            syllabusFromDB.colDevelopmentOfficer = syllabusObject.colDevelopmentOfficer;
            syllabusFromDB.colManager = syllabusObject.colManager;
            syllabusFromDB.colActiveDt = syllabusObject.colActiveDt;

            db.SaveChanges();


            List<tblSyllabusLanguage> syllbusLanguageListFromDB = (from p in db.tblSyllabusLanguages
                                                                   where p.colSyllabusId == syllabusObject.colSyllabusId
                                                                   select p).ToList();


            int counter = syllbusLanguageListFromDB.Count;
            for (int i = 0; i < counter; i++)
            {
                var tester = syllbusLanguageListFromDB[i];
                db.tblSyllabusLanguages.Remove(syllbusLanguageListFromDB[i]);
                db.SaveChanges();

            }


            int[] langId = colLanguageId;
            List<tblSyllabusLanguage> syllabusLanguagesList = new List<tblSyllabusLanguage>();
            foreach (var id in langId)
            {
                tblSyllabusLanguage syllabusLanguage = new tblSyllabusLanguage();
                syllabusLanguage.colSyllabusId = syllabusObject.colSyllabusId;
                syllabusLanguage.colLanguageId = id;
                db.tblSyllabusLanguages.Add(syllabusLanguage);
                // syllabusLanguagesList.Add(syllabusLanguage);
                db.SaveChanges();

            }


            return RedirectToAction("SyllabusList", "Syllabus");
        }

    }
}