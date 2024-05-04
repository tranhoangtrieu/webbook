using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebsiteBook.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Code { get; set; }
        public int Amount { get; set; }
        public decimal Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CategoryId { get; set; }

        public bool IsActive
        {
            get { return DateTime.Now >= StartDate && DateTime.Now <= EndDate; }
        }

        // Navigation property
        public Category? Category { get; set; }
    }
}
