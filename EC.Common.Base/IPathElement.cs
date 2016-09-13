
namespace EC.Common.Base
{

    /// <summary>
    /// Implement this interface on the path elements if you want to support the extension ToPath().
    /// </summary>
    /// <remarks>
    /// A path element should implement this interface if it's part of a path (i.e. /Root/ITB).
    /// </remarks>
    
    public interface IPathElement
    {
        /// <summary>
        /// Path element name.
        /// </summary>

        string PathElementName { get; }
    }
}
