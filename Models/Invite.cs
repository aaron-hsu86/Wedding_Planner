#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace _7_Wedding_Planner.Models;
public class Invite
{
    [Key]
    public int InviteId {get;set;}

    [Required]
    public int UserId {get;set;}

    [Required]
    public int EventId {get;set;}

    public User? InvitedUser {get;set;}

    public Event? EventRSVP {get;set;}
}