using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Modelo.Docente;

namespace Modelo.Cadastros
{
    public class Curso
    {
        [DisplayName("Código")]
        public long? CursoID { get; set; }

        [DisplayName("Curso")]
        public string Nome { get; set; }

        [DisplayName("Departamento")]
        public long? DepartamentoID { get; set; }

        public Departamento Departamento { get; set; }

        public virtual ICollection<CursoDisciplina> CursosDisciplinas { get; set; }
        public virtual ICollection<CursoProfessor> CursosProfessores { get; set; }
    }
}
