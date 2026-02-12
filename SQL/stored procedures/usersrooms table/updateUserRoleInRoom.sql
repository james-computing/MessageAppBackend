SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		James
-- Create date: 12/02/2026
-- Description:	update the role of the user in the room
-- =============================================
CREATE PROCEDURE dbo.updateUserRoleInRoom
	-- Add the parameters for the stored procedure here
	@roomid INT = NULL,
	@userid INT = NULL,
	@roleinroom NVARCHAR(50) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE dbo.usersrooms
	SET roleinroom = @roleinroom
	WHERE roomid = @roomid AND userid = @userid;
END
GO
