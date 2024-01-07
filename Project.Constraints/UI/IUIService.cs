﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Project.Common.Attributes;
using Project.Constraints.Models;
using Project.Constraints.Models.Request;
using Project.Constraints.Store;
using Project.Constraints.UI.Dropdown;
using Project.Constraints.UI.Flyout;
using Project.Constraints.UI.Form;
using Project.Constraints.UI.Props;
using Project.Constraints.UI.Table;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Project.Constraints.UI;

public interface IUIComponent
{
    IUIComponent Set(string key, object value);
    IUIComponent AdditionalParameters(Dictionary<string, object> parameters);
    RenderFragment Render();
}

public interface IClickable<TReturn>
{
    TReturn OnClick(Action callback);
    TReturn OnClick(EventCallback callback);
    TReturn OnClick(Action<object> callback);
    TReturn OnClick(Func<Task> callback);
    TReturn OnClick(Func<object, Task> callback);
    TReturn OnClick(EventCallback<MouseEventArgs> callback);
    TReturn OnClick(Action<MouseEventArgs> callback);
    TReturn OnClick(Func<MouseEventArgs, Task> callback);
}

public interface IBindableInputComponent<TPropModel, TValue> : IUIComponent
{
    IBindableInputComponent<TPropModel, TValue> Bind(Expression<Func<TValue>> expression);
    IBindableInputComponent<TPropModel, TValue> Bind(Expression<Func<TValue>> expression, Func<Task> onchange);
    IBindableInputComponent<TPropModel, TValue> Bind(Expression<Func<TValue>> expression, string valueName, Func<Task>? onchange = null);
    IBindableInputComponent<TPropModel, TValue> Set<TMember>(Expression<Func<TPropModel, TMember>> selector, TMember value);
}

public interface IInputComponent<TPropModel> : IUIComponent
{
    IInputComponent<TPropModel> Set<TMember>(Expression<Func<TPropModel, TMember>> selector, TMember value);
}
public interface IButtonInput : IInputComponent<ButtonProp>, IClickable<IButtonInput>
{

}

public interface ISelectInput<TPropModel, TItem, TValue> : IBindableInputComponent<TPropModel, TValue>
{

}

public enum MessageType
{
    Success,
    Warning,
    Error,
    Information,
}

public enum ButtonType
{
    Default,
    Primary,
    Secondary,
    Danger,
    Success,
}
[AutoInject]
public interface IUIService
{
    void Message(MessageType type, string message);
    void Notify(MessageType type, string title, string message);
    Task<TReturn> ShowDialogAsync<TReturn>(FlyoutOptions<TReturn> options);
    Task<TReturn> ShowDrawerAsync<TReturn>(FlyoutDrawerOptions<TReturn> options);


    /// <summary>
    /// 生成输入框
    /// <code>
    /// UI.BuildInput(this).Bind(() => ValueExpression).Render()
    /// </code>
    /// </summary>
    IBindableInputComponent<EmptyProp, TValue> BuildInput<TValue>(object reciver);

    /// <summary>
    /// 生成密码输入框
    /// <code>
    /// UI.BuildInput(this).Bind(() => ValueExpression).Render()
    /// </code>
    /// </summary>
    IBindableInputComponent<EmptyProp, string> BuildPassword(object reciver);
    /// <summary>
    /// 生成数字输入框
    /// <code>
    /// UI.BuildInput&lt;TValue&gt;(this).Bind(() => ValueExpression).Render()
    /// </code>
    /// </summary>
    IBindableInputComponent<EmptyProp, TValue> BuildNumberInput<TValue>(object reciver);

    IBindableInputComponent<EmptyProp, TValue> BuildDatePicker<TValue>(object reciver);

    IBindableInputComponent<EmptyProp, bool> BuildCheckBox(object reciver);

    /// <summary>
    /// 生成下拉选择框
    /// <code>
    /// UI.BuildSelect&lt;TValue&gt;(this, options).Bind(() => ValueExpression).Render()
    /// </code>
    /// </summary>
    /// <returns></returns>
    IBindableInputComponent<SelectProp, TValue> BuildSelect<TValue>(object reciver, SelectItem<TValue>? options);

    ISelectInput<SelectProp, TItem, TValue> BuildSelect<TItem, TValue>(object reciver, IEnumerable<TItem> options);

    /// <summary>
    /// 生成按钮
    /// </summary>
    IButtonInput BuildButton(object reciver);

    IBindableInputComponent<EmptyProp, bool> BuildSwitch(object reciver);

    RenderFragment BuildTable<TModel, TQuery>(TableOptions<TModel, TQuery> options) where TQuery : IRequest, new();

    RenderFragment BuildTableHeader<TModel, TQuery>(TableOptions<TModel, TQuery> options) where TQuery : IRequest, new();

    RenderFragment BuildForm<TData>(FormOptions<TData> options) where TData : class, new();

    RenderFragment BuildDropdown(DropdownOptions options);

    RenderFragment BuildMenu(IRouterStore router, bool horizontal, IAppStore app);

    RenderFragment BuildLoginForm(LoginFormModel model, Func<Task> handleLogin);

    IUIComponent BuildRow();
    IUIComponent BuildCard();
    IUIComponent BuildCol();
}