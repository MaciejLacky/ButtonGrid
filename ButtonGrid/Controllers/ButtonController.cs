using ButtonGrid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ButtonGrid.Controllers
{
    public class ButtonController : Controller
    {
        static List<ButtonModel> buttons = new List<ButtonModel>();
        Random random = new Random();
        const int GRID_SIZE = 25;
        public IActionResult Index()
        {
            if (buttons.Count < GRID_SIZE)
            {
                for (int i = 0; i < GRID_SIZE; i++)
                {
                    buttons.Add(new ButtonModel { Id = i, ButtonState = random.Next(4) });
                }
            }
           
           
            return View("Index", buttons);
        }

        public IActionResult HandleButtonClick(string buttonNumber)
        {
            int bn = int.Parse(buttonNumber);
            int sum = 0;

            
            buttons.ElementAt(bn).ButtonState = (buttons.ElementAt(bn).ButtonState + 1) % 4;
            for (int i = 0; i < GRID_SIZE; i++)
            {
                sum += buttons.ElementAt(i).ButtonState;
            }

            if (sum == 0 || sum == 25 || sum == 50 || sum ==75)
            {
                return View("Win", buttons);
            }
            else
            {
                return View("Index", buttons);
            }




        }

        public IActionResult ShowOneButton(int buttonNumber)
        {
            buttons.ElementAt(buttonNumber).ButtonState = (buttons.ElementAt(buttonNumber).ButtonState + 1) % 4;
            //1 render a button and save it to string
            string  buttonString = RenderRazorViewToString(this, "ShowOneButton", buttons.ElementAt(buttonNumber));
            //2 Generate a win or los string based on state of buttons array
            bool DidIWinYet = true;
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons.ElementAt(i).ButtonState != buttons.ElementAt(0).ButtonState)
                {
                    DidIWinYet = false;
                }
            }
            string messageString = "";
            if (DidIWinYet)
            {
                messageString = "<p>Congratulations. You WIN!!!</p>";
            }
            else
            {
                messageString = "<p>Play again</p>";
            }
            //3 Assembly a json string that has two parts button string html and win/loss message

            var package = new { part1 = buttonString, part2 = messageString };

            //4 send the json result
            return Json(package);



            //return PartialView(buttons.ElementAt(buttonNumber));
        }

        public static string RenderRazorViewToString(Controller controller, string viewName, object model = null)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                IViewEngine viewEngine =
                    controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as
                        ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );
                viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }




    }

    
}
