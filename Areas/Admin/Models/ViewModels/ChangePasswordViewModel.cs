using DocWorkflow.Areas.Admin.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.ViewModels
{
    /// <summary>
    /// Модель представления смены пароля пользователя
    /// </summary>
    public class ChangePasswordViewModel
    {
        public String UserLogin { get; set; }

        public String OldPassword { get; set; }

        public String NewPassword { get; set; }

        public String NewPasswordConfirm { get; set; }


        public String Validate()
        {
            using (DataContext dbContext = new DataContext())
            {
                User user = dbContext.User.FirstOrDefault(u => u.UserLogin == UserLogin);
                if (user == null)
                    return String.Format("Пользователь {0} не существует.");
                if (String.IsNullOrEmpty(OldPassword))
                    return "Необходимо ввести старый пароль.";
                if (String.IsNullOrWhiteSpace(NewPassword)
                    || String.IsNullOrWhiteSpace(NewPasswordConfirm))
                    return "Необходимо ввести новый пароль и подтверждение пароля.";
                if (NewPassword != NewPasswordConfirm)
                    return "Пароль и подтверждение пароля не совпадают.";
                if (Authentication.ComputePasswordHash(OldPassword) != user.PasswordHash)
                    return "Введен неверный старый пароль.";                
            }

            return "";
        }

        public void ChangePassword()
        {
            using (DataContext dbContext = new DataContext())
            {
                User user = dbContext.User.FirstOrDefault(u => u.UserLogin == UserLogin);
                if (user != null)
                {
                    user.PasswordHash = Authentication.ComputePasswordHash(NewPassword);
                    dbContext.SaveChanges();
                }
            }
        }
    }
}