SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		James
-- Create date: 19/02/2026
-- Description:	get user info from room. It returns the ids and usernames.
-- =============================================
CREATE PROCEDURE dbo.getUsersInfoFromRoom
	-- Add the parameters for the stored procedure here
	@roomid INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT dbo.usersrooms.userid AS 'id',
			dbo.users.username AS 'username',
			dbo.usersrooms.roleinroom AS 'roleinroom'
	FROM dbo.usersrooms INNER JOIN dbo.users ON (dbo.usersrooms.userid = dbo.users.id)
	WHERE dbo.usersrooms.roomid = @roomid;
END
GO
