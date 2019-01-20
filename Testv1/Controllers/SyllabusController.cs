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

            var LevelList = db.tblTrades.ToList();
            ViewBag.Level = LevelList;

            var LangList = db.tblTrades.ToList();
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
    }
}