using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class RequestData
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? fichasDesordenadas {  get; set; }
    }
}
