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

        #region 添加根节点
        public ActionResult Create()
        {
            return View();
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
        #endregion

        #region 添加子节点
        public ActionResult CreateChilNode(int id)
        {

            TreeNodeViewModel model = new TreeNodeViewModel();
            model.ParentId = id;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateChilNode([Bind(Include = "NodeName,ParentId")]TreeNodeViewModel model)
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
        #endregion

        #region 删除节点及其所有后代节点
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

        [HttpPost, ActionName("Delete")]
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
        #endregion

        #region 修改节点名称
        public ActionResult Edit(int id)
        {
            TreeNodeViewModel model = new TreeNodeViewModel(db.QueryTreeNode(id));

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
        #endregion

        #region 获取节点及所有后代节点
        public JsonResult GetTree(int? id)
        {
            #region 注释不用
            //List<TreeNodeJson> jlst = new List<TreeNodeJson>();
            //List<TreeNode> nlst;

            //if (id != null)
            //{
            //    nlst = db.QueryChildrenNodes((int)id);
            //}
            //else
            //{
            //    nlst = db.QueryAllTreeNodes();
            //}

            //nlst.ForEach(m => {

            //    bool flag = false;

            //    jlst.ForEach(j =>
            //    {

            //        if (j.children.Count(k => k.id == m.Node_Id) > 0)
            //        {
            //            flag = true;
            //        }

            //    });
            //    if (!flag)
            //    {

            //        jlst.Add(GetTreeNodeJson(m));

            //    }

            //}); 
            #endregion
            List<TreeNode> nlst = db.QueryAllTreeNodes();
            //如果传入ID值为空，测取根节点
            TreeNode root;
            if (id != null)
            {
                root = nlst.FirstOrDefault(m => m.Node_Id == id);
            }
            else
            {
                return GetLevel1Tree();
            }

            var clist = db.QueryChildrenNode(root);

            List<TreeNodeJson> tree = null;

            if (clist.Count > 0)
            {
                tree = new List<TreeNodeJson>();

                foreach(TreeNode item in clist)
                {
                    tree.Add(new TreeNodeJson(item));
                }
            }


            return Json(tree, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 递归所有子节点，并转为Json格式
        private TreeNodeJson GetJsonTree(TreeNode entry)
        {
            TreeNodeJson result = new TreeNodeJson(entry);

            int lft = entry.Left + 1;
            int rgt = entry.Right - 1;
            //取得当前节点下所有子孙节点
            var clist = db.GetTreeNodesByCodition(m => m.Left >= lft && m.Right <= rgt && !m.IsDeleted);

            
            if (clist.Count > 0)
            {
                result.children = new List<TreeNodeJson>();

                foreach (var item in clist)
                {
                    //判断该节点是否是直接下属子节点，是则添加到子节点列表中，孙节点不添加
                    if (item.Left == lft)
                    {
                        result.children.Add(GetJsonTree(item));
                        lft = item.Right + 1;
                    }

                }
            }

            return result;
        } 
        #endregion

        public JsonResult GetLevel1Tree()
        {
            List<TreeNode> nlst = db.QueryAllTreeNodes();

            TreeNode root= nlst.FirstOrDefault(m => m.Right == nlst.Max(n => n.Right) && !m.IsDeleted);
            TreeNodeJson jroot = new TreeNodeJson(root);

            var clist = db.QueryChildrenNode(root);

            if (clist.Count > 0)
            {
                jroot.children = new List<TreeNodeJson>();
                foreach(TreeNode item in clist)
                {
                    jroot.children.Add(new TreeNodeJson(item));
                }
            }

            List<TreeNodeJson> tree = new List<TreeNodeJson>();
            tree.Add(jroot);

            return Json(tree, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MenuTree(int? id)
        {
            if (id != null)
            {
                ViewBag.NodeId = id;
            }
            return View();
        }
    }
}