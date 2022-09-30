using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using WebApplication2.util;
using WebApplication2.Repository;
using WebApplication2.unitofwork;
using System.IO;
using MvcPaging;
using System.Configuration;
using Newtonsoft.Json;
using System.Drawing;

namespace WebApplication2.Controllers
{     // GET: Admin
    public class AdminController : Controller
    {
        private IUtilRepo _util;
        private int sliderwidth = Convert.ToInt32(ConfigurationManager.AppSettings["width"]);
        private int sliderheigth = Convert.ToInt32(ConfigurationManager.AppSettings["height"]);
        private int defaultPagesize = Convert.ToInt32(ConfigurationManager.AppSettings["pgSize"]);
        
        private UnitofWork utw;
        public AdminController()
        {
            this._util = new UtilRepository();
            this.utw = new UnitofWork();
        }
        // GET: Admin
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }
        public JsonResult userexist(pagemodel page)
        {
            List<pagemodel> obj = new List<pagemodel>();
            if (page.pageid == 0)
            {
                obj = utw.prepo.Get(filter: e => e.pagename == page.pagename && e.isdelete == false && e.isactive == true).ToList();
            }
            else if (page.pageid > 0)
            {
                obj = utw.prepo.Get(filter: e => e.pagename == page.pagename && e.pageid == page.pageid && e.isdelete == false && e.isactive == true).ToList();
            }
            if (obj.Count > 0)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);

            return Json(new { stat = "true" });
        }
        
        public ActionResult Login()
        {
            try { }
            catch { }
            return View();
        }
        [HttpPost]
        
        public ActionResult Login(adminmodel use)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    webtbluser admin = new webtbluser();
                    admin = utw.repo.Get(f => f.username == use.username && f.userpassword == use.password&& f.is_active == true && f.is_delete == false).FirstOrDefault();
                    if (admin != null)
                    {
                        Session["user"] = use.username;
                        return RedirectToAction("slider");
                    }
                }
            }
            catch { }
            return View();
        }




        [HttpGet]
        public ActionResult slider(string slidername, int? page)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            IList<slider> slid = new List<slider>();

            try
            {
                ViewData["slidername"] = slidername;
                int currentpage = page.HasValue ? page.Value : 0;
                slid = utw.srepo.Get(p => p.isdelete == false, p => p.OrderBy(st => st.imageorder)).ToList();
                if (!string.IsNullOrWhiteSpace(slidername))
                    slid = slid.Where(p => p.slidername.ToLower().Contains(slidername.ToLower())).ToList();
                slid = (IList<slider>)slid.ToPagedList(currentpage,defaultPagesize);

                if (Request.IsAjaxRequest())
                    return PartialView("_AjaxSlider", slid);
                else
                    return View(slid);

            }
            catch (Exception)
            {
            }
            return View(slid);
        }
        public ActionResult Logout()
        {
            try
            {
                if (Session["user"] != null)
                {
                    Session.Clear();
                }
                return RedirectToAction("Login");
            }
            catch
            {
            }
            return View();
        }



        public ActionResult bmslider(long? id)
       {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            long ids = id.HasValue ? id.Value : 0;
            slider slid = new slider();
            try
            {
                if (ids > 0)
                    slid = utw.srepo.GetByID(ids);                
                return View(slid);
            }
            catch (Exception)
            {
            }
            return View(slid);
        }
        [HttpPost]
        public ActionResult bmslider(slider obj, HttpPostedFileBase slidimg)
        {
            if (Session["user"] == null)
                return RedirectToAction("Login");
            slider slid = new slider();
            try
            {
                if (slidimg == null && obj.sliderimg == null)
                {
                    ViewBag.ErMsg = "Please upload the image";
                    return RedirectToAction("slider", "Admin", new { id = 0 });
                }
                if (ModelState.IsValid)
                {
                    slid = obj;
                    System.Drawing.Image sliimg = null;
                    string filename = "";
                    string filepath = "";
                    if (slidimg != null)
                    {
                        string Extension = slidimg.FileName.Remove(0, slidimg.FileName.LastIndexOf('.'));
                        if (Extension != "")
                        {
                            if (Extension.ToLower() == ".jpg" || Extension.ToLower() == ".jpeg" || Extension.ToLower() == ".gif" || Extension.ToLower() == ".png")
                            {
                                sliimg = _util.resizeimage(slidimg.InputStream, sliderwidth, sliderheigth);
                                filename = Guid.NewGuid().ToString() + Extension.ToLower();
                                filepath = Server.MapPath("~/upload/Banners/") + filename;
                                slid.sliderimg = filename;
                            }
                            else
                            {
                                ViewBag.Ermsg = "only jpg format upload";
                                return View(obj);

                            }


                        }

                    }
                    if (obj.id == 0)
                    {
                        obj.created = DateTime.Now;
                        utw.srepo.Insert(obj);
                    }
                    else
                    {
                        obj.deleted = DateTime.Now;
                        utw.srepo.Update(obj);
                    }

                    if (sliimg != null)
                    {
                        sliimg.Save(filepath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    utw.Save();
                    TempData["sucess_msg"] = "slider details";
                    return RedirectToAction("slider", "Admin");

                }
            }
            catch
            {
            }
            return View(obj);
        }
        private bool DeleteFile(string path)
        {
            bool result = true;
            string filepath = Path.Combine(Server.MapPath("~" + path));
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
            return result;
        }
        public ActionResult Delete_slider(int id)
        {
            try
            {
                if (id > 0)
                {
                    slider obj = new slider();
                    obj = utw.srepo.GetByID(id);
                    if (obj != null)
                    {
                        slider sobj = new slider();
                        sobj = obj;
                        sobj.isactive = false;
                        sobj.isdelete = true;
                        utw.srepo.Update(sobj);
                        utw.Save();
                        string fileurl = "/Upload/" + obj.sliderimg;
                        bool flag = DeleteFile(fileurl);
                        return Json(new { Status = "true" });
                    }
                }
            }
            catch
            {
            }
            return null;
        }
    }
}