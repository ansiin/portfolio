using DAL.Data;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers;

public class AssetTagsController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IActionResult> Index()
    {
        return View(await _context.Set<AssetTag>().ToListAsync());
    }

    public async Task<IActionResult> Details(long? AssetId, long? TagId)
    {
        if (AssetId is null || TagId is null)
        {
            return NotFound();
        }

        var entity = await _context.Set<AssetTag>().FirstOrDefaultAsync(m => m.AssetId == AssetId && m.TagId == TagId);
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
    public async Task<IActionResult> Create(AssetTag entity)
    {
        if (!ModelState.IsValid)
        {
            return View(entity);
        }

        _context.Add(entity);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(long? AssetId, long? TagId)
    {
        if (AssetId is null || TagId is null)
        {
            return NotFound();
        }

        var entity = await _context.Set<AssetTag>().FindAsync(AssetId, TagId);
        if (entity is null)
        {
            return NotFound();
        }

        return View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long AssetId, long TagId, AssetTag entity)
    {
        if (AssetId != entity.AssetId || TagId != entity.TagId)
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
            if (!AssetTagExists(entity.AssetId, entity.TagId))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(long? AssetId, long? TagId)
    {
        if (AssetId is null || TagId is null)
        {
            return NotFound();
        }

        var entity = await _context.Set<AssetTag>().FirstOrDefaultAsync(m => m.AssetId == AssetId && m.TagId == TagId);
        if (entity is null)
        {
            return NotFound();
        }

        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long AssetId, long TagId)
    {
        var entity = await _context.Set<AssetTag>().FindAsync(AssetId, TagId);
        if (entity is not null)
        {
            _context.Set<AssetTag>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool AssetTagExists(long AssetId, long TagId)
    {
        return _context.Set<AssetTag>().Any(e => e.AssetId == AssetId && e.TagId == TagId);
    }
}
