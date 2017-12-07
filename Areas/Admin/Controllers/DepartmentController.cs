using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using DocWorkflow.Areas.Admin.Models.DataModels;

namespace Admin.Controllers
{
    /// <summary>
    /// Контроллер формы редактирования подразделений
    /// </summary>
    public class DepartmentController : Controller
    {
        /// <summary>
        /// Отображение формы создания новой записи
        /// </summary>
        /// <param name="onCloseClientScript">Клиентский javascript, который следует выполнить при закрытии окна</param>
        /// <returns></returns>
        public ActionResult CreateNew(String onCloseClientScript = null)
        {
            Department model = new Department();
            model.InitNewObject();

            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult()
            {
                ViewName = "EditForm",
                WrapByScriptTag = false,
                Model = model,
                RenderMode = RenderMode.Auto
            };

            if (String.IsNullOrWhiteSpace(onCloseClientScript) || onCloseClientScript == "null")
                onCloseClientScript = "{}";

            result.ViewBag.OnCloseClientScript = onCloseClientScript;
            result.ViewBag.IsNew = true;
            result.ViewBag.UsersList = GetUsersList();

            return result;
        }


        /// <summary>
        /// Отображение формы редактирования существующей записи
        /// </summary>
        /// <param name="id"></param>
        /// <param name="onCloseClientScript">Клиентский javascript, который следует выполнить при закрытии окна</param>
        /// <returns></returns>
        public ActionResult OpenExisting(int id, String onCloseClientScript = null)
        {
            DataContext ctx = new DataContext();
            Department model = ctx.Department.AsNoTracking().FirstOrDefault(u => u.DepartmentId == id);
            if (model == null)
                throw new Exception("Объект не существует");

            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult()
            {
                ViewName = "EditForm",
                WrapByScriptTag = false,
                Model = model,
                RenderMode = RenderMode.Auto
            };

            if (String.IsNullOrWhiteSpace(onCloseClientScript) || onCloseClientScript == "null")
                onCloseClientScript = "{}";

            result.ViewBag.OnCloseClientScript = onCloseClientScript;
            result.ViewBag.IsNew = false;
            result.ViewBag.UsersList = GetUsersList();

            return result;
        }


        /// <summary>
        /// Получение списка пользователей
        /// </summary>
        /// <param name="DepartmentId"></param>
        /// <returns></returns>
        private List<User> GetUsersList()
        {
            using (DataContext ctx = new DataContext())
            {
                var users = ctx.User
                    .Include(u => u.UserGroup)
                    .Include(u => u.Department)
                    .Where(u => u.IsActive)
                    .OrderBy(t => t.UserName)
                    .ToList();

                return users;
            }
        }        


        /// <summary>
        /// Получение всех элементов
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllItems()
        {
            using (DataContext ctx = new DataContext())
            {
                var items = from item in ctx.Department.AsNoTracking()
                            orderby item.DepartmentName
                            select new
                            {
                                item.DepartmentId,
                                item.DepartmentName,
                                DepartmentHead = item.DepartmentHead.UserName,
                                PersonsCount = item.Persons.Count
                            };

                return this.Store(items.ToList());
            }
        }


        /// <summary>
        /// Удаление элемента
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            using (DataContext ctx = new DataContext())
            {
                Department item = ctx.Department.Find(id);

                if (item != null)
                {
                    String msg = item.CheckCanDelete();
                    if (String.IsNullOrEmpty(msg))
                    {
                        ctx.Department.Remove(item);

                        try
                        {
                            ctx.SaveChanges();
                        }
                        catch
                        {
                            X.Msg.Alert("Внимание!", "Элемент справочника не может быть удален, т.к. от этого элемента зависит один или несколько объектов.").Show();
                        }
                    }
                    else
                    {
                        X.Msg.Alert("Внимание!", msg).Show();
                    }
                }

                return this.Direct();
            }
        }


        /// <summary>
        /// Сохранение данных и закрытие окна
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Save(Department model)
        {
            try
            {
                String msg = model.Validate();
                if (!String.IsNullOrEmpty(msg))
                {
                    X.Msg.Alert("Ошибка", msg).Show();
                    return this.Direct();
                }

                DataContext ctx = new DataContext();
                Department item = ctx.Department.Find(model.DepartmentId);
                if (item == null)
                {
                    ctx.Department.Add(model);
                    item = model;
                }
                else
                {
                    item.DepartmentName = model.DepartmentName;
                    item.DepartmentId = model.DepartmentId;
                    item.DepartmentHeadId = model.DepartmentHeadId;
                }

                ctx.SaveChanges();

                X.GetCmp<Window>("Department_window").Close();
                return this.Direct();
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }        
    }
}