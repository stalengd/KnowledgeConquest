using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using KnowledgeConquest.Server.Models;
using KnowledgeConquest.Server.Services;

namespace KnowledgeConquest.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class MapController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly UserMapService _userMapService;

        public MapController(AppDbContext db, UserManager<User> userManager, UserMapService userMapService)
        {
            _db = db;
            _userManager = userManager;
            _userMapService = userMapService;
        }

        [HttpGet]
        public async Task<UserMapDTO> GetUserMapAsync(CancellationToken ct)
        {
            var user = await _userManager.GetUserAsync(User);
            var map = await _userMapService.GetUserCellsAsync(user, ct);
            var mapDto = new UserMapDTO()
            {
                UserId = user.Id,
                Cells = new(),
            };
            foreach (var cell in map)
            {
                mapDto.Cells.Add(new()
                {
                    PositionX = cell.PositionX,
                    PositionY = cell.PositionY,
                    Type = cell.Type,
                });
            }
            return mapDto;
        }

        [HttpGet("CellQuestion")]
        public async Task<ActionResult<QuestionDTO>> GetCellQuestionAsync(int cellX, int cellY, CancellationToken ct)
        {
            var user = await _userManager.GetUserAsync(User);
            var cell = await _userMapService.GetCellWithQuestionAsync(user, new(cellX, cellY), ct);
            if (cell == null)
                return BadRequest();

            return new QuestionDTO()
            {
                Title = cell.Question.Title,
                Answers = cell.Question.Answers.Select(x => new QuestionAnswerDTO() 
                { 
                    Title = x.Title, 
                }).ToList(),
            };
        }

        [HttpPost("AnswerCellQuestion")]
        public async Task<ActionResult<bool>> AnswerQuestionAsync(CellQuestionResponse response, CancellationToken ct)
        {
            var user = await _userManager.GetUserAsync(User);
            var cell = await _userMapService.GetCellWithQuestionAsync(user, response.CellPosition, ct);
            if (cell == null)
                return BadRequest();

            var question = cell.Question;
            var rightCount = question.CheckAnswers(response.Answers);
            if (rightCount == question.Answers.Count(x => x.IsRight))
            {
                await _userMapService.ConquerCellAsync(user, response.CellPosition, ct);
                await _db.SaveChangesAsync(ct);
                return true;
            }
            return false;
        }
    }
}
