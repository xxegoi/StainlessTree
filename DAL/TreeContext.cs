using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using System.Data.Entity;
using System.Configuration;

namespace DAL
{
    public partial class TreeContext:DbContext
    {


        public TreeContext() : base("Tree") {
        }

        public DbSet<Models.TreeNode> TreeNodes { get; set; }
    }
}
