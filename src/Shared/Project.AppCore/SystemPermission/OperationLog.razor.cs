﻿using Microsoft.AspNetCore.Components;
using Project.Constraints.Models.Permissions;
using Project.Constraints.PageHelper;

namespace Project.AppCore.SystemPermission
{
    public partial class OperationLog<TRunLog> : ModelPage<TRunLog, GenericRequest<TRunLog>>, IPageAction
        where TRunLog : class, IRunLog, new()
    {
        [Inject]
        public IRunLogService<TRunLog> RunLogSrv { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Options.LoadDataOnLoaded = true;
        }

        public Task OnShowAsync() => Task.CompletedTask;

        public Task OnHiddenAsync() => Task.CompletedTask;

        protected override object SetRowKey(TRunLog model) => model.LogId;

        protected override Task<IQueryCollectionResult<TRunLog>> OnQueryAsync(GenericRequest<TRunLog> query) => RunLogSrv.GetRunLogsAsync(query);

        protected override Task<IQueryCollectionResult<TRunLog>> OnExportAsync(GenericRequest<TRunLog> query) => base.OnExportAsync(query);

    }
}