﻿using System;
using System.Collections.Generic;

namespace Webapp.Models.Superheroes;

public partial class Race
{
    public int Id { get; set; }

    public string? Race1 { get; set; }

    public virtual ICollection<Superhero> Superheroes { get; set; } = new List<Superhero>();
}
