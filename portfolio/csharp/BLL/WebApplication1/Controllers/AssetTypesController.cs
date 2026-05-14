using DAL.Data;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers;

public class AssetTypesController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IActionResult> Index()
    {
        return View(await _context.Set<AssetType>().ToListAsync());
    }

    public async Task<IActionResult> Details(long? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var entity = await _context.Set<AssetType>().FirstOrDefaultAsync(m => m.Id == id);
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
    public async Task<IActionResult> Create(AssetType entity)
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

        var entity = await _context.Set<AssetType>().FindAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        return View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, AssetType entity)
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
            if (!AssetTypeExists(entity.Id))
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

        var entity = await _context.Set<AssetType>().FirstOrDefaultAsync(m => m.Id == id);
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
        var entity = await _context.Set<AssetType>().FindAsync(id);
        if (entity is not null)
        {
            _context.Set<AssetType>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool AssetTypeExists(long id)
    {
        return _context.Set<AssetType>().Any(e => e.Id == id);
    }
}
