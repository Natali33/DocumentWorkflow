namespace DocWorkflow.Areas.Admin.Models.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    /// <summary>
    /// �������������
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
        /// ������������� ����� ������ �������
        /// </summary>
        public void InitNewObject()
        {
        }

        /// <summary>
        /// �������� ������������ ������.
        /// ���������� ������ ������, ���� ������ ���������. ����� ���������� ���� ��������� ��� ������������.
        /// </summary>
        /// <returns></returns>
        public String Validate()
        {
            if (String.IsNullOrWhiteSpace(DepartmentName))
                return "�� ������� ������������ �������������.";
            if (DepartmentHeadId == null || DepartmentHeadId.Value == 0)
                return "�� ������ ������������ �������������.";

            return "";
        }

        /// <summary>
        /// �������� ����������� �������� �������.
        /// ���������� ������ ������, ���� �������� ��������. ����� ���������� ����� ��������� ��� ������������.
        /// </summary>
        /// <returns></returns>
        public String CheckCanDelete()
        {
            return "";
        }
    }
}
