using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Models;

namespace DAL
{
    public class TreeNodeDAL
    {
        string conStr = ConfigurationManager.ConnectionStrings["Tree"].ConnectionString;

        SqlConnection con;

        public TreeNodeDAL()
        {
            con = new SqlConnection(conStr);

            try
            {
                //检查连接是否成功
                con.Open();

                con.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 调用存储过程插入节点
        /// </summary>
        /// <param name="model"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public int AddNode(TreeNode model,string nodeName)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "exec AddNode @nodeId,@nodeName";
            cmd.Parameters.Add(new SqlParameter ("@nodeId", model.Node_Id ));
            cmd.Parameters.Add(new SqlParameter("@nodeName", nodeName));
            cmd.Connection = con;

            int result=0;

            try
            {
                con.Open();

                result= cmd.ExecuteNonQuery();

                con.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;

        }
    }
}
