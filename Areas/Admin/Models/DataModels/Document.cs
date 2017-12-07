namespace DocWorkflow.Areas.Admin.Models.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    /// <summary>
    /// ��������
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
        /// ������������� ����� ������ �������
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
        /// �������� ������������ ������.
        /// ���������� ������ ������, ���� ������ ���������. ����� ���������� ���� ��������� ��� ������������.
        /// </summary>
        /// <returns></returns>
        public String Validate()
        {
            if (DocTypeId == 0)
                return "�� ������ ��� ���������.";
            if (String.IsNullOrWhiteSpace(DocName))
                return "�� ������� ������������ ���������.";
            if (SignerId == 0)
                return "�� ������ ���������.";
            if (ExecutorId == 0)
                return "�� ������ �����������.";
            if (DateExecution == DateTime.MinValue)
                return "�� ������ ���� ����������.";

            return "";
        }


        /// <summary>
        /// �������� ����������� �������� �������.
        /// ���������� ������ ������, ���� �������� ��������. ����� ���������� ����� ��������� ��� ������������.
        /// </summary>
        /// <returns></returns>
        public String CheckCanDelete()
        {
            if (DocStatusId == 1)
                return "";

            if (Authentication.UserIsSysadmin)
                return "";

            return "���� �������� ����� ���� ������ ������ ��������� ���������������, �.�. �������� �����������.";
        }


        /// <summary>
        /// ������� ����������� �������������� ���������
        /// </summary>
        public bool CanEdit
        {
            get
            {
                return Authentication.User.UserId == CreatorId
                    && (DocStatusId == 1 || DocStatusId == 4); //�������� ��� �������
            }
        }


        /// <summary>
        /// ������� ����������� ��������� ��� ��������
        /// </summary>
        public bool CanSaveAsDraft
        {
            get
            {
                return Authentication.User.UserId == CreatorId
                    && (DocStatusId == 1 || DocStatusId == 4); //�������� ��� �������
            }
        }


        /// <summary>
        /// ������� ����������� ���������� ���������
        /// </summary>
        public bool CanPublication
        {
            get
            {
                return Authentication.User.UserId == CreatorId
                    && (DocStatusId == 1 || DocStatusId==4); //�������� ��� �������
            }
        }

        
        /// <summary>
        /// ������� ����������� ���������� ���������
        /// </summary>
        public bool CanSign
        {
            get
            {
                return Authentication.User.UserId == SignerId
                    && DocStatusId == 2; //�����������
            }
        }


        /// <summary>
        /// ������� ����������� ������� ��������� ��� ������������
        /// </summary>
        public bool CanDone
        {
            get
            {
                return Authentication.User.UserId == ExecutorId
                    && DocStatusId == 3; //��������
            }
        }


        /// <summary>
        /// ������� ����������� ������ ���������
        /// </summary>
        public bool CanCancel
        {
            get
            {
                if (Authentication.User.UserId == CreatorId)
                {
                    if (DocStatusId == 2 || DocStatusId == 3) //����������� ��� ��������
                        return true;
                }

                if (Authentication.User.UserId == SignerId)
                {
                    if (DocStatusId == 2) //�����������
                        return true;
                }

                if (Authentication.User.UserId == ExecutorId)
                {
                    if (DocStatusId == 3) //��������
                        return true;
                }

                return false;
            }
        }


        /// <summary>
        /// ���������� ���������
        /// </summary>
        /// <returns></returns>
        public String DoPublication()
        {
            if (!CanPublication)
                return "�������� �� ����� ���� �����������.";

            DocStatusId = 2; //�����������
            CancellationNote = "";

            return "";
        }


        /// <summary>
        /// ���������� ���������
        /// </summary>
        /// <returns></returns>
        public String DoSign()
        {
            if (!CanSign)
                return "�������� �� ����� ���� �������� ������� �������������.";

            DocStatusId = 3; //��������

            return "";
        }


        /// <summary>
        /// ������� ��������� ��� ������������
        /// </summary>
        /// <returns></returns>
        public String SetDone()
        {
            if (!CanDone)
                return "�������� �� ����� ���� ������� ��� �����������.";

            DocStatusId = 5; //��������
            DateDone = DateTime.Today;

            return "";
        }


        /// <summary>
        /// ������ ���������
        /// </summary>
        /// <returns></returns>
        public String SetCancelled()
        {
            if (!CanCancel)
                return "�������� �� ����� ���� �������.";

            if (String.IsNullOrWhiteSpace(CancellationNote))
                return "���������� ������� �����������.";

            DocStatusId = 4; //�������

            return "";
        }
    }
}
