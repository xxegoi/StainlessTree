using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Models;
using System.Data.Entity.Infrastructure;
using System.Reflection;

namespace DAL
{
    public class TreeNodeDAL:IDisposable
    {
        #region 注释不用
        //string conStr = ConfigurationManager.ConnectionStrings["Tree"].ConnectionString;

        //SqlConnection con;

        //public TreeNodeDAL()
        //{
        //    con = new SqlConnection(conStr);

        //    try
        //    {
        //        //检查连接是否成功
        //        con.Open();

        //        con.Close();
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// 调用存储过程插入节点
        ///// </summary>
        ///// <param name="model"></param>
        ///// <param name="nodeName"></param>
        ///// <returns></returns>
        //public int AddNode(TreeNode model,string nodeName)
        //{
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.CommandText = "exec AddNode @nodeId,@nodeName";
        //    cmd.Parameters.Add(new SqlParameter ("@nodeId", model.Node_Id ));
        //    cmd.Parameters.Add(new SqlParameter("@nodeName", nodeName));
        //    cmd.Connection = con;

        //    int result=0;

        //    try
        //    {
        //        con.Open();

        //        result= cmd.ExecuteNonQuery();

        //        con.Close();
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return result;

        //} 
        #endregion

        TreeNodeContext db = new TreeNodeContext();

        #region 增加节点
        public int Add(int parentId, TreeNode model)
        {
            TreeNode parentNode = db.TreeNodes.Where(m => m.Node_Id == parentId).FirstOrDefault();

            TreeNode entry;


            if (parentNode == null)
            {
                #region //如果没有父节点，则这是一个根节点，直接插入到表中

                entry = new TreeNode() { Node_Name = model.Node_Name, Left = 1, Right = 2 };

                db.TreeNodes.Add(entry);
                #endregion
            }
            else
            {
                #region //根据父节点的左右值计算出新节点的左右值，同时修改所有节点的左右值

                int rgt = parentNode.Right;

                IQueryable<TreeNode> nlst = db.TreeNodes.Where(m => m.Right >= rgt);

                foreach (TreeNode item in nlst)
                {
                    item.Right += 2;
                }

                nlst = db.TreeNodes.Where(m => m.Left >= rgt);

                foreach (TreeNode item in nlst)
                {
                    item.Left += 2;
                }

                model.Left = rgt;
                model.Right = rgt + 1;

                db.TreeNodes.Add(model);
                #endregion


            }
            try { return db.SaveChanges(); } catch (Exception ex) { throw ex; }
        }
        #endregion


        #region 删除节点并删除其所有子孙节点
        public int DeleteById(int id)
        {
            #region 取得节点实体，并设置为删除状态
            TreeNode model = db.TreeNodes.Where(m => m.Node_Id == id).FirstOrDefault();
            model.IsDeleted = true;
            #endregion

            #region 获取所有子孙节点，并设置为删除状态
            IQueryable<TreeNode> nlst = db.TreeNodes.Where(m => m.Left > model.Left && m.Right < model.Right && m.IsDeleted == false);

            foreach (TreeNode item in nlst)
            {
                item.IsDeleted = true;
            }
            #endregion

            #region 更新其它节点的左值
            nlst = db.TreeNodes.Where(m => m.Left > model.Left && m.IsDeleted == false);

            foreach (TreeNode item in nlst)
            {
                item.Left = item.Left - (model.Right - model.Left + 1);
            }
            #endregion

            #region 更新其它节点的右值
            nlst = db.TreeNodes.Where(m => m.Right > model.Right && m.IsDeleted == false);

            foreach (TreeNode item in nlst)
            {
                item.Right = item.Right - (model.Right - model.Left + 1);
            } 
            #endregion

            try
            {
                return db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        #endregion


        #region 更新节点
        public int Update(TreeNode model)
        {
            db.TreeNodes.Attach(model);

            DbEntityEntry<TreeNode> entry = db.Entry(model);

            entry.State = System.Data.Entity.EntityState.Modified;

            try
            {
                return db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        #endregion


        #region 获取所有款删除节点
        public List<TreeNode> QueryAllTreeNodes()
        {
            return db.TreeNodes.Where(m=>!m.IsDeleted).ToList<TreeNode>();
        }
        #endregion


        #region 获取所有子节点
        public List<TreeNode> QueryChildrenNodes(int id)
        {
            TreeNode parent = db.TreeNodes.Where(m => m.Node_Id == id).FirstOrDefault();

            int layer = db.TreeNodes.Count(m => m.Left <= parent.Left && m.Right >= parent.Right);

            return db.TreeNodes.Where(m => db.TreeNodes.Count(x => x.Left <= m.Left && x.Right >= m.Right) == layer + 1).ToList();
        } 
        #endregion


        #region 获取所有子孙节点
        public List<TreeNode> QueryAllChildrenNodes(int id)
        {
            TreeNode parent = db.TreeNodes.Where(m => m.Node_Id == id).FirstOrDefault();

            return db.TreeNodes.Where(m => m.Left > parent.Left && m.Right < parent.Right).ToList();
        }
        #endregion


        #region 获取父节点
        public TreeNode QueryParent(TreeNode model)
        {
            return db.TreeNodes.Where(m => m.Left < model.Left && m.Right > model.Right).FirstOrDefault();
        }


        #endregion


        #region 根据ID获取单个节点
        public TreeNode QueryTreeNode(int id)
        {
            return db.TreeNodes.Where(m => m.Node_Id == id).FirstOrDefault();

        } 
        #endregion


        public void Dispose()
        {
            this.db.Dispose();
        }

    }
}
