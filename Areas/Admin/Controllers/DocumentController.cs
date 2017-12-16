using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using DocWorkflow.Areas.Admin.Models.DataModels;
using System.IO;

namespace Admin.Controllers
{
    /// <summary>
    /// Контроллер формы редактирования документа
    /// </summary>
    public class DocumentController : Controller
    {
        /// <summary>
        /// Отображение формы создания нового документа
        /// </summary>
        /// <param name="onCloseClientScript">Клиентский javascript, который следует выполнить при закрытии окна</param>
        /// <returns></returns>
        public ActionResult CreateNew(String onCloseClientScript = null)
        {
            Document model = new Document();
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
            result.ViewBag.DocTypeList = GetDocTypeList();

            return result;
        }


        /// <summary>
        /// Отображение формы редактирования существующего документа
        /// </summary>
        /// <param name="id"></param>
        /// <param name="onCloseClientScript">Клиентский javascript, который следует выполнить при закрытии окна</param>
        /// <returns></returns>
        public ActionResult OpenExisting(int id, String onCloseClientScript = null)
        {
            DataContext ctx = new DataContext();
            Document model = ctx.Document
                .Include(t => t.DocStatus)
                .Include(t => t.DocType)
                .Include(t => t.Creator)
                .Include(t => t.Signer)
                .Include(t => t.Executor)
                .AsNoTracking()
                .FirstOrDefault(u => u.DocumentId == id);

            if (model == null)
                throw new Exception("Документ не существует");

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
            result.ViewBag.DocTypeList = GetDocTypeList();

            return result;
        }


        /// <summary>
        /// Получение списка пользователей
        /// </summary>
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
        /// Получение списка видов документа
        /// </summary>
        /// <returns></returns>
        private List<DocType> GetDocTypeList()
        {
            using (DataContext ctx = new DataContext())
            {
                var docs = ctx.DocType
                    .OrderBy(t => t.DocTypeName)
                    .ToList();

                return docs;
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
                Document item = ctx.Document.Find(id);

                if (item != null)
                {
                    String msg = item.CheckCanDelete();
                    if (String.IsNullOrEmpty(msg))
                    {
                        ctx.Document.Remove(item);

                        try
                        {
                            ctx.SaveChanges();
                        }
                        catch
                        {
                            X.Msg.Alert("Внимание!", "Документ не может быть удален, т.к. от этого объекта зависит один или несколько объектов.").Show();
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
        /// Сохранение документа как черновика
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult SaveAsDraft(Document model)
        {
            DirectResult result = this.Direct();

            try
            {
                model.DocStatusId = 1; //черновик
                model.CancellationNote = "";
                if (Save(model))
                {
                    X.GetCmp<Window>("Document_window").Close();
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }


        /// <summary>
        /// Сохранение и публикация документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult SaveAndPublicate(Document model)
        {
            DirectResult result = this.Direct();

            try
            {
                String msg = model.DoPublication();
                if (String.IsNullOrEmpty(msg))
                {
                    if (Save(model))
                    {
                        X.GetCmp<Window>("Document_window").Close();
                    }
                }
                else
                {
                    X.Msg.Alert("Ошибка", msg).Show();
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }


        /// <summary>
        /// Сохранение и подписание документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Sign(Document model)
        {
            try
            {
                String msg = model.DoSign();
                if (String.IsNullOrEmpty(msg))
                {
                    if (Save(model))
                    {
                        X.GetCmp<Window>("Document_window").Close();
                    }
                }
                else
                {
                    X.Msg.Alert("Ошибка", msg).Show();
                }

                return this.Direct();
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }



        /// <summary>
        /// Сохранение и отметка документа как исполненного
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult SetDone(Document model)
        {
            try
            {
                String msg = model.SetDone();
                if (String.IsNullOrEmpty(msg))
                {
                    if (Save(model))
                    {
                        X.GetCmp<Window>("Document_window").Close();
                    }
                }
                else
                {
                    X.Msg.Alert("Ошибка", msg).Show();
                }

                return this.Direct();
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }


        /// <summary>
        /// Сохранение и отмена документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult SetCancelled(Document model)
        {
            try
            {
                String msg = model.SetCancelled();
                if (String.IsNullOrEmpty(msg))
                {
                    if (Save(model))
                    {
                        X.GetCmp<Window>("Document_window").Close();
                    }
                }
                else
                {
                    X.Msg.Alert("Ошибка", msg).Show();
                }

                return this.Direct();
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }



        /// <summary>
        /// Сохранение документа. Возвращает true если документ сохранен
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool Save(Document model)
        {
            String msg = model.Validate();
            if (!String.IsNullOrEmpty(msg))
            {
                X.Msg.Alert("Ошибка", msg).Show();
                return false;
            }

            DataContext ctx = new DataContext();
            Document item = ctx.Document.Find(model.DocumentId);
            if (item == null)
            {
                ctx.Document.Add(model);
                item = model;
            }
            else
            {
                item.DocumentId = model.DocumentId;
                item.DocTypeId = model.DocTypeId;
                item.DocStatusId = model.DocStatusId;
                item.CreatorId = model.CreatorId;
                item.DocName = model.DocName;
                item.Content = model.Content;
                item.ExecutorId = model.ExecutorId;
                item.SignerId = model.SignerId;
                item.DateExecution = model.DateExecution;
                item.DateDone = model.DateDone;
                item.DateCreated = model.DateCreated;
                item.CancellationNote = model.CancellationNote;
            }

            ctx.SaveChanges();

            return true;
        }
    }
}