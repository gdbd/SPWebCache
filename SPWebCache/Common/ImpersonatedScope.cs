using System;
using System.Security.Principal;

namespace SPWebCache.Common
{
    internal class ImpersonatedScope : IDisposable
    {
        private WindowsImpersonationContext _ctx;

        public ImpersonatedScope(string userPrincipalName)
        {
            var identity = new WindowsIdentity(userPrincipalName);
            _ctx = identity.Impersonate();
        }

        public ImpersonatedScope(WindowsIdentity identity)
        {
            _ctx = identity.Impersonate();
        }

        public void Dispose()
        {
            _ctx.Undo();
        }
    }
}
