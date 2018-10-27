using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Modelo.Docente
{
    public class Professor
    {
        [DisplayName("Código")]
        public long? ProfessorID { get; set; }

        [DisplayName("Nome")]
        public string Nome { get; set; }

        public virtual ICollection<CursoProfessor> CursosProfessores { get; set; }
    }
}
