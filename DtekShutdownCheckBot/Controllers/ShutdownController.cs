using DtekShutdownCheckBot.Repositories;
using DtekShutdownCheckBot.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DtekShutdownCheckBot.Controllers
{

    [ApiController]
    [Route("api/shutdown")]
    public class ShutdownController :  Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ShutdownController> _logger;

        public ShutdownController(IUnitOfWork unitOfWork, ILogger<ShutdownController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<Shutdown>>> GetAll()
        {
            try
            {
                var shutdowns = _unitOfWork.ShutdownRepository.GetAll().OrderBy(s => s.ShutdownDate);
                return shutdowns != null && shutdowns.Any() ? Ok(shutdowns) : NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("notsend")]
        public async Task<ActionResult<IEnumerable<Shutdown>>> GetNotSentShutdowns()
        {
            try
            {
                var notshutdown = _unitOfWork.ShutdownRepository.GetAllNotSentShutdowns();
                return notshutdown != null && notshutdown.Any() ? Ok(notshutdown) : NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());

                return BadRequest();
            }
        }
        
    }
}