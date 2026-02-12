using Dapper;
using System.Data;

namespace KafkaConsumer.Data
{
    public class DataAccess(IDbConnection dbConnection) : IDataAccess
    {
        private const string ROOMID_VARIABLE = "roomid";
        private const string SENDERID_VARIABLE = "senderid";
        private const string MESSAGE_VARIABLE = "message";
        private const string TIME_VARIABLE = "time";

        private const string SAVE_MESSAGE_PROCEDURE = "dbo.saveMessage";

        public async Task SaveMessage(int senderId, int roomId, string message, DateTime time)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(ROOMID_VARIABLE, roomId);
            parameters.Add(SENDERID_VARIABLE, senderId);
            parameters.Add(MESSAGE_VARIABLE, message);
            parameters.Add(TIME_VARIABLE, time);

            await dbConnection.ExecuteAsync
            (
                SAVE_MESSAGE_PROCEDURE,
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
