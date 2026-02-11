namespace Message.Data
{
    public interface IDataAccess
    {
        public Task<IEnumerable<string>> GetRoomIds(string userId);
    }
}
