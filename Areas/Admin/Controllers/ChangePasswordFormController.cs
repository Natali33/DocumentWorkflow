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
    [DirectController]
    public class ChangePasswordFormController : Controller
    {
        /// <summary>
        /// Отображение формы создания новой записи
        /// </summary>
        /// <param name="onCloseClientScript">Клиентский javascript, который следует выполнить при закрытии окна</param>
        /// <returns></returns>
        public ActionResult ShowForm(String onCloseClientScript = null)
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            model.UserLogin = Authentication.User.UserLogin;            

            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult()
            {
                ViewName = "ChangePasswordForm",
                WrapByScriptTag = false,
                Model = model,
                RenderMode = RenderMode.Auto
            };

            if (String.IsNullOrWhiteSpace(onCloseClientScript) || onCloseClientScript == "null")
                onCloseClientScript = "{}";
            result.ViewBag.OnCloseClientScript = onCloseClientScript;

            return result;
        }


        /// <summary>
        /// Сохранение данных и закрытие окна
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Save(ChangePasswordViewModel model)
        {
            try
            {
                model.UserLogin = Authentication.User.UserLogin;

                String msg = model.Validate();
                if (!String.IsNullOrEmpty(msg))
                {
                    X.Msg.Alert("Ошибка", msg).Show();
                    return this.Direct();
                }

                model.ChangePassword();                

                X.GetCmp<Window>("ChangePasswordForm_window").Close();
                return this.Direct();
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }        
    }
}