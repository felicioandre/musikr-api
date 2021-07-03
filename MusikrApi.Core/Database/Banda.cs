//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MusikrApi.Core.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Banda
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Banda()
        {
            this.BandaFollow = new HashSet<BandaFollow>();
            this.BandaGeneroMusical = new HashSet<BandaGeneroMusical>();
            this.BandaUsuario = new HashSet<BandaUsuario>();
            this.Vaga = new HashSet<Vaga>();
            this.Publicacao = new HashSet<Publicacao>();
        }
    
        public int BandaId { get; set; }
        public string Nome { get; set; }
        public string Logotipo { get; set; }
        public System.DateTime CriadoEm { get; set; }
        public bool Ativo { get; set; }
        public string Foto { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BandaFollow> BandaFollow { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BandaGeneroMusical> BandaGeneroMusical { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BandaUsuario> BandaUsuario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vaga> Vaga { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Publicacao> Publicacao { get; set; }
    }
}
