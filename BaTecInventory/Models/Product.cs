using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaTecInventory.Models
{
    public class Product 
    {
        [Key]
        [ScaffoldColumn(false)]
        public int ProductId { get; set; }

        //[Required(ErrorMessage = "Select the User or Create the new User from the User Tab")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Producer name is required.")]
        [StringLength(100)]
        [Display(Name = "Producer")]
        public string Producer { get; set; }

        [Required(ErrorMessage = "Model name is required.")]
        [StringLength(100)]
        [Display(Name = "Model")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Supplier name is required.")]
        [Display(Name = "Supplier")]
        public string  Supplier { get; set; }

        [Required(ErrorMessage = "Select category Name is required.")]
        [Display(Name = "Category")]
        public string Category { get; set; }

        [Required(ErrorMessage = "SubCategory Name is required.")]
        [Display(Name = "SubCategory Name")]
        public string SubCategory { get; set; }

        [Required(ErrorMessage = "Serial Number is required.")]
        //[Remote("ProductSerialExist", "Inventory", ErrorMessage = "This Serial Number already Exits in the Product List")]
        public string SerialNumber { get; set; }

        [Required(ErrorMessage = "Receipt Number is required.")]
        public int ReceiptNumber { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DisplayFormat(DateTimeOffset ="%Y %m %d")]
        public DateTime DestructionDate { get; set; }

        [Required(ErrorMessage = "Comments required.")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [Required(ErrorMessage = "History required.")]
        [DataType(DataType.MultilineText)]
        public string History { get; set; }




        public Product( string serialNumber, int receiptNumber, DateTime purchaseDate, DateTime destructionDate, string comments, string history,string userName, string model, string supplier, string subCategory,string producer )
        {
            UserName = userName;
            Producer = producer;
            Model = model;
            Supplier = supplier;
            Category = Category;
            SubCategory = subCategory;
            SerialNumber = serialNumber;
            ReceiptNumber = receiptNumber;
            PurchaseDate = purchaseDate;
            DestructionDate = destructionDate;
            Comments = comments;
            History = history;  
        }

        public Product()
        {
            UserName = UserName;
            Producer = Producer;
            Model = Model;
            Supplier = Supplier;
            Category = Category;
            SubCategory = SubCategory;
            SerialNumber = SerialNumber;
            ReceiptNumber = ReceiptNumber;
            PurchaseDate = PurchaseDate;
            DestructionDate = DestructionDate;
            Comments = Comments;
            History = History;

        }


        
    }
}