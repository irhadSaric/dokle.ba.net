namespace testpocetni.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class korisnik : DbContext
    {
        public korisnik()
            : base("name=korisnik")
        {
        }

        public virtual DbSet<Korisnik> korisnicis { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Korisnik>()
                .Property(e => e.ime)
                .IsUnicode(false);

            modelBuilder.Entity<Korisnik>()
                .Property(e => e.prezime)
                .IsUnicode(false);

            modelBuilder.Entity<Korisnik>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<Korisnik>()
                .Property(e => e.password)
                .IsUnicode(false);

            modelBuilder.Entity<Korisnik>()
                .Property(e => e.aktivan)
                .IsUnicode(false);

            modelBuilder.Entity<Korisnik>()
                .Property(e => e.profilna)
                .IsUnicode(false);

            modelBuilder.Entity<Korisnik>()
                .Property(e => e.mjestoStanovanja)
                .IsUnicode(false);

            modelBuilder.Entity<Korisnik>()
                .Property(e => e.brojTelefona)
                .IsUnicode(false);
        }
    }
}
