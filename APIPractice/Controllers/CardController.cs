using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using APIPractice.Models;
using Microsoft.AspNetCore.Mvc;

namespace APIPractice.Controllers
{
    public class CardController : Controller
    {
        private JsonDocument jDoc;
        public static Deck deck = new Deck();



        public async Task<string> GetDeck()
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1"))
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    jDoc = JsonDocument.Parse(stringResponse);

                }

            }
            return jDoc.RootElement.GetProperty("deck_id").GetString();
        }
        public async Task<List<Card>> GetCards(string deckid, int count)
        {
            string code = "";
            List<Card> cardList = new List<Card>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://deckofcardsapi.com/api/deck/{deckid}/draw/?count={count}"))
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    jDoc = JsonDocument.Parse(stringResponse);
                    var jsonList = jDoc.RootElement.GetProperty("cards");

                    for (int i = 0; i < jsonList.GetArrayLength(); i++)
                    {
                        if (i == 0)
                        {
                            code += jsonList[i].GetProperty("code").GetString();
                        }
                        else
                        {
                            code += $",{jsonList[i].GetProperty("code").GetString()}";
                        }

                    }
                    using (var newresponse = await httpClient.GetAsync($"https://deckofcardsapi.com/api/deck/{deckid}/pile/hand/add/?cards={code}"))
                    {

                        using (var nresponse = await httpClient.GetAsync($"https://deckofcardsapi.com/api/deck/{deckid}/pile/hand/list/"))
                        {
                            var sstringResponse = await nresponse.Content.ReadAsStringAsync();
                            var newjDoc = JsonDocument.Parse(sstringResponse);
                            var trying = newjDoc.RootElement.GetProperty("piles");
                            var nList = trying.GetProperty("hand");
                            var newjsonList = nList.GetProperty("cards");

                            for (int i = 0; i < newjsonList.GetArrayLength(); i++)
                            {

                                cardList.Add(new Card()
                                {
                                    image = newjsonList[i].GetProperty("image").GetString(),
                                    value = newjsonList[i].GetProperty("value").GetString(),
                                    suit = newjsonList[i].GetProperty("suit").GetString(),
                                    code = newjsonList[i].GetProperty("code").GetString()
                                }
                                );
                            }
                        }
                    }

                }



                return cardList;
            }
        }
        public async Task<List<Card>> GetCurrentHand(string deckid)
        {
            List<Card> cardList = new List<Card>();
            using (var httpClient = new HttpClient())
            {
                using (var nresponse = await httpClient.GetAsync($"https://deckofcardsapi.com/api/deck/{deckid}/pile/hand/list/"))
                {
                    var sstringResponse = await nresponse.Content.ReadAsStringAsync();
                    var newjDoc = JsonDocument.Parse(sstringResponse);
                    var trying = newjDoc.RootElement.GetProperty("piles");
                    var nList = trying.GetProperty("hand");
                    var newjsonList = nList.GetProperty("cards");

                    for (int i = 0; i < newjsonList.GetArrayLength(); i++)
                    {

                        cardList.Add(new Card()
                        {
                            image = newjsonList[i].GetProperty("image").GetString(),
                            value = newjsonList[i].GetProperty("value").GetString(),
                            suit = newjsonList[i].GetProperty("suit").GetString(),
                            code = newjsonList[i].GetProperty("code").GetString()
                        }
                        );
                    }

                }
                return cardList;
            }
        }
        //public async Task<List<Card>> GetCards(string deckid)
        //{
        //    deck.deckid = deckid;

        //    List<Card> cardList = new List<Card>();
        //    using (var httpClient = new HttpClient())
        //    {
        //        using (var response = await httpClient.GetAsync("https://deckofcardsapi.com/api/deck/" + deckid + "/draw/?count=5"))
        //        {
        //            var stringResponse = await response.Content.ReadAsStringAsync();
        //            jDoc = JsonDocument.Parse(stringResponse);
        //            var jsonList = jDoc.RootElement.GetProperty("cards");

        //            for (int i = 0; i < jsonList.GetArrayLength(); i++)
        //            {

        //                cardList.Add(new Card()
        //                {
        //                    image = jsonList[i].GetProperty("image").GetString(),
        //                    value = jsonList[i].GetProperty("value").GetString(),
        //                    suit = jsonList[i].GetProperty("suit").GetString(),
        //                    code = jsonList[i].GetProperty("code").GetString()
        //                }
        //                );
        //            }
        //            deck.currentCards = cardList;
        //        }
        //    }

        //    return cardList;
        //}
        public async Task<List<Card>> DrawMoreCards(string deckid, string[] ar, int total)
        {
            string code = "";
            List<Card> current = await GetCurrentHand(deckid);
            for(int i = 0; i < current.Count; i++)
            {
                if(!ar.Contains(current[i].code) && i == 0)
                {
                    code += current[i].code;
                }
                else if(!ar.Contains(current[i].code))
                    {
                    code += $",{current[i].code}";
                }
            }

           
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://deckofcardsapi.com/api/deck/{deckid}/pile/discard/add/?cards={code}"))
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    jDoc = JsonDocument.Parse(stringResponse);
                }
            }
            List<Card> cards = await GetCards(deckid, total);
            return cards;
        }
    }
}
