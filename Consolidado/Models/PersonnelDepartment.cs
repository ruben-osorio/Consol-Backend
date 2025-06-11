using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Consolidado.Models
{
  
    public class PersonnelDepartment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string DeptCode { get; set; }

        [Required]
        [StringLength(100)]
        public required string DeptName { get; set; }

        public bool IsDefault { get; set; }

        public int? CompanyId { get; set; }

        public int? ParentDeptId { get; set; }
    }
} 