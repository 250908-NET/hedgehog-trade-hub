

using TradeHub.Api.Models;
using TradeHub.Api.Repository.Interfaces;
using TradeHub.Api.Services.Interfaces;



namespace TradeHub.Api.Services
{
    public class TradeService : ITradeService
    {
        private readonly ITradeRepository _repo;

        public TradeService(ITradeRepository repo)
        {
            _repo = repo;
        }

        public Task<Trade?> AcceptAsync(long tradeId)
        {
            return _repo.AcceptTradeAsync(tradeId);
        }

    }
}