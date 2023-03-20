using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.User;
using Hmcr.Model.Utils;
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

            int result;

            try
            {
                result = base.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
                    entry.Member(AppLastUpdateUserid).CurrentValue = _currentUser.Username;
                    entry.Member(AppLastUpdateUserDirectory).CurrentValue = _currentUser.AuthDirName.ToShortDirectory();
                    entry.Member(AppLastUpdateUserGuid).CurrentValue = _currentUser.UserGuid;
                    entry.Member(AppLastUpdateTimestamp).CurrentValue = currentTime; 

                    if (entry.State == EntityState.Added)
                    {
                        entry.Member(AppCreateUserid).CurrentValue = _currentUser.Username;
                        entry.Member(AppCreateUserDirectory).CurrentValue = _currentUser.AuthDirName.ToShortDirectory();
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
                else if (entry.Members.Any(m => m.Metadata.Name == ConcurrencyControlNumber))
                {
                    if (entry.State == EntityState.Added)
                    {
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
