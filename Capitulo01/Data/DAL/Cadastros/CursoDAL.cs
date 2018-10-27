using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;
using System.Linq;
using System.Threading.Tasks;
using Modelo.Docente;

namespace Capitulo01.Data.DAL.Cadastros
{
    public class CursoDAL
    {
        private IESContext _context;

        public CursoDAL(IESContext context)
        {
            _context = context;
        }

        public IQueryable<Curso> ObterCursosClassificadosPorNome()
        {
            var cursos = _context.Cursos.OrderBy(b => b.Nome);
            _context.Departamentos.ToList();

            return cursos;
        }

        public IQueryable<Curso> ObterCursosPorDepartamento(long departamentoID)
        {
            var cursos = _context.Cursos.Where(c => c.DepartamentoID == departamentoID).OrderBy(d => d.Nome);

            return cursos;
        }

        public async Task<Curso> ObterCursoPorId(long id)
        {
            var curso = await _context.Cursos.SingleOrDefaultAsync(m => m.CursoID == id);

            _context.Departamentos.Where(i => curso.DepartamentoID == i.DepartamentoID).Load();

            return curso;
        }

        public async Task<Curso> GravarCurso(Curso curso)
        {
            if (curso.CursoID == null)
            {
                _context.Cursos.Add(curso);
            }
            else
            {
                _context.Update(curso);
            }

            await _context.SaveChangesAsync();
            return curso;
        }

        public async Task<Curso> EliminarCursoPorId(long id)
        {
            var curso = await ObterCursoPorId(id);

            _context.Cursos.Remove(curso);

            await _context.SaveChangesAsync();

            return curso;
        }

        public void RegistrarProfessor(long cursoID, long professorID)
        {
            if (cursoID > 0 || professorID > 0)
            {                          
                var curso = _context.Cursos.Where(c => c.CursoID == cursoID).Include(cp => cp.CursosProfessores).First();
                var professor = _context.Professores.Find(professorID);
                curso.CursosProfessores.Add(new CursoProfessor() { Curso = curso, Professor = professor });
                _context.SaveChanges();
            }
        }

        public IQueryable<Professor> ObterProfessoresForaDoCurso(long cursoID)
        {
            if (cursoID == 0)
            {
                return null;
            }

            var curso = _context.Cursos.Where(c => c.CursoID == cursoID).Include(cp => cp.CursosProfessores).First();
            var professoresDoCurso = curso.CursosProfessores.Select(cp => cp.ProfessorID).ToArray();
            var professoresForaDoCurso = _context.Professores.Where(p => !professoresDoCurso.Contains(p.ProfessorID));
            return professoresForaDoCurso;
        }
    }
}