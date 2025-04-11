using System.Text;
using System.Text.Json;
using FirstSparrow.Application.Features.Restaurant.LoginCommand;
using FirstSparrow.Application.Features.Restaurant.RegisterRestaurantCommand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSec.Cryptography;

namespace FirstSparrow.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class RestaurantsController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand request, CancellationToken cancellationToken)
    {
        LoginResponse response = await mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRestaurantCommand request, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(request, cancellationToken));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh()
    {
        return Ok();
    }

    [HttpPost("interaction")]
    public async Task<IActionResult> Interaction()
    {
        Request.EnableBuffering();

        // Read the body stream
        using var reader = new StreamReader(Request.Body);

        string body = await reader.ReadToEndAsync();

        // Reset position so the body can be read again by other middleware/controllers
        Request.Body.Position = 0;
        string publicKey = "4000db56ad5b5e4a35be8a0e93c56886d97ccee1154bbd3884546edf3627aa98";
        string signature = Request.Headers["X-Signature-Ed25519"];
        string timestamp = Request.Headers["X-Signature-Timestamp"];

        if (!IsValidSignature(signature, timestamp, body, publicKey))
        {
            return Unauthorized("Invalid request");
        }
        using JsonDocument json = JsonDocument.Parse(body);
        int type = json.RootElement.GetProperty("type").GetInt32();

        if (type == 1)
        {
            // PING
            return Ok(new { type = 1 });
        }

        return Ok();
    }

    private static bool IsValidSignature(string signature, string timestamp, string body, string pubKey)
    {
        try
        {
            var algorithm = SignatureAlgorithm.Ed25519;
            var publicKeyBytes = GetBytesFromHexString(pubKey);
            var publicKey = PublicKey.Import(algorithm, publicKeyBytes, KeyBlobFormat.RawPublicKey);

            var data = Encoding.UTF8.GetBytes(timestamp + body);
            var signatureBytes = GetBytesFromHexString(signature);

            return algorithm.Verify(publicKey, data, signatureBytes);
        }
        catch
        {
            return false;
        }
    }

    private static byte[] GetBytesFromHexString(string hex)
    {
        return Convert.FromHexString(hex);
    }
}

public class InteractionData
{
    public int Type { get; set; }
}