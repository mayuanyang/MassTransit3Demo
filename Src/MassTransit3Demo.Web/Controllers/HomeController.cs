using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using MassTransit;
using MassTransit3Demo.Messages;
using MassTransit3Demo.Messages.Commands;
using MassTransit3Demo.Messages.Events;

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

        public async Task<ActionResult> SendReversal(Guid transactionId)
        {
            await _bus.Publish(new TransactionReversedEvent {TransactionId = transactionId});
            return View("Index");
        }

        public async Task<ActionResult> SendReversalAck(Guid transactionId)
        {
            await _bus.Publish(new TransactionAcknowledgedEvent() { TransactionId = transactionId });
            return View("Index");
        }

        public async Task<ActionResult> SendV1()
        {
            await _bus.Publish(new OrderPlacedEvent() {OrderAmount = 100});
            return View("Index");
        }

        public async Task<ActionResult> SendV2()
        {
            await _bus.Publish(new OrderPlacedEventV2() { OrderAmount = 100, Postage = 20});
            return View("Index");
        }
    }
}
