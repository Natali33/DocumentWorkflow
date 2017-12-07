namespace DocWorkflow.Areas.Admin.Models.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    /// <summary>
    /// ������������� ����
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
