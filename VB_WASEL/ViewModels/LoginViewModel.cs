using System.ComponentModel.DataAnnotations;
namespace Wasel.ViewModels.AuthVMs
{
    #region Login

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }

    #endregion

    #region Register

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string PasswordConfirmation { get; set; }

        [Required(ErrorMessage = "Please select user type")]
        [Display(Name = "I want to register as")]
        public string UserType { get; set; } // buyer, seller, both

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone must be 10 digits")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone must be 10 digits")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        // Buyer specific fields
        [RequiredIf("UserType", new[] { "buyer", "both" }, ErrorMessage = "Buyer name is required")]
        [StringLength(100, ErrorMessage = "Buyer name cannot exceed 100 characters")]
        [Display(Name = "Buyer Name")]
        public string BuyerName { get; set; }

        [RequiredIf("UserType", new[] { "buyer", "both" }, ErrorMessage = "Contact phone is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Contact phone must be 10 digits")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Contact phone must be 10 digits")]
        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; }

        [RequiredIf("UserType", new[] { "buyer", "both" }, ErrorMessage = "Buyer city is required")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        [Display(Name = "Buyer City")]
        public string BuyerCity { get; set; }

        [RequiredIf("UserType", new[] { "buyer", "both" }, ErrorMessage = "Buyer address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        [Display(Name = "Buyer Address")]
        public string BuyerAddress { get; set; }

        // Seller specific fields
        [RequiredIf("UserType", new[] { "seller", "both" }, ErrorMessage = "Seller name is required")]
        [StringLength(100, ErrorMessage = "Seller name cannot exceed 100 characters")]
        [Display(Name = "Seller Name")]
        public string SellerName { get; set; }

        [RequiredIf("UserType", new[] { "seller", "both" }, ErrorMessage = "Seller phone is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Seller phone must be 10 digits")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Seller phone must be 10 digits")]
        [Display(Name = "Seller Phone")]
        public string SellerPhone { get; set; }

        [RequiredIf("UserType", new[] { "seller", "both" }, ErrorMessage = "Seller city is required")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        [Display(Name = "Seller City")]
        public string SellerCity { get; set; }

        [RequiredIf("UserType", new[] { "seller", "both" }, ErrorMessage = "Seller address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        [Display(Name = "Seller Address")]
        public string SellerAddress { get; set; }
    }

    #endregion

    #region Profile

    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        public string UserType { get; set; }

        // Buyer fields
        [RequiredIf("UserType", new[] { "buyer", "both" }, ErrorMessage = "Buyer name is required")]
        [StringLength(100)]
        [Display(Name = "Buyer Name")]
        public string BuyerName { get; set; }

        [RequiredIf("UserType", new[] { "buyer", "both" }, ErrorMessage = "Contact phone is required")]
        [StringLength(20)]
        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; }

        [RequiredIf("UserType", new[] { "buyer", "both" }, ErrorMessage = "City is required")]
        [StringLength(100)]
        [Display(Name = "Buyer City")]
        public string BuyerCity { get; set; }

        [RequiredIf("UserType", new[] { "buyer", "both" }, ErrorMessage = "Address is required")]
        [StringLength(200)]
        [Display(Name = "Buyer Address")]
        public string BuyerAddress { get; set; }

        // Seller fields
        [RequiredIf("UserType", new[] { "seller", "both" }, ErrorMessage = "Seller name is required")]
        [StringLength(100)]
        [Display(Name = "Seller Name")]
        public string SellerName { get; set; }

        [RequiredIf("UserType", new[] { "seller", "both" }, ErrorMessage = "Seller phone is required")]
        [StringLength(20)]
        [Display(Name = "Seller Phone")]
        public string SellerPhone { get; set; }

        [RequiredIf("UserType", new[] { "seller", "both" }, ErrorMessage = "City is required")]
        [StringLength(100)]
        [Display(Name = "Seller City")]
        public string SellerCity { get; set; }

        [RequiredIf("UserType", new[] { "seller", "both" }, ErrorMessage = "Address is required")]
        [StringLength(200)]
        [Display(Name = "Seller Address")]
        public string SellerAddress { get; set; }
    }

    #endregion

    #region Dashboard Selector

    public class DashboardSelectorViewModel
    {
        public string UserName { get; set; }
        public int BuyerTendersCount { get; set; }
        public int SellerBidsCount { get; set; }
    }

    #endregion

    /// <summary>
    /// Validates that a property is required when another property has specific values
    /// </summary>
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _dependentProperty;
        private readonly string[] _targetValues;

        public RequiredIfAttribute(string dependentProperty, string[] targetValues, string errorMessage = null)
        {
            _dependentProperty = dependentProperty;
            _targetValues = targetValues;
            ErrorMessage = errorMessage ?? $"{{0}} is required when {dependentProperty} is {string.Join(" or ", targetValues)}.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dependentProperty = validationContext.ObjectType.GetProperty(_dependentProperty);
            if (dependentProperty == null)
            {
                return new ValidationResult($"Unknown property: {_dependentProperty}");
            }

            var dependentValue = dependentProperty.GetValue(validationContext.ObjectInstance)?.ToString();

            // Check if the dependent property has one of the target values
            if (_targetValues.Any(tv => string.Equals(tv, dependentValue, System.StringComparison.OrdinalIgnoreCase)))
            {
                // If it does, validate that this property is not empty
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }
    }

}
