using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Webapp.Models.Superheroes;

namespace Webapp.Controllers
{
    [Authorize(Roles = "admin,user")]
    public class SuperheroesController : Controller
    {
        private readonly SuperheroesContext _context;
        private readonly ILogger<SuperheroesController> _logger;
        public SuperheroesController(SuperheroesContext context, ILogger<SuperheroesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Superheroes
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 10; // Number of items per page
            int pageNumber = page ?? 1;

            var superheroes = _context.Superheroes
                .Include(s => s.Alignment)
                .Include(s => s.EyeColour)
                .Include(s => s.Gender)
                .Include(s => s.HairColour)
                .Include(s => s.Publisher)
                .Include(s => s.Race)
                .Include(s => s.SkinColour)
                .AsQueryable();

            var paginatedList = await PaginatedList<Superhero>.CreateAsync(superheroes, pageNumber, pageSize);

            return View(paginatedList);
        }

        // GET: Superheroes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var superhero = await _context.Superheroes
                .Include(s => s.Alignment)
                .Include(s => s.EyeColour)
                .Include(s => s.Gender)
                .Include(s => s.HairColour)
                .Include(s => s.Publisher)
                .Include(s => s.Race)
                .Include(s => s.SkinColour)
                .Include(s => s.HeroPowers)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (superhero == null)
            {
                return NotFound();
            }

            return View(superhero);
        }
        public async Task<IActionResult> SuperPowerList(int page = 1, int size = 10)
        {
            var superPowerCounts = _context.Superpowers
                .Select(sp => new SuperPowerViewModel
                {
                    PowerName = sp.PowerName,
                    SuperheroCount = sp.HeroPowers.Count
                });

            var paginatedList = await PaginatedList<SuperPowerViewModel>.CreateAsync(superPowerCounts, page, size);
            return View(paginatedList);
        }

