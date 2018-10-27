using System.Collections.Generic;
using System.ComponentModel;

namespace Modelo.Cadastros
{
    public class Departamento
    {
        [DisplayName("Código")]
        public long? DepartamentoID { get; set; }

        [DisplayName("Departamento")]
        public string Nome { get; set; }

        [DisplayName("Instituição")]
        public long? InstituicaoID { get; set; }

        public Instituicao Instituicao { get; set; }

        public virtual ICollection<Curso> Cursos { get; set; }
    }
}
