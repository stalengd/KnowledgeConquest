﻿using Microsoft.EntityFrameworkCore;
using KnowledgeConquest.Shared.Math;
using KnowledgeConquest.Server.Extensions;
using KnowledgeConquest.Server.Models;

namespace KnowledgeConquest.Server.Services
{
    public class UserMapService
    {
        private readonly AppDbContext _db;

        public UserMapService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<UserMapCell>> GetUserCellsAsync(User user, CancellationToken ct)
        {
            var cells = await _db.UserMapCells.Where(x => x.User == user).ToListAsync(ct);
            if (!cells.Any())
            {
                cells = await InitializeUserMapAsync(user, ct);
            }
            return cells;
        }

        public async Task<Dictionary<User, List<UserMapCell>>> GetUsersCellsAsync(IEnumerable<string> users, CancellationToken ct)
        {
            return (await _db.UserMapCells
                .Where(x => users.Contains(x.User.Id))
                .Include(x => x.User)
                .ToListAsync(ct))
                .GroupBy(x => x.User)
                .ToDictionary(x => x.Key, x => x.ToList());
        }

        public async Task<UserMapCell?> ConquerCellAsync(User user, OffsetCoords position, bool isSuccess, CancellationToken ct)
        {
            var cell = await FindCellAsync(_db.UserMapCells, user, position, ct);
            if (cell == null)
                return null;

            cell.Type = isSuccess ? UserMapCellType.CapturedSuccessfuly : UserMapCellType.CapturedFaily;

            return cell;
        }

        public async Task<UserMapCell?> GetCellWithQuestionAsync(User user, OffsetCoords position, CancellationToken ct)
        {
            var cell = await FindCellAsync(_db.UserMapCells
                .Include(x => x.Question)
                .Include(x => x.Question.Answers),
                user, position, ct);
            return cell;
        }

        private async Task<List<UserMapCell>> InitializeUserMapAsync(User user, CancellationToken ct)
        {
            var cells = new List<UserMapCell>();
            var coords = new List<CubeCoords>();
            HexMath.Spiral(CubeCoords.Zero, 3, coords);
            var questions = await _db.Questions.ToListAsync(ct);
            questions.Ensure(coords.Count);
            questions.Shuffle();
            for (int i = 0; i < coords.Count; i++)
            {
                var pos = coords[i].ToOffsetCoords();
                cells.Add(new() 
                { 
                    User = user,
                    PositionX = pos.X,
                    PositionY = pos.Y,
                    Type = UserMapCellType.Free,
                    Question = questions[i],
                });
            }
            cells[0].Type = UserMapCellType.CapturedSuccessfuly; // Center is owned
            await _db.UserMapCells.AddRangeAsync(cells, ct);
            await _db.SaveChangesAsync(ct);
            return cells;
        }

        private static async Task<UserMapCell?> FindCellAsync(IQueryable<UserMapCell> query, User user, OffsetCoords position, CancellationToken ct)
        {
            return await query.FirstOrDefaultAsync(x => 
                x.User == user && 
                x.PositionX == position.X && 
                x.PositionY == position.Y, ct);
        }
    }
}
