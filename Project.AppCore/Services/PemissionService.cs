﻿using MDbContext.ExpressionSql;
using MDbContext.Repository;
using Project.Constraints.Models;
using Project.Constraints.Models.Request;
using Project.Constraints.Services;
using Project.Models.Entities.Permissions;

namespace Project.AppCore.Services
{
    public partial class PemissionService : IPermissionService
    {
        private readonly IExpressionContext context;

        public PemissionService(IExpressionContext context)
        {
            this.context = context;
        }

        public async Task<IQueryCollectionResult<Power>> GetPowerListAsync(GenericRequest<Power> req)
        {
            var list = await context.Repository<Power>().GetListAsync(req.Expression, out var total, req.PageIndex, req.PageSize, p => p.Sort);
            return list.CollectionResult((int)total);
        }

        public async Task<IQueryCollectionResult<Power>> GetPowerListAsync()
        {
            var list = await context.Repository<Power>().GetListAsync(e => true, e => e.Sort);
            return list.CollectionResult();
        }

        public async Task<IQueryCollectionResult<Role>> GetRoleListAsync(GenericRequest<Role> req)
        {
            var list = await context.Repository<Role>().GetListAsync(req.Expression, out var total, req.PageIndex, req.PageSize);
            return list.CollectionResult((int)total);
        }

        public async Task<IQueryCollectionResult<Role>> GetRoleListAsync()
        {
            var list = await context.Repository<Role>().GetListAsync(e => true);
            return list.CollectionResult();
        }

        public async Task<IQueryCollectionResult<Power>> GetPowerListByUserIdAsync(string usrId)
        {
            var powers = await context.Select<Power, RolePower, UserRole>()
                                      .Distinct()
                                      .InnerJoin<RolePower>(w => w.Tb1.PowerId == w.Tb2.PowerId)
                                      .InnerJoin<UserRole>(w => w.Tb2.RoleId == w.Tb3.RoleId)
                                      .Where(w => w.Tb3.UserId == usrId)
                                      .OrderBy(w => w.Tb1.Sort)
                                      .ToListAsync();
            return powers.CollectionResult();
        }

        public async Task<IQueryCollectionResult<Power>> GetPowerListByRoleIdAsync(string roleId)
        {
            var powers = await context.Select<Power, RolePower>()
                                      .InnerJoin<RolePower>((r, p) => p.PowerId == r.PowerId)
                                      .Where((r, rp) => rp.RoleId == roleId)
                                      .ToListAsync();
            return powers.CollectionResult();
        }

        public async Task<IQueryCollectionResult<Role>> GetUserRolesAsync(string usrId)
        {
            var roles = await context.Select<Role, UserRole>()
                                     .InnerJoin<UserRole>((r, ur) => r.RoleId == ur.RoleId)
                                     .Where<UserRole>(ur => ur.UserId == usrId)
                                     .ToListAsync();
            return roles.CollectionResult();
        }

        public async Task<IQueryResult<bool>> SaveUserRole(string usrId, params string[] roles)
        {
            var db = context.BeginTransaction();
            db.Delete<UserRole>().Where(u => u.UserId == usrId).AttachTransaction();
            foreach (var r in roles)
            {
                var ur = new UserRole() { UserId = usrId, RoleId = r };
                db.Insert<UserRole>().AppendData(ur).AttachTransaction();
            }
            var n = await db.CommitTransactionAsync();
            return n.Result();
        }

        public async Task<IQueryResult<bool>> SaveRolePower(string roleId, params string[] powers)
        {
            var db = context.BeginTransaction();
            db.Delete<RolePower>().Where(r => r.RoleId == roleId).AttachTransaction();
            foreach (var p in powers)
            {
                var rp = new RolePower() { RoleId = roleId, PowerId = p };
                db.Insert<RolePower>().AppendData(rp).AttachTransaction();
            }
            var n = await db.CommitTransactionAsync();
            return n.Result();
        }

        public async Task<IQueryResult<bool>> UpdatePowerAsync(Power power)
        {
            var n = await context.Repository<Power>().UpdateAsync(power, p => p.PowerId == power.PowerId);
            return (n > 0).Result();
        }

        public async Task<IQueryResult<bool>> InsertPowerAsync(Power power)
        {
            var n = await context.Repository<Power>().InsertAsync(power);
            return (n != null).Result();
        }

        public async Task<IQueryResult<bool>> UpdateRoleAsync(Role role)
        {
            var n = await context.Repository<Role>().UpdateAsync(role, r => r.RoleId == role.RoleId);
            return (n > 0).Result();
        }

        public async Task<IQueryResult<bool>> InsertRoleAsync(Role role)
        {
            var n = await context.Repository<Role>().InsertAsync(role);
            return (n != null).Result();
        }

        public async Task<IQueryResult<bool>> DeleteRoleAsync(Role role)
        {
            var trans = context.BeginTransaction();
            trans.Delete<Role>().Where(r => r.RoleId == role.RoleId).AttachTransaction();
            trans.Delete<UserRole>().Where(ur => ur.RoleId == role.RoleId).AttachTransaction();
            trans.Delete<RolePower>().Where(rp => rp.RoleId == role.RoleId).AttachTransaction();
            var flag = await trans.CommitTransactionAsync();
            return flag.Result();
        }

        public async Task<IQueryResult<bool>> DeletePowerAsync(Power power)
        {
            var trans = context.BeginTransaction();
            trans.Delete<Power>().Where(p => p.PowerId == power.PowerId || p.ParentId == power.PowerId).AttachTransaction();
            trans.Delete<RolePower>().Where(p => p.PowerId == power.PowerId).AttachTransaction();
            var flag = await trans.CommitTransactionAsync();
            return flag.Result();
        }
    }
}