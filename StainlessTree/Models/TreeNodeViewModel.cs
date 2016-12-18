using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Models;

namespace StainlessTree.Models
{
    public class TreeNodeViewModel
    {
        [Key]
        public int NodeId { get; set; }

        [DisplayName("名称")]
        [Required]
        public string NodeName { get; set; }

        [Required]
        public int Left { get; set; }
        [Required]
        public int Right { get; set; }

        [DisplayName("上级节点")]
        public TreeNodeViewModel ParentNode { get; set; }

        public string ParentId { get; set; }

        public TreeNodeViewModel() { }

        public override string ToString()
        {
            return this.NodeName;
        }

        public TreeNodeViewModel(TreeNode model)
        {
            this.NodeId = model.Node_Id;
            this.NodeName = model.Node_Name;
            this.Left = model.Left;
            this.Right = model.Right;
        }
    }
}