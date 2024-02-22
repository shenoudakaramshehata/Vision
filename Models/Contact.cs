using System;
using System.ComponentModel.DataAnnotations;

namespace Vision.Models
{
    public class Contact
    {
        public int ContactId { get; set; }
        [Required(ErrorMessage = "Reequired")]

        public string FullName { get; set; }
        [Required(ErrorMessage = "Required"), RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Not Valid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Reequired")]
        public string Message { get; set; }
        public DateTime? SendingDate { get; set; }
        
    }
}
