using BuildingBlocks.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using ApiGateway.Models;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly TransactionEngineOnboarding.TransactionEngineOnboardingClient _grpcClient;

    public CustomersController(
        TransactionEngineOnboarding.TransactionEngineOnboardingClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCustomerRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var grpcRequest = new RegisterCustomerRequest
        {
            FullName = request.FullName,
            NationalId = request.NationalId,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth.ToString("yyyy-MM-dd")
        };

        try
        {
            var response = await _grpcClient.RegisterCustomerAsync(grpcRequest);

            return Ok(new
            {
                response.CustomerId,
                response.Status,
                response.Message
            });
        }
        catch (RpcException ex)
        {
            return StatusCode(500, ex.Status.Detail);
        }
    }
}