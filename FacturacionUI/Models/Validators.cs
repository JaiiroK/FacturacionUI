using System.ComponentModel.DataAnnotations;

namespace FacturacionUI.Models
{
    public class Article
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Description { get; set; } = string.Empty;
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero")]
        public decimal UnitPrice { get; set; }
        public bool IsAvailable { get; set; }
    }
    public class ArticleDto
    {
        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Description { get; set; } = string.Empty;
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor que cero")]
        public decimal UnitPrice { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class Client
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nombre de entidad requerido")]
        public string BusinessName { get; set; }
        [Required(ErrorMessage = "Se requiere un numero de identificacion")]
        [CedulaOrRNC]
        public string IdentificationNumber { get; set; }
        [Required(ErrorMessage = "Se requiere un numero de una cuenta de contabilidad")]
        public string LedgerAccount { get; set; }
        public bool IsActive { get; set; }
    }
    public class ClientDto
    {
        [Required(ErrorMessage = "Se requiere un numero de identificacion")]
        public string BusinessName { get; set; }
        [Required(ErrorMessage = "Se requiere un numero de identificacion")]
        [CedulaOrRNC]
        public  string IdentificationNumber { get; set; }
        [Required(ErrorMessage = "Se requiere un numero de una cuenta de contabilidad")]
        public string LedgerAccount { get; set; }
        public bool IsActive { get; set; }
    }

    public class Seller
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Se requiere un Nombre")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Se requiere un apellido")]
        public string LastName { get; set; }
        [Range(0, 100, ErrorMessage = "De un porcentaje de comision adecuado")]
        public int CommissionPercentage { get; set; }
        public bool IsActive { get; set; }
    }
    public class SellerDto
    {
        [Required(ErrorMessage = "Se requiere un Nombre")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Se requiere un apellido")]
        public string LastName { get; set; }
        [Range(0, 100, ErrorMessage = "De un porcentaje de comision adecuado")]
        public int CommissionPercentage { get; set; }
        public bool IsActive { get; set; }
    }

    public class CedulaOrRNCAttribute : ValidationAttribute
    {
        public CedulaOrRNCAttribute()
        {
            ErrorMessage = "El número de identificación no es válido.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var id = value as string;
            if (IdentificationNumberHelper.IsValidCedulaOrRNC(id))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }
    }
    public static class IdentificationNumberHelper
    {
        public static bool IsValidCedulaOrRNC(string identificationNumber)
        {
            if (string.IsNullOrWhiteSpace(identificationNumber)) return false;
            string cleanId = identificationNumber.Replace("-", "").Replace(" ", "").Trim();
            return IsValidCedula(cleanId) || IsValidRNC(cleanId);
        }

        private static bool IsValidCedula(string cedula)
        {
            if (cedula.Length != 11 || !cedula.All(char.IsDigit)) return false;
            if (cedula.Distinct().Count() == 1) return false;

            int total = 0;
            int[] multipliers = [1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1];

            for (int i = 0; i < 11; i++)
            {
                int digit = int.Parse(cedula[i].ToString());
                int product = digit * multipliers[i];
                total += (product < 10) ? product : (product / 10) + (product % 10);
            }

            return total % 10 == 0;
        }

        private static bool IsValidRNC(string rnc)
        {
            if (rnc.Length != 9 || !rnc.All(char.IsDigit)) return false;
            if (!"145".Contains(rnc[0])) return false;

            int sum = 0;
            int[] multipliers = [7, 9, 8, 6, 5, 4, 3, 2];

            for (int i = 0; i < 8; i++)
            {
                int digito = int.Parse(rnc[i].ToString());
                sum += digito * multipliers[i];
            }

            int modulo = sum % 11;
            int verifier = int.Parse(rnc[8].ToString());

            return (modulo == 0 && verifier == 1) ||
                   (modulo == 1 && verifier == 1) ||
                   ((11 - modulo) == verifier);
        }
    }
}
