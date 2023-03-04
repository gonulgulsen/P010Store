﻿using P010Store.Data.Abstract;
using P010Store.Entities;
using Microsoft.EntityFrameworkCore;

namespace P010Store.Data.Concrete
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository 
	{
        public CategoryRepository(DatabaseContext _context) : base(_context)
        {
        }

        public async Task<Category> GetCategoryByProducts(int id)
        {
            return await context.Categories.Where(k=>k.Id==id ).AsNoTracking().Include(p=>p.Products).FirstOrDefaultAsync();
            //context üzerindeki kategorilerden id si bu metoda gönderilen id ile eşleşen kaydı bul ve bu kaydı izleme(AsNoTracking),bulduğun kategorinin üsürnlerini de include ile join ile birleştir ve ilk kaydı metodun çağrıldığı yere döndür.
        }
    }
}
