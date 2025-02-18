using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BatDongSan_api.Models.DTO
{
    public class PropertyCreateDTO
    {
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

        //[ForeignKey("DistrictId")]
        //public District District { get; set; }
    }
}
