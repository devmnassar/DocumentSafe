using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DocSafe.Model.Models
{
    public class Document
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime Added { get; set; } = DateTime.Now;
        public string Type { get; set; }
        public string? Note { get; set; }
        public string FilePath { get; set; }
        public string ExtractedText { get; set; }
        public string GeneratedDocumentName { get; set; }

        public string ApplicationUserId { get; set; }
        [ValidateNever]
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
