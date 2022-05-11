﻿using AntDesign.TableModels;

namespace BlazorWebAdmin.Template.Tables.Setting
{
    public record ColumnDefinition(string Label, string PropertyOrFieldName)
    {
        public int Index { get; set; }
        public Type DataType { get; set; } = typeof(string);
        public bool IsEnum => DataType.IsEnum || EnumValues != null;
        public string? Fixed { get; set; }
        public string? Width { get; set; }
        public Func<CellData, Dictionary<string, object>> OnCell { get; set; }
        public Dictionary<string, string> EnumValues { get; set; }

    }
}
