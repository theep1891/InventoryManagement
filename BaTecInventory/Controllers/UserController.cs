using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaTecInventory.DB;
using BaTecInventory.Models;

namespace BaTecInventory.Controllers
{
    public class UserController : Controller
    {
        private readonly DBConnect _db = new DBConnect();

        //private List<User> userlist = null;

        //public UserController()
        //{
        //    userlist = _db.Users();
        //}


        // GET: User

        public ActionResult Index()
        {
            var user = _db.Users();

            ViewData["User"] = user;
            if (user == null)
                return HttpNotFound();
            return View(user);
        }


        //public ActionResult GetSearchUser() 
        //{

        //    string query = "SELECT username FROM inventory.user";

        //    List<ItemName> theListThatareReturningToTheView = new List<ItemName>();

        //    List<string> values = _db.AllList(query);

        //    foreach (string val in values)
        //    {
        //        ItemName temperoryInstansOfTheValueFromTheForeachLoop = new ItemName
        //        {
        //            GetName = val
        //        };
        //        theListThatareReturningToTheView.Add(temperoryInstansOfTheValueFromTheForeachLoop);
        //    }

        //    return new JsonResult { Data = theListThatareReturningToTheView, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}
        //public ActionResult GetSearchUser(string searchusername)
        //{
        //    List<User> searchuserlist = userlist.Where(x => x.UserName.Contains(searchusername)).Select(x => new User
        //    {
        //        UserId = x.UserId,
        //        UserName = x.UserName
        //    }).ToList();

        //    return new JsonResult { Data = searchuserlist, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

        // GET: User/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var user = _db.Users().Find(m => m.UserId == id);
            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (UserNameExist(user.UserName))
            {
                var query = "SELECT count(UserId) FROM inventory.user WHERE UserName = ('" +
                            user.UserName + "') LIMIT 1 ";
                _db.DataValidation(query);
                ModelState.AddModelError(string.Empty, " User Name already exists.");
            }

            if (ModelState.IsValid)
            {
                var query = "INSERT INTO  inventory.User (username, password, useremail, userphonenumber) VALUES ('" + user.UserName + "', '" + user.Password + "','" + user.UserEmail + "','" + user.UserPhoneNumber + "') ";
                _db.Create(query);

                return RedirectToAction("Index");
            }
            return View(user);

        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var user = _db.Users().Find(m => m.UserId == id);

            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, User user)
        {
            var usid = _db.Users().Find(i => i.UserId == id);

            if (id != usid.UserId)
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var query = "UPDATE inventory.user Set username = '" + user.UserName + "', password = '" + user.Password + "', useremail = '" + user.UserEmail + "', userphonenumber = '" + user.UserPhoneNumber + "' Where userid = ('" + usid.UserId + "')";
                    _db.Edit(query);
                }
                catch (Exception)
                {
                    if (!UserNameExist(user.UserName))
                        return HttpNotFound();
                    throw;
                }

                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var user = _db.Users().Find(m => m.UserId == id);

            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, User user)
        {
            {
                user = _db.Users().Find(m => m.UserId == id);
                var query = "DELETE  FROM inventory.user Where userid = ('" + user.UserId + "')";
                _db.Delete(query);
                return RedirectToAction("Index");
            }
        }

        
        private bool UserNameExist(string name)
        {
            return _db.Users().Any(e => e.UserName == name);
        }
    }
}
