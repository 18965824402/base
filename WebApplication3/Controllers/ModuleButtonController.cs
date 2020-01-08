﻿using System.Threading.Tasks;
using dotNET.Application;
using Microsoft.AspNetCore.Mvc;
using dotNET.Web.Host.Framework;
using dotNET.Application.Sys;
using dotNET.Web.Host.Model;
using dotNET.Core;
using dotNET.Domain.Entities.Sys;
using dotNET.Dto;
using Microsoft.Extensions.Configuration;
// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860
namespace dotNET.Web.Host.Controllers
{
    [IgnoreAuthorize]
    public class ModuleButtonController : CustomController
    {
        public IModuleButtonApp ModuleButtonApp { get; set; }

  

        // 列表
        // GET: /<controller>/
        public async Task<IActionResult> Index(ModuleButtonOption option)
        {
            //返回json
            if (Request.IsAjaxRequest())
            {
                var modules = await ModuleButtonApp.GetSaasModuleListAsync(option);
                return Json(new { rows = modules });
            }
            ViewData["ModuleId"] = option.ModuleId;
            ViewData["ParentId"] = option.ParentId;
            return View();
        }

        #region 添加
        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        public IActionResult Create(long moduleId, long parentId)
        {
            ViewData["ModuleId"] = moduleId;
            ViewData["ParentId"] = parentId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ModuleButtonModel model, long moduleId, long parentId)
        {
            if (!ModelState.IsValid)
            {
                return Json(R.Err("数据验证失败")) ;
            }
            var module =model.MapTo< ModuleButton>();

            module.ParentId = parentId;
            module.ModuleId = moduleId;
            var r = await ModuleButtonApp.CreateAsync(module);

            return Json(r);
        }
        #endregion

        #region 删除
        [HttpPost]
        public async Task<IActionResult> Delete(long Id)
        {
            var r = await ModuleButtonApp.DeleteAsync(Id);
            return Json(r);
        }
        #endregion

        #region 修改
        public async Task<IActionResult> Edit(long Id)
        {
            ModuleButton module = await ModuleButtonApp.GetAsync(Id);
            if (module == null)
            {
                return NotFind();
            }
            ViewData["Model"] = JsonHelper.SerializeObject(module, false, true);//json 名称用驼峰结构输出
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ModuleButtonModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(R.Err(GetErrorFromModelStateStr()));
               
            }

            var m = await ModuleButtonApp.GetAsync(model.Id);
            if (m == null)
            {
                return Json(R.Err("数据不存在或已被删除"));
             
            }

            m =model.MapToMeg<ModuleButtonModel, ModuleButton>(m);
            var r = await ModuleButtonApp.UpdateAsync(m);

            return Json(r);
        }
        #endregion
    }
}