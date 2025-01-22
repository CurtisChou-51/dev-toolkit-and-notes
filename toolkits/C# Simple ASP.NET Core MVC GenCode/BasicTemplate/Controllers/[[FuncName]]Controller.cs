using [[ProjName]].Models.[[FuncName]];
using [[ProjName]].Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace [[ProjName]].Controllers
{
    public class [[FuncName]]Controller : BaseController
    {
        private readonly I[[FuncName]]Service _service;

        public [[FuncName]]Controller(I[[FuncName]]Service service) : base()
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Index()
        {
            [[FuncName]]ViewModel vm = new [[FuncName]]ViewModel();
            vm.ResultModel = _service.Search(vm.ConditionModel);
            return View(vm);
        }

        /// <summary> 查詢 </summary>
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Search([[FuncName]]ViewModel vm, string submitBtn)
        {
            if (submitBtn == "Search")
                vm.ConditionModel.PageModel.Page = 1;
            vm.ResultModel = _service.Search(vm.ConditionModel);
            return PartialView("_Search", vm);
        }
    }
}
