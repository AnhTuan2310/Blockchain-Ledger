using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Blocks;
using Application.IServices;
using Application.Mappings;
using Application.State;
using Domain.Entities;

namespace Application.Services {
    public class LedgerQueryService : ILedgerQueryService {
        private readonly BlockchainState _state;

        public LedgerQueryService(BlockchainState state) {
            _state = state;
        }

        public async Task<List<BlockDetailDTO>> GetCurrentChainAsync() {
            await Task.Yield();
            return _state.Blocks
                .Select(b => b.ToDTO())
                .ToList();
        }

        public async Task<IEnumerable<BlockSearchResultDTO>> GetBlockSummariesAsync() {
            await Task.Yield();
            return _state.Blocks
                .Select(b => b.ToSearchResult());
        }

        public async Task<bool> ValidateChainAsync() {
            await Task.Yield();
            var chain = _state.Blocks;
            return chain.Zip(chain.Skip(1),
                (prev, next) => next.PreviousHash == prev.Hash).All(x => x);
        }

        public async Task<IEnumerable<BlockSearchResultDTO>> SearchBlocksAsync(string keyword) {
            await Task.Yield();
            keyword = keyword.ToLower();

            return _state.Blocks
                .Select(b => b.ToSearchResult())
                .Where(r =>
                    r.Hash.ToLower().Contains(keyword) ||
                    r.MinerNote.ToLower().Contains(keyword) ||
                    r.MatchedTransactions.Any(t =>
                        t.Content.ToLower().Contains(keyword) ||
                        t.Author.ToLower().Contains(keyword)))
                .ToList();
        }

        public async Task<BlockDetailDTO?> GetBlockDetailAsync(string hash) {
            await Task.Yield();
            var block = _state.Blocks.FirstOrDefault(b => b.Hash == hash);
            return block?.ToDTO();
        }
    }
}
