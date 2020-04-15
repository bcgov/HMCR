using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos.FeedbackMessage;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IFeebackMessageRepository
    {
        Task<HmrFeedbackMessage> CreateFeedbackMessage(FeedbackMessageDto feedback);
    }
    public class FeebackMessageRepository : HmcrRepositoryBase<HmrFeedbackMessage>, IFeebackMessageRepository
    {
        public FeebackMessageRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<HmrFeedbackMessage> CreateFeedbackMessage(FeedbackMessageDto feedback)
        {
            return await AddAsync(feedback);
        }
    }
}
