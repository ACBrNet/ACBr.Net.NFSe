using System;
using ACBr.Net.Core.Extensions;

namespace ACBr.Net.NFSe.Demo
{
    public sealed class ItemData<T> where T : struct
    {
        #region Constructors

        public ItemData()
        {
        }

        public ItemData(string description, T content)
        {
            Description = description;
            Content = content;
        }

        public ItemData(T value)
        {
            Content = value;

            if (value is Enum)
            {
                Description = value.GetDescription();
            }
        }

        #endregion Constructors

        #region Properties

        public string Description { get; set; }

        public T Content { get; set; }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return Description;
        }

        #endregion Methods
    }
}