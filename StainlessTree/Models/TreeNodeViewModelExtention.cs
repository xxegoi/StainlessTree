using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace StainlessTree.Models
{
    public partial class TreeNodeViewModel
    {
        #region 扩展辅助属性
        [DisplayName("上级节点")]
        public TreeNodeViewModel ParentNode { get; set; }
        /// <summary>
        /// 上级ID
        /// </summary>
        public int ParentId { get; set; }
        #endregion

        public override string ToString()
        {
            return this.NodeName;
        }

        #region 构造函数
        public TreeNodeViewModel() { }

        public TreeNodeViewModel(TreeNode model)
        {
            this.NodeId = model.Node_Id;
            this.NodeName = model.Node_Name;
            this.Left = model.Left;
            this.Right = model.Right;
            this.IsDeleted = model.IsDeleted;
        }
        #endregion

        #region 转换为实体对象
        public TreeNode ToEntry()
        {
            TreeNode entry = new TreeNode()
            {
                Node_Id = this.NodeId,
                Node_Name = this.NodeName,
                Left = this.Left,
                Right = this.Right,
                IsDeleted = this.IsDeleted
            };

            return entry;
        } 
        #endregion
    }
}