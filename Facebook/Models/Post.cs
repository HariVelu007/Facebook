using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Facebook.Models
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostID { get; set; }

        [MaxLength(50)]
        public int PostBy { get; set; }

        [MaxLength(2000)]
        public string PostContent { get; set; }

        [MaxLength(200)]
        public string PostImg { get; set; }        
        public DateTime PostedOn { get; set; }
    }
}
