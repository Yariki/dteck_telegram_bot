using DtekShutdownCheckBot.Shared.Entities;
using DtekShutdownCheckBot.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DtekShutdownCheckBot.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IUnitOfWork unitOfWork, ILogger<ChatController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        [Route("all")]
        public ActionResult<IEnumerable<Chat>> All()
        {

            try
            {
                var chats =  _unitOfWork.ChatRepository.GetAll();

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
        public ActionResult GetById([FromRoute] string id)
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



        
    }
}
