namespace DocWorkflow.Areas.Admin.Models.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    /// <summary>
    /// Пользователь
    /// </summary>
    [Table("User")]
    public partial class User
    {
        const String DEFALUT_PASSWORD = "123";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            DepartmentsOwned = new HashSet<Department>();
            DocumentsCreated = new HashSet<Document>();
            DocumentsExecuted = new HashSet<Document>();
            DocumentsSigned = new HashSet<Document>();
        }

        [Required]
        [StringLength(255)]
        public string UserLogin { get; set; }

        [Required]
        [StringLength(255)]
        public string UserName { get; set; }

        public bool IsActive { get; set; }

        public int UserId { get; set; }

        [StringLength(255)]
        public string PasswordHash { get; set; }

        public int UserGroup_UserGroupId { get; set; }

        public int DepartmentId { get; set; }

        [Required]
        [StringLength(255)]
        public string Position { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Department> DepartmentsOwned { get; set; }

        public virtual Department Department { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Document> DocumentsCreated { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Document> DocumentsExecuted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Document> DocumentsSigned { get; set; }

        public virtual UserGroup UserGroup { get; set; }


        /// <summary>
        /// Инициализация полей нового объекта
        /// </summary>
        public void InitNewObject()
        {
            PasswordHash = Authentication.ComputePasswordHash(DEFALUT_PASSWORD);
            IsActive = true;
        }


        /// <summary>
        /// Проверка корректности данных.
        /// Возвращает пустую строку, если данные корректны. Иначе возвращает текс сообщения для пользователя.
        /// </summary>
        /// <returns></returns>
        public String Validate()
        {
            if (String.IsNullOrEmpty(UserLogin))
                return "Необходимо ввести логин.";
            if (String.IsNullOrEmpty(UserName))
                return "Необходимо указать ФИО.";
            if (UserGroup_UserGroupId == 0)
                return "Необходимо указать роль.";
            if (DepartmentId == 0)
                return "Необходимо указать подразделение.";
            if (String.IsNullOrWhiteSpace(Position))
                return "Необходимо указать должность.";

            using (DataContext ctx = new DataContext())
            {
                User existing = ctx.User.FirstOrDefault(t => t.UserLogin.ToLower().Trim() == UserLogin.ToLower().Trim() && t.UserId != UserId);
                if (existing != null)
                    return String.Format("Пользователь с логином {0} уже существует в системе.", UserLogin);

                if (UserId > 0)
                {
                    if (UserGroup_UserGroupId != 1
                        || (UserGroup_UserGroupId == 1 && IsActive == false))
                    {
                        User existingAdmin = ctx.User.FirstOrDefault(t => t.UserGroup_UserGroupId == 1 && t.IsActive == true && t.UserId != UserId);
                        if (existingAdmin == null)
                            return String.Format("Изменения учетной записи {0} не могут быть сохранены, т.к. в системе не останется ни одного активного администратора.", UserLogin);
                    }
                }
            }

            return "";
        }


        /// <summary>
        /// Проверка возможности удаления объекта.
        /// Возвращает пустую строку, если удаление возможно. Иначе возвращает текст сообщения для пользователя.
        /// </summary>
        /// <returns></returns>
        public String CheckCanDelete()
        {
            if (UserGroup_UserGroupId == 1)
            {
                using (DataContext ctx = new DataContext())
                {
                    User existingAdmin = ctx.User.FirstOrDefault(t => t.UserGroup_UserGroupId == 1 && t.IsActive == true && t.UserId != UserId);
                    if (existingAdmin == null)
                        return String.Format("Учетная запись {0} не может быть удалена, т.к. в системе не останется ни одного активного администратора.", UserLogin);
                }
            }

            return "";
        }
    }
}