        // GET: Superheroes/Create
        public IActionResult Create()
        {
            var viewModel = new SuperheroViewModel
            {
                Genders = _context.Genders
                    .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Gender1 })
                    .ToList(),
                EyeColours = _context.Colours
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Colour1 })
                    .ToList(),
                HairColours = _context.Colours
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Colour1 })
                    .ToList(),
                SkinColours = _context.Colours
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Colour1 })
                    .ToList(),
                Races = _context.Races
                    .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Race1 })
                    .ToList(),
                Publishers = _context.Publishers
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.PublisherName })
                    .ToList(),
                Alignments = _context.Alignments
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Alignment1 })
                    .ToList(),
                AvailablePowers = _context.Superpowers
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.PowerName })
                    .ToList(),
                AttributeValues = _context.Attributes
                    .Select(a => new AttributeValueModel
                    {
                        AttributeId = a.Id,
                        AttributeName = a.AttributeName,
                        Value = 0
                    })
                    .ToList()
            };

            return View(viewModel);
        }

        // POST: Superheroes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SuperheroViewModel model)
        {
            _logger.LogDebug("Attempting to create superhero: {@Model}", model);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state: {@ModelState}", ModelState);
                // Reload all dropdown lists
                await ReloadDropdownLists(model);
                return View(model);
            }

            try
            {
                var maxId = await _context.Superheroes.MaxAsync(s => (int?)s.Id) ?? 0;
                var superhero = new Superhero
                {
                    Id = maxId + 1,
                    SuperheroName = model.SuperheroName,
                    FullName = model.FullName,
                    HeightCm = model.HeightCm,
                    WeightKg = model.WeightKg,
                    GenderId = model.GenderId,
                    EyeColourId = model.EyeColourId,
                    HairColourId = model.HairColourId,
                    SkinColourId = model.SkinColourId,
                    RaceId = model.RaceId,
                    PublisherId = model.PublisherId,
                    AlignmentId = model.AlignmentId
                };
                if (model.SelectedPowers != null)
                {
                    foreach (var powerId in model.SelectedPowers)
                    {
                        superhero.HeroPowers.Add(new HeroPower { PowerId = powerId });
                    }
                }

                _context.Superheroes.Add(superhero);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating superhero");
                ModelState.AddModelError("", "Failed to create superhero: " + ex.Message);
                await ReloadDropdownLists(model);
                return View(model);
            }
        }

        private async Task ReloadDropdownLists(SuperheroViewModel model)
        {
            model.Genders = await _context.Genders
                .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Gender1 })
                .ToListAsync();
            // ... similar for other lists
        }

        private async Task<IActionResult> PrepareViewModelForError(SuperheroViewModel model)
        {
            // Załaduj ponownie wszystkie listy wyboru
            model.EyeColours = await _context.Colours
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Colour1
                }).ToListAsync();
            model.HairColours = await _context.Colours
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Colour1
                }).ToListAsync();
            model.SkinColours = await _context.Colours
                .Select(c=> new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Colour1
                }).ToListAsync();
            model.Genders = await _context.Genders
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Gender1
                }).ToListAsync();
            model.Races = await _context.Races
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Race1
                }).ToListAsync();
            model.Publishers = await _context.Publishers
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.PublisherName
                }).ToListAsync();
            model.Alignments = await _context.Alignments
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Alignment1
                }).ToListAsync();
            // ... załaduj pozostałe listy ...

            return View(model);
        }


        // GET: Superheroes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var superhero = await _context.Superheroes.FindAsync(id);
            if (superhero == null)
            {
                return NotFound();
            }
            ViewData["AlignmentId"] = new SelectList(_context.Alignments, "Id", "Id", superhero.AlignmentId);
            ViewData["EyeColourId"] = new SelectList(_context.Colours, "Id", "Id", superhero.EyeColourId);
            ViewData["GenderId"] = new SelectList(_context.Genders, "Id", "Id", superhero.GenderId);
            ViewData["HairColourId"] = new SelectList(_context.Colours, "Id", "Id", superhero.HairColourId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Id", superhero.PublisherId);
            ViewData["RaceId"] = new SelectList(_context.Races, "Id", "Id", superhero.RaceId);
            ViewData["SkinColourId"] = new SelectList(_context.Colours, "Id", "Id", superhero.SkinColourId);
            return View(superhero);
        }

        // POST: Superheroes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SuperheroName,FullName,GenderId,EyeColourId,HairColourId,SkinColourId,RaceId,PublisherId,AlignmentId,HeightCm,WeightKg")] Superhero superhero)
        {
            if (id != superhero.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(superhero);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SuperheroExists(superhero.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AlignmentId"] = new SelectList(_context.Alignments, "Id", "Id", superhero.AlignmentId);
            ViewData["EyeColourId"] = new SelectList(_context.Colours, "Id", "Id", superhero.EyeColourId);
            ViewData["GenderId"] = new SelectList(_context.Genders, "Id", "Id", superhero.GenderId);
            ViewData["HairColourId"] = new SelectList(_context.Colours, "Id", "Id", superhero.HairColourId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Id", superhero.PublisherId);
            ViewData["RaceId"] = new SelectList(_context.Races, "Id", "Id", superhero.RaceId);
            ViewData["SkinColourId"] = new SelectList(_context.Colours, "Id", "Id", superhero.SkinColourId);
            return View(superhero);
        }

        // GET: Superheroes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var superhero = await _context.Superheroes
                .Include(s => s.Alignment)
                .Include(s => s.EyeColour)
                .Include(s => s.Gender)
                .Include(s => s.HairColour)
                .Include(s => s.Publisher)
                .Include(s => s.Race)
                .Include(s => s.SkinColour)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (superhero == null)
            {
                return NotFound();
            }

            return View(superhero);
        }

        // POST: Superheroes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var superhero = await _context.Superheroes.FindAsync(id);
            if (superhero != null)
            {
                _context.Superheroes.Remove(superhero);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SuperheroExists(int id)
        {
            return _context.Superheroes.Any(e => e.Id == id);
        }

        public async Task<IActionResult> ViewSuperPower(int id)
        {
            var superhero = await _context.Superheroes
                .Include(s => s.HeroPowers)
                .ThenInclude(hp => hp.Power)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (superhero == null)
            {
                return NotFound();
            }
            return View(superhero);
        }
    }
}
