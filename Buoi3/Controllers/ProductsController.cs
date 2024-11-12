using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Buoi3.Models;
using System.Drawing.Printing;

namespace Buoi3.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Net1041Bai3Context _context;

        public ProductsController(Net1041Bai3Context context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            //var net1041Bai3Context = _context.Products.Include(p => p.Category);
            //return View(await net1041Bai3Context.ToListAsync());
            var columns = _context.Model.FindEntityType(typeof(Product)).GetProperties()
                
            .Select(p => p.Name)
            .ToList();
            ViewBag.Columns = columns;

            var ProductsList = from sp in _context.Products
                               join cate in _context.Categories on sp.CategoryId equals cate.CategoryId
                               select new Product
                               {
                                   ProductName = sp.ProductName,
                                   Price = sp.Price,
                                   Quantity = sp.Quantity,
                                   Brand = sp.Brand,
                                   Model = sp.Model,
                                   ManufactureDate = sp.ManufactureDate,
                                   ExpirationDate = sp.ExpirationDate,
                                   Rating = sp.Rating,
                                   IsAvailable = sp.IsAvailable,
                                   CreatedDate = sp.CreatedDate,
                                   UpdatedDate = sp.UpdatedDate,
                                   Category = cate
                               };


            return View(await ProductsList.ToListAsync());

        }
        [HttpPost]
        public IActionResult Index(string search, decimal? priceFrom, decimal? priceTo, string sortBy, string order, int page =1, int pageSize =5)
        {
            var filteredProducts = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                filteredProducts = filteredProducts.Where(p => p.ProductName.Contains(search));
            }
            if (priceFrom.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price >= priceFrom.Value);
            }
            if (priceTo.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price <= priceTo.Value);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy == "name")
                {
                    filteredProducts = order == "desc" ? filteredProducts.OrderByDescending(p => p.ProductName) : filteredProducts.OrderBy(p => p.ProductName);
                }
                else if (sortBy == "price")
                {
                    filteredProducts = order == "desc" ? filteredProducts.OrderByDescending(p => p.Price) : filteredProducts.OrderBy(p => p.Price);
                }
                else if (sortBy == "rating")
                {
                    filteredProducts = order == "desc" ? filteredProducts.OrderByDescending(p => p.Rating) : filteredProducts.OrderBy(p => p.Rating);
                }
                else if (sortBy == "model")
                {
                    filteredProducts = order == "desc" ? filteredProducts.OrderByDescending(p => p.Model) : filteredProducts.OrderBy(p => p.Model);
                }
                else if (sortBy == "brand")
                {
                    filteredProducts = order == "desc" ? filteredProducts.OrderByDescending(p => p.Brand) : filteredProducts.OrderBy(p => p.Brand);
                }
                else if (sortBy == "quantity")
                {
                    filteredProducts = order == "desc" ? filteredProducts.OrderByDescending(p => p.Quantity) : filteredProducts.OrderBy(p => p.Quantity);
                }
            }
            ////// Phân trang
            //int totalProducts = filteredProducts.Count();
            //int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            //var products = filteredProducts
            //.Skip((page - 1) * pageSize)
            //    .Take(pageSize)
            //    .ToList();

            ////// Gửi dữ liệu tới View
            //ViewBag.CurrentPage = page;
            //ViewBag.TotalPages = totalPages;
            //ViewBag.PageSize = pageSize;

            // Truyền lại giá trị để giữ nguyên khi view được load lại
            ViewData["Search"] = search;
            ViewData["PriceFrom"] = priceFrom;
            ViewData["PriceTo"] = priceTo;
            ViewData["SortBy"] = sortBy;
            ViewData["Order"] = order;

            //return View(products);

            // Trả về danh sách sản phẩm lọc được tới View
            return View(filteredProducts.ToList());
        }
        public async Task<IActionResult> Index2(string search, decimal? fromPrice, decimal? toPrice, string sortProperty, string sortOrder, int page = 1)
        {
            var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();
            var a = _context.Products.ToList();
            int pageSize = 5;
            var products = await productsQuery.Skip((page - 1) * pageSize).
                Take(pageSize).ToListAsync();
            return View(products);
        }
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Price,Quantity,CategoryId,Brand,Model,ManufactureDate,ExpirationDate,Rating,IsAvailable,CreatedDate,UpdatedDate")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,Price,Quantity,CategoryId,Brand,Model,ManufactureDate,ExpirationDate,Rating,IsAvailable,CreatedDate,UpdatedDate")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }



    }
}
