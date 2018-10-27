using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;
using System.Linq;
using System.Threading.Tasks;

namespace Capitulo01.Data.DAL.Cadastros
{
    public class DepartamentoDAL
    {
        private IESContext _context;        

        public DepartamentoDAL(IESContext context)
        {
            _context = context;
        }

        public IQueryable<Departamento> ObterDepartamentosClassificadosPorNome()
        {
            var departamentos = _context.Departamentos.OrderBy(b => b.Nome);
            _context.Instituicoes.ToList();

            return departamentos;
        }

        public IQueryable<Departamento> ObterDepartamentoPorInstituicao(long instituicaoID)
        {
            var departamentos = _context.Departamentos.Where(d => d.InstituicaoID == instituicaoID).OrderBy(d => d.Nome);

            return departamentos;
        }

        public async Task<Departamento> ObterDepartamentoPorId(long id)
        {
            var departamento = await _context.Departamentos.SingleOrDefaultAsync(m => m.DepartamentoID == id);

            _context.Instituicoes.Where(i => departamento.InstituicaoID == i.InstituicaoID).Load();

            return departamento;
        }

        public async Task<Departamento> GravarDepartamento(Departamento departamento)
        {
            if (departamento.DepartamentoID == null)
            {
                _context.Departamentos.Add(departamento);
            }
            else
            {
                _context.Update(departamento);
            }

            await _context.SaveChangesAsync();
            return departamento;
        }

        public async Task<Departamento> EliminarDepartamentoPorId(long id)
        {
            var departamento = await ObterDepartamentoPorId(id);

            _context.Departamentos.Remove(departamento);

            await _context.SaveChangesAsync();

            return departamento;
        }
    }
}
