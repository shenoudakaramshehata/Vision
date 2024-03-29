﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Vision.Models
{
    [Table("FieldType")]
    public partial class FieldType
    {
        public FieldType()
        {
            AdTemplateConfigs = new HashSet<AdTemplateConfig>();
            AdTemplateConfigs = new HashSet<AdTemplateConfig>();

        }

        [Key]
        public int FieldTypeId { get; set; }
        [StringLength(150)]
        public string? FieldTypeTitle { get; set; }

        [InverseProperty("FieldType")]
        public virtual ICollection<AdTemplateConfig>? AdTemplateConfigs { get; set; }
        public virtual ICollection<BusinessTemplateConfig>? BusinessTemplateConfigs { get; set; }

    }
}