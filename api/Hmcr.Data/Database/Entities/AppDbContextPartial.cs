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
        private const string ConcurrencyControlNumber = "CONCURRENCY_CONTROL_NUMBER";
        private const string AppCreateUserid = "APP_CREATE_USERID";
        private const string AppCreateUserDirectory = "APP_CREATE_USER_DIRECTORY";
        private const string AppCreateUserGuid = "APP_CREATE_USER_GUID";
        private const string AppCreateTimestamp = "APP_CREATE_TIMESTAMP";
        private const string AppLastUpdateUserid = "APP_LAST_UPDATE_USERID";
        private const string AppLastUpdateUserDirectory = "APP_LAST_UPDATE_USER_DIRECTORY";
        private const string AppLastUpdateUserGuid = "APP_LAST_UPDATE_USER_GUID";
        private const string AppLastUpdateTimestamp = "APP_LAST_UPDATE_TIMESTAMP";

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
                if (AuditableEntity(entry.Entity))
                {
                    SetAuditProperty(entry.Entity, AppLastUpdateUserid, _currentUser.UniversalId);
                    SetAuditProperty(entry.Entity, AppLastUpdateUserDirectory, _currentUser.AuthDirName);
                    SetAuditProperty(entry.Entity, AppLastUpdateUserGuid, _currentUser.UserGuid);
                    SetAuditProperty(entry.Entity, AppLastUpdateTimestamp, currentTime);

                    if (entry.State == EntityState.Added)
                    {
                        SetAuditProperty(entry.Entity, AppCreateUserid, _currentUser.UniversalId);
                        SetAuditProperty(entry.Entity, AppCreateUserDirectory, _currentUser.AuthDirName);
                        SetAuditProperty(entry.Entity, AppCreateUserGuid, _currentUser.UserGuid);
                        SetAuditProperty(entry.Entity, AppCreateTimestamp, currentTime);
                        SetAuditProperty(entry.Entity, ConcurrencyControlNumber, 1);
                    }
                    else
                    {
                        int controlNumber = (int)GetAuditProperty(entry.Entity, ConcurrencyControlNumber);
                        controlNumber = controlNumber + 1;
                        SetAuditProperty(entry.Entity, ConcurrencyControlNumber, controlNumber);
                    }
                }
            }
        }

        private static bool AuditableEntity(object objectToCheck)
        {
            Type type = objectToCheck.GetType();
            return type.GetProperty("AppCreateUserDirectory") != null;
        }

        private static object GetAuditProperty(object obj, string property)
        {
            PropertyInfo prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);

            if (prop != null && prop.CanRead)
            {
                return prop.GetValue(obj);
            }

            return null;
        }

        private static void SetAuditProperty(object obj, string property, object value)
        {
            PropertyInfo prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);

            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(obj, value, null);
            }
        }
        #endregion

    }
}
