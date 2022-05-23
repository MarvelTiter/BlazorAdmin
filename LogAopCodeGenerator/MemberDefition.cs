﻿using System.Text.RegularExpressions;

namespace LogAopCodeGenerator
{
    public struct FieldDefinition
    {

    }
    public struct MemberDefinition
    {
        public string AccessLevel { get; set; }// public, private, etc..
        public string TypeName { get; set; }
        public string Name { get; set; }
        public bool IsReturnVoid { get; set; }
        public bool IsAsync { get; set; }
        public string ReturnTypeString { get; set; }
        public string ReturnString => IsReturnVoid ? "void" : ReturnTypeString;
        public string AsyncKeyToken => IsAsync ? " async " : " ";
        public string AwaitKeyToken => IsAsync ? " await " : " ";
        public string ReturnValAsign
        {
            get
            {
                if (IsReturnVoid)
                {
                    return "";
                }
                else
                {
                    if (IsTask)
                    {
                        if (IsTaskWithoutValue)
                            return "";
                        else
                            return "context.ReturnValue = val;";

                    }
                    else
                    {
                        return "context.ReturnValue = val;";
                    }
                }
            }
        }
        public string ReturnValueRecive
        {
            get
            {
                if (IsReturnVoid)
                {
                    return "";
                }
                else
                {
                    if (IsTask)
                    {
                        if (IsTaskWithoutValue)
                            return "";
                        else
                            return "val = ";
                    }
                    else
                    {
                        return "val = ";
                    }
                }
            }
        }
        public string LocalVar
        {
            get
            {
                if (IsReturnVoid || IsTaskWithoutValue) return "";
                else
                {
                    return $"{RealReturnType} val = default;";
                }
            }
        }
        public string ReturnLabel
        {
            get
            {
                if (IsReturnVoid) return "";
                else
                {
                    if (IsTask)
                    {
                        if (IsTaskWithoutValue)
                        {
                            // 返回值是 Task
                            return IsAsync ? "" : "return System.Threading.Tasks.Task.CompletedTask;";
                        }
                        else
                        {
                            // 返回值是 Task<T>
                            return IsAsync ? "return val;" : $"return System.Threading.Tasks.Task.FromResult<{RealReturnType}>(val);";
                        }
                    }
                    else
                    {
                        return "return val;";
                    }
                }
            }
        }
        public string GetTaskValue => IsAsync ? "" : ".Result";
        public string WaitTask => IsAsync ? "" : ".Wait()";
        public string InternalMethodResult
        {
            get
            {
                if (IsReturnVoid)
                {
                    return "";
                }
                else
                {
                    if (IsTask)
                    {
                        if (IsTaskWithoutValue)
                        {
                            // 返回值是 Task
                            return WaitTask;
                        }
                        else
                        {
                            // 返回值是 Task<T>
                            return GetTaskValue;
                        }
                    }
                    else
                    {
                        return "";
                    }
                }

            }
        }
        public string RealReturnType => IsTask ? Regex.Match(ReturnTypeString, "Task<(.+)>").Groups[1].Value : ReturnTypeString;
        public bool IsTask => ReturnTypeString.IndexOf("System.Threading.Tasks") > -1;
        public bool IsTaskWithoutValue => string.IsNullOrEmpty(RealReturnType);
        public string Body { get; set; }
    }

}
