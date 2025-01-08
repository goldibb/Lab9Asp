using Microsoft.AspNetCore.Mvc.Rendering;
using Webapp.Models.Superheroes;

public class SuperheroViewModel
{
    public string SuperheroName { get; set; }
    public string FullName { get; set; }
    public int? GenderId { get; set; }
    public int? EyeColourId { get; set; }
    public int? HairColourId { get; set; }
    public int? SkinColourId { get; set; }
    public int? RaceId { get; set; }
    public int? PublisherId { get; set; }
    public int? AlignmentId { get; set; }
    public int HeightCm { get; set; }
    public int WeightKg { get; set; }
    public List<SelectListItem> Genders { get; set; } = new();
    public List<SelectListItem> EyeColours { get; set; } = new();
    public List<SelectListItem> HairColours { get; set; } = new();
    public List<SelectListItem> SkinColours { get; set; } = new();
    public List<SelectListItem> Races { get; set; } = new();
    public List<SelectListItem> Publishers { get; set; } = new();
    public List<SelectListItem> Alignments { get; set; } = new();
    public List<SelectListItem> AvailablePowers { get; set; } = new();
    public List<int> SelectedPowers { get; set; } = new();
    
    public List<AttributeValueModel> AttributeValues { get; set; } = new();
}

public class AttributeValueModel
{
    public int AttributeId { get; set; }
    public string? AttributeName { get; set; }
    public int Value { get; set; }
}