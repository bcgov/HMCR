using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos.FeedbackMessage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IFeebackMessageRepository
    {
        Task<HmrFeedbackMessage> CreateFeedbackMessageAsync(FeedbackMessageDto feedback);
        Task UpdateFeedbackMessageAsync(FeedbackMessageUpdateDto feedbackMessage);
        Task<IEnumerable<FeedbackMessageUpdateDto>> GetFailedFeedbackMessagesAsync();
    }
    public class FeebackMessageRepository : HmcrRepositoryBase<HmrFeedbackMessage>, IFeebackMessageRepository
    {
        public FeebackMessageRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<HmrFeedbackMessage> CreateFeedbackMessageAsync(FeedbackMessageDto feedback)
        {
            return await AddAsync(feedback);
        }

        public async Task UpdateFeedbackMessageAsync(FeedbackMessageUpdateDto feedbackMessage)
        {
            var entity = await DbSet
                    .FirstAsync(x => x.FeedbackMessageId == feedbackMessage.FeedbackMessageId);

            Mapper.Map(feedbackMessage, entity);
        }

        public async Task<IEnumerable<FeedbackMessageUpdateDto>> GetFailedFeedbackMessagesAsync()
        {
            var hourAgo = DateTime.UtcNow.AddHours(-1);

            return await GetAllAsync<FeedbackMessageUpdateDto>(x => x.IsSent == false && x.CommunicationDate < hourAgo);
        }
    }
}
