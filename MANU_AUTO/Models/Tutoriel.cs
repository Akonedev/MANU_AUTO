using System;
using System.Collections.Generic;

namespace MANU_AUTO.Models
{
    public partial class Tutoriel
    {
        public Tutoriel()
        {
            Ids = new HashSet<Categorie>();
        }

        public int Id { get; set; }
        public string Titre { get; set; } = null!;
        public DateTime Dcc { get; set; }
        public string Contenu { get; set; } = null!;
        public string VideoLink { get; set; } = null!;
        public DateTime Dml { get; set; }

        public virtual ICollection<Categorie> Ids { get; set; }
    }
}
