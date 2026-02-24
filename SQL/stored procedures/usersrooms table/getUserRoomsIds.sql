SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		James
-- Create date: 24/02/2026
-- Description:	get the ids of the rooms the user is in
-- =============================================
CREATE PROCEDURE dbo.getUserRoomsIds
	-- Add the parameters for the stored procedure here
	@userid INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT roomid
	FROM dbo.usersrooms
	WHERE userid = @userid;
END
GO
