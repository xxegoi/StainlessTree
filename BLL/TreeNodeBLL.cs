using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Models;

namespace BLL
{
    public class TreeNodeBLL : IDisposable
    {
        TreeContext db = new TreeContext();

        #region 插入子节点
        public bool AddNode(string nodeName, TreeNode model = null)
        {
            if (model == null)
            {
                //如果没有父节点，则这是一个根节点，直接插入到表中
                TreeNode newmodel = new TreeNode() { Node_Name = nodeName, Left = 1, Right = 2 };

                db.TreeNodes.Add(newmodel);

                try { db.SaveChanges();return true; } catch (Exception ex) { throw ex; }

            }
            else
            {
                //根据父节点的左右值计算出新节点的左右值，同时修改所有节点的左右值
                TreeNode parent = db.TreeNodes.Where(m => m.Node_Id == model.Node_Id).First();

                int rgt = parent.Right;

                if (parent != null)
                {
                    IQueryable<TreeNode> nlst = db.TreeNodes.Where(m => m.Right >= rgt);

                    foreach(TreeNode item in nlst)
                    {
                        item.Right += 2;
                    }

                    nlst = db.TreeNodes.Where(m => m.Left >= rgt);

                    foreach(TreeNode item in nlst)
                    {
                        item.Left += 2;
                    }

                    TreeNode newNode = new TreeNode { Node_Name = nodeName, Left = rgt, Right = rgt + 1 };

                    db.TreeNodes.Add(newNode);

                    try { db.SaveChanges(); return true; }catch(Exception ex) { throw ex; }
                }
            }




            return false;
        }
        #endregion

        #region 删除节点和该节点的所有子节点
        public bool DeleteNode(TreeNode model)
        {
            if (model != null)
            {

                try
                {
                    db.TreeNodes.RemoveRange(db.TreeNodes.Where(m => m.Left >= model.Left && m.Right <= model.Right));

                    db.SaveChanges();
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return false;
        } 
        #endregion

        public bool ModiflyNode(TreeNode model)
        {
            db.TreeNodes.Attach(model);

            try
            {
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Dispose()
        {
            this.db.Dispose();
        }
        

        public List<TreeNode> GetTreeNodes(TreeNode model = null)
        {
            if (model != null)
            {
                return db.TreeNodes.Where(m => m.Left > model.Left && m.Right < model.Right).ToList<TreeNode>();
            }
            else
            {
                return db.TreeNodes.ToList<TreeNode>();
            }
        }

        public TreeNode GetById(int? id)
        {
            return db.TreeNodes.Find(id);
        }
    }
}
