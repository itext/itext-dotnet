namespace iText.Commons.Json {
    /// <summary>Interface which marks classes serializable to JSON AST.</summary>
    public interface IJsonSerializable {
        /// <summary>Serializes object to JSON AST.</summary>
        /// <returns>
        /// 
        /// <see cref="JsonValue"/>
        /// serialized object
        /// </returns>
        JsonValue ToJson();
    }
}
