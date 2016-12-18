using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace Models
{
    public partial class TreeNode
    {
        [Key]
        public int Node_Id { get; set; }

        [DisplayName("节点名称")]
        [Required]
        public string Node_Name { get; set; }

        [Required]
        public int Left { get; set; }

        [Required]
        public int Right { get; set; }
    }

}
