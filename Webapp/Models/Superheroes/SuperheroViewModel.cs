using Webapp.Models.Superheroes;

public class SuperheroViewModel
{
    public Superhero Superhero { get; set; }
    public List<Superpower> AvailableSuperPowers { get; set; }
    public List<int> SelectedSuperPowers { get; set; }
}