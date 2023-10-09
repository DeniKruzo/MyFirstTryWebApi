using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Notes.WebApi.Controllers
{
    [ApiController]
    [Route("api/[contoller]/[action]")]
    public abstract class BaseController : ControllerBase
    {
        //Для формирования команд 
        private IMediator _mediator;
        protected IMediator Mediator => 
            _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        internal Guid UserId => !User.Identity.IsAuthenticated
          ? Guid.Empty
          : Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    }
}
