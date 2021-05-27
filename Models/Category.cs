using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManagementSystem.Models
{
   
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        [Required (ErrorMessage = "Category Name is required")]
        [StringLength(40, MinimumLength = 3)]
        public string CategoryName { get; set; }
    }
}