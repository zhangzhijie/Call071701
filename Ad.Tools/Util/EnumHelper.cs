﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using Ad.Cache;

namespace Ad.Util
{
    /// <summary>
    /// 枚举描述获取类
    /// </summary>
    [Serializable]
    public class EnumHelper
    {
        public static string GetJsDescriptionFunction_Name<T>(){
            var enumType = typeof(T);
            return "get_enum_" + enumType.Name + "_desc";
        }
        
        public static string GetJsDescriptionFunction<T>(string outJsFunctionName = null)
        {
            var enumType = typeof(T);
            var items = GetEnumItems(enumType);
            StringBuilder sb = new StringBuilder();
            if(string.IsNullOrWhiteSpace(outJsFunctionName))
            {
                 outJsFunctionName = "get_enum_" +enumType.Name + "_desc";
            }

            sb.AppendFormat(@"function {0}(v)", outJsFunctionName);
            sb.AppendLine("{");
            sb.AppendLine("var dic = new Array();");
            foreach (var item in items)
            {
                sb.AppendLine(string.Format("dic[{0}] = '{1}'", item.Value, item.Text));
            }
            sb.AppendLine("return dic[v];");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// 获得枚举类型项
        /// </summary>
        /// <param name="enumType">枚举的类型</param>
        /// <returns></returns>
        public static EnumItem GetEnumItem(IEnumerable<EnumItem> items, int i)
        {
            return items.First(f => f.Value == i);
        }

        /// <summary>
        /// 获得枚举类型项
        /// </summary>
        /// <param name="enumType">枚举的类型</param>
        /// <returns></returns>
        public static EnumItem GetEnumItem(Type enumType, int i)
        {
            var items = GetEnumItems(enumType);
            return items.First(f => f.Value == i);
        }

        /// <summary>
        /// 获得枚举类型所包含的全部项的列表。
        /// </summary>
        /// <param name="enumType">枚举的类型</param>
        /// <returns></returns>
        public static IEnumerable<EnumItem> GetEnumItems(Type enumType)
        {
            List<EnumItem> list = new List<EnumItem>();
            // 获得特性Description的类型信息
            Type typeDescription = typeof(DescriptionAttribute);
            // 获得枚举的字段信息（因为枚举的值实际上是一个static的字段的值）
            FieldInfo[] fields = enumType.GetFields();
            // 检索所有字段
            foreach (FieldInfo field in fields)
            {
                // 过滤掉一个不是枚举值的，记录的是枚举的源类型
                if (field.FieldType.IsEnum == false)
                    continue;
                // 通过字段的名字得到枚举的值
                int value = (int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);
                string text = string.Empty;
                // 获得这个字段的所有自定义特性，这里只查找Description特性
                object[] arr = field.GetCustomAttributes(typeDescription, true);
                if (arr.Length > 0)
                {
                    // 因为Description自定义特性不允许重复，所以只取第一个
                    DescriptionAttribute aa = (DescriptionAttribute)arr[0];
                    // 获得特性的描述值
                    text = aa.Description;
                }
                else
                {
                    // 如果没有特性描述，那么就显示英文的字段名
                    text = field.Name;
                }
                var item = new EnumItem(value, text);

                list.Add(item);
            }
            return list;
        }

        public static int? GetFromDescription<T>(string description, bool isCaseUpLower = false)
        {
            var enumType = typeof(T);
            // 获得特性Description的类型信息
            Type typeDescription = typeof(DescriptionAttribute);
            // 获得枚举的字段信息（因为枚举的值实际上是一个static的字段的值）
            FieldInfo[] fields = enumType.GetFields();
            foreach (FieldInfo field in fields)
            {
                // 过滤掉一个不是枚举值的，记录的是枚举的源类型
                if (field.FieldType.IsEnum == false)
                    continue;
                // 通过字段的名字得到枚举的值
                int value = (int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);
                if (isCaseUpLower == false)
                {
                    if (GetDescription<T>(value).ToLower() == description.ToLower())
                    {
                        return value;
                    }
                }
                else
                {
                    if (GetDescription<T>(value) == description)
                    {
                        return value;
                    }
                }
            }
            return null;
        }

        public static string GetDescription<T>(int enumObj)
        {
            var enumType = typeof(T);
            return CacheCallHelper<string, EnterpriseLibraryCacheServiceProvider>.CacheFunRun(enumType + enumObj.ToString(),
                delegate
                {
                    // 获得特性Description的类型信息
                    Type typeDescription = typeof(DescriptionAttribute);
                    // 获得枚举的字段信息（因为枚举的值实际上是一个static的字段的值）
                    FieldInfo[] fields = enumType.GetFields();
                    // 检索所有字段
                    foreach (FieldInfo field in fields)
                    {
                        // 过滤掉一个不是枚举值的，记录的是枚举的源类型
                        if (field.FieldType.IsEnum == false)
                            continue;
                        // 通过字段的名字得到枚举的值
                        int value = (int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);
                        if (value == enumObj)
                        {
                            object[] arr = field.GetCustomAttributes(typeDescription, true);
                            if (arr.Length > 0)
                            {
                                // 因为Description自定义特性不允许重复，所以只取第一个
                                DescriptionAttribute aa = (DescriptionAttribute)arr[0];
                                // 获得特性的描述值
                                return aa.Description;
                            }
                            else
                            {
                                return field.Name;
                            }
                        }
                    }
                    return enumObj.ToString();
                }, CachingExpirationTypes.Invariable, null);
        }

        public static string GetDescription<T>(object enumObj)
        {
            return GetDescription<T>((int)enumObj);
        }

        public static string Get_Description<T>(T enumItem)
        {
            object enumObj = (object)enumItem;
            return GetDescription<T>((int)enumObj);
        }
    }

    [Serializable]
    public class EnumItem
    {
        private int m_value;
        private string m_text;
        public int Value
        {
            get { return m_value; }
            set { m_value = value; }
        }
        public string Text
        {
            get { return m_text; }
            set { m_text = value; }
        }

        public EnumItem(int _value, string _text)
        {
            m_value = _value;
            m_text = _text;
        }

        public const string ValueField = "Value";
        public const string TextField = "Text";

        public const int EnumItemEmptyValue = 0;
    }
}
