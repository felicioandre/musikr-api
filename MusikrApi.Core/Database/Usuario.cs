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
    
    public partial class Usuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuario()
        {
            this.BandaFollow = new HashSet<BandaFollow>();
            this.BandaUsuario = new HashSet<BandaUsuario>();
            this.Publicacao = new HashSet<Publicacao>();
            this.PublicacaoLike = new HashSet<PublicacaoLike>();
            this.UsuarioFollow = new HashSet<UsuarioFollow>();
            this.UsuarioFollow1 = new HashSet<UsuarioFollow>();
            this.VagaCandidato = new HashSet<VagaCandidato>();
            this.UsuarioGeneroMusical = new HashSet<UsuarioGeneroMusical>();
            this.UsuarioInstrumento = new HashSet<UsuarioInstrumento>();
            this.Vaga = new HashSet<Vaga>();
        }
    
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public System.DateTime CriadoEm { get; set; }
        public string SenhaTemporaria { get; set; }
        public Nullable<System.DateTime> DataNascimento { get; set; }
        public string FotoPerfil { get; set; }
        public string Sexo { get; set; }
        public bool EhVocalista { get; set; }
        public bool Ativo { get; set; }
        public Nullable<int> FirstStep { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BandaFollow> BandaFollow { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BandaUsuario> BandaUsuario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Publicacao> Publicacao { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PublicacaoLike> PublicacaoLike { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsuarioFollow> UsuarioFollow { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsuarioFollow> UsuarioFollow1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VagaCandidato> VagaCandidato { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsuarioGeneroMusical> UsuarioGeneroMusical { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsuarioInstrumento> UsuarioInstrumento { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vaga> Vaga { get; set; }
    }
}
