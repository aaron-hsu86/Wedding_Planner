#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace _7_Wedding_Planner.Models;
public class Event
{
    [Key]
    public int EventId {get;set;}

    [Required]
    [MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
    [Display(Name = "Wedder One")]
    public string WedderOne {get;set;}

    [Required]
    [MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
    [Display(Name = "Wedder Two")]
    public string WedderTwo {get;set;}

    [Required]
    [FutureDate(ErrorMessage = "Please select a future date for the wedding")]
    [Display(Name = "Wedding Date")]
    public DateTime WeddingDate {get;set;}

    [Required]
    public string Address {get;set;}

    // foreign key
    public int UserId {get;set;}
    public User? UserPlanner {get;set;}

    public List<Invite> UserInvites {get;set;} = new List<Invite>();
}
public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        DateTime date;
        if ((value != null && DateTime.TryParse(value.ToString(), out date)))
        {
            // return true if date is set to future date, false if date is in the past
            return DateTime.Now < date;
        }
        // date is null, return false
        return false;
    }
}