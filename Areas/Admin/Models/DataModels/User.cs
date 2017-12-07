namespace DocWorkflow.Areas.Admin.Models.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    /// <summary>
    /// ������������
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
        /// ������������� ����� ������ �������
        /// </summary>
        public void InitNewObject()
        {
            PasswordHash = Authentication.ComputePasswordHash(DEFALUT_PASSWORD);
            IsActive = true;
        }


        /// <summary>
        /// �������� ������������ ������.
        /// ���������� ������ ������, ���� ������ ���������. ����� ���������� ���� ��������� ��� ������������.
        /// </summary>
        /// <returns></returns>
        public String Validate()
        {
            if (String.IsNullOrEmpty(UserLogin))
                return "���������� ������ �����.";
            if (String.IsNullOrEmpty(UserName))
                return "���������� ������� ���.";
            if (UserGroup_UserGroupId == 0)
                return "���������� ������� ����.";
            if (DepartmentId == 0)
                return "���������� ������� �������������.";
            if (String.IsNullOrWhiteSpace(Position))
                return "���������� ������� ���������.";

            using (DataContext ctx = new DataContext())
            {
                User existing = ctx.User.FirstOrDefault(t => t.UserLogin.ToLower().Trim() == UserLogin.ToLower().Trim() && t.UserId != UserId);
                if (existing != null)
                    return String.Format("������������ � ������� {0} ��� ���������� � �������.", UserLogin);

                if (UserId > 0)
                {
                    if (UserGroup_UserGroupId != 1
                        || (UserGroup_UserGroupId == 1 && IsActive == false))
                    {
                        User existingAdmin = ctx.User.FirstOrDefault(t => t.UserGroup_UserGroupId == 1 && t.IsActive == true && t.UserId != UserId);
                        if (existingAdmin == null)
                            return String.Format("��������� ������� ������ {0} �� ����� ���� ���������, �.�. � ������� �� ��������� �� ������ ��������� ��������������.", UserLogin);
                    }
                }
            }

            return "";
        }


        /// <summary>
        /// �������� ����������� �������� �������.
        /// ���������� ������ ������, ���� �������� ��������. ����� ���������� ����� ��������� ��� ������������.
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
                        return String.Format("������� ������ {0} �� ����� ���� �������, �.�. � ������� �� ��������� �� ������ ��������� ��������������.", UserLogin);
                }
            }

            return "";
        }
    }
}
