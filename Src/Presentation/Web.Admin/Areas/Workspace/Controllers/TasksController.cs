﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using TaskoMask.Web.Common.Controllers;
using System.Threading.Tasks;
using TaskoMask.Web.Common.Filters;
using TaskoMask.Application.Workspace.Tasks.Services;

namespace TaskoMask.Web.Admin.Areas.Workspace.Controllers
{
    [Authorize]
    [Area("Workspace")]
    public class TasksController : BaseMvcController
    {
        #region Fields

        private readonly ITaskService _taskService;


        #endregion

        #region Ctor

        public TasksController(ITaskService taskService, IMapper mapper) : base(mapper)
        {
            _taskService = taskService;
        }

        #endregion

        #region Public Methods



        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var taskQueryResult = await _taskService.SearchAsync(page: 1, recordsPerPage: recordsPerPage, term: "");

            #region ViewBags

            ViewBag.PageSize = taskQueryResult.Value.PageNumber;
            ViewBag.CurrentPage = 1;
            ViewBag.TotalItemCount = taskQueryResult.Value.TotalCount;

            #endregion

            return View(taskQueryResult);
        }



        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Search(int page = 1, string term = "")
        {
            var taskQueryResult = await _taskService.SearchAsync(page: page, recordsPerPage: recordsPerPage, term: term);

            if (!taskQueryResult.IsSuccess)
                return RedirectToErrorPage(taskQueryResult);

            #region ViewBags

            ViewBag.PageSize = taskQueryResult.Value.PageNumber;
            ViewBag.CurrentPage = page;
            ViewBag.TotalItemCount = taskQueryResult.Value.TotalCount;

            #endregion

            return PartialView("_ItemList", taskQueryResult.Value.Items);
        }



        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var taskQueryResult = await _taskService.GetDetailsAsync(id);

            return View(taskQueryResult);

        }





        #endregion

        #region Private Methods



        #endregion

    }
}
