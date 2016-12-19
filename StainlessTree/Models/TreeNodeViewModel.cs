using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Models;

namespace StainlessTree.Models
{
    public partial class TreeNodeViewModel
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

        public bool IsDeleted { get; set; }

        
    }
}