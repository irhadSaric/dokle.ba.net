namespace testpocetni.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ruta : DbContext
    {
        public ruta()
            : base("name=Model1")
        {
        }

        public virtual DbSet<rute> rutes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<rute>()
                .Property(e => e.polaziste)
                .IsUnicode(false);

            modelBuilder.Entity<rute>()
                .Property(e => e.odrediste)
                .IsUnicode(false);

            modelBuilder.Entity<rute>()
                .Property(e => e.drzava)
                .IsUnicode(false);

            modelBuilder.Entity<rute>()
                .Property(e => e.nacinPlacanja)
                .IsUnicode(false);
        }
    }
}
