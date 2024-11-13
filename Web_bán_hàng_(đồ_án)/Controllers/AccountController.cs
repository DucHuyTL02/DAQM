using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_bán_hàng__đồ_án_.Models;

namespace Web_bán_hàng__đồ_án_.Controllers
{
    public class AccountController : Controller
    {

        // GET: LoginRegister
        private DAEntities _acc;
        public AccountController(DAEntities acc)
        {
            _acc = acc;
        }
        public AccountController() : this(new DAEntities()) // Thay thế DAEntities() bằng cách khởi tạo đúng cách nếu cần
        {
        }
        [HttpGet]
        public ActionResult Register()
        {
            var model = new RegisterModel();
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu Email đã tồn tại trong bảng Customer
                
                if (_acc.customer.Any(c => c.emailcus == model.Email))
                {
                    ModelState.AddModelError("Emailcus", "Email đã tồn tại.");
                    return View(model);
                }

                // Kiểm tra nếu Username đã tồn tại trong bảng User
                if (_acc.User.Any(u => u.username == model.UserName))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                // Tạo đối tượng User
                var user = new User
                {
                    username = model.UserName,
                    password = model.Password,  // Lưu mật khẩu chưa mã hóa (tốt nhất là mã hóa mật khẩu trước khi lưu)
                    permission = false  // Giả sử quyền mặc định là 1
                };

                // Thêm User vào bảng User
                _acc.User.Add(user);
                _acc.SaveChanges();  // Lưu vào bảng User

                // Tạo đối tượng Customer
                var customer = new customer
                {
                    namecus = model.CustomerName,
                    phonecus = model.CustomerPhone,
                    emailcus = model.CustomEmail,
                    username = model.UserName  // Liên kết với User thông qua Username
                };

                // Thêm Customer vào bảng Customer
                _acc.customer.Add(customer);
                _acc.SaveChanges();  // Lưu vào bảng Customer

                // Đăng ký thành công, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login");
            }

            return View(model);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = _acc.User.SingleOrDefault(u => u.username == username && u.password == password);
            if (user != null)
            {
                // Xử lý đăng nhập thành công
                Session["User"] = user.username;
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu");
            return View();
        }

    }
}