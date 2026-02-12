SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		James
-- Create date: 12/02/2026
-- Description:	add user to a room
-- =============================================
CREATE PROCEDURE dbo.addUserToRoom
	-- Add the parameters for the stored procedure here
	@roomid INT = NULL,
	@userId INT = NULL,
	@roleinroom NVARCHAR(50) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.usersrooms (roomid, userid, roleinroom)
		VALUES(@roomid, @userid, @roleinroom);
END
GO
