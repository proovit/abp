﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity.Organizations;

namespace Volo.Abp.Identity.EntityFrameworkCore
{
    public class EfCoreOrganizationUnitRepository : EfCoreRepository<IIdentityDbContext, OrganizationUnit, Guid>, IOrganizationUnitRepository
    {
        public EfCoreOrganizationUnitRepository(IDbContextProvider<IIdentityDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<OrganizationUnit>> GetChildrenAsync(Guid? parentId, CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(x => x.ParentId == parentId)
                .ToListAsync(GetCancellationToken(cancellationToken)).ConfigureAwait(false);
        }

        public async Task<List<OrganizationUnit>> GetAllChildrenWithParentCodeAsync(string code, Guid? parentId, CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(ou => ou.Code.StartsWith(code) && ou.Id != parentId.Value)
                .ToListAsync(GetCancellationToken(cancellationToken)).ConfigureAwait(false);
        }

        public async Task<List<OrganizationUnit>> GetListAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(t => ids.Contains(t.Id)).ToListAsync(GetCancellationToken(cancellationToken)).ConfigureAwait(false);
        }

        public override async Task<List<OrganizationUnit>> GetListAsync(bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .IncludeDetails(includeDetails)
                .ToListAsync(GetCancellationToken(cancellationToken)).ConfigureAwait(false);
        }

        public async Task<OrganizationUnit> GetOrganizationUnit(string displayName, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .IncludeDetails(includeDetails)
                .FirstOrDefaultAsync(
                    ou => ou.DisplayName == displayName,
                    GetCancellationToken(cancellationToken)
                ).ConfigureAwait(false);
        }

        public override IQueryable<OrganizationUnit> WithDetails()
        {
            return GetQueryable().IncludeDetails();
        }

    }
}