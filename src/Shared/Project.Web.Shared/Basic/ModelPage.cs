﻿using LightExcel;
using Microsoft.AspNetCore.Components;
using Project.Constraints;
using Project.Constraints.Models;
using Project.Constraints.Models.Request;
using Project.Constraints.Page;
using Project.Constraints.Store.Models;
using Project.Constraints.UI;
using Project.Constraints.UI.Extensions;
using Project.Constraints.UI.Table;
using System.Reflection;
using Microsoft.AspNetCore.Components.Rendering;

namespace Project.Web.Shared.Basic;

public abstract class ModelPage<TModel, TQuery> : JsComponentBase
    where TQuery : IRequest, new()
{
    [Inject] protected IExcelHelper Excel { get; set; }
    [Inject] IDownloadService DownloadService { get; set; }
    [CascadingParameter] IDomEventHandler DomEvent { get; set; }
    [CascadingParameter] TagRoute? RouteInfo { get; set; }
    public TableOptions<TModel, TQuery> Options { get; set; } = new();
    protected bool HideDefaultTableHeader { get; set; }
    bool IsOverride(string methodName)
    {
        var method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        return method?.DeclaringType != typeof(ModelPage<TModel, TQuery>);
    }
    protected override void OnInitialized()
    {
        base.OnInitialized();
        LoadJs = false;
        Options.AutoRefreshData = true;
        Options.RowKey = SetRowKey;
        Options.Buttons = this.CollectButtons<TModel>();
        Options.OnQueryAsync = OnQueryAsync;
        Options.OnAddItemAsync = OnAddItemAsync;
        Options.OnRowClickAsync = OnRowClickAsync;
        Options.AddRowOptions = OnAddRowOptions;
        Options.OnExportAsync = OnExportAsync;
        Options.OnSaveExcelAsync = OnSaveExcelAsync;
        Options.OnSelectedChangedAsync = OnSelectedChangedAsync;
        // 被重写了
        Options.ShowExportButton = IsOverride(nameof(OnExportAsync));
        Options.ShowAddButton = IsOverride(nameof(OnAddItemAsync));

        //DomEvent.OnKeyDown += DomEvent_OnKeyDown;
    }

    //protected override void OnDispose()
    //{
    //    DomEvent.OnKeyDown -= DomEvent_OnKeyDown;
    //}

    //private Task DomEvent_OnKeyDown(Microsoft.AspNetCore.Components.Web.KeyboardEventArgs arg)
    //{
    //    Console.WriteLine($"{RouteInfo?.RouteUrl} {arg.Key} down");
    //    if (RouteInfo?.IsActive ?? false)
    //    {
    //        return Options.RefreshAsync();
    //    }
    //    return Task.CompletedTask;
    //}

    protected virtual object SetRowKey(TModel model) => model;

    //private List<TableButton<TModel>> CollectButtons()
    //{
    //    List<TableButton<TModel>> buttons = new List<TableButton<TModel>>();

    //    var type = GetType();
    //    var methods = type.GetMethods().Where(m => m.GetCustomAttribute<TableButtonAttribute>() != null);

    //    foreach (var method in methods)
    //    {
    //        var btnOptions = method.GetCustomAttribute<TableButtonAttribute>()!;
    //        ArgumentNullException.ThrowIfNull(btnOptions.Label ?? btnOptions.LabelExpression);
    //        var btn = new TableButton<TModel>(btnOptions);
    //        btn.Callback = method.CreateDelegate<Func<TModel, Task<bool>>>(this);
    //        if (btnOptions.LabelExpression != null)
    //        {
    //            var le = type.GetMethod(btnOptions.LabelExpression);
    //            btn.LabelExpression = le?.CreateDelegate<Func<TableButtonContext<TModel>, string>>(this);
    //        }

    //        if (btnOptions.VisibleExpression != null)
    //        {
    //            var ve = type.GetMethod(btnOptions.VisibleExpression);
    //            btn.VisibleExpression = ve?.CreateDelegate<Func<TableButtonContext<TModel>, bool>>(this);
    //        }

    //        buttons.Add(btn);
    //    }
    //    return buttons;
    //}

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            if (Options.LoadDataOnLoaded)
            {
                await Options.RefreshAsync();
            }
        }
    }

    /// <summary>
    /// 设置行属性
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual Dictionary<string, object>? OnAddRowOptions(TModel model) => null;

    /// <summary>
    /// 行点击处理
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual Task OnRowClickAsync(TModel model) => Task.CompletedTask;

    /// <summary>
    /// 处理新增
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual Task<bool> OnAddItemAsync() => throw new NotImplementedException();

    /// <summary>
    /// 获取导出数据
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual async Task<IQueryCollectionResult<TModel>> OnExportAsync(TQuery query)
    {
        if (Options.Result == null)
        {
            await Options.RefreshAsync();
        }
        return Options.Result;
    }

    /// <summary>
    /// 导出Excel文件
    /// </summary>
    /// <param name="datas"></param>
    /// <returns></returns>
    protected virtual Task OnSaveExcelAsync(IEnumerable<TModel> datas)
    {
        var mainName = Router.Current?.RouteTitle ?? typeof(TModel).Name;
        var filename = $"{mainName}_{DateTime.Now:yyyyMMdd-HHmmss}.xlsx";
        var path = Path.Combine(AppConst.TempFilePath, filename);
        Excel.WriteExcel(path, datas);
        DownloadService.DownloadAsync(filename);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    protected virtual Task OnSelectedChangedAsync(IEnumerable<TModel> enumerable)
    {
        // TODO table的行选择处理
        return Task.CompletedTask;
    }

    /// <summary>
    /// 查询数据
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    protected abstract Task<IQueryCollectionResult<TModel>> OnQueryAsync(TQuery query);

    protected RenderFragment TableFragment => builder =>
    {
        if (!HideDefaultTableHeader)
        {
            builder.AddContent(0, b =>
            {
                b.Component<DefaultTableHeader<TModel, TQuery>>()
                    .SetComponent(c => c.Options, Options)
                    .Build();
            });
        }
        builder.AddContent(1, UI.BuildTable(Options));
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddContent(0, TableFragment);
    }
}
