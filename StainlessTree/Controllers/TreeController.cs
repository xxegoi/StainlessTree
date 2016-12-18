using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Models;
using StainlessTree.Models;
using System.Net;

namespace StainlessTree.Controllers
{
    public class TreeController : Controller
    {
        TreeNodeBLL db = new TreeNodeBLL();
        // GET: Tree
        public ActionResult Index()
        {
            List<TreeNode> nlst = db.GetTreeNodes();

            List<TreeNodeViewModel> viewList = new List<TreeNodeViewModel>();

            nlst.ForEach(m =>
            {

                TreeNodeViewModel newModel = new TreeNodeViewModel(m);

                TreeNode parent=null;

                foreach(TreeNode item in nlst)
                {
                    if (item.Left < newModel.Left && item.Right > newModel.Right)
                    {
                        newModel.ParentNode = new TreeNodeViewModel(item);
                    }
                }

                if (parent != null)
                {
                    newModel.ParentNode = new TreeNodeViewModel( parent );
                }

                viewList.Add(newModel);
            });

            return View(viewList);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult CreateChilNode(string id)
        {

            TreeNodeViewModel model = new TreeNodeViewModel();
            model.ParentId = id;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateChilNode(TreeNodeViewModel model)
        {
            if (ModelState.IsValid)
            {
                TreeNode parent = db.GetTreeNodes().Find(m => m.Node_Id.ToString() == model.ParentId);

                if (parent != null)
                {
                    try
                    {

                        db.AddNode(model.NodeName, parent);

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NodeName,Left,Right")]TreeNodeViewModel model, int parentId = -1)
        {
            if (ModelState.IsValid)
            {
                if (parentId == -1)
                {
                    db.AddNode(model.NodeName);
                }
                else
                {
                    TreeNode parent = db.GetTreeNodes().Find(m => m.Node_Id == parentId);

                    db.AddNode(model.NodeName, parent);
                }

                return RedirectToAction("Index");
            }

            return View(model);

        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TreeNode node = db.GetById(id);

            if (node == null)
            {
                return HttpNotFound();
            }



            return View(new TreeNodeViewModel(node));
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            TreeNode model = db.GetById(id);

            if (model != null)
            {
                db.DeleteNode(model);
            }

            return RedirectToAction("Index");
        }
    }
}