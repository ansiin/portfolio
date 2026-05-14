using DAL.Data;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers;

public class PortfoliosController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IActionResult> Index()
    {
        return View(await _context.Set<Portfolio>().ToListAsync());
    }

    public async Task<IActionResult> Details(long? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var entity = await _context.Set<Portfolio>().FirstOrDefaultAsync(m => m.Id == id);
        if (entity is null)
        {
            return NotFound();
        }

        return View(entity);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Portfolio entity)
    {
        if (!ModelState.IsValid)
        {
            return View(entity);
        }

        _context.Add(entity);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(long? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var entity = await _context.Set<Portfolio>().FindAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        return View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, Portfolio entity)
    {
        if (id != entity.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(entity);
        }

        try
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PortfolioExists(entity.Id))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(long? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var entity = await _context.Set<Portfolio>().FirstOrDefaultAsync(m => m.Id == id);
        if (entity is null)
        {
            return NotFound();
        }

        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var entity = await _context.Set<Portfolio>().FindAsync(id);
        if (entity is not null)
        {
            _context.Set<Portfolio>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool PortfolioExists(long id)
    {
        return _context.Set<Portfolio>().Any(e => e.Id == id);
    }
}
