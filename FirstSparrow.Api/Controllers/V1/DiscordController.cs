using System.Text;
using System.Text.Json;
using FirstSparrow.Api.Controllers.V1.Enums;
using FirstSparrow.Api.Controllers.V1.Models;
using FirstSparrow.Api.Settings;
using FirstSparrow.Application.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;

namespace FirstSparrow.Api.Controllers.V1;

[ApiController]
[Route("api/v1/discord")]
public class DiscordController(
    IOptions<DiscordSettings> discordSettings) : ControllerBase
{
    [HttpPost("interaction")]
    public async Task<IActionResult> Interaction()
    {
        string body = await ReadRequestBody();
        EnsureSignatureIsValid(body);
        DiscordInteractionRequest request = JsonSerializer.Deserialize<DiscordInteractionRequest>(body)!;

        IActionResult result = request.Type switch
        {
            DiscordInteractionType.PING => HandlePingRequest(request),
            _ => Ok()
        };
        return result;
    }

    private IActionResult HandlePingRequest(DiscordInteractionRequest request)
    {
        return Ok(new DiscordInteractionPingResponse()
        {
            Type = DiscordInteractionType.PING,
        });
    }

    private void EnsureEssentialHeadersExist(out string signature, out string timestamp)
    {
        string? signatureFromHeader = Request.Headers[discordSettings.Value.SignatureHeaderKey];
        string? timestampFromHeader = Request.Headers[discordSettings.Value.TimestampHeaderKey];

        if (signatureFromHeader == null || timestampFromHeader == null)
        {
            throw new AppException("Missing essential headers.", ExceptionCode.UNAUTHORIZED);
        }
        signature = signatureFromHeader;
        timestamp = timestampFromHeader;
    }

    private async Task<string> ReadRequestBody()
    {
        Request.EnableBuffering();
        using var reader = new StreamReader(Request.Body);
        string body = await reader.ReadToEndAsync();
        Request.Body.Position = 0;
        return body;
    }

    private void EnsureSignatureIsValid(string body)
    {
        try
        {
            EnsureEssentialHeadersExist(out string signature, out string timestamp);

            //Prepare for signature validation
            byte[] signatureBytes = Convert.FromHexString(signature);
            byte[] publicKeyBytes = Convert.FromHexString(discordSettings.Value.InteractionsPublicKey);
            byte[] messageBytes = Encoding.UTF8.GetBytes(timestamp + body);
            var publicKeyParam = new Ed25519PublicKeyParameters(publicKeyBytes, 0);
            ISigner signer = new Ed25519Signer();
            signer.Init(false, publicKeyParam);
            signer.BlockUpdate(messageBytes, 0, messageBytes.Length);

            //Validate signature
            EnsureSignatureIsValid(signer, signatureBytes);
        }
        catch (AppException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new AppException(ex, ExceptionCode.UNAUTHORIZED);
        }
    }

    private static void EnsureSignatureIsValid(ISigner signer, byte[] signatureBytes)
    {
        bool isValid = signer.VerifySignature(signatureBytes);
        if (!isValid)
        {
            throw new AppException("Signature verification failed.", ExceptionCode.UNAUTHORIZED);
        }
    }
}