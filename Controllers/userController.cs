using Login.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Login.Controllers
{
    public class userController : Controller
    {

        public readonly contextDB _Db;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public userController(contextDB dB , IConfiguration configuration , IWebHostEnvironment webHostEnvironment) {

            _Db = dB;
            _config = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]

        [HttpPost("CreateUser")]

        public IActionResult postUser(users User)
        {

        
            if(_Db.tbl_register.Where(userss => userss.username == User.username).Any()) { 
            return Ok("Email Already In Use !!!");
            }

           /*
             if (_Db.tbl_register.Where(userss => userss.username == User.username).FirstOrDefault() != null)
            {
                return Ok("Email Already In Use !!");
            }

           */


            User.dateCreated = DateTime.Now;

            _Db.tbl_register.Add(User);
            _Db.SaveChanges();

            return Ok("User Created");
        }

        [AllowAnonymous]
        [HttpPost("getUser")]
        public IActionResult getUser(loginUsers user) {

            var user_available = _Db.tbl_register.Where(setUser => setUser.username == user.username && setUser.password == user.password).FirstOrDefault();

            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Secure = true;    
            cookieOptions.HttpOnly = true;

            cookieOptions.Expires = DateTime.Now.AddMinutes(Int32.Parse(_config.GetSection("jwtConfig").GetSection("DURATION").Value));

          

     
          

            if (user_available != null)
            {

                var tokenGenerate = new JwtService(_config).GenerateToken(
                    user_available.ID.ToString(),
                    user_available.username

                    );

                Response.Cookies.Append("token", tokenGenerate, cookieOptions);
                return Ok(tokenGenerate);

                /*
                return Ok(new JwtService(_config).GenerateToken(
                    user_available.ID.ToString(),
                    user_available.username
                    ));

                */


            }
            else
            {
                Response.Cookies.Delete("token");
                return Ok("Wrong Credentials");
            }


        
        
        }


        [HttpGet("getProducts")]


         
        public IActionResult getProducts()
        {

            //var getProd = from product in _Db.tbl_products select product;

            var getProd = from products in _Db.tbl_products
                          select new products
                          {
                              ID = products.ID,
                              prod_category = products.prod_category,
                              prod_name = products.prod_name,
                              prod_detail = products.prod_detail,
                              prod_imageName = products.prod_imageName,
                              prod_image = products.prod_image,
                              prod_imageSrc = String.Format("{0}://{1}{2}/Images/{3}" , Request.Scheme , Request.Host , Request.PathBase , products.prod_imageName),
                    

                          };


            return Json(getProd);
        }

        [HttpPost("updateProducts")]
        public async Task<IActionResult> updateproduct([FromForm]products Prod)
        {

            //Prod.prod_imageName = await saveImage(Prod.prod_image);

      
            /*
             if(Prod.prod_image != null )
             {
                 DeleteImage(Prod.prod_imageName);
                 Prod.prod_imageName = await saveImage(Prod.prod_image);
             }

            _Db.Entry(Prod).State = EntityState.Modified;
            await _Db.SaveChangesAsync();

            */
      
            if(Prod.ID == 0)
            {
                Prod.prod_imageName = await saveImage(Prod.prod_image);
                _Db.tbl_products.Add(Prod);
                await _Db.SaveChangesAsync();
            }
            else
            {
                DeleteImage(Prod.prod_imageName);
                Prod.prod_imageName = await saveImage(Prod.prod_image);
                _Db.Entry(Prod).State = EntityState.Modified;
                await _Db.SaveChangesAsync();


            }


            return RedirectToAction("getProducts");


        }


        [HttpDelete("DeleteProducts")]
        public async Task<IActionResult> deleteProduct(int id , products Prod)
        {

            var chkId = await _Db.tbl_products.FindAsync(id);

            try
            {
                if (chkId != null)
                {
                    //_Db.tbl_products.Remove(chkId);
                    DeleteImage(Prod.prod_imageName);
                    _Db.Entry(chkId).State = EntityState.Deleted;
                    await _Db.SaveChangesAsync();

                }

                return RedirectToAction("getProducts");
            }

            catch (Exception ex)
            {
                return RedirectToAction("getProducts");
            }

      
        }

        [NonAction]
        public async Task<string> saveImage(IFormFile imageFile)
        {
            string imageName =new String (Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');

            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);

            var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", imageName);

            using (var fileStream = new FileStream(imagePath , FileMode.Create))
            {

                await imageFile.CopyToAsync(fileStream);
            }

            return imageName;
        }


        [NonAction]
        public void DeleteImage(string imageName)
        {
            var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", imageName);

            if(System.IO.File.Exists(imagePath)) {
                System.IO.File.Delete(imagePath);
            }
        }

    

 

    }
}
