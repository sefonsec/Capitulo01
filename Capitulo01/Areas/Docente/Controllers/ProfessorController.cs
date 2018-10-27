using System.Collections.Generic;
using System.Linq;
using Capitulo01.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Capitulo01.Areas.Docente.Models;
using Capitulo01.Data.DAL.Cadastros;
using Microsoft.AspNetCore.Mvc;
using Capitulo01.Data.DAL.Docente;
using Modelo.Cadastros;
using Modelo.Docente;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Capitulo01.Areas.Docente.Controllers
{
    [Area("Docente")]
    public class ProfessorController : Controller
    {
        private readonly IESContext _context;
        private readonly ProfessorDAL professorDAL;
        private readonly InstituicaoDAL instituicaoDAL;
        private readonly CursoDAL cursoDAL;
        private readonly DepartamentoDAL departamentoDAL;
        
        public ProfessorController(IESContext context)
        {
            _context = context;
            professorDAL = new ProfessorDAL(context);
            instituicaoDAL = new InstituicaoDAL(context);
            cursoDAL = new CursoDAL(context);
            departamentoDAL = new DepartamentoDAL(context);
        }
        
        public async Task<IActionResult> Index()
        {
            return View(await professorDAL.ObterProfessoresClassificadosPorNome().ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome")] Professor professor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await professorDAL.GravarProfessor(professor);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Não foi possível os dados.");
            }          

            return View(professor);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            ViewResult visaoProfessor = (ViewResult)await ObterVisaoProfessorPorId(id);            

            return visaoProfessor;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("Nome")] Professor professor)
        {
            if (id != professor.ProfessorID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await professorDAL.GravarProfessor(professor);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProfessorExists(professor.ProfessorID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(professor);
        }

        public async Task<IActionResult> Details(long? id)
        {
            return await ObterVisaoProfessorPorId(id);
        }

        private async Task<IActionResult> ObterVisaoProfessorPorId(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var professor = await professorDAL.ObterProfessorPorId((long)id);

            if (professor == null)
            {
                return NotFound();
            }

            return View(professor);
        }

        private async Task<bool> ProfessorExists(long? id)
        {
            return await professorDAL.ObterProfessorPorId((long)id) != null;
        }

        public void PrepararViewBags(List<Instituicao> instituicoes, List<Departamento> departamentos,
            List<Curso> cursos, List<Professor> professores)
        {
            //instituicoes.Insert(0, new Instituicao() { InstituicaoID = 0, Nome = "Selecione a Instituição"});
            ViewBag.Instituicoes = instituicoes;

            //departamentos.Insert(0, new Departamento() { DepartamentoID = 0, Nome = "Selecione o Departamento"});
            ViewBag.Departamentos = departamentos;

            //cursos.Insert(0, new Curso() { CursoID = 0, Nome = "Selecione o Curso"});
            ViewBag.Cursos = cursos;

            //professores.Insert(0, new Professor() { ProfessorID = 0, Nome = "Selecione o Professor"});
            ViewBag.Professores = professores;
        }

        [HttpGet]
        public IActionResult AdicionarProfessor()
        {
            PrepararViewBags(instituicaoDAL.ObterInstituicoesClassificadasPorNome().ToList(), 
                new List<Departamento>().ToList(),
                new List<Curso>().ToList(),
                new List<Professor>().ToList());

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdicionarProfessor([Bind("InstituicaoID, DepartamentoID, CursoID, ProfessorID")] AdicionarProfessorViewModel model)
        {
            if (model.InstituicaoID == 0 || model.DepartamentoID == 0 || model.CursoID == 0 || model.ProfessorID == 0)
            {
                ModelState.AddModelError("", "É preciso selecionar todos os dados");
            }
            else
            {
                cursoDAL.RegistrarProfessor((long)model.CursoID, (long)model.ProfessorID);

                RegistrarProfessoresNaSessao((long)model.CursoID, (long)model.ProfessorID);

                PrepararViewBags(instituicaoDAL.ObterInstituicoesClassificadasPorNome().ToList(),
                    departamentoDAL.ObterDepartamentoPorInstituicao((long)model.InstituicaoID).ToList(),
                    cursoDAL.ObterCursosPorDepartamento((long)model.DepartamentoID).ToList(),
                    cursoDAL.ObterProfessoresForaDoCurso((long)model.CursoID).ToList());
            }

            return View(model);
        }

        public void RegistrarProfessoresNaSessao(long cursoID, long professorID)
        {
            var cursoProfessor = new CursoProfessor()
            {
                ProfessorID = professorID,
                CursoID = cursoID
            };

            List<CursoProfessor> cursosProfessor = new List<CursoProfessor>();

            string cursosProfessoresSession = HttpContext.Session.GetString("cursosProfessores");

            if (cursosProfessoresSession != null)
            {
                cursosProfessor = JsonConvert.DeserializeObject<List<CursoProfessor>>(cursosProfessoresSession);
            }

            cursosProfessor.Add(cursoProfessor);

            HttpContext.Session.SetString("cursosProfessores", JsonConvert.SerializeObject(cursosProfessor));
        }

        public IActionResult VerificarUltimosRegistros()
        {
            var cursosProfessor = professorDAL.ObterCursosProfessores().ToList();

            if (cursosProfessor == null)
            {
                cursosProfessor = new List<CursoProfessor>();
            }                      

            string cursosProfessoresSession = HttpContext.Session.GetString("cursosProfessores");

            if (cursosProfessoresSession != null)
            {
                cursosProfessor = JsonConvert.DeserializeObject<List<CursoProfessor>>(cursosProfessoresSession);                
            }

            return View(cursosProfessor);
        }

        public JsonResult ObterDepartamentosPorInstituicao(long actionID)
        {
            var departamentos = departamentoDAL.ObterDepartamentoPorInstituicao(actionID).ToList();
            return Json(new SelectList(departamentos, "DepartamentoID", "Nome"));
        }

        public JsonResult ObterCursosPorDepartamento(long actionID)
        {
            var cursos = cursoDAL.ObterCursosPorDepartamento(actionID).ToList();
            return Json(new SelectList(cursos, "CursoID", "Nome"));
        }

        public JsonResult ObterProfessoresForaDoCurso(long actionID)
        {
            var professores = cursoDAL.ObterProfessoresForaDoCurso(actionID).ToList();
            return Json(new SelectList(professores, "ProfessorID", "Nome"));
        }
    }
}