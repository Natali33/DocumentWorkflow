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
    /// Контроллер формы редактирования пользователей
    /// </summary>
    public class UserController : Controller
    {
        /// <summary>
        /// Отображение формы создания новой записи
        /// </summary>
        /// <param name="onCloseClientScript">Клиентский javascript, который следует выполнить при закрытии окна</param>
        /// <returns></returns>
        public ActionResult CreateNew(String onCloseClientScript = null)
        {
            User model = new User();
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
            result.ViewBag.UserGroups = GetUserGroups();
            result.ViewBag.Departments = GetDepartments();

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
            User model = ctx.User.AsNoTracking().FirstOrDefault(u => u.UserId == id);
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
            result.ViewBag.UserGroups = GetUserGroups();
            result.ViewBag.Departments = GetDepartments();

            return result;
        }


        /// <summary>
        /// Получение списка групп пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private List<UserGroup> GetUserGroups()
        {
            using (DataContext ctx = new DataContext())
            {
                var groups = ctx.UserGroup.OrderBy(t => t.Name).ToList();
                return groups;
            }
        }


        /// <summary>
        /// Получение списка подразделений
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private List<Department> GetDepartments()
        {
            IEnumerable<Department> deptItems;
            using (DataContext ctx = new DataContext())
            {
                deptItems = ctx.Department
                    .OrderBy(t => t.DepartmentName)
                    .ToList();
            }

            return deptItems.Select(t =>
                new Department()
                {
                    DepartmentId = t.DepartmentId,
                    DepartmentName = t.DepartmentName,
                    DepartmentHeadId = t.DepartmentHeadId
                })
                .ToList();
        }


        /// <summary>
        /// Получение всех элементов
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllItems()
        {
            using (DataContext ctx = new DataContext())
            {
                var items = from item in ctx.User.AsNoTracking()
                            orderby item.UserName
                            select new
                            {
                                item.UserId,
                                item.UserName,
                                item.UserLogin,
                                UserGroupName = item.UserGroup.Name,
                                DepartmentName = item.Department.DepartmentName,
                                Position = item.Position,
                                item.IsActive
                            };

                return this.Store(items.ToList());
            }
        }


        /// <summary>
        /// Получение объекта по ID и возврат на клиент в виде DirectResult
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetById(int id)
        {
            using (DataContext ctx = new DataContext())
            {
                User item = ctx.User
                    .Include(u => u.UserGroup)
                    .Include(u => u.Department)
                    .AsNoTracking()
                    .First(u => u.UserId == id);

                DirectResult result = this.Direct();
                result.Result = new User()
                {
                    UserId = item.UserId,
                    UserName = item.UserName,
                    UserLogin = item.UserLogin,
                    UserGroup_UserGroupId = item.UserGroup_UserGroupId,
                    DepartmentId = item.Department.DepartmentId,
                    Position = item.Position,
                    IsActive = item.IsActive
                };
                return result;
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
                User item = ctx.User.Find(id);

                if (item != null)
                {
                    String msg = item.CheckCanDelete();
                    if (String.IsNullOrEmpty(msg))
                    {
                        ctx.User.Remove(item);

                        try
                        {
                            ctx.SaveChanges();
                        }
                        catch
                        {
                            X.Msg.Alert("Внимание!", "Учетная запись пользователя не может быть удалена из системы, т.к. от данной учетной записи зависит один или несколько объектов. Вместо удаления рекомендуется отключить вход для выбранной учетной записи.").Show();
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
        public ActionResult Save(User model)
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
                User item = ctx.User.Find(model.UserId);
                if (item == null)
                {
                    ctx.User.Add(model);
                    item = model;
                }
                else
                {
                    item.UserLogin = model.UserLogin;
                    item.UserName = model.UserName;
                    item.UserGroup_UserGroupId = model.UserGroup_UserGroupId;
                    item.DepartmentId = model.DepartmentId;
                    item.Position = model.Position;
                    item.IsActive = model.IsActive;
                }

                ctx.SaveChanges();

                X.GetCmp<Window>("User_window").Close();
                return this.Direct();
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }        
    }
}