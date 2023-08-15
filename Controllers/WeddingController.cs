using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _7_Wedding_Planner.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace _7_Wedding_Planner.Controllers;

[SessionCheck]
public class WeddingController : Controller
{
    private readonly ILogger<WeddingController> _logger;
    public MyContext db;

    public WeddingController(ILogger<WeddingController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }

    [HttpGet("weddings")]
    public IActionResult Index()
    {
        List<Event> allEvents = db.Events.Include(e => e.UserInvites).ToList();
        return View(allEvents);
    }

    [HttpGet("weddings/new")]
    public IActionResult New()
    {
        return View("PlanWedding");
    }

    [HttpPost("wedding/create")]
    public IActionResult Create(Event newEvent)
    {
        // check it can't be between two users
        if (!ModelState.IsValid)
        {
            return View("PlanWedding");
        }
        else
        {
            newEvent.UserId = (int) HttpContext.Session.GetInt32("UserId");
            db.Events.Add(newEvent);
            db.SaveChanges();
            return RedirectToAction("ViewOne", new {eventId = newEvent.EventId});
        }
    }

    [HttpGet("weddings/{eventId}")]
    public IActionResult ViewOne(int eventId)
    {
        Event? oneEvent = db.Events.Include(e => e.UserInvites).ThenInclude(i => i.InvitedUser).FirstOrDefault(e => e.EventId == eventId);
        if(oneEvent != null)
        {
            return View("ViewOne" , oneEvent);
        }
        else
        {
            return RedirectToAction("Index");
        }
    }

    [HttpPost("wedding/delete/{eventId}")]
    public IActionResult Delete(int eventId)
    {
        Event? deleteEvent = db.Events.SingleOrDefault(e => e.EventId == eventId);
        db.Events.Remove(deleteEvent);
        db.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpPost("wedding/{eventId}/rsvp")]
    public IActionResult RSVP(int eventId)
    {
        if(!ModelState.IsValid)
        {
            return View("Index");
        }
        else
        {
            Invite? inviteCheck = db.Invites.FirstOrDefault(i => i.EventId == eventId && i.UserId == HttpContext.Session.GetInt32("UserId"));
            if (inviteCheck != null)
            {
                db.Invites.Remove(inviteCheck);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                Invite newInvite = new Invite()
                {
                    UserId = (int) HttpContext.Session.GetInt32("UserId"),
                    EventId = eventId
                };
                db.Invites.Add(newInvite);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        if(userId == null)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}