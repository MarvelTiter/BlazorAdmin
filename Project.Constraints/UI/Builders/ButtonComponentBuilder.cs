﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Project.Common.Attributes;
using Project.Constraints.UI.Props;

namespace Project.Constraints.UI.Builders
{
    [IgnoreAutoInject]
    public class ButtonComponentBuilder<TComponent> : InputComponentBuilder<TComponent, ButtonProp, ButtonComponentBuilder<TComponent>>, IButtonInput
         where TComponent : IComponent
    {

        public ButtonComponentBuilder()
        {

        }

        public ButtonComponentBuilder(Func<ButtonComponentBuilder<TComponent>, RenderFragment> newRender)
        {
            this.newRender = newRender;
        }

        public ButtonComponentBuilder(Action<ButtonComponentBuilder<TComponent>> tpropHandle)
        {
            this.tpropHandle = tpropHandle;
        }

        public override RenderFragment Render()
        {
            tpropHandle?.Invoke(this);
            return newRender?.Invoke(this) ?? base.Render();
        }

        #region OnClick
        public IButtonInput OnClick(Action callback)
        {
            var onclick = EventCallback.Factory.Create<MouseEventArgs>(Reciver, callback);
            Set("OnClick", onclick);
            return this;
        }

        public IButtonInput OnClick(EventCallback callback)
        {
            var onclick = EventCallback.Factory.Create<MouseEventArgs>(Reciver, callback);
            Set("OnClick", onclick);
            return this;
        }

        public IButtonInput OnClick(Action<object> callback)
        {
            var onclick = EventCallback.Factory.Create<MouseEventArgs>(Reciver, callback);
            Set("OnClick", onclick);
            return this;
        }

        public IButtonInput OnClick(Func<Task> callback)
        {
            var onclick = EventCallback.Factory.Create<MouseEventArgs>(Reciver, callback);
            Set("OnClick", onclick);
            return this;
        }

        public IButtonInput OnClick(Func<object, Task> callback)
        {
            var onclick = EventCallback.Factory.Create<MouseEventArgs>(Reciver, callback);
            Set("OnClick", onclick);
            return this;
        }

        public IButtonInput OnClick(EventCallback<MouseEventArgs> callback)
        {
            var onclick = EventCallback.Factory.Create(Reciver, callback);
            Set("OnClick", onclick);
            return this;
        }

        public IButtonInput OnClick(Action<MouseEventArgs> callback)
        {
            var onclick = EventCallback.Factory.Create(Reciver, callback);
            Set("OnClick", onclick);
            return this;
        }

        public IButtonInput OnClick(Func<MouseEventArgs, Task> callback)
        {
            var onclick = EventCallback.Factory.Create(Reciver, callback);
            Set("OnClick", onclick);
            return this;
        }


        #endregion

    }

    
}
