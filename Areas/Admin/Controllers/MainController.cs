using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Ext.Net;
using Ext.Net.MVC;
using System.Globalization;
using Admin.ViewModels;
using DocWorkflow.Areas.Admin.Models.DataModels;

namespace Admin.Controllers
{
    /// <summary>
    /// Контроллер основной формы веб-интерфейса
    /// </summary>
    [DirectController]
    public class MainController : System.Web.Mvc.Controller
    {
        /// <summary>
        /// Отображение главной страницы
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {            
            if (Authentication.User == null)
                return this.Redirect("/Admin/Login/Index");

            MainViewModel model = new MainViewModel();
            model.InitModel();

            return View(model);
        }


        /// <summary>
        /// Получение списка документов с учетом фильтра
        /// </summary>
        /// <param name="taskGroup"></param>
        /// <returns></returns>
        public StoreResult GetDocumentList(String taskGroup)
        {
            int userId = Authentication.User.UserId;

            using (DataContext ctx = new DataContext())
            {
                var items = from item in ctx.Document.AsNoTracking()
                            where (taskGroup == "CreatedByMe" && userId == item.CreatorId)
                                || (taskGroup == "SignToMe" && userId == item.SignerId && item.DocStatusId == 2) //опубликован
                                || (taskGroup == "ExecuteToMe" && userId == item.ExecutorId && item.DocStatusId == 3) //назначен
                                || (taskGroup == "All")
                            orderby item.DocumentId descending
                            select new
                            {
                                item.DocumentId,
                                item.DocName,
                                item.DateCreated,
                                item.DateDone,
                                item.DateExecution,
                                item.DocTypeId,
                                item.DocType.DocTypeName,
                                item.DocStatusId,
                                item.DocStatus.DocStatusName,
                                Creator = item.Creator.UserName,
                                CreatorDepartment = item.Creator.Department.DepartmentName,
                                Signer = item.Signer.UserName,
                                SignerDepartment = item.Signer.Department.DepartmentName,
                                Executor = item.Executor.UserName,
                                ExecutorDepartment = item.Executor.Department.DepartmentName
                            };

                return this.Store(items.ToList());
            }
        }


        /// <summary>
        /// Получение списка подразделений и статистики обработки документов
        /// </summary>
        /// <returns></returns>
        public StoreResult GetDepartmentList()
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
                                PersonsCount = item.Persons.Count,
                                QtyDrafts = ctx.Document.Count(d => d.Creator.DepartmentId == item.DepartmentId && d.DocStatusId == 1), //черновик
                                QtyWaitingForSign = ctx.Document.Count(d => d.Signer.DepartmentId == item.DepartmentId && d.DocStatusId == 2), //опубликован
                                QtyExecuting = ctx.Document.Count(d => d.Executor.DepartmentId == item.DepartmentId && d.DocStatusId == 3) //назначен
                            };

                return this.Store(items.ToList());
            }
        }
    }
}
