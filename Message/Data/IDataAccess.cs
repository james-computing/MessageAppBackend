namespace Message.Data
{
    public interface IDataAccess
    {
        public Task<IEnumerable<int>> GetRoomIdsAsync(int userId);
    }
}
