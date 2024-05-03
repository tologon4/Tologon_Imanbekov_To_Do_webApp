using System.ComponentModel.DataAnnotations;

namespace lesson55.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Заполните ячейку!")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Заполните ячейку!")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Заполните ячейку!")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Заполните ячейку!")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    [Display(Name = "Подтвердить пароль")]
    public string PasswordConfirm { get; set; }
}