SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		James
-- Create date: 19/02/2026
-- Description:	delete all messages a user has sent to a room
-- =============================================
CREATE PROCEDURE dbo.deleteUserMessagesFromRoom
	-- Add the parameters for the stored procedure here
	@roomid INT = NULL,
	@userid INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM dbo.messages
	WHERE roomid = @roomid AND senderid = @userid;
END
GO
