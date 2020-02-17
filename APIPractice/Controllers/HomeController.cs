using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIPractice.Models;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace APIPractice.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        CardController cardController = new CardController();
        //public async Task<IActionResult> Index()
        //{
        //    string apiResponse = "";
        //    using (var httpClient = new HttpClient())
        //    {
        //        using (var response = await httpClient.GetAsync("https://deckofcardsapi.com/api/deck/new/"))
        //        {
        //            apiResponse = await response.Content.ReadAsStringAsync();

        //            var jsonDocument = JsonDocument.Parse(apiResponse);

        //            var deck_id = jsonDocument.RootElement.GetProperty("deck_id").GetString();
        //            //var value = jsonDocument.RootElement.GetProperty("value").GetString();
        //            //var image = jsonDocument.RootElement.GetProperty("image").GetString();
        //            //var code = jsonDocument.RootElement.GetProperty("code").GetString();
        //            //var suit = jsonDocument.RootElement.GetProperty("suit").GetString();

        //        }
        //    }

        public async Task<IActionResult> Index()
        {
            var deckid = await cardController.GetDeck();
            HttpContext.Session.SetString("deckid", deckid);
            //var sessionDeckId = HttpContext.Session.GetString("deckid");
            return View();
        }

      public async Task<IActionResult> GetCards()
        {
           
            List<Card> cardList = await cardController.GetCards(HttpContext.Session.GetString("deckid"), 5);
            return View(cardList);
        }
        public async Task<IActionResult> GetMoreCards(string[] isChecked)
        {
            int total = 5 - isChecked.Length;

            if(total == 0)
            {
                List<Card> cardList = await cardController.GetCurrentHand(HttpContext.Session.GetString("deckid"));
                return View("GetCards",cardList);
            }
            else
            {
                List<Card> cardList = await cardController.DrawMoreCards(HttpContext.Session.GetString("deckid"), isChecked, total);
                return View("GetCards", cardList);
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
