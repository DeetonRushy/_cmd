

namespace cmd
{
    /// <summary>
    /// Return Types, used in all Cmd internal API functions.
    /// </summary>
    public enum RetType
    {
        /// <summary>
        /// Return if the operation completed successfully.
        /// </summary>
        _C_SUCCESS,
        /// <summary>
        /// Return if the operation failed.
        /// </summary>
        _C_FAILURE,
        /// <summary>
        /// Return if there was an IOError.
        /// </summary>
        _C_IOERROR,
        /// <summary>
        /// Return if something that is a NullReference is passed, read, writted to etc.
        /// </summary>
        _C_ACCESSVIOLATION,
        /// <summary>
        /// Return if something unknown happened, something that is impossible.
        /// </summary>
        _C_UNKNOWN_ERROR,
        /// <summary>
        /// Return if something system related failed, such as anything in System.IO.Diagnostics.
        /// </summary>
        _C_SYSTEM_ERROR,
        /// <summary>
        /// Return if the command/CVar is currently disabled.
        /// </summary>
        _C_DISABLED,
        /// <summary>
        /// Return if some sort of file resource doesn't exist that is essential for operating.
        /// </summary>
        _C_RESOURCE_NOT_EXIST,
        /// <summary>
        /// Dummy return type for CVars, read more at <c>cmd.CVarHost.ModifyCVarForce()</c>
        /// </summary>
        _C_DUMMY_VL,
        /// <summary>
        /// Return if parameters passed are incorrect.
        /// </summary>
        _C_INVALID_PARAMS
    }

}
