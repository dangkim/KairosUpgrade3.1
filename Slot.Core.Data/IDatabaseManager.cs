namespace Slot.Core.Data
{
    public interface IDatabaseManager
    {
        /// <summary>
        /// Get default writable database
        /// </summary>
        /// <returns></returns>
        IWritableDatabase GetWritableDatabase();
        /// <summary>
        /// Get writable database by operator
        /// </summary>
        /// <param name="operatorId"></param>
        /// <returns></returns>
        IWritableDatabase GetWritableDatabase(string operatorId);
        /// <summary>
        /// Get default readonly database
        /// </summary>
        /// <returns></returns>
        IReadOnlyDatabase GetReadOnlyDatabase();
        /// <summary>
        /// Get readonly database by operator
        /// </summary>
        /// <param name="operatorId"></param>
        /// <returns></returns>
        IReadOnlyDatabase GetReadOnlyDatabase(string operatorId);
        /// <summary>
        /// Get writable database by Operator ID
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IWritableDatabase GetWritableDatabase(int? operatorId);
        /// <summary>
        /// Get readonly database by Operator ID
        /// </summary>
        /// <param name="operatorId"></param>
        /// <returns></returns>
        IReadOnlyDatabase GetReadOnlyDatabase(int? operatorId);
    }
}
