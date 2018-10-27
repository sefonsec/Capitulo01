using System.Collections.Generic;
using System.ComponentModel;

namespace Modelo.Cadastros
{
    public class Instituicao
    {
        [DisplayName("Código")]
        public long? InstituicaoID { get; set; }

        [DisplayName("Instituição")]
        public string Nome { get; set; }

        [DisplayName("Endereço")]
        public string Endereco { get; set; }

        public virtual ICollection<Departamento> Departamentos { get; set; }
    }
}
