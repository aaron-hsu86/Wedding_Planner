#pragma warning disable CS8618
namespace _7_Wedding_Planner.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[NotMapped]
public class LoginUser
{
    [Required]
    [Display(Name = "Email")]
    public string EmailCheck {get; set;}
    [Required]
    [Display(Name = "Password")]
    public string PasswordCheck {get; set;}
}