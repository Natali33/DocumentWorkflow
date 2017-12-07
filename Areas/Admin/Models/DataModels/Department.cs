namespace DocWorkflow.Areas.Admin.Models.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    /// <summary>
    /// Подразделение
    /// </summary>
    [Table("Department")]
    public partial class Department
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Department()
        {
            Persons = new HashSet<User>();
        }

        public int DepartmentId { get; set; }

        [Required]
        [StringLength(255)]
        public string DepartmentName { get; set; }

        public int? DepartmentHeadId { get; set; }

        public virtual User DepartmentHead { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Persons { get; set; }


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
            if (String.IsNullOrWhiteSpace(DepartmentName))
                return "Не указано наименование подразделения.";
            if (DepartmentHeadId == null || DepartmentHeadId.Value == 0)
                return "Не указан руководитель подразделения.";

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
