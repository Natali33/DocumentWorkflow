namespace DocWorkflow.Areas.Admin.Models.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    /// <summary>
    /// ��� ���������
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
