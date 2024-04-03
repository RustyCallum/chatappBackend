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

            if(Guid == message.Guid)
            {
                var returnMessage = new List<string>
                {
                UserName,
                UserId,
                message.MessageBody,
                message.RecieverId.ToString()
                };

                return Ok(returnMessage);
            }

            else
            {
                return BadRequest("Bad GUID");
            }

        }
    }
}
