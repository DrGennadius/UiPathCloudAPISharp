using Newtonsoft.Json;
using System;

namespace UiPathCloudAPISharp.Models
{
    public enum AssetValueType
    {
        Text,
        Integer,
        Bool,
        Credential
    }

    public class Asset
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string Context { get; set; }
        
        public string Name { get; set; }

        public AssetValueType ValueType { get; set; }

        public string StringValue { get; set; }

        public bool BoolValue { get; set; }

        public int IntValue { get; set; }

        public string CredentialUsername { get; set; }

        public string CredentialPassword { get; set; }

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

        public abstract bool CorrectAssignment { get; }

        public abstract string StringValue { get; }

        public string ForceStringValue()
        {
            if (Asset.ValueType == AssetValueType.Text)
            {
                return Asset.StringValue;
            }
            else if (Asset.ValueType == AssetValueType.Integer)
            {
                return Asset.IntValue.ToString();
            }
            else if (Asset.ValueType == AssetValueType.Bool)
            {
                return Asset.BoolValue.ToString();
            }
            else
            {
                return Asset.CredentialUsername;
            }
        }
    }

    public class TextAsset : ConcreteAsset
    {
        public string Value { get; set; }

        public TextAsset(Asset asset)
            : base(asset)
        {
            if (Asset.ValueType == AssetValueType.Text)
            {
                Value = Asset.StringValue;
            }
            else if (Asset.ValueType == AssetValueType.Integer)
            {
                Value = Asset.IntValue.ToString();
            }
            else if (Asset.ValueType == AssetValueType.Bool)
            {
                Value = Asset.BoolValue.ToString();
            }
            else
            {
                Value = Asset.CredentialUsername;
            }
        }

        public override string StringValue { get { return Value; } }

        public override bool CorrectAssignment { get { return true; } }
    }

    public class IntegerAsset : ConcreteAsset
    {
        private int m_Value;

        public int Value
        {
            get
            {
                if (!m_CorrectAssignment)
                {
                    throw new Exception("Incorrect assignment of value");
                }
                return m_Value;
            }
        }

        private bool m_CorrectAssignment = false;

        public IntegerAsset(Asset asset)
            : base(asset)
        {
            if (Asset.ValueType == AssetValueType.Text)
            {
                m_Value = Convert.ToInt32(Asset.StringValue);
                m_CorrectAssignment = true;
            }
            else if (Asset.ValueType == AssetValueType.Integer)
            {
                m_Value = Asset.IntValue;
                m_CorrectAssignment = true;
            }
            else if (Asset.ValueType == AssetValueType.Bool)
            {
                m_Value = Asset.BoolValue ? 1 : 0;
                m_CorrectAssignment = true;
            }
            else
            {
                m_Value = -1;
            }
        }

        public override string StringValue { get { return Value.ToString(); } }

        public override bool CorrectAssignment { get { return m_CorrectAssignment; } }
    }

    public class BoolAsset : ConcreteAsset
    {
        private bool m_Value;

        public bool Value
        {
            get
            {
                if (!m_CorrectAssignment)
                {
                    throw new Exception("Incorrect assignment of value");
                }
                return m_Value;
            }
        }

        private bool m_CorrectAssignment = false;

        public BoolAsset(Asset asset)
            : base(asset)
        {
            if (Asset.ValueType == AssetValueType.Text)
            {
                try
                {
                    m_Value = Convert.ToBoolean(Asset.StringValue);
                    m_CorrectAssignment = true;
                }
                catch (Exception) { }
            }
            else if (Asset.ValueType == AssetValueType.Integer)
            {
                if (Asset.IntValue == 0)
                {
                    m_Value = false;
                    m_CorrectAssignment = true;
                }
                if (Asset.IntValue == 0)
                {
                    m_Value = true;
                    m_CorrectAssignment = true;
                }
            }
            else if (Asset.ValueType == AssetValueType.Bool)
            {
                m_Value = Asset.BoolValue;
                m_CorrectAssignment = true;
            }
        }

        public override string StringValue { get { return Value.ToString(); } }

        public override bool CorrectAssignment { get { return m_CorrectAssignment; } }
    }

    public class CredentialAsset : ConcreteAsset
    {
        private string m_Value;

        public string Value
        {
            get
            {
                if (!m_CorrectAssignment)
                {
                    throw new Exception("Incorrect assignment of value");
                }
                return m_Value;
            }
        }

        private bool m_CorrectAssignment = false;

        public CredentialAsset(Asset asset)
            : base(asset)
        {
            if (Asset.ValueType == AssetValueType.Text)
            {
                m_Value = Asset.StringValue;
                m_CorrectAssignment = true;
            }
        }

        public override string StringValue { get { return Value.ToString(); } }

        public override bool CorrectAssignment { get { return m_CorrectAssignment; } }
    }
}
