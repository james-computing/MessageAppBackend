SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		James
-- Create date: 18/02/2026
-- Description:	get the ids of the users from a room
-- =============================================
CREATE PROCEDURE dbo.getUsersIdsFromRoom
	-- Add the parameters for the stored procedure here
	@roomid INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT userid
	FROM dbo.usersrooms
	WHERE roomid = @roomid;
END
GO
