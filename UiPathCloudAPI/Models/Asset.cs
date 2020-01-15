using Newtonsoft.Json;
using System;

namespace UiPathCloudAPISharp.Models
{
    public enum AssetValueType
    {
        None,
        Text,
        Integer,
        Bool,
        Credential
    }

    public class Asset
    {
        private bool _lockTypeValue;

        public Asset()
        {
            _lockTypeValue = true;
            ValueTypeString = null;
        }

        public Asset(string name, object value)
        {
            Name = name;
            Value = value;
        }

        [JsonProperty(PropertyName = "@odata.context")]
        public string Context { get; set; }
        
        public string Name { get; set; }

        [JsonProperty(PropertyName = "ValueType")]
        public string ValueTypeString
        {
            get
            {
                return ValueType.ToString();
            }
            set
            {
                _lockTypeValue = true;
                if (string.IsNullOrEmpty(value))
                {
                    ValueType = AssetValueType.None;
                }
                else
                {
                    ValueType = (AssetValueType)Enum.Parse(typeof(AssetValueType), value, true);
                }
            }
        }

        [JsonIgnore]
        public AssetValueType ValueType { get; private set; }

        public string StringValue { get; set; }

        public bool BoolValue { get; set; }

        public int IntValue { get; set; }

        public string CredentialUsername { get; set; }

        public string CredentialPassword { get; set; }

        public int Id { get; set; }

        [JsonIgnore]
        public object Value
        {
            get
            {
                if (ValueType == AssetValueType.None)
                {
                    return null;
                }
                else if (ValueType == AssetValueType.Bool)
                {
                    return BoolValue;
                }
                else if (ValueType == AssetValueType.Integer)
                {
                    return IntValue;
                }
                else if (ValueType == AssetValueType.Text)
                {
                    return StringValue;
                }
                else if (ValueType == AssetValueType.Credential)
                {
                    return CredentialUsername;
                }
                return null;
            }
            set
            {
                if (_lockTypeValue)
                {
                    if (ValueType == AssetValueType.Integer && value is int)
                    {
                        IntValue = (int)value;
                    }
                    else if (ValueType == AssetValueType.Bool && value is bool)
                    {
                        BoolValue = (bool)value;
                    } 
                    else if (ValueType == AssetValueType.Text && value is string)
                    {
                        StringValue = value.ToString();
                    }
                    else if (ValueType == AssetValueType.Credential && value is string)
                    {
                        CredentialUsername = value.ToString();
                    }
                    else
                    {
                        throw new Exception("Incorrect new value type.");
                    }
                }
                else
                {
                    _lockTypeValue = true;
                    if (value is int)
                    {
                        ValueType = AssetValueType.Integer;
                        IntValue = (int)value;
                    }
                    else if (value is bool)
                    {
                        ValueType = AssetValueType.Bool;
                        BoolValue = (bool)value;
                    }
                    else
                    {
                        ValueType = AssetValueType.Text;
                        StringValue = value.ToString();
                    }
                }
            }
        }

        public ConcreteAsset Concrete()
        {
            if (ValueType == AssetValueType.Integer)
            {
                return new IntegerAsset(this);
            }
            else if (ValueType == AssetValueType.Bool)
            {
                return new BoolAsset(this);
            }
            else if (ValueType == AssetValueType.Credential)
            {
                return new CredentialAsset(this);
            }
            else
            {
                return new TextAsset(this);
            }
        }
    }

    public abstract class ConcreteAsset
    {
        public Asset Asset { get; private set; }

        public string Name { get { return Asset.Name; } }

        public ConcreteAsset(Asset asset)
        {
            Asset = asset;
        }

        public abstract string StringValue { get; }
    }

    public class TextAsset : ConcreteAsset
    {
        public string Value
        {
            get
            {
                return Asset.StringValue;
            }
        }

        public TextAsset(string name, string value)
            : this(new Asset(name, value))
        {
        }

        public TextAsset(Asset asset)
            : base(asset)
        {
        }

        public override string StringValue { get { return Value; } }
    }

    public class IntegerAsset : ConcreteAsset
    {
        public int Value
        {
            get
            {
                return Asset.IntValue;
            }
        }
        
        public IntegerAsset(string name, int value)
            : this(new Asset(name, value))
        {
        }

        public IntegerAsset(Asset asset)
            : base(asset)
        {
        }

        public override string StringValue { get { return Value.ToString(); } }
    }

    public class BoolAsset : ConcreteAsset
    {
        public bool Value
        {
            get
            {
                return Asset.BoolValue;
            }
        }
        public BoolAsset(string name, bool value)
            : this(new Asset(name, value))
        {
        }

        public BoolAsset(Asset asset)
            : base(asset)
        {
        }

        public override string StringValue { get { return Value.ToString(); } }
    }

    public class CredentialAsset : ConcreteAsset
    {
        public string Value
        {
            get
            {
                return Asset.CredentialUsername;
            }
        }

        public CredentialAsset(Asset asset)
            : base(asset)
        {
        }

        public override string StringValue { get { return Value.ToString(); } }
    }
}
