using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Models;
using StainlessTree.Models;
using System.Net;
using StainlessTree.Models.JsonModel;

namespace StainlessTree.Controllers
{
    public partial class TreeController : Controller
    {
        TreeNodeBLL db = new TreeNodeBLL();
        // GET: Tree
        public ActionResult Index()
        {
            List<TreeNode> nlst = db.QueryAllTreeNodes();

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

        public ActionResult CreateChilNode(int id)
        {

            TreeNodeViewModel model = new TreeNodeViewModel();
            model.ParentId = id;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateChilNode([Bind(Include ="NodeName,ParentId")]TreeNodeViewModel model)
        {
            if (ModelState.IsValid)
            {
                TreeNode parent = db.QueryTreeNode(model.ParentId);

                if (parent != null)
                {
                    try
                    {
                        TreeNode entry = model.ToEntry();

                        db.Add(parent.Node_Id, entry);

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
                TreeNode entry = model.ToEntry();

                db.Add(parentId, entry);

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

            TreeNode node = db.QueryTreeNode((int)id);

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
            TreeNode model = db.QueryTreeNode((int)id);

            if (model != null)
            {
                db.DeleteById((int)id);
            }

            return RedirectToAction("Index");
        }


        public ActionResult Edit(int id)
        {
            TreeNodeViewModel model =new TreeNodeViewModel( db.QueryTreeNode(id));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TreeNodeViewModel model)
        {
            if (ModelState.IsValid)
            {
                TreeNode entry = db.QueryTreeNode(model.NodeId);

                entry.Node_Name = model.NodeName;

                db.Update(entry);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public JsonResult GetChildrenTreeNodes(int? id)
        {
            List<TreeNodeJson> jlst = new List<TreeNodeJson>();
            List<TreeNode> nlst;

            if (id != null)
            {
                nlst = db.QueryChildrenNodes((int)id);
            }
            else
            {
                nlst = db.QueryAllTreeNodes();
            }

            nlst.ForEach(m => jlst.Add(GetTreeNodeJson(m)));

            return Json(jlst, JsonRequestBehavior.AllowGet);
        }

        private TreeNodeJson GetTreeNodeJson(TreeNode node)
        {
            TreeNodeJson result = new TreeNodeJson(node);

            List<TreeNode> nlst = db.QueryChildrenNodes(result.id);

            nlst.ForEach(m => result.children.Add(new TreeNodeJson(m)));

            return result;
        }

        public ActionResult MenuTree()
        {

            return View();
        }
    }
}