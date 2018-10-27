﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Capitulo01.Data;
using Modelo.Cadastros;
using Microsoft.EntityFrameworkCore;
using Capitulo01.Data.DAL.Cadastros;
using Microsoft.AspNetCore.Authorization;

namespace Capitulo01.Areas.Cadastros.Controllers
{
    [Area("Cadastros")]
    [Authorize]
    public class InstituicaoController : Controller
    {
        private readonly IESContext _context;
        private readonly InstituicaoDAL instituicaoDAL;

        public InstituicaoController(IESContext context)
        {
            _context = context;
            instituicaoDAL = new InstituicaoDAL(context);
        }

        public async Task<IActionResult> Index()
        {
            return View(await instituicaoDAL.ObterInstituicoesClassificadasPorNome().ToListAsync());
        }

        // GET: Instituição/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome, Endereco")] Instituicao instituicao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await instituicaoDAL.GravarInstituicao(instituicao);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Não foi possível os dados.");                
            }

            return View(instituicao);
        }

        // GET: Instituição/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {          
            return await ObterVisaoInstituicaoPorId(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("InstituicaoID, Nome, Endereco")] Instituicao instituicao)
        {
            if (id != instituicao.InstituicaoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {                   
                    await instituicaoDAL.GravarInstituicao(instituicao);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (! await InstituicaoExists(instituicao.InstituicaoID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(instituicao);
        }

        public async Task<IActionResult> Details(long? id)
        {           
            return await ObterVisaoInstituicaoPorId(id);
        }

        // GET: Instituição/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {       
            return await ObterVisaoInstituicaoPorId(id);
        }

        // POST: Instituição/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>  DeleteConfirmed(long? id)
        {
            var instituicao = await instituicaoDAL.EliminarInstituicaoPorId((long) id);

            TempData["Message"] = "Instituição " + instituicao.Nome.ToUpper() + " foi removida";
                   
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> ObterVisaoInstituicaoPorId(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instituicao = await instituicaoDAL.ObterInstituicaoPorId((long) id);

            if (instituicao == null)
            {
                return NotFound();
            }

            return View(instituicao);
        }

        private async Task<bool> InstituicaoExists(long? id)
        {
            return await instituicaoDAL.ObterInstituicaoPorId((long) id) != null;
        }
    }
}