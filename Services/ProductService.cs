using Microsoft.EntityFrameworkCore;
using Tst_bot.Database;
using Tst_bot.Models;

namespace Tst_bot.Services;

public class ProductService
{
    private readonly AppDbcontext _context;
    
    public ProductService(AppDbcontext context)
    {
        _context = context;
        
    }
    public async Task<List<Product>> GetAvaibleProductsAsync()
    {
        return await _context.Products
            .Where(p => p.IsAvailable)
            .ToListAsync();
    }
}