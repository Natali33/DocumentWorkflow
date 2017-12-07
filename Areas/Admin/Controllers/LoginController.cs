using Admin.ViewModels;
using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    /// <summary>
    /// Контроллер формы авторизации в системе
    /// </summary>
    [DirectController]
    public class LoginController : Controller
    {
        /// <summary>
        /// Отображение формы авторизации
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }


        /// <summary>
        /// Проверка логина и пароля, авторизация в системе
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult CheckLogin(LoginViewModel model)
        {
            //model.Login = "admin";
            //model.Password = "123";

            String result = model.CheckLogin();
            return Json(result);            
        }


        /// <summary>
        ///  Выход из системы
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Authentication.User = null;
            return this.Redirect(Url.Action("Index", "Login", new { area = "Admin" }));
        }
    }
}