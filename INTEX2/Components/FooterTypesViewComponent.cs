using Microsoft.AspNetCore.Mvc;

namespace INTEX2.Components
{
    public class FooterTypesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
