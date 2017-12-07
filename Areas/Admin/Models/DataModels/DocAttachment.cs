namespace DocWorkflow.Areas.Admin.Models.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    /// <summary>
    /// Прикрепленный файл
    /// </summary>
    [Table("DocAttachment")]
    public partial class DocAttachment
    {
        public int DocAttachmentId { get; set; }

        public int DocumentId { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        [Column(TypeName = "image")]
        public byte[] FileData { get; set; }

        public virtual Document Document { get; set; }


        /// <summary>
        /// Инициализация полей нового объекта
        /// </summary>
        public void InitNewObject()
        {
        }

        /// <summary>
        /// Проверка корректности данных.
        /// Возвращает пустую строку, если данные корректны. Иначе возвращает текс сообщения для пользователя.
        /// </summary>
        /// <returns></returns>
        public String Validate()
        {
            return "";
        }

        /// <summary>
        /// Проверка возможности удаления объекта.
        /// Возвращает пустую строку, если удаление возможно. Иначе возвращает текст сообщения для пользователя.
        /// </summary>
        /// <returns></returns>
        public String CheckCanDelete()
        {
            return "";
        }
    }
}
