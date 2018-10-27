using System.Linq;
using System.Threading.Tasks;
using Capitulo01.Data;
using Capitulo01.Data.DAL.Cadastros;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;

namespace Capitulo01.Areas.Cadastros.Controllers
{
    [Area("Cadastros")]
    [Authorize]
    public class CursoController : Controller
    {
        private readonly IESContext _context;
        private readonly CursoDAL cursoDAL;
        private readonly DepartamentoDAL departamentoDAL;

        public CursoController(IESContext context)
        {
            _context = context;
            cursoDAL = new CursoDAL(context);
            departamentoDAL = new DepartamentoDAL(context);
        }

        public async Task<IActionResult> Index()
        {
            return View(await cursoDAL.ObterCursosClassificadosPorNome().ToListAsync());
        }

        public IActionResult Create()
        {
            var departamentos = departamentoDAL.ObterDepartamentosClassificadosPorNome().ToList();
            departamentos.Insert(0, new Departamento() { DepartamentoID = 0, Nome = "Selecione o departamento" });
            ViewBag.Departamentos = departamentos;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome, DepartamentoID")] Curso curso)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await cursoDAL.GravarCurso(curso);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Não foi possível os dados.");               
            }

            return View(curso);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            ViewResult visaoCurso = (ViewResult)await ObterVisaoCursoPorId(id);

            var curso = (Curso)visaoCurso.Model;

            ViewBag.Departamentos = new SelectList(departamentoDAL.ObterDepartamentosClassificadosPorNome(), "DepartamentoID", "Nome", curso.DepartamentoID);

            return visaoCurso;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("CursoID, DepartamentoID, Nome")] Curso curso)
        {
            if (id != curso.DepartamentoID)
            {
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                try
                {
                    await cursoDAL.GravarCurso(curso);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CursoExists(curso.CursoID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Departamentos = new SelectList(departamentoDAL.ObterDepartamentosClassificadosPorNome());

            return View(curso);
        }

        public async Task<IActionResult> Details(long? id)
        {
            return await ObterVisaoCursoPorId(id);
        }

        private async Task<IActionResult> ObterVisaoCursoPorId(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await cursoDAL.ObterCursoPorId((long)id);

            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        private async Task<bool> CursoExists(long? id)
        {
            return await cursoDAL.ObterCursoPorId((long)id) != null;
        }
    }
}
