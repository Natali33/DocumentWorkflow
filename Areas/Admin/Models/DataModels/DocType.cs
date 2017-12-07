namespace DocWorkflow.Areas.Admin.Models.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    /// <summary>
    /// Вид документа
    /// </summary>
    [Table("DocType")]
    public partial class DocType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DocType()
        {
            Documents = new HashSet<Document>();
        }

        public int DocTypeId { get; set; }

        [Required]
        [StringLength(255)]
        public string DocTypeName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Document> Documents { get; set; }


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
