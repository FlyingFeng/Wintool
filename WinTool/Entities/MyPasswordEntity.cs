using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTool.Entities
{
    [Table("MyPassword")]
    internal class MyPasswordEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column("LoginName")]
        public string LoginName { get; set; } = string.Empty;

        [Column("Password")]
        public byte[] Password { get; set; } = Array.Empty<byte>();

        [Column("Url")]
        public string Url { get; set; } = string.Empty;

        [Column("Remark")]
        public string Remark { get; set; } = string.Empty;


        [Column("CreatedTime")]
        public DateTime? CreatedTime { get; set; }

    }
}
