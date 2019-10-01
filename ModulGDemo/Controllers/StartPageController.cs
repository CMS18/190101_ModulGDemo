using System.Web.Mvc;
using ModulGDemo.Models.Pages;
using ModulGDemo.Models.ViewModels;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using System;
using System.Threading;
using System.Runtime.Caching;
using EPiServer.Logging;

namespace ModulGDemo.Controllers
{

    
    public class StartPageController : PageControllerBase<StartPage>
    {
        private ILogger logger = LogManager.GetLogger();

        [ContentOutputCache(Duration = 30)]
        public ActionResult Index(StartPage currentPage)
        {
            var model = PageViewModel.Create(currentPage);

            if (SiteDefinition.Current.StartPage.CompareToIgnoreWorkID(currentPage.ContentLink)) // Check if it is the StartPage or just a page of the StartPage type.
            {
                //Connect the view models logotype property to the start page's to make it editable
                var editHints = ViewData.GetEditHints<PageViewModel<StartPage>, StartPage>();
                editHints.AddConnection(m => m.Layout.Logotype, p => p.SiteLogotype);
                editHints.AddConnection(m => m.Layout.ProductPages, p => p.ProductPageLinks);
                editHints.AddConnection(m => m.Layout.CompanyInformationPages, p => p.CompanyInformationPageLinks);
                editHints.AddConnection(m => m.Layout.NewsPages, p => p.NewsPageLinks);
                editHints.AddConnection(m => m.Layout.CustomerZonePages, p => p.CustomerZonePageLinks);
            }


            // Demo av cache

            ObjectCache cache = MemoryCache.Default;


            // Kolla om värdet fanns sparat - hoppa över det som tar lång tid i så fall
            var value = cache.Get("TaskThatTakesLooongTimeToDo") as string;

            if (value == null)
            {
                // Hämta värde tar lång tid
                value = TaskThatTakesLooongTimeToDo();

                // Spara för framtiden

                var policy = new CacheItemPolicy
                {

                    AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(15)
                    //SlidingExpiration = new TimeSpan(0, 0, 30)
                };
                cache.Set("TaskThatTakesLooongTimeToDo", value, policy);
            }
            else
            {
                logger.Debug("TaskThatTakesLooongTimeToDo found in cache.");

            }



            model.LongTimeWork = value;



            return View(model);
        }


        public string TaskThatTakesLooongTimeToDo()
        {
            logger.Debug("TaskThatTakesLooongTimeToDo took 3 seconds.");

            // Simulera något som tar lång tid att göra med en Sleep.
            Thread.Sleep(3000);

            return DateTime.Now.ToString();
        }

    }
}
