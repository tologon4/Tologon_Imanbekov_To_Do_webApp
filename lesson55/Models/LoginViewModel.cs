using System.ComponentModel.DataAnnotations;

namespace lesson55.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Заполните ячейку!")]
    [Display(Name = "Email")]
    public string Email { get; set; }
   
    [Required(ErrorMessage = "Заполните ячейку!")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }
   
    [Display(Name = "Запомнить?")]
    public bool RememberMe { get; set; }
   
    public string? ReturnUrl { get; set; }
}