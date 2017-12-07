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
    /// Контроллер формы редактирования видов документов
    /// </summary>
    public class DocTypeController : Controller
    {
        /// <summary>
        /// Отображение формы создания новой записи
        /// </summary>
        /// <param name="onCloseClientScript">Клиентский javascript, который следует выполнить при закрытии окна</param>
        /// <returns></returns>
        public ActionResult CreateNew(String onCloseClientScript = null)
        {
            DocType model = new DocType();
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
            DocType model = ctx.DocType.AsNoTracking().FirstOrDefault(u => u.DocTypeId == id);
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

            return result;
        }


        /// <summary>
        /// Получение всех элементов
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllItems()
        {
            using (DataContext ctx = new DataContext())
            {
                var items = from item in ctx.DocType.Include(t => t.Documents).AsNoTracking()
                            orderby item.DocTypeName
                            select new
                            {
                                item.DocTypeId,
                                item.DocTypeName,
                                DocumentsCount = item.Documents.Count
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
                DocType item = ctx.DocType.Find(id);

                if (item != null)
                {
                    String msg = item.CheckCanDelete();
                    if (String.IsNullOrEmpty(msg))
                    {
                        ctx.DocType.Remove(item);

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
        public ActionResult Save(DocType model)
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
                DocType item = ctx.DocType.Find(model.DocTypeId);
                if (item == null)
                {
                    ctx.DocType.Add(model);
                    item = model;
                }
                else
                {
                    item.DocTypeName = model.DocTypeName;
                    item.DocTypeId = model.DocTypeId;
                }

                ctx.SaveChanges();

                X.GetCmp<Window>("DocType_window").Close();
                return this.Direct();
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }        
    }
}