namespace Dynamitey
{
    /// <summary>
    /// Get access to target of original proxy
    /// </summary>
    public interface IForwarder
    {
        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        object Target { get; }
    }
}