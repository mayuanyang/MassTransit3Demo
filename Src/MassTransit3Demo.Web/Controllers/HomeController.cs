﻿using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using MassTransit;
using MassTransit3Demo.Messages;
using MassTransit3Demo.Messages.Commands;

namespace MassTransit3Demo.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBus _bus;
        private readonly IRequestClient<SimpleRequest, SimpleResonse> _requestClient;

        public HomeController(IBus bus, IRequestClient<SimpleRequest, SimpleResonse> requestClient)
        {
            _bus = bus;
            _requestClient = requestClient;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> SendRequest(string message)
        {
            var response = await _requestClient.Request(new SimpleRequest(message));
            TempData["Content"] = response.Result;
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> SendCommand(string message)
        {
            await _bus.Publish(new PrintToConsoleCommand(message));
            return RedirectToAction("Index");
        }
    }
}
