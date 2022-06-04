using System;
using System.Collections.Generic;

namespace MANU_AUTO.Models
{
    public partial class Categorie
    {
        public Categorie()
        {
            IdTutoriels = new HashSet<Tutoriel>();
        }

        public int Id { get; set; }
        public string Label { get; set; } = null!;

        public virtual ICollection<Tutoriel> IdTutoriels { get; set; }
    }
}
