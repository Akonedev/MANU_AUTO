using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace MANU_AUTO.Models;

public partial class Categorie
{
    public Categorie()
    {
        IdTutoriels = new HashSet<Tutoriel>();
    }

    public int Id { get; set; }
    [Display(Name = "Libellé de la catégorie"), MinLength(5)]
    [Required(ErrorMessage = "Veuillez saisir un libellé pour la catégorie.")]
    public string Label { get; set; } = null!;

    public virtual ICollection<Tutoriel> IdTutoriels { get; set; }
}
