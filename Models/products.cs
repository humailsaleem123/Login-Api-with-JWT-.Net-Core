using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Login.Models
{
    public class products
    {

        [Key]
        public int ID { get; set; }

        [Required]
        public string prod_category { get; set; }


        [Required]
        public string prod_name { get; set; }


        public string prod_detail { get; set; }


        public string prod_imageName { get; set; }

        [NotMapped]
        public IFormFile prod_image { get; set; }

        [NotMapped]

        public string prod_imageSrc { get; set; }




    }
}
