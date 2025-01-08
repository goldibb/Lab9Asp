using System;
using System.Collections.Generic;

namespace Webapp.Models.Superheroes;

public class Superpower
{
    public int Id { get; set; }
    public string PowerName { get; set; }
    public virtual ICollection<HeroPower> HeroPowers { get; set; }
    
}