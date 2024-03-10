using System.ComponentModel.DataAnnotations;

namespace Domain.FichaDomino
{
    public class FichaDominoEntity
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Range(1, 6, ErrorMessage = "El campo {0} debe ser un número entre {1} y {2}.")]
        public int Izquierda { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Range(1, 6, ErrorMessage = "El campo {0} debe ser un número entre {1} y {2}.")]
        public int Derecha { get; set; }

    }
}
