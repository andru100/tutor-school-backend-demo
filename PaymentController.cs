using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;

namespace PaymentController 
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
         

        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
    
        }

        [HttpPost("create")]
        public IActionResult Create()
        {
            var domain = _configuration["ALLOWED_CORS_ORIGIN"];
             var options = new SessionCreateOptions
            {
                UiMode = "embedded",
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                    Price = "price_1OxrllRpl1Mrfk4hxvGdQUfx",
                    Quantity = 1,
                  },
                },
                Mode = "subscription",
                ReturnUrl = domain + "/return?session_id={CHECKOUT_SESSION_ID}"
            };
            var service = new SessionService();
            Session session = service.Create(options);

            return Ok(new {clientSecret = session.RawJObject["client_secret"]});
        }
    

        [HttpGet("session-status")]
        public IActionResult SessionStatus([FromQuery] string session_id)
        {
            var sessionService = new SessionService();
            Session session = sessionService.Get(session_id);

            return Ok(new {status = session.RawJObject["status"],  customer_email = session.RawJObject["customer_details"]["email"]});
        }
        
    }
}