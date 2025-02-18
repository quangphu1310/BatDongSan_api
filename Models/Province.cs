using System.ComponentModel.DataAnnotations;

namespace BatDongSan_api.Models
{
    public class Province
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}
