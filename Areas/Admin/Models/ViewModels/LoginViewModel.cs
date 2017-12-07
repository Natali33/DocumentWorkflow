using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using DocWorkflow.Areas.Admin.Models.DataModels;

namespace Admin.ViewModels
{
    /// <summary>
    /// Модель представления входа в систему
    /// </summary>
    public class LoginViewModel
    {
        public String Login { get; set; }
        public String Password { get; set; }


        /// <summary>
        /// Получение списка пользователей системы
        /// </summary>
        /// <returns></returns>
        public List<User> GetUsersList()
        {
            using (DataContext ctx = new DataContext())
            {
                var users = ctx.User.AsNoTracking()
                    .Include(u => u.UserGroup)
                    .Where(u => u.IsActive)
                    .OrderBy(t => t.UserName)
                    .ToList();

                return users;
            }
        }


        /// <summary>
        /// Проверка авторизации по логину и паролю.
        /// Возвращает "ОК", если авторизация успешна, иначе возвращает сообщение об ошибке.
        /// </summary>
        /// <returns></returns>
        public String CheckLogin()
        {
            if (!String.IsNullOrEmpty(Login) && !String.IsNullOrEmpty(Password))
            {
                DataContext ctx = new DataContext();
                User user = ctx.User
                    .Where(u => u.UserLogin.ToUpper() == Login.ToUpper() && u.IsActive)
                    .FirstOrDefault();

                if (user != null)
                {
                    bool result = Authentication.CheckAutentication(user.UserLogin, Password);
                    if (result)
                    {
                        Authentication.User = user;
                        return "OK";
                    }
                }

                return "Неверный логин или пароль.";
            }
            else
            {
                return "Необходимо ввести имя пользователя и пароль.";
            }
        }
    }
}