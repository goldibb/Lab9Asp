﻿using System;
using System.Collections.Generic;

namespace Webapp.Models.Superheroes;

public partial class HeroAttribute
{
    public int? HeroId { get; set; }

    public int? AttributeId { get; set; }

    public int? AttributeValue { get; set; }

    public virtual Attribute? Attribute { get; set; }

    public virtual Superhero? Hero { get; set; }
}
