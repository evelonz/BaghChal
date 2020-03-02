using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaghChal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace BaghChalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {

        private IMemoryCache _cache;

        public GameController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        [HttpGet]
        public ReturnBoard GetGameState()
        {
            if (_cache.TryGetValue<string>("0", out var CacheEntry))
            {
                var board2 = JsonConvert.DeserializeObject<GameBoard>(CacheEntry);
                return new ReturnBoard(board2);
            }
            var board = new GameBoard();
            string jsonData = JsonConvert.SerializeObject(board);

            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));
            // Save data in cache.
            _cache.Set("0", jsonData, cacheEntryOptions);

            var ret = new ReturnBoard(board);

            return ret;
        }

        [HttpPost]
        public object MakeMove(Move t)
        {
            if (_cache.TryGetValue<string>("0", out var CacheEntry))
            {
                var board = JsonConvert.DeserializeObject<GameBoard>(CacheEntry);
                var (move, nextState) = board.Move(board.CurrentUsersTurn, (t.xs, t.ys), (t.xe, t.ye));

                var resultOK = GoodMoves.Contains(move);
                // If result is ok, then update the board for both users.
                if (resultOK) {
                    // AI move
                    var move2 = BaghChalAI.MinMaxExternal.GetMove(nextState);
                    (move, nextState) = nextState.Move(move2.Piece, move2.Start, move2.End);

                    string jsonData = JsonConvert.SerializeObject(nextState);

                    // Set cache options.
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        // Keep in cache for this time, reset time if accessed.
                        .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                    // Save data in cache.
                    _cache.Set("0", jsonData, cacheEntryOptions);
                }
                var ret = new ReturnBoard(nextState);

                return new { result = resultOK, error = move.ToString(), newState = ret };

            }

            return new { result = false, error = "No session found. Try and restart." };
        }

        private static readonly MoveResult[] GoodMoves = { MoveResult.MoveOK, MoveResult.TigerWin, MoveResult.GoatCaptured, MoveResult.GoatPlaced, MoveResult.GoatWin, MoveResult.Draw };

    }

    public class Move
    {
        public int xs { get; set; }
        public int ys { get; set; }
        public int xe { get; set; }
        public int ye { get; set; }
    }

    public class ReturnBoard
    {
        public int[] Board { get; set; }
        public int Ply { get; set; }
        public Pieces Turn { get; set; }
        public int GoatsLeftToPlace { get; set; }
        public int GoatsCaptured { get; set; }
        public int GameWinner { get; set; }

        public ReturnBoard(GameBoard board)
        {
            Board = board.GetBoardState();
            Ply = board.Ply;
            Turn = board.CurrentUsersTurn;
            GoatsLeftToPlace = board.GoatsLeftToPlace;
            GoatsCaptured = board.GoatsCaptured;

            var t = board.CheckGameEnd(board.CurrentUsersTurn, checkAfterMove: false);
            GameWinner = t ? (int)(board.CurrentUsersTurn + 1) % 2 : -1;
        }
    }
}