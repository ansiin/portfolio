using DAL.Data;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers;

public class WatchlistItemsController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IActionResult> Index()
    {
        return View(await _context.Set<WatchlistItem>().ToListAsync());
    }

    public async Task<IActionResult> Details(long? WatchlistId, long? AssetId)
    {
        if (WatchlistId is null || AssetId is null)
        {
            return NotFound();
        }

        var entity = await _context.Set<WatchlistItem>().FirstOrDefaultAsync(m => m.WatchlistId == WatchlistId && m.AssetId == AssetId);
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
    public async Task<IActionResult> Create(WatchlistItem entity)
    {
        if (!ModelState.IsValid)
        {
            return View(entity);
        }

        _context.Add(entity);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(long? WatchlistId, long? AssetId)
    {
        if (WatchlistId is null || AssetId is null)
        {
            return NotFound();
        }

        var entity = await _context.Set<WatchlistItem>().FindAsync(WatchlistId, AssetId);
        if (entity is null)
        {
            return NotFound();
        }

        return View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long WatchlistId, long AssetId, WatchlistItem entity)
    {
        if (WatchlistId != entity.WatchlistId || AssetId != entity.AssetId)
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
            if (!WatchlistItemExists(entity.WatchlistId, entity.AssetId))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(long? WatchlistId, long? AssetId)
    {
        if (WatchlistId is null || AssetId is null)
        {
            return NotFound();
        }

        var entity = await _context.Set<WatchlistItem>().FirstOrDefaultAsync(m => m.WatchlistId == WatchlistId && m.AssetId == AssetId);
        if (entity is null)
        {
            return NotFound();
        }

        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long WatchlistId, long AssetId)
    {
        var entity = await _context.Set<WatchlistItem>().FindAsync(WatchlistId, AssetId);
        if (entity is not null)
        {
            _context.Set<WatchlistItem>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool WatchlistItemExists(long WatchlistId, long AssetId)
    {
        return _context.Set<WatchlistItem>().Any(e => e.WatchlistId == WatchlistId && e.AssetId == AssetId);
    }
}
