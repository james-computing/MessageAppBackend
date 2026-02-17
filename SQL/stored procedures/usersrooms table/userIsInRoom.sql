SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		James
-- Create date: 17/02/2026
-- Description:	check if user is in room
-- =============================================
CREATE PROCEDURE dbo.userIsInRoom
	-- Add the parameters for the stored procedure here
	@roomid INT = NULL,
	@userid INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT CASE WHEN EXISTS (
		SELECT 1 FROM dbo.usersrooms
			WHERE roomid = @roomid AND userid = @userid)
	THEN CAST(1 AS BIT)
	ELSE CAST(0 AS BIT)
	END
END
GO
