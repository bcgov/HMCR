using Hmcr.Model;
using Hmcr.Model.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hmcr.Data.Database.Entities
{
    public partial class AppDbContext
    {
        private const string ConcurrencyControlNumber = "ConcurrencyControlNumber";
        private const string AppCreateUserid = "AppCreateUserid";
        private const string AppCreateUserDirectory = "AppCreateUserDirectory";
        private const string AppCreateUserGuid = "AppCreateUserGuid";
        private const string AppCreateTimestamp = "AppCreateTimestamp";
        private const string AppLastUpdateUserid = "AppLastUpdateUserid";
        private const string AppLastUpdateUserDirectory = "AppLastUpdateUserDirectory";
        private const string AppLastUpdateUserGuid = "AppLastUpdateUserGuid";
        private const string AppLastUpdateTimestamp = "AppLastUpdateTimestamp";

        public readonly HmcrCurrentUser _currentUser;

        public AppDbContext(DbContextOptions<AppDbContext> options, HmcrCurrentUser currentUser)
            : base(options)
        {
            _currentUser = currentUser;
        }

        public override int SaveChanges()
        {
            PerformAudit();

            int result = 0;
            try
            {
                result = base.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                if (e.InnerException.Message.Contains(" Cannot insert duplicate key in object."))
                {
                    throw new Exception("This record already exists.", e);
                }
                else if (e.InnerException.Message.StartsWith("20180"))
                {
                    throw new Exception("This record has been updated by another user.", e);
                }

                throw;
            }

            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            PerformAudit();

            int result = 0;
            try
            {
                result = await base.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                if (!e.InnerException.Message.Contains(" Cannot insert duplicate key in object."))
                {
                    throw new Exception("This record already exists.");
                }
                else if (e.InnerException.Message.StartsWith("20180"))
                {
                    throw new Exception("This record has been updated by another user.");
                }

                throw;
            }

            return result;
        }

        #region Audit Helper

        private void PerformAudit()
        {
            IEnumerable<EntityEntry> modifiedEntries = ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added ||
                                e.State == EntityState.Modified);

            DateTime currentTime = DateTime.UtcNow;

            foreach (var entry in modifiedEntries)
            {
                if (entry.Members.Any(m => m.Metadata.Name == AppCreateUserGuid)) //auditable entity
                {
                    entry.Member(AppLastUpdateUserid).CurrentValue = _currentUser.UniversalId;
                    entry.Member(AppLastUpdateUserDirectory).CurrentValue = _currentUser.AuthDirName;
                    entry.Member(AppLastUpdateUserGuid).CurrentValue = _currentUser.UserGuid;
                    entry.Member(AppLastUpdateTimestamp).CurrentValue = currentTime; 

                    if (entry.State == EntityState.Added)
                    {
                        entry.Member(AppCreateUserid).CurrentValue = _currentUser.UniversalId;
                        entry.Member(AppCreateUserDirectory).CurrentValue = _currentUser.AuthDirName;
                        entry.Member(AppCreateUserGuid).CurrentValue = _currentUser.UserGuid;
                        entry.Member(AppCreateTimestamp).CurrentValue = currentTime;
                        entry.Member(ConcurrencyControlNumber).CurrentValue = (long)1;
                    }
                    else
                    {
                        var controlNumber = (long)entry.Member(ConcurrencyControlNumber).CurrentValue + 1;
                        entry.Member(ConcurrencyControlNumber).CurrentValue = controlNumber;
                    }
                }
            }
        }
        #endregion
    }
}
