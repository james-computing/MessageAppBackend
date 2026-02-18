namespace MessageRealTime.Data
{
    public interface IDataAccess
    {
        public Task<IEnumerable<int>> GetRoomsIdsAsync(int userId);
        public Task<int> GetUserIdAsync(string userEmail);
        public Task<int> SaveMessageAsync(int roomId, int senderId, string content, DateTime time);
    }
}
