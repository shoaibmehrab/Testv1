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


        #region SyllabusCreate()

        [HttpGet]
        public ActionResult SyllabusCreate()
        {
            var tradeList = db.tblTrades.ToList();
            ViewBag.Trade = tradeList;
            var levelList = db.tblLevels.ToList();
            ViewBag.Level = levelList;

            var languageList = db.tblLanguages.ToList();
            ViewBag.Language = languageList;
            return View();
        }

        [HttpPost]
        public ActionResult SyllabusCreate(tblSyllabu syllabusObject, int[] colLanguageId, HttpPostedFileBase syllabusfile, HttpPostedFileBase testPlanFile)
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

            // Language Choice
            try
            {
                List<tblSyllabu> listSyllabus = db.tblSyllabus.ToList();
                int syllId = listSyllabus.Count + 1;
                syllabusObject.colSyllabusId = syllId;

                int[] langId = colLanguageId;
                List<tblSyllabusLanguage> syllabusLanguagesList = new List<tblSyllabusLanguage>();
                foreach (var id in langId)
                {
                    tblSyllabusLanguage syllabusLanguage = new tblSyllabusLanguage();
                    syllabusLanguage.colSyllabusId = syllId;
                    syllabusLanguage.colLanguageId = id;

                    syllabusLanguagesList.Add(syllabusLanguage);
                }

                db.tblSyllabus.Add(syllabusObject);
                var message = db.SaveChanges();

                foreach (var lang in syllabusLanguagesList)
                {
                    db.tblSyllabusLanguages.Add(lang);
                    var message2 = db.SaveChanges();

                }






            }
            catch (Exception ex)
            {

                Response.Write(ex);
            }


            var tradeList = db.tblTrades.ToList();
            ViewBag.Trade = tradeList;
            var levelList = db.tblLevels.ToList();
            ViewBag.Level = levelList;

            var languageList = db.tblLanguages.ToList();
            ViewBag.Language = languageList;
            return View();

        }


        #endregion

     


        #region SyllabusList()


        [HttpGet]
        public ActionResult SyllabusList()
        {
            Session["SyllabusId"] = null;
            List<tblSyllabu> syllabusList = db.tblSyllabus.ToList();
            List<tblSyllabusLanguage> syllabusLanguage = db.tblSyllabusLanguages.ToList();
            List<tblLanguage> totalLanguages = db.tblLanguages.ToList();
            List<string> eachSyllabusLanguage = new List<string>();
            List<tblTrade> totalTrades = db.tblTrades.ToList();
            List<tblLevel> totalLevels = db.tblLevels.ToList();
            foreach (var syllabus in syllabusList)
            {
                string s = "";

                foreach (var language in syllabusLanguage)
                {
                    if (syllabus.colSyllabusId == language.colSyllabusId)
                    {
                        s = s + (from p in totalLanguages
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
                string eachTradeName =
                    (from p in totalTrades where p.colTradeId == syllabus.colTradeId select p.colTradeName)
                        .SingleOrDefault();

                tradeName.Add(eachTradeName);

            }

            List<string> levelName = new List<string>();


            foreach (var syllabus in syllabusList)
            {
                string eachLevelName =
                    (from p in totalLevels where p.colLevelId == syllabus.colLevelId select p.colLevelName)
                        .SingleOrDefault();

                levelName.Add(eachLevelName);

            }

            ViewBag.SyllabusList = syllabusList;
            ViewBag.Language = eachSyllabusLanguage;
            ViewBag.Trade = tradeName;
            ViewBag.Level = levelName;
            ViewBag.SyllabusNameList = syllabusName;
            ViewBag.TotalTradeList = totalTrades;
            ViewBag.TotalLevelList = totalLevels;
            //List<string> testString = eachSyllabusLanguage;
            //List<string> testName = syllabusName;
            //List<string> testTrade = tradeName;
            //List<string> testLevel = levelName;
            //int a;

            return View(syllabusList);
        }


        [HttpPost]
        public ActionResult SyllabusList(string colTradeId, string colLevelId)
        {

            if (colTradeId == "" && colLevelId == "")
            {
                return RedirectToAction("SyllabusList", "Syllabus");
            }

            //int debugger;
            else if (colTradeId != "" && colLevelId == "")
            {
                int colTdId = Convert.ToInt32(colTradeId);
                List<tblTrade> totalTrades = db.tblTrades.ToList();
                List<tblLevel> totalLevels = db.tblLevels.ToList();
                ViewBag.TotalTradeList = totalTrades;
                ViewBag.TotalLevelList = totalLevels;
                List<tblSyllabu> syllabusList = db.tblSyllabus.Where(x => x.colTradeId == colTdId).ToList();

                List<tblSyllabusLanguage> syllabusLanguage = db.tblSyllabusLanguages.ToList();
                List<tblLanguage> totalLanguages = db.tblLanguages.ToList();
                List<string> eachSyllabusLanguage = new List<string>();

                foreach (var syllabus in syllabusList)
                {
                    string s = "";

                    foreach (var language in syllabusLanguage)
                    {
                        if (syllabus.colSyllabusId == language.colSyllabusId)
                        {
                            s = s + (from p in totalLanguages
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
                    string eachTradeName =
                        (from p in totalTrades where p.colTradeId == syllabus.colTradeId select p.colTradeName)
                            .SingleOrDefault();

                    tradeName.Add(eachTradeName);

                }

                List<string> levelName = new List<string>();


                foreach (var syllabus in syllabusList)
                {
                    string eachLevelName =
                        (from p in totalLevels where p.colLevelId == syllabus.colLevelId select p.colLevelName)
                            .SingleOrDefault();

                    levelName.Add(eachLevelName);

                }

                ViewBag.SyllabusList = syllabusList;
                ViewBag.Language = eachSyllabusLanguage;
                ViewBag.Trade = tradeName;
                ViewBag.Level = levelName;
                ViewBag.SyllabusNameList = syllabusName;

                //int debugger;

                return View(syllabusList);


            }

            else if (colTradeId == "" && colLevelId != "")
            {

                int colLvlId = Convert.ToInt32(colLevelId);
                List<tblTrade> totalTrades = db.tblTrades.ToList();
                List<tblLevel> totalLevels = db.tblLevels.ToList();
                ViewBag.TotalTradeList = totalTrades;
                ViewBag.TotalLevelList = totalLevels;
                List<tblSyllabu> syllabusList = db.tblSyllabus.Where(x => x.colLevelId == colLvlId).ToList();

                List<tblSyllabusLanguage> syllabusLanguage = db.tblSyllabusLanguages.ToList();
                List<tblLanguage> totalLanguages = db.tblLanguages.ToList();
                List<string> eachSyllabusLanguage = new List<string>();

                foreach (var syllabus in syllabusList)
                {
                    string s = "";

                    foreach (var language in syllabusLanguage)
                    {
                        if (syllabus.colSyllabusId == language.colSyllabusId)
                        {
                            s = s + (from p in totalLanguages
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
                    string eachTradeName =
                        (from p in totalTrades where p.colTradeId == syllabus.colTradeId select p.colTradeName)
                            .SingleOrDefault();

                    tradeName.Add(eachTradeName);

                }

                List<string> levelName = new List<string>();


                foreach (var syllabus in syllabusList)
                {
                    string eachLevelName =
                        (from p in totalLevels where p.colLevelId == syllabus.colLevelId select p.colLevelName)
                            .SingleOrDefault();

                    levelName.Add(eachLevelName);

                }



                ViewBag.SyllabusList = syllabusList;
                ViewBag.Language = eachSyllabusLanguage;
                ViewBag.Trade = tradeName;
                ViewBag.Level = levelName;
                ViewBag.SyllabusNameList = syllabusName;

                int debugger;

                return View(syllabusList);


            }

            else
            {


                int colLvlId = Convert.ToInt32(colLevelId);
                int colTdId = Convert.ToInt32(colTradeId);
                List<tblTrade> totalTrades = db.tblTrades.ToList();
                List<tblLevel> totalLevels = db.tblLevels.ToList();
                ViewBag.TotalTradeList = totalTrades;
                ViewBag.TotalLevelList = totalLevels;
                List<tblSyllabu> syllabusList = db.tblSyllabus.Where(x => x.colLevelId == colLvlId && x.colTradeId == colTdId).ToList();

                List<tblSyllabusLanguage> syllabusLanguage = db.tblSyllabusLanguages.ToList();
                List<tblLanguage> totalLanguages = db.tblLanguages.ToList();
                List<string> eachSyllabusLanguage = new List<string>();

                foreach (var syllabus in syllabusList)
                {
                    string s = "";

                    foreach (var language in syllabusLanguage)
                    {
                        if (syllabus.colSyllabusId == language.colSyllabusId)
                        {
                            s = s + (from p in totalLanguages
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
                    string eachTradeName =
                        (from p in totalTrades where p.colTradeId == syllabus.colTradeId select p.colTradeName)
                            .SingleOrDefault();

                    tradeName.Add(eachTradeName);

                }

                List<string> levelName = new List<string>();


                foreach (var syllabus in syllabusList)
                {
                    string eachLevelName =
                        (from p in totalLevels where p.colLevelId == syllabus.colLevelId select p.colLevelName)
                            .SingleOrDefault();

                    levelName.Add(eachLevelName);

                }

                ViewBag.SyllabusList = syllabusList;
                ViewBag.Language = eachSyllabusLanguage;
                ViewBag.Trade = tradeName;
                ViewBag.Level = levelName;
                ViewBag.SyllabusNameList = syllabusName;

                int debugger;

                return View(syllabusList);


                //return View();
            }


        }

        #endregion


        #region Download Uploaded File
        public FileResult Download(string filePath)
        {
            var FileVirtualPath = filePath;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }

        #endregion

        #region Edit

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

                tblSyllabu syllabusDetails =
                    (from p in db.tblSyllabus where p.colSyllabusId == id select p).SingleOrDefault();
                var languageList =
                     db.tblSyllabusLanguages.Where(x => x.colSyllabusId == id).Select(x => x.colLanguageId).ToList();

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
        #endregion



    }
}