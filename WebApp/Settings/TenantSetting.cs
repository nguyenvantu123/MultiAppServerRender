using BlazorWebApi.Users.Models;
using Breeze.Sharp;
using Google.Protobuf.WellKnownTypes;
using System.ComponentModel;

namespace WebApp.Settings
{
    public partial class TenantSettingModels : BaseEntity
    {
        public String TenantId
        {
            get { return GetValue<String>(); }
            set { SetValue(value); }
        }

        public SettingKey Key
        {
            get { return GetValue<SettingKey>(); }
            set { SetValue(value); }
        }

        public String Value
        {
            get { return GetValue<String>(); }
            set { SetValue(value); }
        }

        public SettingType Type
        {
            get { return GetValue<SettingType>(); }
            set { SetValue(value); }
        }

        private T GetterValue<T>()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            T result = default;

            try
            {
                result = (T)converter.ConvertFromString(Value);
            }
            catch (Exception)
            {
                Value = result.ToString();
            }

            return result;
        }

        public int ValueAsInt
        {
            get => GetterValue<int>();

            set => Value = value.ToString();
        }

        public bool ValueAsBool
        {
            get => GetterValue<bool>();

            set => Value = value.ToString();
        }

    }
}
