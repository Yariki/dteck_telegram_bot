using DtekShutdownCheckBot.Shared.Entities;
using DtekShutdownCheckBot.Repositories;
using DtekShutdownCheckBot.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;

namespace DtekShutdownCheckBot.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : Controller
    {
        private readonly ITelegramBotClient _client;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChatController> _logger;

        public ChatController(ITelegramBotClient client, IUnitOfWork unitOfWork, ILogger<ChatController> logger)
        {
            _client = client;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<Chat>>> All()
        {

            try
            {
                var chats =  _unitOfWork.ChatRepository.GetAll("Words");

                return chats != null ? Ok(chats) : NotFound("There is no chat(s) yet.");
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return BadRequest();
            }
            
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Chat>> GetById([FromRoute] int id)
        {
            try
            {
                var chat = _unitOfWork.ChatRepository.GetById(id);

                return chat != null ? Ok(chat) : NotFound($"There is no chat with id - {id}");
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete([FromQuery] int id)
        {
            try
            {
                _unitOfWork.ChatRepository.Delete(id);
                _unitOfWork.SaveChanges();
                
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("send")]
        public async Task<ActionResult> Send([FromBody] Message message)
        {
            if (message == null)
            {
                return BadRequest();
            }

            _client?.SendTextMessageAsync(message.ChatId, message.Text);

            return Ok();
        }
        
    }
}
