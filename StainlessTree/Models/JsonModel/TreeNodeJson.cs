using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StainlessTree.Models.JsonModel
{
    public class TreeNodeJson
    {
        public int id { get; set; }
        public string text { get; set; }
        public string state { get; set; }
        public List<TreeNodeJson> children { get; set; }

        public TreeNodeJson() { }

        public TreeNodeJson(TreeNode entry)
        {
            this.id = entry.Node_Id;
            this.text = entry.Node_Name;
            this.state = "open";
            this.children = new List<TreeNodeJson>();
        }
    }
}