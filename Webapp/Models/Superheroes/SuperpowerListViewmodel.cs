using Microsoft.AspNetCore.Authorization;

namespace Webapp.Models.Superheroes;
[Authorize(Roles = "admin,user")]
public class SuperPowerViewModel
{
    public string PowerName { get; set; }
    public int SuperheroCount { get; set; }
    public virtual ICollection<HeroPower> HeroPowers { get; set; } = new List<HeroPower>();
}