using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult<UserMapDTO>> GetUserMapAsync(string? userId = null, CancellationToken ct = default)
        {
            User user;
            if (userId == null)
            {
                user = await _userManager.GetUserAsync(User);
            }
            else
            {
                user = await _userManager.FindByIdAsync(userId);
            }
            if (user == null)
            {
                return BadRequest("Specified user does not exists");
            }
            return await GetUserMapAsync(user, ct);
        }

        [HttpGet("NeighbourMaps")]
        public async Task<List<UserMapDTO>> GetNeighbourMapsAsync(CancellationToken ct)
        {
            var neighbours = await _userManager.Users.Select(x => x.Id).ToListAsync(ct);
            var currentUser = await _userManager.GetUserAsync(User);
            neighbours.Remove(currentUser.Id);
            var cellGroups = await _userMapService.GetUsersCellsAsync(neighbours, ct);
            var result = new List<UserMapDTO>();
            foreach ((var user, var cells) in cellGroups)
            {
                result.Add(ConvertCellsListToUserMap(user, cells));
            }
            return result;
        }

        [HttpGet("UserInfo")]
        public async Task<ActionResult<UserInfoDTO>> GetUserInfoAsync(string? userId = null, CancellationToken ct = default)
        {
            User user;
            if (userId == null)
            {
                user = await _userManager.GetUserAsync(User);
            }
            else
            {
                user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return BadRequest("Specified user does not exists");
                }
            }
            return new UserInfoDTO()
            {
                Id = user.Id,
                Username = user.UserName,
                Firstname = user.Firstname,
                Surname = user.Surname,
            };
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
            var isSuccess = rightCount == question.Answers.Count(x => x.IsRight);
            await _userMapService.ConquerCellAsync(user, response.CellPosition, isSuccess, ct);
            await _db.SaveChangesAsync(ct);
            return isSuccess;
        }

        private async Task<UserMapDTO> GetUserMapAsync(User user, CancellationToken ct)
        {
            var map = await _userMapService.GetUserCellsAsync(user, ct);
            return ConvertCellsListToUserMap(user, map);
        }

        private static UserMapDTO ConvertCellsListToUserMap(User user, IReadOnlyList<UserMapCell> cells)
        {
            var mapDto = new UserMapDTO()
            {
                UserId = user.Id,
                Cells = new(),
            };
            foreach (var cell in cells)
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
    }
}
