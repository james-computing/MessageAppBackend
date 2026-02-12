SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		James
-- Create date: 12/02/2026
-- Description:	remove user from room
-- =============================================
CREATE PROCEDURE dbo.removeUserFromRoom
	-- Add the parameters for the stored procedure here
	@roomid INT = NULL,
	@userid INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM dbo.usersrooms
	WHERE roomid = @roomid AND userid = @userid;
END
GO
