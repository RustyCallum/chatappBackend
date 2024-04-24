using chatappBackend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace chatappBackend.Message.GetListOf10
{
    [Route("api/Messages/{secondaryUserId}")]
    [ApiController]
    public class GetListOf10Messages : ControllerBase
    {
        private UserDataContext _context;
        private List<Messages> _messagesList;

        public GetListOf10Messages(UserDataContext context)
        {
            _context = context;
            _messagesList = new List<Messages>();
        }

        [HttpGet]
        public async Task<ObjectResult> Get(Guid secondaryUserId)
        {
            var JWTHandler = new JwtSecurityTokenHandler();
            var token = Request.Cookies["token"];
            var jsonToken = JWTHandler.ReadToken(token);
            var tokenDecrypted = jsonToken as JwtSecurityToken;

            var LoggedUserId = tokenDecrypted.Claims.First(claim => claim.Type == "ClientId").Value;
            Guid loggedUserId = Guid.Parse(LoggedUserId);

            _messagesList = _context.Messages
                .Where(x => x.SecondaryUserId == secondaryUserId && x.LoggedUserId == loggedUserId)
                .OrderByDescending(y => y.DateCreated)
                .Take(7)
                .ToList();

            _messagesList.AddRange(_context.Messages
                .Where(x => x.LoggedUserId == secondaryUserId && x.SecondaryUserId == loggedUserId)
                .OrderByDescending(y => y.DateCreated)
                .Take(7)
                .ToList()
                );

            _messagesList = _messagesList.OrderBy(x => x.DateCreated).ToList();

            return Ok(_messagesList);
        }
    }
}
