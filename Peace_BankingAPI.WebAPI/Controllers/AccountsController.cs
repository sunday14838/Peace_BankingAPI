using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Peace_BankingAPI.Application.DTOs.Account;
using Peace_BankingAPI.Application.Features.Accounts.Commands;
using Peace_BankingAPI.Application.Features.Accounts.Queries;

namespace Peace_BankingAPI.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<AccountDto>> CreateAccount([FromBody] CreateAccountRequest request)
        {
            var command = new CreateAccountCommand
            {
                UserId = GetCurrentUserId(),
                AccountName = request.AccountName,
                Currency = request.Currency,
                AccountType = request.AccountType
            };

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAccount), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AccountDto>> GetAccount(Guid id)
        {
            var query = new GetAccountQuery 
            { 
                Id = id,
                RequestingUserId = GetCurrentUserId()
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyAccounts()
        {
            var query = new GetUserAccountsQuery
            {
                UserId = GetCurrentUserId()
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }


        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                throw new UnauthorizedAccessException("User ID not found in token");

            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("User ID in token is malformed");

            return userId;
        }
    }



}
