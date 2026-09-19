using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Peace_BankingAPI.Application.DTOs.Transaction;
using Peace_BankingAPI.Application.Features.Transactions.Commands;

namespace Peace_BankingAPI.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("transfer")]
        [ProducesResponseType(typeof(TransferResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransferResultDto>> Transfer([FromBody] TransferCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
