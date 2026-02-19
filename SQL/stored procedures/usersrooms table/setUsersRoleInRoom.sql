SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		James
-- Create date: 19/02/2026
-- Description:	set all users from a room to the same given role
-- =============================================
CREATE PROCEDURE dbo.setUsersRoleInRoom
	-- Add the parameters for the stored procedure here
	@roomid INT = NULL,
	@roleinroom NVARCHAR(50) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE dbo.usersrooms
	SET roleinroom = @roleinroom
	WHERE roomid = @roomid;
END
GO
