using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BatDongSan_api.Models
{
    public class Property
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Square { get; set; }

        public int? Rooms { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }

        public int? DistrictId { get; set; }

        [ForeignKey("DistrictId")]
        public District District { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PostedDate { get; set; }
        [ValidateNever]
        public List<PropertyImage> PropertyImages { get; set; }
    }
}
