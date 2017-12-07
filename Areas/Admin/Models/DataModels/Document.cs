namespace DocWorkflow.Areas.Admin.Models.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    /// <summary>
    /// Документ
    /// </summary>
    [Table("Document")]
    public partial class Document
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Document()
        {
            DocAttachment = new HashSet<DocAttachment>();
        }

        public int DocumentId { get; set; }

        public int DocTypeId { get; set; }

        public int DocStatusId { get; set; }

        public int CreatorId { get; set; }

        [Required]
        [StringLength(255)]
        public string DocName { get; set; }

        public string Content { get; set; }

        public int ExecutorId { get; set; }

        public int SignerId { get; set; }

        public DateTime DateExecution { get; set; }

        public DateTime? DateDone { get; set; }

        public DateTime DateCreated { get; set; }

        public string CancellationNote { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocAttachment> DocAttachment { get; set; }

        public virtual DocStatus DocStatus { get; set; }

        public virtual DocType DocType { get; set; }

        public virtual User Creator { get; set; }

        public virtual User Executor { get; set; }

        public virtual User Signer { get; set; }


        /// <summary>
        /// Инициализация полей нового объекта
        /// </summary>
        public void InitNewObject()
        {
            Creator = Authentication.User;
            CreatorId = Authentication.User.UserId;
            DateCreated = DateTime.Today;

            using (DataContext ctx = new DataContext())
            {
                DocStatus = ctx.DocStatus.First(t => t.DocStatusId == 1);
                DocStatusId = DocStatus.DocStatusId;
            }
        }

        /// <summary>
        /// Проверка корректности данных.
        /// Возвращает пустую строку, если данные корректны. Иначе возвращает текс сообщения для пользователя.
        /// </summary>
        /// <returns></returns>
        public String Validate()
        {
            if (DocTypeId == 0)
                return "Не указан вид документа.";
            if (String.IsNullOrWhiteSpace(DocName))
                return "Не указано наименование документа.";
            if (SignerId == 0)
                return "Не указан подписант.";
            if (ExecutorId == 0)
                return "Не указан исполнитель.";
            if (DateExecution == DateTime.MinValue)
                return "Не указан срок исполнения.";

            return "";
        }


        /// <summary>
        /// Проверка возможности удаления объекта.
        /// Возвращает пустую строку, если удаление возможно. Иначе возвращает текст сообщения для пользователя.
        /// </summary>
        /// <returns></returns>
        public String CheckCanDelete()
        {
            if (DocStatusId == 1)
                return "";

            if (Authentication.UserIsSysadmin)
                return "";

            return "Этот документ может быть удален только системным администратором, т.к. документ опубликован.";
        }


        /// <summary>
        /// Признак возможности редактирования документа
        /// </summary>
        public bool CanEdit
        {
            get
            {
                return Authentication.User.UserId == CreatorId
                    && (DocStatusId == 1 || DocStatusId == 4); //черновик или отменен
            }
        }


        /// <summary>
        /// Признак возможности сохранить как черновик
        /// </summary>
        public bool CanSaveAsDraft
        {
            get
            {
                return Authentication.User.UserId == CreatorId
                    && (DocStatusId == 1 || DocStatusId == 4); //черновик или отменен
            }
        }


        /// <summary>
        /// Признак возможности публикации документа
        /// </summary>
        public bool CanPublication
        {
            get
            {
                return Authentication.User.UserId == CreatorId
                    && (DocStatusId == 1 || DocStatusId==4); //черновик или отменен
            }
        }

        
        /// <summary>
        /// Признак возможности подписания документа
        /// </summary>
        public bool CanSign
        {
            get
            {
                return Authentication.User.UserId == SignerId
                    && DocStatusId == 2; //опубликован
            }
        }


        /// <summary>
        /// Признак возможности отметки документа как исполненного
        /// </summary>
        public bool CanDone
        {
            get
            {
                return Authentication.User.UserId == ExecutorId
                    && DocStatusId == 3; //назначен
            }
        }


        /// <summary>
        /// Признак возможности отмены документа
        /// </summary>
        public bool CanCancel
        {
            get
            {
                if (Authentication.User.UserId == CreatorId)
                {
                    if (DocStatusId == 2 || DocStatusId == 3) //опубликован или назначен
                        return true;
                }

                if (Authentication.User.UserId == SignerId)
                {
                    if (DocStatusId == 2) //опубликован
                        return true;
                }

                if (Authentication.User.UserId == ExecutorId)
                {
                    if (DocStatusId == 3) //назначен
                        return true;
                }

                return false;
            }
        }


        /// <summary>
        /// Публикация документа
        /// </summary>
        /// <returns></returns>
        public String DoPublication()
        {
            if (!CanPublication)
                return "Документ не может быть опубликован.";

            DocStatusId = 2; //опубликован
            CancellationNote = "";

            return "";
        }


        /// <summary>
        /// Подписание документа
        /// </summary>
        /// <returns></returns>
        public String DoSign()
        {
            if (!CanSign)
                return "Документ не может быть подписан текущим пользователем.";

            DocStatusId = 3; //назначен

            return "";
        }


        /// <summary>
        /// Отметка документа как исполненного
        /// </summary>
        /// <returns></returns>
        public String SetDone()
        {
            if (!CanDone)
                return "Документ не может быть отмечен как исполненный.";

            DocStatusId = 5; //выполнен
            DateDone = DateTime.Today;

            return "";
        }


        /// <summary>
        /// Отмена документа
        /// </summary>
        /// <returns></returns>
        public String SetCancelled()
        {
            if (!CanCancel)
                return "Документ не может быть отменен.";

            if (String.IsNullOrWhiteSpace(CancellationNote))
                return "Необходимо указать комментарий.";

            DocStatusId = 4; //отменен

            return "";
        }
    }
}
