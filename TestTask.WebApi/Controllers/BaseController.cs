using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TestTask.WebApi.Controllers
{
    [ApiController]
    [Route("notes/[controller]/[action]")]
    public abstract class BaseController : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    }
}
