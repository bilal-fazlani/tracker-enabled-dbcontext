using System.Linq;
using System.Net;
using System.Web.Mvc;
using SampleLogMaker.Models;

namespace SampleLogMaker.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Comments/
        public ActionResult Index(int? blogId = null)
        {

            var vm = blogId.HasValue? db.Comments.Where(c => c.ParentBlog.Id == blogId).ToList()
                :
                db.Comments.ToList()
            ;
            return View(vm);
        }
         
        // GET: /Comments/Details/5
        public ActionResult Details(int id)
        {
            Comment comment = db.Comments.Find(id);
            return View(comment);
        }

        // GET: /Comments/Create
        public ActionResult Create()
        {
            ViewBag.Blogs = db.Blogs;
            return View();
        }

        // POST: /Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Comment comment)
        {
            ViewBag.Blogs = db.Blogs;
            if (ModelState.Any(x => x.Key == "ParentBlog.Title"))
            {
                ModelState.Remove("ParentBlog.Title");
            }

            if (ModelState.IsValid)
            {
                comment.ParentBlog = db.Blogs.Find(comment.ParentBlog.Id);
                db.Comments.Add(comment);
                db.SaveChanges(User.Identity.Name);

                return RedirectToAction("Index");
            }

            return View(comment);
        }

        // GET: /Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            Comment comment = db.Comments.Find(id);
            ViewBag.Blogs = db.Blogs;
            return View(comment);
        }

        // POST: /Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Comment comment)
        {
            if (ModelState.Any(x => x.Key == "ParentBlog.Title"))
            {
                ModelState.Remove("ParentBlog.Title");
            }

            if (ModelState.IsValid)
            {
                var c = db.Comments.Find(comment.Id);
                var b = db.Blogs.Find(comment.ParentBlog.Id);

                c.ParentBlog = b;
                c.Text = comment.Text;

                db.SaveChanges(User.Identity.Name);
                return RedirectToAction("Index");
            }
            ViewBag.Blogs = db.Blogs;
            return View(comment);
        }

        // GET: /Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: /Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges(User.Identity.Name);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
