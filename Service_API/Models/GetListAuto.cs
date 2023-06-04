using System.ComponentModel.DataAnnotations;

namespace Service_API.Models
{
    public class GetListAuto
    {
        public int Id { get; set; }

        [Required]  // указывает что поле является обязательным для заполнения
        public string Name { get; set; }

        [Required] // указывает что поле является обязательным для заполнения
        public int Price { get; set; }
    }
}
