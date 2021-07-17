using System;


namespace DataComparisonTool.Model
{
    public class DynamicProperty
    {
        /// <summary>
        /// The Name of the property.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// The Display Name of the property for the end-user.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The Name of the underlying System Type of the property.
        /// </summary>
        public string SystemTypeName { get; set; }

        /// <summary>
        /// The underlying System Type of the property.
        /// </summary>
        public Type SystemType => Type.GetType(SystemTypeName);
    }
}
