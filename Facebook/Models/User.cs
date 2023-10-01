using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Facebook.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [MaxLength(50)]
        public string UserName { get; set; }

        [MaxLength(10)]
        public string Gender { get; set; }

        [MaxLength(100)]
        public string Address { get; set; }
        public DateTime DOB { get; set; }

        [MaxLength(50)]
        public string Password { get; set; }

        [MaxLength(200)]
        public string Profile { get; set; }

        [MaxLength(10)]
        public string Mobile { get; set; }

        [MaxLength(20)]
        public string Character { get; set; }
    }
}
