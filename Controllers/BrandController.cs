using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class BrandController : Controller

    {

        private readonly ApplicationDbContext _dbContext; 
        private readonly IWebHostEnvironment _webHostEnvironmment; //For importing images for the form upload.

        public BrandController(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironmment= webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Brand> brands = _dbContext.Brand.ToList();

            return View(brands);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Brand brand)
        {
            //Image creation by adding 

            string webRootPath= _webHostEnvironmment.WebRootPath; // for storing image

            var file = HttpContext.Request.Form.Files; //taking filles from the form

            if ((file.Count>0)) //shouldnot be zero
            {
                string newFileName=Guid.NewGuid().ToString();

                var upload = Path.Combine(webRootPath, @"images\brand"); //store images at the path 

                var extension = Path.GetExtension(file[0].FileName);

                using(var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }

                brand.BrandLogo=@"\images\brand\"+newFileName+extension;
            }
            // Upto this image adding 

            // Adding brand name 
            if (ModelState.IsValid)
            {
                _dbContext.Brand.Add(brand);
                _dbContext.SaveChanges();

                TempData["success"] = "Record Created Successfully! ";

                return RedirectToAction(nameof(Index)); //redirect to index after everything
            }

        return View(); //else if anything wrong return same page
        }

        //Details showing function
        [HttpGet]

        public IActionResult Details(Guid id)
        {
            Brand brand = _dbContext.Brand.FirstOrDefault(x=>x.Id==id); // asking to fetch the same details from db , if id matches id == id 

            return View(brand);
        }

        //Edit Method 
        [HttpGet]

        public IActionResult Edit(Guid id)
        {
            Brand brand = _dbContext.Brand.FirstOrDefault(x => x.Id == id); // asking to fetch the same details from db , if id matches id == id 

            return View(brand);
        }

        //Edit Post Method for name and year
        [HttpPost]
        public IActionResult Edit(Brand brand)
        {
            //Edit for updating image (logo)
            string webRootPath = _webHostEnvironmment.WebRootPath; // for storing image

            var file = HttpContext.Request.Form.Files; //taking filles from the form

            if ((file.Count > 0)) //shouldnot be zero
            {
                string newFileName = Guid.NewGuid().ToString();

                var upload = Path.Combine(webRootPath, @"images\brand"); //store images at the path 

                var extension = Path.GetExtension(file[0].FileName);


                //delete old image
                var objFromDb = _dbContext.Brand.AsNoTracking().FirstOrDefault(x => x.Id == brand.Id); // taking url of the image object

                if (objFromDb.BrandLogo != null) 
                {
                    var oldImagePath = Path.Combine(webRootPath, objFromDb.BrandLogo.Trim('\\')); // url will have multiple \\ deleting them by trim method

                    if (System.IO.File.Exists(oldImagePath))
                        {

                        System.IO.File.Delete(oldImagePath); //Delete the old image 
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }

                brand.BrandLogo = @"\images\brand\" + newFileName + extension;
            }

            if (ModelState.IsValid)
            {
                var objFromDb = _dbContext.Brand.AsNoTracking().FirstOrDefault(x => x.Id == brand.Id);

                objFromDb.Name = brand.Name;
                objFromDb.EstablishedYear = brand.EstablishedYear;

                if (brand.BrandLogo != null) // Avoiding of no image update can lead to deletion of old image even if it is same
                { 
                  objFromDb.BrandLogo = brand.BrandLogo; 
                }

                _dbContext.Brand.Update(objFromDb);
                _dbContext.SaveChanges();

                TempData["warning"] = "Record updated Successfully! ";

                return RedirectToAction(nameof(Index)); //redirect to index after everything
            }
            return View(); //else if anything wrong return same page
        }


        //Delete GET method
        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            Brand brand = _dbContext.Brand.FirstOrDefault(x => x.Id == id); // asking to fetch the same details from db , if id matches id == id 

            return View(brand);
        }

        //Delete POST method
        [HttpPost]
        public IActionResult Delete(Brand brand)
        {
            //Deleting image
            string webRootPath = _webHostEnvironmment.WebRootPath;
            //Checking for image presence
            if(string.IsNullOrEmpty(brand.BrandLogo))
            {
                var objFromDb = _dbContext.Brand.AsNoTracking().FirstOrDefault(x => x.Id == brand.Id); // taking url of the image object

                if (objFromDb.BrandLogo != null)
                {
                    var oldImagePath = Path.Combine(webRootPath, objFromDb.BrandLogo.Trim('\\')); // url will have multiple \\ deleting them by trim method

                    if (System.IO.File.Exists(oldImagePath))
                    {

                        System.IO.File.Delete(oldImagePath); //Delete the old image 
                    }
                }
            }
            _dbContext.Brand.Remove(brand);
            _dbContext.SaveChanges();

            TempData["error"] = "Record Deleted Successfully! ";

            return RedirectToAction(nameof(Index));
        }
    }
}
