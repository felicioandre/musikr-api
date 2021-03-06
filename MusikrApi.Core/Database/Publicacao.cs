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
    
    public partial class Publicacao
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Publicacao()
        {
            this.PublicacaoLike = new HashSet<PublicacaoLike>();
        }
    
        public int PublicacaoId { get; set; }
        public string Texto { get; set; }
        public string Video { get; set; }
        public string Imagem { get; set; }
        public System.DateTime CriadoEm { get; set; }
        public bool Ativo { get; set; }
        public Nullable<int> UsuarioFK { get; set; }
        public Nullable<int> BandaFK { get; set; }
    
        public virtual Banda Banda { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PublicacaoLike> PublicacaoLike { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
