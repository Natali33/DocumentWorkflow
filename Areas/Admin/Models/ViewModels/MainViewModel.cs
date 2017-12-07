using DocWorkflow.Areas.Admin.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Admin.ViewModels
{
    /// <summary>
    /// Модель главного представления
    /// </summary>
    public class MainViewModel
    {
        /// <summary>
        /// Текущий авторизованный пользователь
        /// </summary>
        public User CurrentUser { get; set; }

        /// <summary>
        /// Строка с ФИО текущего пользователя
        /// </summary>
        public String UserAuthorizationStatus
        {
            get
            {
                if(Authentication.User!=null)
                {
                    return Authentication.User.UserName + " (" + Authentication.User.UserGroup.Name + ")";
                }
                else
                {
                    return "";
                }
            }
        }


        /// <summary>
        /// Инициализация модели
        /// </summary>
        public void InitModel()
        {
            DataContext ctx = new DataContext();
            CurrentUser = ctx.User.Find(Authentication.User.UserId);
        }
    }
}