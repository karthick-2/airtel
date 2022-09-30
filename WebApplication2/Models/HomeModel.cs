using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Data.Entity;
using System.Linq;
using System.Xml.Linq;

namespace WebApplication2.Models
{
    public partial class HomeModel : DbContext
    {
        public HomeModel()
            : base("name=HomeModel")
        {
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
       
        public virtual DbSet<slider> sliders { get; set; }
       
        public virtual DbSet<webtbluser> Webtblusers { get; set; }
        public virtual DbSet<pagemodel> pagemodel { get; set; }
        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<slider>()
                .Property(e => e.slidername)
                .IsUnicode(false);

            modelBuilder.Entity<slider>()
                .Property(e => e.sliderimg)
                .IsUnicode(false);

            modelBuilder.Entity<slider>()
                .Property(e => e.redirecturl)
                .IsUnicode(false);
        }
    }
    [Table("__MigrationHistory")]
    public partial class C__MigrationHistory
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(150)]
        public string MigrationId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(300)]
        public string ContextKey { get; set; }

        [Required]
        public byte[] Model { get; set; }

        [Required]
        [StringLength(32)]
        public string ProductVersion { get; set; }
    }



    [Table("slider")]
    public partial class slider
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [StringLength(50)]
        public string slidername { get; set; }

        [StringLength(50)]
        public string sliderimg { get; set; }

        [StringLength(50)]
        public string redirecturl { get; set; }

        public int imageorder { get; set; }

        public bool isactive { get; set; }

        public bool isdelete { get; set; }

        [Column(TypeName = "date")]
        public DateTime? created { get; set; }

        [Column(TypeName = "date")]
        public DateTime? deleted { get; set; }
    }

    public class pagemodel

    {
        [Key]
        public int pageid { get; set; }
        public string pagename { get; set; }
        public string pagetitle { get; set; }
        public string pagetxt { get; set; }
        public bool isactive { get; set; }
        public bool isdelete { get; set; }

    }

    [Table("webtbluser")]
    public class webtbluser
    {
        public int id { get; set; }
        public int roleid { get; set; }
        public string username { get; set; }
        public string userpassword { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string emailid { get; set; }
        public string contactnumber { get; set; }
        public bool is_active { get; set; }
        public bool is_delete { get; set; }

        public DateTime created { get; set; }
        public DateTime modified { get; set; }
    }

    public class adminmodel
    {
        [Required()]
        [Display(Name = "Username")]
        public string username { get; set; }
        [Required()]
        [Display(Name = "Password")]
        public string password { get; set; }
    }

}
