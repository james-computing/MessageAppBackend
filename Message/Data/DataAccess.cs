namespace Message.Data
{
    public class DataAccess : IDataAccess
    {
        public async Task<IEnumerable<string>> GetRoomIds(string userId)
        {
            IEnumerable<string> roomIds = ["groupName"];
            return roomIds;
        }
    }
}
