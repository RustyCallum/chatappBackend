using chatappBackend.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace chatappBackend.Message.Create
{
    [Route("api/Message")]
    [ApiController]
    public class CreateMessage : ControllerBase
    {
        private readonly UserDataContext _context;

        public CreateMessage(UserDataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ObjectResult> Post(CreateMessageRequest message)
        {
            var JWTHandler = new JwtSecurityTokenHandler();
            var token = Request.Cookies["token"];
            var jsonToken = JWTHandler.ReadToken(token);
            var tokenDecrypted = jsonToken as JwtSecurityToken;

            var UserName = tokenDecrypted.Claims.First(claim => claim.Type == "UserName").Value;
            var UserId = tokenDecrypted.Claims.First(claim => claim.Type == "ClientId").Value;
            var Guid = tokenDecrypted.Claims.First(claim => claim.Type == "X-CSRF-TOKEN").Value;

            if (Guid == message.Guid)
            {
                if (_context.Users.Any(x => x.Id == message.SecondaryUserId))
                {
                    var returnMessage = new Messages
                    {
                        Id = new Guid(),
                        LoggedUserId = message.LoggedUserId,
                        SecondaryUserId = message.SecondaryUserId,
                        MessageBody = message.MessageBody,
                        DateCreated = DateTime.UtcNow
                    };

                    _context.Messages.Add(returnMessage);
                    await _context.SaveChangesAsync();

                    return Ok(returnMessage);
                }
                else
                {
                    return BadRequest("User doesn't exist");
                }
            }
            else
            {
                return BadRequest("Bad GUID");
            }

        }
    }
}
